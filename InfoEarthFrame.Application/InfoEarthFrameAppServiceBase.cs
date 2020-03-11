using Abp.Application.Services;

namespace InfoEarthFrame
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class InfoEarthFrameAppServiceBase : ApplicationService
    {
        protected InfoEarthFrameAppServiceBase()
        {
            LocalizationSourceName = InfoEarthFrameConsts.LocalizationSourceName;
        }
    }
}