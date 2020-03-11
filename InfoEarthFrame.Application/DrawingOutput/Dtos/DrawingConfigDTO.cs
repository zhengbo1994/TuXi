using System.Collections.Generic;

namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class DrawingConfigDTO
    {
        public DrawingConfigDTO()
        {
            Resolution = 300;
            Layers = new List<DrawingLayerDTO>();
            GeoSystem = 0;
        }
        /// <summary>
        /// ID
        /// </summary>
        public string DrawId { get; set; }
        
        /// <summary>
        /// 图层集合
        /// </summary>
        public List<DrawingLayerDTO> Layers { get; set; }

        /// <summary>
        /// 矢量面集合
        /// </summary>
        public List<PolygonInfoDTO> PolygonInfos { get; set; }

        /// <summary>
        /// 标注信息集合
        /// </summary>
        public List<LabelInfoDTO> LabelInfos { get; set; }

        /// <summary>
        /// Icon信息集合
        /// </summary>
        public List<IconInfoDTO> IconInfos { get; set; }

        /// <summary>
        /// 出图名称
        /// </summary>
        public string DrawingName { get; set; }

        /// <summary>
        /// 图符号
        /// </summary>
        public string TFBH { get; set; }

        /// <summary>
        /// 是否是标准图幅
        /// </summary>
        public bool IsBasicTF { get; set; }

        #region 出图范围

        /// <summary>
        /// 东
        /// </summary>
        public double East { get; set; }

        /// <summary>
        ///  北
        /// </summary>
        public double North { get; set; }

        /// <summary>
        /// 南
        /// </summary>
        public double South { get; set; }

        /// <summary>
        /// 西
        /// </summary>
        public double West { get; set; }

        #endregion

        /// <summary>
        /// WKT串
        /// </summary>
        public string WKTString { get; set; }

        public string WKTScale { get; set; }

        /// <summary>
        /// 行政区划Code
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 比例尺
        /// </summary>
        public string Scale { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 责任人信息
        /// </summary>
        public ResponsibilityTableInfomationDTO ResponsibilityTableInfomation { get; set; }

        /// <summary>
        /// 出图分辨率即DPI,单位像素每英寸，默认300
        /// </summary>
        public int Resolution { get; set; }

        /// <summary>
        /// 生成成果文件的坐标系,默认WGS84,0:WGS84,1:GCS_Beijing_1954,2:GCS_Xian_1980
        /// </summary>
        public int GeoSystem { get; set; }

        /// <summary>
        /// 标注是否允许重叠
        /// </summary>
        public bool IsIconOverlay { get; set; }

        /// <summary>
        /// 图例
        /// </summary>
        public LegendsInfomationDTO LengendInfo { get; set; }

        /// <summary>
        /// 指北针
        /// </summary>
        public NorthArrowInfoDTO NorthArrowInfo { get; set; }

        /// <summary>
        /// 比例尺
        /// </summary>
        public ScaleBarInfoDTO ScaleBarInfo { get; set; }
    }
}
