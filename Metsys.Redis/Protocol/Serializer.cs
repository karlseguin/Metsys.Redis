using System;

namespace Metsys.Redis
{
   public delegate object WriteFunction(DynamicBuffer buffer);

   public class Serializer
   {
      public static T Deserialize<T>(DynamicBuffer buffer)
      {
         if (Serializer<T>.CacheFunction == null)
         {
            return default(T);
         }
         return (T)Serializer<T>.CacheFunction(buffer);
      }
   }

   public static class Serializer<T>
   {
      public static WriteFunction CacheFunction;

      static Serializer()
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
            CacheFunction = d => int.Parse(Encoding.GetString(d));
         }
         else if (type == typeof(long))
         {
            CacheFunction = d => long.Parse(Encoding.GetString(d));
         }
         else if (type == typeof(string))
         {
            CacheFunction = d => Encoding.GetString(d);
         }
      }
   }
}