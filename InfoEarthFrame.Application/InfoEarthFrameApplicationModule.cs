using System.Reflection;
using Abp.Modules;

namespace InfoEarthFrame
{
    [DependsOn(typeof(InfoEarthFrameCoreModule))]
    public class InfoEarthFrameApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            DtoMappings.Map();
        }
    }
}
