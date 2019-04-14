using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.IServices
{
    public interface IBlogArticleServices : IBaseServices<Model.BlogAtricle>
    {
        System.Threading.Tasks.Task<List<Model.BlogAtricle>> GetBlogs();

        System.Threading.Tasks.Task<Model.BlogViewModels> GetBlogDetail(int id);
        System.Threading.Tasks.Task<Model.BlogViewModels> GetBlogDetailOfMapper(int id);
    }
}
