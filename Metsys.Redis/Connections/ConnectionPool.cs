using System;
using System.Collections.Generic;
using System.Threading;

namespace Metsys.Redis
{
   public class ConnectionPool
   {
      private const int _poolSize = 5;
      private const int _maximumPoolSize = 10;
      private readonly object _lock = new object();
      private readonly ConnectionInfo _connectionInfo;
      private readonly Queue<IConnection> _freeConnections = new Queue<IConnection>();
      private readonly List<IConnection> _invalidConnections = new List<IConnection>();
      private volatile int _connectionsInUse;
      private Timer _timer;

      public ConnectionPool(ConnectionInfo connectionInfo)
      {
         _connectionInfo = connectionInfo;
         _connectionsInUse = 0;
         _timer = new Timer(o => Cleanup(), null, 30000, 30000);
      }

      public IConnection CheckOut()
      {
         lock (_lock)
         {
            if (_freeConnections.Count > 0)
            {
               Interlocked.Increment(ref _connectionsInUse);
               return _freeConnections.Dequeue();
            }
            if (_connectionsInUse >= _maximumPoolSize)
            {
               if (!Monitor.Wait(_lock, 10000))
               {
                  throw new RedisException("Connection timeout trying to get connection from connection pool");
               }
               return CheckOut();
            }
         }

         Interlocked.Increment(ref _connectionsInUse);
         return new Connection(_connectionInfo);
      }

      public void CheckIn(IConnection connection)
      {
         if (!IsAlive(connection))
         {
            _invalidConnections.Add(connection);
            Interlocked.Decrement(ref _connectionsInUse);
            return;
         }
         lock (_lock)
         {
            _freeConnections.Enqueue(connection);
            Interlocked.Decrement(ref _connectionsInUse);
            Monitor.Pulse(_lock);
         }
      }

      public void Cleanup()
      {
         CheckFreeConnectionsAlive();
         DisposeInvalidConnections();
      }

      private void CheckFreeConnectionsAlive()
      {
         lock (_lock)
         {
            var freeConnections = _freeConnections.ToArray();
            _freeConnections.Clear();

            foreach (var connection in freeConnections)
            {
               if (IsAlive(connection) && _freeConnections.Count < _poolSize)
               {
                  _freeConnections.Enqueue(connection);
               }
               else
               {
                  _invalidConnections.Add(connection);
               }
            }
         }
      }

      private void DisposeInvalidConnections()
      {
         IConnection[] invalidConnections;
         lock (_lock)
         {
            invalidConnections = _invalidConnections.ToArray();
            _invalidConnections.Clear();
         }

         foreach (var invalidConnection in invalidConnections)
         {
            invalidConnection.Dispose();
         }
      }

      private static bool IsAlive(IConnection connection)
      {
         return DateTime.Now.Subtract(connection.Created).TotalMinutes < 10;
      }
   }
}