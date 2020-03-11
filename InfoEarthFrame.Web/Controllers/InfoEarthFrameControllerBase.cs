using Abp.Web.Mvc.Controllers;

namespace InfoEarthFrame.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class InfoEarthFrameControllerBase : AbpController
    {
        protected InfoEarthFrameControllerBase()
        {
            LocalizationSourceName = InfoEarthFrameConsts.LocalizationSourceName;
        }
    }
}