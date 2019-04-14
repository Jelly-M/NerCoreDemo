using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Model;

namespace Blog.Core.Repository
{
    public class BlogArticleRepository : BaseRepository<Model.BlogAtricle>, Blog.Core.IRepository.IBlogArticleRepository
    {
        
    }
}
