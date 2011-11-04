using System;

namespace Metsys.Redis
{
   public class Writer
   {
      private static readonly Pool<WriteContext> _contextPool = new Pool<WriteContext>(500, p => new WriteContext(p));
      private const byte _argumentCountMarker = (byte)'*';
      private const byte _argumentBytesMarker = (byte)'$';
      
      public static WriteContext Serialize(byte[] command, params object[] parameters)
      {
         var parameterValues = (parameters.Length + 1).ToString();
         var commandValue = command.Length.ToString();

         var length = 8 + parameterValues.Length + commandValue.Length + command.Length;

         foreach (var parameter in parameters)
         {
            length += GetLength(parameter);
         }

         var context = _contextPool.CheckOut();
         context.SetLength(length);
         var buffer = context.Buffer;
         var offset = 0;
         offset = WriteString(buffer, offset, parameterValues, _argumentCountMarker);
         offset = WriteString(buffer, offset, commandValue, _argumentBytesMarker);
         offset = WriteBytes(buffer, offset, command, command.Length);

         foreach(var parameter in parameters)
         {
            offset = WriteValue(buffer, offset, parameter);
         }
         return context;
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

      private static int WriteValue(byte[] buffer, int offset, object value)
      {
         if (value is string)
         {
            var v = (string)value;
            offset = WriteString(buffer, offset, v.Length.ToString(), _argumentBytesMarker);
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
         buffer[offset] = (byte) '\r';
         buffer[offset+1] = (byte) '\n';
         return offset + 2;
      }

      private static int GetLength(object value)
      {
         var valueLength = value is string ? ((string) value).Length : ((byte[])value).Length;
         return 5 + valueLength + valueLength.ToString().Length;
      }
   }
}