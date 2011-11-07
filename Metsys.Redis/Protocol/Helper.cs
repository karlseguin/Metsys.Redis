using System;

namespace Metsys.Redis
{
   public class Helper
   {
      private static readonly System.Text.Encoding _encoder = System.Text.Encoding.UTF8;
      public static readonly DateTime Epoch = new DateTime(1970, 1, 1);

      public static int GetBytes(string value, int index, int length, byte[] buffer, int offset)
      {
         return _encoder.GetBytes(value, 0, value.Length, buffer, offset);
      }

      public static byte[] GetBytes(string value)
      {
         return _encoder.GetBytes(value);
      }

      public static string GetString(DynamicBuffer buffer)
      {
         return _encoder.GetString(buffer.Buffer, 0, buffer.Length);
      }
   }
}