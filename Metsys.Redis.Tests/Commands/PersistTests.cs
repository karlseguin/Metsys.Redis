using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class PersistTests : BaseCommandTests
   {
      [Test]
      public void DoesNotPersistANonExistingKey()
      {
         Assert.AreEqual(false, Redis.Persist("invalid"));
      }

      [Test]
      public void DoesNotPersistANonExpiringKey()
      {
         Redis.Incr("key:1");
         Assert.AreEqual(false, Redis.Persist("key:1"));
      }

      [Test]
      public void PersistAKey()
      {
         Redis.Incr("key:1");
         Redis.Expire("key:1", 20000);
         Assert.AreEqual(true, Redis.Persist("key:1"));
         Assert.AreEqual(-1, Redis.Ttl("key:1"));
      }
   }
}