using System;
using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class TtlAndExpireTests : BaseCommandTests
   {
      [Test]
      public void TtlReturnsMinusOneIfTheKeyDoesntExist()
      {
         Assert.AreEqual(-1, Redis.Ttl("invalid"));
      }

      [Test]
      public void ExpireAndExpireAtReturnFalseIfTheKeyDoestExist()
      {
         Assert.AreEqual(false, Redis.Expire("invalid", 123));
         Assert.AreEqual(false, Redis.ExpireAt("invalid", DateTime.Now));
      }

      [Test]
      public void ExpireSetsTheKeysExpiry()
      {
         //this looks brittle
         Redis.Incr("key1");
         Redis.Expire("key1", 1000);
         Assert.AreEqual(1000, Redis.Ttl("key1"));
      }

      [Test, Ignore("whatever")]
      public void ExpireAtSetsTheKeysExpiry()
      {
         //this looks brittle
         Redis.Incr("key2");
         var date = DateTime.Now.AddDays(1);
         var expected = (long) date.Subtract(Helper.Epoch).TotalMilliseconds;
         Redis.ExpireAt("key2", date);
         Assert.AreEqual(expected, Redis.Ttl("key2"));
      }
   }
}