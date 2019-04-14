using Blog.Core.Common;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Core
{
    /// <summary>
    /// 切面缓存设置
    /// </summary>
    public class BlogCacheAOP : IInterceptor
    {
        private ICaching _cache;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="cache"></param>
        public BlogCacheAOP(ICaching cache)
        {
            _cache = cache;
        }
        /// <summary>
        /// 实现方法Intercept方法是拦截的关键所在，也是IInterceptor接口中的唯一定义
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法进行特性验证  
            var qCachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;
            //只有验证了的才可以被，需要验证
            if (qCachingAttribute!=null)
            {
                //获取自定义缓存键
                var cacheKey = CustomCacheKey(invocation); //拿到的key值，AdvertisementServices:Query
                var cacheValue = _cache.Get(cacheKey);

                if (cacheValue != null)
                {
                    invocation.ReturnValue = cacheValue;
                    return;
                }

                //去执行当前方法
                invocation.Proceed();
                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    _cache.Set(cacheKey, invocation.ReturnValue);
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
            
        }

        /// <summary>
        /// 自定义缓存键
        /// </summary>
        /// <returns></returns>
        private string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();


            string key = $"{typeName}:{methodName}:";
            foreach (var param in methodArguments)
            {
                key += $"{param}:";
            }

            return key.TrimEnd(':');
        }

        //object 转 string
        private string GetArgumentValue(object arg)
        {
            if (arg is int || arg is long || arg is string)
                return arg.ToString();

            if (arg is DateTime)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");

            return "";
        }
    }
}
