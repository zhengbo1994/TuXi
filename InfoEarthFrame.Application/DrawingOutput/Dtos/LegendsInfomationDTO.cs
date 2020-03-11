using System.Collections.Generic;

namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    /// <summary>
    /// 图例
    /// </summary>
    public class LegendsInfomationDTO
    {
        /// <summary>
        /// 列优先
        /// </summary>
        public bool ColFirst { get; set; }
        /// <summary>
        /// 列数
        /// </summary>
        public int ColNum { get; set; }
        /// <summary>
        /// 字体信息
        /// </summary>
        public FontSymbolDTO FontSymbol { get; set; }
        /// <summary>
        /// 图例边框样式
        /// </summary>
        public LineSymbolDTO FrameLineSymbol { get; set; }
        /// <summary>
        ///  图例宽
        /// </summary>
        public UnitValueDTO Height { get; set; }
        /// <summary>
        /// 标注位置( 左 = 0, 右 = 1,上 = 2,下 = 3,)
        /// </summary>
        public int LabelPosition { get; set; }
        /// <summary>
        /// 是否显示图例注记
        /// </summary>
        public bool LabelVisible { get; set; }
        /// <summary>
        /// 注记的宽度
        /// </summary>
        public UnitValueDTO LabelWidth { get; set; }
        /// <summary>
        /// 图例
        /// </summary>
        public List<LegendDTO> Legends { get; set; }
        /// <summary>
        /// 间距
        /// </summary>
        public UnitValueDTO Margin { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        public int RowNum { get; set; }

        /// <summary>
        /// SuggestFonts2仅用于替代SuggestFonts序列化
        /// </summary>
        public List<string> SuggestFonts2 { get; set; }

        /// <summary>
        /// 是否绘制图例图片
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 图例高
        /// </summary>
        public UnitValueDTO Width { get; set; }
    }
}
