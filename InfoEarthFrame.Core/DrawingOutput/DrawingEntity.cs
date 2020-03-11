using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.DrawingOutput
{
    /// <summary>
    /// 版 本
    /// Copyright (c) 2013-2018 武汉地大信息股份有限公司
    /// 创建人：liuq
    /// 日 期：2018.09.30
    /// 描 述：制图输出表
    /// </summary>
     [Table("DrawingEntity")]
    public class DrawingEntity : Entity<string>
    {
        #region 实体成员
        /// <summary>
        /// ID
        /// </summary>
        /// <returns></returns>
        [Column("ID")]
        public override string Id { get; set; }
        /// <summary>
        /// 制图输出名称
        /// </summary>
        [Column("DRAWINGNAME")]
        public string DRAWINGNAME { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Column("USERID")]
        public string USERID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Column("USERNAME")]
        public string USERNAME { get; set; }

        /// <summary>
        /// 制图输出所需参数
        /// </summary>
        [Column("PARA")]
        public string PARA { get; set; }

        /// <summary>
        /// 结果路径
        /// </summary>
        [Column("OUTPUTPATH")]
        public string OUTPUTPATH { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column("STAUE")]
        public string STAUE { get; set; }

        [Column("ERRORMSG")]
        public string ERRORMSG { get; set; }

        [Column("JD")]
        public string JD { get; set; }

        [Column("COMPLETE")]
        public string COMPLETE { get; set; }

        [Column("CREATETIME")]
        public DateTime? CREATETIME { get; set; }

        [Column("BZ")]
        public string BZ { get; set; }
        #endregion

    }
}
