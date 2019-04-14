using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Blog.Core.Model
{
    /// <summary>
    /// 博客文章
    /// </summary>
   public class BlogAtricle
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsNullable =false,IsPrimaryKey =true,IsIdentity =true)]
        public int bId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(IsNullable =true,Length =60)]
        public string bsubmitter { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [SugarColumn(Length =256,IsNullable =true)]
        public string bTitle { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        [SugarColumn(Length =int.MaxValue,IsNullable =true)]
        public string bCategory { get; set; }
        /// <summary>
        /// 内容，对应的是数据库的类型
        /// </summary>
        [SugarColumn(IsNullable =true,ColumnDataType ="text")]
        public string bContent { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int bTraffic { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int bCommentNum { get; set; }

        /// <summary> 
        /// 修改时间
        /// </summary>
        public DateTime bUpdateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public System.DateTime bCreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = int.MaxValue, IsNullable = true)]
        public string bRemark { get; set; }
    }
}
