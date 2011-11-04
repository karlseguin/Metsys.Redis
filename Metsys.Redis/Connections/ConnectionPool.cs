using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Metsys.Redis
{
   public class ConnectionPool
   {
      private const int _maximumPoolSize = 10;
      private readonly AutoResetEvent _notifier = new AutoResetEvent(false);
      private readonly ConnectionInfo _connectionInfo;
      private readonly ConcurrentQueue<IConnection> _freeConnections = new ConcurrentQueue<IConnection>();
      private readonly ConcurrentQueue<IConnection> _invalidConnections = new ConcurrentQueue<IConnection>();
      private volatile int _connectionsInUse;
      private Timer _timer;

      public ConnectionPool(ConnectionInfo connectionInfo)
      {
         _connectionInfo = connectionInfo;
         _connectionsInUse = 0;
         _timer = new Timer(o => Cleanup(), null, 30000, 30000);
      }

      public bool CheckOut(out IConnection connection)
      {
         if (_freeConnections.TryDequeue(out connection))
         {
            Interlocked.Increment(ref _connectionsInUse);
            return false;
         }

         if (_connectionsInUse < _maximumPoolSize)
         {
            Interlocked.Increment(ref _connectionsInUse);
            connection = new Connection(_connectionInfo);
            return true;
         }
         if (!_notifier.WaitOne(10000))
         {
            throw new RedisException("Connection timeout trying to get connection from connection pool");
         }
         return CheckOut(out connection);
      }

      public void CheckIn(IConnection connection, bool error)
      {
         if (error || !IsAlive(connection))
         {
            _invalidConnections.Enqueue(connection);
            Interlocked.Decrement(ref _connectionsInUse);
            return;
         }
         _freeConnections.Enqueue(connection);
         Interlocked.Decrement(ref _connectionsInUse);
         _notifier.Set();
      }

      public void Cleanup()
      {
         CheckFreeConnectionsAlive();
         DisposeInvalidConnections();
      }

      private void CheckFreeConnectionsAlive()
      {
         IConnection connection;
         if (_freeConnections.TryDequeue(out connection))
         {
            if (IsAlive(connection))
            {
               _freeConnections.Enqueue(connection);
            }
            else
            {
               _invalidConnections.Enqueue(connection);
            }
         } 
      }

      private void DisposeInvalidConnections()
      {
         IConnection connection;
         while (_invalidConnections.TryDequeue(out connection))
         {
            connection.Dispose();
         }
      }

      private static bool IsAlive(IConnection connection)
      {
         return DateTime.Now.Subtract(connection.Created).TotalMinutes < 10 && connection.IsAlive();
      }
   }
}