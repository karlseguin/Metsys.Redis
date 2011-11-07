using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class RenameTests : BaseCommandTests
   {
      [Test]
      public void RenamesTheKey()
      {
         Redis.Incr("key:1");
         Redis.Rename("key:1", "key:2");
         Assert.AreEqual(false, Redis.Exists("key:1"));
         Assert.AreEqual(true, Redis.Exists("key:2"));
      }

      [Test]
      public void RenamesNxTheKey()
      {
         Redis.Incr("key:1");
         Assert.AreEqual(true, Redis.RenameNx("key:1", "key:2"));
         Assert.AreEqual(false, Redis.Exists("key:1"));
         Assert.AreEqual(true, Redis.Exists("key:2"));
      }

      [Test]
      public void RenamesNxDoesNotOverwriteAnExistingKey()
      {
         Redis.Incr("key:1");
         Redis.Incr("key:2");
         Assert.AreEqual(false, Redis.RenameNx("key:1", "key:2"));
         Assert.AreEqual(true, Redis.Exists("key:1"));
         Assert.AreEqual(true, Redis.Exists("key:2"));
      }
   }
}