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
    /// 元素类型
    /// </summary>
    [Table("DIC_ELEMENTTYPE")]
    public class ElementType : Entity<string>
    {
        /// <summary>
        /// 图系类别 
        /// </summary>
        [MaxLength(50)]
        public string TXName { get; set; }

        /// <summary>
        /// 图件名称 
        /// </summary>
        [MaxLength(50)]
        public string TJName { get; set; }

        /// <summary>
        /// 图层名称 
        /// </summary>
        [MaxLength(50)]
        public string LayerName { get; set; }

        /// <summary>
        /// 几何特征 
        /// </summary>
        [MaxLength(50)]
        public string JHTZ { get; set; }

        /// <summary>
        /// 图层代码 
        /// </summary>
        [MaxLength(20)]
        public string TCDM { get; set; }
    }
}
