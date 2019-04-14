using Blog.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.IRepository
{
    using System.Linq.Expressions;  //表达式树的引用
  public  interface IAdvertisementRepository :IBaseRepository<Advertisement>
    {

        int sum(int i,int j);

        //int Add(Advertisement advertisement);
        //bool Delete(Advertisement advertisement);
        //bool Update(Advertisement advertisement);
        //List<Advertisement> Query(Expression<Func<Advertisement, bool>> whereExpression);
    }
}
