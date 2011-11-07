using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class SetRangeTests : BaseCommandTests
   {
      [Test]
      public void SetsTheRange()
      {
         Redis.Set("key1", "it's over 9");
         Assert.AreEqual(14, Redis.SetRange("key1", 10, "9000"));
         Assert.AreEqual("it's over 9000", Redis.Get<string>("key1"));
      }
   }
}