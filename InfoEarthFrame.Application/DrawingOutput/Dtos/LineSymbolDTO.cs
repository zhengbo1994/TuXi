namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class LineSymbolDTO
    {
        /// <summary>
        /// 颜色
        /// </summary>
        public int LineColor { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string LineColorEx { get; set; }

        /// <summary>
        /// 透明度
        /// </summary>
        public int Opacity { get; set; }

        /// <summary>
        /// 线宽
        /// </summary>
        public UnitValueDTO Width { get; set; }
    }
}
