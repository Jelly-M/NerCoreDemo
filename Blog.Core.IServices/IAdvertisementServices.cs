using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Core.IServices
{
    //写在内部表示只在内部起作用
    using Model;
    using System.Linq.Expressions;
   public interface IAdvertisementServices:IBaseServices<Advertisement>
    {


        #region 这些在IBaseServices中都已经存在了，只需要写自己所特有的接口即可
        //int Add(Advertisement advertisement);
        //bool Delete(Advertisement advertisement);
        //bool Update(Advertisement advertisement);
        //List<Advertisement> Query(Expression<Func<Advertisement, bool>> whereExpression); 
        #endregion


        //这里写属于自己的接口特性
        /*********************************
         
        **********************************/
        int sum(int i, int j);
    }
}
