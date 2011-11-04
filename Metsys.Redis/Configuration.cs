namespace Metsys.Redis
{
   public interface IConfiguration
   {
      Configuration ConnectTo(string host, int port);
   }

   public class Configuration : IConfiguration
   {
      public ConnectionInfo Server { get; private set; }
      public int Database;

      public Configuration ConnectTo(string host, int port)
      {
         Server = new ConnectionInfo(host, port);
         return this;
      }

      public Configuration UsingDatabase(int database)
      {
         Database = database;
         return this;
      }
   }
}