namespace Metsys.Redis
{
   public class Commands
   {
      public static readonly byte[] DbSize = Encoding.GetBytes("DBSIZE");
      public static readonly byte[] Del = Encoding.GetBytes("DEL");
      public static readonly byte[] FlushDb = Encoding.GetBytes("FLUSHDB");
      public static readonly byte[] Get = Encoding.GetBytes("GET");
      public static readonly byte[] Incr = Encoding.GetBytes("INCR");
      public static readonly byte[] IncrBy = Encoding.GetBytes("INCRBY");
      public static readonly byte[] Keys = Encoding.GetBytes("KEYS");
      public static readonly byte[] Select = Encoding.GetBytes("SELECT");
   }
}