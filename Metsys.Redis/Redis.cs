using System;
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

      public int DbSize()
      {
         return (int)Send(Writer.Serialize(Commands.DbSize, _dynamicBuffer), Reader.Integer);
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

      public bool Move(string key, int database)
      {
         return Send(Writer.Serialize(Commands.Move, _dynamicBuffer, key, database), Reader.Bool);
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