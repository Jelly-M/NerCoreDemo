using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace Blog.Core.Common
{
    //注意：
    //需要引入nuget包 StackExchange.Redis 对Redis做基础连接操作

    public class RedisCacheManager : IRedisCacheManager
    {
        private readonly string redisConnenctionString;
        public volatile StackExchange.Redis.ConnectionMultiplexer redisConnection;
        private readonly object redisConnectionLock = new object();

        public RedisCacheManager()
        {
            string redisConfiguration = Appsettings.app(new string[] { "AppSettings", "RedisCaching", "ConnectionString" });
            if(string.IsNullOrWhiteSpace(redisConfiguration))
            {
                throw new ArgumentException("redis config is empty",nameof(redisConfiguration));
            }
            this.redisConnenctionString = redisConfiguration;
            this.redisConnection = GetRedisConnection();
        }
        /// <summary>
        /// 核心代码，获取连接实例
        /// 通过双if 夹lock的方式，实现单例模式
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetRedisConnection()
        {
            //如果已经连接实例，直接返回
            if (this.redisConnection != null && this.redisConnection.IsConnected)
            {
                return this.redisConnection;
            }
            //加锁，防止异步编程中，出现单例无效的问题
            lock (redisConnectionLock)
            {
                if (this.redisConnection != null)
                {
                    //释放redis连接
                    this.redisConnection.Dispose();
                }
                try
                {
                    this.redisConnection = ConnectionMultiplexer.Connect(redisConnenctionString);
                }
                catch (Exception)
                {

                    throw new Exception("Redis服务未启用，请开启该服务");
                }
            }
            return this.redisConnection;
        }


        /// <summary>
        /// 实现类
        /// </summary>

        //全部清除
        public void Clear()
        {
            foreach (var endPoint in this.GetRedisConnection().GetEndPoints())
            {
                var server = this.GetRedisConnection().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    redisConnection.GetDatabase().KeyDelete(key);
                }
            }
        }
        //获取值，并序列化
        public TEntity Get<TEntity>(string key)
        {
            var value = redisConnection.GetDatabase().StringGet(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return Helper.SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default(TEntity);
            }
        }
        //判断是否存在
        public bool Get(string key)
        {
            return redisConnection.GetDatabase().KeyExists(key);
        }
        /// <summary>
        /// 获取redis的缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return redisConnection.GetDatabase().StringGet(key);
        }
        //移除某一个缓存值
        public void Remove(string key)
        {
            redisConnection.GetDatabase().KeyDelete(key);
        }
        //保存
        public void Set(string key, object value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                //序列化，将object值生成RedisValue
                redisConnection.GetDatabase().StringSet(key, Helper.SerializeHelper.Serialize(value), cacheTime);
            }
        }

        public bool SetValue(string key, byte[] value)
        {
            return redisConnection.GetDatabase().StringSet(key, value, TimeSpan.FromSeconds(120));
        }
    }
}
