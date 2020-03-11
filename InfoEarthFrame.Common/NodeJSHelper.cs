using SocketIOClient;
using SocketIOClient.Messages;
using Newtonsoft.Json;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace InfoEarthFrame.Common
{
    public class NodeJSHelper
    {
        Client _client;
        string _url = ConfigurationManager.AppSettings["NodeJSSocketServer"];

        public NodeJSHelper()
        {
            _client = new Client(_url);
        }

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="groupName">消息组名称</param>
        /// <param name="jsonMsgContent">JSON的消息内容</param>
        /// <returns></returns>
        public bool SendMsg(string groupName,string userName, string jsonMsgContent)
        {
            try
            {
                string group = "/" + groupName;
                _client.Connect(group);

                _client.On("connect", (fn) =>
                {
                    //发消息
                    _client.Emit("send", new MessageJSON(group,userName,jsonMsgContent).ToJsonString());
                    //_client.Emit("send","tesrer");//{groupName:group,userName:userName,msgContent:jsonMsgContent}
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string ReceiveMsg(string groupName, string userName)
        {
            string result = "";
            try
            {
                string group = "/" + groupName;
                _client.Connect(group);
                _client.Emit("subscribe", userName, group);

                ////接收消息
                _client.On("receive", group, (data) =>
                {
                    object obj = data.Json.Args[0];
                    result = obj.ToString();
                });

                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 消息发送的json对象
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class MessageJSON
        {

            [JsonProperty]
            string groupName;
            [JsonProperty]
            string userName;
            [JsonProperty]
            string msgContent;

            public MessageJSON(string group, string user, string content)
            {
                this.groupName = group;
                this.userName = user;
                this.msgContent = content;
            }
            public string ToJsonString()
            {
                return JsonConvert.SerializeObject(this);
            }
            public static MessageJSON Deserialize(string jsonString)
            {
                return JsonConvert.DeserializeObject<MessageJSON>(jsonString);
            }
        }
    }
}