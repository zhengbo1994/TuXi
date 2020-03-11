private static string _serverIp = "localhost";
private static string _serverPort = "8080";

GeoServer _geoserver = new GeoServer(_serverIp, _serverPort);

#region 工作区
//获取所有工作区
//IEnumerable<WorkSpace> actual = _geoserver.GetWorkSpaces();
//foreach (var item in actual)
//{
//    Console.WriteLine(item.Name + " " + item.Href);
//}

//获取默认工作区
//WorkSpace defaultWorkSpace = _geoserver.GetDefaultWorkSpace();
//Console.WriteLine(defaultWorkSpace.Name + " " + defaultWorkSpace.Href);

//添加工作区
//_geoserver.AddWorkSpace("karls");

//删除工作区
//_geoserver.DeleteWorkSpace("karl");
#endregion

#region 数据存储
//获取数据存储
//IEnumerable<DataStore> actual = _geoserver.GetDataStores("topp");
//foreach (var item in actual)
//{
//    Console.WriteLine(item.Name + " " + item.Href);
//}
//添加数据存储
//_geoserver.AddDataStore("karl", "dbGIS", "localhost", "5432", "postgis_test", "postgres", "1qaz2wsx");
//删除数据存储
//_geoserver.DeleteDataStore("karl", "dbGIS");
#endregion

#region 图层组
//获取图层组
//IEnumerable<LayerGroup> actual = _geoserver.GetLayerGroups("karl");
//foreach (var item in actual)
//{
//    Console.WriteLine(item.Name + " " + item.Href);
//}
//添加图层组
//_geoserver.AddLayerGroup("TestGroup", new List<string>() { "dijishi", "diqujie" }, new List<string>() { "point", "polygon" }, "karl");
//删除图层组
//_geoserver.DeleteLayerGroup("testgroup", "nurc");
#endregion

#region 图层
//IEnumerable<Layer> actual = _geoserver.GetLayers("karl");
//foreach (var item in actual)
//{
//    Console.WriteLine(item.Name + " " + item.Href);
//}
//添加图层
//_geoserver.AddLayer("diqujie", "karl", "dbGIS", SrsType.Epsg4326);
//删除图层
//_geoserver.DeleteLayer("karl", "dbGIS", "diqujie");
#endregion

#region 样式
//IEnumerable<Style> actual = _geoserver.GetStyles();
//foreach (var item in actual)
//{
//    Console.WriteLine(item.Name + " " + item.Href);
//}
//_geoserver.ModifyLayerStyle("chanzhuangdian", "point");
_geoserver.AddStyle("H49Z016D23", "D23.sld", @"E:\Data\shuiwendizhitu20W\样式\D23.sld");
#endregion

#region 切片
//查询
//List<string> listGridSet = new List<string>();
//var geoServerLayer = _geoserver.GetLayerGridSet("karl:dicengxian");
//foreach (var item in geoServerLayer)
//{
//    //Console.WriteLine(item);
//    listGridSet.Add(item);
//}
//listGridSet.Add("iTelluro");
////修改切片规则
//_geoserver.BindLayerGridSet("karl:dicengxian", listGridSet);
//切片任务
//_geoserver.SendGridSetTask("karl:dicengxian", "iTelluro", 0, 13);
#endregion
#endregion