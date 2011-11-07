using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class GetAndSetBitTests : BaseCommandTests
   {
      [Test]
      public void ReturnsZeroIfTheKeyDoesntExist()
      {
         Assert.AreEqual(false, Redis.GetBit("invalid", 32));
      }
      [Test]
      public void ReturnsTheBit()
      {
         Assert.AreEqual(false, Redis.SetBit("key1", 0, true));
         Assert.AreEqual(false, Redis.SetBit("key1", 1, false));
         
         Assert.AreEqual(true, Redis.GetBit("key1", 0));
         Assert.AreEqual(false, Redis.GetBit("key1", 1));
         Assert.AreEqual(false, Redis.GetBit("key1", 2));

         Assert.AreEqual(true, Redis.SetBit("key1", 0, false));
      }
   }
}