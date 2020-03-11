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
using System.Web;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Application
{

    public class LayerListAppService : ApplicationService, ILayerListAppService
    {
       // private readonly ILog _logger = log4net.LogManager.GetLogger(typeof(LayerListAppService));

        private readonly ILayerListRepository _iLayerListRepository;
        private readonly InfoEarthFrame.Core.ITopiccategoryCodeRepository _TopiccategoryCodeRepository = null;
        private readonly InfoEarthFrame.Core.IGroupRightRepository _GroupRightRepository = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iLayerManagerRepository"></param>
        public LayerListAppService(ILayerListRepository iLayerListRepository, ITopiccategoryCodeRepository iTopiccatCodeRepository, IGroupRightRepository iGroupRightRepository)
        {
            _iLayerListRepository = iLayerListRepository;
            _TopiccategoryCodeRepository = iTopiccatCodeRepository;
            _GroupRightRepository = iGroupRightRepository;
        }
        /// <summary>
        /// 获取带count的分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<LayerListOutput> GetPageListAndCount(string userid,LayerListInput input)
        {
            Layer l = new Layer();
            l.MappingTypeID = input.MappingTypeID;
            l.MappingClassName = (input.MappingClassName??"").Trim();
            l.StartDate = (!string.IsNullOrEmpty(input.StartDate)) ? input.StartDate.Substring(0, 10).Trim() : null;
            l.EndDate = (!string.IsNullOrEmpty(input.EndDate)) ? input.EndDate.Substring(0, 10).Trim() : null;
            l.OrgName = (input.OrgName??"").Trim();
            l.Name = (input.Name ?? "").Trim();
            try
            {
                var result =  _iLayerListRepository.GetPageList(userid,input.PageIndex, input.PageSize, l);


                var middleResult = result.data;

               // var tpCatCode = _TopiccategoryCodeRepository.GetAll().ToList();

                List<LayerListOutput> lstlo = new List<LayerListOutput>();

                string strPusUnit = System.Configuration.ConfigurationManager.AppSettings["PublishUnit"].ToString();

                middleResult.ForEach((x) =>
                {
                    string strTPCatName = "";
                    try
                    {
                        var metaData = Metadata.GetInstance(x.DataMainID, "元数据", x.MetaDataPath);
                        if (metaData != null)
                        {
                            strTPCatName = metaData.mdContact.rpOrgName;
                            x.IdAbs = metaData.dataIdInfo.idAbs;
                        }
                    }
                    catch
                    {
                    }
                    //if (!string.IsNullOrEmpty(x.TPCat) && x.TPCat.Contains(","))
                    //{
                    //    string[] arrTPCat = x.TPCat.Split(',');
                    //    for (int i = 0; i < arrTPCat.Length; i++)
                    //    {
                    //        TopiccategoryCode tc = tpCatCode.Find(
                    //        delegate(TopiccategoryCode tcc)
                    //        {
                    //            return tcc.TopiccatCode == arrTPCat[i];
                    //        });

                    //        if (tc != null)
                    //        {
                    //            strTPCatName += tc.TopCategoryName + "-" + tc.SecCategoryName + "；";
                    //        }
                    //    }
                    //}
                    LayerListOutput llo = x.MapTo<LayerListOutput>();
                    llo.TPCat = strTPCatName;
                    llo.PusUnit = strPusUnit;
                    llo.isDownload = x.IsDownload == 1 ? true : false;
                    llo.isBrowse = x.IsBrowse == 1 ? true : false;

                    //拼接下载文件路径
                  //  String strAbsolutePath = System.Configuration.ConfigurationManager.AppSettings["LayerPath"].ToString() + llo.DataMainID + ".zip";
                 //   String strApplicationPath = HttpContext.Current.Request.PhysicalApplicationPath;
                  //  llo.DownLoadFilePath = strAbsolutePath.Replace(strApplicationPath, "").Replace(@"\", @"/");
                    llo.DownLoadFilePath = "";
                    //SHP文件路径
                    llo.ShpFilePath = llo.DataMainID + "/" + llo.MainShpFileName;
                    llo.ThumbFilePath = ConfigContext.Current.FtpConfig["Package"].Site + "/" + (llo.ThumbFilePath ?? "").Replace("\\", "/");

                    lstlo.Add(llo);
                });
                input.Total = result.counts;
                return lstlo;
            }
            catch (Exception ex)
            {
              //  _logger.Error(ex.ToString());
                throw ex;
            }
        }

    }
}
