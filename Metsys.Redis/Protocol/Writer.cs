namespace Metsys.Redis
{
   public class Writer
   {
      private const byte _argumentCountMarker = (byte)'*';
      private const byte _argumentBytesMarker = (byte)'$';

      public static DynamicBuffer Serialize(byte[] command, DynamicBuffer buffer, params object[] parameters)
      {
         buffer.Start();
         WriteBinarySafeString(buffer, (parameters.Length + 1).ToString(), _argumentCountMarker);
         WriteValue(buffer, command);
         foreach (var parameter in parameters)
         {
            WriteValue(buffer, parameter);
         }
         return buffer;
      }

      private static void WriteBinarySafeString(DynamicBuffer buffer, string value, byte marker)
      {
         buffer.Write(marker);
         buffer.WriteBinarySafe(value);
         WriteLineTerminator(buffer);
      }

      private static void WriteValue(DynamicBuffer buffer, object value)
      {
         var data = value is string ? Encoding.GetBytes((string) value) : (byte[]) value;
         WriteBinarySafeString(buffer, data.Length.ToString(), _argumentBytesMarker);
         buffer.Write(data);
         WriteLineTerminator(buffer);
      }
      
      private static void WriteLineTerminator(DynamicBuffer buffer)
      {
         buffer.Write(0x0D);
         buffer.Write(0x0A);
      }
   }
}