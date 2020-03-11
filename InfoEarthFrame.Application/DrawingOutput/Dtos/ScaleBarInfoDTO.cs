namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    /// <summary>
    /// 比例尺
    /// </summary>
    public class ScaleBarInfoDTO
    {
        /// <summary>
        /// 字体
        /// </summary>
        public FontSymbolDTO DigitalScaleFontSymbol { get; set; }
        /// <summary>
        /// 线画比例尺注记字体
        /// </summary>
        public FontSymbolDTO LineScaleFontSymbol { get; set; }
        /// <summary>
        /// 用户自定义比例尺样式图片完整路径（样式ID为-1时有效）
        /// </summary>
        public string ScaleBarImgPath { get; set; }
        /// <summary>
        /// 比例尺线参数
        /// </summary>
        public LineSymbolDTO ScaleLineSymbol { get; set; }
        /// <summary>
        /// 比例尺样式ID号,目前只支持0和1和-1，-1为用户自定义图片
        /// </summary>
        public int ScaleStyleID { get; set; }
        /// <summary>
        /// 是否绘制
        /// </summary>
        public bool Visible { get; set; }
    }
}
