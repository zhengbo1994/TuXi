namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class PolygonInfoDTO
    {
        public PolygonInfoDTO()
        {
            this.Fill = true;
            this.Outline = true;
        }
        public string PolyWKT { get; set; }

        public bool Fill { get; set; }

        public int FillColor { get; set; }

        public string FillColorExt { get; set; }

        public int LineColor { get; set; }

        public string LineColorExt { get; set; }

        public float LineWidth { get; set; }

        public bool Outline { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string PolyType { get; set; }

        /// <summary>
        /// 中心点
        /// </summary>
        public Point Center { get; set; }

        /// <summary>
        /// 半径
        /// </summary>
        public double Radius { get; set; }
    }

    public class Point
    {
        public double X { get; set; }

        public double Y { get; set; }
    }
}
