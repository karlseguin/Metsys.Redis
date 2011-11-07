using System;

namespace Metsys.Redis
{
   public delegate object DeserializeFunction(DynamicBuffer buffer);
   
   public class Serializer
   {
      private static readonly byte[] _true = new byte[] {1};
      private static readonly byte[] _false = new byte[] {0};

      public static T Deserialize<T>(DynamicBuffer buffer)
      {
         if (Deserializer<T>.CacheFunction == null)
         {
            return default(T);
         }
         return (T)Deserializer<T>.CacheFunction(buffer);
      }

      public static void Serialize(DynamicBuffer buffer, object value)
      {
         if (value is bool)
         {
            WriteByteArray(buffer, (bool)value ? _true : _false);
         }
         else if (value is int)
         {
            WriteBinarySafeString(buffer, ((int)value).ToString());
         }
         else if (value is long)
         {
            WriteBinarySafeString(buffer, ((long)value).ToString());
         }
         else if (value is DateTime)
         {
            WriteBinarySafeString(buffer, ((long)((DateTime)value).Subtract(Helper.Epoch).TotalMilliseconds).ToString());
         }
         else if (value is string)
         {
            WriteByteArray(buffer, Helper.GetBytes((string)value));
         }
         else if (value is byte[])
         {
            WriteByteArray(buffer, (byte[])value);
         }
         else
         {
            throw new ArgumentException("Can't serialize " + value.GetType());
         }
      }

      private static void WriteByteArray(DynamicBuffer buffer, byte[] value)
      {
         Writer.WriteLength(buffer, value.Length);
         buffer.Write(value);
         Writer.WriteLineTerminator(buffer);
      }

      private static void WriteBinarySafeString(DynamicBuffer buffer, string value)
      {
         //tight-coupling, but it reduces the amount of byte[] we need to create
         Writer.WriteLength(buffer, value.Length);
         Writer.WriteBinarySafeString(buffer, value);
      }
   }

   public static class Deserializer<T>
   {
      public static DeserializeFunction CacheFunction;

      static Deserializer()
      {
         var type = typeof(T);
         if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
         {
            type = Nullable.GetUnderlyingType(type);
         }
         if (type == typeof(bool))
         {
            CacheFunction = d => d.Buffer[0] == 1;
         }
         else if (type == typeof(int))
         {
            CacheFunction = d => int.Parse(Helper.GetString(d));
         }
         else if (type == typeof(long))
         {
            CacheFunction = d => long.Parse(Helper.GetString(d));
         }
         else if (type == typeof(DateTime))
         {
            CacheFunction = d => Helper.Epoch.AddMilliseconds(long.Parse(Helper.GetString(d)));
         }
         else if (type == typeof(string))
         {
            CacheFunction = d => Helper.GetString(d);
         }
         else if (type == typeof(byte[]))
         {
            CacheFunction = d => d.ToArray();
         }
      }
   }
}