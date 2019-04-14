using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.Common
{
    /// <summary>
    /// 这个Attribute就是使用的验证，把它添加到要缓存数据的方法中，即可完成缓存操作。注意他是对Method验证有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,Inherited =true)]
  public  class CachingAttribute:Attribute
    {
        /// <summary>
        /// 缓存的绝对过期时间
        /// </summary>
        public int AbsoluteExpiration { get; set; } = 30;
    }
}
