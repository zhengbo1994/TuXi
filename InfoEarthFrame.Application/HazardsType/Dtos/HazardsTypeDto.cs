using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public class HazardsTypeDto 
    {
       public string ID { get; set; }

       /// <summary>
       /// 灾害体类型
       /// </summary>
       [StringLength(36)]
       public string HAZARDSTYPE { get; set; }

       /// <summary>
       /// 表名
       /// </summary>
       [StringLength(36)]
       public string TABNAME { get; set; }

       /// <summary>
       /// CGHmdb系统灾害体类型ID
       /// </summary>
       /// 
      [StringLength(36)]
       public string CGHID { get; set; }
    }
}
