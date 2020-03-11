using System;
using System.Data.Entity;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.EntityFramework;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using EntityFramework.DynamicFilters;
using InfoEarthFrame.Core;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.DrawingOutput;
using System.Data.Entity.Validation;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace InfoEarthFrame.EntityFramework
{
    public class InfoEarthFrameDbContext : AbpDbContext
    {
        //TODO: Define an IDbSet for each Entity...

        //Example:
        //public virtual IDbSet<User> Users { get; set; }

        public virtual IDbSet<Log> Log { get; set; }

        public virtual IDbSet<ConvertFile> ConvertFile { get; set; }

        public virtual IDbSet<MultimediaTypeEntity> MultimediaType { get; set; }
        public virtual IDbSet<AttachmentEntity> Attachment { get; set; }

        public virtual IDbSet<DataStyleEntity> DataStyle { get; set; }

        public virtual IDbSet<DataTagEntity> DataTag { get; set; }

        public virtual IDbSet<DataTypeEntity> DataType { get; set; }

        public virtual IDbSet<DicDataCodeEntity> DicDataCode { get; set; }

        public virtual IDbSet<DicDataTypeEntity> DicDataType { get; set; }

        public virtual IDbSet<LayerContentEntity> LayerContent { get; set; }

        public virtual IDbSet<LayerFieldEntity> LayerField { get; set; }

        public virtual IDbSet<MapEntity> Map { get; set; }

        public virtual IDbSet<MapMetaDataEntity> MapMetaData { get; set; }

        public virtual IDbSet<MapReleationEntity> MapReleation { get; set; }

        public virtual IDbSet<TagReleationEntity> TagReleation { get; set; }

        public virtual IDbSet<ShpFileReadLogEntity> ShpFileReadLog { get; set; }

        public virtual IDbSet<SystemUserEntity> SysUser { get; set; }

        public virtual IDbSet<LayerFieldDictEntity> LayerFieldDict { get; set; }

        public virtual IDbSet<OperateLogEntity> OperateLog { get; set; }

        public virtual IDbSet<GeologyMappingType> GeologyMappingType { get; set; }


        public virtual IDbSet<MetaData> MetaData { get; set; }
        public virtual IDbSet<DataMain> DataMain { get; set; }

        public virtual IDbSet<DataManageFile> DataManageFile { get; set; }

        public virtual IDbSet<TopiccategoryCode> objTopiccategoryCode { get; set; }
        public virtual IDbSet<DrawingEntity> DrawingEntities { get; set; }

        public virtual IDbSet<Tbl_LayerManager> LayerManagers { get; set; }

        public virtual IDbSet<GroupEntity> GroupEntities { get; set; }

        public virtual IDbSet<GroupRightEntity> GroupRightEntities { get; set; }

        public virtual IDbSet<GroupUserEntity> GroupUserEntities { get; set; }
        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public InfoEarthFrameDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in InfoEarthFrameDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of InfoEarthFrameDbContext since ABP automatically handles it.
         */
        public InfoEarthFrameDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            //Database.Initialize(false);
            //this.SetFilterScopedParameterValue(AbpDataFilters.MustHaveTenant, AbpDataFilters.Parameters.TenantId, AbpSession.TenantId ?? 0);
            //this.SetFilterScopedParameterValue(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, AbpSession.TenantId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Oracle数据库请加上这个sql和mysql请去掉这是schema不是表空间
            //string OracleSchame = System.Configuration.ConfigurationSettings.AppSettings["OracleSchame"];
            //modelBuilder.HasDefaultSchema(OracleSchame);

            modelBuilder.HasDefaultSchema("public");

            //区域数据过滤
            string areaCode = "1";// UserInfo.areaRight;
            string code = string.Empty;
            if (areaCode != null && areaCode.Length > 0)
            {
                string[] arrStrings = areaCode.Split(',');
                //组装表达式
                Expression<Func<IAreaRight, bool>> predicate = null;
                foreach (var arrString in arrStrings)
                {
                    predicate = p => p.AREARIGHTCODE == arrString;
                    predicate = predicate.Or(predicate);
                }
                modelBuilder.Filter("AreaRightFilter", (IAreaRight t) => predicate, "");
            }


        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException exception)
            {
                var errorMessages =
                    exception.EntityValidationErrors
                        .SelectMany(validationResult => validationResult.ValidationErrors)
                        .Select(m => m.ErrorMessage);

                var fullErrorMessage = string.Join(", ", errorMessages);
                //记录日志
                //Log.Error(fullErrorMessage);
                var exceptionMessage = string.Concat(exception.Message, " 验证异常消息是：", fullErrorMessage);

                throw new DbEntityValidationException(exceptionMessage, exception.EntityValidationErrors);
            }

            //其他异常throw到上层
        }
    }
}
