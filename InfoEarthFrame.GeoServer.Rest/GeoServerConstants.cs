using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.GeoServerRest
{
    public class GeoServerConstants
    {
        public static readonly string Workspace = "[WORKSPACE]";
        public static readonly string Layers = "[LAYERS]";
        public static readonly string Styles = "[STYLES]";
        public static readonly string ServerIp = "[SERVER]";
        public static readonly string ServerPort = "[PORT]";
        public static readonly string Format = "[FORMAT]";
        public static readonly string BaseUrl = "http://" + ServerIp + ":" + ServerPort + "/geoserver";
        public static readonly string RestUrl = BaseUrl + "/rest";
    }

    public enum RequestMethod
    {
        Delete,
        Get,
        Post,
        Put
    }

    public enum ContentType
    {
        Html,
        Json,
        Xml,
        SldFile,
        Zip
    }

    public enum AcceptType
    {
        Html,
        Json,
        Xml,
        Byte
    }

    public enum SrsType
    {
        Epsg4326,
        EPSG404000
    }

    public enum TaskState
    {
        Running,
        Pending,
        All
    }
}
