using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.SystemUserApp
{
    public interface ISystemUserAppService : IApplicationService
    {
        #region 自动生成
        Task<ListResultOutput<SystemUserDto>> GetAllList();

        Task<PagedResultOutput<SystemUserDto>> GetAllListByName(SystemUserDto input, int PageSize, int PageIndex);

        Task<ListResultOutput<SystemUserDto>> GetAllListByCondition(SystemUserDto input);

        Task<PagedResultOutput<SystemUserDto>> GetAllPageListByCondition(SystemUserDto input, int PageSize, int PageIndex);

        Task<List<SystemUserDto>> GetUserDataByUserCodeAsync(string userCode);

        Task<SystemUserOutputDto> GetDetailById(string id);

        Task<SystemUserOutputDto> GetDetailByName(string username);

        SystemUserOutputDto GetDetailByNamePassword(SystemUserDto input);

        Task<SystemUserDto> Insert(SystemUserDto input);

        Task<SystemUserDto> Update(SystemUserDto input);

        Task<SystemUserDto> UpdatePassword(SystemUserDto input);

        Task Delete(string id);

       Task<bool> Delete(IEnumerable<string> ids);

        Task<bool> ResetPassword(IEnumerable<string> ids,string password);

        bool IsUserCodeExists(string userCode);

        string GetAccessToken(string username, string password,string apiUrl,out string errMsg);
        #endregion

        #region  重置密码

        bool GetLoginPasswordByUserCode(string password, string currentUserName);

        Task<bool> ChangePassword(string password, string currentUserName);

        #endregion
    }
}

