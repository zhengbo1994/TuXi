namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class IconInfoDTO
    {
        public IconInfoDTO()
        {
            this.ShowIcon = true;
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 是否加粗,默认false
        /// </summary>
        public bool Bold { get; set; }
        /// <summary>
        /// 字色，默认黑色
        /// </summary>
        public int FontColor { get; set; }
        /// <summary>
        /// 字体，默认宋体
        /// </summary>
        public string FontName { get; set; }
        /// <summary>
        /// 默认14
        /// </summary>
        public float FontSize { get; set; }
        /// <summary>
        /// 图片路径，可以是网络地址或者本地图片路径
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 是否斜体，默认false
        /// </summary>
        public bool Italic { get; set; }
        /// <summary>
        /// 是否显示图标，默认显示
        /// </summary>
        public bool ShowIcon { get; set; }
        /// <summary>
        /// 是否显示文本，默认显示
        /// </summary>
        public bool ShowText { get; set; }
    }
}
