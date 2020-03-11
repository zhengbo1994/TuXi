using System.Data.Entity;
using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using InfoEarthFrame.EntityFramework;

namespace InfoEarthFrame
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(InfoEarthFrameCoreModule))]
    public class InfoEarthFrameDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Database.SetInitializer<InfoEarthFrameDbContext>(null);
        }
    }
}
