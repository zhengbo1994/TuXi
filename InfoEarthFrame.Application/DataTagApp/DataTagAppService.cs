using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.DataTagApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;

namespace InfoEarthFrame.Application.DataTagApp
{
    public class DataTagAppService : IApplicationService, IDataTagAppService
    {
        private readonly IDataTagRepository _IDataTagRepository;
        private readonly ITagReleationRepository _ITagReleationRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataTagAppService(IDataTagRepository iDataTagRepository, ITagReleationRepository iTagReleationRepository)
        {
            _IDataTagRepository = iDataTagRepository;
            _ITagReleationRepository = iTagReleationRepository;
        }

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public async Task<ListResultOutput<DataTagDto>> GetAllList()
        {
            try
            {
                //var query = await _IDataTagRepository.GetAllListAsync();
                var query = _IDataTagRepository.GetAllList();
                var list = new ListResultOutput<DataTagDto>(query.MapTo<List<DataTagDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据基本代码获取数据
        /// </summary>
        public ListResultOutput<DataTagDto> GetAllListByDataType(string dataType)
        {
            try
            {
                List<string> listTags = new List<string>();
                //地图标签
                //if (dataType == "ba5ab799-6acd-11e7-87f3-005056bb1c7e")
                //{
                //    var listTag = _IMapRepository.GetAll().Where(t => !string.IsNullOrEmpty(t.MapTag)).Select(t => t.MapTag).ToList();
                //    listTag.ForEach(t =>
                //    {
                //        listTags.AddRange(t.Split(','));
                //    });
                //    listTags = listTags.Distinct().ToList();
                //}
                ////图层标签
                //else if (dataType == "a2faae61-6acd-11e7-87f3-005056bb1c7e")
                //{
                //    var listTag = _ILayerContentRepository.GetAll().Where(t => !string.IsNullOrEmpty(t.LayerTag)).Select(t => t.LayerTag).ToList();
                //    listTag.ForEach(t =>
                //    {
                //        listTags.AddRange(t.Split(','));
                //    });
                //    listTags = listTags.Distinct().ToList();
                //}

                var listTagReleation = _ITagReleationRepository.GetAll().Select(t=> t.DataTagID).ToList();
                listTagReleation.ForEach(t =>
                {
                    listTags.AddRange(t.Split(','));
                });
                listTags = listTags.Distinct().ToList();

                var query = _IDataTagRepository.GetAll().Where(q => q.DictCodeID == dataType && listTags.Contains(q.Id));
                var list = new ListResultOutput<DataTagDto>(query.MapTo<List<DataTagDto>>());
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
        public async Task<DataTagOutputDto> GetDetailById(string id)
        {
            try
            {
                var query = await _IDataTagRepository.GetAsync(id);
                var result = query.MapTo<DataTagOutputDto>();
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
        public async Task<DataTagDto> Insert(DataTagInputDto input)
        {
            try
            {
                input.Id = Guid.NewGuid().ToString();
                DataTagEntity entity = new DataTagEntity
                {
                    Id = input.Id,
                    TagName = input.TagName,
                    TagDesc = input.TagDesc,
                    DictCodeID = input.DictCodeID
                };
                var query = await _IDataTagRepository.InsertAsync(entity);
                var result = entity.MapTo<DataTagDto>();
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
        public async Task<DataTagDto> Update(DataTagInputDto input)
        {
            try
            {
                DataTagEntity entity = new DataTagEntity
                {
                    Id = input.Id,
                    TagName = input.TagName,
                    TagDesc = input.TagDesc,
                    DictCodeID = input.DictCodeID
                };
                var query = await _IDataTagRepository.UpdateAsync(entity);
                var result = entity.MapTo<DataTagDto>();
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
                await _IDataTagRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}

