## *iTelluro.DataTools.Utility*  
原本是为DataTool工具开发的辅助类库，但已包含诸多实用功能，可减少开发工作量。  
项目wiki请见：  
http://gitlabnet.infoearth.com/framework/iTelluro.DataTools.Utility/wikis/Home  

### 1.0
*  【新增功能】DOMTile增加按天地图规则切片功能
*  【新增功能】DEMInterpolation增加一种插值函数
*  【功能优化】ImgExport生成缩略图使用nodata值
*  【新增功能】BilDemFile类增加一个写dem的方法
*  【新增功能】ImgHelp类增加使用GDI绘制带洞面的方法

### 1.0.5990.17576  
* 【新增功能】新增影像图快速打包成MBT(DOM中的DOMTile),切片直接通过二进制流方式存储，不用存储临时文件图片。  
* 【修改BUG】修改GdalHelp.GetGeoCoords函数的bug，增加单元测试用例  
* 【功能优化】等值线晕眩图增加输出分辨率  
* 【修改BUG】修改道格拉斯抽稀算法里的一个bug  

### 1.0.5795.30508  
* 【修改BUG】DEMInterpolation类解决分段渲染时nodata导致的显示错误  
* 【功能优化】GIS空间Const类重新整理北京54和西安80、CGC2000的3度带和6度带的带号和wkt  
* 【修改BUG】解决影像太小时前几级切片丢失的问题  
* 【功能优化】影像切片跨度较大时，被切影像区域不满足像素显示则作为空白图片显示  

### 1.0.5772.28359  
* 【修改BUG】修改根据范围计算图幅号函数（MapCode.GetMapCodeByScale）的bug，增加单元测试用例。  
* 【新增功能】新增矢量切片模块（DLG文件夹），切片支持纹理渲染。  
* 【新增功能】GIS命名空间增加ProjectSystemConst类，提供对投影坐标系的枚举和查询等。  
* 【新增功能】GIS命名空间新增NEUSystem类，提供XYZ直角空间坐标和NEU测站地方空间直角坐标系的坐标相互转换。增加单元测试用例。  
* 【新增功能】Marshal命名空间新增Matrix类，提供矩阵计算功能。  
* 【功能优化】DEM切片类在分块读取数据时将分块尺寸修改为按实时内存占用量调整，避免内存溢出。  
* 【新增功能】增加切片插值类ImgInterpolation，支持最邻近、双线性、halftone三种插值算法。  
* 【功能优化】BilDemFile.ReadData方法自动识别BIL文件内数据格式，兼容int16和float32.  
* 【功能优化】BilDemFile.WriteData方法输出INT16时使用四舍五入代替直接取整。  
* 【新增功能】DEMTile类增加输出single类型bil文件，使用TileExportArg.outputint16参数设置。  
* 【新增功能】DEMHelp类中DEM晕眩图和灰度渲染图都支持float32数据精度。  

### 1.0.5596.20367  
* 【修改BUG】修改矢量栅格化模块栅格化点图层的BUG  
* 【修改BUG】修改UIRecord类保存TabControl控件参数时的动作。  
* 【新增功能】Contour类新增生成等高线切片数据集功能  
* 【新增功能】GdalHelp类增加删除栅格文件和矢量文件功能，增加获取shp驱动和tif驱动属性。  
* 【新增功能】DemConvert类增加将dem数据转成矢量图层功能  
* 【新增功能】ImgHelp类增加图片按不规则路径裁剪方法  
* 【新增功能】ShpHelp类增加设置坐标系方法  
* 【新增功能】CoordTransform类增加矢量文件坐标系投影转换方法。  
* 【新增功能】Const类增加bj54/xian80/cgcs2000系列投影坐标系常量。  
* 【新增功能】IOHelp类增加readfile方法，直接读取文件的所有内容。  
* 【功能优化】使用新算法处理nodata数据，大幅提升处理效率，同时降低内存占用。  
* 【新增功能】Marshal类增加异步运行隐藏进程函数。  
* 【功能优化】投影转换功能使用多线程处理。  
* 【新增功能】GeoAlg/RoutePlanner包增加最短路径规划算法  
* 【新增功能】GIS包增加最短路径规划模块，指定起点终点后从shp文件提取路网规划路径。  
* 【新增功能】增加线线关系算法GeoAlg/TwoLines  
* 【新增功能】Point类增加向量运算函数、增加LineString类等。  
* 【功能优化】DirPackTool修改获取切片编号的方法，跳过格式不正确的文件。  
* 【新增功能】UIRecord类增加保存用户附加信息和提取用户附加信息功能。  
* 【新增功能】增加ImgFillFlood和GrayFillFlood类，提供将图片指定处或边缘处的某种颜色（及相连续的同颜色区域）替换成另一种颜色功能。  
* 【新增功能】Marshal类增加FromColorStr方法将颜色字符串转成Color对象，支持超过四种格式。  

### 1.0.5436.31497  
* 【修改BUG】修改某些情况下GetAllKeys结果错误的bug  
* 【【重要bug修复】解决globeDB格式数据包内切片错误问题。  
* 【新增功能】DEM模块增加绘制等高线功能，由坐标串生成等高线图层或等高线图片。  
* 【新增功能】DEM模块增加dem数据文件格式互转功能。  
* 【新增功能】增加GeoAlg模块，提供几何相关算法。  
* 【新增功能】Marshal类增加GetBinDir方法，获取程序启动目录或web的bin目录，并修改库中所有获取启动目录的代码。  
* 【新增功能】线程池增加所有工作完成后通知事件。用于异步通知。  

### 1.0.5406.19084  
* 【新增功能】创建缓冲区方法增加进度通知，InvokeHelp类增加进度条invoke方法  
* 【新增功能】增加用explorer打开文件目录功能(IOHelp.ExplorerFolder)  
* 【新增功能】增加对栅格影像进行最大最小值和标准差拉伸处理的功能  
* 【修改BUG】修改MapCode.GetMapCodeByScale方法里计算图符号的算法错误  
* 【新增功能】增加实时读取命令行输出函数ConsoleLog.ReadConsoleLog/ReadConsoleErr  
* 【新增功能】Marshal类增加数组内容比较和List元素值比较的函数。  
* 【修改BUG】修改Marshal.Unique方法的错误。  

### 1.0.5366.18769  
* 【新增功能】增加将二进制数据和16进制字符串互转的函数Marshal.GetHexStr，FromHexStr  
* 【新增功能】增加利用GEOS库创建缓冲区功能  
* 【新增功能】增加道路线打断功能  
* 【功能优化】增加矢量feature内存释放代码  

### 1.0.5357.29273  
* 【新增功能】MakeBufferArea类增加对单个geometry创建缓冲区的方法  
* 【新增功能】增加一种TilePackTool.Connect方法形式。  
* 【功能优化】优化DEM切图算法，解决多线程切图时内存溢出问题  
* 【【解决bug】屏蔽gdb文件打开失败的异常，避免程序崩溃。  
* 【【解决bug】解决在中文路径下dem彩色渲染失败问题  
* 【新增功能】CoordTransform类增加经纬度到影像行列号转换的方法。  

### 1.0.5346.17841  
* 【修改BUG】解决生成缩略图时颜色通道错误  

### 1.0.5345.31661  
* 【修改BUG】解决非黑白色的透明色和背景色无效问题。影响DOM切片功能。  
* 【新增功能】GdalHelp类增加CreateRasterFile方法。  
