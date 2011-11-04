using System;

namespace Metsys.Redis
{
   public class Writer
   {
      private const byte _argumentCountMarker = (byte)'*';
      private const byte _argumentBytesMarker = (byte)'$';
      
      public static DynamicBuffer Serialize(byte[] command, DynamicBuffer dynamicBuffer, params object[] parameters)
      {
         var parameterValues = (parameters.Length + 1).ToString();
         var commandValue = command.Length.ToString();

         var length = 8 + parameterValues.Length + commandValue.Length + command.Length;

         foreach (var parameter in parameters)
         {
            length += GetLength(parameter);
         }

         dynamicBuffer.SetLength(length);
         var buffer = dynamicBuffer.Buffer;
         var offset = 0;
         offset = WriteFastString(buffer, offset, parameterValues, _argumentCountMarker);
         offset = WriteFastString(buffer, offset, commandValue, _argumentBytesMarker);
         offset = WriteBytes(buffer, offset, command, command.Length);

         foreach(var parameter in parameters)
         {
            offset = WriteValue(buffer, offset, parameter);
         }
         return dynamicBuffer;
      }

      private static int WriteString(byte[] buffer, int offset, string value, byte marker)
      {
         buffer[offset] = marker;
         return WriteString(buffer, offset + 1, value);
      }

      private static int WriteString(byte[] buffer, int offset, string value)
      {
         offset += Encoding.GetBytes(value, 0, value.Length, buffer, offset);
         return WriteLineTerminator(buffer, offset);
      }

      private static int WriteFastString(byte[] buffer, int offset, string value, byte marker)
      {
         buffer[offset] = marker;
         return WriteFastString(buffer, offset + 1, value);
      }

      private static int WriteFastString(byte[] buffer, int offset, string value)
      {
         var length = value.Length;
         for (var i = 0; i < length; ++i)
         {
            buffer[offset++] = (byte) value[i];
         }
         return WriteLineTerminator(buffer, offset);
      }

      private static int WriteValue(byte[] buffer, int offset, object value)
      {
         if (value is string)
         {
            var v = (string)value;
            offset = WriteFastString(buffer, offset, v.Length.ToString(), _argumentBytesMarker);
            return WriteString(buffer, offset, v);
         }
         else
         {
            var v = (byte[]) value;
            var length = v.Length;
            offset = WriteString(buffer, offset, length.ToString(), _argumentBytesMarker);
            return WriteBytes(buffer, offset, v, length);
         }
      }

      private static int WriteBytes(byte[] buffer, int offset, byte[] value, int length)
      {
         Buffer.BlockCopy(value, 0, buffer, offset, length);
         return WriteLineTerminator(buffer, offset + length);
      }

      private static int WriteLineTerminator(byte[] buffer, int offset)
      {
         buffer[offset] = 0x0D;
         buffer[offset + 1] = 0x0A;
         return offset + 2;
      }

      private static int GetLength(object value)
      {
         var valueLength = value is string ? ((string) value).Length : ((byte[])value).Length;
         return 5 + valueLength + valueLength.ToString().Length;
      }
   }
}