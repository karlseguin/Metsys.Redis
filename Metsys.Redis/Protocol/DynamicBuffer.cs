namespace Metsys.Redis
{
   public class DynamicBuffer
   {
      private const int _smallBufferLength = 4096;
      private readonly byte[] _smallBuffer;
      private byte[] _dynamicBuffer;
      private int _length;
      
      public byte[] Buffer
      {
         get { return _dynamicBuffer ?? _smallBuffer; }
      }

      public int Length
      {
         get { return _length; }
      }

      public DynamicBuffer()
      {
         _smallBuffer = new byte[_smallBufferLength];
      }

      public void SetLength(int length)
      {
         _length = length;
         if (length > _smallBufferLength && (_dynamicBuffer == null || length > _dynamicBuffer.Length))
         {
            _dynamicBuffer = new byte[length];
         } 
         else
         {
            _dynamicBuffer = null;
         }
      }
   }
}