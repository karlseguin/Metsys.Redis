using System;
using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class GetAndSetTests : BaseCommandTests
   {
      [Test]
      public void SetsDifferentTypes()
      {
         AssertEqual(System.Text.Encoding.UTF8.GetBytes("abc123"));
         AssertEqual(0);
         AssertEqual(int.MinValue);
         AssertEqual(int.MaxValue);
         AssertEqual(long.MinValue);
         AssertEqual(long.MaxValue);
         AssertEqual(true);
         AssertEqual(false);
         AssertEqual("hello world");
         AssertEqual("a伦b");
         AssertDateEqual(DateTime.Now);
         AssertDateEqual(DateTime.MinValue);
         AssertDateEqual(new DateTime(9999, 12, 31));
      }

      [Test]
      public void GetSetTest()
      {
         Redis.Set("key1", "old");
         Assert.AreEqual("old", Redis.GetSet<string>("key1", 44));
         Assert.AreEqual(44, Redis.Get<int>("key1"));
      }

      [Test]
      public void SetNxDoesntSetTheValueIfTheKeyAlreadyExists()
      {
         Assert.AreEqual(true, Redis.SetNx("key1", "old"));
         Assert.AreEqual(false, Redis.SetNx("key1", "new"));
         Assert.AreEqual("old", Redis.Get<string>("key1"));
      }

      [Test]
      public void SetExSetsAKeyWithAnExpiry()
      {
         Redis.SetEx("key1", 5000, "value");
         Assert.AreEqual(5000, Redis.Ttl("key1"));
         Assert.AreEqual("value", Redis.Get<string>("key1"));
      }

      private void AssertEqual<T>(T i)
      {
         Redis.Set("get:set:test:1", i);
         Assert.AreEqual(i, Redis.Get<T>("get:set:test:1"));
      }

      private void AssertDateEqual(DateTime i)
      {
         Redis.Set("get:set:test:2", i);
         Assert.AreEqual(i.ToString(), Redis.Get<DateTime>("get:set:test:2").ToString());
      }
   }
}