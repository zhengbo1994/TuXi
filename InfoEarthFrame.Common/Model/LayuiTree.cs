using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoEarthFrame.Common
{
    public class LayuiTreeItem
    {
        public LayuiTreeItem()
        {
            this.children = new List<LayuiTreeItem>();
        }
        public string id { get; set; }

        public string label { get; set; }

        public List<LayuiTreeItem> children { get; set; }
    }

    public class DTreeItem
    {
        public string id { get; set; }

        public string title { get; set; }

        public int level { get; set; }

        public string parentId { get; set; }
        public List<DTreeItem> children { get; set; }

        public DTreeItemCheckProp checkArr { get; set; }

        public bool spread { get; set; }

        public object tag { get; set; }
       
    }


    public class DTreeItemCheckProp
    {
        public string type { get; set; }

        public string isChecked { get; set; }
    }

    public class ZTreeItem
    {
        public string id { get; set; }

        public string pId { get; set; }

        public string name { get; set; }

        public bool open { get; set; }

        public List<ZTreeItem> children { get; set; }

        public bool isParent { get; set; }

        public object tag { get; set; }
        public object tag1 { get; set; }

        public object tag2 { get; set; }


        public object tag3 { get; set; }

        public object tag4 { get; set; }
    }
}