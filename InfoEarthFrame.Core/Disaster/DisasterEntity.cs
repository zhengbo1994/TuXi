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
    [Table("Disaster")]
    public class DisasterEntity : Entity, IAreaRight, ISoftDelete, IMustHaveTenant
    {
        /// <summary>
        /// 灾害点名称
        /// </summary>
        [MaxLength(100)]
        public string DISASTERNAME { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>

        public DateTime? OCCURTIME { get; set; }

        /// <summary>
        /// 具体位置
        /// </summary>
        [MaxLength(200)]
        public string POSITION { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string REMARK { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
        [MaxLength(12)]
        public string AREARIGHTCODE
        {
            get;
            set;
        }

        public bool IsDeleted
        {
            get;
            set;
        }

        public int TenantId
        { get; set; }



    } 
}
