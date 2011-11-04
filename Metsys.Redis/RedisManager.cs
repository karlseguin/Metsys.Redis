using System;

namespace Metsys.Redis
{
   public interface IRedisManager
   {
      IRedis Redis();
   }

   public class RedisManager : IRedisManager
   {
      private ConnectionPool _connectionPool;
      private Pool<Redis> _redisPool;
      private readonly Configuration _configuration = new Configuration();

      public static IRedisManager Configure(Action<IConfiguration> action)
      {
         var manager = new RedisManager();
         action(manager._configuration);
         manager._connectionPool = new ConnectionPool(manager._configuration.Server);
         manager._redisPool = new Pool<Redis>(25, p => new Redis(manager));
         return manager;
      }

      public IRedis Redis()
      {
         return _redisPool.CheckOut();
      }

      public IConnection GetConnection()
      {
         return _connectionPool.CheckOut();
      }

      public void CheckIn(Redis redis)
      {
         var connection = redis.Connection;
         if (connection != null) {_connectionPool.CheckIn(connection);}
         _redisPool.CheckIn(redis);
      }
   }
}