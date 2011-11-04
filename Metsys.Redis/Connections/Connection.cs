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
      bool IsAlive();
   }

   public class Connection : IConnection
   {
      private bool _disposed;
      private readonly DateTime _created;
      private readonly TcpClient _client;
      private readonly NetworkStream _stream;
      private bool _isValid;

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
         _isValid = true;
      }

      public void Send(byte[] data, int length)
      {
         try
         {
            _stream.Write(data, 0, length);
         }
         catch (IOException)
         {
            _isValid = false;
            throw;
         }
      }

      public Stream GetStream()
      {
         return _stream;
      }

      public bool IsAlive()
      {
         return _client.Connected && _isValid;
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }
      protected virtual void Dispose(bool disposing)
      {
         if (_disposed) { return; }
         if (disposing)
         {
            _client.Close();
         }
         _disposed = true;
      }

      ~Connection()
      {
         Dispose(false);
      }
   }
}