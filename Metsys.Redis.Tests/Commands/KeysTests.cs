using NUnit.Framework;
using System.Linq;

namespace Metsys.Redis.Tests.Commands
{
   public class KeysTests : BaseCommandTests
   {
      [Test]
      public void ReturnsNoFoundKeys()
      {
         Assert.AreEqual(new string[0], Redis.Keys());
         Redis.Incr("key1");
         Assert.AreEqual(new string[0], Redis.Keys("blah"));
      }

      [Test]
      public void FindsAllKeys()
      {
         Redis.Incr("key1");
         Redis.Incr("key2");
         Assert.AreEqual(new[] { "key1", "key2" }.OrderBy(k => k), Redis.Keys().OrderBy(k => k));
         Redis.Incr("key3");
         Assert.AreEqual(new[] { "key2", "key1", "key3" }.OrderBy(k => k), Redis.Keys().OrderBy(k => k));
      }

      [Test]
      public void FindKeysByPattern()
      {
         Redis.Incr("key:1");
         Redis.Incr("key:2");
         Redis.Incr("key3");
         Assert.AreEqual(new[] { "key:1", "key:2" }.OrderBy(k => k), Redis.Keys("key:*").OrderBy(k => k));
      }
   }
}