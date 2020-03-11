using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace InfoEarthFrame.Core.Entities
{
    /// <summary>
    /// 实体类MapMetaData
    /// </summary>
    [Table("sdms_metadata")]
    public class MapMetaDataEntity : Entity<string>
    {
        //与数据库字段对应

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(36)]
        public string MapID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(36)]
        public string Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(100)]
        public string Summary { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(100)]
        public string Target { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(36)]
        public string MaintenanceFre { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(200)]
        public string AdministrativeDivisions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(50)]
        public string NomalLimit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(100)]
        public string OtherLimit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(200)]
        public string SpatialGeographical { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(200)]
        public string AdditionalInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PublishDT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModifyDT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(200)]
        public string MetaDataQualityDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(100)]
        public string ThumbnalAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(100)]
        public string MetaDataType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(200)]
        public string MetaDataTag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(36)]
        public string CreateBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(36)]
        public string Owner { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? IsPublish { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDT { get; set; }
    }
}