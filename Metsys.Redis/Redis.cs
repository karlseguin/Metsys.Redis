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

      
      public T Get<T>(string key)
      {
         var length = Send(Writer.Serialize(Commands.Get, _dynamicBuffer, key), Reader.Bulk);
         return length == 0 ? default(T) : Serializer.Deserialize<T>(_dynamicBuffer);
      }

      public long Incr(string key)
      {
         return Send(Writer.Serialize(Commands.Incr, _dynamicBuffer, key), Reader.Integer);
      }

      public long IncrBy(string key, int value)
      {
         return Send(Writer.Serialize(Commands.IncrBy, _dynamicBuffer, key, value.ToString()), Reader.Integer);
      }

      public long Del(params string[] key)
      {
         return Send(Writer.Serialize(Commands.Del, _dynamicBuffer, key), Reader.Integer);
      }

      public void FlushDb()
      {
         Send(Writer.Serialize(Commands.FlushDb, _dynamicBuffer), Reader.Status);
      }

      public void Select(int database)
      {
         Select(database, true);
      }

      private void Select(int database, bool flagAsDifferent)
      {
         if (flagAsDifferent) { _selectedADatabase = true; }
         Send(Writer.Serialize(Commands.Select, _dynamicBuffer, database.ToString()), Reader.Status);
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