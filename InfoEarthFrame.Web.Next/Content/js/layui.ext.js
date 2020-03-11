//layui base
(function () {
    function use(modules, callback,checkPermession) {
        var allModules = ['index'];
        if (typeof modules != "undefined") {
            allModules = allModules.concat(modules);
        }

        layui.config({
            base: Config['LayuiPath'] //静态资源所在路径
        }).extend({
            index: 'lib/index' //主入口模块
        }).use(allModules, function () {
            if (typeof callback != "function") {
                throw new Error("layui use has no callback");
            }

            callback();
            if (typeof checkPermession == "undefined")
            {
                checkPermession = true;
            }
            if (checkPermession) {
               // Base.initPermession();
            }
            //layui.autoHeight();
        });
    }

    layui.initModule = use;
})();

//tree
(function ($) {
    var layuiTree = {
        init: function (option) {
            var index = null;
            var defaultOption = {
                elem: null,
                showCheckbox: false,
                defaultExpandAll: false,
                draggable: false,
                highlightCurrent: true,
                defaultExpandedKeys: [0],
                checkOnClickNode: true,
                done: function () {
                    layer.close(index);
                },
                where: {
                },
                headers: {
                    "Authorization": "Bearer " + $.cookie(Config['AccessTokenKey']),
                    "Lang": $.cookie(Config["CurrentLangKey"]) || 'zh-cn'
                },
                response: {   // 对于后台数据重新定义名字
                    statusName: "code",
                    statusCode: 0,
                    dataName: "data"
                }
            }

            option = $.extend(defaultOption, option);
            index = layer.load();
            var obj = layui.eleTree.render(option);
            Base.initPermession(location.href);
            return obj;
        }
    }

    layui.initTree = layuiTree.init;
})(jQuery);

//Grid
(function ($) {
    var layuiGrid = {
        init: function (option) {
            var index = null;
            var defaultOption = {
                page: true
              , response: {
                  statusCode: 0 //重新规定成功的状态码为 200，table 组件默认为 0
              }
              , parseData: function (res) { //将原始数据解析成 table 组件所规定的数据
                  return {
                      "code": res.status, //解析接口状态
                      "msg": res.message, //解析提示文本
                      "count": res.total, //解析数据长度
                      "data": res.rows.item //解析数据列表
                  };
              },
                request: {
                    pageName: 'pageIndex' //页码的参数名称，默认：page
                   , limitName: 'pageSize' //每页数据量的参数名，默认：limit
                },
                done: function () {
                    layer.close(index);
                },
                headers: {
                    "Authorization": "Bearer " + $.cookie(Config['AccessTokenKey']),
                    "Lang": $.cookie(Config["CurrentLangKey"]) || 'zh-cn'
                }
            }

            option = $.extend(defaultOption, option);
            index = layer.load();

            this.option = option;
            var obj = layui.table.render(option);

            Base.initPermession(location.href);
            return obj;
        },
        getCheckedIds: function (id) {
            var checkStatus = layui.table.checkStatus(id);

            var ids = [];
            for (var i = 0, length = checkStatus.data.length; i < length; i++) {
                ids.push(checkStatus.data[i].Id);
            }
            return ids;
        }
    }


    layui.initGird = layuiGrid.init;
    layui.Grid = {
        getCheckedIds: layuiGrid.getCheckedIds
    };
})(jQuery);

//TreeGrid
(function ($) {
    var layuiTreeGrid = {
        init: function (option) {
            var defaultOption = {
                id:null
             , elem: null
             , url:null
             , method: "get"
             , idField: 'id'//必須字段
             , treeId: 'id'//树形id字段名称
             , treeUpId: 'pId'//树形父id字段名称
             , treeShowName: 'name'//以树形式显示的字段
             , heightRemove: [".dHead", 10]//不计算的高度,表格设定的是固定高度，此项不生效
             , height: '100%'
             , isFilter: false
             , iconOpen: false//是否显示图标【默认显示】
             , isOpenDefault: false//节点默认是展开还是折叠【默认展开】
             , loading: true
             , isPage: false
             , parseData: function (res) {//数据加载后回调
                 return res;
             },
             headers: {
                 "Authorization": "Bearer " + $.cookie(Config['AccessTokenKey']),
                 "Lang": $.cookie(Config["CurrentLangKey"]) || 'zh-cn'
             }
             ,onClickRow: function (index, o) {
                
             }
             , onDblClickRow: function (index, o) {
               
             },
             toolbar:null
            }

            option = $.extend(defaultOption, option);
            this.option = option;
            var obj = layui.treeGrid.render(option);
            Base.initPermession(location.href);
            return obj;
        },
        getCheckedIds: function (id) {
            var checkStatus = layui.treeGrid.checkStatus(id)
    
            var ids = [];
            for (var i = 0, length = checkStatus.data.length; i < length; i++) {
                ids.push(checkStatus.data[i].id);
            }
            return ids;

        }
    }


    layui.initTreeGird = layuiTreeGrid.init;
    layui.TreeGird = {
        getCheckedIds: layuiTreeGrid.getCheckedIds
    };
})(jQuery);

//dialog
(function ($) {
    var layuiDlg = {
        init: function (option) {
            var defaultOption = {
                type: 2,
                shadeClose: true,
                shade: 0.5,
                btn: ['确定', '关闭']
            };
            option = $.extend(defaultOption, option);
            layer.open(option);
        }
    }

    layui.showDlg = layuiDlg.init;
})(jQuery);


//form
(function ($) {

    layui.resetForm = function (id) {
        if (!id.indexOf("#") >= 0)
        {
            id = "#" + id;
        }
        $(id + " input").val('');
        $(id + " select").val('');

        layui.form.render();
    }
})(jQuery);


//select
(function ($) {
    var layuiSelect = {
        init: function (option) {
            var defaultOption = {
                valueField: "value",
                textField: "text",
                id: '',
                method: 'get',
                url: '',
                where: {}
            };

            option = $.extend(defaultOption, option);
            var id = option.id;

            //无请求
            if (option.data) {
                renderData(option.data);
                return;
            }

            //ajax
            if (option.url) {
                option.data = option.where;
                option.success = function (resp) {
                    renderData(resp.data);
                    //callback
                    if (option.callback && typeof option.callback == "function") {
                        option.callback(resp);
                    }
                }

                if (option.method == "get") {
                    $.get(option)
                }
                else {
                    $.post(option)
                }
            }

            function renderData(data)
            {
                $(id).html("<option value=''>请选择</option>");
                $.each(data, function (index, ele) {
                    var isSelected = option.selectedValue == ele[option.valueField];
                    $(id).append("<option value='" + ele[option.valueField] + "' "+(isSelected?'selected':"")+">" + ele[option.textField] + "</option>");
                });
                layui.form.render("select");

                layui.form.on("select(" + id.replace("#", "") + ")", function (data) {
             
                    if (option.onSelect && typeof option.onSelect == "function") {
                        option.onSelect(data);
                    }
                });
            }
        }
    }

    layui.initSelect = layuiSelect.init;
})(jQuery);

(function ($) {
    var layuiSelect2 = {
        init: function (option) {
            var defaultOption = {
                id:'',
                type: 'get',                //请求方式: post, get, put, delete...
                header: {
                    "Authorization": "Bearer " + $.cookie(Config['AccessTokenKey']),
                    "Lang": $.cookie(Config["CurrentLangKey"]) || 'zh-cn'
                },                 //自定义请求头
                data: {},                   //自定义除搜索内容外的其他数据
                searchUrl: '',              //搜索地址, 默认使用xm-select-search的值, 此参数优先级高
                searchName: 'keyword',      //自定义搜索内容的key值
                searchVal: '',              //自定义搜索内容, 搜素一次后失效, 优先级高于搜索框中的值
                keyName: 'name',            //自定义返回数据中name的key, 默认 name
                keyVal: 'value',            //自定义返回数据中value的key, 默认 value
                keySel: 'selected',         //自定义返回数据中selected的key, 默认 selected
                keyDis: 'disabled',         //自定义返回数据中disabled的key, 默认 disabled
                keyChildren: 'children',    //联动多选自定义children
                delay: 500,                 //搜索延迟时间, 默认停止输入500ms后开始搜索
                direction: 'auto',          //多选下拉方向, auto|up|down
                response: {
                    statusCode: 0,          //成功状态码
                    statusName: 'code',     //code key
                    msgName: 'message',         //msg key
                    dataName: 'data'        //data key
                },
                success: function (id, url, searchVal, result) {      //使用远程方式的success回调

                },
                error: function (id, url, searchVal, err) {           //使用远程方式的error回调

                },
                beforeSuccess: function (id, url, searchVal, result) {        //success之前的回调, 干嘛呢? 处理数据的, 如果后台不想修改数据, 你也不想修改源码, 那就用这种方式处理下数据结构吧
                    return result;  //必须return一个结果, 这个结果要符合对应的数据结构
                },
                beforeSearch: function (id, url, searchVal) {         //搜索前调用此方法, return true将触发搜索, 否则不触发
                    //if (!searchVal) {//如果搜索内容为空,就不触发搜索
                    //    return false;
                    //}
                    return true;
                },
                clearInput: false,          //当有搜索内容时, 点击选项是否清空搜索内容, 默认不清空
            };
            option = $.extend(defaultOption, option);
            if (option.method)
            {
                option.type = option.method;
            }
            if (option.url) {
                option.searchUrl = option.url;
            }
            var id = option.id;


            layui.formSelects.config(id, option, false);


            layui.formSelects.on(id, function (id, vals, val, isAdd, isDisabled) {
                if (typeof option.onSelect == "function")
                {
                   return option.onSelect(id, vals, val, isAdd, isDisabled);
                }
                return true;
            });
        }
    }

    layui.initSelect2 = layuiSelect2.init;
})(jQuery);


//uploader
(function ($) {
    var layuiUploader = {
        init: function (option) {
            var defaultOption ={
                listView:"#fileList"
                   , elem: null
                  , url: null
                  , exts: 'zip|rar' //只允许上传压缩文件
                  , accept: 'file'
                  , multiple: true
                  , auto: false
                  , bindAction: '#btnUpload'
                  , choose: function (obj) {
                      var files = this.files = obj.pushFile(); //将每次选择的文件追加到文件队列

                      //读取本地文件
                      obj.preview(function (index, file, result) {
                          var tr = $(['<tr id="upload-' + index + '">'
                            , '<td>' + file.name + '</td>'
                            , '<td>' + (file.size / 1024).toFixed(1) + 'kb</td>'
                            , '<td><a class="layui-btn  layui-btn-primary layui-btn-xs">等待上传</a></td>'
                            , '<td>'
                              , '<button class="layui-btn layui-btn-sm upload-reload layui-hide"><i class="layui-icon">&#xe681;</i>重传</button>'
                              , '<button class="layui-btn layui-btn-sm layui-btn-danger upload-delete"><i class="layui-icon"></i>删除</button>'
                            , '</td>'
                          , '</tr>'].join(''));

                          //单个重传
                          tr.find('.upload-reload').on('click', function () {
                              obj.upload(index, file);
                          });

                          //删除
                          tr.find('.upload-delete').on('click', function () {
                              delete files[index]; //删除对应的文件
                              tr.remove();
                              console.log(uploader);
                              uploader.config.elem.next()[0].value = ''; //清空 input file 值，以免删除后出现同名文件不可选
                          });

                          $(option.listView).append(tr);
                      });
                  }
                  , done: function (res, index, upload) {
                      layer.closeAll("loading");
                      if (res.code == 0) { //上传成功
                          var tr = $(option.listView).find('tr#upload-' + index)
                          , tds = tr.children();
                          tds.eq(2).html('<a class="layui-btn  layui-btn-xs">已上传</a>');
                          tds.eq(3).html(''); //清空操作
                          if (typeof option.okCallback == "function")
                          {
                              option.okCallback(res, index, upload);
                          }
                          return delete this.files[index]; //删除文件队列已经上传成功的文件
                      }
                      this.error(index, upload, res);
                  }
                  , error: function (index, upload, res) {
                      layer.closeAll("loading");
                      var tr = $(option.listView).find('tr#upload-' + index)
                      , tds = tr.children();
                      var s = '<a class="layui-btn layui-btn-danger layui-btn-xs">上传失败</a>';
                      if (typeof option.failureCallback == "function") {
                          s += "<button class=\"layui-btn layui-btn-normal layui-btn-xs\">查看</button>"
                      }
                      tds.eq(2).html(s);
                      tds.eq(2).find("button:eq(0)").click(function () {
                          option.failureCallback(res, index, upload);
                      })
                      tds.eq(3).find('.upload-reload').removeClass('layui-hide'); //显示重传
                  },
                  headers: {
                      "Authorization": "Bearer " + $.cookie(Config['AccessTokenKey']),
                      "Lang": $.cookie(Config["CurrentLangKey"]) || 'zh-cn'
                  }
                }

            option = $.extend(defaultOption, option);
            return   layui.upload.render(option);
        }
    }

    layui.initUploader = layuiUploader.init;
})(jQuery);

//globel
(function ($) {
    //单击行勾选checkbox事件
    $(document).on("click", ".layui-table-body table.layui-table tbody tr", function () {
        var index = $(this).attr('data-index');
        var tableBox = $(this).parents('.layui-table-box');
        //存在固定列
        if (tableBox.find(".layui-table-fixed.layui-table-fixed-l").length > 0) {
            tableDiv = tableBox.find(".layui-table-fixed.layui-table-fixed-l");
        } else {
            tableDiv = tableBox.find(".layui-table-body.layui-table-main");
        }
        var checkCell = tableDiv.find("tr[data-index=" + index + "]").find("td div.laytable-cell-checkbox div.layui-form-checkbox I");
        if (checkCell.length > 0) {
            checkCell.click();
        }
    });

    $(document).on("click", "td div.laytable-cell-checkbox div.layui-form-checkbox", function (e) {
        e.stopPropagation();
    });
})(jQuery);

layui.autoHeight = function () {
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.iframeAuto(index);
}