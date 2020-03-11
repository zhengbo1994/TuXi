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
    [AbpAuthorize]
    public class UploadFileAppService : ApplicationService, IUploadFileAppService
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iLayerManagerRepository"></param>
        public UploadFileAppService()
        {

        }

        public List<UploadDTO> Upload(List<UploadDTO> fileList)
        {
            List<UploadDTO> RetList = new List<UploadDTO>();

            foreach (UploadDTO ud in fileList)
            {
                FileInfo fi = new FileInfo(ud.Path);
                string name = Guid.NewGuid().ToString() + fi.Extension;
            }

            return RetList;
        }
        
    }
}
