using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Runtime.Session;
using System.Security.Claims;
using System.Threading;
using Abp.Runtime.Security;
using Abp.MultiTenancy;

namespace InfoEarthFrame.Web
{
    public class SessionConfig :  Abp.Configuration.Startup.IMultiTenancyConfig
    {
        public bool IsEnabled { get; set; }
    }
    public class Sessions : IAbpSession
    {
        public virtual long? UserId
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                if (claimsIdentity == null)
                {
                    return null;
                }

                var userIdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "given_name");
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return null;
                }

                long userId;
                if (!long.TryParse(userIdClaim.Value, out userId))
                {
                    return null;
                }

                return userId;
            }
        }

        public virtual int? TenantId
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var tenantIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "given_name");
                if (tenantIdClaim == null || string.IsNullOrEmpty(tenantIdClaim.Value))
                {
                    return null;
                }

                return Convert.ToInt32(tenantIdClaim.Value);
            }
        }

        public virtual MultiTenancySides MultiTenancySide
        {
            get
            {
                return !TenantId.HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }
        }

        public virtual long? ImpersonatorUserId
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var impersonatorUserIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "given_name");
                if (impersonatorUserIdClaim == null || string.IsNullOrEmpty(impersonatorUserIdClaim.Value))
                {
                    return null;
                }

                return Convert.ToInt64(impersonatorUserIdClaim.Value);
            }
        }

        public virtual int? ImpersonatorTenantId
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var impersonatorTenantIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "given_name");
                if (impersonatorTenantIdClaim == null || string.IsNullOrEmpty(impersonatorTenantIdClaim.Value))
                {
                    return null;
                }

                return Convert.ToInt32(impersonatorTenantIdClaim.Value);
            }
        }


        public void Index()
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            var tenantIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "given_name");
            string str=tenantIdClaim.Value;            
        }
    }
}