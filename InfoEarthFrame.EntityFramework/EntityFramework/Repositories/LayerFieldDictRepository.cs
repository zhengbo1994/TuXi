using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.EntityFramework;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Core.Repositories;

namespace InfoEarthFrame.EntityFramework.Repositories
{
	public class LayerFieldDictRepository : InfoEarthFrameRepositoryBase<LayerFieldDictEntity, string>, ILayerFieldDictRepository
	{
		public LayerFieldDictRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
			:base(dbContextProvider)
		{
		}
	}
}

