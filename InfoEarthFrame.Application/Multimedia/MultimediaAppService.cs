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
    public class MultimediaAppService : ApplicationService, IMultimediaAppService
    {
        private readonly IMultimediaTypeRepository _iMultimediaTypeRepository;
        private readonly IAttachmentRepository _iAttachmentRepository;  
        /// <summary>
        /// 构造函数
        /// </summary>
        public MultimediaAppService(IMultimediaTypeRepository iMultimediaTypeRepository, IAttachmentRepository iAttachmentRepository)
        {
            _iMultimediaTypeRepository = iMultimediaTypeRepository;
            _iAttachmentRepository = iAttachmentRepository;
        }

        /// <summary>
        /// 通过外键获取附件列表
        /// </summary>
        /// <param name="fkid"></param>
        /// <returns></returns>
        public List<MultimediaOutput> GetAllList(ModuleInfoInput input)
        {
            var att = _iAttachmentRepository.GetAll().Where(q => q.FKey == input.FKey);
            var query = _iMultimediaTypeRepository.Query(q =>q.GroupJoin(att, m => m.Id, a => a.TypeCode, (m, a) => new { m, a }));
            var ret = query.Where(q => q.m.ModuleType == input.ModuleType).ToList();
            List<MultimediaOutput> list = new List<MultimediaOutput>();
            if (ret.Count > 0)
            {
                ret.ForEach((x) =>
                {
                    MultimediaOutput output = new MultimediaOutput();
                    List<MultimediaAttachmentInput> mlist = new List<MultimediaAttachmentInput>();
                    if (x.a.Count() > 0)
                    {
                        foreach (var tb in x.a)
                        {
                            MultimediaAttachmentInput mul = new MultimediaAttachmentInput();
                            mul.Id = tb.Id;
                            mul.FKey = tb.FKey;
                            mul.PhysicalName = tb.PhysicalName;
                            mul.LogicName = tb.LogicName;
                            mul.PhysicalPath = tb.PhysicalPath;
                            mul.HttpPath = tb.HttpPath;
                            mul.FileSize = tb.FileSize;
                            mul.Extension = tb.Extension;
                            mul.TypeCode = tb.TypeCode;
                            mlist.Add(mul);
                        }
                    }
                    output.Id = x.m.Id;
                    output.Name = x.m.Name;
                    output.ModuleType = x.m.ModuleType;
                    output.Children = mlist;
                    list.Add(output);
                });
            }
            return list;
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> Insert(MultimediaTypeInput input)
        {
            MultimediaTypeEntity mul = new MultimediaTypeEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = input.Name,
                ModuleType = input.ModuleType,
                CreateTime = DateTime.Now
            };
            try
            {
                string ret = await _iMultimediaTypeRepository.InsertAndGetIdAsync(mul);
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<bool> Update(MultimediaTypeInput input)
        {
            MultimediaTypeEntity mul = new MultimediaTypeEntity
            {
                Id = input.Id,
                Name = input.Name,
                ModuleType = input.ModuleType               
            };
            try
            {
                await _iMultimediaTypeRepository.UpdateAsync(mul);
            }
            catch (Exception)
            {
                return false;
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
            int ret = _iAttachmentRepository.Count(q => q.TypeCode == id);
            if (ret>0)
            {
                return false;
            }
            try
            {
                await _iMultimediaTypeRepository.DeleteAsync(id);
            }
            catch (Exception)
            {
                return false;
            }          
            return true;
        }
    }
}
