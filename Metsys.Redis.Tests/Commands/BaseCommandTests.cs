using NUnit.Framework;

namespace Metsys.Redis.Tests.Commands
{
   public class BaseCommandTests
   {
      protected IRedis Redis;

      [TestFixtureSetUp]
      public void FixtureSetUp()
      {
         Redis = RedisManager.Configure(c => c.ConnectTo("127.0.0.1", 6379).UsingDatabase(5)).Redis();
      }

      [SetUp]
      public void SetUp()
      {
         Redis.FlushDb();
         BeforeEachTest();
      }
      protected virtual void BeforeEachTest() {}

   }
}