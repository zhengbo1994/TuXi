using System;

namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class DrawingDTO
    {
        public string ID { get; set; }

        /// <summary>
        /// 制图输出名称
        /// </summary>
        public string DRAWINGNAME { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string USERID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string USERNAME { get; set; }

        /// <summary>
        /// 制图输出所需参数
        /// </summary>
       
        public string PARA { get; set; }

        /// <summary>
        /// 结果路径
        /// </summary>
        public string OUTPUTPATH { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string STAUE { get; set; }

        public string ERRORMSG { get; set; }

        public string JD { get; set; }

        public string COMPLETE { get; set; }

        public DateTime? CREATETIME { get; set; }

        public string BZ { get; set; }
    }
}
