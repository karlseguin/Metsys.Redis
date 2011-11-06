using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class SelectTests : BaseCommandTests
   {
      [Test]
      public void ChangesDatabase()
      {
         Redis.Incr("key1");
         Redis.Select(Database + 1);
         Assert.AreEqual(null, Redis.Get<int?>("key1"));
         Redis.Select(Database);
         Assert.AreEqual(1, Redis.Get<int?>("key1"));
      }

      [Test]
      public void ResetsTheDatabaseAfterBeingDisposed()
      {
         Redis.Incr("key1");
         Redis.Select(Database + 1);
         Redis.Dispose();
         Assert.AreEqual(1, Redis.Get<int?>("key1"));
      }
   }
}