using System;
using ProjNet.Converters.WellKnownText;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace InfoEarthFrame.Common
{
    public enum CSType
    {
        GCS_Xian_1980,
        Xian_1980_GK_Zone_13,
        Xian_1980_GK_Zone_14,
        Xian_1980_GK_Zone_15,
        Xian_1980_GK_Zone_16,
        Xian_1980_GK_Zone_17,
        Xian_1980_GK_Zone_18,
        Xian_1980_GK_Zone_19,
        Xian_1980_GK_Zone_20,
        Xian_1980_GK_Zone_21,
        Xian_1980_GK_Zone_22,
        Xian_1980_GK_Zone_23,

        Xian_1980_3_Degree_GK_Zone_25,
        Xian_1980_3_Degree_GK_Zone_26,
        Xian_1980_3_Degree_GK_Zone_27,
        Xian_1980_3_Degree_GK_Zone_28,
        Xian_1980_3_Degree_GK_Zone_29,
        Xian_1980_3_Degree_GK_Zone_30,
        Xian_1980_3_Degree_GK_Zone_31,
        Xian_1980_3_Degree_GK_Zone_32,
        Xian_1980_3_Degree_GK_Zone_33,
        Xian_1980_3_Degree_GK_Zone_34,
        Xian_1980_3_Degree_GK_Zone_35,
        Xian_1980_3_Degree_GK_Zone_36,
        Xian_1980_3_Degree_GK_Zone_37,
        Xian_1980_3_Degree_GK_Zone_38,
        Xian_1980_3_Degree_GK_Zone_39,
        Xian_1980_3_Degree_GK_Zone_40,
        Xian_1980_3_Degree_GK_Zone_41,
        Xian_1980_3_Degree_GK_Zone_42,
        Xian_1980_3_Degree_GK_Zone_43,
        Xian_1980_3_Degree_GK_Zone_44,
        Xian_1980_3_Degree_GK_Zone_45,

        GCS_Beijing_1954,
        Beijing_1954_GK_Zone_13,
        Beijing_1954_GK_Zone_14,
        Beijing_1954_GK_Zone_15,
        Beijing_1954_GK_Zone_16,
        Beijing_1954_GK_Zone_17,
        Beijing_1954_GK_Zone_18,
        Beijing_1954_GK_Zone_19,
        Beijing_1954_GK_Zone_20,
        Beijing_1954_GK_Zone_21,
        Beijing_1954_GK_Zone_22,
        Beijing_1954_GK_Zone_23,

        Beijing_1954_3_Degree_GK_Zone_25,
        Beijing_1954_3_Degree_GK_Zone_26,
        Beijing_1954_3_Degree_GK_Zone_27,
        Beijing_1954_3_Degree_GK_Zone_28,
        Beijing_1954_3_Degree_GK_Zone_29,
        Beijing_1954_3_Degree_GK_Zone_30,
        Beijing_1954_3_Degree_GK_Zone_31,
        Beijing_1954_3_Degree_GK_Zone_32,
        Beijing_1954_3_Degree_GK_Zone_33,
        Beijing_1954_3_Degree_GK_Zone_34,
        Beijing_1954_3_Degree_GK_Zone_35,
        Beijing_1954_3_Degree_GK_Zone_36,
        Beijing_1954_3_Degree_GK_Zone_37,
        Beijing_1954_3_Degree_GK_Zone_38,
        Beijing_1954_3_Degree_GK_Zone_39,
        Beijing_1954_3_Degree_GK_Zone_40,
        Beijing_1954_3_Degree_GK_Zone_41,
        Beijing_1954_3_Degree_GK_Zone_42,
        Beijing_1954_3_Degree_GK_Zone_43,
        Beijing_1954_3_Degree_GK_Zone_44,
        Beijing_1954_3_Degree_GK_Zone_45,
    }


    public class CSWKTUtility
    {
        public static double[] TransForm(double x, double y, CSType fromCSType, CSType toCSType)
        {
            ICoordinateSystem fromCS = GetCoordinateSystem(fromCSType);
            ICoordinateSystem toCS = GetCoordinateSystem(toCSType);
            var point = new double[2] {y, x};
            var result = new double[2];
            if (y.ToString().Length < 3)
            {
                return null;
            }
            if (fromCS != null && toCS != null)
            {
                PtsToPts(fromCS, toCS, point, out result);
            }
            else
            {
                return null;
            }
            return result;
        }

        private static ICoordinateSystem GetCoordinateSystem(CSType csType)
        {
            ICoordinateSystem cs = null;
            switch (csType)
            {
                case CSType.GCS_Xian_1980:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.GCS_Xian_1980) as IGeographicCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_25:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_25) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_26:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_26) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_27:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_27) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_28:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_28) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_29:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_29) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_30:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_30) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_31:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_31) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_32:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_32) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_33:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_33) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_34:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_34) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_35:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_35) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_36:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_36) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_37:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_37) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_38:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_38) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_39:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_39) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_40:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_40) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_41:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_41) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_42:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_42) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_43:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_43) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_44:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_44) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_3_Degree_GK_Zone_45:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_3_Degree_GK_Zone_45) as
                        IProjectedCoordinateSystem;
                    break;


                case CSType.Xian_1980_GK_Zone_13:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_13) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_14:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_14) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_15:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_15) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_16:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_16) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_17:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_17) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_18:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_18) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_19:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_19) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_20:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_20) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_21:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_21) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_22:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_22) as IProjectedCoordinateSystem;
                    break;
                case CSType.Xian_1980_GK_Zone_23:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Xian_1980_GK_Zone_23) as IProjectedCoordinateSystem;
                    break;


                case CSType.GCS_Beijing_1954:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.GCS_Beijing_1954) as IGeographicCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_25:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_25) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_26:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_26) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_27:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_27) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_28:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_28) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_29:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_29) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_30:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_30) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_31:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_31) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_32:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_32) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_33:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_33) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_34:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_34) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_35:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_35) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_36:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_36) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_37:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_37) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_38:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_38) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_39:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_39) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_40:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_40) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_41:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_41) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_42:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_42) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_43:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_43) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_44:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_44) as
                        IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_3_Degree_GK_Zone_45:
                    cs =
                        CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_3_Degree_GK_Zone_45) as
                        IProjectedCoordinateSystem;
                    break;

                case CSType.Beijing_1954_GK_Zone_13:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_13) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_14:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_14) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_15:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_15) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_16:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_16) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_17:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_17) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_18:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_18) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_19:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_19) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_20:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_20) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_21:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_21) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_22:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_22) as IProjectedCoordinateSystem;
                    break;
                case CSType.Beijing_1954_GK_Zone_23:
                    cs = CoordinateSystemWktReader.Parse(CSWKT.Beijing_1954_GK_Zone_23) as IProjectedCoordinateSystem;
                    break;
            }
            return cs;
        }

        public static string DegreeToDMS(double degree)
        {
            double deg = Math.Floor(degree);

            double min = Math.Floor((degree - deg)*60);

            double sec = (degree - deg)*3600 - min*60;


            string d = deg.ToString();
            string m = min.ToString("00");
            string s = sec.ToString("#00.00");

            return d + m + s;
        }

        public static double DMSToDegree(string dms)
        {
            string[] str = dms.Split('.');

            //bool right = false;
            double d = 0;
            double m = 0;
            double s = 0;

            if (str.Length >= 1)
            {
                if ((str[0].Length%2) == 1)
                {
                    if (dms.Length < 3)
                    {
                        return -1;
                    }
                    else
                    {
                        if (!double.TryParse(dms.Substring(0, 3), out d))
                        {
                            return -1;
                        }

                        if (dms.Length == 5)
                        {
                            if (!double.TryParse(dms.Substring(3, 2), out m))
                            {
                                return -1;
                            }
                        }

                        if (dms.Length > 5)
                        {
                            if (!double.TryParse(dms.Substring(3, 2), out m))
                            {
                                return -1;
                            }

                            if (!double.TryParse(dms.Substring(5), out s))
                            {
                                return -1;
                            }
                        }
                    }
                }
                else if ((str[0].Length%2) == 0)
                {
                    if (dms.Length < 2)
                    {
                        return -1;
                    }
                    else
                    {
                        if (!double.TryParse(dms.Substring(0, 2), out d))
                        {
                            return -1;
                        }

                        if (dms.Length == 4)
                        {
                            if (!double.TryParse(dms.Substring(2, 2), out m))
                            {
                                return -1;
                            }
                        }

                        if (dms.Length > 4)
                        {
                            if (!double.TryParse(dms.Substring(2, 2), out m))
                            {
                                return -1;
                            }
                            if (!double.TryParse(dms.Substring(4), out s))
                            {
                                return -1;
                            }
                        }
                    }
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }

            if (m > 60 && s > 60)
            {
                return -1;
            }
            return d + m/60 + s/3600;
        }

        public static double GetValue(string value)
        {
            double result;
            bool b = double.TryParse(value, out result);
            if (!b)
            {
                result = -1;
            }
            return result;
        }

        private static void PtsToPts(ICoordinateSystem fromCS, ICoordinateSystem toCS, double[] point,
                                     out double[] result)
        {
            try
            {
                var ctfac = new CoordinateTransformationFactory();
                ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(fromCS, toCS);
                result = trans.MathTransform.Transform(point);
            }
            catch (Exception ex)
            {
                result = null;
            }
        }

        /// <summary>
        /// 判断即将入ZHDA01A表（现今变形迹象表）中的数据是否合理
        /// </summary>
        /// <param name="dgv">datagridview控件名称</param>
        /// <param name="index">主键index</param>
        /// <returns>bool值</returns>        
        //public static bool IsValidDatagridView(DataGridView dgv, int index)
        //{
        //    bool result = false;
        //    try
        //    {
        //        for (int i = 0; i < dgv.Rows.Count - 1; i++)
        //        {
        //            if (dgv[index, i].Value.ToString().Trim() == "")
        //            {
        //                return false;
        //            }
        //            //for (int j = i+1; j < dgv.Rows.Count - 1; j++)
        //            //{
        //            //    if (dgv[index,i].Value.ToString()==dgv[index,j].Value.ToString())
        //            //    {
        //            //        return false;
        //            //    }
        //            //}
        //        }
        //        result = true;

        //    }
        //    catch (System.Exception e)
        //    {
        //        result = false;
        //    }
        //    return result;
        //}
    }

    internal class CSWKT
    {
        private static string _GCS_Beijing_1954 =
            "GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";

        private static string _Beijing_1954_GK_Zone_13 =
            "PROJCS[\"Beijing_1954_GK_Zone_13\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",13500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",75],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_14 =
            "PROJCS[\"Beijing_1954_GK_Zone_14\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",14500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",81],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_15 =
            "PROJCS[\"Beijing_1954_GK_Zone_15\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",15500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",87],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_16 =
            "PROJCS[\"Beijing_1954_GK_Zone_16\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",16500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",93],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_17 =
            "PROJCS[\"Beijing_1954_GK_Zone_17\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",17500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",99],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_18 =
            "PROJCS[\"Beijing_1954_GK_Zone_18\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",18500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",105],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_19 =
            "PROJCS[\"Beijing_1954_GK_Zone_19\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",19500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",111],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_20 =
            "PROJCS[\"Beijing_1954_GK_Zone_20\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",20500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",117],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_21 =
            "PROJCS[\"Beijing_1954_GK_Zone_21\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",21500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",123],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_22 =
            "PROJCS[\"Beijing_1954_GK_Zone_22\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",22500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",129],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_GK_Zone_23 =
            "PROJCS[\"Beijing_1954_GK_Zone_23\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245,298.3]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",23500000],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",135],PARAMETER[\"Scale_Factor\",1],PARAMETER[\"Latitude_Of_Origin\",0],UNIT[\"Meter\",1]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_25 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_25\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",25500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",75.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_26 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_26\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",26500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",78.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_27 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_27\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",27500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",81.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_28 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_28\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",28500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",84.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_29 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_29\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",29500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",87.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_30 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_30\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",30500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",90.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_31 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_31\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",31500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",93.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_32 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_32\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",32500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",96.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_33 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_33\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",33500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",99.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_34 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_34\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",34500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",102.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_35 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_35\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",35500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",105.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_36 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_36\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",36500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",108.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_37 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_37\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",37500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",111.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_38 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_38\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",38500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",114.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_39 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_39\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",39500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",117.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_40 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_40\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",40500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",120.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_41 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_41\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",41500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",123.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_42 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_42\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",42500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",126.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_43 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_43\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",43500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",129.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_44 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_44\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",44500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",132.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Beijing_1954_3_Degree_GK_Zone_45 =
            "PROJCS[\"Beijing_1954_3_Degree_GK_Zone_45\",GEOGCS[\"GCS_Beijing_1954\",DATUM[\"D_Beijing_1954\",SPHEROID[\"Krasovsky_1940\",6378245.0,298.3]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",45500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",135.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";


        private static string _GCS_Xian_1980 =
            "GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";

        private static string _Xian_1980_GK_Zone_13 =
            "PROJCS[\"Xian_1980_GK_Zone_13\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",13500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",75.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_14 =
            "PROJCS[\"Xian_1980_GK_Zone_14\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",14500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",81.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_15 =
            "PROJCS[\"Xian_1980_GK_Zone_15\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",15500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",87.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_16 =
            "PROJCS[\"Xian_1980_GK_Zone_16\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",16500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",93.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_17 =
            "PROJCS[\"Xian_1980_GK_Zone_17\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",17500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",99.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_18 =
            "PROJCS[\"Xian_1980_GK_Zone_18\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",18500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",105.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_19 =
            "PROJCS[\"Xian_1980_GK_Zone_19\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",19500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",111.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_20 =
            "PROJCS[\"Xian_1980_GK_Zone_20\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",20500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",117.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_21 =
            "PROJCS[\"Xian_1980_GK_Zone_21\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",21500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",123.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_22 =
            "PROJCS[\"Xian_1980_GK_Zone_22\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",22500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",129.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_GK_Zone_23 =
            "PROJCS[\"Xian_1980_GK_Zone_23\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",23500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",135.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_25 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_25\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",25500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",75.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_26 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_26\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",26500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",78.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_27 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_27\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",27500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",81.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_28 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_28\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",28500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",84.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_29 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_29\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",29500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",87.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_30 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_30\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",30500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",90.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_31 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_31\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",31500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",93.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_32 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_32\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",32500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",96.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_33 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_33\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",33500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",99.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_34 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_34\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",34500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",102.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_35 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_35\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",35500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",105.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_36 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_36\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",36500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",108.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_37 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_37\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",37500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",111.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_38 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_38\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",38500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",114.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_39 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_39\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",39500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",117.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_40 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_40\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",40500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",120.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_41 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_41\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",41500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",123.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_42 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_42\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",42500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",126.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_43 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_43\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",43500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",129.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_44 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_44\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",44500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",132.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        private static string _Xian_1980_3_Degree_GK_Zone_45 =
            "PROJCS[\"Xian_1980_3_Degree_GK_Zone_45\",GEOGCS[\"GCS_Xian_1980\",DATUM[\"D_Xian_1980\",SPHEROID[\"Xian_1980\",6378140.0,298.257]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",45500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",135.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";

        public static string GCS_Beijing_1954
        {
            get { return _GCS_Beijing_1954; }
        }

        public static string Beijing_1954_GK_Zone_13
        {
            get { return _Beijing_1954_GK_Zone_13; }
        }

        public static string Beijing_1954_GK_Zone_14
        {
            get { return _Beijing_1954_GK_Zone_14; }
        }

        public static string Beijing_1954_GK_Zone_15
        {
            get { return _Beijing_1954_GK_Zone_15; }
        }

        public static string Beijing_1954_GK_Zone_16
        {
            get { return _Beijing_1954_GK_Zone_16; }
        }

        public static string Beijing_1954_GK_Zone_17
        {
            get { return _Beijing_1954_GK_Zone_17; }
        }

        public static string Beijing_1954_GK_Zone_18
        {
            get { return _Beijing_1954_GK_Zone_18; }
        }

        public static string Beijing_1954_GK_Zone_19
        {
            get { return _Beijing_1954_GK_Zone_19; }
        }

        public static string Beijing_1954_GK_Zone_20
        {
            get { return _Beijing_1954_GK_Zone_20; }
        }

        public static string Beijing_1954_GK_Zone_21
        {
            get { return _Beijing_1954_GK_Zone_21; }
        }

        public static string Beijing_1954_GK_Zone_22
        {
            get { return _Beijing_1954_GK_Zone_22; }
        }

        public static string Beijing_1954_GK_Zone_23
        {
            get { return _Beijing_1954_GK_Zone_23; }
        }


        public static string Beijing_1954_3_Degree_GK_Zone_25
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_25; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_26
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_26; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_27
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_27; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_28
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_28; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_29
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_29; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_30
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_30; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_31
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_31; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_32
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_32; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_33
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_33; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_34
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_34; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_35
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_35; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_36
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_36; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_37
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_37; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_38
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_38; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_39
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_39; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_40
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_40; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_41
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_41; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_42
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_42; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_43
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_43; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_44
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_44; }
        }

        public static string Beijing_1954_3_Degree_GK_Zone_45
        {
            get { return _Beijing_1954_3_Degree_GK_Zone_45; }
        }


        public static string GCS_Xian_1980
        {
            get { return _GCS_Xian_1980; }
        }


        public static string Xian_1980_GK_Zone_13
        {
            get { return _Xian_1980_GK_Zone_13; }
        }

        public static string Xian_1980_GK_Zone_14
        {
            get { return _Xian_1980_GK_Zone_14; }
        }

        public static string Xian_1980_GK_Zone_15
        {
            get { return _Xian_1980_GK_Zone_15; }
        }

        public static string Xian_1980_GK_Zone_16
        {
            get { return _Xian_1980_GK_Zone_16; }
        }

        public static string Xian_1980_GK_Zone_17
        {
            get { return _Xian_1980_GK_Zone_17; }
        }

        public static string Xian_1980_GK_Zone_18
        {
            get { return _Xian_1980_GK_Zone_18; }
        }

        public static string Xian_1980_GK_Zone_19
        {
            get { return _Xian_1980_GK_Zone_19; }
        }

        public static string Xian_1980_GK_Zone_20
        {
            get { return _Xian_1980_GK_Zone_20; }
        }

        public static string Xian_1980_GK_Zone_21
        {
            get { return _Xian_1980_GK_Zone_21; }
        }

        public static string Xian_1980_GK_Zone_22
        {
            get { return _Xian_1980_GK_Zone_22; }
        }

        public static string Xian_1980_GK_Zone_23
        {
            get { return _Xian_1980_GK_Zone_23; }
        }


        public static string Xian_1980_3_Degree_GK_Zone_25
        {
            get { return _Xian_1980_3_Degree_GK_Zone_25; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_26
        {
            get { return _Xian_1980_3_Degree_GK_Zone_26; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_27
        {
            get { return _Xian_1980_3_Degree_GK_Zone_27; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_28
        {
            get { return _Xian_1980_3_Degree_GK_Zone_28; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_29
        {
            get { return _Xian_1980_3_Degree_GK_Zone_29; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_30
        {
            get { return _Xian_1980_3_Degree_GK_Zone_30; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_31
        {
            get { return _Xian_1980_3_Degree_GK_Zone_31; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_32
        {
            get { return _Xian_1980_3_Degree_GK_Zone_32; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_33
        {
            get { return _Xian_1980_3_Degree_GK_Zone_33; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_34
        {
            get { return _Xian_1980_3_Degree_GK_Zone_34; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_35
        {
            get { return _Xian_1980_3_Degree_GK_Zone_35; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_36
        {
            get { return _Xian_1980_3_Degree_GK_Zone_36; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_37
        {
            get { return _Xian_1980_3_Degree_GK_Zone_37; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_38
        {
            get { return _Xian_1980_3_Degree_GK_Zone_38; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_39
        {
            get { return _Xian_1980_3_Degree_GK_Zone_39; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_40
        {
            get { return _Xian_1980_3_Degree_GK_Zone_40; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_41
        {
            get { return _Xian_1980_3_Degree_GK_Zone_41; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_42
        {
            get { return _Xian_1980_3_Degree_GK_Zone_42; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_43
        {
            get { return _Xian_1980_3_Degree_GK_Zone_43; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_44
        {
            get { return _Xian_1980_3_Degree_GK_Zone_44; }
        }

        public static string Xian_1980_3_Degree_GK_Zone_45
        {
            get { return _Xian_1980_3_Degree_GK_Zone_45; }
        }
    }
}