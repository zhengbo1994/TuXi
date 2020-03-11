namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class DrawingUpdateInfoDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        public string DrawingID { get; set; }

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
    }
}
