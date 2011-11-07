namespace Metsys.Redis
{
   public class Writer
   {
      private const byte _argumentCountMarker = (byte)'*';
      private const byte _argumentBytesMarker = (byte)'$';

      public static DynamicBuffer Serialize(byte[] command, DynamicBuffer buffer, params object[] values)
      {
         buffer.Start();
         WriteBinarySafeString(buffer, (values.Length + 1).ToString(), _argumentCountMarker);
         Serializer.Serialize(buffer, command);
         foreach (var value in values)
         {
            Serializer.Serialize(buffer, value);
         }
         return buffer;
      }

      public static void WriteLength(DynamicBuffer buffer, int length)
      {
         WriteBinarySafeString(buffer, length.ToString(), _argumentBytesMarker);
      }

      public static void WriteBinarySafeString(DynamicBuffer buffer, string value)
      {
         buffer.WriteBinarySafe(value);
         WriteLineTerminator(buffer);
      }

      private static void WriteBinarySafeString(DynamicBuffer buffer, string value, byte marker)
      {
         buffer.Write(marker);
         WriteBinarySafeString(buffer, value);
      }
      
      public static void WriteLineTerminator(DynamicBuffer buffer)
      {
         buffer.Write(0x0D);
         buffer.Write(0x0A);
      }
   }
}