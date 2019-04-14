using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Blog.Core.Aop
{
    /// <summary>
    /// Redis缓存AOP
    /// </summary>
    public class BlogRedisCacheAOP:Castle.DynamicProxy.IInterceptor  //IInterceptor拦截器
    {
        /// <summary>
        /// 通过注入的方式，把缓存操作接口通过构造注入
        /// </summary>
        public Common.IRedisCacheManager _redisCache;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="redisCache"></param>
        public BlogRedisCacheAOP(Common.IRedisCacheManager redisCache)
        {
            _redisCache = redisCache;
        }
        /// <summary>
        /// 实现拦截器防范  Intercept方法是拦截的关键所在也是IInterceptor接口中唯一的定义
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法特性验证
            var qCachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(Common.CachingAttribute)) as Common.CachingAttribute;
            if(qCachingAttribute!=null)
            {
                //获取自定义缓存键
                var cacheKey = CustomCacheKey(invocation);
                //核心1：注意这里和之前不同，是获取的string值，之前是object
                var cacheValue = _redisCache.GetValue(cacheKey);
                if(cacheValue!=null)
                {
                    var type = invocation.Method.ReturnType;
                    var resultType = type.GenericTypeArguments;
                    if(type.FullName=="System.Void")
                    {
                        return;
                    }
                    object response;
                    if(type!=null&&typeof(Task).IsAssignableFrom(type))
                    {
                        //核心2返回异步对象Task<T>
                        if(resultType.Count()>0)
                        {
                            var resultTypes = resultType.FirstOrDefault();
                            //核心3 直接序列化成dynamic类型，之前我一直纠结特定的实体
                            dynamic temp = Newtonsoft.Json.JsonConvert.DeserializeObject(cacheValue, resultTypes);
                            response = Task.FromResult(temp);
                        }
                        else
                        {
                            //Task无返回方法时，指定时间内不允许重新允许
                            response = Task.Yield();
                        }
                    }
                    else
                    {
                        //核心4 要进行ChangeType
                        response = System.Convert.ChangeType(_redisCache.Get<object>(cacheKey), type);
                    }
                    invocation.ReturnValue = response;
                    return;
                }
                //执行当前方法
                invocation.Proceed();
                //存入缓存
                if(!string.IsNullOrWhiteSpace(cacheKey))
                {
                    object response;
                    var type = invocation.Method.ReturnType;
                    if(type!=null&&typeof(Task).IsAssignableFrom(type))
                    {
                        var resultProperty = type.GetProperty("Result");
                        response = resultProperty.GetValue(invocation.ReturnValue);
                    }
                    else
                    {
                        response = invocation.ReturnValue;
                    }
                    if (response == null)
                        response = string.Empty;
                    //核心5 将获取到指定的reponse和特性的缓存时间 进行set操作
                    _redisCache.Set(cacheKey, response, TimeSpan.FromMinutes(qCachingAttribute.AbsoluteExpiration));
                }
            }
            else
            {
                //继续调用被拦截方法
                invocation.Proceed();
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
