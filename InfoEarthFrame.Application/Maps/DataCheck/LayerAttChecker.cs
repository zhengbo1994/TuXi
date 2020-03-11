using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace iTelluro.GeologicMap.TopologyCheck
{
    public static class LayerAttChecker
    {
        public static List<string> CheckLayer(LayerArgs lyrArgs, DicInfoReader dicInfo)
        {
            List<string> rlts = new List<string>();
            TxLayerInfo txLyrInfo = dicInfo.TxLyrList.First(t => t.DataLayer == lyrArgs.DataLayer); //t.TxName == lyrArgs.TxName && t.TjName == lyrArgs.TjName && t.LayerName == lyrArgs.LayerName && t.DataLayer == lyrArgs.DataLayer);
            //List<LayerAttInfo> attInfoList = dicInfo.LyrAttList.Where(t => t.TxName == txLyrInfo.TxName).Where(t => t.TjName == txLyrInfo.TjName).Where(t => t.LayerName == txLyrInfo.LayerName).ToList();
            List<LayerAttInfo> attInfoList = dicInfo.LyrAttList.Where(t => t.DataLayer == txLyrInfo.DataLayer).ToList();
            DataTable tbl = ShpUtility.GetRecords(lyrArgs.LayerPath, null);

            // 检查列名是否有特殊字符
            for(int i=0; i<tbl.Columns.Count; i++)
            {
                if (tbl.Columns[i].ColumnName.IndexOf('?') != -1)
                {
                    string info = string.Format("数据检查：列名长度不符合ArcGIS规范：{0}", tbl.Columns[i].ColumnName);
                    rlts.Add(info);
                }
            }

            //检查字段及名称是否齐全
            List<string> logs = CheckFiledNums(tbl, attInfoList);
            if (logs != null && logs.Count > 0)
            {
                rlts.AddRange(logs);
            }
            //检查字段值是否溢出（超过限定长度或不在给定阈值范围）
            logs = CheckFiledValueRange(tbl, attInfoList);
            if (logs != null && logs.Count > 0)
            {
                rlts.AddRange(logs);
            }
            //检查字段字典项
            for (int i = 0; i < attInfoList.Count; i++)
            {
                LayerAttInfo lyrAtt = attInfoList[i];
                logs = CheckDicAtt(tbl, lyrAtt, dicInfo);
                if (logs != null && logs.Count > 0)
                {
                    rlts.AddRange(logs);
                }
            }
            return rlts;
        }

        //检查字段及名称是否齐全
        private static List<string> CheckFiledNums(DataTable tbl, List<LayerAttInfo> attInfoList)
        {
            List<string> rlts = new List<string>();

            List<string> cols = new List<string>();
            for (int i = 0; i < tbl.Columns.Count; i++)
            {
                cols.Add(tbl.Columns[i].ColumnName);
            }
            List<string> needColsCN = attInfoList.Select(t => t.DataCode).ToList(); // 原来的 List<string> needColsCN = attInfoList.Select(t => t.DataName).ToList();
            List<string> needColsName = attInfoList.Select(t => t.DataName).ToList();
            for (int i = 0; i < needColsCN.Count; i++)
            {
                if (cols.Contains(needColsCN[i]) == false && attInfoList[i].InputControl.ToUpper()== "M")
                {
                    string info = string.Format("数据检查：缺少字段：{0}({1})", needColsCN[i], needColsName[i]);
                    rlts.Add(info);
                }
            }
            return rlts;
        }

        //检查字段值是否溢出（超过限定长度或不在给定阈值范围）
        private static List<string> CheckFiledValueRange(DataTable tbl, List<LayerAttInfo> attInfoList)
        {
            List<string> rlts = new List<string>();
            List<string> cols = new List<string>();
            for (int i = 0; i < tbl.Columns.Count; i++)
            {
                cols.Add(tbl.Columns[i].ColumnName);
            }
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                for (int j = 0; j < attInfoList.Count; j++)
                {
                    LayerAttInfo attInfo = attInfoList[j];
                    object obj = null;
                    if (cols.Contains(attInfo.DataCode)) // 原来if (cols.Contains(attInfo.DataName))
                    {
                        obj = tbl.Rows[i][attInfo.DataCode]; // obj = tbl.Rows[i][attInfo.DataName];
                    }
                    if (obj == null)
                    {
                        continue;
                    }
                    bool overflow = IsFileValueOverflow(obj, attInfo.DataType);
                    if (overflow)
                    {
                        string info = string.Format("数据检查：第{0}行“{1}”字段值超过“{2}”长度限制", i, attInfo.DataName, attInfo.DataType);
                        rlts.Add(info);
                    }
                    overflow = IsFileValueOverRange(obj, attInfo.ValueType);
                    if (overflow)
                    {
                        string info = string.Format("数据检查：第{0}行“{1}”字段值不在“{2}”值域范围内", i, attInfo.DataName, attInfo.ValueType);
                        rlts.Add(info);
                    }
                }
            }
            return rlts;
        }

        //检查字段字典项
        private static List<string> CheckDicAtt(DataTable tbl, LayerAttInfo attInfo, DicInfoReader dicInfo)
        {
            List<string> rlts = new List<string>();
            List<AttValueInfo> attValList = dicInfo.AttValueList.Where(t => t.AttId == attInfo.Guid).ToList();
            if (attValList == null || attValList.Count < 1)
            {
                return rlts;
            }
            List<string> cols = new List<string>();
            for (int i = 0; i < tbl.Columns.Count; i++)
            {
                cols.Add(tbl.Columns[i].ColumnName);
            }
            if (attValList.Count == 1 && string.IsNullOrEmpty(attValList[0].ParentId) == false)
            {
                AttValueInfo attVal = attValList[0];
                //检查大类是符合字典项
                AttValueInfo pAttVal = dicInfo.AttValueList.First(t => t.Guid == attVal.ParentId);
                LayerAttInfo pAttInfo = dicInfo.LyrAttList.First(t => t.Guid == pAttVal.AttId);
                List<string> infoList = CheckDicAtt(tbl, pAttInfo, dicInfo);
                if (infoList != null && infoList.Count > 0)
                {
                    rlts.AddRange(infoList.ToArray());
                }
            }
            //检查是否符合字典项
            List<string> valueList = GetDicValues(attValList);
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                object obj = null;
                if (cols.Contains(attInfo.DataName))
                {
                    obj = tbl.Rows[i][attInfo.DataName];
                }
                if (obj == null)
                {
                    continue;
                }
                string valStr = obj.ToString();
                if (attInfo.Constraint.Equals("M", StringComparison.CurrentCultureIgnoreCase) && valueList.Contains(valStr) == false)
                {
                    string info = string.Format("数据检查：第{0}行“{1}”字段值不在字典项({2})里", i, attInfo.DataName, JoinStr(valueList));
                    rlts.Add(info);
                }
                else if (attInfo.Constraint.Equals("O", StringComparison.CurrentCultureIgnoreCase))
                {
                    //有待完善
                    bool ok = false;
                    for (int j = 0; j < valueList.Count; j++)
                    {
                        if (valStr.Contains(valueList[j]))
                        {
                            ok = true;
                        }
                    }
                    if (ok == false)
                    {
                        string info = string.Format("数据检查：第{0}行{1}字段值不在字典项({2})里", i, attInfo.DataName, JoinStr(valueList));
                        rlts.Add(info);
                    }
                }
            }
            return rlts;
        }

        //判断字段值是否溢出（超过限定长度）
        private static bool IsFileValueOverflow(object obj, string dataType)
        {
            try
            {
                if (obj.GetType() != typeof(string) && obj.GetType() != typeof(char)) // 原来 if (obj.GetType() != typeof(string) || obj.GetType() != typeof(char))
                {
                    return false;
                }
                if(dataType.Trim() == "")
                {
                    return false;
                }
                string length = dataType.Substring(1);

                int num = (int)double.Parse(length);
                if (obj.ToString().Length > num)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        //判断字段值是否溢出（不在给定阈值范围）
        private static bool IsFileValueOverRange(object obj, string valueType)
        {
            try
            {
                if (valueType.IndexOf('～') < 1)
                {
                    return false;
                }
                string[] range = valueType.Split('～');
                double min = double.Parse(range[0]);
                double max = double.Parse(range[1]);
                double val = double.Parse(obj.ToString());
                if (val < min || val > max)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private static List<string> GetDicValues(List<AttValueInfo> attValList)
        {
            List<string> valueList = new List<string>();
            try
            {
                for (int i = 0; i < attValList.Count; i++)
                {
                    AttValueInfo attVal = attValList[i];
                    if (string.IsNullOrEmpty(attVal.Memo))
                    {
                        string[] temp = attVal.Value.Split('、');
                        valueList.AddRange(temp);
                    }
                    else if (attVal.Memo == "$")
                    {
                        string[] temp = attVal.Value.Split('$');
                        valueList.AddRange(temp);
                    }
                }
                return valueList;
            }
            catch (Exception ex)
            {
                return valueList;
            }
        }

        private static string JoinStr(List<string> items)
        {
            string rlt = string.Empty;
            for (int i = 0; i < items.Count; i++)
            {
                rlt += items[i];
                if (i != items.Count - 1)
                {
                    rlt += ",";
                }
            }
            return rlt;
        }
    }
}
