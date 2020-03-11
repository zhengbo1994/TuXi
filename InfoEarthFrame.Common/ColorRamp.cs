using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;


namespace InfoEarthFrame.Common
{
    #region 辅助类

    /// <summary>
    /// 渐变色信息对象
    /// </summary>
    public class LinearColorRampInfo
    {
        private Color start;
        /// <summary>
        /// 渐变色起始颜色
        /// </summary>
        public Color Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
            }
        }

        private Color end;
        /// <summary>
        /// 渐变色终止颜色
        /// </summary>
        public Color End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
            }
        }

        private List<Color> colorList = new List<Color>();
        /// <summary>
        /// 分一定间隔取出来的颜色列表
        /// </summary>
        public List<Color> ColorList
        {
            get
            {
                return colorList;
            }
            set
            {
                if (value != null)
                {
                    colorList = value;
                }
            }
        }

        /// <summary>
        /// 颜色个数，只读
        /// </summary>
        public int Count
        {
            get
            {
                return colorList.Count;
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
                    return colorList[index];
                }
                else
                {
                    throw new Exception("index超出范围");
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="start">渐变色起始颜色</param>
        /// <param name="end">渐变色终止颜色</param>
        public LinearColorRampInfo(Color start, Color end)
        {
            this.start = start;
            this.end = end;
        }
    }

    /// <summary>
    /// 随机色信息对象
    /// </summary>
    public class RandomColorRampInfo
    {
        private List<Color> colorList = new List<Color>();
        /// <summary>
        /// 随即颜色列表
        /// </summary>
        public List<Color> ColorList
        {
            get
            {
                return colorList;
            }
            set
            {
                if (value != null)
                {
                    colorList = value;
                }
            }
        }

        /// <summary>
        /// 随即颜色个数，只读
        /// </summary>
        public int Count
        {
            get
            {
                return colorList.Count;
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
                    return colorList[index];
                }
                else
                {
                    throw new Exception("index超出范围");
                }
            }
        }
    }
    #endregion

    public static class ColorRamp
    {
        /// <summary>
        /// 获取Color下所有已定义141种的颜色
        /// </summary>
        /// <returns> Color数组</returns>
        public static List<Color> GetAllDefColors()
        {
            List<Color> colorList = new List<Color>();
            Type type = Color.Aqua.GetType();
            PropertyInfo[] infos = type.GetProperties();
            if (infos != null)
            {
                foreach (PropertyInfo info in infos)
                {
                    Color c = Color.FromName(info.Name);
                    if (c.IsKnownColor)
                    {
                        colorList.Add(c);
                    }
                }
            }
            return colorList;
        }

        /// <summary>
        /// 获取预设的渐变颜色条带列表
        /// </summary>
        /// <param name="colNum">每一行的颜色个数</param>
        /// <param name="alpha">透明度</param>
        /// <returns></returns>
        public static List<LinearColorRampInfo> GetLinearColorRamps1(int colNum, int alpha)
        {
            List<LinearColorRampInfo> colorRampList = new List<LinearColorRampInfo>();
            Color[] starts = {Color.Green,Color.Black,Color.White,Color.Red,Color.Red, Color.LightBlue, Color.LightCoral, Color.LightCyan, Color.LightGray, Color.LightGreen, Color.LightPink, Color.LightSalmon, Color.LightSeaGreen, Color.LightSkyBlue, Color.LightSlateGray, Color.LightSteelBlue, Color.LightYellow };
            Color[] ends = {Color.Blue,Color.White,Color.Black,Color.Green,Color.Blue, Color.Blue, Color.Coral, Color.Cyan, Color.Gray, Color.Green, Color.Pink, Color.Salmon, Color.SeaGreen, Color.SkyBlue, Color.SlateGray, Color.SteelBlue, Color.Yellow };
            for (int i = 0; i < starts.Length; i++)
            {
                LinearColorRampInfo colorRamp = new LinearColorRampInfo(starts[i], ends[i]);
                for (int j = 0; j < colNum; j++)
                {
                    int r = starts[i].R + (ends[i].R - starts[i].R) * j / colNum;
                    int g = starts[i].G + (ends[i].G - starts[i].G) * j / colNum;
                    int b = starts[i].B + (ends[i].B - starts[i].B) * j / colNum;
                    Color c = Color.FromArgb(alpha, r, g, b);
                    colorRamp.ColorList.Add(c);
                }
                colorRampList.Add(colorRamp);
            }
            return colorRampList;
        }

        /// <summary>
        /// 获取预设的渐变颜色条带列表
        /// </summary>
        /// <param name="colNum">每一行的颜色个数</param>
        /// <param name="alpha">透明度</param>
        /// <returns></returns>
        public static List<LinearColorRampInfo> GetLinearColorRamps(int colNum, int alpha)
        {
            List<LinearColorRampInfo> colorRampList = new List<LinearColorRampInfo>();
            
            List<Color> starts = new List<Color>();
            List<Color> ends = new List<Color>();
            
            starts.AddRange(new Color[] { Color.Green, Color.White, Color.Red, Color.Red, Color.LightBlue, Color.LightCyan });
            ends.AddRange(new Color[] { Color.Blue, Color.Black, Color.Green, Color.Blue, Color.Blue, Color.Cyan });
            
            List<Color> defColor = GetAllDefColors();
            if (defColor.Count > 20)
            {
                int count = 0;
                int index = 0;
                int i = 0;
                while (count < 22)
                {
                    i++;
                    if (i > 1000)
                    {
                        break;
                    }
                    if (index > defColor.Count - 2)
                    {
                        index = 0;
                    }
                    Color start = defColor[index];
                    Color end = defColor[++index];
                    if (Math.Abs(start.R - end.R) < 50 && Math.Abs(start.B - end.B)<50 && Math.Abs(start.G - end.G) < 50)
                    {
                        continue;
                    }
                    starts.Add(start);
                    ends.Add(end);
                    count++;
                }
            }
            for (int i = 0; i < starts.Count; i++)
            {
                LinearColorRampInfo colorRamp = new LinearColorRampInfo(starts[i], ends[i]);
                for (int j = 0; j < colNum; j++)
                {
                    int r = starts[i].R + (ends[i].R - starts[i].R) * j / colNum;
                    int g = starts[i].G + (ends[i].G - starts[i].G) * j / colNum;
                    int b = starts[i].B + (ends[i].B - starts[i].B) * j / colNum;
                    Color c = Color.FromArgb(alpha, r, g, b);
                    colorRamp.ColorList.Add(c);
                }
                colorRampList.Add(colorRamp);
            }
            return colorRampList;
        }

        /// <summary>
        /// 获取渐变颜色条带
        /// </summary>
        /// <param name="start">起始颜色</param>
        /// <param name="end">终止颜色</param>
        /// <param name="alpha">透明度</param>
        /// <param name="num">颜色个数</param>
        /// <returns></returns>
        public static List<Color> GetLinearColorRamp(Color start, Color end, int alpha, int num)
        {
            List<Color> colorRamp = new List<Color>();
            for (int j = 0; j < num; j++)
            {
                int r = start.R + (end.R - start.R) * j / num;
                int g = start.G + (end.G - start.G) * j / num;
                int b = start.B + (end.B - start.B) * j / num;
                Color c = Color.FromArgb(alpha, r, g, b);
                colorRamp.Add(c);
            }
            return colorRamp;
        }

        /// <summary>
        /// 获取随机的渐变颜色条带列表
        /// </summary>
        /// <param name="colNum">每一行的颜色个数</param>
        /// <param name="rowNum">行数</param>
        /// <param name="alpha">透明度</param>
        /// <returns></returns>
        public static List<RandomColorRampInfo> GetRandomColorRamps(int colNum, int rowNum, int alpha)
        {
            List<RandomColorRampInfo> colorRampList = new List<RandomColorRampInfo>();
            for (int i = 1; i <= rowNum; i++)
            {
                RandomColorRampInfo colorRamp = new RandomColorRampInfo();
                for (int j = 1; j <= colNum; j++)
                {
                    Random rd = new Random((int)DateTime.Now.Ticks / i * j);
                    Color c = Color.FromArgb(alpha, rd.Next(255), rd.Next(255), rd.Next(255));
                    colorRamp.ColorList.Add(c);
                }
                colorRampList.Add(colorRamp);
            }
            return colorRampList;
        }

    }
}
