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
    /// 日志
    /// </summary>
    [Table("sdms_log")]
    public class Log : Entity<string>
    {
        /// <summary>
        /// 类型
        /// </summary>
        public int LogType { get; set; }

        /// <summary>
        /// key
        /// </summary>
        [MaxLength(100)]
        public string LogKey { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [MaxLength(100)]
        public string LogValue { get; set; } 

        /// <summary>
        /// 内容
        /// </summary>
        [MaxLength(500)]
        public string  LogContent { get; set; }
        
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 其它1
        /// </summary>
        [MaxLength(100)]
        public string Other1 { get; set; }

        /// <summary>
        /// 其它2
        /// </summary>
        [MaxLength(100)]
        public string Other2 { get; set; }




    }


}
