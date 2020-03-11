using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using AutoMapper;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public interface IAttachmentAppService : IApplicationService
    {
        Task<ListResultOutput<AttachmentDto>> GetAllList(string fkid);
        string InsertMultimedia(MultimediaAttachmentInput input);
        Task<bool> Insert(AttachmentInput input);
        Task<bool> Insert(List<AttachmentInput> inputList);
        Task<bool> Delete(string id);
        Task<bool> DeleteAllByFKId(string FKId);
    }
}
