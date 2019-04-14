using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Blog.Core.IRepository;
using Blog.Core.Model;
using Blog.Core.Repository;

namespace Blog.Core.Services
{
    public class AdvertisementServices :BaseServices<Advertisement>, IServices.IAdvertisementServices
    {

        IAdvertisementRepository repository;

        public AdvertisementServices(IAdvertisementRepository repository)
        {
            this.repository = repository;
            this.baseDal = repository;
        }


        #region 这些在base中都已经实现，只需要实现在接口中自己所特有的
        //public int Add(Advertisement advertisement)
        //{
        //    return repository.Add(advertisement);
        //}

        //public bool Delete(Advertisement advertisement)
        //{
        //    return repository.Delete(advertisement);
        //}

        //public List<Advertisement> Query(Expression<Func<Advertisement, bool>> whereExpression)
        //{
        //    return repository.Query(whereExpression);
        //} 
        //public bool Update(Advertisement advertisement)
        //{
        //    return repository.Update(advertisement);
        //}
        #endregion

        public int sum(int i, int j)
        {
            return repository.sum(i, j);
        }

       
    }
}
