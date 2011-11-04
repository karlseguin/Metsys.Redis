using System;
using System.IO;

namespace Metsys.Redis
{
   public class Redis : IRedis
   {
      private readonly RedisManager _manager;

      public IConnection Connection { get; private set; }

      internal Redis(RedisManager manager)
      {
         _manager = manager;
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
      public long Del(string key)
      {
         return Send(Writer.Serialize(_delCommand, key), Reader.Integer);
      }

      public T Send<T>(WriteContext context, Func<Stream, T> callback)
      {
         if (Connection == null)
         {
            Connection = _manager.GetConnection();
         }
         using (context)
         {
            Connection.Send(context.Buffer, context.Length);
            return callback(Connection.GetStream());
         }
      }

      public void Dispose()
      {
         _manager.CheckIn(this);
         Connection = null;
      }
   }
}