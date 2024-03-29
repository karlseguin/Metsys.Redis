using System;
using System.Collections.Generic;
using System.IO;

namespace Metsys.Redis
{
   public class Redis : IRedis
   {
      private readonly RedisManager _manager;
      private readonly Configuration _configuration;
      private DynamicBuffer _dynamicBuffer;
      private IConnection _connection;
      private bool _selectedADatabase;

      internal Redis(RedisManager manager)
      {
         _manager = manager;
         _configuration = manager.Configuration;
         _dynamicBuffer = new DynamicBuffer();
      }

      public int Append(string key, string value)
      {
         return (int)Send(Writer.Serialize(Commands.Append, _dynamicBuffer, key, value), Reader.Integer);
      }

      public int DbSize()
      {
         return (int)Send(Writer.Serialize(Commands.DbSize, _dynamicBuffer), Reader.Integer);
      }

      public long Decr(string key)
      {
         return Send(Writer.Serialize(Commands.Decr, _dynamicBuffer, key), Reader.Integer);
      }

      public long DecrBy(string key, int value)
      {
         return Send(Writer.Serialize(Commands.DecrBy, _dynamicBuffer, key, value), Reader.Integer);
      }

      public long Del(params string[] key)
      {
         return Send(Writer.Serialize(Commands.Del, _dynamicBuffer, key), Reader.Integer);
      }

      public bool Exists(string key)
      {
         return Send(Writer.Serialize(Commands.Exists, _dynamicBuffer, key), Reader.Bool);
      }

      public bool Expire(string key, int seconds)
      {
         return Send(Writer.Serialize(Commands.Expire, _dynamicBuffer, key, seconds), Reader.Bool);
      }

      public bool ExpireAt(string key, DateTime date)
      {
         return Send(Writer.Serialize(Commands.ExpireAt, _dynamicBuffer, key, date), Reader.Bool);
      }

      public void FlushDb()
      {
         Send(Writer.Serialize(Commands.FlushDb, _dynamicBuffer), Reader.Status);
      }

      public T Get<T>(string key)
      {
         return Send(Writer.Serialize(Commands.Get, _dynamicBuffer, key), Reader.Bulk<T>);
      }

      public bool GetBit(string key, int offset)
      {
         return Send(Writer.Serialize(Commands.GetBit, _dynamicBuffer, key, offset), Reader.Bool);
      }

      public string GetRange(string key, int start, int end)
      {
         return Send(Writer.Serialize(Commands.GetRange, _dynamicBuffer, key, start, end), Reader.Bulk<string>);
      }

      public T GetSet<T>(string key, object value)
      {
         return Send(Writer.Serialize(Commands.GetSet, _dynamicBuffer, key, value), Reader.Bulk<T>);
      }

      public long Incr(string key)
      {
         return Send(Writer.Serialize(Commands.Incr, _dynamicBuffer, key), Reader.Integer);
      }

      public long IncrBy(string key, int value)
      {
         return Send(Writer.Serialize(Commands.IncrBy, _dynamicBuffer, key, value.ToString()), Reader.Integer);
      }

      public string[] Keys(string pattern)
      {
         return Send(Writer.Serialize(Commands.Keys, _dynamicBuffer, pattern), Reader.MultiBulk<string>);
      }

      public T[] MGet<T>(params string[] keys)
      {
         return Send(Writer.Serialize(Commands.MGet, _dynamicBuffer, keys), Reader.MultiBulk<T>);
      }

      public bool Move(string key, int database)
      {
         return Send(Writer.Serialize(Commands.Move, _dynamicBuffer, key, database), Reader.Bool);
      }

      public void MSet(ICollection<KeyValuePair<string, object>> keyAndValues)
      {
         Send(Writer.Serialize(Commands.MSet, _dynamicBuffer, DictionaryToValueArray(keyAndValues)), Reader.Status);
      }

      public bool MSetNx(ICollection<KeyValuePair<string, object>> keyAndValues)
      {
         return Send(Writer.Serialize(Commands.MSetNx, _dynamicBuffer, DictionaryToValueArray(keyAndValues)), Reader.Bool);
      }

      private static object[] DictionaryToValueArray(ICollection<KeyValuePair<string, object>> keyAndValues)
      {
         var values = new object[keyAndValues.Count * 2];
         var i = 0;
         foreach (var kvp in keyAndValues)
         {
            values[i++] = kvp.Key;
            values[i++] = kvp.Value;
         }
         return values;
      }


      public bool Persist(string key)
      {
         return Send(Writer.Serialize(Commands.Persist, _dynamicBuffer, key), Reader.Bool);
      }

      public string RandomKey()
      {
         return Send(Writer.Serialize(Commands.RandomKey, _dynamicBuffer), Reader.Bulk<string>);
      }

      public void Rename(string key, string newName)
      {
         Send(Writer.Serialize(Commands.Rename, _dynamicBuffer, key, newName), Reader.Status);
      }

      public bool RenameNx(string key, string newName)
      {
         return Send(Writer.Serialize(Commands.RenameNx, _dynamicBuffer, key, newName), Reader.Bool);
      }

      public void Select(int database)
      {
         Select(database, true);
      }

      public void Set(string key, object value)
      {
         Send(Writer.Serialize(Commands.Set, _dynamicBuffer, key, value), Reader.Status);
      }

      public bool SetBit(string key, int offset, bool bit)
      {
         return Send(Writer.Serialize(Commands.SetBit, _dynamicBuffer, key, offset, bit ? 1 : 0), Reader.Bool);
      }

      public void SetEx(string key, int seconds, object value)
      {
         Send(Writer.Serialize(Commands.SetEx, _dynamicBuffer, key, seconds, value), Reader.Status);
      }

      public bool SetNx(string key, object value)
      {
         return Send(Writer.Serialize(Commands.SetNx, _dynamicBuffer, key, value), Reader.Bool);
      }

      public int SetRange(string key, int offset, object value)
      {
         return (int) Send(Writer.Serialize(Commands.SetRange, _dynamicBuffer, key, offset, value), Reader.Integer);
      }

      public int StrLen(string key)
      {
         return (int) Send(Writer.Serialize(Commands.StrLen, _dynamicBuffer, key), Reader.Integer);
      }

      public long Ttl(string key)
      {
         return Send(Writer.Serialize(Commands.Ttl, _dynamicBuffer, key), Reader.Integer);
      }

      public string Type(string key)
      {
         return Send(Writer.Serialize(Commands.Type, _dynamicBuffer, key), Reader.String);
      }

      private void Select(int database, bool flagAsDifferent)
      {
         if (flagAsDifferent) { _selectedADatabase = true; }
         Send(Writer.Serialize(Commands.Select, _dynamicBuffer, database), Reader.Status);
      }

      private T Send<T>(DynamicBuffer context, Func<Stream, DynamicBuffer, T> callback)
      {
         EnsureConnection();
         try
         {
            _connection.Send(context.Buffer, context.Length);
            return callback(_connection.GetStream(), _dynamicBuffer);
         }
         catch
         {
            KillConnection();
            throw;
         }
      }

      private void EnsureConnection()
      {
         if (_connection != null && !_connection.IsAlive())
         {
            KillConnection();
         }
         if (_connection == null)
         {
            _connection = _manager.GetConnection();
            if (_configuration.Database != 0)
            {
               var realBuffer = _dynamicBuffer;
               _dynamicBuffer = new DynamicBuffer();
               Select(_configuration.Database, false);
               _dynamicBuffer = realBuffer;
            }
         }
      }

      private void KillConnection()
      {
         _connection.Dispose();
         _connection = null;
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }
      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            if (_connection != null && _selectedADatabase)
            {
               Select(_configuration.Database, false);
            }
            _manager.CheckIn(this);
         }
      }

      ~Redis()
      {
         Dispose(false);
      }

   }
}