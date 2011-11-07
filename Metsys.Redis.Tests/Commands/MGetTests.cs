using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class MGetTests : BaseCommandTests
   {
      [Test]
      public void GetsMultipleValues()
      {
         Redis.Set("key1", "v1");
         Redis.Set("key2", "v2");
         Redis.Set("key3", "v3");
         Assert.AreEqual(new[] {"v1", "v3", null}, Redis.MGet<string>("key1", "key3", "key4"));
      }
   }
}