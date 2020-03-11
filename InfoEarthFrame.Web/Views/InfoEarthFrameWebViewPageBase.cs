using Abp.Web.Mvc.Views;

namespace InfoEarthFrame.Web.Views
{
    public abstract class InfoEarthFrameWebViewPageBase : InfoEarthFrameWebViewPageBase<dynamic>
    {

    }

    public abstract class InfoEarthFrameWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected InfoEarthFrameWebViewPageBase()
        {
            LocalizationSourceName = InfoEarthFrameConsts.LocalizationSourceName;
        }
    }
}