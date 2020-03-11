using System.Reflection;
using Abp.Modules;
using System.Transactions;

namespace InfoEarthFrame
{
    public class InfoEarthFrameCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            //是否启用区域参数过滤
            string AreaRightFilter = System.Configuration.ConfigurationSettings.AppSettings["AreaRightFilter"];
            if (AreaRightFilter.ToUpper()=="TRUE")
            {
                Configuration.UnitOfWork.RegisterFilter("AreaRightFilter", true);
            }
            else
            {
                Configuration.UnitOfWork.RegisterFilter("AreaRightFilter", false);
            }

            //Configuration.UnitOfWork.Scope = TransactionScopeOption.Suppress;
            //Configuration.UnitOfWork.IsolationLevel = IsolationLevel.RepeatableRead;
            
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
