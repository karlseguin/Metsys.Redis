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