using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Core
{
    //业务模块的多媒体附件类别
    [Table("MultimediaType")]
    public class MultimediaTypeEntity : Entity<string>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(500)]
        public string Name { get; set; }

        /// <summary>
        /// 业务模块的类别
        /// </summary>
        [MaxLength(100)]
        public string ModuleType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

    }
}
