using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AutoMapper;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using Abp.AutoMapper;

namespace InfoEarthFrame.Application
{
    public class DisasterService : ApplicationService, IDisasterService
    {
        
        private readonly IDisasterRepository _iDisasterRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="idDisasterRepository"></param>
        public DisasterService(IDisasterRepository idDisasterRepository)
        {
            _iDisasterRepository = idDisasterRepository;
        }

        #region 系统生成
        
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>

        public async Task<ListResultOutput<DisasterDto>> GetAllList()
        {
            try
            {
                //var disasterEntity = await _iDisasterRepository.GetAllListAsync();
                var disasterEntity =  _iDisasterRepository.GetAllList();
                var list = new ListResultOutput<DisasterDto>(disasterEntity.MapTo<List<DisasterDto>>());
                return list;
            }
            catch (Exception exception)
            {

                throw exception;
            }

        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="queryDisasterDato"></param>
        /// <returns></returns>
        public async Task<ListResultOutput<DisasterDto>> GetPageList(QueryDisasterInput queryDto)
        {
            try
            {
                var disasterEntity = await _iDisasterRepository.GetPageList(queryDto.PageIndex, queryDto.PageSize);
                var list = new ListResultOutput<DisasterDto>(disasterEntity.MapTo<List<DisasterDto>>());
                return list;
            }
            catch (Exception exception)
            {

                throw exception;
            }
        }


        /// <summary>
        /// 分页并返回数据总条数
        /// </summary>
        /// <param name="queryDto"></param>
        /// <returns></returns>
        public async Task<PagedResultOutput<DisasterDto>> GetPageAndCountList(QueryDisasterInput queryDto)
        {
            try
            {
                var disasterEntity = await _iDisasterRepository.GetPageList(queryDto.PageIndex, queryDto.PageSize);

                IReadOnlyList<DisasterDto> ir = disasterEntity.MapTo<List<DisasterDto>>();
                int count =  _iDisasterRepository.Count();
                PagedResultOutput<DisasterDto> list = new PagedResultOutput<DisasterDto>(count, ir);
                return list;
            }
            catch (Exception exception)
            {

                throw exception;
            }
        }

        /// <summary>
        /// 获取符合的数据条数
        /// </summary>
        /// <param name="queryDisasterDato"></param>
        /// <returns></returns>
        public async Task<int> GetCount(QueryDisasterInput queryDto)
        {
            int pageCount = await _iDisasterRepository.CountAsync();
            return pageCount;
        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<DisasterOutput> GetDetailById(int Id)
        {
            try
            {
                var entity = await _iDisasterRepository.GetAsync(Id);
                DisasterOutput disasterDtoRtn = entity.MapTo<DisasterOutput>();
                return disasterDtoRtn;
            }
            catch (Exception exception)
            {

                throw exception;
            }

        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="disasterDto"></param>
        /// <returns></returns>
        public async Task<DisasterDto> Insert(DisasterInput input)
        {
            try
            {
                DisasterEntity disasterEntity = new DisasterEntity
                {
                    DISASTERNAME = input.DISASTERNAME,
                    OCCURTIME = input.OCCURTIME,
                    POSITION = input.POSITION,
                    REMARK = input.REMARK,
                    AREARIGHTCODE = input.AREARIGHTCODE,
                    IsDeleted = false
                };
                DisasterEntity entity = await _iDisasterRepository
                                                        .InsertAsync(disasterEntity);
                DisasterDto disasterDtoRtn = entity.MapTo<DisasterDto>();
                return disasterDtoRtn;
            }
            catch (Exception exception)
            {

                throw exception;
            }


        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="disasterDto"></param>
        /// <returns></returns>
        public async Task<DisasterDto> Update(DisasterInput input)
        {
            try
            {
                DisasterEntity disasterEntity = new DisasterEntity
                {
                    Id = input.Id,
                    DISASTERNAME = input.DISASTERNAME,
                    OCCURTIME = input.OCCURTIME,
                    POSITION = input.POSITION,
                    REMARK = input.REMARK,
                    AREARIGHTCODE = input.AREARIGHTCODE,
                    IsDeleted = false
                };
                DisasterEntity entity = _iDisasterRepository.Update(disasterEntity);
                DisasterDto disasterDtoRtn = entity.MapTo<DisasterDto>();
                return disasterDtoRtn;
            }
            catch (Exception exception)
            {

                throw exception;
            }

        }

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="Id"></param>
        public async Task DeleteById(int Id)
        {
            try
            {
               await _iDisasterRepository.DeleteAsync(Id);
            }
            catch (Exception exception)
            {

                throw exception;
            }

        }

        #endregion

        public async Task<DisasterDto>  InertDisaster()
        {
            try
            {
                DisasterDto disasterDtoRtn = new DisasterDto();
                for (int i = 0; i < 100; i++)
                {
                    DisasterDto disasterDto = new DisasterDto();
                    disasterDto.DISASTERNAME = "滑坡";
                    disasterDto.IsDeleted = false;
                    disasterDto.AREARIGHTCODE = "420010";
                    disasterDto.OCCURTIME = DateTime.Now;
                    disasterDto.POSITION = "湖北武汉洪山区";

                    Mapper.CreateMap<DisasterEntity, DisasterDto>();
                    DisasterEntity disasterEntity = Mapper.DynamicMap<DisasterDto, DisasterEntity>(disasterDto);


                    DisasterEntity entity = await _iDisasterRepository.InsertAsync(disasterEntity);
                    disasterDtoRtn = Mapper.Map<DisasterDto>(entity);
                }

                return disasterDtoRtn;
            }
            catch (Exception exception)
            {

                throw exception;
            }

        }


        #region 自定义
        #endregion 

    }
}
