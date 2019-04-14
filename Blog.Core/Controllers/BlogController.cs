using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// Blog控制器所有接口
    /// </summary>
    //[Route("api/[controller]")]
    //[ApiController]
    [Produces("application/json")]
    [Route("api/Blog")]
   // [Authorize(Policy = "Admin")]   //设置的身份验证过不去所以就先注释掉
    public class BlogController : Controller
    {
        IServices.IAdvertisementServices aderService { get; set; }
        IServices.IBlogArticleServices _blogAtricleService { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="services"></param>
        /// /// <param name="blogAtricleService"></param>
        public BlogController(IServices.IAdvertisementServices services,IServices.IBlogArticleServices blogAtricleService )
        {
            aderService = services;
            _blogAtricleService = blogAtricleService;
        }
        /// <summary>
        /// 获取博文列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="bcategory"></param>
        /// <returns></returns>
       
        [HttpGet]
        [Route("GetBlogs")]        
        public async Task<object> GetBlogs()
        {
            var list= await _blogAtricleService.GetBlogs();
            
            return Json(list);

        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        [HttpGet("{id},{str:int}")]        
        public async Task<object> GetDetail(int id,int str=0)
        {
            var model = await _blogAtricleService.GetBlogDetailOfMapper(id);
            var data = new { success = true, data = model };
            return data;
        }




        ///// <summary>
        ///// 获取博客测试信息 v2版本
        ///// </summary>
        ///// <returns></returns>
        //// [HttpGet]
        // ////MVC自带特性 对 api 进行组管理
        // [ApiExplorerSettings(GroupName = "v1")]
        // //////路径 如果以 / 开头，表示绝对路径，反之相对 controller 的想u地路径
        // [Route("/api/v1/blog/Blogtest")]

        // //和上边的版本控制以及路由地址都是一样的
        // // [CustomRoute(ApiVersions.v2, "Blogtest")]
        // public async Task<object> V2_BlogTest()
        // {
        //     return Ok(new { status = 220, data = "我是第二版的博客信息" });
        // }
        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        public async Task<List<Model.Advertisement>> Get(int id)
        {
            //这里运用了构造注入的方法
           // IServices.IAdvertisementServices services = new Services.AdvertisementServices();
            return await aderService.Query(x => x.Id == id);
        }
        /// <summary>
        /// 测试Model 文字说明
        /// </summary>
        /// <param name="test"></param>
        [HttpPost]
        public void PostTest(Model.Class1 test)
        {

        }
        /// <summary>
        /// Sum接口
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        [HttpGet]
        // [Route("Sum")]
        public  int Sum(int i, int j)
        {
           // IServices.IAdvertisementServices services = new Services.AdvertisementServices();
            return  aderService.sum(i, j);
        }
    }
}