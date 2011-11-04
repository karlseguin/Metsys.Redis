using System;

namespace Metsys.Redis
{
   public class WriteContext : IDisposable
   {
      private static readonly Pool<byte[]> _smallBuffer = new Pool<byte[]>(1000, p => new byte[250]);
      private byte[] _buffer;
      private bool _fromPool;
      private int _length;
      private readonly Pool<WriteContext> _parentPool;

      public byte[] Buffer
      {
         get { return _buffer; }
      }

      public int Length
      {
         get { return _length; }
      }

      public WriteContext(Pool<WriteContext> parentPool)
      {
         _parentPool = parentPool;
      }

      public void SetLength(int length)
      {
         _length = length;
         if (length < 250)
         {
            _buffer = _smallBuffer.CheckOut();
            _fromPool = true;
         }
         else
         {
            _fromPool = false;
            _buffer = new byte[length];
         }
      }

      public void Dispose()
      {
         Dispose(true);
      }
      protected virtual void Dispose(bool disposing)
      {
         if (!disposing || _buffer == null || !_fromPool) { return; }
         _smallBuffer.CheckIn(_buffer);
         _buffer = null;
         _parentPool.CheckIn(this);
      }

      ~WriteContext()
      {
         Dispose(false);
      }
      
      
   }
}