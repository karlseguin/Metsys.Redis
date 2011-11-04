using System;
using System.Collections.Concurrent;

namespace Metsys.Redis
{
   public class Pool<T>
   {
      private readonly int _count;
      private readonly ConcurrentQueue<T> _pool;
      private readonly Func<Pool<T>, T> _create;
      
      public Pool(int count, Func<Pool<T>, T> create)
      {
         _count = count;
         _create = create;
         _pool = new ConcurrentQueue<T>();
         for (var i = 0; i < count; ++i)
         {
            _pool.Enqueue(create(this));
         }
      }
      public T CheckOut()
      {
         T t;
         return _pool.TryDequeue(out t) ? t : _create(this);
      }
      public void CheckIn(T value)
      {
         if (_pool.Count < _count)
         {
            _pool.Enqueue(value);
         }
      }
   }
}