using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.AutoMapper;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InfoEarthFrame.Application
{
    public class AttachmentAppService : ApplicationService, IAttachmentAppService
    { 
        private readonly IAttachmentRepository _iAttachmentRepository;   

        /// <summary>
        /// 构造函数
        /// </summary>
        public AttachmentAppService(IAttachmentRepository iAttachmentRepository)
        {
             _iAttachmentRepository = iAttachmentRepository;
        }

        /// <summary>
        /// 通过外键获取附件列表
        /// </summary>
        /// <param name="fkid"></param>
        /// <returns></returns>
        public async Task<ListResultOutput<AttachmentDto>> GetAllList(string fkid)
        {
            //var result = await _iAttachmentRepository.GetAllListAsync(q => q.FKey == fkid);
            var result =  _iAttachmentRepository.GetAllList(q => q.FKey == fkid);
            var outputList = new ListResultOutput<AttachmentDto>(result.MapTo<List<AttachmentDto>>());
            return outputList;
        }

        /// <summary>
        /// 多媒体插入一条记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string InsertMultimedia(MultimediaAttachmentInput input)
        {
            AttachmentEntity att = new AttachmentEntity
            {
                Id = Guid.NewGuid().ToString(),
                FKey = input.FKey,
                PhysicalName = input.PhysicalName,
                LogicName = input.LogicName,
                FileSize = input.FileSize,
                Extension = input.Extension,
                Sn = input.Sn,
                iState = 1,
                CreateTime = DateTime.Now,
                CreateName = input.CreateName,
                PhysicalPath = input.PhysicalPath,
                HttpPath = input.HttpPath,
                TypeCode = input.TypeCode
            };
            try
            {
                return _iAttachmentRepository.InsertAndGetId(att);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 异步插入一个
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> Insert( AttachmentInput input)
        {
            AttachmentEntity att = new AttachmentEntity
            {
                Id = Guid.NewGuid().ToString(),
                FKey = input.FKey,
                PhysicalName = input.PhysicalName,
                LogicName = input.LogicName,
                FileSize = input.FileSize,
                Extension = input.Extension,
                Sn = input.Sn,
                iState = 1,
                CreateTime = DateTime.Now,
                CreateName = input.CreateName,
                PhysicalPath = input.PhysicalPath,
                HttpPath = input.HttpPath
            };
            try
            {
               await _iAttachmentRepository.InsertAsync(att);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 异步插入多个
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public async Task<bool> Insert(List<AttachmentInput> inputList)
        {
            foreach(var item in inputList){
               await Insert(item);
            }
            return true;
        }
        /// <summary>
        /// 根据ID主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string id)
        {
            try
            {
                await _iAttachmentRepository.DeleteAsync(id);
            }
            catch (Exception)
            {
                return false;
            }
          
            return true;
        }
        /// <summary>
        /// 根据外键删除
        /// </summary>
        /// <param name="FKId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAllByFKId(string FKId)
        {
            try
            {
                await _iAttachmentRepository.DeleteAsync(q => q.FKey == FKId);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
