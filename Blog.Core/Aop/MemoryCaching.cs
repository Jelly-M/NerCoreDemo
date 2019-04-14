using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Core
{
    public class MemoryCaching : ICaching
    {
        private readonly IMemoryCache memoryCache;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="imemoryCache"></param>
        public MemoryCaching(IMemoryCache imemoryCache)
        {
            this.memoryCache = imemoryCache;
        }
        public object Get(string strKey)
        {
           return  memoryCache.Get(strKey);
        }

        public void Set(string cacheKey, object cacheValue)
        {
            memoryCache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(7200));
        }
    }
}
