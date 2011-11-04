using System;
using System.Collections.Generic;

namespace Metsys.Redis
{
   public class Serializer
   {
      private static readonly IDictionary<Type, Func<byte[], object>> _deserializationTypeLookup = new Dictionary<Type, Func<byte[], object>>
        {
           {typeof (bool), d => d[0] == 1},
           {typeof (int), d => int.Parse(Encoding.GetString(d))},
           {typeof (long), d => long.Parse(Encoding.GetString(d))},

        };

      public static T Deserialize<T>(byte[] data)
      {
         Func<byte[], object> reader;
         var type = typeof (T);
         if (_deserializationTypeLookup.TryGetValue(type, out reader))
         {
            return (T) reader(data);
         }
         return default(T);
      }
   }
}