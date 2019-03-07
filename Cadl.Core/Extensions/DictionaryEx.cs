using System;
using System.Collections.Generic;

namespace Cloudform.Core.Extensions
{
    public static class DictionaryEx
    {
        public static Dictionary<TKey, TValue> Combine<TKey, TValue>(params Dictionary<TKey, TValue>[] dics)
        {
            var combined = new Dictionary<TKey, TValue>();
            foreach (var dic in dics)
            {
                foreach (var kv in dic)
                {
                    combined.Add(kv.Key, kv.Value);
                }
            }

            return combined;
        }

        public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            var copy = new Dictionary<TKey, TValue>();
            foreach (var kv in dic)
            {
                copy.Add(kv.Key, kv.Value);
            }

            return copy;
        }
    }
}
