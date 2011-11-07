using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class AppendTests : BaseCommandTests
   {
      [Test]
      public void AppendsOntoANonExistingKey()
      {
         Assert.AreEqual(4, Redis.Append("key:1", "9000"));
         Assert.AreEqual("9000", Redis.Get<string>("key:1"));
      }
      [Test]
      public void AppendsToAnExistingKey()
      {
         Redis.Set("key:1", "it's over");
         Assert.AreEqual(14, Redis.Append("key:1", " 9000"));
         Assert.AreEqual("it's over 9000", Redis.Get<string>("key:1"));
      }
   }
}