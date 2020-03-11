using Abp.EntityFramework;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{

    public  class SlopeRepository : InfoEarthFrameRepositoryBase<SlopeEntity, String>, ISlopeRepository
    {
        public SlopeRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        

        public async Task<List<SlopeEntity>> GetPageList(int pageIndex, int pageSize, string disasterunitname, string location)
        {
            var query = new List<SlopeEntity>();            
            var eps = ExpressionExtensions.True<SlopeEntity>();

            if (disasterunitname == "" && location == "")
            {
                 query = GetAll().OrderBy(s =>s.UNIFIEDCODE).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                
            }
            else
            {
                if (disasterunitname != "")
                {
                    eps = eps.And(s => s.DISASTERUNITNAME == disasterunitname);
                }
                if (location != "")
                {
                    eps = eps.And(s => s.LOCATION == location);
                }            
                query = GetAll().Where(eps).OrderBy(s => s.UNIFIEDCODE).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
           
           
            return query;
        }

 

        public async Task<int> GetPageCount(int pageIndex, int pageSize, string disasterunitname, string location)
        {
            var counts =0;
            var eps = ExpressionExtensions.True<SlopeEntity>();

            if (disasterunitname == "" && location == "")
            {
                counts = GetAll().OrderBy(s => s.UNIFIEDCODE).Count();

            }
            else
            {
                if (disasterunitname != "")
                {
                    eps = eps.And(s => s.DISASTERUNITNAME == disasterunitname);
                }
                if (location != "")
                {
                    eps = eps.And(s => s.LOCATION == location);
                }
                counts = GetAll().Where(eps).OrderBy(s => s.UNIFIEDCODE).Count();
            }                
            return counts;
        }

        public Expression<Func<SlopeEntity,bool>> GetWhere(string disasterunitname, string location)
        {
            var eps = ExpressionExtensions.True<SlopeEntity>();

           
                if (disasterunitname != "")
                {
                    eps = eps.And(s => s.DISASTERUNITNAME == disasterunitname);
                }
                if (location != "")
                {
                    eps = eps.And(s => s.LOCATION == location);
                }
                
            
            return eps;
        }
       

    }
}
