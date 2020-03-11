using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common.Style
{
    public class PolygonStyle
    {
        /// <summary>
        /// 样式类型，1点，2线，3面
        /// </summary>
        public string Type;

        //点样式
        public bool IsIcon = false;
        public string IconPath;
        public string IconSize;
        public string IconRotation;
        public string PointType; //圆形，三角形，方形

        public bool IsOutline = false;
        public double? LineWidth ;
        public string OutlineColor ;
        public double? LineTransparency;
        //线型
        public int LineType;//实线1，虚线2, 点线3， 虚点线 4

        public bool IsFill = false;
        public double? FillTransparency ;
        public string PolygonColor ;
    }

}
