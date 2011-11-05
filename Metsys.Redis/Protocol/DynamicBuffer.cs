namespace Metsys.Redis
{
   public class DynamicBuffer
   {
      private const int _smallBufferLength = 4096;
      private byte[] _currentBuffer;
      private readonly byte[] _smallBuffer;
      private int _size;
      private int _offset;

      public byte[] Buffer
      {
         get { return _currentBuffer; }
      }

      public int Length
      {
         get { return _offset; }
      }

      public DynamicBuffer()
      {
         _smallBuffer = new byte[_smallBufferLength];
      }

      public void Start()
      {
         _offset = 0;
         _size = _smallBufferLength;
         _currentBuffer = _smallBuffer;
      }

      public void Start(int size)
      {
         _offset = size;
         _size = size;
         _currentBuffer = size > _smallBufferLength ? new byte[size] : _smallBuffer;
      }

      public void Write(byte b)
      {
         EnsureSize(1);
         _currentBuffer[_offset++] = b;
      }

      public void Write(byte[] data)
      {
         var length = data.Length;
         EnsureSize(length);
         System.Buffer.BlockCopy(data, 0, _currentBuffer, _offset, length);
         _offset += length;
      }

      public void WriteBinarySafe(string value)
      {
         var length = value.Length;
         EnsureSize(length);
         for (var i = 0; i < length; ++i)
         {
            _currentBuffer[_offset++] = (byte) value[i];
         }
      }

      private void EnsureSize(int length)
      {
         if (_offset + length < _size)
         {
            return;
         }
         var newSize = _size + (length * 2) < 8196 ? 8196 :length * 2;
         var newBuffer = new byte[newSize];
         System.Buffer.BlockCopy(_currentBuffer, 0, newBuffer, 0, _size);
         _currentBuffer = newBuffer;
         _size = newSize;
      }
   }
}