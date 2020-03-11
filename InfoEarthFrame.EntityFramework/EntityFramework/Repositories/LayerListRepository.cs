using Abp.EntityFramework;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class LayerListRepository : ILayerListRepository
    {


        public LayerListRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
        {
        }

        public LayerPageData GetPageList(string userId,int PageIndex, int PageSize, Layer l)
        {

            LayerPageData d = new LayerPageData();
//            var strSql = "SELECT mm.*, nn.\"IsDownload\", nn.\"IsBrowse\"" +
//"  from (SELECT bb.\"Id\" AS MappingTypeID," +
//"               bb.\"ClassName\" AS MappingClassName," +
//"               \'\' AS StartDate," +
//"               \'\' AS EndDate," +
//"               aa.\"Id\" as DataMainID," +
//"               cc.\"Id\" as MetaDataID," +
//"               aa.\"ImagePath\"," +
//"               ee.\"INDEXID\" AS ImageID," +
//"               ee.\"PID\" AS ImagePID," +
//"               ee.\"TEXT\" AS ImageText," +
//"               ee.\"URL\" AS ImageURL," +
//"               ee.\"DATASERVERKEY\" AS ImageDataServerKEY," +
//"               ee.\"TILESIZE\" AS ImageTileSize," +
//"               ee.\"ZEROLEVELSIZE\" AS ImageZeroLevelSize," +
//"               ee.\"PICTYPE\" AS ImagePicType," +
//"               to_char(cc.\"mdDataSt\", \'yyyy-mm-dd\') AS MDDataSt," +
//"               dd.\"tpCat\" AS TPCat," +
//"               (SELECT wmsys.wm_concat(to_char(ee.\"rpOrgName\")) as OrgName" +
//"                  from TBL_MD_CONTACT ee" +
//"                 where cc.\"Id\" = ee.\"MetaDataID\"" +
//"                 group by ee.\"MetaDataID\") AS OrgName," +
//"               dd.\"idAbs\" AS IdAbs," +
//"               \'\' AS PusUnit," +
//"               aa.\"MainShpFileName\"" +
//"          FROM TBL_DATAMAIN           aa" +
//"             join \"TBL_LAYERMANAGER\"     ee on aa.\"Id\" = ee.\"DataMainID\"" +
//"             join \"TBL_GEOLOGYMAPPINGTYPE\" bb on aa.\"MappingTypeID\" = bb.\"Id\"" +
//"      left join \"TBL_MD_METADATA\" cc on aa.\"Id\" = cc.\"MainID\"" +
//"       left join \"TBL_MD_IDENTIFICATION\" dd on cc.\"Id\" = dd.\"MetaDataID\"" +
//"          where 1=1" +
//"           and (instr(bb.\"ClassName\", '" + l.MappingClassName + "') > 0 or '" + l.MappingClassName + "' is null)" +
//"           and (exists" +
//"                (select ff.\"Id\"" +
//"                   from TBL_MD_CONTACT ff" +
//"                  where cc.\"Id\" = ff.\"MetaDataID\"" +
//"                    and instr(ff.\"rpOrgName\",'" + l.OrgName + "') > 0) or '" + l.OrgName + "' is null)" +
//"           and ((cc.\"mdDataSt\" >= to_date('" + l.StartDate + "', \'yyyy/mm/dd\') or '" + l.StartDate + "' is null) and" +
//"               (\"mdDataSt\" <= to_date('" + l.EndDate + "', \'yyyy/mm/dd\') or '" + l.EndDate + "' is null))" +
//"           and instr(bb.\"Paths\", '" + l.MappingTypeID + "') > 0) mm" +
//"  left join (SELECT \"LayerId\"," +
//"                    max(\"IsDownload\") as \"IsDownload\"," +
//"                    max(\"IsBrowse\") as \"IsBrowse\"" +
//"               FROM \"TBL_GROUP_RIGHT\"" +
//"              where \"GroupId\" in" +
//"                    (SELECT \"GroupId\"" +
//"                       FROM \"TBL_GROUP_USER\"" +
//"                      where \"UserId\" = '" + UserId + "')" +
//"              group by \"LayerId\") nn" +
//"    on mm.MappingTypeID = nn.\"LayerId\"";
             using (var ctx = new InfoEarthFrameDbContext())
            {
            var where = "";
            if (!string.IsNullOrEmpty(l.Name))
            {
                where += " AND aa.\"Name\" like '%" + l.Name + "%'";
            }

            if (l.MappingClassName == "全国"
              || l.MappingClassName == "省域"
              || l.MappingClassName == "区域")
            {
                var mappingType = ctx.GeologyMappingType.FirstOrDefault(p => p.ClassName == l.MappingClassName);
                if (mappingType != null)
                {
                    l.MappingTypeID = mappingType.Id;
                }
            }
            if (!string.IsNullOrEmpty(l.MappingTypeID))
            {
                where += " AND bb.\"Paths\" like '%" + l.MappingTypeID + "%'";
            }
    
            var strSql = "SELECT * FROM ( SELECT mm.*, nn.\"IsDownload\", nn.\"IsBrowse\"" +
"  from (SELECT bb.\"Id\" AS MappingTypeID," +
"               bb.\"ClassName\" AS MappingClassName," +
"               \'\' AS StartDate," +
"               \'\' AS EndDate," +
"               aa.\"Id\" as DataMainID," +
"              '' as MetaDataID," +
"               aa.\"ImagePath\" as ThumbFilePath," +
"               ee.\"INDEXID\" AS ImageID," +
"               ee.\"PID\" AS ImagePID," +
"               ee.\"TEXT\" AS ImageText," +
"               ee.\"URL\" AS ImageURL," +
"               aa.\"ReleaseTime\" AS PublishTime," +
"               ee.\"DATASERVERKEY\" AS ImageDataServerKEY," +
"               ee.\"TILESIZE\" AS ImageTileSize," +
"               ee.\"ZEROLEVELSIZE\" AS ImageZeroLevelSize," +
"               ee.\"PICTYPE\" AS ImagePicType," +
"(select \"FileData\"  FROM \"TBL_DATAMANAGEFILE\""+
"WHERE \"MainID\"=     aa.\"Id\"" +
"and \"FolderName\"='5元数据')  as MetaDataPath," +
"               '' AS MDDataSt," +
"               '' AS TPCat," +
"               '' as OrgName," +
"              '' AS IdAbs," +
"               \'\' AS PusUnit," +
"               aa.\"MainShpFileName\"" +
"          FROM \"TBL_DATAMAIN\"           aa" +
"             join \"TBL_LAYERMANAGER\"     ee on aa.\"Id\" = ee.\"DataMainID\"" +
"             join \"TBL_GEOLOGYMAPPINGTYPE\" bb on aa.\"MappingTypeID\" = bb.\"Id\"" +
"          where 1=1" + where+
"           ) mm" +
"  left join (SELECT \"LayerId\"," +
"                    max(\"IsDownload\") as \"IsDownload\"," +
"                    max(\"IsBrowse\") as \"IsBrowse\"" +
"               FROM \"TBL_GROUP_RIGHT\"" +
"              where \"GroupId\" in" +
"                    (SELECT \"GroupId\"" +
"                       FROM \"TBL_GROUP_USER\"" +
"                      where \"UserId\" = '" + userId + "')" +
"              group by \"LayerId\") nn" +
"    on mm.MappingTypeID = nn.\"LayerId\") T WHERE  \"IsBrowse\"=1 ";
            if (userId.ToLower() == "admin")
            {
                 strSql = "SELECT * FROM ( SELECT mm.*,1 as \"IsDownload\", 1 as \"IsBrowse\"" +
 "  from (SELECT bb.\"Id\" AS MappingTypeID," +
 "               bb.\"ClassName\" AS MappingClassName," +
 "               \'\' AS StartDate," +
 "               \'\' AS EndDate," +
 "               aa.\"Id\" as DataMainID," +
 "              '' as MetaDataID," +
 "               aa.\"ImagePath\" as ThumbFilePath," +
 "               ee.\"INDEXID\" AS ImageID," +
 "               ee.\"PID\" AS ImagePID," +
 "               ee.\"TEXT\" AS ImageText," +
 "               ee.\"URL\" AS ImageURL," +
 "               aa.\"ReleaseTime\" AS PublishTime," +
 "               ee.\"DATASERVERKEY\" AS ImageDataServerKEY," +
 "               ee.\"TILESIZE\" AS ImageTileSize," +
 "               ee.\"ZEROLEVELSIZE\" AS ImageZeroLevelSize," +
 "               ee.\"PICTYPE\" AS ImagePicType," +
 "(select \"FileData\"  FROM \"TBL_DATAMANAGEFILE\"" +
 "WHERE \"MainID\"=     aa.\"Id\"" +
 "and \"FolderName\"='5元数据')  as MetaDataPath," +
 "               '' AS MDDataSt," +
 "               '' AS TPCat," +
 "               '' as OrgName," +
 "              '' AS IdAbs," +
 "               \'\' AS PusUnit," +
 "               aa.\"MainShpFileName\"" +
 "          FROM \"TBL_DATAMAIN\"           aa" +
 "             join \"TBL_LAYERMANAGER\"     ee on aa.\"Id\" = ee.\"DataMainID\"" +
 "             join \"TBL_GEOLOGYMAPPINGTYPE\" bb on aa.\"MappingTypeID\" = bb.\"Id\"" +
 "          where 1=1" + where +
 "           ) mm) T";
            }
           
                var ret = ctx.Database.SqlQuery<Layer>(strSql);
                var list = ret.OrderByDescending(p=>p.PublishTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                d.data = list;
                d.counts = ret.Count();

 
                return d;
            }

        }
    }
}
