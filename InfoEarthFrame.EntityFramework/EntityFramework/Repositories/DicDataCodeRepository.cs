using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.EntityFramework;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Core.Repositories;

namespace InfoEarthFrame.EntityFramework.Repositories
{
	public class DicDataCodeRepository : InfoEarthFrameRepositoryBase<DicDataCodeEntity, string>, IDicDataCodeRepository
	{
		public DicDataCodeRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
			:base(dbContextProvider)
		{
		}
	}
}

