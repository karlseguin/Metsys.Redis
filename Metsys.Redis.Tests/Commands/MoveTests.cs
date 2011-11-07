using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class MoveTests : BaseCommandTests
   {
      [Test]
      public void ReturnsFalseIfTheKeyDoesntExist()
      {
         Assert.AreEqual(false, Redis.Move("invalid", Database + 1));
      }

      [Test]
      public void MovesTheKey()
      {
         Redis.Incr("key:1");
         Assert.AreEqual(true, Redis.Move("key:1", Database + 1));
         Redis.Select(Database + 1);
         Assert.AreEqual(true, Redis.Exists("key:1"));
      }
   }
}