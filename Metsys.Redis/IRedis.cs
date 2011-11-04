using System;

namespace Metsys.Redis
{
   public interface IRedis : IDisposable
   {
      T Get<T>(string key);
      long Incr(string key);
      long IncrBy(string key, int value);
      long Del(params string[] key);
      void FlushDb();
      void Select(int database);
   }
}