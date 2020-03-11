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
    /// 静态浏览图分类
    /// </summary>
    [Table("DIC_MD_BROWGRAPHTYPE")]
    public class BrowGraphType : Entity<string>
    {

        /// <summary>
        /// 静态浏览图分类名
        /// </summary>
        [MaxLength(255)]
        public string TypeName { get; set; }
    }
}
