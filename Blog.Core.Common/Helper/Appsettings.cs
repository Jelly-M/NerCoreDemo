//using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace Blog.Core.Common
{
    /// <summary>
    /// appsettings.json操作类
    /// 这里引用nuget包Microsoft.Extensions.Configuration.Json
    /// </summary>
    public class Appsettings
    {
        static Microsoft.Extensions.Configuration.IConfiguration _configuration { get; set; }

        static Appsettings()
        {
            _configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource {Path= "appsettings.json", ReloadOnChange =true})
                .Build();
        }
        public static string app(params string[] strPara)
        {
            try
            {
                var val = string.Empty;
                for (int i = 0; i < strPara.Length; i++)
                {
                    val += strPara[i] + ":";
                }
                return _configuration[val.TrimEnd(':')];
            }
            catch (Exception)
            {

                return "";
            }
        }
    }
}
