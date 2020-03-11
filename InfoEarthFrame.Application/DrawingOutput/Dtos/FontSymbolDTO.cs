namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class FontSymbolDTO
    {
        /// <summary>
        /// 是否加粗
        /// </summary>
        public bool Bold { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public int FontColor { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public string FontColorEx { get; set; }

        /// <summary>
        /// 字体类型,默认为宋体
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public UnitValueDTO FontSize { get; set; }

        /// <summary>
        /// 是否倾斜
        /// </summary>
        public bool Italic { get; set; }

        /// <summary>
        /// 透明度
        /// </summary>
        public int Opacity { get; set; }

        /// <summary>
        /// 是否有下划线
        /// </summary>
        public bool Underline { get; set; }
    }
}
