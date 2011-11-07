using System;
using System.Collections.Generic;

namespace Metsys.Redis
{
   public interface IRedis : IDisposable
   {
      int Append(string key, string value);
      int DbSize();
      long Decr(string key);
      long DecrBy(string key, int value);
      long Del(params string[] key);
      bool Exists(string key);
      bool Expire(string key, int seconds);
      bool ExpireAt(string key, DateTime date);
      void FlushDb();
      T Get<T>(string key);
      bool GetBit(string key, int offset);
      string GetRange(string key, int start, int end);
      T GetSet<T>(string key, object value);
      long Incr(string key);
      long IncrBy(string key, int value);
      string[] Keys(string pattern = "*");
      T[] MGet<T>(params string[] keys);
      bool Move(string key, int database);
      void MSet(ICollection<KeyValuePair<string, object>> keyAndValues);
      bool MSetNx(ICollection<KeyValuePair<string, object>> keyAndValues);
      bool Persist(string key);
      string RandomKey();
      void Rename(string key, string newName);
      bool RenameNx(string key, string newName);
      void Select(int database);
      void Set(string key, object value);
      bool SetBit(string key, int offset, bool bit);
      void SetEx(string key, int seconds, object value);
      bool SetNx(string key, object value);
      int SetRange(string key, int offset, object value);
      int StrLen(string key);
      long Ttl(string key);
      string Type(string key);
   }
}