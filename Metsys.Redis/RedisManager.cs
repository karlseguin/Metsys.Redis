using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Metsys.Redis
{
   public interface IRedisManager
   {
      IRedis Redis();
   }

   public class RedisManager : IRedisManager
   {
      private readonly ConcurrentQueue<IRedis> _redisPool;
      private readonly Configuration _configuration = new Configuration();
      private readonly AutoResetEvent _notifier = new AutoResetEvent(false);

      public Configuration Configuration
      {
         get { return _configuration; }
      }

      public static IRedisManager Configure(Action<IConfiguration> action)
      {
         var manager = new RedisManager();
         action(manager._configuration);
         return manager;
      }

      private RedisManager()
      {
         _redisPool = new ConcurrentQueue<IRedis>();
         for(var i = 0; i < 25; ++i)
         {
            _redisPool.Enqueue(new Redis(this));
         }
      }

      public IRedis Redis()
      {
         IRedis redis;
         if (_redisPool.TryDequeue(out redis))
         {
            return redis;
         }
         if (!_notifier.WaitOne(10000))
         {
            throw new RedisException("Timed out waiting for a connection from the pool");
         }
         return Redis();
      }

      public Connection GetConnection()
      {
         return new Connection(_configuration.Server);
      }

      public void CheckIn(Redis redis)
      {
         _redisPool.Enqueue(redis);
         _notifier.Set();
      }
   }
}