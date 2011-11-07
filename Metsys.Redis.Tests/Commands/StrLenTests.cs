using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class StrLenTests : BaseCommandTests
   {
      [Test]
      public void ReturnsTheLenghtOfTheValue()
      {
         Assert.AreEqual(0, Redis.StrLen("invalid"));
         Redis.Set("key1", "this is boring");
         Assert.AreEqual(14, Redis.StrLen("key1"));
      }
   }
}