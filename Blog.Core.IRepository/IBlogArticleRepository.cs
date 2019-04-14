using System;
using System.Collections.Generic;
using System.Text;
using Blog.Core;

namespace Blog.Core.IRepository
{
   public interface IBlogArticleRepository:IBaseRepository<Model.BlogAtricle>
    {
        //System.Threading.Tasks.Task<Model.BlogAtricle> GetBlogs();

    }
}
