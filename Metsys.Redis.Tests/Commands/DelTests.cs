using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class DelTests : BaseCommandTests
   {
      [Test]
      public void DoesNothingIfTheKeyDoesntExist()
      {
         Assert.AreEqual(0, Redis.Del("invalid"));
      }

      [Test]
      public void DeletesAKey()
      {
         Redis.Incr("key1");
         Assert.AreEqual(1, Redis.Del("key1"));
      }

      [Test]
      public void DeletesMultipleKeys()
      {
         Redis.Incr("key1");
         Redis.Incr("key2");
         Redis.Incr("key3");
         Assert.AreEqual(2, Redis.Del("key1", "key2", "key4"));
      }
   }
}