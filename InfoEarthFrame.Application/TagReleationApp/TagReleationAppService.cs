using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.TagReleationApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;
using InfoEarthFrame.TagReleationApp.Dtos;

namespace InfoEarthFrame.Application.TagReleationApp
{
    public class TagReleationAppService : ApplicationService, ITagReleationAppService
    {
        private readonly ITagReleationRepository _ITagReleationRepository;
        private readonly IDataTagRepository _IDataTagRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TagReleationAppService(ITagReleationRepository iTagReleationRepository, IDataTagRepository iDataTagRepository)
        {
            _ITagReleationRepository = iTagReleationRepository;
            _IDataTagRepository = iDataTagRepository;
        }

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public async Task<ListResultOutput<TagReleationDto>> GetAllList()
        {
            try
            {
                //var query = await _ITagReleationRepository.GetAllListAsync();
                var query =  _ITagReleationRepository.GetAllList();
                var list = new ListResultOutput<TagReleationDto>(query.MapTo<List<TagReleationDto>>());
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
        public async Task<TagReleationOutputDto> GetDetailById(string id)
        {
            try
            {
                var query = await _ITagReleationRepository.GetAsync(id);
                var result = query.MapTo<TagReleationOutputDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取地图的所有标签描述
        /// </summary>
        /// <param name="mapLayerID"></param>
        /// <returns></returns>
        public string GetMultiTagNameByMapID(string mapLayerID)
        {
            try
            {
                string tagName = "";
                var query = _ITagReleationRepository.GetAllList().Where(q => q.MapID == mapLayerID).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        var tag = _IDataTagRepository.Get(item.DataTagID);
                        tagName += tag.TagName + ",";
                    }
                }
                if (tagName.Length > 0)
                {
                    tagName = tagName.Substring(0, tagName.Length - 1);
                }
                return tagName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        public async Task<TagReleationDto> Insert(TagReleationInputDto input)
        {
            try
            {
                input.Id = Guid.NewGuid().ToString();
                TagReleationEntity entity = new TagReleationEntity
                {
                    Id = input.Id,
                    DataTagID = input.DataTagID,
                    MapID = input.MapID
                };
                var query = await _ITagReleationRepository.InsertAsync(entity);
                var result = entity.MapTo<TagReleationDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool MultiInsert(TagDto tagDto)
        {
            try
            {
                string tagName = tagDto.tagName;
                string dataID = tagDto.dataID;
                string mapLayerID = tagDto.mapLayerID;

                _ITagReleationRepository.Delete(q => q.MapID == mapLayerID);

                string multiTagID = "";
                string[] arr;
                if (!string.IsNullOrEmpty(tagName))
                {
                    tagName = tagName.Replace("，", ",").Replace(" ", ",").Replace("　", ",");

                    if (tagName.Contains(","))
                    {
                        arr = tagName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        arr = new string[] { tagName };
                    }

                    for (int i = 0; i < arr.Length; i++)
                    {
                        string tagID = "";
                        var name = arr[i];
                        var ret = _IDataTagRepository.GetAll().Where(q => q.TagName == name && q.DictCodeID == dataID).FirstOrDefault();
                        if (ret != null && ret.Id != null)
                        {
                            tagID = ret.Id;
                        }
                        else
                        {
                            DataTagEntity entity = new DataTagEntity();
                            entity.Id = Guid.NewGuid().ToString();
                            entity.TagName = arr[i];
                            entity.DictCodeID = dataID;
                            tagID = _IDataTagRepository.InsertAndGetId(entity);
                        }

                        if (!string.IsNullOrEmpty(tagID))
                        {
                            TagReleationEntity rel = new TagReleationEntity();
                            rel.Id = Guid.NewGuid().ToString();
                            rel.MapID = mapLayerID;
                            rel.DataTagID = tagID;
                            multiTagID += _ITagReleationRepository.InsertAndGetId(rel) + ",";
                        }
                    }
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task<TagReleationDto> Update(TagReleationInputDto input)
        {
            try
            {
                var entity = _ITagReleationRepository.Get(input.Id);
                if (entity != null)
                {
                    entity.MapID = input.MapID;
                    entity.DataTagID = input.DataTagID;

                }
                var dto = await _ITagReleationRepository.UpdateAsync(entity);
                var result = dto.MapTo<TagReleationDto>();
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
                await _ITagReleationRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除标签关系数据
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns></returns>
        public async Task DeleteByMapID(string mapID)
        {
            #region [删除标签关系数据]

            await _ITagReleationRepository.DeleteAsync(q => q.MapID == mapID);

            #endregion
        }

        #endregion
    }
}

