using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.DicDataTypeApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.DicDataTypeApp
{
	public class DicDataTypeAppService : IApplicationService,IDicDataTypeAppService
	{
		private readonly IDicDataTypeRepository _IDicDataTypeRepository;

		/// <summary>
		/// 构造函数
		/// </summary>
		public DicDataTypeAppService(IDicDataTypeRepository iDicDataTypeRepository)
		{
			_IDicDataTypeRepository = iDicDataTypeRepository;
		}

		#region 自动生成
		/// <summary>
		/// 获取所有数据
		/// </summary>
		public async Task<ListResultOutput<DicDataTypeDto>> GetAllList()
		{
			try
			{
                //var query = await _IDicDataTypeRepository.GetAllListAsync();
                var query =  _IDicDataTypeRepository.GetAllList();
                var list = new ListResultOutput<DicDataTypeDto>(query.MapTo<List<DicDataTypeDto>>());
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
		public async Task<DicDataTypeOutputDto> GetDetailById(string id)
		{
			try
			{
				var query = await _IDicDataTypeRepository.GetAsync(id);
				var result = query.MapTo<DicDataTypeOutputDto>();
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
		public async Task<DicDataTypeDto> Insert(DicDataTypeInputDto input)
		{
			try
			{
                input.Id = Guid.NewGuid().ToString();
				DicDataTypeEntity entity = new DicDataTypeEntity
				{
					Id = input.Id,
					TypeName = input.TypeName,
					TypeDesc = input.TypeDesc,
					TypeCode = input.TypeCode,
					TypeSort = input.TypeSort,
					ParentID = input.ParentID,
					Keywords = input.Keywords
				};
				var query = await _IDicDataTypeRepository.InsertAsync(entity);
				var result = entity.MapTo<DicDataTypeDto>();
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
		public async Task<DicDataTypeDto> Update(DicDataTypeInputDto input)
		{
			try
			{
				DicDataTypeEntity entity = new DicDataTypeEntity
				{
					Id = input.Id,
					TypeName = input.TypeName,
					TypeDesc = input.TypeDesc,
					TypeCode = input.TypeCode,
					TypeSort = input.TypeSort,
					ParentID = input.ParentID,
					Keywords = input.Keywords
				};
				var query = await _IDicDataTypeRepository.UpdateAsync(entity);
				var result = entity.MapTo<DicDataTypeDto>();
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
				await _IDicDataTypeRepository.DeleteAsync(id);
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		#endregion
	}
}

