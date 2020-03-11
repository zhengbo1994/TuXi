using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.DataStyleApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;
using InfoEarthFrame.LayerFieldApp;
using Abp.Domain.Uow;
using InfoEarthFrame.Common.Style;
using InfoEarthFrame.Common;
using System.IO;
using System.Data;
using System.Drawing;
using Newtonsoft.Json;
using System.Web;
using System.Xml;
using System.Text;
using InfoEarthFrame.Application.LayerContentApp.Dtos;
using InfoEarthFrame.Application.SystemUserApp;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Application.OperateLogApp;
using iTelluro.ZoomifyTile;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using InfoEarthFrame.DataManage.DTO;
using System.Data.Entity;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Application.LayerContentApp;

namespace InfoEarthFrame.Application.DataStyleApp
{
    public class DataStyleAppService : IApplicationService, IDataStyleAppService
    {
        #region 变量
        private readonly IDataStyleRepository _IDataStyleRepository;
        private readonly IDataTypeRepository _IDataTypeRepository;
        private readonly ILayerContentRepository _ILayerContentRepository;
        private readonly ILayerContentAppService _layerContentAppService;
        private readonly ILayerFieldRepository _ILayerFieldRepository;
        private readonly ISystemUserRepository _ISystemUserRepository;
        private readonly SystemUserAppService _SystemUserApp;
        private readonly IOperateLogAppService _IOperateLogAppService;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataStyleAppService(
            IDataStyleRepository iDataStyleRepository, 
            IDataTypeRepository iDataTypeRepository, 
            ILayerContentRepository iLayerContentRepository,
            ILayerFieldRepository iLayerFieldRepository, 
            ISystemUserRepository iSystemUserRepository,
            IOperateLogAppService iOperateLogAppService,
            ILayerContentAppService layerContentAppService)
        {
            _IDataStyleRepository = iDataStyleRepository;
            _IDataTypeRepository = iDataTypeRepository;
            _ILayerContentRepository = iLayerContentRepository;
            _ILayerFieldRepository = iLayerFieldRepository;
            _ISystemUserRepository = iSystemUserRepository;
            _layerContentAppService=layerContentAppService;
            _SystemUserApp = new SystemUserAppService(_ISystemUserRepository);
        _IOperateLogAppService = iOperateLogAppService;
        }

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
		public ListResultOutput<DataStyleDto> GetAllList(string name)
        {
            try
            {
                var lst = _IDataStyleRepository.GetAll().Where(q => string.IsNullOrEmpty(name) ? true : q.StyleName.Contains(name));
                var list = new ListResultOutput<DataStyleDto>(lst.MapTo<List<DataStyleDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsExists(string name)
        {
            //DataStyleEntity entity = _IDataStyleRepository.GetAll().FirstOrDefault(q => string.IsNullOrEmpty(name) ? true : q.StyleName == name);
            DataStyleEntity entity;
            if (string.IsNullOrEmpty(name))
            {
                entity = _IDataStyleRepository.GetAll().FirstOrDefault();
            }
            else
            {
                entity = _IDataStyleRepository.GetAll().FirstOrDefault(q => q.StyleName == name);
            }
            return entity != null;
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        public PagedResultOutput<DataStyleDto> GetAllListPage(DataStyleInputDto input, int PageSize, int PageIndex)
        {
            try
            {
                string name = input.StyleName, type = string.Empty, dataType = string.Empty;

                if (!string.IsNullOrWhiteSpace(input.StyleType))
                {
                    type = GetMultiChildTypeByType(input.StyleType);
                }

                if (!string.IsNullOrWhiteSpace(input.StyleDataType))
                {
                    dataType = input.StyleDataType;
                }

       

                var layerContent = GetDataByUserCodeAsync(input.CreateBy).Result;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    layerContent = layerContent.Where(s => !string.IsNullOrWhiteSpace(s.StyleName) && s.StyleName.Contains(name)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(type))
                {
                    layerContent = layerContent.Where(s => !string.IsNullOrWhiteSpace(s.StyleType) && type.Contains(s.StyleType)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(dataType))
                {
                    layerContent = layerContent.Where(s => !string.IsNullOrWhiteSpace(s.StyleDataType) && dataType.Contains(s.StyleDataType)).ToList();
                }
                 
                var listStyleType = _IDataTypeRepository.GetAll();
                var query = (from d in layerContent
                             join t in listStyleType on d.StyleType equals t.Id into tt
                             from dt in tt.DefaultIfEmpty()
                             select new DataStyleDto
                             {
                                 Id = d.Id,
                                 StyleName = d.StyleName,
                                 StyleType = d.StyleType,
                                 StyleTypeName = (dt == null) ? "" : dt.TypeName,
                                 StyleContent = d.StyleContent,
                                 //LayerDescriptor = GetReleaseStyleContent(d.StyleContent),
                                 StyleDesc = d.StyleDesc,
                                 CreateDT = d.CreateDT,
                                 CreateBy = d.CreateBy,
                                 StyleConfigType = d.StyleConfigType,
                                 StyleDataType = d.StyleDataType,
                                 StyleDefaultLayer = d.StyleDefaultLayer,
                                 StyleRenderColorBand = d.StyleRenderColorBand,
                                 StyleRenderField = d.StyleRenderField,
                                 StyleRenderRule = d.StyleRenderRule,
                                 CurrentStyleId = input.DefaultStyleId
                             }).OrderByDescending(x => x.CreateDT);

                var count = query.Count();

                var result = query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

                IReadOnlyList<DataStyleDto> ir;

                if (result != null && result.Count > 0)
                {
                    ir = result.MapTo<List<DataStyleDto>>();
                }
                else
                {
                    ir = new List<DataStyleDto>();
                }

                PagedResultOutput<DataStyleDto> outputList = new PagedResultOutput<DataStyleDto>(count, ir);
                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region 根据当前登录人所在行政区划，获取对应行政区划下所有人员信息,在返回对应人员信息相关数据
        /// <summary>
        /// 根据当前登录人所在行政区划，获取对应行政区划下所有人员信息,在返回对应人员信息相关数据
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        private async Task<List<DataStyleEntity>> GetDataByUserCodeAsync(string userCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return null;
                }

                List<DataStyleEntity> query = null;
                if (userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    #region 超级用户
                    //query = await _IDataStyleRepository.GetAllListAsync();
                    query =  _IDataStyleRepository.GetAllList();
                    #endregion
                }
                else
                {
                    List<SystemUserDto> userQuery = await _SystemUserApp.GetUserDataByUserCodeAsync(userCode);
                    if (userQuery != null && userQuery.Count > 0)
                    {
                        var userids = userQuery.Select(s => s.UserCode).ToArray();
                        if (userids != null && userids.Any())
                        {
                            //query = await _IDataStyleRepository.GetAllListAsync(s => userids.Contains(s.CreateBy));
                            query =  _IDataStyleRepository.GetAllList(s => userids.Contains(s.CreateBy));
                        }
                    }

                }
                return query;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        public string GetMultiChildTypeByType(string typeID)
        {
            try
            {
                string multiTypeID = typeID + ",";
                var query = _IDataTypeRepository.GetAllList().Where(q => q.ParentID == typeID).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        multiTypeID += item.Id + ",";
                    }
                }
                if (multiTypeID.Length > 0)
                {
                    multiTypeID = multiTypeID.Substring(0, multiTypeID.Length - 1);
                }
                return multiTypeID;
            }
            catch (Exception ex)
            {
                return typeID;
            }
        }

        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        public async Task<DataStyleOutputDto> GetDetailById(string id)
        {
            try
            {
                var query = await _IDataStyleRepository.GetAsync(id);
                var result = query.MapTo<DataStyleOutputDto>();
                result.StyleContent = result.StyleContent.Replace("\\\"", "\"");
                if (result.StyleConfigType == "1")
                {
                    result.StyleInfo = GetStyleInfo(result.StyleContent);
                }
                if (result.StyleConfigType == "2")
                {
                    string layerid = result.StyleDefaultLayer;
                    string fieldid = result.StyleRenderField;
                    LayerFieldEntity entity = _ILayerFieldRepository.GetAll().FirstOrDefault(t => t.LayerID == layerid && t.Id == fieldid);
                    if (entity != null)
                    {
                        result.StyleRenderFieldName = entity.AttributeName;
                        result.RuleDatas = GetRuleDatas(result.StyleContent, entity.AttributeName);
                    }
                    LayerContentEntity lentity = _ILayerContentRepository.GetAll().FirstOrDefault(t => t.Id == layerid);
                    if (lentity != null)
                    {
                        result.StyleDefaultLayerName = lentity.LayerName;
                        result.LayerContent = lentity.MapTo<LayerContentOutputDto>();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataStyleOutputDto> GetDetailByLayerID(string layerID)
        {
            try
            {
                var query = await _IDataStyleRepository.FirstOrDefaultAsync(q => q.StyleDefaultLayer == layerID);
                var result = query.MapTo<DataStyleOutputDto>();
                if (result.StyleConfigType == "1")
                {
                    result.StyleInfo = GetStyleInfo(result.StyleContent);
                }
                if (result.StyleConfigType == "2")
                {
                    string layerid = result.StyleDefaultLayer;
                    string fieldid = result.StyleRenderField;
                    LayerFieldEntity entity = _ILayerFieldRepository.GetAll().FirstOrDefault(t => t.LayerID == layerid && t.Id == fieldid);
                    if (entity != null)
                    {
                        result.RuleDatas = GetRuleDatas(result.StyleContent, entity.AttributeName);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new DataStyleOutputDto();
            }
        }

        private StyleInfo GetStyleInfo(string content)
        {
            content = content.Replace("\\\"", "\"");
            
            StyledLayerDescriptor sld = SLDSerialize.Deserialize(typeof(StyledLayerDescriptor), content) as StyledLayerDescriptor;
            List<Common.Style.Rule> rules = GetRules(sld);
            if (rules != null && rules.Count > 0)
            {
                StyleInfo styleInfo = new StyleInfo();
                if (!string.IsNullOrEmpty(rules[0].MaxScaleDenominator))
                {
                    styleInfo.MaxScaleDenominator = rules[0].MaxScaleDenominator;
                }
                if (!string.IsNullOrEmpty(rules[0].MinScaleDenominator))
                {
                    styleInfo.MinScaleDenominator = rules[0].MinScaleDenominator;
                }
                PolygonStyle pstyle = GetColorString(rules[0]);
                if (rules[0].PointSymbolizer != null && rules[0].PointSymbolizer.Graphic != null)
                {
                    if (rules[0].PointSymbolizer.Graphic.ExternalGraphic != null && rules[0].PointSymbolizer.Graphic.ExternalGraphic.OnlineResource != null)
                    {
                        styleInfo.IconPath = rules[0].PointSymbolizer.Graphic.ExternalGraphic.OnlineResource.href;
                    }
                    styleInfo.IconSize = rules[0].PointSymbolizer.Graphic.Size;
                    styleInfo.IconRotation = rules[0].PointSymbolizer.Graphic.Rotation;
                }

                if (rules[0].LineSymbolizer != null)
                {
                    styleInfo.LineWidth = pstyle.LineWidth;
                    styleInfo.LineTransparency = pstyle.LineTransparency;
                    styleInfo.OutlineColor = pstyle.OutlineColor;
                }
                if (rules[0].PolygonSymbolizer != null)
                {
                    styleInfo.LineWidth = pstyle.LineWidth;
                    styleInfo.LineTransparency = pstyle.LineTransparency;

                    styleInfo.OutlineColor = pstyle.OutlineColor;
                    styleInfo.PolygonColor = pstyle.PolygonColor;
                    styleInfo.FillTransparency = pstyle.FillTransparency;
                }
            }
            return null;
        }

        private List<RuleData> GetRuleDatas(string content, string columnName)
        {
            List<RuleData> ruleList = new List<RuleData>();
            StyledLayerDescriptor sld = SLDSerialize.Deserialize(typeof(StyledLayerDescriptor), content) as StyledLayerDescriptor;
            List<Common.Style.Rule> rules = GetRules(sld);
            if (rules != null && rules.Count > 0)
            {
                foreach (Common.Style.Rule rule in rules)
                {
                    PolygonStyle pstyle = GetColorString(rule);
                    if (pstyle != null)
                    {
                        RuleData ruleData = new RuleData();
                        ruleData.PolygonStyle = JsonConvert.SerializeObject(pstyle);
                        if (rule.Filter != null && rule.Filter.PropertyIsEqualTo != null && rule.Filter.PropertyIsEqualTo.PropertyName == columnName)
                        {
                            ruleData.Value = rule.Filter.PropertyIsEqualTo.Literal;
                        }
                        if (!string.IsNullOrEmpty(rule.Title))
                        {
                            ruleData.Title = rule.Title;
                        }
                        ruleData.Count = rule.Count;
                        ruleList.Add(ruleData);
                    }
                }
            }
            return ruleList;
        }


        /// <summary>
        /// 新增数据
        /// </summary>
        [UnitOfWork(IsDisabled = true)]

        public async Task<DataStyleDto> Insert(DataStyleInputDto input)
        {
            input.Id = Guid.NewGuid().ToString();

            string styleContent = input.StyleContent;
            if (input.StyleConfigType == "1" && input.StyleInfo != null)
            {
                //解析后生成xml内容值
                styleContent = GetSld(input.StyleInfo, input.StyleDataType);
            }
            if (input.StyleConfigType == "2" && input.RuleDatas != null)
            {
                string layerid = input.StyleDefaultLayer;
                string fieldid = input.StyleRenderField;
                //LayerFieldEntity lfentity = _ILayerFieldRepository.GetAll().FirstOrDefault(t => t.LayerID == layerid && t.Id == fieldid);
                if (!string.IsNullOrEmpty(input.StyleRenderFieldName))
                {
                    //解析后生成xml内容值
                    styleContent = GetSld(input.RuleDatas, input.StyleRenderFieldName, input.StyleDataType);
                }
            }
            //input.StyleContent = FormatXml(styleContent);
            input.StyleContent = styleContent;

            DataStyleEntity entity = new DataStyleEntity
            {
                Id = input.Id,
                StyleName = input.StyleName,
                StyleType = input.StyleType,
                StyleContent = input.StyleContent,
                StyleDesc = input.StyleDesc,
                StyleDataType = input.StyleDataType,
                StyleConfigType = input.StyleConfigType,
                CreateDT = DateTime.Now,
                CreateBy = input.CreateBy,
                StyleDefaultLayer = input.StyleDefaultLayer,
                StyleRenderColorBand = input.StyleRenderColorBand,
                StyleRenderField = input.StyleRenderField,
                StyleRenderRule = input.StyleRenderRule
            };
            try
            {
                if (string.IsNullOrEmpty(entity.StyleRenderRule)&& input.StyleInfo!=null)
                {
                    entity.StyleRenderRule = JsonConvert.SerializeObject(input.StyleInfo);
                }
                InsertStyle(input.StyleName, input.StyleContent);
            }
            catch (Exception)
            {

                throw;
            }


            try
            {
                using (var db = new InfoEarthFrameDbContext())
                {
                    var query = db.DataStyle.Add(entity);
                    db.SaveChanges();
                    var result = entity.MapTo<DataStyleDto>();

                    _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateBy, 1003, 1101, 1201, 1511, "(" + input.StyleName + ")");
                    return result;
                }
          
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateBy, 1003, 1101, 1202, 1512, "(" + input.StyleName + ")");
                throw new Exception(ex.Message);
            }
        }

        private string GetSld(StyleInfo sInfo, string type)
        {
            StyledLayerDescriptor sld = new StyledLayerDescriptor();
            UserLayer ul = new UserLayer();
            UserStyle us = new UserStyle();
            FeatureTypeStyle fts = new FeatureTypeStyle();
            fts.FeatureTypeName = "Feature";
            fts.SemanticTypeIdentifiers = new List<string>() { "generic:geometry", "simple" };

            Common.Style.Rule rule = new Common.Style.Rule();
            if (!string.IsNullOrEmpty(sInfo.MaxScaleDenominator))
            {
                rule.MaxScaleDenominator = sInfo.MaxScaleDenominator;
            }
            if (!string.IsNullOrEmpty(sInfo.MinScaleDenominator))
            {
                rule.MinScaleDenominator = sInfo.MinScaleDenominator;
            }

            if (type == "6b6941f1-67a3-11e7-8eb2-005056bb1c7e")
            {
                PointSymbolizer ps = GetPointSymbolizer(sInfo);
                rule.PointSymbolizer = ps;
            }
            else if (type == "7776934c-67a3-11e7-8eb2-005056bb1c7e")
            {
                LineSymbolizer ls = GetLineSymbolizer(sInfo);
                rule.LineSymbolizer = new List<LineSymbolizer>() { ls };
                if (sInfo.LineType == 4)
                {
                    LineSymbolizer ls2 = GetDashLine(sInfo);
                    rule.LineSymbolizer.Add(ls2);
                }
            }
            else
            {
                PolygonSymbolizer ps = GetPolygonSymbolizer(sInfo);
                rule.PolygonSymbolizer = ps;
            }

            fts.Rules = new List<Common.Style.Rule>() { rule };
            us.FeatureTypeStyles = new List<FeatureTypeStyle>() { fts };
            //nl.LayerFeatureConstraints = new FeatureTypeConstraints() { FeatureTypeConstraint = new List<FeatureTypeConstraint>() { new FeatureTypeConstraint() } };
            ul.UserStyles = new List<UserStyle>() { us };
            sld.UserLayers = new List<UserLayer>() { ul };

            string strss = SLDSerialize.Serialize(sld);
            return strss;
        }

        private LineSymbolizer GetDashLine(PolygonStyle sInfo)
        {
            LineSymbolizer ls2 = new LineSymbolizer();
            Stroke stroke = new Stroke();

            List<CssParameter> cssParameters1 = new List<CssParameter>();

            if (!string.IsNullOrEmpty(sInfo.OutlineColor))
            {
                cssParameters1.Add(new CssParameter() { name = "stroke", value = ColorRGBtoHx16(sInfo.OutlineColor) });
            }
            if (sInfo.LineTransparency != null)
            {
                cssParameters1.Add(new CssParameter() { name = "stroke-opacity", value = sInfo.LineTransparency.Value.ToString() });
            }
            if (sInfo.LineWidth != null)
            {
                cssParameters1.Add(new CssParameter() { name = "stroke-width", value = sInfo.LineWidth.Value.ToString() });
            }
            cssParameters1.Add(new CssParameter() { name = "stroke-dasharray", value = "10 10" });

            stroke.CssParameters = cssParameters1;
            ls2.Stroke = stroke;
            return ls2;
        }

        private PolygonSymbolizer GetPolygonSymbolizer(PolygonStyle sInfo)
        {
            PolygonSymbolizer ps = new PolygonSymbolizer();

            if (sInfo.IsOutline)
            {
                Stroke stroke = new Stroke();

                GetStroke(sInfo, stroke);

                ps.Stroke = stroke;
            }
            Fill fill = new Fill();
            if (sInfo.IsFill)
            {
                GetFill(sInfo, fill);

                ps.Fill = fill;
            }
            if (sInfo.IsIcon)
            {
                GraphicFill gf = new GraphicFill();
                Graphic g = new Graphic();

                GetGraphic(sInfo, g);

                gf.Graphic = g;
                fill.GraphicFill = gf;
                ps.Fill = fill;
            }

            return ps;
        }

        private LineSymbolizer GetLineSymbolizer(PolygonStyle sInfo)
        {
            LineSymbolizer ls = new LineSymbolizer();
            Stroke stroke = new Stroke();

            GetStroke(sInfo, stroke);

            ls.Stroke = stroke;
            return ls;
        }

        private PointSymbolizer GetPointSymbolizer(PolygonStyle sInfo)
        {
            PointSymbolizer ps = new PointSymbolizer();
            Graphic g = new Graphic();
            if (sInfo.IsIcon)
            {
                GetGraphic(sInfo, g);
            }
            else
            {
                Mark mark = new Mark();
                mark.WellKnownName = sInfo.PointType;
                Fill fill = new Fill();

                GetFill(sInfo, fill);

                if (!string.IsNullOrEmpty(sInfo.IconSize))
                {
                    g.Size = sInfo.IconSize;
                }
                if (!string.IsNullOrEmpty(sInfo.IconRotation))
                {
                    g.Rotation = sInfo.IconRotation;
                }

                mark.Fill = fill;
                g.Mark = mark;
            }
            ps.Graphic = g;
            return ps;
        }

        private void GetFill(PolygonStyle sInfo, Fill fill)
        {
            List<CssParameter> cssParameters1 = new List<CssParameter>();
            if (!string.IsNullOrEmpty(sInfo.PolygonColor))
            {
                cssParameters1.Add(new CssParameter() { name = "fill", value = ColorRGBtoHx16(sInfo.PolygonColor) });
            }
            if (sInfo.FillTransparency != null)
            {
                cssParameters1.Add(new CssParameter() { name = "fill-opacity", value = sInfo.FillTransparency.Value.ToString() });
            }
            fill.CssParameters = cssParameters1;
        }

        private void GetGraphic(PolygonStyle sInfo, Graphic g)
        {
            if (!string.IsNullOrEmpty(sInfo.IconSize))
            {
                g.Size = sInfo.IconSize;
            }
            if (!string.IsNullOrEmpty(sInfo.IconRotation))
            {
                g.Rotation = sInfo.IconRotation;
            }
            if (!string.IsNullOrEmpty(sInfo.IconPath))
            {
                ExternalGraphic eg = new ExternalGraphic();
                eg.OnlineResource = new OnlineResource() { href = sInfo.IconPath };
                eg.Format = "image/png";
                g.ExternalGraphic = eg;
            }
        }

        private void GetStroke(PolygonStyle sInfo, Stroke stroke)
        {
            //if (sInfo.LineType != 1)
            //{
            switch (sInfo.LineType)
            {
                case 3://点线
                    GetPointStroke(sInfo, stroke);
                    break;
                case 4://虚点线
                    GetPointStrokeO(sInfo, stroke);
                    break;
                default:
                    GetDashStroke(sInfo, stroke);
                    break;
            }
            //}
        }

        private void GetDashStroke(PolygonStyle sInfo, Stroke stroke)
        {
            List<CssParameter> cssParameters1 = new List<CssParameter>();

            if (!string.IsNullOrEmpty(sInfo.OutlineColor))
            {
                cssParameters1.Add(new CssParameter() { name = "stroke", value = ColorRGBtoHx16(sInfo.OutlineColor) });
            }
            if (sInfo.LineTransparency != null)
            {
                cssParameters1.Add(new CssParameter() { name = "stroke-opacity", value = sInfo.LineTransparency.Value.ToString() });
            }
            if (sInfo.LineWidth != null)
            {
                cssParameters1.Add(new CssParameter() { name = "stroke-width", value = sInfo.LineWidth.Value.ToString() });
            }
            if (sInfo.LineType != 1)
            {
                switch (sInfo.LineType)
                {
                    case 2://虚线
                        cssParameters1.Add(new CssParameter() { name = "stroke-dasharray", value = "5 2" });
                        break;
                    case 4://虚点
                        cssParameters1.Add(new CssParameter() { name = "stroke-dasharray", value = "10 10" });
                        break;
                }
            }
            stroke.CssParameters = cssParameters1;
        }

        private void GetPointStroke(PolygonStyle sInfo, Stroke stroke)
        {
            GraphicStroke gs = new GraphicStroke();
            Graphic g = new Graphic();

            string lineColor = "#FFFFFF";
            if (!string.IsNullOrEmpty(sInfo.OutlineColor))
            {
                lineColor = ColorRGBtoHx16(sInfo.OutlineColor);
            }

            string lineWidth = "1";
            if (sInfo.LineWidth != null)
            {
                lineWidth = sInfo.LineWidth.Value.ToString();
            }

            Mark mark = new Mark();

            mark.WellKnownName = "circle";
            mark.Fill = new Fill()
            {
                CssParameters = new List<CssParameter>() {
                    new CssParameter() { name = "fill", value = lineColor}
                }
            };
            mark.Stroke = new Stroke()
            {
                CssParameters = new List<CssParameter>() {
                    new CssParameter() { name = "stroke", value = lineColor},
                    new CssParameter() { name = "stroke-width", value = lineWidth}
                }
            };
            g.Size = "4";
            g.Mark = mark;
            gs.Graphic = g;
            stroke.GraphicStroke = gs;
            stroke.CssParameters = new List<CssParameter>() {
                    new CssParameter() { name = "stroke-dasharray", value = "4 6"}
                };
        }

        private void GetPointStrokeO(PolygonStyle sInfo, Stroke stroke)
        {
            GraphicStroke gs = new GraphicStroke();
            Graphic g = new Graphic();

            string lineColor = "#FFFFFF";
            if (!string.IsNullOrEmpty(sInfo.OutlineColor))
            {
                lineColor = ColorRGBtoHx16(sInfo.OutlineColor);
            }

            string lineWidth = "1";
            if (sInfo.LineWidth != null)
            {
                lineWidth = sInfo.LineWidth.Value.ToString();
            }

            Mark mark = new Mark();

            mark.WellKnownName = "circle";
            mark.Fill = new Fill()
            {
                CssParameters = new List<CssParameter>() {
                    new CssParameter() { name = "fill", value = lineColor}
                }
            };
            mark.Stroke = new Stroke()
            {
                CssParameters = new List<CssParameter>() {
                    new CssParameter() { name = "stroke", value = lineColor},
                    new CssParameter() { name = "stroke-width", value = lineWidth}
                }
            };
            g.Size = "5";
            g.Mark = mark;
            gs.Graphic = g;
            stroke.GraphicStroke = gs;
            stroke.CssParameters = new List<CssParameter>() {
                    new CssParameter() { name = "stroke-dasharray", value = "5 15"},
                    new CssParameter() { name = "stroke-dashoffset", value = "7.5"}
                };
        }

        private string GetSld(List<RuleData> ruleDatas, string columnName, string type)
        {
            StyledLayerDescriptor sld = new StyledLayerDescriptor();
            UserLayer ul = new UserLayer();
            UserStyle us = new UserStyle();
            FeatureTypeStyle fts = new FeatureTypeStyle();
            fts.FeatureTypeName = "Feature";
            fts.SemanticTypeIdentifiers = new List<string>() { "generic:geometry", "simple" };

            List<Common.Style.Rule> ruleList = new List<Common.Style.Rule>();

            int i = 0;
            foreach (RuleData rData in ruleDatas)
            {
                Common.Style.Rule rule = new Common.Style.Rule();
                rule.Name = "rule" + (i++).ToString();
                rule.Count = rData.Count;
                PolygonStyle pstyle = JsonConvert.DeserializeObject<PolygonStyle>(rData.PolygonStyle);
                if (pstyle != null)
                {
                    if (type == "6b6941f1-67a3-11e7-8eb2-005056bb1c7e")
                    {
                        PointSymbolizer ps = GetPointSymbolizer(pstyle);
                        rule.PointSymbolizer = ps;
                    }
                    else if (type == "7776934c-67a3-11e7-8eb2-005056bb1c7e")
                    {
                        LineSymbolizer ls = GetLineSymbolizer(pstyle);
                        rule.LineSymbolizer = new List<LineSymbolizer>() { ls };
                        if (pstyle.LineType == 4)
                        {
                            LineSymbolizer ls2 = GetDashLine(pstyle);
                            rule.LineSymbolizer.Add(ls2);
                        }
                    }
                    else
                    {
                        PolygonSymbolizer ps = GetPolygonSymbolizer(pstyle);
                        rule.PolygonSymbolizer = ps;
                    }
                }
                if (!string.IsNullOrEmpty(rData.Title))
                {
                    rule.Title = rData.Title;
                }
                if (!string.IsNullOrEmpty(rData.Value) && !string.IsNullOrEmpty(columnName))
                {
                    Filter filter = new Filter();
                    filter.PropertyIsEqualTo = new PropertyIsEqualTo() { PropertyName = columnName, Literal = rData.Value };
                    rule.Filter = filter;
                }
                ruleList.Add(rule);
            }


            fts.Rules = ruleList.ToList();
            us.FeatureTypeStyles = new List<FeatureTypeStyle>() { fts };
            //nl.LayerFeatureConstraints = new FeatureTypeConstraints() { FeatureTypeConstraint = new List<FeatureTypeConstraint>() { new FeatureTypeConstraint() } };
            ul.UserStyles = new List<UserStyle>() { us };
            sld.UserLayers = new List<UserLayer>() { ul };

            string strss = SLDSerialize.Serialize(sld);
            return strss;
        }

        //[ValidateInput(false)]
        /// <summary>
        /// 新增样式
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="styleContent"></param>
        public void InsertStyle(string styleName, string styleContent)
        {
            if (string.IsNullOrEmpty(styleName) && HttpContext.Current != null)
            {
                styleName = HttpContext.Current.Request.Form["styleName"];
            }
            if (string.IsNullOrEmpty(styleContent))
            {
                styleContent = HttpContext.Current.Request.Form["styleContent"];
            }
            GeoServerHelper geoHelp = new GeoServerHelper();
            geoHelp.UpdateStyle(styleName, styleContent);
        }

        /// <summary>
        /// 新增样式
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="styleContent"></param>
        public void InsertGeoServerStyle(DataStyleInputDto input)
        {
            GeoServerHelper geoHelp = new GeoServerHelper();
            geoHelp.UpdateStyle(input.StyleName, input.StyleContent);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task<DataStyleDto> Update(DataStyleInputDto input)
        {
            try
            {
                string styleContent = input.StyleContent;
                if (input.StyleConfigType == "1" && input.StyleInfo != null)
                {
                    //解析后生成xml内容值
                    styleContent = GetSld(input.StyleInfo, input.StyleDataType);
                }
                if (input.StyleConfigType == "2" && input.RuleDatas != null)
                {
                    string layerid = input.StyleDefaultLayer;
                    string fieldid = input.StyleRenderField;
                    if (!string.IsNullOrEmpty(input.StyleRenderFieldName))
                    {
                        //解析后生成xml内容值
                        styleContent = GetSld(input.RuleDatas, input.StyleRenderFieldName, input.StyleDataType);
                    }
                }
                //input.StyleContent = FormatXml(styleContent);
                input.StyleContent = styleContent;

                DataStyleEntity entity = new DataStyleEntity
                {
                    Id = input.Id,
                    StyleName = input.StyleName,
                    StyleType = input.StyleType,
                    StyleContent = input.StyleContent,
                    StyleDesc = input.StyleDesc,
                    StyleDataType = input.StyleDataType,
                    StyleConfigType = input.StyleConfigType,
                    CreateDT = DateTime.Now,
                    CreateBy = input.CreateBy,
                    StyleDefaultLayer = input.StyleDefaultLayer,
                    StyleRenderColorBand = input.StyleRenderColorBand,
                    StyleRenderField = input.StyleRenderField,
                    StyleRenderRule = input.StyleRenderRule
                };
                if (string.IsNullOrEmpty(entity.StyleRenderRule) && input.StyleInfo != null)
                {
                    entity.StyleRenderRule = JsonConvert.SerializeObject(input.StyleInfo);
                }

                var db=(InfoEarthFrameDbContext)_IDataStyleRepository.GetDbContext();
             //   var query = await _IDataStyleRepository.UpdateAsync(entity);
                var old = db.DataStyle.FirstOrDefault(p => p.Id == input.Id);
                if (old != null)
                {
                    old.StyleName = input.StyleName;
                    old.StyleDataType = input.StyleDataType;
                    old.StyleDesc = input.StyleDesc;
                    old.StyleContent = input.StyleContent;

                    db.Entry(old).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var result = entity.MapTo<DataStyleDto>();

                InsertStyle(result.StyleName, result.StyleContent);

                _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateBy, 1003, 1102, 1201, 1521, "(" + input.StyleName + ")");
                return result;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(input.Id, input.CreateBy, 1003, 1102, 1202, 1522, "(" + input.StyleName + ")");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task<bool> Delete(string id, string user)
        {
            bool bFlag = false;
            try
            {

                var query = _IDataStyleRepository.Get(id);
                var result = query.MapTo<DataStyleDto>();

               // await _IDataStyleRepository.DeleteAsync(id);

                var db = (InfoEarthFrameDbContext)_IDataStyleRepository.GetDbContext();
                var old = db.DataStyle.FirstOrDefault(p => p.Id == id);
                if (old != null)
                {
                    db.DataStyle.Remove(old);
                    db.SaveChanges();
                }

                try
                {
                    GeoServerHelper geoHelp = new GeoServerHelper();
                    geoHelp.DeleteStyle(result.StyleName);
                }
                catch
                {

              
                }

                LayerContentEntity entity = _ILayerContentRepository.GetAll().FirstOrDefault(t => t.LayerDefaultStyle == id);
                if (entity != null)
                {
                    entity.LayerDefaultStyle = null;
                    _ILayerContentRepository.Update(entity);
                }

                _IOperateLogAppService.WriteOperateLog(id, user, 1003, 1105, 1201, 1531, null);
                bFlag = true;
            }
            catch (Exception ex)
            {
                _IOperateLogAppService.WriteOperateLog(id, user, 1003, 1105, 1202, 1532, null);
                bFlag = false;
            }
            return bFlag;
        }

        /// <summary>
        /// 获取随机颜色带
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public ListResultOutput<ColorRampInfo> GetRandomColorRamps()
        {
            ColorRampPlus rampUtility = new ColorRampPlus();
            List<ColorRampInfo> colorRamps = rampUtility.GetRandomColorRamps(rampUtility.RandomBestColorNum);

            return new ListResultOutput<ColorRampInfo>(colorRamps);
        }

        /// <summary>
        /// 获取随机颜色带
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public ColorRampInfo GetRandomColorRampsByName(string colorName, int count)
        {
            ColorRampPlus rampUtility = new ColorRampPlus();
            ColorRampInfo colorRamp = rampUtility.GetRandomColorRamp(colorName, count);

            return colorRamp;
        }

        /// <summary>
        /// 获取渐变颜色带
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public ListResultOutput<ColorRampInfo> GetLinearColorRamps()
        {
            List<ColorRampInfo> colorRamps = ColorRampPlusStatic.ColorRampPlus.GetLinearColorRamps(ColorRampPlusStatic.ColorRampPlus.LinearBestColorNum);

            return new ListResultOutput<ColorRampInfo>(colorRamps);
        }

        /// <summary>
        /// 获取渐变颜色带
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public ColorRampInfo GetLinearColorRampsByName(string colorName, int count)
        {
            ColorRampInfo colorRamp = ColorRampPlusStatic.ColorRampPlus.GetLinearColorRamp(colorName, count);

            return colorRamp;
        }

        /// <summary>
        /// 获取所有文件夹目录
        /// </summary>
        public List<string> GetAllFolders()
        {
            List<string> list = new List<string>();
            list.Add(UtilityMessageConvert.Get("点图片"));
            list.Add(UtilityMessageConvert.Get("面图片"));

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (string str in list)
            {
                string folder = Path.Combine(path, str);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            string[] listFolder = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            List<string> listF = new List<string>();
            foreach (string str in listFolder)
            {
                listF.Add(str.Replace(path, "").Replace("\\", ""));
            }

            return listF;
        }

        /// <summary>
        /// 新增文件夹
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <returns></returns>
        public bool AddFolder(string folderName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style");

                string folder = Path.Combine(path, folderName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.Logger.Error(ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// 修改文件夹名称
        /// </summary>
        /// <param name="oldName">旧名称</param>
        /// <param name="newName">新名称</param>
        /// <returns></returns>
        public bool UpdateFolder(string oldName, string newName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style");

                string folder = Path.Combine(path, oldName);
                string nfolder = Path.Combine(path, newName);
                if (Directory.Exists(folder) && !Directory.Exists(nfolder))
                {
                    Directory.Move(folder, nfolder);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.Logger.Error(ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <returns></returns>
        public bool DeleteFolder(string folderName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style");

                string folder = Path.Combine(path, folderName);
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.Logger.Error(ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// 获取指定目录所有文件
        /// </summary>
        /// <param name="folder">文件夹名称</param>
        /// <returns></returns>
        public List<DataStyleOutputImgFileInfo> GetAllFiles(string folderName)
        {
            List<DataStyleOutputImgFileInfo> list = new List<DataStyleOutputImgFileInfo>();
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style");
                path = Path.Combine(path, folderName);
                if (Directory.Exists(path))
                {
                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
                    DirectoryInfo folder = new DirectoryInfo(path);
                    foreach (FileInfo file in folder.GetFiles("*.*", SearchOption.TopDirectoryOnly))
                    {
                        string hpptPath = url + "/" + file.Directory.Parent.Name + "/" + file.Directory.Name + "/" + file.Name;
                        DataStyleOutputImgFileInfo info = new DataStyleOutputImgFileInfo
                        {
                            Name = file.Name,
                            Path = hpptPath
                        };
                        list.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.Logger.Error(ex.Message, ex);
            }
            return list;
        }

        /// <summary>
        /// 修改文件名称
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="oldName">旧文件名称</param>
        /// <param name="newName">新文件名称</param>
        /// <returns></returns>
        public bool UpdateFile(string folderName, string oldName, string newName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style");
                path = Path.Combine(path, folderName);
                string folder = Path.Combine(path, oldName);
                string nfolder = Path.Combine(path, newName);
                if (File.Exists(folder) && !File.Exists(nfolder))
                {
                    File.Move(folder, nfolder);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.Logger.Error(ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        ///  删除指定文件
        /// </summary>
        /// <param name="folder">文件夹名称</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public bool DeleteFile(string folderName, string fileName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style");

                string folder = Path.Combine(path, folderName);
                if (Directory.Exists(folder))
                {
                    string filePath = Path.Combine(folder, fileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.Logger.Error(ex.Message, ex);
            }
            return false;
        }


        #endregion

        /// <summary>
        /// 分页获取图层属性数据
        /// </summary>
        /// <param name="layerId">图层id</param>
        /// <param name="layerAttr">属性名</param>
        /// <param name="colorName">色带名称</param>
        /// <param name="style">颜色字符串</param>
        /// <returns></returns>
        public string GetDataAttributesPage(string layerId, string layerAttr, string colorName, string style, int PageSize, int PageIndex)
        {
            DataTable newdt = new DataTable();
            DataTable dt = QueryAttribute(layerId, layerAttr, colorName, style);
            if (dt != null && dt.Rows.Count > 0)
            {
                newdt = dt.AsEnumerable().Skip((PageIndex - 1) * PageSize).Take(PageSize).CopyToDataTable();
            }
            return Json.ToJson(new { Count = dt.Rows.Count, DataTableJson = newdt });
        }

        /// <summary>
        /// 获取图层属性数据
        /// </summary>
        /// <param name="layerId">图层id</param>
        /// <param name="layerAttr">属性名</param>
        /// <param name="colorName">色带名称</param>
        /// <param name="style">颜色字符串</param>
        /// <returns></returns>
        public object GetDataAttributes(string layerId, string layerAttr, string colorName, string style)
        {
            DataTable dt = QueryAttribute(layerId, layerAttr, colorName, style);
            return DataTableConvertJson.DataTable2Json("table", dt);
        }

        private DataTable QueryAttribute(string layerId, string layerAttr, string colorName, string style)
        {
            DataTable dt = null;
            LayerContentEntity layerContent = _ILayerContentRepository.Get(layerId);
            LayerFieldEntity layerField = _ILayerFieldRepository.GetAll().First(t => t.LayerID == layerId && t.AttributeName == layerAttr);
            if (layerContent != null && layerField != null && !string.IsNullOrEmpty(layerContent.LayerAttrTable))
            {
                try
                {
                    string sql = string.Format("SELECT cast({0} as char) {2},{0} {3},{0} {4},COUNT(*) {5} FROM {1} GROUP BY {0}", layerAttr, layerContent.LayerAttrTable
                        , UtilityMessageConvert.Get("符号"), UtilityMessageConvert.Get("值"), UtilityMessageConvert.Get("注记"), UtilityMessageConvert.Get("个数"));
                    //MySqlHelper mysql = new MySqlHelper();
                    //dt = mysql.ExecuteQuery(sql);
                    PostgrelVectorHelper postgis = new PostgrelVectorHelper();
                    dt = postgis.getDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt.Columns[0].DataType = typeof(string);
                        dt.Columns[0].MaxLength = int.MaxValue;
                        PolygonStyle pstyle = new PolygonStyle();
                        if (!string.IsNullOrEmpty(style))
                        {
                            try
                            {
                                pstyle = JsonConvert.DeserializeObject<PolygonStyle>(style);
                            }
                            catch (Exception ex)
                            { }
                        }

                        ColorRampInfo colorRamp = ColorRampPlusStatic.ColorRampPlus.GetRampInfo(colorName, dt.Rows.Count);

                        if (colorRamp != null)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                int ci = i % colorRamp.Count;
                                pstyle.PolygonColor = string.Format("{0},{1},{2}", colorRamp[ci].R, colorRamp[ci].G, colorRamp[ci].B);
                                pstyle.OutlineColor = pstyle.PolygonColor;
                                dt.Rows[i][0] = JsonConvert.SerializeObject(pstyle);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Abp.Logging.LogHelper.Logger.Error(ex.Message, ex);
                }
            }
            if (dt == null)
            {
                dt = new DataTable();
            }

            return dt;
        }

        public object GetDataAttributesById(string styleId)
        {
            Dictionary<string, PolygonStyle> dicValues = new Dictionary<string, PolygonStyle>();
            DataTable dt = null;
            DataStyleEntity entity = _IDataStyleRepository.Get(styleId);
            if (entity != null)
            {
                dt = QueryAttribute(entity.StyleDefaultLayer, entity.StyleRenderField, entity.StyleRenderColorBand, entity.StyleRenderRule);
                if (dt != null && !string.IsNullOrEmpty(entity.StyleContent))
                {
                    StyledLayerDescriptor sld = SLDSerialize.Deserialize(typeof(StyledLayerDescriptor), entity.StyleContent) as StyledLayerDescriptor;

                    List<Common.Style.Rule> rules = GetRules(sld);
                    if (rules != null)
                    {
                        foreach (var rule in rules)
                        {
                            if (rule.Filter != null)
                            {
                                if (rule.Filter.PropertyIsEqualTo != null && rule.Filter.PropertyIsEqualTo.PropertyName == entity.StyleRenderField)
                                {
                                    string value = rule.Filter.PropertyIsEqualTo.Literal;
                                    //conditions.Add(rule.Filter.PropertyIsEqualTo);
                                    PolygonStyle pstyle = GetColorString(rule);
                                    if (pstyle != null && !dicValues.ContainsKey(value))
                                    {
                                        dicValues.Add(value, pstyle);
                                    }
                                }
                            }
                        }
                    }
                    if (dicValues.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string value = row[1].ToString();
                            string style = row[0].ToString();

                            if (dicValues.ContainsKey(value))
                            {
                                row[0] = JsonConvert.SerializeObject(dicValues[value]);
                            }
                        }
                    }
                }
            }
            if (dt == null)
            {
                dt = new DataTable();
            }
            return DataTableConvertJson.DataTable2Json("table", dt);
        }

        [UnitOfWork(IsDisabled = true)]
        public string GetXmlContent(DataStyleInputDto input)
        {
            string styleContent = input.StyleContent;
            if (input.StyleConfigType == "1" && input.StyleInfo != null)
            {
                //解析后生成xml内容值
                styleContent = GetSld(input.StyleInfo, input.StyleDataType);
            }
            if (input.StyleConfigType == "2" && input.RuleDatas != null)
            {
                //解析后生成xml内容值
                styleContent = GetSld(input.RuleDatas, input.StyleRenderFieldName, input.StyleDataType);
            }
            return FormatXml(styleContent);
        }
        private string FormatXml(string sUnformattedXml)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(sUnformattedXml);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = System.Xml.Formatting.Indented;
                xtw.Indentation = 1;
                xtw.IndentChar = '\t';
                xd.WriteTo(xtw);
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }


        private List<Common.Style.Rule> GetRules(StyledLayerDescriptor sld)
        {
            List<Common.Style.Rule> rules = new List<Common.Style.Rule>();
            if (sld != null)
            {
                List<UserStyle> usList = new List<UserStyle>();
                List<FeatureTypeStyle> ftsList = new List<FeatureTypeStyle>();
                if (sld.NamedLayers != null)
                {
                    foreach (var nl in sld.NamedLayers)
                    {
                        if (nl.UserStyles != null && nl.UserStyles.Count > 0)
                        {
                            usList.AddRange(nl.UserStyles.ToArray());
                        }
                    }
                }
                if (sld.UserLayers != null)
                {
                    foreach (var ul in sld.UserLayers)
                    {
                        if (ul.UserStyles != null && ul.UserStyles.Count > 0)
                        {
                            usList.AddRange(ul.UserStyles.ToArray());
                        }
                    }
                }

                foreach (var us in usList)
                {
                    if (us.FeatureTypeStyles != null && us.FeatureTypeStyles.Count > 0)
                    {
                        foreach (var fts in us.FeatureTypeStyles)
                        {
                            if (fts.Rules != null && fts.Rules.Count > 0)
                            {
                                rules.AddRange(fts.Rules.ToArray());
                            }
                        }
                    }
                }
            }
            return rules;
        }

        private List<Condition> GetConditions(List<Common.Style.Rule> rules)
        {
            List<Condition> conditions = new List<Condition>();
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    if (rule.Filter != null)
                    {
                        if (rule.Filter.PropertyIsEqualTo != null)
                        {
                            conditions.Add(rule.Filter.PropertyIsEqualTo);
                        }
                    }
                }
            }
            return conditions;

        }

        private PolygonStyle GetColorString(Common.Style.Rule rule)
        {
            PolygonStyle ps = new PolygonStyle();
            List<CssParameter> list = new List<CssParameter>();
            if (rule != null)
            {
                if (rule.PointSymbolizer != null && rule.PointSymbolizer.Graphic != null && rule.PointSymbolizer.Graphic.Mark != null)
                {
                    if (rule.PointSymbolizer.Graphic.Mark.Fill != null)
                    {
                        ps = InitColor(ps, rule.PointSymbolizer.Graphic.Mark.Fill.CssParameters, "0");
                    }

                    if (rule.PointSymbolizer.Graphic.Mark.Stroke != null)
                    {
                        ps = InitColor(ps, rule.PointSymbolizer.Graphic.Mark.Stroke.CssParameters, "1");
                    }
                }
                if (rule.LineSymbolizer != null && rule.LineSymbolizer.Count > 0 && rule.LineSymbolizer[0].Stroke != null)
                {
                    ps = InitColor(ps, rule.LineSymbolizer[0].Stroke.CssParameters, "1");
                }

                if (rule.PolygonSymbolizer != null)
                {
                    if (rule.PolygonSymbolizer.Stroke != null)
                    {
                        ps = InitColor(ps, rule.PolygonSymbolizer.Stroke.CssParameters, "1");
                    }
                    if (rule.PolygonSymbolizer.Fill != null)
                    {
                        ps = InitColor(ps, rule.PolygonSymbolizer.Fill.CssParameters, "0");
                    }
                }
            }
            return ps;
        }

        private PolygonStyle InitColor(PolygonStyle pstyle, List<CssParameter> list, string type)
        {
            if (list != null)
            {
                if (type == "1")
                {
                    string stroke = GetValue(list, "stroke");
                    string strokeo = GetValue(list, "stroke-opacity");
                    string strokew = GetValue(list, "stroke-width");

                    pstyle.IsOutline = true;
                    if (!string.IsNullOrEmpty(strokeo))
                    {
                        pstyle.LineTransparency = OpacityToByte(strokeo);
                    }
                    if (!string.IsNullOrEmpty(stroke))
                    {
                        pstyle.OutlineColor = ColorHx16toRGB(stroke);
                    }
                    double d;
                    if (!string.IsNullOrEmpty(strokew) && double.TryParse(strokew, out d))
                    {
                        pstyle.LineWidth = d;
                    }
                }
                else
                {
                    string fill = GetValue(list, "fill");
                    string fillo = GetValue(list, "fill-opacity");
                    pstyle.IsFill = true;
                    if (!string.IsNullOrEmpty(fillo))
                    {
                        pstyle.FillTransparency = OpacityToByte(fillo);
                    }
                    if (!string.IsNullOrEmpty(fill))
                    {
                        pstyle.PolygonColor = ColorHx16toRGB(fill);
                    }
                }
            }
            return pstyle;
        }

        private string GetValue(List<CssParameter> list, string name)
        {
            CssParameter cp = list.FirstOrDefault(t => !string.IsNullOrEmpty(t.name) && t.name.ToLower() == name);
            if (cp != null)
            {
                return cp.value;
            }
            return null;
        }

        private double OpacityToByte(string value)
        {
            double f = 1;
            double.TryParse(value, out f);

            //return Math.Ceiling(f * 255);
            return f;
        }

        #region [颜色：16进制转成RGB]

        /// <summary>
        /// [颜色：16进制转成RGB]
        /// </summary>
        /// <param name="strColor">设置16进制颜色 [返回RGB]</param>
        /// <returns></returns>
        private string ColorHx16toRGB(string strHxColor)
        {
            try
            {
                if (strHxColor.Length == 0)
                {
                    return string.Empty;
                }
                else
                {
                    Color c = System.Drawing.Color.FromArgb(System.Int32.Parse(strHxColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier), System.Int32.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier), System.Int32.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                    //转换颜色
                    return string.Format("{0},{1},{2}", c.R, c.G, c.B);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ColorRGBtoHx16(string str)
        {
            try
            {
                str = str.Trim().ToLower();
                if (str.Contains("rgb") && str.Contains(","))
                {
                    str = str.Substring(str.IndexOf("(") + 1, str.IndexOf(")") - str.IndexOf("(") - 1);
                    string[] values = str.Split(',');
                    return ToHexColor(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2]));

                }
                else if (str.Contains(",") && str.IndexOf("(") < 0 && str.IndexOf(")") < 0)
                {
                    string[] values = str.Split(',');
                    return ToHexColor(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2]));
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }

        private string ToHexColor(int r, int g, int b)
        {
            string R = Convert.ToString(r, 16);
            if (R == "0")
                R = "00";
            string G = Convert.ToString(g, 16);
            if (G == "0")
                G = "00";
            string B = Convert.ToString(b, 16);
            if (B == "0")
                B = "00";
            string HexColor = "#" + R + G + B;
            return HexColor;
        }

        #endregion


        public class RuleData
        {
            public string PolygonStyle;
            public string Value;
            public string Title;
            public string Count;
        }

        public class StyleInfo : PolygonStyle
        {
            /// <summary>
            /// 最小比例尺
            /// </summary>
            public string MinScaleDenominator;

            /// <summary>
            /// 最大比例尺
            /// </summary>
            public string MaxScaleDenominator;

        }

        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        public async Task<PagedResultOutput<DataStyleOutputDto>> GetDataStylePageListByCondition(QueryDataStyleInputParamDto input)
        {
            try
            {
                IEnumerable<DataStyleEntity> query = _IDataStyleRepository.GetAll();

                // 条件过滤
                if (input.StyleName != null && input.StyleName.Trim().Length > 0)
                {
                    query = query.Where(p => p.StyleName.ToUpper().Contains(input.StyleName.ToUpper()));
                }
                if (input.StyleType != null && input.StyleType.Trim().Length > 0)
                {
                    query = query.Where(p => p.StyleType.ToUpper().Contains(input.StyleName.ToUpper()));
                }
                if (input.Createby != null && input.Createby.Trim().Length > 0)
                {
                    query = query.Where(p => p.CreateBy.ToUpper().Contains(input.Createby.ToUpper()));
                }
                if (input.StartDate != null)
                {
                    query = query.Where(s => s.CreateDT >= input.StartDate.Value.AddHours(-1));
                }
                if (input.EndDate != null)
                {
                    query = query.Where(s => s.CreateDT <= input.EndDate.Value.AddDays(1).AddHours(-1));
                }

                int count = 0;
                List<DataStyleEntity> result = null;
                if (query != null)
                {
                    count = query.Count();
                    result = query.OrderByDescending(p => p.CreateDT).Skip((input.pageIndex - 1) * input.pageSize).Take(input.pageSize).ToList();
                }

                IReadOnlyList<DataStyleOutputDto> ir;
                if (result != null)
                {
                    ir = result.MapTo<List<DataStyleOutputDto>>();
                }
                else
                {
                    ir = new List<DataStyleOutputDto>();
                }
                PagedResultOutput<DataStyleOutputDto> outputList = new PagedResultOutput<DataStyleOutputDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        //public async Task<DataStyleDto> Insert(DataStyleDto input)
        //{
        //    try
        //    {
        //        DataStyleEntity entity = new DataStyleEntity
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            StyleName = input.StyleName,
        //            StyleContent = input.StyleContent,
        //            StyleDesc = input.StyleDesc,
        //            CreateDT = DateTime.Now,
        //            CreateBy = input.CreateBy,
        //            StyleType = input.StyleType,
        //            StyleDataType = input.StyleDataType,
        //            StyleConfigType = input.StyleConfigType,
        //            StyleDefaultLayer = input.StyleDefaultLayer,
        //            StyleRenderField = input.StyleRenderField,
        //            StyleRenderColorBand = input.StyleRenderColorBand,
        //            StyleRenderRule = input.StyleRenderRule
        //        };

        //        var db = _IDataStyleRepository.GetDbContext();
        //        var sql = _IDataStyleRepository.GenerateInsertSql(entity);
        //        var flag = (await db.Database.ExecuteSqlCommandAsync(sql)) > 0;
        //        if (!flag)
        //        {
        //            return null;
        //        }
        //        var result = entity.MapTo<DataStyleDto>();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        /// <summary>
        /// 更新数据
        /// </summary>
        //public async Task<DataStyleDto> Update(DataStyleDto input)
        //{
        //    try
        //    {
        //        var oldModel = await GetDetailById(input.Id);
        //        if (oldModel == null)
        //        {
        //            throw new Exception("表[sdms_user]未找到[Id='" + input.Id + "']的数据");
        //        }

        //        oldModel.Id = input.Id;
        //        oldModel.StyleName = input.StyleName;
        //        oldModel.StyleContent = input.StyleContent;
        //        oldModel.StyleDesc = input.StyleDesc;
        //        oldModel.CreateDT = DateTime.Now;
        //        oldModel.CreateBy = input.CreateBy;
        //        oldModel.StyleType = input.StyleType;
        //        oldModel.StyleDataType = input.StyleDataType;
        //        oldModel.StyleConfigType = input.StyleConfigType;
        //        oldModel.StyleDefaultLayer = input.StyleDefaultLayer;
        //        oldModel.StyleRenderField = input.StyleRenderField;
        //        oldModel.StyleRenderColorBand = input.StyleRenderColorBand;
        //        oldModel.StyleRenderRule = input.StyleRenderRule;

        //        var entity = oldModel.MapTo<DataStyleEntity>();
        //        var sql = _IDataStyleRepository.GenerateUpdateSql(entity);
        //        var db = _IDataStyleRepository.GetDbContext();
        //        var flag = (await db.Database.ExecuteSqlCommandAsync(sql)) > 0;
        //        if (!flag)
        //        {
        //            return null;
        //        }
        //        var result = entity.MapTo<SystemUserDto>();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}



        public string GetSLDContentByLayerIdOrTableName(string layerId, string tableName)
        {
            var sql = string.Format(@"select stylename from sdms_datastyle s
 join sdms_layer l
on l.layerdefaultstyle=s.id
where l.id='{0}'
UNION
select stylename from sdms_datastyle s
 join sdms_layer l
on l.layerdefaultstyle=s.id
where l.layerattrtable='{1}'", layerId, tableName);
            var db = (InfoEarthFrameDbContext)_IDataStyleRepository.GetDbContext();
            var styleContent = db.Database.SqlQuery<string>(sql).FirstOrDefault();
            if (string.IsNullOrEmpty(styleContent))
            {
                //获取layername
                var layer = db.LayerContent.FirstOrDefault(m => m.Id == layerId);
                if (layer == null)
                {
                    layer = db.LayerContent.FirstOrDefault(m => m.LayerAttrTable == tableName);
                }
                if (layer != null)
                {
                    layerId = layer.Id;
                    string layerName = layer.LayerName;

                    //根据name获取style
                    var style = db.DataStyle.FirstOrDefault(p => !string.IsNullOrEmpty(p.StyleName) && p.StyleName.ToLower() == layerName.ToLower());
                    if (style == null)
                    {
                        return "";
                    }


                    _layerContentAppService.UpdateDefaultStyle(layerId, style.Id, "admin", db);
                    return style.StyleContent;
                }
            }
            return styleContent;
        }

        public bool IsDataStyleExists(string styleName)
        {
            var dataStyleName = _IDataStyleRepository.FirstOrDefault(s => !string.IsNullOrEmpty(s.StyleName) && s.StyleName.ToLower() == styleName.ToLower());
            return dataStyleName != null;
        }
    }
}

