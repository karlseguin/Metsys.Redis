namespace Metsys.Redis
{
   public class Commands
   {
      public static readonly byte[] Append = Helper.GetBytes("APPEND");
      public static readonly byte[] DbSize = Helper.GetBytes("DBSIZE");
      public static readonly byte[] Decr = Helper.GetBytes("DECR");
      public static readonly byte[] DecrBy = Helper.GetBytes("DECRBY");
      public static readonly byte[] Del = Helper.GetBytes("DEL");
      public static readonly byte[] Exists = Helper.GetBytes("EXISTS");
      public static readonly byte[] Expire = Helper.GetBytes("EXPIRE");
      public static readonly byte[] ExpireAt = Helper.GetBytes("EXPIREAT");
      public static readonly byte[] FlushDb = Helper.GetBytes("FLUSHDB");
      public static readonly byte[] Get = Helper.GetBytes("GET");
      public static readonly byte[] GetBit = Helper.GetBytes("GETBIT");
      public static readonly byte[] GetRange = Helper.GetBytes("GETRANGE");
      public static readonly byte[] GetSet = Helper.GetBytes("GetSet");
      public static readonly byte[] Incr = Helper.GetBytes("INCR");
      public static readonly byte[] IncrBy = Helper.GetBytes("INCRBY");
      public static readonly byte[] Keys = Helper.GetBytes("KEYS");
      public static readonly byte[] Move = Helper.GetBytes("MOVE");
      public static readonly byte[] Persist = Helper.GetBytes("PERSIST");
      public static readonly byte[] RandomKey = Helper.GetBytes("RANDOMKEY");
      public static readonly byte[] Rename = Helper.GetBytes("RENAME");
      public static readonly byte[] RenameNx = Helper.GetBytes("RENAMENX");
      public static readonly byte[] Select = Helper.GetBytes("SELECT");
      public static readonly byte[] Set = Helper.GetBytes("SET");
      public static readonly byte[] SetBit = Helper.GetBytes("SETBIT");
      public static readonly byte[] Ttl = Helper.GetBytes("TTL");
      public static readonly byte[] Type = Helper.GetBytes("TYPE");
   }
}