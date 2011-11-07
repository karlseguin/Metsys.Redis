using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class GetRangeTests : BaseCommandTests
   {
      [Test]
      public void ReturnsNullIfTheKeyDoesntExist()
      {
         Assert.AreEqual(string.Empty, Redis.GetRange("key1", 0, 100));
      }

      [Test]
      public void ReturnsTheRange()
      {
         Redis.Set("key1", "fear is the mind killer");
         Assert.AreEqual("ar i", Redis.GetRange("key1", 2, 5));
      }
   }
}