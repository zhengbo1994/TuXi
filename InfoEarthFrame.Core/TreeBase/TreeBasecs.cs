using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InfoEarthFrame.Core
{
    public abstract class TreeBase : Entity<string>
    {
        /// <summary>
        /// PID
        /// </summary>
        [MaxLength(100)]
        public string ParentID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(150)]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sn { get; set; }
    }
}
