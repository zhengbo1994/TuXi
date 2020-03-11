namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    /// <summary>
    /// 指北针
    /// </summary>
    public class NorthArrowInfoDTO
    {
        /// <summary>
        /// 指北针图标路径，如果不指定将为默认样式
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// 指北针图标路径，如果不指定将为默认样式
        /// </summary>
        public string IconPathEx { get; set; }

        /// <summary>
        /// 是否绘制
        /// </summary>
        public bool Visible { get; set; }
    }
}
