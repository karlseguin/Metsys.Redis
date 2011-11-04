using System;

namespace Metsys.Redis
{
   public class RedisException : Exception
   {
      public RedisException(string message) : base(message) {}
   }
}