namespace Metsys.Redis
{
   public class ConnectionInfo
   {
      //if you don't get this, you don't get me
      public readonly string Host;
      public readonly int Port;

      public ConnectionInfo(string host, int port)
      {
         Host = host;
         Port = port;
      }
   }
}