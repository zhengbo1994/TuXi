using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.EntityFramework;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;


namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class AttachmentRepository : InfoEarthFrameRepositoryBase<AttachmentEntity, string>, IAttachmentRepository
    {
        public AttachmentRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
           
             
        }
    }
}
