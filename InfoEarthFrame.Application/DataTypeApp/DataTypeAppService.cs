using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.DataTypeApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;

namespace InfoEarthFrame.Application.DataTypeApp
{
    public class DataTypeAppService : IApplicationService, IDataTypeAppService
    {
        private readonly IDataTypeRepository _IDataTypeRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataTypeAppService(IDataTypeRepository iDataTypeRepository)
        {
            _IDataTypeRepository = iDataTypeRepository;
        }

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public async Task<ListResultOutput<DataTypeDto>> GetAllList()
        {
            try
            {
                //var query = await _IDataTypeRepository.GetAllListAsync();
                var query =  _IDataTypeRepository.GetAllList();
                var list = new ListResultOutput<DataTypeDto>(query.MapTo<List<DataTypeDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取分类管理(通过图层与地图类型区分)
        /// </summary>
        /// <param name="dataType">图层或地图基本代码</param>
        /// <returns></returns>
        public ListResultOutput<DataTypeDto> GetAllListByDataType(string dataType)
        {
            try
            {
                var query = _IDataTypeRepository.GetAll().Where(q => q.DictCodeID == dataType);
                var list = new ListResultOutput<DataTypeDto>(query.MapTo<List<DataTypeDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        public async Task<DataTypeOutputDto> GetDetailById(string id)
        {
            try
            {
                var query = await _IDataTypeRepository.GetAsync(id);
                var result = query.MapTo<DataTypeOutputDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据通过名称获取数据
        /// </summary>
        public ListResultOutput<DataTypeDto> GetDetailByName(string name, string dataType)
        {
            try
            {
                var query = _IDataTypeRepository.GetAll().Where(q => q.TypeName == name && q.DictCodeID == dataType);
                var result = new ListResultOutput<DataTypeDto>(query.MapTo<List<DataTypeDto>>());
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        public async Task<DataTypeDto> Insert(DataTypeInputDto input)
        {
            try
            {
                input.Id = Guid.NewGuid().ToString();
                DataTypeEntity entity = new DataTypeEntity
                {
                    Id = input.Id,
                    TypeName = input.TypeName,
                    TypeDesc = input.TypeDesc,
                    DictCodeID = input.DictCodeID,
                    ParentID = input.ParentID
                };
                var query = await _IDataTypeRepository.InsertAsync(entity);
                var result = entity.MapTo<DataTypeDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task<DataTypeDto> Update(DataTypeInputDto input)
        {
            try
            {
                DataTypeEntity entity = new DataTypeEntity
                {
                    Id = input.Id,
                    TypeName = input.TypeName,
                    TypeDesc = input.TypeDesc,
                    DictCodeID = input.DictCodeID,
                    ParentID = input.ParentID
                };
                var query = await _IDataTypeRepository.UpdateAsync(entity);
                var result = entity.MapTo<DataTypeDto>();
                return result;
            }
            catch (Exception ex)
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
                await _IDataTypeRepository.DeleteAsync(id);
                _IDataTypeRepository.Delete(q => q.ParentID == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}

