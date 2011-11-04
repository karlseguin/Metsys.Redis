using System;

namespace Metsys.Redis
{
   public interface IRedis : IDisposable
   {
      long Incr(string key);
      long IncrBy(string key, int value);
      long Del(string key);
   }
}