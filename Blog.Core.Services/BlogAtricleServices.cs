using Blog.Core.IServices;
using Blog.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Blog.Core;
using System.Linq;

namespace Blog.Core.Services
{
    /// <summary>
    /// 使用Automapper引入nuget包
    /// AutoMapper和 AutoMapper.Extensions.Microsoft.DependencyInjection
    /// </summary>



    public class BlogAtricleServices : BaseServices<Model.BlogAtricle>, IBlogArticleServices
    {
        IRepository.IBlogArticleRepository iRepository { get; set; }
        AutoMapper.IMapper _iMapper { get; set; }
        public BlogAtricleServices(IRepository.IBlogArticleRepository repository,AutoMapper.IMapper mapper)
        {
            iRepository = repository;
            base.baseDal = repository;
            _iMapper = mapper;
        }
        /// <summary>
        /// 获取博客列表
        /// </summary>
        /// <returns></returns>
      //  [Common.Caching(AbsoluteExpiration = 10)] //增加特性
        public async Task<List<BlogAtricle>> GetBlogs()
        {


            string redisConfiguration = Common.Appsettings.app(new string[] { "AppSettings", "RedisCaching", "ConnectionString" });
            List<BlogAtricle> blogArticleList = new List<BlogAtricle>();
            Common.RedisCacheManager redisCacheManager = new Common.RedisCacheManager();
            if (redisCacheManager.Get<object>("Redis.Blog") != null)
            {
                blogArticleList = redisCacheManager.Get<List<BlogAtricle>>("Redis.Blog");
            }
            else
            {
                blogArticleList = await iRepository.Query(a => a.bId > 0, a => a.bId);
                redisCacheManager.Set("Redis.Blog", blogArticleList, TimeSpan.FromHours(2));//缓存2小时
            }

            //var blogList = await iRepository.Query(a => a.bId > 0, a => a.bId);
            return blogArticleList;
        }
        /// <summary>
        /// 没有使用AutoMapper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BlogViewModels> GetBlogDetail(int id)
        {
            //如果不用automapper时
            var list = await iRepository.Query(a => a.bId > 0, a => a.bId);
            var idMin = list.FirstOrDefault() != null ? list.FirstOrDefault().bId : 0;  //最小的ID
            var idMax = list.FirstOrDefault() != null ? list.LastOrDefault().bId : 1;   //最大的ID

            var idMinShow = id;
            var idMaxShow = id;

            //当前这一条
            BlogAtricle blogAtricle = new BlogAtricle();
            blogAtricle = (await iRepository.Query(a => a.bId == idMinShow)).FirstOrDefault();

            //前一条信息
            BlogAtricle prevBlog = new BlogAtricle();
            while (idMinShow > idMin)
            {
                idMinShow--;
                prevBlog = (await iRepository.Query(a => a.bId == idMinShow)).FirstOrDefault();
                if (prevBlog != null)
                {
                    break;
                }
            }
            //后一条信息
            BlogAtricle nextBlog = new BlogAtricle();
            while (idMaxShow < idMax)
            {
                idMaxShow++;
                nextBlog = (await iRepository.Query(a => a.bId == idMaxShow)).FirstOrDefault();
                if (nextBlog != null)
                {
                    break;
                }
            }
            blogAtricle.bTraffic += 1; //访问量
            await iRepository.Update(blogAtricle, new List<string> { "bTraffic" });  //直接进行更新

            BlogViewModels model = new BlogViewModels()
            {
                bsubmitter = blogAtricle.bsubmitter,
                btitleAuto = blogAtricle.bTitle,
                bcategory = blogAtricle.bCategory,
                bcontent = blogAtricle.bContent,
                btraffic = blogAtricle.bTraffic,
                bcommentNum = blogAtricle.bCommentNum,
                bUpdateTime = blogAtricle.bUpdateTime,
                bCreateTime = blogAtricle.bCreateTime,
                bRemark = blogAtricle.bRemark,
            };

            if(nextBlog!=null)
            {
                model.next = nextBlog.bTitle;
                model.nextID = nextBlog.bId;
            }

            if(prevBlog!=null)
            {
                model.previous = prevBlog.bTitle;
                model.previousID = prevBlog.bId;
            }
            return model;
        }

        public async Task<BlogViewModels> GetBlogDetailOfMapper(int id)
        {
            var blogList =await iRepository.Query(a => a.bId > 0, a => a.bId);
            var blogArticle = (await iRepository.Query(a => a.bId == id)).FirstOrDefault();
            BlogViewModels model=null;
            if(blogArticle!=null)
            {
                BlogAtricle prevBlog;
                BlogAtricle nextBlog;

                int index = blogList.FindIndex(a => a.bId == id);
                if(index>=0)
                {
                    try
                    {
                        prevBlog = index > 0 ? ((BlogAtricle)(blogList[index - 1])) : null;

                        nextBlog = index + 1 < blogList.Count() ?((BlogAtricle) (blogList[index + 1])) : null;
                        //这里进行mapper
                        model = _iMapper.Map<BlogViewModels>(blogArticle);

                        if(prevBlog!=null)
                        {
                            model.previous = prevBlog.bTitle;
                            model.previousID = prevBlog.bId;
                        }
                        if(nextBlog!=null)
                        {
                            model.next = nextBlog.bTitle;
                            model.nextID = nextBlog.bId;
                        }


                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                blogArticle.bTraffic += 1;
                await iRepository.Update(blogArticle, new List<string> { "bTraffic" });                
            }
            return model;
        }
    }
}
