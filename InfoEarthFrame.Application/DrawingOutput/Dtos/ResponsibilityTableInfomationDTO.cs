namespace Infoearth.Application.Entity.DrawingOutput.Dtos
{
    /// <summary>
    /// 责任表
    /// </summary>
    public class ResponsibilityTableInfomationDTO
    {
        public ResponsibilityTableInfomationDTO()
        {
            Visible = true;
        }

        /// <summary>
        /// 审核员
        /// </summary>
        public string Assessor { get; set; }

        /// <summary>
        /// 总工程师
        /// </summary>
        public string ChiefEngineer { get; set; }
        /// <summary>
        /// 单位负责人
        /// </summary>
        public string CompanyPrincipal { get; set; }

        /// <summary>
        /// 资料来源
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 制图日期
        /// </summary>
        public string DrawingDate { get; set; }

        /// <summary>
        ///  制图
        /// </summary>
        public string DrawingMan { get; set; }

        /// <summary>
        /// 编图
        /// </summary>
        public string DrawingWeaver { get; set; }

        /// <summary>
        /// 审核员label,可自行编辑
        /// </summary>
        public string LblAssessor { get; set; }
        /// <summary>
        /// 总工程师label,可自行编辑
        /// </summary>
        public string LblChiefEngineer { get; set; }

        /// <summary>
        /// 单位负责人label,可自行编辑
        /// </summary>
        public string LblCompanyPrincipal { get; set; }

        /// <summary>
        ///  资料来源label,可自行编辑
        /// </summary>
        public string LblDataSource { get; set; }

        /// <summary>
        /// 制图日期label,可自行编辑
        /// </summary>
        public string LblDrawingDate { get; set; }

        /// <summary>
        /// 制图label,可自行编辑
        /// </summary>
        public string LblDrawingMan { get; set; }

        /// <summary>
        /// 编图label,可自行编辑
        /// </summary>
        public string LblDrawingWeaver { get; set; }

        /// <summary>
        ///  图号label,可自行编辑
        /// </summary>
        public string LblMapCode { get; set; }

        /// <summary>
        ///  图名label,可自行编辑
        /// </summary>
        public string LblMapName { get; set; }

        /// <summary>
        ///  项目负责人label,可自行编辑
        /// </summary>
        public string LblProjectPrincipal { get; set; }

        /// <summary>
        /// 项目承担单位label,可自行编辑
        /// </summary>
        public string LblResponsibleCompany { get; set; }

        /// <summary>
        /// 顺序号label,可自行编辑
        /// </summary>
        public string LblSequentialCode { get; set; }
       /// <summary>
        ///  图号
       /// </summary>
        public string MapCode { get; set; }

        /// <summary>
        /// 图名
        /// </summary>
        public string MapName { get; set; }

        /// <summary>
        ///  项目负责人
        /// </summary>
        public string ProjectPrincipal { get; set; }

        /// <summary>
        /// 项目承担单位
        /// </summary>
        public string ResponsibleCompany { get; set; }

        /// <summary>
        /// 顺序号
        /// </summary>
        public string SequentialCode { get; set; }

        public bool Visible { get; set; }
    }
}
