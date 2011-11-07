using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class ExistsTests : BaseCommandTests
   {
      [Test]
      public void ReturnsTrueIfTheKeyExists()
      {
         Redis.Incr("key:1");
         Assert.AreEqual(true, Redis.Exists("key:1"));
      }

      [Test]
      public void ReturnsFalseIfTheKeyDoesNotExists()
      {
         Assert.AreEqual(false, Redis.Exists("key:2"));
      }
   }
}