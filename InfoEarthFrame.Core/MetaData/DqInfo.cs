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
    /// 数据质量信息
    /// </summary>
    [Table("TBL_MD_DQINFO")]
    public class DqInfo : Entity<string>
    {
        /// <summary>
        /// 所属元数据ID
        /// </summary>
        [MaxLength(50)]
        public string MetaDataID { get; set; }

        /// <summary>
        /// 处理步骤说明
        /// </summary>
        [MaxLength(200)]
        public string stepDesc { get; set; }

        /// <summary>
        /// 数据源说明
        /// </summary>
        [MaxLength(100)]
        public string srcDesc { get; set; }

        /// <summary>
        /// 数据源比例尺分母
        /// </summary>
        [MaxLength(50)]
        public string srcScale { get; set; }

        /// <summary>
        /// 数据源参考系
        /// </summary>
        [MaxLength(50)]
        public string srcDatum { get; set; }


        /// <summary>
        /// 引用名称
        /// </summary>
        [MaxLength(50)]
        public string resTitle { get; set; }

        /// <summary>
        /// 引用日期
        /// </summary>
        public DateTime? resDate { get; set; }

        /// <summary>
        /// 引用资料的负责单位
        /// </summary>
        [MaxLength(50)]
        public string citRespParty { get; set; }

        /// <summary>
        /// 表达形式
        /// </summary>
        [MaxLength(50)]
        public string PersForm { get; set; }

        /// <summary>
        /// 数据质量说明
        /// </summary>
        [MaxLength(50)]
        public string dqDescription { get; set; }

        /// <summary>
        /// 验收说明
        /// </summary>
        [MaxLength(50)]
        public string dqFnlChcDesc { get; set; }

        /// <summary>
        /// 图件输出质量
        /// </summary>
        [MaxLength(50)]
        public string dqOutputDesc { get; set; }

        /// <summary>
        /// 附件质量
        /// </summary>
        [MaxLength(50)]
        public string dqDocuDesc { get; set; }

        /// <summary>
        /// 完整形
        /// </summary>
        [MaxLength(50)]
        public string dqComplete { get; set; }

        /// <summary>
        /// 逻辑一致性
        /// </summary>
        [MaxLength(50)]
        public string dqLogConsis { get; set; }

        /// <summary>
        /// 准确度
        /// </summary>
        [MaxLength(50)]
        public string dqAcc { get; set; }
    }
}
