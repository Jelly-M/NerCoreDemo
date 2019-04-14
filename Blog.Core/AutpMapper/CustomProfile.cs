using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Core.Model;

namespace Blog.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomProfile:Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public CustomProfile()
        {
            CreateMap<BlogAtricle,BlogViewModels>().ForMember(c=>c.btitleAuto,x=>x.MapFrom(o=>o.bTitle));
        }
    }
}
