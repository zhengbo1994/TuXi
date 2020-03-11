using Abp.EntityFramework;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class GroupRepository : InfoEarthFrameRepositoryBase<GroupEntity, string>, IGroupRepository
    {
        public GroupRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
    public class GroupUserRepository : InfoEarthFrameRepositoryBase<GroupUserEntity, string>, IGroupUserRepository
    {
        public GroupUserRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
    public class GroupRightRepository : InfoEarthFrameRepositoryBase<GroupRightEntity, string>, IGroupRightRepository
    {
        public GroupRightRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
           
        }
    }
}