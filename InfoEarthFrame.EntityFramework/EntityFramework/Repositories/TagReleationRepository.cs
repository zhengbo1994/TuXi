using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.EntityFramework;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Core.Repositories;

namespace InfoEarthFrame.EntityFramework.Repositories
{
	public class TagReleationRepository : InfoEarthFrameRepositoryBase<TagReleationEntity, string>, ITagReleationRepository
	{
		public TagReleationRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
			:base(dbContextProvider)
		{
		}
	}
}

