using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Blog.Core.AuthHelper.OverWrite
{
    using System.Security.Claims;
    /// <summary>
    /// 
    /// </summary>
    public class JwtTokenAuth
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public JwtTokenAuth(RequestDelegate next)
        {
            _next = next;
        }
        private void PreProceed(HttpContext next)
        {
            Console.WriteLine($"{DateTime.Now} middleware invoke preproceed");
            //...
        }
        private void PostProceed(HttpContext next)
        {
            Console.WriteLine($"{DateTime.Now} middleware invoke postproceed");
            //....
        }

        public Task Invoke(HttpContext httpContext)
        {
           // PreProceed(httpContext);


            //检测是否包含'Authorization'请求头
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                PostProceed(httpContext);

                return _next(httpContext);
            }
            //var tokenHeader = httpContext.Request.Headers["Authorization"].ToString();
            var tokenHeader = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                if (tokenHeader.Length >= 128)
                {
                    TokenModelJWT tm = JwtHelper.SerializeJWT(tokenHeader);

                    //授权
                    var claimList = new List<Claim>();
                    var claim = new Claim(ClaimTypes.Role, tm.Role);
                    claimList.Add(claim);
                    var identity = new ClaimsIdentity(claimList);
                    var principal = new ClaimsPrincipal(identity);
                    httpContext.User = principal;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} middleware wrong:{e.Message}");
            }


           // PostProceed(httpContext);


            return _next(httpContext);
        }
    }
}
