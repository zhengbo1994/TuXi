using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Repositories;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Core.Repositories
{
	public interface ILayerFieldDictRepository : IRepository<LayerFieldDictEntity, string>
	{
	}
}

