using System;

namespace Metsys.Redis
{
   public interface IRedis : IDisposable
   {
      int DbSize();
      long Del(params string[] key);
      void FlushDb();
      T Get<T>(string key);
      long Incr(string key);
      long IncrBy(string key, int value);
      string[] Keys(string pattern = "*");
      void Select(int database);
   }
}