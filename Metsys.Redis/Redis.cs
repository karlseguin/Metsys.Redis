using System;
using System.IO;

namespace Metsys.Redis
{
   public class Redis : IRedis
   {
      private readonly RedisManager _manager;
      private readonly Configuration _configuration;
      private IConnection _connection;
      private bool _selectedADatabase;
      private bool _error;

      public IConnection Connection
      {
         get { return _connection; }
      }

      internal Redis(RedisManager manager)
      {
         _manager = manager;
         _configuration = manager.Configuration;
      }

      private static readonly byte[] _getCommand = Encoding.GetBytes("GET");
      public T Get<T>(string key)
      {
         var data = Send(Writer.Serialize(_getCommand, key), Reader.Bulk);
         return data == null ? default(T) : Serializer.Deserialize<T>(data);
      }

      private static readonly byte[] _incrCommand = Encoding.GetBytes("INCR");
      public long Incr(string key)
      {
         return Send(Writer.Serialize(_incrCommand, key), Reader.Integer);
      }

      private static readonly byte[] _incrByCommand = Encoding.GetBytes("INCRBY");
      public long IncrBy(string key, int value)
      {
         return Send(Writer.Serialize(_incrByCommand, key, value.ToString()), Reader.Integer);
      }

      private static readonly byte[] _delCommand = Encoding.GetBytes("DEL");
      public long Del(params string[] key)
      {
         return Send(Writer.Serialize(_delCommand, key), Reader.Integer);
      }

      private static readonly byte[] _flushDbCommand = Encoding.GetBytes("FLUSHDB");
      public void FlushDb()
      {
         Send(Writer.Serialize(_flushDbCommand), Reader.Status);
      }

      
      public void Select(int database)
      {
         Select(database, true);
      }

      private static readonly byte[] _selectCommand = Encoding.GetBytes("SELECT");
      private void Select(int database, bool flagAsDifferent)
      {
         if (flagAsDifferent) { _selectedADatabase = true; }
         Send(Writer.Serialize(_selectCommand, database.ToString()), Reader.Status);
      }

      private T Send<T>(WriteContext context, Func<Stream, T> callback)
      {
         if (_connection == null)
         {
            _error = false;
            if (_manager.GetConnection(out _connection) && _configuration.Database != 0)
            {
               Select(_configuration.Database, false);
            }
         }
         using (context)
         {
            try
            {
               Connection.Send(context.Buffer, context.Length);
               return callback(Connection.GetStream());
            }
            catch
            {
               _error = true;
               throw;
            }
         }
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
            if (!_error && _selectedADatabase)
            {
               Select(_configuration.Database, false);
            }
            _manager.CheckIn(this, _error);
            _error = false;
            _connection = null;
         }
      }

      ~Redis()
      {
         Dispose(false);
      }

   }
}