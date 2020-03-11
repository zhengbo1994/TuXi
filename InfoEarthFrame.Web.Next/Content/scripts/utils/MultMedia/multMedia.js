/// <reference path="../../jquery/jquery-1.10.2.min.js" />
var modeId_Question = "fddeed1c-d4f8-438a-98be-28cbca206671";//矿山问题
var modeId_BaseInfo = "fddeed1c-d4f8-438a-98be-28cbca206671";//矿山基本信息
var modeId_Avalanche = "b33b8ef2-cdb8-5e3a-951b-f73125ba1c13";//崩塌
var modeId_Landslip = "13b9c8c0-039c-bfe7-f631-b97dda76c56d";//滑坡
var modeId_Debrisflow = "7EA5B92C-7D80-49E4-84E6-9634D7C268F5";//泥石流
var modeId_CollapseandLandcrack = "2419D71A-98DA-4351-AA5C-C1CF5F6F9B96";//地面塌陷,地裂缝
var modeId_AquiferInfo = "35E9271A-98DA-4351-AA5C-C1CF5F6F9B96";//地下含水层影响破坏
var modeId_TerrainInfo = "7F18E51B-98DA-4351-AA5C-C1CF5F6F9B96";//地形地貌景观破坏
var modeId_WasteInfo = "6EB1831B-98DA-4351-AA5C-C1CF5F6F9B96";//废水废液固液

var modeId_WasteGather = "90E2F8E2-98DA-4351-AA5C-C1CF5F6F9B96";//水质样品
var modeId_SoilGather = "2419D71A-98DA-4351-AA5C-C1CF5F6F9B96";//土壤样品

var modeId_KJInfo = "83C7F5B8-5C73-48cc-9B90-449D947FD98F";//矿井
var modeId_ZLGCInfo = "7761ab3a-1cda-4fbb-3923-c87399c17626";//治理工程
var modeId_CKQInfo = "78DC9B1A-870D-47F4-B7EB-083212890E4E";//采空区
var deleteFiles = [];
var addFiles = [];
var existFiles = [];
var belongObjectGuid = "";
Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
    }
    return -1;
}

Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
}
//初始化多媒体页
function cfInitMultiMedia(mediaId, modeId, belongId) {
    var height = $(window).height() - 60;
    var url = "/SystemManage/MultMedia/Index?SaveIsHidden=true&moduleID=" + modeId + "&belongObjectGuid=" + belongId;
    $("#" + mediaId).html("<iframe frameborder='0' id ='frmMultimedia' scrolling='no' src='" + url + "'style='width:100%;height:" + height + "px;'><iframe>");
}

////////////////////////////////////////////////////////////////////////照片记录 遥感影像记录

var moduledClassZP = "790C8F14-DB5A-4706-9ED7-E2278BDF5BF7";
var moduledClassYGXY = "790C8F14-DB5A-4706-9ED7-E2278BDF5BF8";
var moduledClassPouM = "47EAFD4B-7832-403D-B201-7FC8194E6547";//剖面
var moduledClassPingM = "4C2BF8BD-FD31-46C8-A6BA-8704582C5D7B";//平面
function cfUPfile(divId, moduledClassID) {
    dialogOpen({
        id: "FileSelectForm",
        title: '附件列表',
        url: '/SystemManage/MultMedia/FileSelectForm?folderId=' + moduledClassID,
        width: "800px",
        height: "600px",
        isClose: true,
        callBack: function (iframeId) {
            var rowData = getInfoTop().frames[iframeId].selectFileData;
            if (rowData.length) {
                for (var i = 0; i < rowData.length; i++) {
                    var fileGuid = rowData[i].GUID;
                    var fileName = rowData[i].FILENAME;
                    if (moduledClassID != null) {
                        var nid = moduledClassID + "#" + fileGuid;
                        var index = addFiles.indexOf(nid);
                        var index1 = existFiles.indexOf(nid);
                        if (index <= -1 && index1 <= -1) {
                            addFiles.push(nid);
                            var html = getUPHTML(fileGuid, fileName, moduledClassID);
                            $("#" + divId).append(html);
                        }
                    }
                }
            }
            return true;
        }
    });
}
function getUPHTML(fileGuid, fileName, moduledClassID, businessID) {
    var html = "";
    if (businessID) {
        html = " <div style=\"float:left;margin-left:10px;margin-top:3px;\"> <div style=\"float:left;cursor:pointer;\" onclick=\"ShowPIC('" + fileGuid + "','" + fileName + "')\">" + fileName + "</div><div class=\"div_close\" value='" + fileGuid + "'  moduledClassID='" + moduledClassID + "' businessID='" + businessID + "'  onclick='delFile(this)'></div></div>";
    }
    else {
        html = " <div style=\"float:left;margin-left:10px;margin-top:3px;\"> <div style=\"float:left;cursor:pointer;\" onclick=\"ShowPIC('" + fileGuid + "','" + fileName + "')\">" + fileName + "</div><div class=\"div_close\" value='" + fileGuid + "'  moduledClassID='" + moduledClassID + "'  onclick='delFile(this)'></div></div>";
    }
    return html;
}
function delFile(obj) {
    var fileGuid = $(obj).attr("value");
    var moduledClassID = $(obj).attr("moduledClassID");
    var node = moduledClassID + "#" + fileGuid;
    var businessID = $(obj).attr("businessID");
    if (businessID) {
        deleteFiles.push(businessID);
    }
    addFiles.remove(node);
    $(obj).parent().remove();

}
function showMedia(divId, ClassID, moduleID, belongGuid) {
    if ($("#" + divId).length = 0) {
        return;
    }
    $.ajax({
        url: "/SystemManage/MultMedia/FindMedInfoListAndClassID",
        async: false,
        type: "POST",
        data: {
            moduleID: moduleID,
            belongObjectGuid: belongGuid,
            ClassID: ClassID
        },
        success: function (data) {
            $("#" + divId).empty();
            var dataList = JSON.parse(data);
            for (var i = 0; i < dataList.length; i++) {
                var html = getUPHTML(dataList[i].FILEINFOGUID, dataList[i].FILENAME, ClassID, dataList[i].BUSSNISSFILEINFOGUID);
                $("#" + divId).append(html);
            }
        }, error: function (e) {
        },
        cache: false
    });
}
function ShowPIC(Guid, fileName) {
    dialogOpen({
        id: 'FileSelectForm',
        title: '预览',
        url: '/SystemManage/MultMedia/Show?fileGuid=' + escape(Guid) + '&fileName=' + escape(fileName),
        width: "90%",
        height: "100%",
        btn: null
    });
}
function cfupfile(id) {
    if (id == "divZPJL") {
        cfUPfile(id, moduledClassZP);
    }
    else if (id == "divYGYXJL") {
        cfUPfile(id, moduledClassYGXY);
    }
    else if (id == "divPoumian") {
        cfUPfile(id, moduledClassPouM);
    }
    else if (id == "divPingmian") {
        cfUPfile(id, moduledClassPingM);
    }
}
//删除关联多媒体信息
function cfRemoveMediaInfo(moduleID, belongGuid, parmData) {
    if (parmData) {
        $.ajax({
            url: '../../MineBaseManage/TBL_MINE_BASEINFO/RemoveMediaInfo',
            data: parmData,
            type: "post",
            async: false,
            success: function (data) {

            }, error: function (e) {
            },
            cache: false
        })
    } else {
        $.ajax({
            url: "../../SystemManage/MultMedia/RemoveMediaInfo",
            data: { "moduleID": moduleID, "belongObjectGuid": belongGuid },
            type: "post",
            async: false,
            success: function (data) {

            }, error: function (e) {
            },
            cache: false
        })
    }

}

function SaveFileInfo(belongGuid) {
    if (belongGuid != "" && belongGuid != undefined) {
        belongObjectGuid = belongGuid;
    }
    var addStr = "";
    var deleteStr = "";
    for (var i = 0; i < addFiles.length; i++) {
        addStr += addFiles[i] + ",";
    }

    for (var i = 0; i < deleteFiles.length; i++) {
        deleteStr += deleteFiles[i] + ",";
    }

    var queryUrl = "/SystemManage/MultMedia/UpdateMediaInfo";
    $.ajax({
        url: queryUrl,
        async: true,
        type: "POST",
        data: {
            addData: addStr,
            deleteData: deleteStr,
            moduleID: moduleID,
            belongObjectGuid: belongObjectGuid
        },
        success: function (data) {

        }, error: function (e) {
        },
        cache: false
    });
};
