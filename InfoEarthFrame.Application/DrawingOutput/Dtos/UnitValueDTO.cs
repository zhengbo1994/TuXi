namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    public class UnitValueDTO
    {
        /// <summary>
        /// 出图分别率计算之后的像素值
        /// </summary>
        public float PixcelValue { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public int Unit { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public float Value { get; set; }
    }
}
