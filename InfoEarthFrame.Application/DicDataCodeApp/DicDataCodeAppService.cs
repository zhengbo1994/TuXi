using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.DicDataCodeApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;

namespace InfoEarthFrame.Application.DicDataCodeApp
{
	public class DicDataCodeAppService : IApplicationService,IDicDataCodeAppService
	{
		private readonly IDicDataCodeRepository _IDicDataCodeRepository;

		/// <summary>
		/// 构造函数
		/// </summary>
		public DicDataCodeAppService(IDicDataCodeRepository iDicDataCodeRepository)
		{
			_IDicDataCodeRepository = iDicDataCodeRepository;
		}

		#region [查询]

		/// <summary>
		/// 获取所有数据
		/// </summary>
		public async Task<ListResultOutput<DicDataCodeDto>> GetAllList()
		{
			try
			{
                //var query = await _IDicDataCodeRepository.GetAllListAsync();
                var query =  _IDicDataCodeRepository.GetAllList();
                var list = new ListResultOutput<DicDataCodeDto>(query.MapTo<List<DicDataCodeDto>>());
				return list;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

        /// <summary>
        /// 根据关键字与类型查询
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public ListResultOutput<DicDataCodeDto> GetDetailByConn(string typeID, string keyWord)
        {
            try
            {
                var query = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == typeID && q.Keywords == keyWord).ToList();
                var list = new ListResultOutput<DicDataCodeDto>(query.MapTo<List<DicDataCodeDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 通过类别获取所有数据
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public ListResultOutput<DicDataCodeDto> GetDetailByTypeID(string typeID)
        {
            try
            {
                var query = _IDicDataCodeRepository.GetAll().Where(q => q.DataTypeID == typeID).ToList();
                var list = new ListResultOutput<DicDataCodeDto>(query.MapTo<List<DicDataCodeDto>>());
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
		public async Task<DicDataCodeOutputDto> GetDetailById(string id)
		{
			try
			{
				var query = await _IDicDataCodeRepository.GetAsync(id);
				var result = query.MapTo<DicDataCodeOutputDto>();
				return result;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

        #endregion

        #region [操作]

        /// <summary>
		/// 新增数据
		/// </summary>
		public async Task<DicDataCodeDto> Insert(DicDataCodeInputDto input)
		{
			try
			{
                input.Id = Guid.NewGuid().ToString();
				DicDataCodeEntity entity = new DicDataCodeEntity
				{
					Id = input.Id,
					CodeName = input.CodeName,
					CodeValue = input.CodeValue,
					CodeDesc = input.CodeDesc,
					DataTypeID = input.DataTypeID,
					CodeSort = input.CodeSort,
					Remark = input.Remark,
					Keywords = input.Keywords
				};
				var query = await _IDicDataCodeRepository.InsertAsync(entity);
				var result = entity.MapTo<DicDataCodeDto>();
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
		public async Task<DicDataCodeDto> Update(DicDataCodeInputDto input)
		{
			try
			{
				DicDataCodeEntity entity = new DicDataCodeEntity
				{
					Id = input.Id,
					CodeName = input.CodeName,
					CodeValue = input.CodeValue,
					CodeDesc = input.CodeDesc,
					DataTypeID = input.DataTypeID,
					CodeSort = input.CodeSort,
					Remark = input.Remark,
					Keywords = input.Keywords
				};
				var query = await _IDicDataCodeRepository.UpdateAsync(entity);
				var result = entity.MapTo<DicDataCodeDto>();
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
				await _IDicDataCodeRepository.DeleteAsync(id);
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		#endregion
	}
}

