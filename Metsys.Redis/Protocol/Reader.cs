using System.IO;
using System.Text;

namespace Metsys.Redis
{
   public class Reader
   {
      private const byte _integerReply = (byte)':';
      private const byte _errorReply = (byte)'-';

      public static long Integer(Stream stream)
      {
         AssertReplyKind(_integerReply, stream);

         const char zero = '0';
         var negative = false;
         var value = 0L;

         var b = stream.ReadByte();
         if (b == '-') { negative = true; }
         else { value = b - zero; }

         while ((b = stream.ReadByte()) != -1 && b != '\r')
         {
            value = value*10 + (b - zero);
         }
         stream.ReadByte(); // \n
         return negative ? -value : value;
      }

      private static void AssertReplyKind(byte type, Stream stream)
      {
         var b = stream.ReadByte();
         if (b == type) { return; }

         if (b == _errorReply)
         {
            throw new RedisException(ReadLine(stream));
         }
         throw new RedisException(string.Format("Expecting a reply of type '{0}' but got '{1}'", (char)type, (char)b));
      }

      private static string ReadLine(Stream stream)
      {
         int b;
         var sb = new StringBuilder(100);
         while ((b = stream.ReadByte()) != -1 && b != '\r')
         {
            sb.Append((char)b);
         }
         stream.ReadByte(); // \n
         return sb.ToString();
      }


   }
}