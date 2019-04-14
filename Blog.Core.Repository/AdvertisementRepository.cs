using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Blog.Core.Model;
using Blog.Core.Repository.sugar;
using SqlSugar;

namespace Blog.Core.Repository
{
    public class AdvertisementRepository :BaseRepository<Advertisement>, IRepository.IAdvertisementRepository
    {
        #region 在BaseRepository中都已经实现了具体的方法，然后在实现接口中所特有的
        //private DbContext context;
        //private SqlSugarClient db;
        //private SimpleClient<Advertisement> entityDb;

        //internal SqlSugarClient Db
        //{
        //    get { return db; }
        //    set { db = value; }
        //}

        //public DbContext Context
        //{
        //    get { return context; }
        //    set { context = value; }
        //}

        ////构造函数
        //public AdvertisementRepository() {
        //    DbContext.Init(BaseDBConfig.ConnectionString);   //初始话连接字符串
        //    context = DbContext.GetDbContext();  //得到数据连接对象
        //    db = context.Db;
        //    entityDb = context.GetEntityDB<Advertisement>(db);
        //}
        ///// <summary>
        ///// 添加数据
        ///// </summary>
        ///// <param name="advertisement"></param>
        ///// <returns></returns>
        //public int Add(Advertisement advertisement)
        //{
        //    var i = db.Insertable(advertisement).ExecuteReturnBigIdentity();
        //    return i.ObjToInt();
        //}

        //public bool Delete(Advertisement advertisement)
        //{
        //    var i = db.Deleteable<Advertisement>(advertisement).ExecuteCommand();
        //    return i > 0 ? true : false;
        //}
        ///// <summary>
        ///// 查询接口
        ///// </summary>
        ///// <param name="whereExpression"></param>
        ///// <returns></returns>
        //public List<Advertisement> Query(Expression<Func<Advertisement, bool>> whereExpression)
        //{
        //    return entityDb.GetList(whereExpression);
        //} 

        //public bool Update(Advertisement advertisement)
        //{
        //    var i = db.Updateable<Advertisement>(advertisement).ExecuteCommand();
        //    return i > 0 ? true : false;
        //}
        #endregion

        public int sum(int i, int j)
        {
            return i + j;
        }

        
    }
}
