using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class TypeTests : BaseCommandTests
   {
      [Test]
      public void ReturnsTheKeyType()
      {
         Assert.AreEqual("none", Redis.Type("invalid"));
         Redis.Incr("key:1");
         Assert.AreEqual("string", Redis.Type("key:1"));
         //todo add more types once you have them
      }
   }
}