using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace InfoEarthFrame.Core
{
    public interface IGroupRepository : IRepository<GroupEntity, string>
    {    
    }
    public interface IGroupUserRepository : IRepository<GroupUserEntity, string>
    {
    }
    public interface IGroupRightRepository : IRepository<GroupRightEntity, string>
    {
    }
}


