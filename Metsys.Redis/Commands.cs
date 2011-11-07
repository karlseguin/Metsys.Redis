namespace Metsys.Redis
{
   public class Commands
   {
      public static readonly byte[] DbSize = Helper.GetBytes("DBSIZE");
      public static readonly byte[] Del = Helper.GetBytes("DEL");
      public static readonly byte[] FlushDb = Helper.GetBytes("FLUSHDB");
      public static readonly byte[] Get = Helper.GetBytes("GET");
      public static readonly byte[] Incr = Helper.GetBytes("INCR");
      public static readonly byte[] IncrBy = Helper.GetBytes("INCRBY");
      public static readonly byte[] Keys = Helper.GetBytes("KEYS");
      public static readonly byte[] Select = Helper.GetBytes("SELECT");
      public static readonly byte[] Set = Helper.GetBytes("SET");
   }
}