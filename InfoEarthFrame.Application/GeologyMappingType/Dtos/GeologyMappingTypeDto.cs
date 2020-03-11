using Abp.Application.Services.Dto;
using System;

namespace InfoEarthFrame.Application
{

    //TODO:去掉EntityDto不知道后期会不会引发BUG
    public class GeologyMappingTypeDto 
        //:EntityDto
    {
        /// <summary>
        /// 序号ID
        /// </summary>
        ///  
        public string Id { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public string ParentID { get; set; }

        public string ParentName { get;set; }


        /// <summary>
        /// 路径
        /// </summary>
        public string Paths { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sn { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ClassName { get; set; }

        public string OldClassName { get; set; }
    }
}