using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class DecrTests : BaseCommandTests
   {
      [Test]
      public void DecrementsAValue()
      {
         Assert.AreEqual(-1, Redis.Decr("key1"));
         Assert.AreEqual(-11, Redis.DecrBy("key1", 10));
         Assert.AreEqual(-11, Redis.Get<int>("key1"));
      }
   }
}