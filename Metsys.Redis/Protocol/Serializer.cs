using System;
using System.Collections.Generic;

namespace Metsys.Redis
{
   public class Serializer
   {
      private static readonly IDictionary<Type, Func<DynamicBuffer, object>> _deserializationTypeLookup = new Dictionary<Type, Func<DynamicBuffer, object>>
        {
           {typeof (bool), d => d.Buffer[0] == 1},
           {typeof (int), d => int.Parse(Encoding.GetString(d))},
           {typeof (long), d => long.Parse(Encoding.GetString(d))},

        };

      public static T Deserialize<T>(DynamicBuffer dynamicBuffer)
      {
         Func<DynamicBuffer, object> reader;
         var type = typeof (T);
         if (_deserializationTypeLookup.TryGetValue(type, out reader))
         {
            return (T)reader(dynamicBuffer);
         }
         return default(T);
      }
   }
}