using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class RandomKeyTests : BaseCommandTests
   {
      [Test]
      public void ReturnsNullIfThereAreNoKeys()
      {
         Assert.AreEqual(null, Redis.RandomKey());
      }

      [Test]
      public void ReturnsARandomKey()
      {
         Redis.Incr("key1");
         Redis.Incr("key2");
         var key = Redis.RandomKey();
         Assert.IsTrue(key == "key1" || key == "key2");
      }
   }
}