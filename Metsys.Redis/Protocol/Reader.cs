using System.IO;
using System.Text;

namespace Metsys.Redis
{
   public class Reader
   {
      private const byte _integerMarker = (byte)':';
      private const byte _lineMarker = (byte)'+';
      private const byte _errorMarker = (byte)'-';
      private const byte _bulkMarker = (byte)'$';
      private const byte _multiBulkMarker = (byte)'*';
      private const byte _OReply = (byte)'O';
      private const byte _KReply = (byte)'K';
      private const byte _CRReply = (byte)'\r';
      private const byte _LFReply = (byte)'\n';

      public static long Integer(Stream stream, DynamicBuffer context)
      {
         AssertReplyKind(_integerMarker, stream);
         return ReadNumber(stream);
      }

      public static bool Status(Stream stream, DynamicBuffer context)
      {
         AssertReplyKind(_lineMarker, stream);
         AssertNextByteIs(stream, _OReply);
         AssertNextByteIs(stream, _KReply);
         ReadCrLf(stream);
         return true;
      }

      public static int Bulk(Stream stream, DynamicBuffer context)
      {
         AssertReplyKind(_bulkMarker, stream);
         var length = (int)ReadNumber(stream);
         if (length == -1)
         {
            return 0;
         }
         context.Start(length);
         var buffer = context.Buffer;
         var read = 0;
         while (read < length)
         {
            read += stream.Read(buffer, read, length);
         }
         ReadCrLf(stream);
         return length;
      }

      public static T[] MultiBulk<T>(Stream stream, DynamicBuffer buffer)
      {
         AssertReplyKind(_multiBulkMarker, stream);
         var count = ReadNumber(stream);
         var values = new T[count];
         for(var i = 0; i < count; ++i)
         {
            var length = Bulk(stream, buffer);
            values[i] = length == 0 ? default(T) : Serializer.Deserialize<T>(buffer);
         }
         return values;
      }

      private static void AssertNextByteIs(Stream stream, byte expected)
      {
         var b = stream.ReadByte();
         if (b == expected) { return; }
         throw new RedisException(string.Format("Expecting '{0}' but got '{1}'", (char)expected, (char)b));
      }

      private static void AssertReplyKind(byte expected, Stream stream)
      {
         var b = stream.ReadByte();
         if (b == expected) { return; }

         if (b == _errorMarker)
         {
            throw new RedisException(ReadLine(stream));
         }
         throw new RedisException(string.Format("Expecting a reply of type '{0}' but got '{1}'", (char)expected, (char)b));
      }

      private static long ReadNumber(Stream stream)
      {
         const char zero = '0';
         var negative = false;
         var value = 0L;

         var b = stream.ReadByte();
         if (b == '-') { negative = true; }
         else { value = b - zero; }

         while ((b = stream.ReadByte()) != -1 && b != '\r')
         {
            value = value * 10 + (b - zero);
         }
         stream.ReadByte(); // \n
         return negative ? -value : value;
      }

      private static void ReadCrLf(Stream stream)
      {
         AssertNextByteIs(stream, _CRReply);
         AssertNextByteIs(stream, _LFReply);
      }

      private static string ReadLine(Stream stream)
      {
         int b;
         var sb = new StringBuilder(100);
         while ((b = stream.ReadByte()) != -1 && b != _CRReply)
         {
            sb.Append((char)b);
         }
         stream.ReadByte(); // \n
         return sb.ToString();
      }
   }
}