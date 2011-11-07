using System.Collections.Generic;
using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class MSetTests : BaseCommandTests
   {
      [Test]
      public void SetsMultipleKeys()
      {
         Redis.MSet(new Dictionary<string, object>{{"key1", 9000}, {"key2", "it's over"}});
         Assert.AreEqual(9000, Redis.Get<int>("key1"));
         Assert.AreEqual("it's over", Redis.Get<string>("key2"));
      }

      [Test]
      public void DoesNotSetAnyKeyIfOneExistsUsingNx()
      {
         Redis.Set("key1", "existing");
         Assert.AreEqual(false, Redis.MSetNx(new Dictionary<string, object> { { "key1", 9000 }, { "key2", "it's over" } }));
         Assert.AreEqual("existing", Redis.Get<string>("key1"));
         Assert.AreEqual(null, Redis.Get<string>("key2"));
      }

      [Test]
      public void SetsMultipleKeysIfNoneExistUsingNx()
      {
         Redis.MSetNx(new Dictionary<string, object> { { "key1", 9000 }, { "key2", "it's over" } });
         Assert.AreEqual(9000, Redis.Get<int>("key1"));
         Assert.AreEqual("it's over", Redis.Get<string>("key2"));
      }
   }
}