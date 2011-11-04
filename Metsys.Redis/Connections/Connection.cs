using System;
using System.IO;
using System.Net.Sockets;

namespace Metsys.Redis
{
   public interface IConnection : IDisposable
   {
      void Send(byte[] data, int length);
      Stream GetStream();
      DateTime Created { get; }
   }

   public class Connection : IConnection
   {
      private bool _disposed;
      private readonly DateTime _created;
      private readonly TcpClient _client;
      private readonly NetworkStream _stream;

      public DateTime Created
      {
         get { return _created; }
      }

      public Connection(ConnectionInfo connectionInfo)
      {
         _client = new TcpClient
         {
            NoDelay = true,
            ReceiveTimeout = 10000,
            SendTimeout = 10000
         };
         _client.Connect(connectionInfo.Host, connectionInfo.Port);
         _stream = _client.GetStream();
         _created = DateTime.Now;
      }

      public void Send(byte[] data, int length)
      {
         _stream.Write(data, 0, length);
      }

      public void BeginSend(byte[] data, int length)
      {
         _stream.Write(data, 0, length);
      }


      public Stream GetStream()
      {
         return _stream;
      }

      public void Dispose()
      {
         Dispose(true);
      }
      protected virtual void Dispose(bool disposing)
      {
         if (_disposed) { return; }
         _client.Close();
         _disposed = true;
      }

      ~Connection()
      {
         Dispose(false);
      }
   }
}