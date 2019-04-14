using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Core
{
    /// <summary>
    /// 简单的缓存接口，只有查询和添加，后面会扩展
    /// </summary>
   public interface ICaching
    {
        object Get(string strKey);
        void Set(string cacheKey, object cacheValue);
    }
}
