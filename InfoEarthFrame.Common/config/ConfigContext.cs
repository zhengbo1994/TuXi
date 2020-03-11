using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common
{
    [XmlRoot]
    public class ConfigContext
    {
        public const string ConfigContextKey = "ConfigContext.xml";
        private static readonly Cache Cache = System.Web.HttpRuntime.Cache;
        public static object FileLock = new object();
        static ConfigContext()
        {
            if (!File.Exists(XmlPath))
            {
                CreateFile(new ConfigContext());
            }
        }
        private static string XmlPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"\configs\" + ConfigContextKey;
            }
        }


        [XmlIgnore]
        public static ConfigContext Current
        {
            get
            {
                try
                {
                    var obj = Cache[ConfigContextKey];
                    if (obj == null)
                    {
                        lock (FileLock)
                        {
                            if (obj == null)
                            {
                                CacheDependency dp = new CacheDependency(XmlPath);//建立缓存依赖项dp
                                using (var fs = new FileStream(XmlPath, FileMode.Open))
                                {
                                    using (var sr = new StreamReader(fs))
                                    {
                                        var context = InfoEarthFrame.Common.XmlConvert.XmlDeserialize<ConfigContext>(sr.ReadToEnd(), System.Text.Encoding.UTF8);

                                        //set permession path
                                        foreach (var module in context.ModuleConfig.Modules)
                                        {
                                            module.Path = module.ID;
                                            module.ParentID = "";

                                            foreach (var menu in module.Menus)
                                            {
                                                menu.Path = module.Path + "/" + menu.ID;
                                                menu.ParentID = module.ID;
                                            }

                                            SetPath(module);
                                        }

                                        Cache.Insert(ConfigContextKey, context, dp);
                                        return context;
                                    }
                                }
                            }
                        }

                    }
                    return (ConfigContext)obj;
                }
                catch (Exception ex)
                {
                    throw new Exception("获取配置文件发生错误，请检查【" + ConfigContextKey + "】文件", ex);
                }
            }
        }

        private static void SetPath(Module parentModule)
        {
            var modules = parentModule.Children;
            var menus = parentModule.Menus;
            if (modules != null && modules.Any())
            {
                foreach (var module in modules)
                {
                    module.Path = parentModule.Path + "/" + module.ID;
                    module.ParentID = parentModule.ID;
                    SetPath(module);
                }
            }

            if (menus != null && menus.Any())
            {
                foreach (var menu in menus)
                {
                    menu.Path = parentModule.Path + "/" + menu.ID;
                    menu.ParentID = parentModule.ID;
                }
            }

        }

        public void Save()
        {
            CreateFile(Current);
        }

        private static void CreateFile(ConfigContext context)
        {
            using (var fs = new FileStream(XmlPath, FileMode.Create))
            {
                using (var sr = new StreamWriter(fs))
                {
                    var configText = InfoEarthFrame.Common.XmlConvert.XmlSerialize<ConfigContext>(context);
                    sr.Write(configText);
                }
            }
        }
        private AuthConfig _authConfig;
        [XmlElement]
        public AuthConfig AuthConfig
        {
            get
            {
                if (_authConfig == null)
                {
                    _authConfig = new AuthConfig();
                }
                return _authConfig;
            }
            set
            {
                _authConfig = value;
            }
        }

        private ApiConfig _apiConfig;
        [XmlElement]
        public ApiConfig ApiConfig
        {
            get
            {
                if (_apiConfig == null)
                {
                    _apiConfig = new ApiConfig();
                }
                return _apiConfig;
            }
            set
            {
                _apiConfig = value;
            }
        }

        private FtpConfig _ftpConfig;
        [XmlElement]
        public FtpConfig FtpConfig
        {
            get
            {
                if (_ftpConfig == null)
                {
                    _ftpConfig = new FtpConfig();
                }
                return _ftpConfig;
            }
            set
            {
                _ftpConfig = value;
            }
        }


        private DefaultConfig _defaultConfig;

        [XmlElement]
        public DefaultConfig DefaultConfig
        {
            get
            {
                if (_defaultConfig == null)
                {
                    _defaultConfig = new DefaultConfig();
                }
                return _defaultConfig;
            }
            set
            {
                _defaultConfig = value;
            }
        }
        private ModuleConfig _moduleConfig;
        [XmlElement]
        public ModuleConfig ModuleConfig
        {
            get
            {
                if (_moduleConfig == null)
                {
                    _moduleConfig = new ModuleConfig();
                }
                return _moduleConfig;
            }
            set
            {
                _moduleConfig = value;
            }
        }

        private PermessionConfig _permessionConfig;
        [XmlElement]
        public PermessionConfig PermessionConfig
        {
            get
            {
                if (_permessionConfig == null)
                {
                    _permessionConfig = new PermessionConfig();
                }
                return _permessionConfig;
            }
            set
            {
                _permessionConfig = value;
            }
        }

        [XmlIgnore]
        public string DefaultConfigJson
        {
            get
            {
                var dic = new Dictionary<string, object>();

                DefaultConfig.Items.ForEach(p =>
                {
                    if (!dic.ContainsKey(p.Key))
                    {
                        dic.Add(p.Key, p.Value);
                    }
                });
                dic.Add("BaseUrl", ApiConfig.BaseUrl);
                return JsonConvert.SerializeObject(dic);
            }
        }

        private Dictionary<string, string> _packageCategory;
        [XmlIgnore]
        public Dictionary<string, string> PackageCategory
        {
            get
            {
                if (_packageCategory == null)
                {
                    _packageCategory = new Dictionary<string, string>();
                    _packageCategory.Add("文档", "1文档");
                    _packageCategory.Add("系统库", "2系统库");
                    _packageCategory.Add("栅格图", "3栅格图");
                    _packageCategory.Add("说明书", "4说明书");
                    _packageCategory.Add("元数据", "5元数据");
                    _packageCategory.Add("要素类文件", "6要素类文件");
                    _packageCategory.Add("地理图层", "6要素类文件\\地理图层");
                    _packageCategory.Add("专业图层", "6要素类文件\\专业图层");
                    _packageCategory.Add("制图文件", "7制图文件");
                }
                return _packageCategory;
            }
        }
    }

    public class DefaultConfig
    {
        public DefaultConfig()
        {
            this.Items = new List<ConfigItem>();
        }

        public string this[string key]
        {
            get
            {
                var item = Items.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());
                if (item == null)
                {
                    throw new Exception("未找到[/ConfigContext/DefaultConfig/Item[Key=" + key + "]]的配置项，请检查[" + ConfigContext.ConfigContextKey + "]文件");
                }

                return item.Value;
            }
        }

        [XmlElement(ElementName = "Item")]
        public List<ConfigItem> Items { get; set; }
    }

    public class ConfigItem
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public string Value { get; set; }

        [XmlAttribute]
        public string Comment { get; set; }
    }
    public class AuthConfig
    {
        [XmlAttribute]
        public bool IsEnabled { get; set; }

        [XmlElement]
        public AllowControllers AllowControllers { get; set; }

        public bool IsValid(System.Web.Mvc.AuthorizationContext context)
        {
            if (!IsEnabled)
            {
                return true;
            }

            var controllerName = context.RouteData.Values["controller"].ToString();
            var action = context.RouteData.Values["action"].ToString();
            var controller = AllowControllers.All.FirstOrDefault(p => p.Name.ToLower() == controllerName.ToLower());
            if (controller == null)
            {
                return false;
            }

            return !controller.DisallowActions.Any(p => p.ToLower() == action.ToLower());
        }
    }



    public class AllowControllers
    {
        public AllowControllers()
        {
            this.All = new List<Controller>();
        }

        [XmlElement(ElementName = "Controller")]
        public List<Controller> All { get; set; }
    }

    public class Controller
    {
        [XmlAttribute]
        public string Name { get; set; }

        private string _disallowAction;
        [XmlAttribute]
        public string DisallowAction
        {
            get
            {
                return _disallowAction = _disallowAction ?? string.Empty;
            }
            set
            {
                _disallowAction = value;
            }
        }

        [XmlIgnore]
        public string[] DisallowActions
        {
            get
            {
                return DisallowAction.Split(',');
            }
        }
    }

    public class ApiConfig
    {
        public ApiConfig()
        {
            this.Items = new List<ConfigItem>();
        }
        [XmlAttribute]
        public string BaseUrl { get; set; }


        [XmlElement(ElementName = "Item")]
        public List<ConfigItem> Items { get; set; }

        public string this[string key]
        {
            get
            {
                var item = Items.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());
                if (item == null)
                {
                    throw new Exception("未找到[/ConfigContext/ApiConfig/Item[Key=" + key + "]]的配置项，请检查[" + ConfigContext.ConfigContextKey + "]文件");
                }

                return item.Value;
            }
        }
    }

    public class FtpConfig
    {

        public FtpConfig()
        {
            this.Ftps = new List<Ftp>();
        }

        public Ftp this[string key]
        {
            get
            {
                var item = Ftps.FirstOrDefault(p => p.Name.ToLower() == key.ToLower());
                if (item == null)
                {
                    throw new Exception("未找到[/ConfigContext/FtpConfig/Ftp[Name=" + key + "]]的配置项，请检查[" + ConfigContext.ConfigContextKey + "]文件");
                }

                return item;
            }
        }

        [XmlElement(ElementName = "Ftp")]
        public List<Ftp> Ftps { get; set; }
    }

    public class Ftp
    {

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Site { get; set; }


        [XmlElement]
        public string DirectoryPath
        {
            get;
            set;
        }

        [XmlElement]
        public string Host { get; set; }

        private int? _port;
        [XmlElement]
        public int Port
        {
            get
            {
                if (_port == null)
                {
                    _port = 21;
                }
                return _port.Value;
            }
            set
            {
                _port = value;
            }
        }

        [XmlElement]
        public string UserName { get; set; }


        [XmlElement]
        public string Password { get; set; }
    }

    public class ModuleConfig
    {
        public ModuleConfig()
        {
            this.Modules = new List<Module>();
        }

        [XmlElement(ElementName = "Module")]
        public List<Module> Modules { get; set; }

        public Module this[string moduleId]
        {
            get
            {
                var module = Modules.FirstOrDefault(p => p.ID == moduleId);
                if (module == null)
                {
                    throw new KeyNotFoundException("未找到[/ConfigContext/ModuleConfig/Module[ID=" + moduleId + "]]的配置项，请检查[" + ConfigContext.ConfigContextKey + "]文件");
                }

                return module;
            }
        }

        public bool AddModuleOperate(string moduleId, string parentId, string currentPath, string Name, string MenuValue, string OperatingState)
        {
            bool flag = InsertAndUpdateParentModulePermession(this.Modules, moduleId, parentId, currentPath, Name, MenuValue, OperatingState);
            return flag;
        }

        public bool UpdateModuleOperate(string moduleId, string parentId, string currentPath, string Name, string MenuValue, string OperatingState)
        {
            bool flag = InsertAndUpdateParentModulePermession(this.Modules, moduleId, parentId, currentPath, Name, MenuValue, OperatingState);
            return flag;
        }

        public bool AddMenuOperate(string menuId, string parentId, string currentPath, string Name, string MenuValue, string OperatingState, List<string> SelectValueList)
        {
            bool flag = InsertAndUpdateParentMenuPermession(this.Modules, "", menuId, currentPath, Name, MenuValue, OperatingState, SelectValueList);
            return flag;
        }

        public bool UpdateMenuOperate(string menuId, string parentId, string currentPath, string Name, string MenuValue, string OperatingState, List<string> SelectValueList)
        {
            bool flag = InsertAndUpdateParentMenuPermession(this.Modules, menuId, parentId, currentPath, Name, MenuValue, OperatingState, SelectValueList);
            return flag;
        }
        public void DeleteMenu(string moduleId, string menuId)
        {
            RemoveMenu(this.Modules, moduleId, menuId);
        }

        public Dictionary<string, string> GetMenuInfo(string menuId, string parentId, string currentPath, string Name, string MenuValue, string OperatingState, string CurrentType)
        {
            Dictionary<string, string> parentModule = new Dictionary<string, string>();
            if (CurrentType == "module")
            {
                var current = FindModule(this.Modules, menuId);
                parentModule.Add(current.Name, current.Icon);
            }
            else
            {
                Menu current = new Menu();
                var modulesData = FindModule(this.Modules, parentId);
                foreach (Menu menu in modulesData.Menus)
                {
                    if (menu.ID == menuId)
                    {
                        current = menu;
                        break;
                    }
                }
                parentModule.Add(current.Name, current.Url);

            }

            return parentModule;
        }

        public ParentMenu GetMenuDataInfo(string menuId, string parentId, string currentPath, string Name, string MenuValue, string OperatingState, string CurrentType)
        {
            ParentMenu parentModule = new ParentMenu();
            if (CurrentType == "module")
            {
                var current = FindModule(this.Modules, menuId);
                parentModule.Name = current.Name;
                parentModule.Icon = current.Icon;
                parentModule.Url = null;
                parentModule.Buttons = null;
            }
            else
            {
                Menu current = new Menu();
                var modulesData = FindModule(this.Modules, parentId);
                foreach (Menu menu in modulesData.Menus)
                {
                    if (menu.ID == menuId)
                    {
                        current = menu;
                        break;
                    }
                }

                List<string> ButtonsList = new List<string>();
                var currentButtons = "";
                for (int i = 0; i < current.Buttons.Count; i++)
                {
                    ButtonsList.Add(current.Buttons[i].Name);
                }



                parentModule.Name = current.Name;
                parentModule.Url = current.Url;
                parentModule.Icon = null;
                parentModule.Buttons = ButtonsList;
            }

            return parentModule;
        }

        public class ParentMenu
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Url { get; set; }
            public List<string> Buttons { get; set; }
        }

        private void RemoveMenu(IEnumerable<Module> modules, string moduleId, string menuId)
        {
            if (modules == null || !modules.Any())
            {
                return;
            }

            foreach (var module in modules)
            {
                if (module.ID == moduleId)
                {
                    foreach (var menu in module.Menus)
                    {
                        if (menu.ID == menuId)
                        {
                            module.Menus.Remove(menu);
                            break;
                        }
                    }

                    foreach (var children in module.Children)
                    {
                        if (children.ID == menuId)
                        {
                            module.Children.Remove(children);
                            break;
                        }
                    }
                }
                if (moduleId == "" && module.ID == menuId)
                {
                    this.Modules.Remove(module);
                    break;
                }

                RemoveMenu(module.Children, moduleId, menuId);
            }
        }


        private bool InsertAndUpdateParentMenuPermession(IList<Module> modules, string menuId, string parentId, string currentPath, string Name, string MenuValue, string OperatingState, List<string> SelectValueList)
        {
            var parentModule = FindModule(modules, parentId);

            if (parentModule == null)
            {
                return false;
            }

            if (OperatingState == "insert")
            {
                string idsre = (parentModule.Children.Count + parentModule.Menus.Count + 1).ToString();

                List<Button> ButtonList = new List<Button>();
                foreach (var item in SelectValueList)
                {
                    ButtonList.Add(new Button()
                    {
                        Name = item,
                        HasPermession = true
                    });
                }

                parentModule.Menus.Add(new Menu
                {
                    Name = Name,
                    ID = parentId + idsre,
                    Url = MenuValue,
                    Buttons = ButtonList
                });

        
            }
            else
            {
                var menu = parentModule.Menus.FirstOrDefault(p => p.ID == menuId);
                if (menu != null)
                {
                    menu.Name = Name;
                    menu.Url = MenuValue;
                }

                menu.Buttons = new List<Button>();
                foreach (var item in SelectValueList)
                {
                    menu.Buttons.Add(new Button()
                    {
                        Name = item,
                        HasPermession = true
                    });
                }
            }

            return true;
        }

        private bool InsertAndUpdateParentModulePermession(IList<Module> modules, string moduleId, string parentId, string currentPath, string Name, string MenuValue, string OperatingState)
        {
            var parentModule = FindModule(modules, moduleId);

            if (parentModule == null)
            {
                if (moduleId == "0" && parentId == "0" && currentPath == "0" && OperatingState == "insert")
                {
                    string idsre = (modules.Count + 1).ToString();
                    modules.Add(new Module
                    {
                        Name = Name,
                        ID = idsre,
                        Icon = MenuValue,
                    });
                    return true;
                }

                return false;
            }

            if (OperatingState == "insert")
            {
                string idsre = (parentModule.Children.Count + parentModule.Menus.Count + 1).ToString();

                parentModule.Children.Add(new Module
                {
                    Name = Name,
                    ID = moduleId + idsre,
                    Icon = MenuValue,
                });
                return true;
            }
            else
            {
                parentModule.Name = Name;
                parentModule.Icon = MenuValue;
            }
            return true;
        }

        private Module FindModule(IEnumerable<Module> modules, string moduleId)
        {
            if (modules == null || !modules.Any())
            {
                return null;
            }

            Module current = null;
            foreach (var module in modules)
            {
                if (module.ID == moduleId)
                {
                    current = module;
                    break;
                }
                else
                {
                    current = FindModule(module.Children, moduleId);
                    if (current != null)
                    {
                        break;
                    }
                }
            }

            return current;
        }

        private Dictionary<string, List<Button>> _menuButtons;
        [XmlIgnore]
        public Dictionary<string, List<Button>> MenuButtons
        {
            get
            {
                if (_menuButtons == null)
                {
                    _menuButtons = new Dictionary<string, List<Button>>();

                    var modules = this.Modules;
                    foreach (var module in modules)
                    {
                        var menus = module.Menus;
                        foreach (var menu in menus)
                        {
                            _menuButtons.Add(menu.Url, menu.Buttons);
                        }

                        GetModuleMenuButtons(module);
                    }
                }
                return _menuButtons;
            }
        }

        private void GetModuleMenuButtons(Module parentModule)
        {
            foreach (var module in parentModule.Children)
            {
                var menus = module.Menus;
                foreach (var menu in menus)
                {
                    _menuButtons.Add(menu.Url, menu.Buttons);
                }
                GetModuleMenuButtons(module);
            }
        }
        public List<Button> GetMenuButton(string menuUrl)
        {
            var modules = this.Modules;
            foreach (var module in modules)
            {
                var menus = module.Menus;
                foreach (var menu in menus)
                {
                    if (menuUrl.ToLower().Contains(menu.Url.ToLower()))
                    {
                        return menu.Buttons;
                    }
                }
            }
            return null;
        }

        public void SetMenuButtonArea(string moduleId, string menuId, List<string> SelectValueList)
        {
            var menu = FindMenu(this.Modules, moduleId, menuId);
            if (menu == null)
            {
                throw new Exception("请先设置菜单权限");
            }

            var buttons = menu.Buttons;
            //var current = buttons.FirstOrDefault(p => p.Name.ToLower() == buttonName.ToLower());

            //if (current == null)
            //{
            //    buttons.Add(new Button
            //    {
            //        Name = buttonName
            //    });
            //}
        }

        //public bool DelMenuButtonArea(string moduleId, string menuId, string buttonName)
        //{
        //    var menu = FindMenu(this[groupId].Modules, moduleId, menuId);
        //    if (menu == null)
        //    {
        //        throw new Exception("请先设置菜单权限");

        //    }

        //    var buttons = menu.Buttons;
        //    var current = buttons.FirstOrDefault(p => p.Name.ToLower() == buttonName.ToLower());
        //    if (!HasPermession)
        //    {
        //        if (current != null)
        //        {
        //            buttons.Remove(current);
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        if (current == null)
        //        {
        //            buttons.Add(new Button
        //            {
        //                Name = buttonName
        //            });
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public bool SetMenuButtonArea(string moduleId, string menuId, string buttonName, bool HasPermession)
        {
            var menu = FindMenu(this.Modules, moduleId, menuId);
            if (menu == null)
            {
                throw new Exception("请先设置菜单权限");

            }

            var buttons = menu.Buttons;
            var current = buttons.FirstOrDefault(p => p.Name.ToLower() == buttonName.ToLower());
            if (!HasPermession)
            {
                if (current != null)
                {
                    buttons.Remove(current);
                }
                return true;
            }
            else
            {
                if (current == null)
                {
                    buttons.Add(new Button
                    {
                        Name = buttonName
                    });
                    return true;
                }
            }
            return false;
        }

        private Menu FindMenu(IEnumerable<Module> modules, string moduleId, string menuId)
        {
            if (modules == null || !modules.Any())
            {
                return null;
            }

            foreach (var module in modules)
            {
                if (module.ID == moduleId)
                {
                    foreach (var menu in module.Menus)
                    {
                        if (menu.ID == menuId)
                        {
                            return menu;
                        }
                    }
                }

                var item = FindMenu(module.Children, moduleId, menuId);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }
    }
    public class Module
    {
        public Module()
        {
            this.Menus = new List<Menu>();
            this.Children = new List<Module>();
        }

        public Menu this[string menuId]
        {
            get
            {
                var menu = Menus.FirstOrDefault(p => p.ID == menuId);
                if (menu == null)
                {
                    throw new Exception("未找到[/ConfigContext/ModuleConfig/Module/Menu[ID=" + menuId + "]]的配置项，请检查[" + ConfigContext.ConfigContextKey + "]文件");
                }

                return menu;
            }
        }

        [XmlAttribute]
        public string ID { get; set; }


        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Icon
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Menu")]
        public List<Menu> Menus { get; set; }

        [XmlElement(ElementName = "Module")]
        public List<Module> Children { get; set; }

        [XmlIgnore]
        public string ParentID { get; set; }

        [XmlIgnore]
        public string Path { get; set; }
    }

    public class Menu
    {
        public Menu()
        {
            this.Buttons = new List<Button>();
        }

        [XmlAttribute]
        public string ID { get; set; }
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Url
        {
            get;
            set;
        }

        [XmlIgnore]
        public string ParentID { get; set; }

        [XmlIgnore]
        public string Path { get; set; }

        [XmlElement(ElementName = "Button")]
        public List<Button> Buttons { get; set; }
    }


    public class Button
    {
        [XmlText]
        public string Name { get; set; }

        [XmlIgnore]
        public bool HasPermession { get; set; }
    }
    public class PermessionConfig
    {
        public PermessionConfig()
        {
            this.Groups = new List<Group>();
        }

        [XmlElement(ElementName = "Group")]
        public List<Group> Groups { get; set; }

        public Group this[string groupId]
        {
            get
            {
                var group = Groups.FirstOrDefault(p => p.ID == groupId);
                if (group == null)
                {
                    group = new Group
                    {
                        ID = groupId
                    };
                    Groups.Add(group);
                }

                return group;
            }
        }


        public List<Module> GetModules(string groupId)
        {
            return this[groupId].Modules;
        }

        public List<Button> GetMenuButtonPermession(IEnumerable<string> groupIds, string menuUrl)
        {
            var list = new List<Button>();
            foreach (var groupId in groupIds)
            {
                var modules = this[groupId].Modules;
                foreach (var module in modules)
                {
                    var menus = module.Menus;
                    foreach (var menu in menus)
                    {
                        if (menuUrl.ToLower().Contains(menu.Url.ToLower()))
                        {
                            list.AddRange(menu.Buttons);
                            break;
                        }
                    }
                }
            }

            List<Button> buttons;
            menuUrl = menuUrl.Replace(ConfigContext.Current.DefaultConfig["KJWebUrl"], "");
            var flag = ConfigContext.Current.ModuleConfig.MenuButtons.TryGetValue(menuUrl, out buttons);
            if (flag)
            {
                foreach (var current in list)
                {
                    foreach (var button in buttons)
                    {
                        if (button.Name == current.Name)
                        {
                            button.HasPermession = true;
                            break;
                        }
                    }
                }
            }
            return buttons;
        }

        private Module FindModule(IEnumerable<Module> modules, string moduleId)
        {
            if (modules == null || !modules.Any())
            {
                return null;
            }

            Module current = null;
            foreach (var module in modules)
            {
                if (module.ID == moduleId)
                {
                    current = module;
                    break;
                }
                else
                {
                    current = FindModule(module.Children, moduleId);
                    if (current != null)
                    {
                        break;
                    }
                }
            }

            return current;
        }





        public void RemoveModule(string groupId, string moduleId, string currentPath)
        {
            var modules = ConfigContext.Current.PermessionConfig[groupId].Modules;
            var moduleIds = currentPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in moduleIds)
            {
                var module = FindModule(modules, id);
                if (module != null)
                {
                    if (module.ID == moduleId)
                    {
                        modules.Remove(module);
                    }
                    RemoveModule(moduleId, module);
                }
            }
        }

        private void RemoveModule(string moduleId, Module parentModule)
        {
            if (parentModule == null)
            {
                return;
            }
            var modules = parentModule.Children;
            if (modules == null || !modules.Any())
            {
                return;
            }
            for (int i = 0, length = modules.Count; i < length; i++)
            {

                var module = FindModule(modules, moduleId);
                if (module != null)
                {
                    modules.Remove(module);
                }
                RemoveModule(moduleId, module);
            }
        }

        private bool CreateParentModulePermession(IList<Module> modules, string id, bool hasChildren)
        {
            var parentModule = FindModule(modules, id);
            if (parentModule == null)
            {
                parentModule = FindModule(ConfigContext.Current.ModuleConfig.Modules, id);
                if (parentModule == null)
                {
                    return false;
                }
                modules.Add(new Module
                {
                    Name = parentModule.Name,
                    ID = parentModule.ID,
                    Children = hasChildren ? parentModule.Children : new List<Module>(),
                    Menus = hasChildren ? parentModule.Menus : new List<Menu>(),
                    Icon = parentModule.Icon,
                    ParentID = parentModule.ParentID,
                    Path = parentModule.Path
                });
                return true;
            }
            return true;
        }

        private void RemoveMenu(IEnumerable<Module> modules, string moduleId, string menuId)
        {
            if (modules == null || !modules.Any())
            {
                return;
            }


            foreach (var module in modules)
            {
                if (module.ID == moduleId)
                {
                    foreach (var menu in module.Menus)
                    {
                        if (menu.ID == menuId)
                        {
                            module.Menus.Remove(menu);
                            break;
                        }
                    }
                }

                RemoveMenu(module.Children, moduleId, menuId);
            }
        }

        private Menu FindMenu(IEnumerable<Module> modules, string moduleId, string menuId)
        {
            if (modules == null || !modules.Any())
            {
                return null;
            }

            foreach (var module in modules)
            {
                if (module.ID == moduleId)
                {
                    foreach (var menu in module.Menus)
                    {
                        if (menu.ID == menuId)
                        {
                            return menu;
                        }
                    }
                }

                var item = FindMenu(module.Children, moduleId, menuId);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        public void RemoveMenu(string groupId, string moduleId, string menuId)
        {
            RemoveMenu(ConfigContext.Current.PermessionConfig[groupId].Modules, moduleId, menuId);
        }

        public void AddModule(string groupId, string moduleId, string parentId, string currentPath)
        {
            var moduleIds = currentPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in moduleIds)
            {
                CreateParentModulePermession(this[groupId].Modules, id, id == moduleId);
            }
        }


        public void AddMenu(string groupId, string moduleId, string menuId, string currentPath)
        {
            var moduleIds = currentPath.Replace("/" + menuId, "").Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in moduleIds)
            {
                //判断模块节点是否存在
                CreateParentModulePermession(this[groupId].Modules, id, false);

                if (id == moduleId)
                {
                    //判断菜单节点是否存在
                    var module = FindModule(ConfigContext.Current.PermessionConfig[groupId].Modules, moduleId);
                    var menu = module.Menus.FirstOrDefault(p => p.ID == menuId);
                    if (menu == null)
                    {
                        var menuConfig = FindMenu(ConfigContext.Current.ModuleConfig.Modules, moduleId, menuId);
                        menu = new Menu
                        {
                            ID = menuId,
                            Url = menuConfig.Url,
                            Name = menuConfig.Name
                        };
                        module.Menus.Add(menu);
                    }
                }
            }
        }


        public void SetMenuButton(string groupId, string moduleId, string menuId, string buttonName, bool HasPermession)
        {
            var menu = FindMenu(this[groupId].Modules, moduleId, menuId);
            if (menu == null)
            {
                throw new Exception("请先设置菜单权限");
            }

            var buttons = menu.Buttons;
            var current = buttons.FirstOrDefault(p => p.Name.ToLower() == buttonName.ToLower());
            if (!HasPermession)
            {
                if (current != null)
                {
                    buttons.Remove(current);
                }
            }
            else
            {
                if (current == null)
                {
                    buttons.Add(new Button
                    {
                        Name = buttonName
                    });
                }
            }
        }

        public bool HasModulePermession(string groupId, string moduleId)
        {
            var modules = GetModules(groupId);
            if (modules == null || !modules.Any())
            {
                return false;
            }

            foreach (var module in modules)
            {
                if (module.ID == moduleId)
                {
                    return true;
                }

                if (HasChildModulePermession(module, moduleId))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasChildModulePermession(Module parentModule, string moduleId)
        {
            var modules = parentModule.Children;
            if (modules != null && modules.Any())
            {
                foreach (var current in modules)
                {
                    if (current.ID == moduleId)
                    {
                        return true;
                    }

                    if (HasChildModulePermession(current, moduleId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public bool HasMenuPermession(string groupId, string moduleId, string menuId)
        {
            var modules = GetModules(groupId);
            if (modules == null)
            {
                return false;
            }

            foreach (var module in modules)
            {
                if (module.ID == moduleId)
                {
                    foreach (var menu in module.Menus)
                    {
                        if (menu.ID == menuId)
                        {
                            return true;
                        }
                    }
                }

                if (HasChildMenuPermession(module.Children, menuId))
                {
                    return true;
                }
            }

            return false;
        }



        public bool HasChildMenuPermession(IEnumerable<Module> modules, string menuId)
        {

            if (modules == null || !modules.Any())
            {
                return false;
            }

            foreach (var module in modules)
            {
                foreach (var menu in module.Menus)
                {
                    if (menu.ID == menuId)
                    {
                        return true;
                    }
                }

                if (HasChildMenuPermession(module.Children, menuId))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class Group
    {
        public Group()
        {
            this.Modules = new List<Module>();
        }

        [XmlAttribute]
        public string ID { get; set; }

        [XmlElement(ElementName = "Module")]
        public List<Module> Modules { get; set; }

        public Module this[string moduleId]
        {
            get
            {
                var module = Modules.FirstOrDefault(p => p.ID == moduleId);
                if (module == null)
                {
                    module = new Module
                    {
                        ID = moduleId
                    };
                    Modules.Add(module);
                }

                return module;
            }
        }


    }
}