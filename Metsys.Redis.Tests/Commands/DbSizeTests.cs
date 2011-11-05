using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class DbSizeTests : BaseCommandTests
   {
      [Test]
      public void ReturnsTheNumberOfKeysInTheDatabase()
      {
         Assert.AreEqual(0, Redis.DbSize());
         Redis.Incr("key1");
         Assert.AreEqual(1, Redis.DbSize());
         Redis.Incr("key2");
         Assert.AreEqual(2, Redis.DbSize());
      }
   }
}