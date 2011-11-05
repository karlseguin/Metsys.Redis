using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class FlushDbTests  : BaseCommandTests
   {
      [Test]
      public void ErasesTheDatabase()
      {
         Redis.Incr("key1");
         Redis.Select(Database + 1);
         Redis.Incr("key2");
         Redis.Incr("key3");
         Redis.FlushDb();
         Assert.AreEqual(0, Redis.DbSize());
         Redis.Select(Database);
         Assert.AreEqual(1, Redis.DbSize());
      }
   }
}