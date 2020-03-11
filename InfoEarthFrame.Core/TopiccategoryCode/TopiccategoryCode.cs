using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InfoEarthFrame.Core
{
    /// <summary>
    /// 专题类别
    /// </summary>
    [Table("DIC_MD_TOPICCATEGORYCODE")]
    public class TopiccategoryCode : Entity<string>
    {
        /// <summary>
        /// 一级分类名称
        /// </summary>
        [MaxLength(100)]
        public string TopCategoryName { get; set; }

        /// <summary>
        /// 二级分类名称
        /// </summary>
        [MaxLength(100)]
        public string SecCategoryName { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        [MaxLength(100)]
        public string TopiccatCode { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(150)]
        public string CateoryDesc { get; set; }
    }
}
