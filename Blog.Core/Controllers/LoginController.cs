using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.AuthHelper.OverWrite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        [HttpGet]
        [Route("Token3")]      //这个是做什么用的？？？？
        public JsonResult GetJWTStr(long id = 1, string sub = "Admin")
        {
            //这里是用户登陆以后通过数据库调取数据
            TokenModelJWT modelJWT = new TokenModelJWT();
            modelJWT.Uid = id;
            modelJWT.Role = sub;

            string jwtStr = JwtHelper.IssueJWT(modelJWT);
            return Json(jwtStr);
        }
        /// <summary>
        /// 测试跨域请求
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="id"></param>
        /// <param name="sub"></param>
        /// <param name="expiresSliding"></param>
        /// <param name="expiresAbsoulute"></param>

        [HttpGet]
        [Route("jsonp")]
        public void Getjsonp(string callBack, long id = 1, string sub = "Admin", int expiresSliding = 30, int expiresAbsoulute = 30)
        {
            TokenModelJWT tokenModel = new TokenModelJWT();
            tokenModel.Uid = id;
            tokenModel.Role = sub;

            string jwtStr = JwtHelper.IssueJWT(tokenModel);

            string response = string.Format("\"value\":\"{0}\"", jwtStr);
            //     string call = callBack + "({" + response + "})";
              //   string call = "{" + response + "}";
            Response.WriteAsync(response);
            
        }
    }
}