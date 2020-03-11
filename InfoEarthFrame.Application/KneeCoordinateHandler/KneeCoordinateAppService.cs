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
using System.IO;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Application
{
    //[AbpAuthorize]
    public class KneeCoordinateAppService : ApplicationService, IKneeCoordinateAppService
    {
        /// <summary>
        /// 通过转换获得地理坐标列表（经、纬度）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public KneeCoordinateOutput GeoCoordinateList(KneeCoordinateInput input)
        {
            KneeCoordinateOutput output = new KneeCoordinateOutput
            {
                status = false,
                msg = "数据是空的，坐标转换失败！"
            };
            if (!input.IsProjectiveGeo)//过滤地理坐标的情况
            {
                output.msg = "已经是地理坐标，不需要转换，仅提供投影转地理坐标的情况！";
                output.data = input.InitialData;
               return output;
            }

            List<KneeCoordinateModal> list = new List<KneeCoordinateModal>();


            if (input != null && input.InitialData.Count != 0)
            {
                object fromCSType = null;
                object toCSType = null;

                if (input.CoordSys == "北京54坐标系")
                {
                    toCSType = CSType.GCS_Beijing_1954;
                }
                else if (input.CoordSys == "西安80坐标系")
                {
                    toCSType = CSType.GCS_Xian_1980;
                }
                //else if (input.CoordSys == "国家2000坐标系"):
                //{
                //    toCSType = CSType.GCS_COUNTRY_2000;                    
                //}
                //else if (input.CoordSys == "WGS84坐标系"):
                //{
                //    toCSType = CSType.GCS_WGS_84;                    
                //}
                if (toCSType == null)
                {
                    output.msg = "toCSType为null,坐标转换失败！";
                    return output;
                }

                int i = 1;
                foreach (KneeCoordinateModal item in input.InitialData)
                {
                    if (item.CoordinateY == 0.0)
                    {
                        output.msg = "第" + i + "行Y坐标值为0，坐标转换失败！";
                        output.data = null;
                        return output;
                    }
                    fromCSType = SelectCSType(item.CoordinateY, input.CoordSys);
                    if (fromCSType == null)
                    {
                        output.msg = "第" + i + "行Y坐标值错误导致fromCSType为null，坐标转换失败！";
                        output.data = null;
                        return output;
                    }
                    var point = CSWKTUtility.TransForm(item.CoordinateX, item.CoordinateY, (CSType)fromCSType, (CSType)toCSType);
                    if (point != null)
                    {
                        KneeCoordinateModal kcm = new KneeCoordinateModal
                        {
                            CoordinateX = point[0],
                            CoordinateY = point[1]
                        };
                        list.Add(kcm);
                    }
                    i++;
                }

                if (list[0] != list[list.Count - 1])
                {
                    //尾巴补齐：首尾坐标要重合
                    list.Add(list[0]);
                }
                output.status = true;
                output.msg = "坐标转换成功！";
                output.data = list;
            }
            return output;
        }

        private object SelectCSType(double CoordinateY, string coordSys)
        {
            object obj = null;
            string str = CoordinateY.ToString().Substring(0, 2);
            int num = -99;
            Int32.TryParse(str, out num);
            if (num == -99) { return null; }

            if (coordSys == "北京54坐标系")
            {
                obj = SwicthBeijing(num);
            }
            else if (coordSys == "西安80坐标系")
            {
                obj = SwicthXian(num);
            }
            else if (coordSys == "国家2000坐标系")
            {
                obj = SwicthXian(num);
            }
            else if (coordSys == "WGS84坐标系")
            {
                obj = SwicthXian(num);
            }
            return obj;
        }
        /// <summary>
        /// 北京54坐标系
        /// 注意：3度分带:y坐标前两位数为 25—45 之间  6度分带:y坐标前两位数为 13—23 之间  
        /// </summary>
        /// <param name="num"></param>
        private object SwicthBeijing(int num)
        {
            object obj = null;
            #region switch 选择
            switch (num)
            {
                case 13: obj = CSType.Beijing_1954_GK_Zone_13;
                    break;
                case 14: obj = CSType.Beijing_1954_GK_Zone_14;
                    break;
                case 15: obj = CSType.Beijing_1954_GK_Zone_15;
                    break;
                case 16: obj = CSType.Beijing_1954_GK_Zone_16;
                    break;
                case 17: obj = CSType.Beijing_1954_GK_Zone_17;
                    break;
                case 18: obj = CSType.Beijing_1954_GK_Zone_18;
                    break;
                case 19: obj = CSType.Beijing_1954_GK_Zone_19;
                    break;
                case 20: obj = CSType.Beijing_1954_GK_Zone_20;
                    break;
                case 21: obj = CSType.Beijing_1954_GK_Zone_21;
                    break;
                case 22: obj = CSType.Beijing_1954_GK_Zone_22;
                    break;
                case 23: obj = CSType.Beijing_1954_GK_Zone_23;
                    break;

                case 25: obj = CSType.Beijing_1954_3_Degree_GK_Zone_25;
                    break;
                case 26: obj = CSType.Beijing_1954_3_Degree_GK_Zone_26;
                    break;
                case 27: obj = CSType.Beijing_1954_3_Degree_GK_Zone_27;
                    break;
                case 28: obj = CSType.Beijing_1954_3_Degree_GK_Zone_28;
                    break;
                case 29: obj = CSType.Beijing_1954_3_Degree_GK_Zone_29;
                    break;
                case 30: obj = CSType.Beijing_1954_3_Degree_GK_Zone_30;
                    break;
                case 31: obj = CSType.Beijing_1954_3_Degree_GK_Zone_31;
                    break;
                case 32: obj = CSType.Beijing_1954_3_Degree_GK_Zone_32;
                    break;
                case 33: obj = CSType.Beijing_1954_3_Degree_GK_Zone_33;
                    break;
                case 34: obj = CSType.Beijing_1954_3_Degree_GK_Zone_34;
                    break;
                case 35: obj = CSType.Beijing_1954_3_Degree_GK_Zone_35;
                    break;
                case 36: obj = CSType.Beijing_1954_3_Degree_GK_Zone_36;
                    break;
                case 37: obj = CSType.Beijing_1954_3_Degree_GK_Zone_37;
                    break;
                case 38: obj = CSType.Beijing_1954_3_Degree_GK_Zone_38;
                    break;
                case 39: obj = CSType.Beijing_1954_3_Degree_GK_Zone_39;
                    break;
                case 40: obj = CSType.Beijing_1954_3_Degree_GK_Zone_40;
                    break;
                case 41: obj = CSType.Beijing_1954_3_Degree_GK_Zone_41;
                    break;
                case 42: obj = CSType.Beijing_1954_3_Degree_GK_Zone_42;
                    break;
                case 43: obj = CSType.Beijing_1954_3_Degree_GK_Zone_43;
                    break;
                case 44: obj = CSType.Beijing_1954_3_Degree_GK_Zone_44;
                    break;
                case 45: obj = CSType.Beijing_1954_3_Degree_GK_Zone_45;
                    break;
            }
            #endregion
            return obj;
        }
        /// <summary>
        /// 西安80坐标系
        /// 注意：3度分带:y坐标前两位数为 25—45 之间  6度分带:y坐标前两位数为 13—23 之间  
        /// </summary>
        /// <param name="num"></param>
        private object SwicthXian(int num)
        {
            object obj = null;
            #region switch 选择
            switch (num)
            {
                case 13: obj = CSType.Xian_1980_GK_Zone_13;
                    break;
                case 14: obj = CSType.Xian_1980_GK_Zone_14;
                    break;
                case 15: obj = CSType.Xian_1980_GK_Zone_15;
                    break;
                case 16: obj = CSType.Xian_1980_GK_Zone_16;
                    break;
                case 17: obj = CSType.Xian_1980_GK_Zone_17;
                    break;
                case 18: obj = CSType.Xian_1980_GK_Zone_18;
                    break;
                case 19: obj = CSType.Xian_1980_GK_Zone_19;
                    break;
                case 20: obj = CSType.Xian_1980_GK_Zone_20;
                    break;
                case 21: obj = CSType.Xian_1980_GK_Zone_21;
                    break;
                case 22: obj = CSType.Xian_1980_GK_Zone_22;
                    break;
                case 23: obj = CSType.Xian_1980_GK_Zone_23;
                    break;

                case 25: obj = CSType.Xian_1980_3_Degree_GK_Zone_25;
                    break;
                case 26: obj = CSType.Xian_1980_3_Degree_GK_Zone_26;
                    break;
                case 27: obj = CSType.Xian_1980_3_Degree_GK_Zone_27;
                    break;
                case 28: obj = CSType.Xian_1980_3_Degree_GK_Zone_28;
                    break;
                case 29: obj = CSType.Xian_1980_3_Degree_GK_Zone_29;
                    break;
                case 30: obj = CSType.Xian_1980_3_Degree_GK_Zone_30;
                    break;
                case 31: obj = CSType.Xian_1980_3_Degree_GK_Zone_31;
                    break;
                case 32: obj = CSType.Xian_1980_3_Degree_GK_Zone_32;
                    break;
                case 33: obj = CSType.Xian_1980_3_Degree_GK_Zone_33;
                    break;
                case 34: obj = CSType.Xian_1980_3_Degree_GK_Zone_34;
                    break;
                case 35: obj = CSType.Xian_1980_3_Degree_GK_Zone_35;
                    break;
                case 36: obj = CSType.Xian_1980_3_Degree_GK_Zone_36;
                    break;
                case 37: obj = CSType.Xian_1980_3_Degree_GK_Zone_37;
                    break;
                case 38: obj = CSType.Xian_1980_3_Degree_GK_Zone_38;
                    break;
                case 39: obj = CSType.Xian_1980_3_Degree_GK_Zone_39;
                    break;
                case 40: obj = CSType.Xian_1980_3_Degree_GK_Zone_40;
                    break;
                case 41: obj = CSType.Xian_1980_3_Degree_GK_Zone_41;
                    break;
                case 42: obj = CSType.Xian_1980_3_Degree_GK_Zone_42;
                    break;
                case 43: obj = CSType.Xian_1980_3_Degree_GK_Zone_43;
                    break;
                case 44: obj = CSType.Xian_1980_3_Degree_GK_Zone_44;
                    break;
                case 45: obj = CSType.Xian_1980_3_Degree_GK_Zone_45;
                    break;
            }
            #endregion
            return obj;
        }


    }
}