using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data.OleDb;
using System.Data;
using Abp.Application.Services.Dto;
using Abp.Domain.Uow;

namespace InfoEarthFrame.Common
{
    #region 辅助类
    /// <summary>
    /// 颜色条带信息
    /// </summary>
    public class ColorRampInfo : EntityDto
    {
        /// <summary>
        /// 颜色个数
        /// </summary>
        public int Count
        {
            get
            {
                return _colorList.Count;
            }
        }

        private string _colorName = "";
        /// <summary>
        /// 颜色条带的名称
        /// </summary>
        public string ColorName
        {
            get
            {
                return _colorName;
            }
            set
            {
                _colorName = value;
            }
        }

        private List<Color> _colorList = new List<Color>();
        /// <summary>
        /// 颜色列表
        /// </summary>
        public List<Color> ColorList
        {
            get
            {
                return _colorList;
            }
            set
            {
                if (value != null)
                {
                    _colorList = value;
                }
            }
        }

        /// <summary>
        /// 通过索引序号获取颜色
        /// </summary>
        /// <param name="index">索引序号</param>
        /// <returns>颜色</returns>
        public Color this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    return _colorList[index];
                }
                else
                {
                    return Color.Empty;
                }
            }
        }
    }
    #endregion

    public static class ColorRampPlusStatic
    {
        private static ColorRampPlus _ColorRampPlus = null;
        public static ColorRampPlus ColorRampPlus
        {
            set
            {
                _ColorRampPlus = value;
            }
            get
            {
                if (_ColorRampPlus == null)
                {
                    _ColorRampPlus = new ColorRampPlus();
                }
                return _ColorRampPlus;
            }
        }
    }

    /// <summary>
    /// 随机和渐变颜色条带实用类
    /// </summary>
    public class ColorRampPlus
    {
        //颜色配置文件
        private readonly string _configPath = AppDomain.CurrentDomain.BaseDirectory + @"\Data\ColorBrewer.xls";

        //所有的渐变色颜色条带
        private List<ColorRampInfo> _linearColorRamps = new List<ColorRampInfo>();

        //所有随机色颜色条带
        private List<ColorRampInfo> _randomColorRamps = new List<ColorRampInfo>();

        /// <summary>
        /// 获取渐变色颜色条带列表
        /// </summary>
        /// <param name="colNum">每个颜色条带的颜色个数</param>
        /// <returns>渐变色颜色条带列表</returns>
        public List<ColorRampInfo> GetLinearColorRamps(int colNum)
        {
            List<ColorRampInfo> result = new List<ColorRampInfo>();
            for (int i = 0; i < _linearColorRamps.Count; i++)
            {
                if (_linearColorRamps[i].Count == colNum)
                {
                    result.Add(_linearColorRamps[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取随机色颜色条带列表
        /// </summary>
        /// <param name="colNum">每个颜色条带的颜色个数</param>
        /// <returns>随机色条带颜色列表</returns>
        public List<ColorRampInfo> GetRandomColorRamps(int colNum)
        {
            List<ColorRampInfo> result = new List<ColorRampInfo>();
            for (int i = 0; i < _randomColorRamps.Count; i++)
            {
                if (_randomColorRamps[i].Count == colNum)
                {
                    result.Add(_randomColorRamps[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 渐变颜色条带的最大颜色个数
        /// </summary>
        private int LinearMaxColorNum
        {
            get
            {
                int max = _linearColorRamps[0].Count;
                for (int i = 0; i < _linearColorRamps.Count; i++)
                {
                    if (_linearColorRamps[i].Count > max)
                    {
                        max = _linearColorRamps[i].Count;
                    }
                }
                return max;
            }
        }

        /// <summary>
        ///随机颜色条带的最大颜色个数
        /// </summary>
        private int RandomMaxColorNum
        {
            get
            {
                int max = _randomColorRamps[0].Count;
                for (int i = 0; i < _randomColorRamps.Count; i++)
                {
                    if (_randomColorRamps[i].Count > max)
                    {
                        max = _randomColorRamps[i].Count;
                    }
                }
                return max;
            }
        }

        /// <summary>
        /// 渐变颜色条带的最小颜色个数
        /// </summary>
        private int LinearMinColorNum
        {
            get
            {
                int min = _linearColorRamps[0].Count;
                for (int i = 0; i < _linearColorRamps.Count; i++)
                {
                    if (_linearColorRamps[i].Count < min)
                    {
                        min = _linearColorRamps[i].Count;
                    }
                }
                return min;
            }
        }

        /// <summary>
        ///随机颜色条带的最小颜色个数
        /// </summary>
        private int RandomMinColorNum
        {
            get
            {
                int min = _randomColorRamps[0].Count;
                for (int i = 0; i < _randomColorRamps.Count; i++)
                {
                    if (_randomColorRamps[i].Count < min)
                    {
                        min = _randomColorRamps[i].Count;
                    }
                }
                return min;
            }
        }

        /// <summary>
        /// 渐变颜色条带的最佳颜色个数
        /// </summary>
        public int LinearBestColorNum
        {
            get
            {
                int max = 0;
                int maxColorNum = 0;
                for (int i = LinearMinColorNum; i <= LinearMaxColorNum; i++)
                {
                    List<ColorRampInfo> rampList = this.GetLinearColorRamps(i);
                    int num = rampList.Count;
                    if (num >= max)
                    {
                        max = num;
                        maxColorNum = rampList[0].Count;
                    }
                }
                return maxColorNum;
            }
        }

        /// <summary>
        ///随机颜色条带的最佳颜色个数
        /// </summary>
        public int RandomBestColorNum
        {
            get
            {
                int max = 0;
                int maxColorNum = 0;
                for (int i = RandomMinColorNum; i <= RandomMaxColorNum; i++)
                {
                    List<ColorRampInfo> rampList = this.GetRandomColorRamps(i);
                    int num = rampList.Count;
                    if (num >= max)
                    {
                        max = num;
                        maxColorNum = rampList[0].Count;
                    }
                }
                return maxColorNum;
            }
        }

        /// <summary>
        /// 获取某个颜色的渐变颜色条带的最大颜色个数
        /// </summary>
        private int GetLinearMaxColorNum(string colorName)
        {
            int max = _linearColorRamps[0].Count;
            for (int i = 0; i < _linearColorRamps.Count; i++)
            {
                if (_linearColorRamps[i].ColorName == colorName && _linearColorRamps[i].Count > max)
                {
                    max = _linearColorRamps[i].Count;
                }
            }
            return max;
        }

        /// <summary>
        ///获取某个颜色的随机颜色条带的最大颜色个数
        /// </summary>
        private int GetRandomMaxColorNum(string colorName)
        {
            int max = _randomColorRamps[0].Count;
            for (int i = 0; i < _randomColorRamps.Count; i++)
            {
                if (_randomColorRamps[i].ColorName == colorName && _randomColorRamps[i].Count > max)
                {
                    max = _randomColorRamps[i].Count;
                }
            }
            return max;
        }

        /// <summary>
        /// 获取某个颜色的渐变颜色条带的最小颜色个数
        /// </summary>
        private int GetLinearMinColorNum(string colorName)
        {
            int min = _linearColorRamps[0].Count;
            for (int i = 0; i < _linearColorRamps.Count; i++)
            {
                if (_linearColorRamps[i].ColorName == colorName && _linearColorRamps[i].Count < min)
                {
                    min = _linearColorRamps[i].Count;
                }
            }
            return min;
        }

        /// <summary>
        ///获取某个颜色的随机颜色条带的最小颜色个数
        /// </summary>
        private int GetRandomMinColorNum(string colorName)
        {
            int min = _randomColorRamps[0].Count;
            for (int i = 0; i < _randomColorRamps.Count; i++)
            {
                if (_randomColorRamps[i].ColorName == colorName && _randomColorRamps[i].Count < min)
                {
                    min = _randomColorRamps[i].Count;
                }
            }
            return min;
        }

        /// <summary>
        /// 获取一个渐变颜色条带
        /// </summary>
        ///<param name="colorName">颜色条带名称</param>
        /// <param name="num">颜色个数,若num大于最大颜色个数，则返回最大颜色个数的条带</param>
        /// <returns>颜色条带</returns>
        public ColorRampInfo GetLinearColorRamp(string colorName, int num)
        {
            ColorRampInfo ramp = null;
            int colNum = num;
            int max = GetLinearMaxColorNum(colorName);
            int min = GetLinearMinColorNum(colorName);
            if (num > max)
            {
                colNum = max;
            }
            else if (num < min)
            {
                colNum = min;
            }
            for (int i = 0; i < _linearColorRamps.Count; i++)
            {
                if (_linearColorRamps[i].ColorName == colorName && _linearColorRamps[i].Count == colNum)
                {
                    ramp = _linearColorRamps[i];
                    break;
                }
            }
            return ramp;
        }

        /// <summary>
        /// 获取一个随机颜色条带
        /// </summary>
        ///<param name="colorName">颜色条带名称</param>
        /// <param name="num">颜色个数,若num大于最大颜色个数，则返回最大颜色个数的条带</param>
        /// <returns>颜色条带</returns>
        public ColorRampInfo GetRandomColorRamp(string colorName, int num)
        {
            ColorRampInfo ramp = null;
            int colNum = num;
            int max = GetRandomMaxColorNum(colorName);
            int min = GetRandomMinColorNum(colorName);
            if (num > max)
            {
                colNum = max;
            }
            else if (num < min)
            {
                colNum = min;
            }
            for (int i = 0; i < _randomColorRamps.Count; i++)
            {
                if (_randomColorRamps[i].ColorName == colorName && _randomColorRamps[i].Count == colNum)
                {
                    ramp = _randomColorRamps[i];
                    break;
                }
            }
            return ramp;
        }

        public ColorRampInfo GetRampInfo(string colorName, int num)
        {
            ColorRampInfo cri = new ColorRampInfo();
            if (_linearColorRamps.Exists(t => t.ColorName == colorName))
            {
                cri = GetLinearColorRamp(colorName, num);
            }
            else if (_randomColorRamps.Exists(t => t.ColorName == colorName))
            {
                cri = GetRandomColorRamp(colorName, num);
            }
            return cri;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ColorRampPlus()
        {
            try
            {
                string cmdStr = "select * from [ColorBrewer$]";
                string conStr = "provider=microsoft.jet.oledb.4.0;data source=" + _configPath + ";extended properties='Excel 8.0;HDR=yes;IMEX=1'";

                OleDbConnection con = new OleDbConnection(conStr);
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmdStr, con);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                con.Close();

                DataTable table = dataSet.Tables[0];

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string colorName = table.Rows[i]["ColorName"].ToString();
                    string numOfColor = table.Rows[i]["NumOfColors"].ToString();
                    string type = table.Rows[i]["Type"].ToString();

                    if (string.IsNullOrEmpty(colorName) == false && string.IsNullOrEmpty(numOfColor) == false && string.IsNullOrEmpty(type) == false)
                    {
                        int num = int.Parse(numOfColor);
                        if (num > 0)
                        {
                            ColorRampInfo info = new ColorRampInfo();
                            info.ColorName = colorName;
                            while (num > 0)
                            {
                                string R = table.Rows[i]["R"].ToString();
                                string G = table.Rows[i]["G"].ToString();
                                string B = table.Rows[i]["B"].ToString();
                                if (string.IsNullOrEmpty(R) || string.IsNullOrEmpty(G) || string.IsNullOrEmpty(B))
                                {
                                    break;
                                }
                                int r = int.Parse(R);
                                int g = int.Parse(G);
                                int b = int.Parse(B);
                                info.ColorList.Add(Color.FromArgb(r, g, b));
                                num--;
                                i++;
                            }
                            if (type.ToString() == "seq" || type.ToString() == "div")
                            {
                                _linearColorRamps.Add(info);
                            }
                            else if (type.ToString() == "qual")
                            {
                                _randomColorRamps.Add(info);
                            }
                            i--;
                        }
                    }
                }
                table.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
