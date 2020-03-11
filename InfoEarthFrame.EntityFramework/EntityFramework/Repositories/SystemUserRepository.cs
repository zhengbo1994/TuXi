using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.EntityFramework;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Core.Repositories;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class SystemUserRepository : InfoEarthFrameRepositoryBase<SystemUserEntity, string>, ISystemUserRepository
	{
		public SystemUserRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
			:base(dbContextProvider)
		{
		}
	}
}