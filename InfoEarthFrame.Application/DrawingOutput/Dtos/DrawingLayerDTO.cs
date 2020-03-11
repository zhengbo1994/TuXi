namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class DrawingLayerDTO
    {
        public DrawingLayerDTO()
        {
            ZeroVaule = 36;
            Opacity = 255;
            NumLevels = 15;
            PixelSize = 512;
            LayerType = "iTelluro";
        }

        /// <summary>
        /// 图层地址
        /// </summary>
        public string LayerUrl { get; set; }

        /// <summary>
        /// 图层类型
        /// </summary>
        public string LayerType { get; set; }

        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName { get; set; }

        /// <summary>
        /// 零级大小
        /// </summary>
        public double? ZeroVaule { get; set; }

        /// <summary>
        /// 图层Key
        /// </summary>
        public string LayerEx { get; set; }

        /// <summary>
        /// 级数
        /// </summary>
        public int? NumLevels { get; set; }

        /// <summary>
        /// 透明度
        /// </summary>
        public int? Opacity { get; set; }

        /// <summary>
        /// 切片大小
        /// </summary>
        public int? PixelSize { get; set; }

        /// <summary>
        /// 返回格式
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        ///  请求图层类
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// 切片方式
        /// </summary>
        public string TileMatrixset { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
    }
}
