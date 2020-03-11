using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.MapMetaDataApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;

namespace InfoEarthFrame.Application.MapMetaDataApp
{
	public class MapMetaDataAppService : IApplicationService,IMapMetaDataAppService
	{
		private readonly IMapMetaDataRepository _IMapMetaDataRepository;

		/// <summary>
		/// 构造函数
		/// </summary>
		public MapMetaDataAppService(IMapMetaDataRepository iMapMetaDataRepository)
		{
			_IMapMetaDataRepository = iMapMetaDataRepository;
		}

		#region 自动生成
		/// <summary>
		/// 获取所有数据
		/// </summary>
		public async Task<ListResultOutput<MapMetaDataDto>> GetAllList()
		{
			try
			{
                //var query = await _IMapMetaDataRepository.GetAllListAsync();
                var query =  _IMapMetaDataRepository.GetAllList();
                var list = new ListResultOutput<MapMetaDataDto>(query.MapTo<List<MapMetaDataDto>>());
				return list;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		/// <summary>
		/// 根据编号获取数据
		/// </summary>
		public async Task<MapMetaDataOutputDto> GetDetailById(string id)
		{
			try
			{
				var query = await _IMapMetaDataRepository.GetAsync(id);
				var result = query.MapTo<MapMetaDataOutputDto>();
				return result;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		/// <summary>
		/// 新增数据
		/// </summary>
		public async Task<MapMetaDataDto> Insert(MapMetaDataInputDto input)
		{
			try
			{
				MapMetaDataEntity entity = new MapMetaDataEntity
				{
					Id = input.Id,
					MapID = input.MapID,
					Version = input.Version,
					Summary = input.Summary,
					Target = input.Target,
					MaintenanceFre = input.MaintenanceFre,
					AdministrativeDivisions = input.AdministrativeDivisions,
					NomalLimit = input.NomalLimit,
					OtherLimit = input.OtherLimit,
					SpatialGeographical = input.SpatialGeographical,
					StartDT = input.StartDT,
					EndDT = input.EndDT,
					AdditionalInfo = input.AdditionalInfo,
					PublishDT = input.PublishDT,
					ModifyDT = input.ModifyDT,
					MetaDataQualityDesc = input.MetaDataQualityDesc,
					ThumbnalAddress = input.ThumbnalAddress,
					MetaDataType = input.MetaDataType,
					MetaDataTag = input.MetaDataTag,
					CreateBy = input.CreateBy,
					Owner = input.Owner,
					IsPublish = input.IsPublish,
					CreateDT = input.CreateDT
				};
				var query = await _IMapMetaDataRepository.InsertAsync(entity);
				var result = entity.MapTo<MapMetaDataDto>();
				return result;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		/// <summary>
		/// 更新数据
		/// </summary>
		public async Task<MapMetaDataDto> Update(MapMetaDataInputDto input)
		{
			try
			{
				MapMetaDataEntity entity = new MapMetaDataEntity
				{
					Id = input.Id,
					MapID = input.MapID,
					Version = input.Version,
					Summary = input.Summary,
					Target = input.Target,
					MaintenanceFre = input.MaintenanceFre,
					AdministrativeDivisions = input.AdministrativeDivisions,
					NomalLimit = input.NomalLimit,
					OtherLimit = input.OtherLimit,
					SpatialGeographical = input.SpatialGeographical,
					StartDT = input.StartDT,
					EndDT = input.EndDT,
					AdditionalInfo = input.AdditionalInfo,
					PublishDT = input.PublishDT,
					ModifyDT = input.ModifyDT,
					MetaDataQualityDesc = input.MetaDataQualityDesc,
					ThumbnalAddress = input.ThumbnalAddress,
					MetaDataType = input.MetaDataType,
					MetaDataTag = input.MetaDataTag,
					CreateBy = input.CreateBy,
					Owner = input.Owner,
					IsPublish = input.IsPublish,
                    //CreateDT = input.CreateDT
				};
				var query = await _IMapMetaDataRepository.UpdateAsync(entity);
				var result = entity.MapTo<MapMetaDataDto>();
				return result;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		/// <summary>
		/// 删除数据
		/// </summary>
		public async Task Delete(string id)
		{
			try
			{
				await _IMapMetaDataRepository.DeleteAsync(id);
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

        /// <summary>
        /// 删除地图元数据
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns></returns>
        public async Task DeleteByMapID(string mapID)
        {
            #region [删除地图元数据]

            var metaData = _IMapMetaDataRepository.GetAll().Where(q => q.MapID == mapID).ToList();

            if (metaData != null && metaData.Count > 0)
            {
                foreach (var item in metaData)
                {
                    await _IMapMetaDataRepository.DeleteAsync(item.Id);
                }
            }

            #endregion
        }

		#endregion
	}
}

