using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.UsersRight.Dtos;
using InfoEarthFrame.Application.SystemUserApp.Dtos;

namespace InfoEarthFrame.Application
{
    public interface IUsersRightAppService : IApplicationService
    {
        void DeleteMenu(string moduleId, string menuId);



        Task<List<GroupDto>> GetAllGroup();
        GroupDto Intsert(GroupInput input);
        void Update(GroupInput input);
        void Delete(string id);
        void SetRight(string GroupId, List<SetMapRightInput> input);

        void SetMapRight(SetMapRightInput model);

       void SetMenuRight(SetMenuRightInput model);

       void SetMenuButtonRight(SetMenuButtonRightInput model);

        void IntsertGroupUser(GroupUserComparaInput input);

        void IntsertGroupUser(string groupId,IEnumerable<string> userIds);
        void DelGroupUser(List<GroupUserInput> input);

        bool DelGroupUser(IEnumerable<string> ids);

        void UpdateGroupUser(GroupUserInput Input);
        List<GroupOutput> GetGroupWithRight();
        List<GroupOutput> GetGroupWithUser();

        List<UserGroupMapPermessionDto> GetUserGroupMapPermession(string groupId);

        List<UserGroupMenuPermessionDto> GetUserGroupMenuPermession(string groupId);


        PagedResultOutput<SystemUserDto> GetUserGroupRelatedUsers(string groupId, string userName,int PageSize, int PageIndex);


        PagedResultOutput<SystemUserDto> GetUserGroupAlternativeUsers(string groupId, string userName, int PageSize, int PageIndex);

        void AddOrEditMenu(SetMenuRightPreamInput model);


        InfoEarthFrame.Common.ModuleConfig.ParentMenu GetMenuInfo(SetMenuRightPreamInput model);

        void SetMenuButtonArea(SetMenuButtonRightInput model);
    }
}


