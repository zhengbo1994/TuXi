using System;
using System.Threading;
using Common.Logging;
using System.Data;
using InfoEarthFrame.Common;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;

namespace InfoEarth.Task
{
    /// <summary>
    /// A sample job that just prints info on console for demostration purposes.
    /// </summary>
    public class ThumbnailInitialize: IThumbnailInitialize 
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ThumbnailInitialize));
        //private MySqlHelper mysql;
        private DataTable dtData = new DataTable();

        PostgrelVectorHelper postgis = new PostgrelVectorHelper();

        /// <summary>
        /// Called by the <see cref="IScheduler" /> when a <see cref="ITrigger" />
        /// fires that is associated with the <see cref="IJob" />.
        /// </summary>
        /// <remarks>
        /// The implementation may wish to set a  result object on the 
        /// JobExecutionContext before this method exits.  The result itself
        /// is meaningless to Quartz, but may be informative to 
        /// <see cref="IJobListener" />s or 
        /// <see cref="ITriggerListener" />s that are watching the job's 
        /// execution.
        /// </remarks>
        /// <param name="context">The execution context.</param>
        public void ExcuteJob(PerformContext context)
        {
            logger.Info("ThumbnailInitialize running...");
            //mysql = new MySqlHelper();

            DataTable dt = dtCreate();
            dtLayer(dt);
            dtMap(dt);

            if(dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    string bboxStr = string.Format("{0},{1},{2},{3}", dr["MinX"], dr["MinY"], dr["MaxX"], dr["MaxY"]);
                    ThumbnailCreate(dr["Name"].ToString(), dr["Type"].ToString(), bboxStr);
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(5));
            logger.Info("ThumbnailInitialize run finished.");
        }

        /// <summary>
        /// 获取图层数据
        /// </summary>
        /// <param name="dtData"></param>
        public void dtLayer(DataTable dtData)
        {

            string strSQL = "SELECT LayerAttrTable as Name,'layer' as Type,MinX,MinY,MaxX,MaxY FROM sdms_layer where UploadStatus = '1'";

            //dtData.Merge(mysql.ExecuteQuery(strSQL));
            dtData.Merge(postgis.getDataTable(strSQL));

        }

        /// <summary>
        /// 获取地图数据
        /// </summary>
        /// <param name="dtData"></param>
        public void dtMap(DataTable dtData)
        {
            string strSQL = "SELECT MapEnName as Name,'map' as Type,MinX,MinY,MaxX,MaxY  FROM sdms_map ";
            //dtData.Merge(mysql.ExecuteQuery(strSQL));
            dtData.Merge(postgis.getDataTable(strSQL));
        }
        /// <summary>
        /// 创建空表
        /// </summary>
        /// <returns></returns>
        public DataTable dtCreate()
        {
            DataTable dt = new DataTable();
            DataColumn dc;

            dc = new DataColumn("Name",typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("Type", typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("MinX", typeof(decimal));
            dt.Columns.Add(dc);
            dc = new DataColumn("MinY", typeof(decimal));
            dt.Columns.Add(dc);
            dc = new DataColumn("MaxX", typeof(decimal));
            dt.Columns.Add(dc);
            dc = new DataColumn("MaxY", typeof(decimal));
            dt.Columns.Add(dc);

            return dt;
        }


        /// <summary>
        /// 下载缩略图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="bbox"></param>
        public void ThumbnailCreate(string name,string type,string bbox)
        {
            try
            {
                ThumbnailHelper tbh = new ThumbnailHelper();
                string imagePath = tbh.CreateThumbnail(name, type, bbox);
            }
            catch(Exception ex)
            {
                logger.Info(UtilityMessageConvert.Get("缩略图下载异常：图层名称") + "（" + name + ")" + ex.Message);
            }
        }
        
    }
}