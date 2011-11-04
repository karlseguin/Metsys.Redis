using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class IncrTests : BaseCommandTests
   {
      [Test]
      public void IncrementsAKey()
      {
         for (var i = 0; i < 5; ++i)
         {
            IncrementAndAssert("test:1", i+1);
         }
         IncrementAndAssert("test:2", 1);
         AssertKeyValue("test:1", 5);
      }

      private void IncrementAndAssert(string key, int expected)
      {
         Assert.AreEqual(expected, Redis.Incr(key));
         AssertKeyValue(key, expected);
      }

      private void AssertKeyValue(string key, int expected)
      {
         Assert.AreEqual(expected, Redis.Get<long>(key));
      }
   }
}