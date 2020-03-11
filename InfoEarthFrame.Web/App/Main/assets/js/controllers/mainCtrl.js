'use strict';
/**
 * Clip-Two Main Controller
 */
app.controller('AppCtrl', ['$rootScope', '$scope', '$state', '$translate', '$localStorage', '$window', '$document', '$timeout', 'cfpLoadingBar', '$location', 'abp.services.app.systemUser', 'abp.services.app.dataType', 'abp.services.app.dataTag', 'abp.services.app.dataStyle', 'abp.services.app.layerContent', 'waitmask', 'abp.services.app.layerField', 'abp.services.app.dicDataCode', 'abp.services.app.setSys', '$filter',
    function ($rootScope, $scope, $state, $translate, $localStorage, $window, $document, $timeout, cfpLoadingBar, $location, systemUser, dataType, dataTag, dataStyle, layerContent, waitmask, layerField, dicDataCode, setSys, $filter) {

        $rootScope.loginOut();

        // Loading bar transition
        // -----------------------------------
        var $win = $($window);

        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            //start loading bar on stateChangeStart
            cfpLoadingBar.start();

        });
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {

            //stop loading bar on stateChangeSuccess
            event.targetScope.$watch("$viewContentLoaded", function () {

                cfpLoadingBar.complete();
            });

            // scroll top the page on change state

            $document.scrollTo(0, 0);

            if (angular.element('.email-reader').length) {
                angular.element('.email-reader').animate({
                    scrollTop: 0
                }, 0);
            }

            // Save the route title
            $rootScope.currTitle = $state.current.title;
        });

        // State not found
        $rootScope.$on('$stateNotFound', function (event, unfoundState, fromState, fromParams) {
            //$rootScope.loading = false;
            console.log(unfoundState.to);
            //console.log(98989);
            // "lazy.state"
            console.log(unfoundState.toParams);
            // {a:1, b:2}
            console.log(unfoundState.options);
            // {inherit:false} + default options
        });

        $rootScope.pageTitle = function () {
            //return $filter('translate')('dashboard.MAIN');
            //return $rootScope.app.name + ' - ' + ($rootScope.currTitle || $rootScope.app.description);
        };

        // save settings to local storage
        if (angular.isDefined($localStorage.layout)) {
            $scope.app.layout = $localStorage.layout;

        } else {
            $localStorage.layout = $scope.app.layout;
        }
        $scope.$watch('app.layout', function () {
            // save to local storage
            $localStorage.layout = $scope.app.layout;
        }, true);

        //global function to scroll page up
        $scope.toTheTop = function () {

            $document.scrollTopAnimated(0, 600);

        };

        // angular translate
        // ----------------------

        $scope.language = {
            // Handles language dropdown
            listIsOpen: false,
            // list of available languages
            available: {
                'en': 'English',
                'it_IT': 'Italiano',
                'de_DE': 'Deutsch',
                'cn': 'Chinese'
            },
            // display always the current ui language
            init: function () {
                var proposedLanguage = $translate.proposedLanguage() || $translate.use();
                var preferredLanguage = $translate.preferredLanguage();
                // we know we have set a preferred one in app.config
                $scope.language.selected = $scope.language.available[(proposedLanguage || preferredLanguage)];
            },
            set: function (localeId, ev) {
                $translate.use(localeId);
                $scope.language.selected = $scope.language.available[localeId];
                $scope.language.listIsOpen = !$scope.language.listIsOpen;
            }
        };

        //$scope.language.init();

        if (language_English == '1') {
            $rootScope.showLanguage = false;
            $scope.language.set('en');            
        }
        else {
            $rootScope.showLanguage = true;
            $scope.language.set('cn');            
        }

        // Function that find the exact height and width of the viewport in a cross-browser way
        var viewport = function () {
            var e = window, a = 'inner';
            if (!('innerWidth' in window)) {
                a = 'client';
                e = document.documentElement || document.body;
            }
            return {
                width: e[a + 'Width'],
                height: e[a + 'Height']
            };
        };
        // function that adds information in a scope of the height and width of the page
        $scope.getWindowDimensions = function () {
            return {
                'h': viewport().height,
                'w': viewport().width
            };
        };
        // Detect when window is resized and set some variables
        $scope.$watch($scope.getWindowDimensions, function (newValue, oldValue) {
            $scope.windowHeight = newValue.h;
            $scope.windowWidth = newValue.w;

            //屏幕分辨率大于1024时，菜单默认展开，小于1024时，菜单默认收缩
            if (newValue.w > 1024) {
                $rootScope.app.layout.isSidebarClosed = false;
            } else {
                $rootScope.app.layout.isSidebarClosed = true;
            }

            if (newValue.w >= 992) {
                $scope.isLargeDevice = true;
            } else {
                $scope.isLargeDevice = false;
            }
            if (newValue.w < 992) {
                $scope.isSmallDevice = true;
            } else {
                $scope.isSmallDevice = false;
            }
            if (newValue.w <= 768) {
                $scope.isMobileDevice = true;
            } else {
                $scope.isMobileDevice = false;
            }
        }, true);
        // Apply on resize
        $win.on('resize', function () {
            $scope.$apply();
        });
        /*************************************导航联动效果-start************************************************/
        $rootScope.navUrl1 = '#/app/homePage';//导航一的跳转地址
        //$rootScope.navUrl2 = '#/appPages/typicalMainPage';//导航二的跳转地址
        $rootScope.tplUrl = 'App/Main/assets/views/partials/nav.html';//默认菜单模板//导航一

        //解决刷新问题
        //if ($location.$$path.indexOf('appPages') > -1) {
        //    $rootScope.tplUrl = 'App/Main/assets/views/partials/nav1.html';//导航二
        //}

        //$rootScope.navChange = function (a) {
        //    if (a) {
        //        $rootScope.tplUrl = 'App/Main/assets/views/partials/nav.html';
        //        window.location.href = $rootScope.navUrl1;
        //    } else {
        //        $rootScope.tplUrl = 'App/Main/assets/views/partials/nav1.html';
        //        window.location.href = $rootScope.navUrl2;
        //        console.log(window.location.href);
        //    }
        //};
        /************************************导航联动效果-end*************************************************/



        /************************************右上角用户按钮-start*************************************************/

        //加载页面时获取登录用户信息
        if (decodeURI(localStorage.getItem('infoearth_spacedata_userCode')) === "admin") {
            $rootScope.loginUserCode = "admin";
            $rootScope.isAdmin = true;
        }
        else {
            $rootScope.loginOut();
            systemUser.getAllListByName({ UserName: decodeURI(localStorage.getItem('infoearth_spacedata_userCode')) }, 1, 1).success(function (data, status) {
                //console.log(data, status);
                $rootScope.isAdmin = false;
                $rootScope.personMenuModal.id = data.items[0].id;
                $rootScope.personMenuModal.userCode = data.items[0].userCode;
                $rootScope.personMenuModal.password = data.items[0].password;
                $rootScope.personMenuModal.userName = data.items[0].userName;
                $rootScope.personMenuModal.telPhone = data.items[0].telPhone;
                $rootScope.personMenuModal.phone = data.items[0].phone;
                $rootScope.personMenuModal.department = data.items[0].department;
                $rootScope.personMenuModal.position = data.items[0].position;
                $rootScope.personMenuModal.sex = data.items[0].userSex;
                $rootScope.personMenuModal.remark = data.items[0].remark;


                $rootScope.loginUserCode = angular.copy($rootScope.personMenuModal.userCode);
            }).error(function (data, status) {
                console.log(data, status);
            });
        }

        //展开用户头像的下拉
        $rootScope.dropdownUser = function () {
            $rootScope.isShowUserMenu = !$rootScope.isShowUserMenu;
            if (!!$rootScope.isShowUserMenu) {
                $rootScope.isShowLanguageMenu = false;
            }
        }

        $rootScope.dropdownLanguage = function () {
            $rootScope.isShowLanguageMenu = !$rootScope.isShowLanguageMenu;
            if (!!$rootScope.isShowLanguageMenu) {
                $rootScope.isShowUserMenu = false;
            }
        }
        $rootScope.changeToChinese = function () {
            $scope.language.set('cn');
            $timeout(function () {
                waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                $scope.$broadcast("LanguageChange");
                setConfig('Language', 'CN', function () {
                    $window.location.reload();
                }, "切换语言错误！");
            }, 100);
        }
        $rootScope.changeToEnglish = function () {
            $scope.language.set('en');
            $timeout(function () {
                waitmask.onShowMask($filter('translate')('setting.waitText'), 200);
                $scope.$broadcast("LanguageChange");
                setConfig('Language', 'ENGLISH', function () {
                    $window.location.reload();
                }, "切换语言错误！");
            }, 100);
        }

        //设置参数
        function setConfig(configName, value, successFun, errTitle) {
            setSys.updateAppSetting(configName, value).success(function (data, status) {
                console.log(data);
                successFun() || function () { };
            }).error(function (data, status) {
                console.log(data);
                waitmask.onHideMask();
                $rootScope.alertFun(!!errTitle ? errTitle : $filter('translate')('views.System.alertFun.error'), "", 'warning', '#007AFF');
            });
        }

        //打开个人中心弹窗
        $rootScope.openPerCenterMenu = function () {
            $rootScope.isShowUserMenu = false;
            $rootScope.personMenuModal.active1 = true;
            $rootScope.personMenuModal.active2 = false;
            $rootScope.personMenuModal.title = $filter('translate')('topbar.user.userCenter.MAIN');

            $rootScope.personMenuModal.writeList.userName = $rootScope.personMenuModal.userName;
            $rootScope.personMenuModal.writeList.telPhone = $rootScope.personMenuModal.telPhone;
            $rootScope.personMenuModal.writeList.phone = $rootScope.personMenuModal.phone;
            $rootScope.personMenuModal.writeList.department = $rootScope.personMenuModal.department;
            $rootScope.personMenuModal.writeList.position = $rootScope.personMenuModal.position;
            $rootScope.personMenuModal.writeList.sex = $rootScope.personMenuModal.sex;
            $rootScope.personMenuModal.writeList.remark = $rootScope.personMenuModal.remark;

            //console.log($rootScope.personMenuModal);

            $timeout(function () {
                $rootScope.personMenuModal.openPerMenu();
            }, 100);
        }

        //个人中心弹窗model
        $rootScope.personMenuModal = {
            title: "",
            id: '',
            userCode: '',
            password: '',
            userName: '',
            telPhone: '',
            phone: '',
            department: '',
            position: '',
            sex: '',
            remark: '',

            writeList: {
                userName: '',
                telPhone: '',
                phone: '',
                department: '',
                position: '',
                sex: '',
                remark: ''
            },

            oldPw: '',
            newPw: '',
            reNewPw: '',

            active1: true,
            active2: false,
            select: function (val) {
                if (val === 1 && !$rootScope.personMenuModal.active1) {
                    $rootScope.personMenuModal.active1 = true;
                    $rootScope.personMenuModal.active2 = false;
                }
                if (val === 2 && !$rootScope.personMenuModal.active2) {
                    $rootScope.personMenuModal.active1 = false;
                    $rootScope.personMenuModal.active2 = true;
                }
            }
        }

        //提交数据操作
        $rootScope.submitPersonMenu = function (modalInstance, form) {
            var dto = {};
            dto.Id = angular.copy($rootScope.personMenuModal.id);
            dto.UserCode = angular.copy($rootScope.personMenuModal.userCode);

            if (!$rootScope.personMenuModal.writeList.userName) {
                abp.message.warning('', $filter('translate')('topbar.user.userCenter.inputName'));
                //alert('请填写姓名!');
                return;
            }
            if (!!$scope.personMenuModal.newPw || !!$rootScope.personMenuModal.oldPw) {
                //填写了密码
                if (!$scope.personMenuModal.newPw) {
                    abp.message.warning('', $filter('translate')('topbar.user.userCenter.inputNew'));
                    //alert('请填写新密码!');
                    return;
                }
                if (!$scope.personMenuModal.reNewPw) {
                    abp.message.warning('', $filter('translate')('topbar.user.userCenter.inputCon'));
                    //alert('请确认密码!');
                    return;
                }
                if ($rootScope.personMenuModal.password !== $rootScope.personMenuModal.oldPw) {
                    abp.message.warning('', $filter('translate')('topbar.user.userCenter.oldErr'));
                    //alert('原密码错误!');
                    return;
                }
                if ($rootScope.personMenuModal.newPw !== $rootScope.personMenuModal.reNewPw) {
                    abp.message.warning('', $filter('translate')('topbar.user.userCenter.newDeCon'));
                    //alert('输入密码不匹配!');
                    return;
                }
                dto.Password = angular.copy($rootScope.personMenuModal.newPw);
            }
            else {
                //没填密码
                dto.Password = angular.copy($rootScope.personMenuModal.password);
            }

            dto.UserName = angular.copy($rootScope.personMenuModal.writeList.userName);
            dto.UserSex = angular.copy($rootScope.personMenuModal.writeList.sex);
            dto.TelPhone = angular.copy($rootScope.personMenuModal.writeList.telPhone);
            dto.Phone = angular.copy($rootScope.personMenuModal.writeList.phone);
            dto.Department = angular.copy($rootScope.personMenuModal.writeList.department);
            dto.Position = angular.copy($rootScope.personMenuModal.writeList.position);
            dto.Remark = angular.copy($rootScope.personMenuModal.writeList.remark);

            //console.log(dto);
            //编辑
            systemUser.update(dto).success(function (data, status) {
                //console.log(data, status);
                if ($rootScope.personMenuModal.active2) {
                    //localStorage.setItem('infoearth_spacedata_userCode', data.userCode);
                    localStorage.setItem('infoearth_spacedata_password', data.password);

                    $rootScope.personMenuModal.password = data.password;
                    abp.message.successing("", $filter('translate')('topbar.user.userCenter.editSucc'));

                    //$rootScope.loginOut();
                }
                else {
                    $rootScope.personMenuModal.userName = data.userName;
                    $rootScope.personMenuModal.telPhone = data.telPhone;
                    $rootScope.personMenuModal.phone = data.phone;
                    $rootScope.personMenuModal.department = data.department;
                    $rootScope.personMenuModal.position = data.position;
                    $rootScope.personMenuModal.sex = data.userSex;
                    $rootScope.personMenuModal.remark = data.remark;
                    //console.log($rootScope.personMenuModal);
                    abp.message.successing("", $filter('translate')('topbar.user.userCenter.editSucc'));
                }

                modalInstance.close();
            }).error(function (data, status) {
                abp.message.erroring("", $filter('translate')('topbar.user.userCenter.editErr'));
                console.log(data, status);
            });
        }

        //退出登录
        $rootScope.signOutSys = function () {
            $rootScope.isShowUserMenu = false;
            abp.message.confirm($filter('translate')('topbar.user.signout.MAIN'), $filter('translate')('topbar.user.signout.confirmOut'), function (a) {
                if (a) {
                    //console.log(localStorage.getItem('infoearth_spacedata_remUserStatus'));

                    localStorage.setItem('infoearth_spacedata_userCode', '');
                    $rootScope.loginOut();
                }
            });
        }

        /************************************右上角用户按钮-end*************************************************/



        /************************************样式管理多域调用的公共部分------start*************************************************/
        //初始化
        var dataTypeID = 'c755eeea-986d-11e7-90b1-005056bb1c7e';
        //图层的标签、分类的TYPE
        var layerTypeID = "a2faae61-6acd-11e7-87f3-005056bb1c7e";
        //图层类型查询的ID
        var layerDataTypeID = "1cfe51dd-67a3-11e7-8eb2-005056bb1c7e";
        var linearComData = [{ value: 1, name: 'line' }, { value: 2, name: 'no-line' }, { value: 3, name: 'point' }, { value: 4, name: 'point-line' }];//线型下拉
        var pointGraphicalComData = [{ value: 'triangle', name: '三角形' }, { value: 'circle', name: '圆形' }, { value: 'square', name: '方形' }];

        var isClickAble = true;
        var initRgb = 'rgb(17, 85, 204)';

        //重置弹框数据
        function resetModalData(obj) {

            console.log(obj);

            //单一样式 之  面    
            obj.isPlaneFrame = true,//边框
                obj.frameRgbValue = initRgb,//边框----线颜色
                obj.planeLinearSelect = { selected: angular.copy(linearComData[0]) },//边框----线型
                obj.planeFrameDiaphaneity = 1,//边框----透明度
                obj.planeLinewidth = 1,//边框----线宽度
                obj.isPlaneColor = true,//填充----颜色
                obj.planeRgbValue = initRgb,//填充----填充色
                obj.planeFillDiaphaneity = 1,//填充----透明度
                obj.isPlaneFillPicture = false,//是否填充图片
                obj.selectedPlaneImgInfo = { path: '' },//填充面的图片信息 包含 图片地址和名称
                obj.planeImgSize = null,//图片大小
                obj.planeImgRotation = null,//图片旋转角度
                //单一样式 之  线   
                obj.lineRgbValue = initRgb,//线颜色
                obj.lineLinearSelect = { selected: angular.copy(linearComData[0]) },//线型-----
                obj.lineDiaphaneity = 1,//透明度
                obj.lineLinewidth = 1,//线宽度
                //单一样式 之  点
                obj.pointRgbValue = initRgb,//填充色
                obj.pointGraphicalSelect = { selected: angular.copy(pointGraphicalComData[0]) },//图型
                obj.pointSize = 8,
                obj.pointDiaphaneity = 1,//透明度
                obj.isStylePointChecked1 = true,//图片与颜色切换 ,  颜色
                obj.isStylePointChecked2 = false,//图片与颜色切换 ,  图片
                obj.selectedPointImgInfo = { path: '' },//图标信息 包含 图片地址和名称
                obj.pointImgSize = null,//图片大小
                obj.pointImgRotation = null;//图片旋转角度
        }
        //编辑时赋值弹框数据
        function toSetModalData(myobj, info) {
            console.log(info);

            //console.log('Debug: styleDataType: styleTypeSelect.selected ', $rootScope.addStyleModol.styleTypeSelect.selected, '   info :   ', info);
            var styleDataType = $rootScope.addStyleModol.styleTypeSelect.selected.codeName;
            // 分类： 点  线  面 
            if (styleDataType === '点' || styleDataType === 'Point') {
                myobj.isStylePointChecked2 = info.isIcon;
                if (myobj.isStylePointChecked2) {
                    myobj.selectedPointImgInfo.path = info.iconPath ? info.iconPath : '',
                        myobj.pointImgSize = info.iconSize,
                        myobj.pointImgRotation = info.iconRotation;
                } else {

                    for (var i in pointGraphicalComData) {
                        if (pointGraphicalComData[i].value == info.pointType) {
                            myobj.pointGraphicalSelect.selected = pointGraphicalComData[i];//图形
                            break;
                        }
                    }

                    myobj.isStylePointChecked1 = info.isFill;//是否填充
                    myobj.pointRgbValue = info.polygonColor;//填充颜色
                    myobj.pointDiaphaneity = info.fillTransparency ? info.fillTransparency : 1;//填充透明度
                    myobj.pointSize = info.iconSize ? info.iconSize : 8;//图片大小
                }

                /*
                  public bool IsIcon = false;
                  public string IconPath;
                  public string IconSize;
                  public string IconRotation;
                
                */
            } else if (styleDataType === '线' || styleDataType === 'Line') {

                for (var i in pointGraphicalComData) {
                    if (linearComData[i].value == info.lineType) {
                        myobj.lineLinearSelect.selected.value = linearComData[i];//线型
                        break;
                    }
                }

                myobj.lineLinewidth = info.lineWidth ? info.lineWidth : 1,//线宽度
                    myobj.lineRgbValue = info.outlineColor,//线条颜色
                    myobj.lineDiaphaneity = info.lineTransparency ? info.lineTransparency : 1;//线条透明度

            } else if (styleDataType === '面'||styleDataType === 'Polygon') {
                myobj.isPlaneFrame = info.isOutline,//是否有边框
                    myobj.planeLinewidth = info.lineWidth,//线宽度
                    myobj.frameRgbValue = info.outlineColor,//线条颜色
                    myobj.planeFrameDiaphaneity = info.lineTransparency,//线条透明度
                    myobj.planeFillDiaphaneity = info.fillTransparency,//填充透明度
                    myobj.planeRgbValue = info.polygonColor;//填充颜色

                for (var i in pointGraphicalComData) {
                    if (linearComData[i].value == info.lineType) {
                        myobj.planeLinearSelect.selected = linearComData[i];//线型
                        break;
                    }
                }

                //myobj.isPlaneFillPicture = !!info.iconPath;
                myobj.isPlaneFillPicture = !!info.isIcon;
                myobj.isPlaneColor = !myobj.isPlaneFillPicture;

                if (myobj.isPlaneFillPicture) {
                    myobj.isPlaneFillPicture = info.isIcon,
                        myobj.selectedPlaneImgInfo.path = info.iconPath ? info.iconPath : '',
                        myobj.planeImgSize = info.iconSize,
                        myobj.planeImgRotation = info.iconRotation;
                }
            }
        }

        //新增样式弹窗model
        $rootScope.styleColorBarSelect = {};//颜色带
        $rootScope.styleColorBarComData = [];//颜色带下拉
        dataStyle.getRandomColorRamps().success(function (a, b) {
            dataStyle.getLinearColorRamps().success(function (c, d) {
                $rootScope.styleColorBarComData = a.items.concat(c.items);
                $rootScope.styleColorBarSelect.selected = angular.copy(a.items[0]);
            });
        });
        //分情况给StyleInfo的属性附值
        function getStyleInfoByCondition(styleDataType, infos) {
            console.log(infos);

            var obj = {
                isOutline: false,//是否有边框
                lineWidth: '',//线宽度
                outlineColor: '',//线条颜色
                lineTransparency: '',//线条透明度
                fillTransparency: '',//填充透明度
                polygonColor: '',//填充颜色
                isFill: false,//是否填充
                lineType: 0,//线型
                isIcon: false,//是否填充图片
                iconPath: '',//图片地址
                iconSize: '',//图片大小
                iconRotation: '',//图片旋转角度
                pointType: '',//图形
            };
            // 分类： 点  线  面 
            if (styleDataType === '点' || styleDataType === 'Point') {//??????
                if (infos.isStylePointChecked2) {
                    obj.isIcon = infos.isStylePointChecked2,
                        obj.iconPath = !!infos.selectedPointImgInfo ? infos.selectedPointImgInfo.path : "",
                        obj.iconSize = infos.pointImgSize,
                        obj.iconRotation = infos.pointImgRotation;
                } else {
                    obj.pointType = infos.pointGraphicalSelect.selected.value;//图形
                    obj.isFill = infos.isStylePointChecked1;//是否填充
                    obj.polygonColor = infos.pointRgbValue;//填充颜色
                    obj.fillTransparency = infos.pointDiaphaneity;//填充透明度
                    obj.iconSize = infos.pointSize;//图片大小
                }

                /*
                  public bool IsIcon = false;
                  public string IconPath;
                  public string IconSize;
                  public string IconRotation;
                
                */
            } else if (styleDataType === '线' || styleDataType === 'Line') {
                obj.isOutline = true,//是否有边框
                    obj.lineType = infos.lineLinearSelect.selected.value,//线型
                    obj.lineWidth = infos.lineLinewidth,//线宽度
                    obj.outlineColor = infos.lineRgbValue,//线条颜色
                    obj.lineTransparency = infos.lineDiaphaneity;//线条透明度

                console.log(obj);

            } else if (styleDataType === '面'|| styleDataType === 'Polygon') {
                obj.isOutline = infos.isPlaneFrame,//是否有边框
                    obj.lineWidth = infos.planeLinewidth,//线宽度
                    obj.outlineColor = infos.frameRgbValue,//线条颜色
                    obj.lineTransparency = infos.planeFrameDiaphaneity,//线条透明度
                    obj.fillTransparency = infos.planeFillDiaphaneity,//填充透明度
                    obj.polygonColor = infos.planeRgbValue,//填充颜色
                    obj.isFill = infos.isPlaneColor,//是否填充
                    obj.lineType = infos.planeLinearSelect.selected.value;//线型

                if (infos.isPlaneFillPicture) {
                    obj.isIcon = infos.isPlaneFillPicture,
                        obj.iconPath = !!infos.selectedPlaneImgInfo ? infos.selectedPlaneImgInfo.path : "",
                        obj.iconSize = infos.planeImgSize,
                        obj.iconRotation = infos.planeImgRotation;
                } else {

                }
            }
            return obj;
        }


        $rootScope.addStyleModol = {
            XMLContentView: '',
            XMLviewName: '',
            XMLviewNameTxt: '',
            PolygonStyle: '{"IsIcon":false,"IconPath":"","IconSize":8,"IconRotation":"","PointType":"triangle","IsOutline":false,"LineWidth":"","LineType":0,"OutlineColor":"","LineTransparency":"","IsFill":true,"FillTransparency":"1.0","PolygonColor":"rgb(17, 85, 204)"}',

            planeLinearSelect: { selected: angular.copy(linearComData[0]) },//边框----线型
            lineLinearSelect: { selected: angular.copy(linearComData[0]) },//线型-----线
            pointGraphicalSelect: { selected: angular.copy(pointGraphicalComData[0]) },//图型

            isPlaneColor: true,//填充----颜色
            isPlaneFillPicture: false,//是否填充图片

            isStylePointChecked1: true,//图片与颜色切换 ,  颜色
            isStylePointChecked2: false,//图片与颜色切换 ,  图片
            selectedPlaneImgInfo: { path: '' },//填充面的图片信息 包含 图片地址和名称

            selectedPointImgInfo: { path: '' },//图标信息 包含 图片地址和名称

            selectedLayerInfo: {},//渲染图层,
            selectedLayerInfoTxt: '',
            styleRenderRule: '',//预设符号JSON串   样式渲染预设规则
            selectedRenderRuleImgInfo: { path: 'App/Main/assets/images/' + pointGraphicalComData[0].value + '.png' },//预设符号图片信息

            pointGraphicalComData: angular.copy(pointGraphicalComData),//点的---图形下拉
            planeLinearComData: angular.copy(linearComData),//面的---线形下拉
            lineLinearComData: angular.copy(linearComData),//线的---线形下拉

            currentRow: '',

            //分类渲染
            fieldTypeSelect: {},//渲染字段
            fieldTypeComData: [],//渲染字段下拉
            tableDataSource: [],//颜色表格数据源
            fieldTypeToView: function () {
                //console.log('Debug:  PolygonStyle     ', $rootScope.addStyleModol.PolygonStyle);

                //判断
                if (!$rootScope.addStyleModol.selectedLayerInfo.id) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.ChooseLayer'), '', 'warning', '#007AFF');
                    return;
                }
                if (!$rootScope.addStyleModol.PolygonStyle) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.SetSymbol'), '', 'warning', '#007AFF');
                    return;
                }
                if (!$rootScope.addStyleModol.fieldTypeSelect.selected || !$rootScope.addStyleModol.fieldTypeSelect.selected.codeName) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.RenderingNull'), '', 'warning', '#007AFF');
                    return;
                }
                if (!$rootScope.styleColorBarSelect.selected || !$rootScope.styleColorBarSelect.selected.colorName) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.ColorNull'), '', 'warning', '#007AFF');
                    return;
                }
                $rootScope.addStyleModol.tableDataSource = [];
                $rootScope.addStyleModol.pageIndex = 1;
                //console.log('Debug:   ', ' ng-style="{\'background-color\':\'rgb(\'' + 'row' + '.' + 'key' + '\')\'}" ');
                var layerId = $rootScope.addStyleModol.selectedLayerInfo.id,
                    layerAttr = $rootScope.addStyleModol.fieldTypeSelect.selected.attributeName,
                    colorName = $rootScope.styleColorBarSelect.selected.colorName,
                    style = $rootScope.addStyleModol.PolygonStyle;
                //console.log('Debug:   ', layerId, '     ', layerAttr, '     ', colorName, '     ', style);
                getDataAttributesPage(100, 1, function () {
                    if ($rootScope.addStyleModol.pageCounts < 1000) {
                        $rootScope.addStyleModol.tableDataSource = [];
                        getDataAttributesPage(1000, 1, function () { });
                    }
                });
            },//预览事件

            //加载更多数据
            loadMore: function () {
                
                $rootScope.addStyleModol.isBtnActive = true;
                $rootScope.addStyleModol.pageIndex++;
                getDataAttributesPage(100, $rootScope.addStyleModol.pageIndex, function () {
                    $rootScope.addStyleModol.isBtnActive = false;
                });
            },
            //分类渲染表格分页
            maxSize: 4,//页码个数显示数
            goPage: 1,//转到多少页
            pageCounts: 0,//32;//总条数
            pageIndex: 1,//1;//起始页
            pageSize: 100,//10;//每页显示条数
            //分类渲染表格分页的事件
            //pageChanged: function (a, evt) {
            //    if (evt && evt.keyCode !== 13) { return; }//注：回车键为13
            //    if (a) {
            //        a = parseInt(a);
            //        if (isNaN(a) || a < 1 || a > $rootScope.addStyleModol.totalPages) {
            //            $rootScope.addStyleModol.goPage = $rootScope.addStyleModol.pageIndex;
            //            return;
            //        }
            //        $rootScope.addStyleModol.goPage = a;
            //        $rootScope.addStyleModol.pageIndex = a;
            //    }
            //    //调用AJAX
            //    getDataAttributesPage($rootScope.addStyleModol.pageSize, $rootScope.addStyleModol.pageIndex);
            //},

            fieldTypeToReset: function () {
                $rootScope.addStyleModol.tableDataSource = [];
            },//清空事件

            //显示比例尺范围
            minDistance: '',//最小比例尺
            maxDistance: '',//最大比例尺

            styleTypeSelect: {},
            styleTypeComData: [],
            threeToOne: 1,

            //事件
            threeToOneChecked: function (bo) {
                if (bo) {
                    $rootScope.addStyleModol.currentLayer = null;
                    $rootScope.addStyleModol.XMLviewName = '';
                    $rootScope.addStyleModol.XMLviewNameTxt = $rootScope.addStyleModol.XMLviewName ? $rootScope.addStyleModol.XMLviewName : $filter('translate')('views.Style.SelectLayer');
                } else {
                    $rootScope.addStyleModol.currentLayer = angular.copy($rootScope.addStyleModol.selectedLayerInfo);
                    $rootScope.addStyleModol.XMLviewName = $rootScope.addStyleModol.currentLayer ? $rootScope.addStyleModol.currentLayer.layerName : "";
                    $rootScope.addStyleModol.XMLviewNameTxt = $rootScope.addStyleModol.XMLviewName ? $rootScope.addStyleModol.XMLviewName : $filter('translate')('views.Style.SelectLayer');
                }
            },

            //面复选框点击事件
            onStylePlaneChecked1: function () {
                $rootScope.addStyleModol.isPlaneFillPicture = $rootScope.addStyleModol.isPlaneColor;
                $rootScope.addStyleModol.isPlaneColor = !$rootScope.addStyleModol.isPlaneColor;
                //if ($rootScope.addStyleModol.isPlaneColor) {
                //    $rootScope.addStyleModol.isPlaneFillPicture = false;
                //}
                console.log('Debug:   ', $rootScope.addStyleModol.isPlaneColor, $rootScope.addStyleModol.isPlaneFillPicture);

            },
            onStylePlaneChecked2: function () {
                $rootScope.addStyleModol.isPlaneColor = $rootScope.addStyleModol.isPlaneFillPicture;
                $rootScope.addStyleModol.isPlaneFillPicture = !$rootScope.addStyleModol.isPlaneFillPicture;
                //if ($rootScope.addStyleModol.isPlaneFillPicture) {
                //    $rootScope.addStyleModol.isPlaneColor = false;
                //}
                console.log('Debug:   ', $rootScope.addStyleModol.isPlaneColor, $rootScope.addStyleModol.isPlaneFillPicture);
            },

            //点复选框点击事件
            onStylePointChecked1: function () {
                $rootScope.addStyleModol.isStylePointChecked2 = $rootScope.addStyleModol.isStylePointChecked1;
                $rootScope.addStyleModol.isStylePointChecked1 = !$rootScope.addStyleModol.isStylePointChecked1;
                //if ($rootScope.addStyleModol.isStylePointChecked1) {
                //    $rootScope.addStyleModol.isStylePointChecked2 = false;
                //}

                console.log('Debug:   ', $rootScope.addStyleModol.isStylePointChecked1, $rootScope.addStyleModol.isStylePointChecked2);

            },
            onStylePointChecked2: function () {
                $rootScope.addStyleModol.isStylePointChecked1 = $rootScope.addStyleModol.isStylePointChecked2;
                $rootScope.addStyleModol.isStylePointChecked2 = !$rootScope.addStyleModol.isStylePointChecked2;
                //if ($rootScope.addStyleModol.isStylePointChecked2) {
                //    $rootScope.addStyleModol.isStylePointChecked1 = false;
                //}
                console.log('Debug:   ', $rootScope.addStyleModol.isStylePointChecked1, $rootScope.addStyleModol.isStylePointChecked2);
            },

            //选择图层按钮的点击事件
            selectLayer: function () {
                $rootScope.addStyleModol.isCurrentLayer = false;
                $rootScope.styleSelectLayerModal.styleSelectLayerModalOpenFun();
            },
            onStyleTypeSelected: function () {
                //console.log('Debug:   ', $rootScope.stylePresetMarkModal.pointGraphicalSelect.selected);
                var val;
                var codeName = $rootScope.addStyleModol.styleTypeSelect.selected.codeName;
                if (codeName === '点' || codeName === 'Point') {
                    val = $rootScope.stylePresetMarkModal.pointGraphicalSelect.selected.value;
                } else if (codeName === '线' || codeName === 'Line') {
                    val = $rootScope.stylePresetMarkModal.lineLinearSelect.selected.name;
                } else if (codeName === '面' || codeName === 'Polygon') {
                    var tpmval = $rootScope.stylePresetMarkModal.planeLinearSelect.selected.value;
                    if (tpmval == 1)
                        val = 'shixian';
                    else if (tpmval == 2)
                        val = 'xuxian';
                    else if (tpmval == 3)
                        val = 'dianxian';
                    else if (tpmval == 4)
                        val = 'xudianxian';

                    if (!$rootScope.stylePresetMarkModal.isPlaneFrame) {
                        val = 'wubianxian';
                    }
                }
                $rootScope.addStyleModol.selectedRenderRuleImgInfo.path = val ? 'App/Main/assets/images/' + val + '.png' : '';
            },

            onSelectImg: function (str) {//图标点击事件
                $rootScope.styleForSelectImgModal.type = str;
                $rootScope.styleForSelectImgModal.styleForSelectImgOpenFun();
            },
            //弹出预设符号的弹框
            presetMarkClick: function () {
                $rootScope.stylePresetMarkModal.styleType = $rootScope.addStyleModol.styleTypeSelect.selected.codeName;
                $rootScope.stylePresetMarkModal.pointSize = $rootScope.stylePresetMarkModal.pointSize ? $rootScope.stylePresetMarkModal.pointSize : 8;
                $rootScope.stylePresetMarkModal.pointDiaphaneity = $rootScope.stylePresetMarkModal.pointDiaphaneity ? $rootScope.stylePresetMarkModal.pointDiaphaneity : 1;
                $rootScope.stylePresetMarkModal.lineDiaphaneity = $rootScope.stylePresetMarkModal.lineDiaphaneity ? $rootScope.stylePresetMarkModal.lineDiaphaneity : 1;
                $rootScope.stylePresetMarkModal.lineLinewidth = $rootScope.stylePresetMarkModal.lineLinewidth ? $rootScope.stylePresetMarkModal.lineLinewidth : 1;                
                $rootScope.stylePresetMarkModal.stylePresetMarkModalOpenFun();
            },
            //双击符号表格事件
            onDetailClick: function (a, b) {
                //isClickAble = !isClickAble;
                //var bo = true;
                //if (isClickAble === true) {
                // bo = false;
                //console.log('Debug:双击符号表格事件触发弹框   ', a, b);
                var srr = angular.fromJson(a.PolygonStyle);

                var styleInfo = {
                    isOutline: srr.IsOutline,//是否有边框
                    lineWidth: !!srr.LineWidth ? srr.LineWidth : 1,//线宽度
                    outlineColor: !srr.OutlineColor ? '' : (srr.OutlineColor.indexOf('rgb')>=0 ? srr.OutlineColor:('rgb(' + srr.OutlineColor + ')')),//线条颜色
                    lineTransparency: !!srr.LineTransparency ? srr.LineTransparency : 1,//线条透明度
                    fillTransparency: !!srr.FillTransparency ? srr.FillTransparency : 1,//填充透明度
                    polygonColor: !srr.PolygonColor ? '' : (srr.PolygonColor.indexOf('rgb') >= 0 ? srr.PolygonColor :('rgb(' + srr.PolygonColor + ')')),//填充颜色
                    isFill: srr.IsFill,//是否填充
                    lineType: srr.LineType,//线型
                    isIcon: srr.IsIcon,//是否填充图片
                    iconPath: srr.IconPath,//图片地址
                    iconSize: srr.IconSize,//图片大小
                    iconRotation: srr.IconRotation,//图片旋转角度
                    pointType: srr.PointType,//图形
                };
                
                $rootScope.addStyleModol.currentRow = a;
                toSetModalData($rootScope.styleSingleMarkModal, styleInfo);
                $rootScope.styleSingleMarkModal.title = $filter('translate')('views.Style.SetSymbol');
                $rootScope.styleSingleMarkModal.styleType = $rootScope.addStyleModol.styleTypeSelect.selected.codeName;
                $rootScope.styleSingleMarkModal.styleSingleMarkModalOpenFun();

                console.log($rootScope.addStyleModol.currentRow);

                // }
                //if (bo) {
                //   $timeout(function () {
                //        isClickAble = true;
                //    }, 800);
                //}
            },
            XMLclick: function (bo) {
                if (bo && !$rootScope.addStyleModol.styleName) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.PreviewStyle'), '', 'warning', '#007AFF');
                    return;
                }

                //XML预览
                if ($rootScope.addStyleModol.threeToOne == 3) {
                    editor2.setValue(editor1.getValue());
                    editor2.gotoLine(1);
                    $rootScope.addStyleModol.XMLContentView = editor1.getValue();
                    return;
                }
                var infos = $rootScope.addStyleModol;
                var styleDataTypeId = infos.styleTypeSelect.selected.id;
                var styleInfo = getStyleInfoByCondition(infos.styleTypeSelect.selected.codeName, infos);
                var obj = {
                    /*Id: '',
                    StyleName: '',
                    StyleDesc: '',
                    StyleContent: '',
                    StyleType: '',
                    
                    StyleDefaultLayer: '',
                    StyleRenderColorBand: '',
                    StyleRenderRule: '',如果还是发生请示错误就打开吧*/
                    CreateBy: localStorage.getItem('infoearth_spacedata_userCode'),
                    StyleConfigType: infos.threeToOne,
                    StyleRenderField: infos.fieldTypeSelect.selected ? infos.fieldTypeSelect.selected.id : '',
                    StyleRenderFieldName: infos.fieldTypeSelect.selected ? infos.fieldTypeSelect.selected.attributeName : '',
                    StyleDataType: styleDataTypeId,// 样式类型，1点，2线，3面

                    StyleInfo: {
                        Type: styleDataTypeId,// 样式类型，1点，2线，3面
                        MinScaleDenominator: infos.minDistance,// 最小比例尺
                        MaxScaleDenominator: infos.maxDistance,// 最大比例尺

                        //点样式
                        IsIcon: styleInfo.isIcon,
                        IconPath: encodeURI(styleInfo.iconPath),
                        IconSize: styleInfo.iconSize,
                        IconRotation: styleInfo.iconRotation,

                        //线样式
                        IsOutline: styleInfo.isOutline,
                        LineWidth: styleInfo.lineWidth,
                        OutlineColor: styleInfo.outlineColor,
                        LineTransparency: styleInfo.lineTransparency,
                        PointType: styleInfo.pointType,

                        //线型
                        LineType: styleInfo.lineType.value ? styleInfo.lineType.value: styleInfo.lineType,
                        IsFill: styleInfo.isFill,
                        FillTransparency: styleInfo.fillTransparency,
                        PolygonColor: styleInfo.polygonColor
                    },
                    RuleDatas: infos.tableDataSource
                };
                waitmask.onShowMask($filter('translate')('setting.waitText'), 200);

                console.log('Debug: obj  11111  ', obj);

                dataStyle.getXmlContent(obj).success(function (a, b) {
                    if (b == 200) {
                        editor2.setValue(a);
                        editor2.gotoLine(1);
                        $rootScope.addStyleModol.XMLContentView = a;
                        if (bo && $rootScope.addStyleModol.threeToOne == 2) {
                            preStyle($rootScope.addStyleModol.styleName, $rootScope.addStyleModol.currentLayer, $rootScope.addStyleModol.XMLContentView);
                        }
                    }
                    else
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.RequestError'), $filter('translate')('views.Style.alertFun.PleaseAgain'), 'error', '#007AFF');
                    waitmask.onHideMask();
                });

            },

            id: '',
            title: '',
            styleName: '',
            styleClass: '',
            styleDesc: '',
            imgUrl: '',
            //样式分类的下拉树
            typePullDownTreeData: [],
            typePullDownTreeSelData: {},
            initialTypeTreeData: {},

            isPre: false,

            preFile: function () {
                var file = document.getElementById('styleFile').files[0];
                if (!file) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.UploadFiles'), '', 'warning', '#007AFF');
                    return;
                }
                var reader = new FileReader();
                reader.readAsText(file, 'GBK');
                reader.onload = function (e) {
                    $rootScope.addStyleModol.content = this.result;
                    editor1.setValue(this.result)
                    //$timeout(function () {
                    //    $rootScope.addStyleModol.content = $rootScope.addStyleModol.content;
                    //});
                }
            },
            uploadImg: function () {
                var formData = new FormData();
                var name = $('#imgfile').val();
                formData.append('name', name);

                formData.append('file', $('#imgfile')[0].files[0]);

                if (!$('#imgfile')[0].files[0]) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.UploadFiles'), '', 'warning', '#007AFF');
                    return;
                }
                $.ajax({
                    url: 'upload',
                    type: 'POST',
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        //console.log(data);
                        if (data.success == "true") {
                            $rootScope.addStyleModol.imgUrl = data.httpPath;
                            $rootScope.alertFun($filter('translate')('views.Style.alertFun.UploadSuccessfully'), '', 'success', '#007AFF');
                            //console.log($rootScope.addStyleModol.imgUrl);
                        } else {
                            $rootScope.alertFun($filter('translate')('views.Style.alertFun.uploadFailed'), '', 'error', '#007AFF');
                        }
                    }
                });
            },
            //创建分类弹窗
            createClaOpenWinIn: function () {
                $rootScope.creatStyleTypeModal.classTreedata = angular.copy($rootScope.typeTreeData[0].children);
                //console.log($rootScope.creatTypeModal.classTreedata);
                $rootScope.creatStyleTypeModal.createStyle();
            },
            openStylePreviewWin: function () {
                var content = $.trim(editor1.getValue());
                if (!content) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.FillStyle'), '', 'warning', '#007AFF');
                    return;
                }
                var xmlError = editor1.env.document.$annotations;
                if (xmlError.length !== 0) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_1') + xmlError[0].row + $filter('translate')('views.Style.alertFun.StyleError_2'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                    return;
                }
                //正则匹配XML
                if (!(/<([^>]+)>([^<>]+)<\/\1>/m.exec(content))) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_4'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                    return;
                }
                if (!matchingXmlHeadAndEnd(content)) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_5'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                    return;
                }
                $rootScope.stylePreviewModal.openStylePreview();
            },
            // 地图数据
            mapDataset: mapDataset,
            mapheight: 500,
            isLoadTianDiTu: parseInt(isLoadTianDiTu),
            choseLayer: function () {
                var threeToOne = $rootScope.addStyleModol.threeToOne;
                // 分类： 单一样式 1  分类渲染 2    导入样式 3 
                if (threeToOne == 3) {
                    var content = $.trim(editor1.getValue());
                    if (!content) {
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleEmpty'), '', 'warning', '#007AFF');
                        return;
                    }
                    var xmlError = editor1.env.document.$annotations;
                    if (xmlError != null && xmlError.length !== 0) {
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_1') + xmlError[0].row + $filter('translate')('views.Style.alertFun.StyleError_2'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                        return;
                    }
                    //正则匹配XML
                    if (!(/<([^>]+)>([^<>]+)<\/\1>/m.exec(content))) {
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_4'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                        return;
                    }
                    if (!matchingXmlHeadAndEnd(content)) {
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_5'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                        return;
                    }
                }
                $rootScope.addStyleModol.isCurrentLayer = true;
                $rootScope.styleSelectLayerModal.styleSelectLayerModalOpenFun();
            },
            currentLayer: {}
        };

        //查询样式类型
        dicDataCode.getDetailByConn(layerDataTypeID, 'vector').success(function (data, statues) {
            //console.log('Debug:   ',data.items);
            $rootScope.addStyleModol.styleTypeComData = data.items;
            $rootScope.addStyleModol.styleTypeSelect.selected = $rootScope.addStyleModol.styleTypeComData[0];
        });

        //获取图层列表
        function getLayersLists(layerType, layerTag, layerName, mapId, PageSize, PageIndex) {
            //console.log('Debug:xxxx   ', $rootScope.addStyleModol.styleTypeSelect.selected.id);

            layerContent.getPageListByName({ DataType: $rootScope.addStyleModol.styleTypeSelect.selected.id, LayerType: layerType, LayerTag: layerTag, LayerName: layerName, CreateBy: localStorage.getItem('infoearth_spacedata_userCode') }, PageSize, PageIndex).success(function (data, statues) {
                $rootScope.styleSelectLayerModal.layersA = angular.copy(data.items);;
                $rootScope.styleSelectLayerModal.pageingB.pageCounts = data.totalCount;

            }).error(function (data, status) {
                $rootScope.alertFun($filter('translate')('views.Style.alertFun.error'), data.message, 'error', '#007AFF');
            });
        }

        //获取分类渲染列表的分页数据
        function getDataAttributesPage(size, index, fun) {
            var layerId = $rootScope.addStyleModol.selectedLayerInfo.id,
                    layerAttr = $rootScope.addStyleModol.fieldTypeSelect.selected.attributeName,
                    colorName = $rootScope.styleColorBarSelect.selected.colorName,
                    style = $rootScope.addStyleModol.PolygonStyle;
            dataStyle.getDataAttributesPage(layerId, layerAttr, colorName, style, size, index).success(function (a, b) {
                if (b == 200) {
                    a = JSON.parse(a);
                    //console.log(a);

                    var dt = a.DataTableJson;
                    $rootScope.addStyleModol.pageCounts = a.Count;
                    $rootScope.addStyleModol.totalPages = Math.ceil($rootScope.addStyleModol.pageCounts / $rootScope.addStyleModol.pageSize);

                    var sc = $filter('translate')('views.Style.symbol');

                    for (var i = 0; i < dt.length; i++) {
                        dt[i][sc] = angular.fromJson(dt[i][sc]);
                    }

                    var arr = [];
                    for (var k in dt) {
                        var o = {};
                        o.data = dt[k];
                        o.mark = dt[k][sc]['PolygonColor'];
                        o.Value = dt[k][$filter('translate')('views.Style.value')];
                        o.Title = dt[k][$filter('translate')('views.Style.Note')];
                        o.Count = dt[k][$filter('translate')('views.Style.Number')];
                        o.GUID = getRandom();
                        var s = angular.fromJson(style);
                        if (!!o.mark) {
                            s.OutlineColor = "rgb(" + o.mark + ")";
                            s.PolygonColor = "rgb(" + o.mark + ")";
                        }
                        else {
                            s.OutlineColor = "";
                            s.PolygonColor = "";
                        }
                        
                        o.PolygonStyle = JSON.stringify(s);
                        arr.push(o);
                    }
                    $rootScope.addStyleModol.tableDataSource = $rootScope.addStyleModol.tableDataSource.concat(arr);
                    if (typeof (fun) === "function") {
                        fun();
                    }
                    //console.log($rootScope.addStyleModol.tableDataSource);
                } else {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.RequestError'), $filter('translate')('views.Style.alertFun.PleaseAgain'), 'error', '#007AFF');
                }
            }).error(function (data, status) {
                $rootScope.alertFun($filter('translate')('views.Style.alertFun.RequestError'), $filter('translate')('views.Style.alertFun.PleaseAgain'), 'error', '#007AFF');
                console.log(data, status);
            });
        }


        //样式管理-------------预设符号和单条符号信息编辑弹框-----------------------------start
        function toGetStyleRenderRuleJson(styleType, infos) {
            var styleInfo = getStyleInfoByCondition(styleType, infos);
            var obj = {
                IsIcon: styleInfo.isIcon,
                IconPath: styleInfo.iconPath,
                IconSize: styleInfo.iconSize,
                IconRotation: styleInfo.iconRotation,
                PointType: styleInfo.pointType,
                IsOutline: styleInfo.isOutline,
                LineWidth: styleInfo.lineWidth,
                LineType: styleInfo.lineType,
                OutlineColor: styleInfo.outlineColor,
                LineTransparency: styleInfo.lineTransparency,
                IsFill: styleInfo.isFill,
                FillTransparency: styleInfo.fillTransparency,
                PolygonColor: styleInfo.polygonColor
            };
            return JSON.stringify(obj);
        }
        //样式管理之预设符号
        $rootScope.stylePresetMarkModal = {
            title:"",
            planeLinearSelect: { selected: angular.copy(linearComData[0]) },//边框----线型
            lineLinearSelect: { selected: angular.copy(linearComData[0]) },//线型-----线
            pointGraphicalSelect: { selected: angular.copy(pointGraphicalComData[0]) },//图型
            isPlaneColor: true,//填充----颜色
            isPlaneFillPicture: false,//是否填充图片

            isStylePointChecked1: true,//图片与颜色切换 ,  颜色
            isStylePointChecked2: false,//图片与颜色切换 ,  图片
            selectedPointImgInfo: { path: '' },
            selectedPlaneImgInfo: { path: '' },

            pointGraphicalComData: angular.copy(pointGraphicalComData),//点的---图形下拉
            planeLinearComData: angular.copy(linearComData),//面的---线形下拉
            lineLinearComData: angular.copy(linearComData),//线的---线形下拉

            //面复选框点击事件
            onStylePlaneChecked1: function () {
                $rootScope.stylePresetMarkModal.isPlaneFillPicture = $rootScope.stylePresetMarkModal.isPlaneColor;
                $rootScope.stylePresetMarkModal.isPlaneColor = !$rootScope.stylePresetMarkModal.isPlaneColor;
                //if ($rootScope.addStyleModol.isPlaneColor) {
                //    $rootScope.addStyleModol.isPlaneFillPicture = false;
                //}
                console.log('Debug:   ', $rootScope.stylePresetMarkModal.isPlaneColor, $rootScope.stylePresetMarkModal.isPlaneFillPicture);

            },
            onStylePlaneChecked2: function () {
                $rootScope.stylePresetMarkModal.isPlaneColor = $rootScope.stylePresetMarkModal.isPlaneFillPicture;
                $rootScope.stylePresetMarkModal.isPlaneFillPicture = !$rootScope.stylePresetMarkModal.isPlaneFillPicture;
                //if ($rootScope.addStyleModol.isPlaneFillPicture) {
                //    $rootScope.addStyleModol.isPlaneColor = false;
                //}
                console.log('Debug:   ', $rootScope.stylePresetMarkModal.isPlaneColor, $rootScope.stylePresetMarkModal.isPlaneFillPicture);
            },

            //点复选框点击事件
            onStylePointChecked1: function () {
                $rootScope.stylePresetMarkModal.isStylePointChecked2 = $rootScope.stylePresetMarkModal.isStylePointChecked1;
                $rootScope.stylePresetMarkModal.isStylePointChecked1 = !$rootScope.stylePresetMarkModal.isStylePointChecked1;
                //if ($rootScope.addStyleModol.isStylePointChecked1) {
                //    $rootScope.addStyleModol.isStylePointChecked2 = false;
                //}

                console.log('Debug:   ', $rootScope.stylePresetMarkModal.isStylePointChecked1, $rootScope.stylePresetMarkModal.isStylePointChecked2);

            },
            onStylePointChecked2: function () {
                $rootScope.stylePresetMarkModal.isStylePointChecked1 = $rootScope.stylePresetMarkModal.isStylePointChecked2;
                $rootScope.stylePresetMarkModal.isStylePointChecked2 = !$rootScope.stylePresetMarkModal.isStylePointChecked2;
                //if ($rootScope.addStyleModol.isStylePointChecked2) {
                //    $rootScope.addStyleModol.isStylePointChecked1 = false;
                //}
                //console.log('Debug:   ', $rootScope.stylePresetMarkModal.isStylePointChecked1, $rootScope.stylePresetMarkModal.isStylePointChecked2);
            },
            onSelectImg: function (str) {//图标点击事件
                $rootScope.addStyleModol.onSelectImg(str);
            },
            submitForm: function (a, b) {

                //console.log('Debug:   ', $rootScope.addStyleModol.styleTypeSelect.selected, $rootScope.stylePresetMarkModal.isStylePointChecked2);
                $rootScope.addStyleModol.PolygonStyle = toGetStyleRenderRuleJson($rootScope.stylePresetMarkModal.styleType, $rootScope.stylePresetMarkModal);

                if (($rootScope.addStyleModol.styleTypeSelect.selected.codeName === '点' || $rootScope.addStyleModol.styleTypeSelect.selected.codeName === 'Point') && $rootScope.stylePresetMarkModal.isStylePointChecked2) {
                    $rootScope.addStyleModol.selectedRenderRuleImgInfo = angular.copy($rootScope.stylePresetMarkModal.selectedPointImgInfo);
                } else if ($rootScope.addStyleModol.styleTypeSelect.selected.codeName === '线' || $rootScope.addStyleModol.styleTypeSelect.selected.codeName === 'Line') {
                    $rootScope.addStyleModol.selectedRenderRuleImgInfo = { path: '' };
                } else if (($rootScope.addStyleModol.styleTypeSelect.selected.codeName === '面' || $rootScope.addStyleModol.styleTypeSelect.selected.codeName === 'Polygon') && $rootScope.stylePresetMarkModal.isPlaneFillPicture) {
                    $rootScope.addStyleModol.selectedRenderRuleImgInfo.path = angular.copy($rootScope.stylePresetMarkModal.selectedPointImgInfo);
                }
                $rootScope.addStyleModol.onStyleTypeSelected();
                a.close();
            },
            cancel: function(){
                $rootScope.stylePresetMarkModal.planeLinearSelect = { selected: angular.copy(linearComData[0]) };
                $rootScope.stylePresetMarkModal.lineLinearSelect = { selected: angular.copy(linearComData[0]) };
                $rootScope.stylePresetMarkModal.pointGraphicalSelect = { selected: angular.copy(pointGraphicalComData[0]) };
            },
            openWin: function(){
                $rootScope.stylePresetMarkModal.title = $filter('translate')('views.Style.DefaultSymbol');
            },
            //模板
            html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            ' <div class="form-group" style="padding-left:15px;padding-right:15px;" id="DefaultSymbol">' +
            '    <!--单一样式 之  点-->' +
            '    <div ng-show="(popwinmodal.styleType === \'点\')||(popwinmodal.styleType === \'Point\')">' +
            '        <div style="margin-bottom: 10px; padding-left: 0px; width: 100%;">' +
            '            <div class="col-sm-1" style="padding-left: 0px;">' +
            '                <div style="float: right; padding-right: 0px; padding-left: 0px; width: 100%;">' +
            '                    <div class="checkbox clip-check check-primary check-column">' +
            '                        <input type="checkbox" id="style_mark_dian1" ng-checked="popwinmodal.isStylePointChecked1" ng-click="popwinmodal.onStylePointChecked1()">' +
            '                        <label for="style_mark_dian1" translate="views.Style.colour">颜色</label>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div class="col-sm-11">' +
            '                <div ng-show="!popwinmodal.isStylePointChecked1" style="right: -5px; width: 98%; height: 100%;cursor:not-allowed; position: absolute; z-index: 3; opacity: 0.5; background-color: rgb(252, 253, 254)"></div>' +
            '                <div style="margin-bottom: 10px; width: 100%;">' +
            '                    <div class="col-sm-6 select-item">' +
            '                        <div class="col-sm-3" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px; padding-left: 10px;" translate="views.Style.FillColor">填充色</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9">' +
            '                            <mic-spectrum rgb-value="popwinmodal.pointRgbValue"></mic-spectrum>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div class="col-sm-6 select-item">' +
            '                        <div class="col-sm-3" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px; padding-left: 10px;" translate="views.Style.Graphics">图形</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9">' +
            '                            <ui-select class="ui-select-width" ng-model="popwinmodal.pointGraphicalSelect.selected" search-enabled="0" theme="select2" title="{{\'views.Style.Choose\'|translate}}">' +
            '                                <ui-select-match placeholder="{{\'views.Style.Choose\'|translate}}"><img style="width:20px;height:20px;" src="{{\'App/Main/assets/images/\'+$select.selected.value+\'.png\'}}"></img></ui-select-match>' +
            '                                <ui-select-choices class="ui-select-height" repeat="p in popwinmodal.pointGraphicalComData">' +
            '                                    <img style="width:20px;height:20px;" src="{{\'App/Main/assets/images/\'+p.value+\'.png\'}}"></img>' +
            '                                </ui-select-choices>' +
            '                            </ui-select>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div style="width: 100%;">' +
            '                    <div class="col-sm-6 select-item">' +
            '                        <div class="col-sm-3" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px; padding-left: 10px;" translate="views.Style.size">大小</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9">' +
            '                            <num-box max-num="200" min-num="8" precision="0" width="179" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="1" text-model="popwinmodal.pointSize"></num-box>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div class="col-sm-6 select-item">' +
            '                        <div class="col-sm-3" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px; padding-left: 0;" translate="views.Style.transparency">透明度</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9">' +
            '                            <num-box max-num="1.0" min-num="0.1" precision="1" width="179" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="0.1" text-model="popwinmodal.pointDiaphaneity"></num-box>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div class="col-sm-6"></div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div style="clear: both;"></div>' +
            '        </div>' +
            '        <div style="margin-bottom: 10px; padding-left: 10px; width: 100%; display: none;">' +
            '            <div class="col-sm-1">' +
            '                <div style="float: right; padding-right: 10px; padding-left: 0px; width: 100%;">' +
            '                    <div class="checkbox clip-check check-primary check-column">' +
            '                        <input type="checkbox" id="style_mark_dian2" ng-checked="popwinmodal.isStylePointChecked2" ng-click="popwinmodal.onStylePointChecked2()">' +
            '                        <label for="style_mark_dian2" translate="views.Style.image">图片</label>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div class="col-sm-11">' +
            '                <div ng-show="!popwinmodal.isStylePointChecked2"style="right: -5px; width: 98%; height: 100%; cursor: not-allowed; position: absolute; z-index: 1; opacity: 0.5; background-color: rgb(252, 253, 254); "></div>' +
            '                <div class="col-sm-6">' +
            '                    <div class="col-sm-2" style="padding-left: 0px;">' +
            '                        <label class="font-title-little" style="padding-top: 8px; padding-left: 15px;" translate="views.Style.icon">图标</label>' +
            '                    </div>' +
            '                    <div class="col-sm-10" style="padding-left: 0px;">' +
            '                        <img style="width:32px;height:32px;cursor:pointer;" src="{{popwinmodal.selectedPointImgInfo.path?popwinmodal.selectedPointImgInfo.path:\'App/Main/assets/images/new-defult.png\'}}" ng-click="popwinmodal.onSelectImg(\'预设符号点图标\')" />' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-6"></div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div style="clear: both;"></div>' +
            '        </div>' +
            '    </div>' +
            '    <!--单一样式 之  线-->' +
            '    <div ng-show="(popwinmodal.styleType === \'线\')||(popwinmodal.styleType === \'Line\')">' +
            '        <fieldset style="width: 100%; margin: 0; padding: 0;">' +
            '            <legend></legend>' +
            '            <div style="width: 100%; margin-bottom: 10px;">' +
            '                <div class="col-sm-6 select-item" style="margin-top:10px;">' +
            '                    <div class="col-sm-3" style="padding-left: 0;">' +
            '                        <label class="font-title-little" style="padding-top: 6px; padding-left: 5px;" translate="views.Style.LineColor">线颜色</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9">' +
            '                        <mic-spectrum rgb-value="popwinmodal.frameRgbValue"></mic-spectrum>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-6 select-item" style="margin-top:10px;">' +
            '                    <div class="col-sm-3" style="padding-left: 0;">' +
            '                        <label class="font-title-little" style="padding-top: 6px; padding-left: 15px;" translate="views.Style.Linear">线型</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9">' +
            '                        <ui-select style="width:206px;" class="ui-select-width" ng-model="popwinmodal.lineLinearSelect.selected" search-enabled="0" theme="select2" title="{{\'views.Style.Choose\'|translate}}">' +
            '                            <ui-select-match placeholder="{{\'views.Style.Choose\'|translate}}"><img src="{{\'App/Main/assets/images/\'+$select.selected.name+\'.png\'}}" style="position:absolute;bottom:5px;width:150px;"></img></ui-select-match>' +
            '                            <ui-select-choices class="ui-select-height" repeat="p in popwinmodal.lineLinearComData">' +
            '                                <img src="{{\'App/Main/assets/images/\'+p.name+\'.png\'}}"></img>' +
            '                            </ui-select-choices>' +
            '                        </ui-select>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div class="col-sm-6 select-item" style="margin-top:10px;">' +
            '                    <div class="col-sm-3" style="padding-left: 0;">' +
            '                        <label class="font-title-little" style="padding-top: 6px; padding-left: 0;" translate="views.Style.transparency">透明度</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9">' +
            '                        <num-box max-num="1.0" min-num="0.1" precision="1" width="179" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="0.1" text-model="popwinmodal.lineDiaphaneity"></num-box>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-6 select-item" style="margin-top:10px;">' +
            '                    <div class="col-sm-3" style="padding-left: 0;">' +
            '                        <label class="font-title-little" style="padding-top: 6px; padding-left: 5px;" translate="views.Style.LineWidth">线宽度</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9">' +
            '                        <num-box max-num="20" min-num="1" precision="0" width="185" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="1" text-model="popwinmodal.lineLinewidth"></num-box>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '        </fieldset>' +
            '    </div>' +
            '    <!--单一样式 之  面-->' +
            '    <div ng-show="(popwinmodal.styleType === \'面\')||(popwinmodal.styleType === \'Polygon\')">' +
            '        <fieldset style="width: 100%; margin: 0px; padding: 0 10px;">' +
            '            <legend translate="views.Style.Frame">边框</legend>' +
            '            <div style="width: 100%; margin-bottom: 10px;margin-top: 10px;">' +
            '                <div class="col-sm-1">' +
            '                    <div class="checkbox clip-check check-primary check-column">' +
            '                        <input type="checkbox" id="style_mark_ch1" ng-checked="popwinmodal.isPlaneFrame" ng-click="popwinmodal.isPlaneFrame =! popwinmodal.isPlaneFrame;">' +
            '                        <label for="style_mark_ch1" translate="views.Style.Frame">边框</label>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-11" style="padding-left: 15px; margin-top: 1px;">' +
            '                    <div ng-show="!popwinmodal.isPlaneFrame" style="right: -6px; width: 98%; height: 100%;cursor:not-allowed; position: absolute; z-index: 3; opacity: 0.5; background-color: rgb(252, 253, 254)"></div>' +
            '                    <div style="width: 100%; margin-bottom: 10px;">' +
            '                        <div class="col-sm-6 select-item">' +
            '                            <div class="col-sm-3" style="padding-left: 0;">' +
            '                                <label class="font-title-little" style="padding-top: 6px; padding-left: 0;" translate="views.Style.LineColor">线颜色</label>' +
            '                            </div>' +
            '                            <div class="col-sm-9">' +
            '                                <mic-spectrum rgb-value="popwinmodal.frameRgbValue"></mic-spectrum>' +
            '                            </div>' +
            '                            <div style="clear: both;"></div>' +
            '                        </div>' +
            '                        <div class="col-sm-6 select-item">' +
            '                            <div class="col-sm-3" style="padding-left: 0;">' +
            '                                <label class="font-title-little" style="padding-top: 6px; padding-left: 15px;" translate="views.Style.Linear">线型</label>' +
            '                            </div>' +
            '                            <div class="col-sm-9">' +
            '                                <ui-select style="width: 186px;" class="ui-select-width" ng-model="popwinmodal.planeLinearSelect.selected" search-enabled="0" theme="select2" title="{{\'views.Style.Choose\'|translate}}">' +
            '                                    <ui-select-match placeholder="{{\'views.Style.Choose\'|translate}}"><img src="{{\'App/Main/assets/images/\'+$select.selected.name+\'.png\'}}" style="position:absolute;bottom:5px;width:150px;"></img></ui-select-match>' +
            '                                    <ui-select-choices class="ui-select-height" repeat="p in popwinmodal.planeLinearComData">' +
            '                                        <img src="{{\'App/Main/assets/images/\'+p.name+\'.png\'}}"></img>' +
            '                                    </ui-select-choices>' +
            '                                </ui-select>' +
            '                            </div>' +
            '                            <div style="clear: both;"></div>' +
            '                        </div>' +
            '                        <div style="clear: both;"></div>' +
            '                    </div>' +
            '                    <div style="margin-bottom: 10px;">' +
            '                        <div class="col-sm-6 select-item">' +
            '                            <div class="col-sm-3" style="padding-left: 0;">' +
            '                                <label class="font-title-little" style="padding-top: 6px; padding-left: 0;" translate="views.Style.transparency">透明度</label>' +
            '                            </div>' +
            '                            <div class="col-sm-9">' +
            '                                <num-box max-num="1.0" min-num="0.1" precision="1" width="159" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="0.1" text-model="popwinmodal.planeFrameDiaphaneity"></num-box>' +
            '                            </div>' +
            '                            <div style="clear: both;"></div>' +
            '                        </div>' +
            '                        <div class="col-sm-6 select-item">' +
            '                            <div class="col-sm-3" style="padding-left: 0;">' +
            '                                <label class="font-title-little" style="padding-top: 6px; padding-left: 0;" translate="views.Style.LineWidth">线宽度</label>' +
            '                            </div>' +
            '                            <div class="col-sm-9">' +
            '                                <num-box max-num="20" min-num="1" precision="0" width="165" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="1" text-model="popwinmodal.planeLinewidth"></num-box>' +
            '                            </div>' +
            '                            <div style="clear: both;"></div>' +
            '                        </div>' +
            '                        <div style="clear: both;"></div>' +
            '                    </div>' +
            '                </div>' +
            '            </div>' +
            '        </fieldset>' +
            '        <fieldset style="width: 100%; margin: 15px 0px 0px 0px; padding: 0 10px;">' +
            '            <legend translate="views.Style.Fill">填充</legend>' +
            '            <div style="width: 100%; margin-top: 10px;">' +
            '                <div class="col-sm-1" style="margin-top: 1px;">' +
            '                    <div style="float: right; padding-right: 10px; padding-left: 0px; width: 100%;">' +
            '                        <div class="checkbox clip-check check-primary check-column">' +
            '                            <input type="checkbox" id="style_mark_ch2" ng-checked="popwinmodal.isPlaneColor" ng-click="popwinmodal.onStylePlaneChecked1()">' +
            '                            <label for="style_mark_ch2" translate="views.Style.colour">颜色</label>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div class="col-sm-11" style="margin-top: 5px; padding-left: 15px;">' +
            '                    <div ng-show="!popwinmodal.isPlaneColor" style="right: -5px; width: 98%; height: 100%; cursor: not-allowed; position: absolute; z-index: 3; opacity: 0.5; background-color: rgb(252, 253, 254)"></div>' +
            '                    <div style="margin-bottom: 10px; width: 100%;">' +
            '                        <div class="col-sm-6 select-item">' +
            '                            <div class="col-sm-3" style="padding-left: 0;">' +
            '                                <label class="font-title-little" style="padding-top: 4px; padding-left: 15px;" translate="views.Style.FillColor">填充色</label>' +
            '                            </div>' +
            '                            <div class="col-sm-9">' +
            '                                <mic-spectrum rgb-value="popwinmodal.planeRgbValue"></mic-spectrum>' +
            '                            </div>' +
            '                        </div>' +
            '                        <div class="col-sm-6 select-item">' +
            '                            <div class="col-sm-3" style="padding-left: 0;">' +
            '                                <label class="font-title-little" style="padding-top: 4px; padding-left: 0;" translate="views.Style.transparency">透明度</label>' +
            '                            </div>' +
            '                            <div class="col-sm-9">' +
            '                                <num-box max-num="1.0" min-num="0.1" precision="1" width="165" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="0.1" text-model="popwinmodal.planeFillDiaphaneity"></num-box>' +
            '                            </div>' +
            '                        </div>' +
            '                        <div style="clear: both;"></div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div style="width: 100%; margin-bottom: 10px; display: none;">' +
            '                <div class="col-sm-1">' +
            '                    <div style="float: right; padding-right: 10px; padding-left: 0px; width: 100%;">' +
            '                        <div class="checkbox clip-check check-primary check-column">' +
            '                            <input type="checkbox" id="style_mark0_ch3" ng-checked="popwinmodal.isPlaneFillPicture" ng-click="popwinmodal.onStylePlaneChecked2()">' +
            '                            <label for="style_mark0_ch3" translate="views.Style.image">图片</label>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div class="col-sm-11">' +
            '                    <div ng-show="!popwinmodal.isPlaneFillPicture" style="right: -5px; width: 98%; height: 100%; cursor: not-allowed; position: absolute; z-index: 1; opacity: 0.5; background-color: rgb(252, 253, 254); "></div>' +
            '                    <div class="col-sm-6">' +
            '                        <div class="col-sm-2" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px; padding-left: 15px;" translate="views.Style.icon">图标</label>' +
            '                        </div>' +
            '                        <div class="col-sm-10" style="padding-left: 0;">' +
            '                            <img style="width: 32px; height: 32px; cursor: pointer;" src="{{popwinmodal.selectedPlaneImgInfo.path?popwinmodal.selectedPlaneImgInfo.path:\'App/Main/assets/images/new-defult.png\'}}" ng-click="popwinmodal.onSelectImg(\'预设符号面图标\')" />' +
            '                        </div>' +
            '                    </div>' +
            '                    <div class="col-sm-6"></div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '        </fieldset>' +
            '    </div>' +
            '</div>' +
            '<div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '    <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.sure">确定</button>' +
            '    <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()" translate="setting.cancel">取消</a>' +
            '</div>' +
            '</form>',
        };

        //样式管理之 分类渲染单条符号信息编辑弹框
        $rootScope.styleSingleMarkModal = {
            title: "",
            planeLinearSelect: { selected: angular.copy(linearComData[0]) },//边框----线型
            lineLinearSelect: { selected: angular.copy(linearComData[0]) },//线型-----线
            pointGraphicalSelect: { selected: angular.copy(pointGraphicalComData[0]) },//图型
            isPlaneColor: true,//填充----颜色
            pointRgbValue: "",//点填充色
            lineRgbValue: "",//线填充色
            planeRgbValue: "",//面填充色
            isPlaneFillPicture: false,//是否填充图片

            isStylePointChecked1: true,//图片与颜色切换 ,  颜色
            isStylePointChecked2: false,//图片与颜色切换 ,  图片


            selectedPointImgInfo: { path: '' },
            selectedPlaneImgInfo: { path: '' },

            pointGraphicalComData: angular.copy(pointGraphicalComData),//点的---图形下拉
            planeLinearComData: angular.copy(linearComData),//面的---线形下拉
            lineLinearComData: angular.copy(linearComData),//线的---线形下拉

            //面复选框点击事件
            onStylePlaneChecked1: function () {
                $rootScope.styleSingleMarkModal.isPlaneFillPicture = $rootScope.styleSingleMarkModal.isPlaneColor;
                $rootScope.styleSingleMarkModal.isPlaneColor = !$rootScope.styleSingleMarkModal.isPlaneColor;
                //if ($rootScope.addStyleModol.isPlaneColor) {
                //    $rootScope.addStyleModol.isPlaneFillPicture = false;
                //}
                console.log('Debug:   ', $rootScope.styleSingleMarkModal.isPlaneColor, $rootScope.styleSingleMarkModal.isPlaneFillPicture);

            },
            onStylePlaneChecked2: function () {
                $rootScope.styleSingleMarkModal.isPlaneColor = $rootScope.styleSingleMarkModal.isPlaneFillPicture;
                $rootScope.styleSingleMarkModal.isPlaneFillPicture = !$rootScope.styleSingleMarkModal.isPlaneFillPicture;
                //if ($rootScope.addStyleModol.isPlaneFillPicture) {
                //    $rootScope.addStyleModol.isPlaneColor = false;
                //}
                console.log('Debug:   ', $rootScope.styleSingleMarkModal.isPlaneColor, $rootScope.styleSingleMarkModal.isPlaneFillPicture);
            },
            //点复选框点击事件
            onStylePointChecked1: function () {
                $rootScope.styleSingleMarkModal.isStylePointChecked2 = $rootScope.styleSingleMarkModal.isStylePointChecked1;
                $rootScope.styleSingleMarkModal.isStylePointChecked1 = !$rootScope.styleSingleMarkModal.isStylePointChecked1;
                //if ($rootScope.addStyleModol.isStylePointChecked1) {
                //    $rootScope.addStyleModol.isStylePointChecked2 = false;
                //}

                console.log('Debug:   ', $rootScope.styleSingleMarkModal.isStylePointChecked1, $rootScope.styleSingleMarkModal.isStylePointChecked2);

            },
            onStylePointChecked2: function () {
                $rootScope.styleSingleMarkModal.isStylePointChecked1 = $rootScope.styleSingleMarkModal.isStylePointChecked2;
                $rootScope.styleSingleMarkModal.isStylePointChecked2 = !$rootScope.styleSingleMarkModal.isStylePointChecked2;
                //if ($rootScope.addStyleModol.isStylePointChecked2) {
                //    $rootScope.addStyleModol.isStylePointChecked1 = false;
                //}
                console.log('Debug:   ', $rootScope.styleSingleMarkModal.isStylePointChecked1, $rootScope.styleSingleMarkModal.isStylePointChecked2);
            },
            onSelectImg: function (str) {//图标点击事件
                $rootScope.addStyleModol.onSelectImg(str);
            },
            submitForm: function (a, b) {
                var rgbNum = "";
                $rootScope.addStyleModol.styleRenderRule = toGetStyleRenderRuleJson($rootScope.styleSingleMarkModal.styleType, $rootScope.styleSingleMarkModal);
                $rootScope.addStyleModol.currentRow.PolygonStyle = $rootScope.addStyleModol.styleRenderRule;

                if ($rootScope.addStyleModol.styleTypeSelect.selected.codeName === '点' || $rootScope.addStyleModol.styleTypeSelect.selected.codeName === 'Point') {
                    if ($rootScope.styleSingleMarkModal.isStylePointChecked1){
                        rgbNum = $rootScope.styleSingleMarkModal.pointRgbValue.substr(4, 13).split(")")[0];
                    }
                }
                else if ($rootScope.addStyleModol.styleTypeSelect.selected.codeName === '线' || $rootScope.addStyleModol.styleTypeSelect.selected.codeName === 'Line') {
                    //$rootScope.addStyleModol.selectedRenderRuleImgInfo = { path: '' };
                    rgbNum = $rootScope.styleSingleMarkModal.lineRgbValue.substr(4, 13).split(")")[0];
                }
                else if ($rootScope.addStyleModol.styleTypeSelect.selected.codeName === '面' || $rootScope.addStyleModol.styleTypeSelect.selected.codeName === 'Polygon') {
                    if ($rootScope.styleSingleMarkModal.isPlaneColor) {
                        //填充的颜色
                        rgbNum = $rootScope.styleSingleMarkModal.planeRgbValue.substr($rootScope.styleSingleMarkModal.planeRgbValue.indexOf('(') + 1, $rootScope.styleSingleMarkModal.planeRgbValue.indexOf(')') - $rootScope.styleSingleMarkModal.planeRgbValue.indexOf('(') - 1);
                    }
                }

                $rootScope.addStyleModol.currentRow.mark = angular.copy(rgbNum);

                //console.log($rootScope.addStyleModol.currentRow);
                var sc = $filter('translate')('views.Style.symbol');
                for (var i = 0; i < $rootScope.addStyleModol.tableDataSource.length; i++) {
                    if ($rootScope.addStyleModol.tableDataSource[i].GUID === $rootScope.addStyleModol.currentRow.GUID) {
                        $rootScope.addStyleModol.tableDataSource[i].mark = angular.copy($rootScope.addStyleModol.currentRow.mark);
                        $rootScope.addStyleModol.tableDataSource[i].PolygonStyle = angular.copy($rootScope.addStyleModol.currentRow.PolygonStyle);
                        $rootScope.addStyleModol.tableDataSource[i].data[sc] = angular.copy(JSON.parse($rootScope.addStyleModol.currentRow.PolygonStyle));
                        console.log($rootScope.addStyleModol.tableDataSource[i]);
                        break;
                    }
                }
                a.close();
            },
            //模板
            html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            ' <div class="form-group" style="padding-left:15px;padding-right:15px;">' +
            '    <!--单一样式 之  点-->' +
            '    <div ng-show="(popwinmodal.styleType === \'点\')||(popwinmodal.styleType === \'Point\')">' +
            '        <div style="margin-bottom: 10px;width: 100%;">' +
            '            <div class="col-sm-1">' +
            '                <div style="float: right; padding-right: 10px; padding-left: 0; width: 100%;">' +
            '                    <div class="checkbox clip-check check-primary check-column">' +
            '                        <input type="checkbox" id="style_mark1_dian1" ng-checked="popwinmodal.isStylePointChecked1" ng-click="popwinmodal.onStylePointChecked1()">' +
            '                        <label for="style_mark1_dian1" translate="views.Style.colour">颜色</label>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div class="col-sm-11">' +
            '                <div ng-show="!popwinmodal.isStylePointChecked1" style="right: -5px; width: 98%; height: 100%; cursor:not-allowed; position: absolute; z-index: 3; opacity: 0.5; background-color: rgb(252, 253, 254)"></div>' +
            '                <div style="margin-bottom: 10px; width: 100%;">' +
            '                    <div class="col-sm-6">' +
            '                        <div class="col-sm-3" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px;padding-left: 15px;" translate="views.Style.FillColor">填充色</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9" style="padding-left: 0;">' +
            '                            <mic-spectrum rgb-value="popwinmodal.pointRgbValue"></mic-spectrum>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div class="col-sm-6">' +
            '                        <div class="col-sm-3" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px;" translate="views.Style.Graphics">图形</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9">' +
            '                            <ui-select class="ui-select-width" ng-model="popwinmodal.pointGraphicalSelect.selected" search-enabled="0" theme="select2" title="{{\'views.Style.Choose\'|translate}}">' +
            '                                <ui-select-match placeholder="{{\'views.Style.Choose\'|translate}}"><img style="width:20px;height:20px;" src="{{\'App/Main/assets/images/\'+$select.selected.value+\'.png\'}}"></img></ui-select-match>' +
            '                                <ui-select-choices class="ui-select-height" repeat="p in popwinmodal.pointGraphicalComData">' +
            '                                    <img style="width:20px;height:20px;" src="{{\'App/Main/assets/images/\'+p.value+\'.png\'}}"></img>' +
            '                                </ui-select-choices>' +
            '                            </ui-select>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div style="width: 100%;">' +
            '                    <div class="col-sm-6">' +
            '                        <div class="col-sm-3" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px; padding-left: 15px;" translate="views.Style.size">大小</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9" style="padding-left: 0;">' +
            '                            <num-box max-num="200" min-num="8" precision="0" width="179" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="1" text-model="popwinmodal.pointSize"></num-box>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div class="col-sm-6">' +
            '                        <div class="col-sm-3" style="padding-left: 0;">' +
            '                            <label class="font-title-little" style="padding-top: 8px;" translate="views.Style.transparency">透明度</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9">' +
            '                            <num-box max-num="1.0" min-num="0.1" precision="1" width="179" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="0.1" text-model="popwinmodal.pointDiaphaneity"></num-box>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div style="clear: both;"></div>' +
            '        </div>' +
            '        <div style="margin-bottom: 10px; width: 100%;">' +
            '            <div class="col-sm-1">' +
            '                <div style="float: right; padding-right: 10px; padding-left: 0px; width: 100%;">' +
            '                    <div class="checkbox clip-check check-primary check-column">' +
            '                        <input type="checkbox" id="style_mark1_dian2" ng-checked="popwinmodal.isStylePointChecked2" ng-click="popwinmodal.onStylePointChecked2()">' +
            '                        <label for="style_mark1_dian2" translate="views.Style.image">图片</label>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div class="col-sm-11">' +
            '                <div ng-show="!popwinmodal.isStylePointChecked2"style="right: -5px; width: 98%; height: 100%; cursor: not-allowed; position: absolute; z-index: 1; opacity: 0.5; background-color: rgb(252, 253, 254); "></div>' +
            '                <div class="col-sm-6">' +
            '                    <div class="col-sm-3" style="padding-left: 0;">' +
            '                        <label class="font-title-little" style="padding-top: 8px; padding-left: 15px;" translate="views.Style.icon">图标</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9" style="padding-left: 0px;">' +
            '                        <img style="width:32px;height:32px;cursor:pointer;" src="{{popwinmodal.selectedPointImgInfo.path?popwinmodal.selectedPointImgInfo.path:\'App/Main/assets/images/new-defult.png\'}}" ng-click="popwinmodal.onSelectImg(\'列表单条符号设置点图标\')" />' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-6"></div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div style="clear: both;"></div>' +
            '        </div>' +
            '    </div>' +
            '    <!--单一样式 之  线-->' +
            '    <div ng-show="(popwinmodal.styleType === \'线\')||(popwinmodal.styleType === \'Line\')">' +
            '        <fieldset style="width: 100%; margin: 0; padding: 0;">' +
            '            <legend></legend>' +
            '            <div style="width: 100%; margin-top: 10px;margin-bottom: 10px;">' +
            '                <div class="col-sm-6">' +
            '                    <div class="col-sm-2">' +
            '                        <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.LineColor">线颜色</label>' +
            '                    </div>' +
            '                    <div class="col-sm-10" style="padding-left: 0px;">' +
            '                        <mic-spectrum rgb-value="popwinmodal.lineRgbValue"></mic-spectrum>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-6">' +
            '                    <div class="col-sm-2">' +
            '                        <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.Linear">线型</label>' +
            '                    </div>' +
            '                    <div class="col-sm-10" style="padding-left: 0px;">' +
            '                        <ui-select style="width:206px;" class="ui-select-width" ng-model="popwinmodal.lineLinearSelect.selected" search-enabled="0" theme="select2" title="{{\'views.Style.Choose\'|translate}}">' +
            '                            <ui-select-match placeholder="{{\'views.Style.Choose\'|translate}}"><img src="{{\'App/Main/assets/images/\'+$select.selected.name+\'.png\'}}" style="position:absolute;bottom:5px;width:150px;"></img></ui-select-match>' +
            '                            <ui-select-choices class="ui-select-height" repeat="p in popwinmodal.lineLinearComData">' +
            '                                <img src="{{\'App/Main/assets/images/\'+p.name+\'.png\'}}"></img>' +
            '                            </ui-select-choices>' +
            '                        </ui-select>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div style="width: 100%; margin-bottom: 10px;">' +
            '                <div class="col-sm-6">' +
            '                    <div class="col-sm-2">' +
            '                        <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.transparency">透明度</label>' +
            '                    </div>' +
            '                    <div class="col-sm-10" style="padding-left: 0px;">' +
            '                        <num-box max-num="1.0" min-num="0.1" precision="1" width="179" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="0.1" text-model="popwinmodal.lineDiaphaneity"></num-box>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-6">' +
            '                    <div class="col-sm-2">' +
            '                        <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.LineWidth">线宽度</label>' +
            '                    </div>' +
            '                    <div class="col-sm-10" style="padding-left: 0px;">' +
            '                        <num-box max-num="20" min-num="1" precision="0" width="185" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="1" text-model="popwinmodal.lineLinewidth"></num-box>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '        </fieldset>' +
            '    </div>' +
            '    <!--单一样式 之  面-->' +
            '    <div ng-show="(popwinmodal.styleType === \'面\')||(popwinmodal.styleType === \'Polygon\')">' +
            '        <fieldset style="width: 100%; margin: 0px; padding: 10px;">' +
            '            <legend>边框</legend>' +
            '            <div style="width: 100%; margin-bottom: 10px; margin-top: 10px;">' +
            '                <div class="col-sm-1">' +
            '                    <div class="checkbox clip-check check-primary check-column">' +
            '                        <input type="checkbox" id="style_mark11_ch1" ng-checked="popwinmodal.isPlaneFrame" ng-click="popwinmodal.isPlaneFrame =! popwinmodal.isPlaneFrame;">' +
            '                        <label for="style_mark11_ch1" translate="views.Style.Frame">边框</label>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-6">' +
            '                    <div class="col-sm-3">' +
            '                        <label class="font-title-little" style="padding-top: 6px; padding-left: 5px; margin-left: 5px;" translate="views.Style.LineColor">线颜色</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9">' +
            '                        <mic-spectrum rgb-value="popwinmodal.frameRgbValue"></mic-spectrum>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-5" style="padding-left: 0;">' +
            '                    <div class="col-sm-3">' +
            '                        <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.Linear">线型</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9" style="padding-left: 0px;">' +
            '                        <ui-select style="width:206px" class="ui-select-width" ng-model="popwinmodal.planeLinearSelect.selected" search-enabled="0" theme="select2" title="{{\'views.Style.Choose\'|translate}}">' +
            '                            <ui-select-match placeholder="{{\'views.Style.Choose\'|translate}}"><img src="{{\'App/Main/assets/images/\'+$select.selected.name+\'.png\'}}"  style="position:absolute;bottom:5px;width:150px;"></img></ui-select-match>' +
            '                            <ui-select-choices class="ui-select-height" repeat="p in popwinmodal.planeLinearComData">' +
            '                                <img src="{{\'App/Main/assets/images/\'+p.name+\'.png\'}}"></img>' +
            '                            </ui-select-choices>' +
            '                        </ui-select>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div style="width: 100%; margin-bottom: 0px;">' +
            '                <div class="col-sm-1"></div>' +
            '                <div class="col-sm-6">' +
            '                    <div class="col-sm-3">' +
            '                        <label class="font-title-little" style="padding-top: 6px; padding-left: 0;" translate="views.Style.transparency">透明度</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9">' +
            '                        <num-box max-num="1.0" min-num="0.1" precision="1" width="179" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="0.1" text-model="popwinmodal.planeFrameDiaphaneity"></num-box>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="col-sm-5" style="padding-left: 0;">' +
            '                    <div class="col-sm-3" style="padding-left: 0;">' +
            '                        <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.LineWidth">线宽度</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9" style="padding-left: 0;">' +
            '                        <num-box max-num="20" min-num="1" precision="0" width="185" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="1" text-model="popwinmodal.planeLinewidth"></num-box>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '        </fieldset>' +
            '        <fieldset style="width: 100%; margin: 15px 0px 0px 0px; padding: 0 10px;">' +
            '            <legend translate="views.Style.Fill">填充</legend>' +
            '            <div style="width: 100%; margin-top: 10px;">' +
            '                <div class="col-sm-1">' +
            '                    <div style="float: right; padding-right: 10px; padding-left: 0px; width: 100%;">' +
            '                        <div class="checkbox clip-check check-primary check-column">' +
            '                            <input type="checkbox" id="style_mark12_ch2" ng-checked="popwinmodal.isPlaneColor" ng-click="popwinmodal.onStylePlaneChecked1()">' +
            '                            <label for="style_mark12_ch2" translate="views.Style.colour">颜色</label>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div class="col-sm-11" style="margin-top: 3px;">' +
            '                    <div ng-show="!popwinmodal.isPlaneColor" style="right: -5px; width: 98%; height: 100%;cursor:not-allowed; position: absolute; z-index: 3; opacity: 0.5; background-color: rgb(252, 253, 254)"></div>' +
            '                    <div style="margin-bottom: 10px; width: 100%;">' +
            '                        <div class="col-sm-6">' +
            '                            <div class="col-sm-3" style="padding-left: 0px;">' +
            '                                <label class="font-title-little" style="padding-top: 4px;padding-left: 15px;" translate="views.Style.FillColor">填充色</label>' +
            '                            </div>' +
            '                            <div class="col-sm-9">' +
            '                                <mic-spectrum rgb-value="popwinmodal.planeRgbValue"></mic-spectrum>' +
            '                            </div>' +
            '                        </div>' +
            '                        <div class="col-sm-6" style="padding-left: 17px;">' +
            '                            <div class="col-sm-3" style="padding-left: 0;">' +
            '                                <label class="font-title-little" style="padding-top: 4px;" translate="views.Style.transparency">透明度</label>' +
            '                            </div>' +
            '                            <div class="col-sm-9">' +
            '                                <num-box max-num="1.0" min-num="0.1" precision="1" width="185" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="0.1" text-model="popwinmodal.planeFillDiaphaneity"></num-box>' +
            '                            </div>' +
            '                        </div>' +
            '                        <div style="clear: both;"></div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '            <div style="width: 100%; margin-bottom: 10px;">' +
            '                <div class="col-sm-1">' +
            '                    <div style="float: right; padding-right: 10px; padding-left: 0px; width: 100%;">' +
            '                        <div class="checkbox clip-check check-primary check-column">' +
            '                            <input type="checkbox" id="style_mark11_ch3" ng-checked="popwinmodal.isPlaneFillPicture" ng-click="popwinmodal.onStylePlaneChecked2()">' +
            '                            <label for="style_mark11_ch3" translate="views.Style.image">图片</label>' +
            '                        </div>' +
            '                    </div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div class="col-sm-11">' +
            '                    <div ng-show="!popwinmodal.isPlaneFillPicture" style="right: -5px; width: 98%; height: 100%; cursor: not-allowed; position: absolute; z-index: 1; opacity: 0.5; background-color: rgb(252, 253, 254); "></div>' +
            '                    <div class="col-sm-6">' +
            '                        <div class="col-sm-3">' +
            '                            <label class="font-title-little" style="padding-top: 8px; padding-left: 15px;" translate="views.Style.icon">图标</label>' +
            '                        </div>' +
            '                        <div class="col-sm-9">' +
            '                            <img style="width: 32px; height: 32px; cursor: pointer;" src="{{popwinmodal.selectedPlaneImgInfo.path?popwinmodal.selectedPlaneImgInfo.path:\'App/Main/assets/images/new-defult.png\'}}" ng-click="popwinmodal.onSelectImg(\'列表单条符号设置面图标\')" />' +
            '                        </div>' +
            '                    </div>' +
            '                    <div class="col-sm-6"></div>' +
            '                    <div style="clear: both;"></div>' +
            '                </div>' +
            '                <div style="clear: both;"></div>' +
            '            </div>' +
            '        </fieldset>' +
            '    </div>' +
            '</div>' +
            '<div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '    <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.sure">确定</button>' +
            '    <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()" translate="setting.cancel">取消</a>' +
            '</div>' +
            '</form>',
        };
        //样式管理-------------预设符号和单条符号信息编辑弹框-----------------------------end


        //样式管理之选择图层
        $rootScope.styleSelectLayerModal = {
            title: '',
            pageingB: { pageIndex: 1, pageSize: 10, pageCounts: 0, maxSize: 2 },
            checkedData: [],
            layersA: [],
            layerName: '',
            tagSelectedB: {},
            typeSelectedB: {},
            tagTreeData: [],
            typeTreeData: [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }],
            showtab1: false,
            openWin: function () {
                $rootScope.styleSelectLayerModal.title = $filter('translate')('views.Style.SelectLayer');
                $rootScope.styleSelectLayerModal.typeTreeData[0].label = $filter('translate')('views.Layer.Query.class.all');
            },
            selectTabB: function (state) {
                //console.log(state)
                $rootScope.styleSelectLayerModal.showtab1 = state;
                $rootScope.styleSelectLayerModal.treeQueryCtrl3.select_branch($rootScope.styleSelectLayerModal.typeTreeData[0]);
                $rootScope.styleSelectLayerModal.onTypeSelected($rootScope.styleSelectLayerModal.typeTreeData[0]);
            },

            onTypeSelected: function (node) {
                //console.log(node);
                $rootScope.styleSelectLayerModal.pageingB.pageIndex = 1;
                $rootScope.styleSelectLayerModal.layerName = "";
                getLayersLists(node.id, '', '', '', $rootScope.styleSelectLayerModal.pageingB.pageSize, $rootScope.styleSelectLayerModal.pageingB.pageIndex);
                $rootScope.styleSelectLayerModal.treeQueryCtrl4.select_branch();
            },
            onTagSelected: function (node) {
                //console.log(node);
                $rootScope.styleSelectLayerModal.pageingB.pageIndex = 1;
                $rootScope.styleSelectLayerModal.layerName = "";
                getLayersLists('', node.id, '', '', $rootScope.styleSelectLayerModal.pageingB.pageSize, $rootScope.styleSelectLayerModal.pageingB.pageIndex);
            },


            //分页点击事件回调函数
            pageChangedB: function () {
                var typeBId = typeof ($rootScope.styleSelectLayerModal.typeSelectedB.id) === "undefined" ? '' : $rootScope.styleSelectLayerModal.typeSelectedB.id;
                var tagBId = typeof ($rootScope.styleSelectLayerModal.tagSelectedB.id) === "undefined" ? '' : $rootScope.styleSelectLayerModal.tagSelectedB.id;

                getLayersLists(typeBId, tagBId, $rootScope.styleSelectLayerModal.layerName, '', $rootScope.styleSelectLayerModal.pageingB.pageSize, $rootScope.styleSelectLayerModal.pageingB.pageIndex);
            },


            searchLayerByName: function () {
                $rootScope.styleSelectLayerModal.pageingB.pageIndex = 1;
                var typeBId = typeof ($rootScope.styleSelectLayerModal.typeSelectedB.id) === "undefined" ? '' : $rootScope.styleSelectLayerModal.typeSelectedB.id;
                var tagBId = typeof ($rootScope.styleSelectLayerModal.tagSelectedB.id) === "undefined" ? '' : $rootScope.styleSelectLayerModal.tagSelectedB.id;
                getLayersLists(typeBId, tagBId, $rootScope.styleSelectLayerModal.layerName, '', $rootScope.styleSelectLayerModal.pageingB.pageSize, $rootScope.styleSelectLayerModal.pageingB.pageIndex);
            },
            reset: function () {
                $rootScope.styleSelectLayerModal.pageingB = { pageIndex: 1, pageSize: 10, pageCounts: 0, maxSize: 2 };
                $rootScope.styleSelectLayerModal.checkedData = [];
            },
            //事件
            submitForm: function (a, b) {
                if ($rootScope.styleSelectLayerModal.checkedData.length === 0) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.SelectLayer'), '', 'warning', '#007AFF');
                    return;
                }
                $rootScope.addStyleModol.currentLayer = angular.copy($rootScope.styleSelectLayerModal.checkedData[0]);
                $rootScope.addStyleModol.XMLviewName = $rootScope.addStyleModol.currentLayer.layerName;
                $rootScope.addStyleModol.XMLviewNameTxt = $rootScope.addStyleModol.XMLviewName ? $rootScope.addStyleModol.XMLviewName : $filter('translate')('views.Style.SelectLayer');
                if ($rootScope.addStyleModol.isCurrentLayer) {
                    preStyle($rootScope.addStyleModol.styleName, $rootScope.addStyleModol.currentLayer, $rootScope.addStyleModol.XMLContentView);
                    a.close();
                    $rootScope.styleSelectLayerModal.reset();
                    return;
                }
                layerField.getDetailByLayerID($rootScope.styleSelectLayerModal.checkedData[0].id).success(function (data) {

                    //console.log('Debug: xxxxy  ',data );
                    $rootScope.addStyleModol.selectedLayerInfo = angular.copy($rootScope.styleSelectLayerModal.checkedData[0]);
                    $rootScope.addStyleModol.selectedLayerInfoTxt = $rootScope.addStyleModol.selectedLayerInfo.layerName ? $rootScope.addStyleModol.selectedLayerInfo.layerName : $filter('translate')('views.Style.SelectLayer');
                    $rootScope.addStyleModol.fieldTypeComData = data.items;
                    $rootScope.addStyleModol.fieldTypeSelect.selected = angular.copy(data.items[0]);
                    a.close();
                    $rootScope.styleSelectLayerModal.reset();
                });
            },
            //模板
            html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            '<div class="form-group">' +
            '	<div class="col-sm-12 container-col-stretch" style="padding: 10px !important;">' +
            '	    <div class="col-sm-5 container-col-stretch">' +
            '		<div class="panel panel-white panel-bottom0">' +
            '		    <div class="panel-heading panel-head border-bottom">' +
            '			<span class="font-title-middle" translate="views.Style.QueryConditions">查询条件</span>' +
            '		    </div>' +
            '		    <div class="panel-body panel-fixed self_bottom">' +
            '			<div style="padding-bottom: 10px; ">' +
            '			    <tabset type="roundline" justified="true" class="tabbable" no-tab-content>' +
            '				<tab class="font-content-middle" heading="{{\'views.Style.CategorySearch\'|translate}}" select="popwinmodal.selectTabB(true)"></tab>' +
            '				<tab class="font-content-middle" heading="{{\'views.Style.Tags\'|translate}}" select="popwinmodal.selectTabB(false)"></tab>' +
            '			    </tabset>' +
            '			    <div style="height: 349px; overflow-y: auto;">' +
            '				<div ng-init="popwinmodal.showtab1=true" ng-show="popwinmodal.showtab1===true">' +
            '				    <abn-tree class="font-title-btn" icon-leaf="" tree-data="popwinmodal.typeTreeData" expand-level="2" selected-data="popwinmodal.typeSelectedB" on-select="popwinmodal.onTypeSelected" tree-control="popwinmodal.treeQueryCtrl3"></abn-tree>' +//initial-selection="{{popwinmodal.treedata1[0].label}}"
            '				</div>' +
            '				<div ng-show="popwinmodal.showtab1===false">' +
            '				    <abn-tree class="font-title-btn" icon-leaf="" tree-data="popwinmodal.tagTreeData" expand-level="2" selected-data="popwinmodal.tagSelectedB" on-select="popwinmodal.onTagSelected" tree-control="popwinmodal.treeQueryCtrl4"></abn-tree>' +
            '				</div>' +
            '			    </div>' +
            '			</div>' +
            '		    </div>' +
            '		</div>' +
            '	    </div><style>.mystyleelet .mic-tables thead tr th:first-child div{display:none}</style>' +
            '	    <div class="col-sm-7 container-col-stretch mystyleelet" style="padding-left: 10px !important;">' +
            '		<input type="text" style="height: 34px; width: 214px; margin:0 10px 10px 0;" ng-model="popwinmodal.layerName" />' +
            '		<a class="btn btn-primary btn-o font-title-little" href="javascript:;" ng-click="popwinmodal.searchLayerByName()" translate="views.Style.search">搜索</a>' +
            '		<mic-data-tables class="layerClass form-table-checkbox font-content-small" pageing="popwinmodal.pageingB" on-page-change="popwinmodal.pageChangedB" checkable checked-data="popwinmodal.checkedData" table-bordered tbody-height="373" thead="{{\'views.Style.layername\'|translate}}" td-params="{\'layerName\':{\'type\':\'span\',\'isclick\':false,\'isedit\':false}}" datasets="popwinmodal.layersA"></mic-data-tables>' +
            '	    </div>' +
            '	</div>' +

            '</div>' +
            '    <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '        <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.sure">确定</button>' +
            '        <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()" translate="setting.cancel">取消</a>' +
            '    </div>' +
            '</form>',
        };

        $rootScope.$watch(function () {
            if ($rootScope.styleSelectLayerModal.checkedData.length > 1) {
                $rootScope.styleSelectLayerModal.checkedData.splice(0, 1);
                return $rootScope.styleSelectLayerModal.checkedData[0].id;
            }
            return false;
        }, function (val, old) {
            if (val === false) { return; }
            for (var i in $rootScope.styleSelectLayerModal.layersA) {
                if ($rootScope.styleSelectLayerModal.layersA[i].id != val) {
                    $rootScope.styleSelectLayerModal.layersA[i].ischecked = false;
                }
            }

        });
        //分类列表--图层
        dataType.getAllListByDataType(layerTypeID).success(function (data, statues) {
            var arr = [];
            for (var i in data.items) {
                var tempTypeData = {};
                tempTypeData.id = data.items[i].id;
                tempTypeData.dictCodeID = data.items[i].dictCodeID;
                tempTypeData.label = data.items[i].typeName;
                tempTypeData.typeDesc = data.items[i].typeDesc;
                tempTypeData.parentID = data.items[i].parentID;
                tempTypeData.children = [];
                arr = arr.concat(tempTypeData);
            }
            arr.forEach(function (each) {
                if (each.parentID == 0) {
                    $rootScope.styleSelectLayerModal.typeTreeData[0].children.push(each);
                }
            });
            $rootScope.styleSelectLayerModal.typeTreeData[0].children.forEach(function (each) {
                each.children = [];
                arr.forEach(function (item) {
                    if (item.parentID == each.id) {
                        each.children.push(item);
                    }
                });
            });
        });

        //标签列表--图层
        dataTag.getAllListByDataType(layerTypeID).success(function (data, statues) {
            for (var i in data.items) {
                var tempTagData = {};
                tempTagData.id = data.items[i].id;
                tempTagData.dictCodeID = data.items[i].dictCodeID;
                tempTagData.label = data.items[i].tagName;
                tempTagData.tagDesc = data.items[i].tagDesc;
                tempTagData.children = [];

                $rootScope.styleSelectLayerModal.tagTreeData.push(tempTagData);
            }
        });

        //$rootScope.$watch(function () {
        //    return $rootScope.addStyleModol.isPlaneColor;
        //}, function (val) {
        //    console.log('Debug: isPlaneColor  ', val);
        //});

        //样式管理之选择图片
        $rootScope.styleForSelectImgModal = {
            type: null,//区分提交数据时给谁赋值
            imgInfo: null,
            imgItem: [],
            angle: 0,//角度
            size: 0,//大小
            title:'',
            typeFolderSelect: {},
            typeFolderComboData: [],// '线图片',
            openWin: function () {
                $rootScope.styleForSelectImgModal.title = $filter('translate')('views.Style.SymbolSelector')
            },
            submitForm: function (a, b) {

                console.log('Debug:  1111 ', $rootScope.styleForSelectImgModal.imgInfo, '    ', $rootScope.styleForSelectImgModal.type);

                if ($rootScope.styleForSelectImgModal.type === '选择点图标') {
                    //单一样式  点图片 赋值
                    $rootScope.addStyleModol.selectedPointImgInfo = angular.copy($rootScope.styleForSelectImgModal.imgInfo);
                    $rootScope.addStyleModol.pointImgSize = angular.copy($rootScope.styleForSelectImgModal.size);
                    $rootScope.addStyleModol.pointImgRotation = angular.copy($rootScope.styleForSelectImgModal.angle);
                } else if ($rootScope.styleForSelectImgModal.type === '选择面图标') {
                    //单一样式  面图片 赋值
                    $rootScope.addStyleModol.selectedPlaneImgInfo = angular.copy($rootScope.styleForSelectImgModal.imgInfo);
                    $rootScope.addStyleModol.planeImgSize = angular.copy($rootScope.styleForSelectImgModal.size);
                    $rootScope.addStyleModol.planeImgRotation = angular.copy($rootScope.styleForSelectImgModal.angle);
                } else if ($rootScope.styleForSelectImgModal.type === '图片管理') {
                    //图片管理往xml插入值 
                    editor1.insert($rootScope.styleForSelectImgModal.imgInfo.path);//??????大小和角度 没插入
                } else if ($rootScope.styleForSelectImgModal.type === '预设符号点图标') {
                    //预设符号点图标赋值
                    $rootScope.stylePresetMarkModal.selectedPointImgInfo = angular.copy($rootScope.styleForSelectImgModal.imgInfo);
                    $rootScope.stylePresetMarkModal.pointImgSize = angular.copy($rootScope.styleForSelectImgModal.size);
                    $rootScope.stylePresetMarkModal.pointImgRotation = angular.copy($rootScope.styleForSelectImgModal.angle);
                } else if ($rootScope.styleForSelectImgModal.type === '预设符号面图标') {
                    //预设符号面图标赋值
                    $rootScope.stylePresetMarkModal.selectedPlaneImgInfo = angular.copy($rootScope.styleForSelectImgModal.imgInfo);
                    $rootScope.stylePresetMarkModal.planeImgSize = angular.copy($rootScope.styleForSelectImgModal.size);
                    $rootScope.stylePresetMarkModal.planeImgRotation = angular.copy($rootScope.styleForSelectImgModal.angle);
                } else if ($rootScope.styleForSelectImgModal.type === '列表单条符号设置点图标') {
                    //列表单条符号设置点图标赋值
                    $rootScope.styleSingleMarkModal.selectedPointImgInfo = angular.copy($rootScope.styleForSelectImgModal.imgInfo);
                    $rootScope.styleSingleMarkModal.pointImgSize = angular.copy($rootScope.styleForSelectImgModal.size);
                    $rootScope.styleSingleMarkModal.pointImgRotation = angular.copy($rootScope.styleForSelectImgModal.angle);
                } else if ($rootScope.styleForSelectImgModal.type === '列表单条符号设置面图标') {
                    //列表单条符号设置面图标赋值
                    $rootScope.styleSingleMarkModal.selectedPlaneImgInfo = angular.copy($rootScope.styleForSelectImgModal.imgInfo);
                    $rootScope.styleSingleMarkModal.planeImgSize = angular.copy($rootScope.styleForSelectImgModal.size);
                    $rootScope.styleSingleMarkModal.planeImgRotation = angular.copy($rootScope.styleForSelectImgModal.angle);
                }
                a.close();
            },
            //上传
            formDataUpload: [],
            uploadSuccess: function (data) {
                $rootScope.styleForSelectImgModal.onFolderSelect();
            },

            onFolderSelect: function () {
                getAllFiles($rootScope.styleForSelectImgModal.typeFolderSelect.selected);
            },
            //添加文件夹开始弹框
            addFolder: function () {
                $rootScope.addFolderModal.addFolderModalOpenFun();
                $rootScope.addFolderModal.title = $filter('translate')('views.Style.AddFolder');
            },
            //删除图片
            delImgs: function (img) {
                $rootScope.alertConfirm($filter('translate')('views.Style.alertFun.delete'), $filter('translate')('views.Style.alertFun.ConfirmDelete'), "warning", function () {

                    //console.log('Debug:imgimgimgimg   ', img);

                    dataStyle.deleteFile($rootScope.styleForSelectImgModal.typeFolderSelect.selected, img.name).success(function (bo) {
                        console.log('Debug:bool   ', bo);
                        if (bo) {
                            $rootScope.styleForSelectImgModal.onFolderSelect();
                            $rootScope.alertFun($filter('translate')('views.Style.alertFun.DeleteSuccessfully'), '', 'success', '#007AFF');
                        } else {
                            $rootScope.alertFun($filter('translate')('views.Style.alertFun.FailedDelete'), '', 'error', '#007AFF');
                        }
                    }).error(function (data, status) {
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.error'), data.message, 'error', '#007AFF');
                    });
                });
            },
            //预览
            toView: function (info) {
                $rootScope.styleForSelectImgModal.imgInfo = info;
                //$rootScope.styleForSelectImgModal.formDataUpload = [{ folder: $rootScope.styleForSelectImgModal.typeFolderSelect.selected }];
            },
            //设置大小
            toSize: function (info) {
                $rootScope.styleForSelectImgModal.size = info;
                //$rootScope.styleForSelectImgModal.formDataUpload = [{ folder: $rootScope.styleForSelectImgModal.typeFolderSelect.selected }];
            },
            //旋转角度
            toDegrees: function (info) {
                $rootScope.styleForSelectImgModal.angle = info;
                //$rootScope.styleForSelectImgModal.formDataUpload = [{ folder: $rootScope.styleForSelectImgModal.typeFolderSelect.selected }];
            },
            html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            ' <div class="form-group" style="padding-left:15px;padding-right:15px;">' +
            '         <div class="col-sm-8" style="padding:0 7px 0 0;">' +
            '             <div style="width:100%">' +
            '		            <div class="col-sm-8" style="padding-left:0px">' +
            '		                <ui-select style="width:100%" class="ui-select-width" ng-model="popwinmodal.typeFolderSelect.selected" on-select="popwinmodal.onFolderSelect()" search-enabled="0" theme="select2" title="{{\'views.Style.Choose\'|translate}}">' +
            '			                <ui-select-match placeholder="{{\'views.Style.Choose\'|translate}}">{{$select.selected}}</ui-select-match>' +
            '			                <ui-select-choices style="height:120px" class="ui-select-height" repeat="p in popwinmodal.typeFolderComboData">' +
            '			                	<div ng-bind-html="p"></div>' +
            '			                </ui-select-choices>' +
            '		                </ui-select>' +
            '		            </div>' +
            '	        	    <div class="col-sm-4" style="">' +
            '		              <a ng-click="popwinmodal.addFolder()" style="margin-left:15px" href="#"><i style="font-size: 40px;line-height: 0.82;" class="mdi-folder-plus" tooltip="{{\'views.Style.AddFolder\'|translate}}" tooltip-append-to-body></i></a>' +
            '	        	    </div><div style="clear: both;"></div>' +
            '	            </div>' +
            '        	    <div ng-init="tmpimgs={}" style="width:100%;margin-top:10px;border: solid 1px rgba(27, 26, 26, 0.11);overflow: auto;height: 510px; max-height: 510px;">' +

            '        	        <div ng-repeat="m in popwinmodal.imgItem" style="position:relative;display:inline-block;margin-top:15px;margin-left:15px;width:32px;height:35px;"><img ng-style="{\'border\':(popwinmodal.imgInfo.name === m.name?\'3px solid #007AFF\':\'3px solid #fff\')}" ng-mouseover="tmpimgs[m.name]=true" ng-mouseout="tmpimgs[m.name]=false" ng-click="popwinmodal.toView(m)" src="{{m.path}}" style="width:35px;height:35px;" /><!--<div style="width:50px;height:50px">{{m.name}}</div>--><a style="position:absolute;right:2px;top:1px;color:red;" class="my-pop-win-close" href="#" ng-click="popwinmodal.delImgs(m)"><i ng-mouseout="tmpimgs[m.name]=false" ng-mouseover="tmpimgs[m.name]=true" ng-show="tmpimgs[m.name]" class="ti-close" tooltip="{{\'setting.delete\'|translate}}" tooltip-append-to-body></i></a></div>' +

            '        	    <div style="clear: both;"></div></div>' +
            '         </div>' +
            '         <div class="col-sm-4" style="padding:0 0 0 7px;">' +
            '             <fieldset style="width:100%;">' +
            '                <legend translate="views.Style.Preview">预览</legend>' +
            '                <img src="{{popwinmodal.imgInfo.path}}" ng-init="mysize=8;degrees=0" ng-style="{\'width\':mysize,\'height\':mysize,\'-webkit-transform\': \'rotate(\' + degrees + \'deg)\',\'-moz-transform\': \'rotate(\' + degrees + \'deg)\',\'-ms-transform\': \'rotate(\' + degrees + \'deg)\',\'-o-transform\': \'rotate(\' + degrees + \'deg)\',\'transform\': \'rotate(\' + degrees + \'deg)\'}" />' +
            '                <div style="float: right; width: 100%;margin-top:10px;">' +
            '                    <div class="col-sm-3">' +
            '                        <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.size">大小</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9" style="padding-right: 14px;">' +
            '                     <num-box max-num="64" min-num="8" precision="0" width="120" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="1" text-model="mysize" ng-click="popwinmodal.toSize(mysize)"></num-box>' +
            '                    </div>' +
            '                </div>' +
            '                <div style="float: right; width: 100%;margin-top:10px;">' +
            '                    <div class="col-sm-3">' +
            '                        <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.angle">角度</label>' +
            '                    </div>' +
            '                    <div class="col-sm-9" style="padding-right: 14px;">' +
            '                     <num-box max-num="360" min-num="0" precision="0" width="120" text-placeholder="{{\'views.Style.numeral\'|translate}}" show-button="true" step="1" text-model="degrees" ng-click="popwinmodal.toDegrees(degrees)"></num-box>' +
            '                    </div>' +
            '                </div>' +
            '             </fieldset>' +
            '             <btn-upload multiple upload-url="\'uploadImage\'" form-data="popwinmodal.formDataUpload" on-success="popwinmodal.uploadSuccess">{{\'views.Style.AddLocally\'|translate}}</btn-upload> ' +
            '         </div>' +
            ' </div>' +
            '    <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '        <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.sure">确定</button>' +
            '        <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()" translate="setting.cancel">取消</a>' +
            '    </div>' +
            '</form>',
        };
        function getAllFiles(folderName) {
            $rootScope.styleForSelectImgModal.formDataUpload = [{ "folder": folderName }];
            dataStyle.getAllFiles(folderName).success(function (data) {
                //console.log('Debug: 2222  ', data);
                $rootScope.styleForSelectImgModal.imgItem = data;
                $rootScope.styleForSelectImgModal.imgInfo = data[0];
            });
        }
        function getAllFolders() {
            dataStyle.getAllFolders().success(function (data) {
                $rootScope.styleForSelectImgModal.typeFolderComboData = data;
                $rootScope.styleForSelectImgModal.typeFolderSelect.selected = data[0];
                getAllFiles(data[0]);
            });
        }
        getAllFolders();


        //添加文件夹弹框
        $rootScope.addFolderModal = {
            title:'',
            submitForm: function (a, b) {
                if (!$rootScope.addFolderModal.folderName) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.FolderEmpty'), '', 'warning', '#007AFF');
                    return;
                }
                dataStyle.addFolder($rootScope.addFolderModal.folderName).success(function (bo) {
                    console.log('Debug:  addFolder  bo   ', bo);
                    if (!bo) {
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.FailedAddFolder'), '', 'error', '#007AFF');
                    }
                    getAllFolders();
                    a.close();
                });
            },
            html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            ' <div class="form-group" style="padding-left:15px;padding-right:15px;">' +
            '    <div class="col-sm-6">' +
            '        <div style="float: right; width: 100%;">' +
            '            <div class="col-sm-3">' +
            '                <label class="font-title-little" style="padding-top: 6px;" translate="views.Style.FolderName">文件夹名称<span class="symbol required"></span></label>' +
            '            </div>' +
            '            <div class="col-sm-9" style="padding-right: 14px;">' +
            '                <input type="text" style="width: 100%; height: 34px;" ng-model="popwinmodal.folderName" />' +
            '            </div>' +
            '        </div>' +
            '    </div>' +
            ' </div>' +
            '    <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '        <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.submit">提交</button>' +
            '        <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()" translate="setting.cancel">取消</a>' +
            '    </div>' +
            '</form>',
        };

        //切换服务
        $rootScope.layers = '';
        var legend = GeoServerWmsUrl + '?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=20&HEIGHT=20&LAYER=' + WorkSpace + ':';
        $rootScope.changeMap = function (item) {
            var _name = WorkSpace + ':' + item.layerAttrTable;
            $timeout(function () {
                if (!!$rootScope.layers) {
                    $rootScope.addStyleModol.removeLayer($rootScope.layers);
                }
                $rootScope.layers = newLocalTilesByWMS(GeoServerUrl + '/wms', _name, 'image/png');
                $rootScope.layers.getSource().updateParams({ "_": new Date().getTime() + Math.random() });
                $rootScope.layers.setZIndex(55);
                var bounds = [item.minX, item.minY, item.maxX, item.maxY];//范围
                var map = $rootScope.addStyleModol.addLayer($rootScope.layers, bounds);
                map.updateSize();
                map.getView().fit(bounds, map.getSize());
            });
        };

        //code editor1
        var editor1, editor2;
        ace.require("ace/ext/language_tools");
        function init_ace() {
            $timeout(function () {
                if ($document.find('#editor1').length === 0) {
                    init_ace();
                    return;
                }
                editor1 = ace.edit("editor1");
                editor1.session.setMode("ace/mode/xml");
                editor1.setTheme("ace/theme/tomorrow");
                editor2 = ace.edit('editor2');

                //console.log('Debug:   ', editor2);

                editor2.session.setMode("ace/mode/xml");
                editor2.setTheme("ace/theme/tomorrow");
                editor2.$readOnly = true;
            });
        }

        //新建样式弹窗
        $rootScope.addStyleWin = function (fun, backFun, typeid) {
            styleBackFun = backFun;
            styleInit = fun;
            $rootScope.addStyleModol.title = $filter('translate')('views.Style.addStyle');
            $rootScope.addStyleModol.isPre = false;
            $rootScope.addStyleModol.styleName = '';
            $rootScope.addStyleModol.styleDesc = '';
            $rootScope.addStyleModol.imgUrl = '';
            $rootScope.addStyleModol.typePullDownTreeData = [];
            $rootScope.addStyleModol.typePullDownTreeSelData = {};

            $rootScope.addStyleModol.pageCounts = 0;
            $rootScope.addStyleModol.pageIndex = 1;

            $rootScope.addStyleModol.styleRenderRule = '',//预设符号JSON串   样式渲染预设规则
                $rootScope.addStyleModol.selectedLayerInfo = {},
                $rootScope.addStyleModol.selectedLayerInfoTxt = $rootScope.addStyleModol.selectedLayerInfo.layerName ? $rootScope.addStyleModol.selectedLayerInfo.layerName : $filter('translate')('views.Style.SelectLayer');

                //分类渲染
                $rootScope.addStyleModol.fieldTypeSelect = {},//渲染字段
                $rootScope.addStyleModol.fieldTypeComData = [],//渲染字段下拉
                //$rootScope.styleColorBarSelect = {},//颜色带
                $rootScope.addStyleModol.tableDataSource = [],//颜色表格数据源

                //显示比例尺范围
                $rootScope.addStyleModol.minDistance = '',//最小比例尺
                $rootScope.addStyleModol.maxDistance = '',//最大比例尺
                $rootScope.addStyleModol.styleTypeSelect.selected = $rootScope.addStyleModol.styleTypeComData[0];
            $rootScope.addStyleModol.threeToOne = 1;

            resetModalData($rootScope.addStyleModol);
            resetModalData($rootScope.stylePresetMarkModal);
            resetModalData($rootScope.styleSingleMarkModal);

            if (typeid) {
                //点线面赋值
                for (var i in $rootScope.addStyleModol.styleTypeComData) {
                    if (typeid == $rootScope.addStyleModol.styleTypeComData[i].id) {
                        $rootScope.addStyleModol.styleTypeSelect.selected = $rootScope.addStyleModol.styleTypeComData[i];
                        $rootScope.addStyleModol.onStyleTypeSelected();
                        break;
                    }
                }
                $rootScope.addStyleModol.styleTypeSelectedDisabled = true;
            } else {
                $rootScope.addStyleModol.styleTypeSelectedDisabled = false;
            }

            //查询分类
            $rootScope.addStyleModol.typePullDownTreeData = angular.copy($rootScope.typeTreeData[0].children);
            init_ace();

            $rootScope.choseLayerModol.checkedData = {};

            $rootScope.addStyleModol.addStyle();
        }

        //详情的弹窗方法
        $rootScope.detail = function (tr) {

        }
        //编辑的弹窗方法
        $rootScope.edit = function (tr, fun, backFun) {
            $rootScope.addStyleModol.styleTypeSelectedDisabled = true;
            styleBackFun = backFun;
            styleInit = fun;

            $rootScope.addStyleModol.pageCounts = 0;
            $rootScope.addStyleModol.pageIndex = 1;
            $rootScope.addStyleModol.tableDataSource = [];
            dataStyle.getDetailById(tr.id).success(function (item, b) {

                console.log('Debug: 编辑的弹窗方法11  ', item);

                if (b === 200) {
                    $rootScope.addStyleModol.id = item.id;
                    $rootScope.addStyleModol.title = $filter('translate')('views.Style.EditStyle');
                    $rootScope.addStyleModol.styleName = item.styleName;
                    $rootScope.addStyleModol.styleDesc = item.styleDesc;
                    $rootScope.addStyleModol.typePullDownTreeData = [];
                    $rootScope.addStyleModol.typePullDownTreeSelData = {};
                    $rootScope.addStyleModol.initialTypeTreeData = {};

                    $rootScope.addStyleModol.imgUrl = '';
                    $rootScope.addStyleModol.isPre = true;

                    //console.log('Debug:xxx   ', $rootScope.addStyleModol.styleTypeSelect.selected);

                    //点线面赋值
                    for (var ii in $rootScope.addStyleModol.styleTypeComData) {
                        if (item.styleDataType == $rootScope.addStyleModol.styleTypeComData[ii].id) {
                            $rootScope.addStyleModol.styleTypeSelect.selected = $rootScope.addStyleModol.styleTypeComData[ii];
                            break;
                        }
                    }

                    //单一样式，分类渲染，导入样式三选一赋值
                    $rootScope.addStyleModol.threeToOne = item.styleConfigType ? item.styleConfigType : 1;
                    $rootScope.addStyleModol.selectedLayerInfo = item.layerContent;
                    if ($rootScope.addStyleModol.selectedLayerInfo !== null) {
                        $rootScope.addStyleModol.selectedLayerInfoTxt = $rootScope.addStyleModol.selectedLayerInfo.layerName ? $rootScope.addStyleModol.selectedLayerInfo.layerName : $filter('translate')('views.Style.SelectLayer');
                    }
                    $rootScope.addStyleModol.currentLayer = item.layerContent;
                    $rootScope.addStyleModol.fieldTypeSelect.selected = { id: item.styleRenderField, attributeName: item.styleRenderFieldName, codeName: item.styleRenderFieldName };
                    $rootScope.addStyleModol.PolygonStyle = item.styleRenderRule;
                    $rootScope.addStyleModol.XMLviewName = $rootScope.addStyleModol.currentLayer ? $rootScope.addStyleModol.currentLayer.layerName : "";
                    $rootScope.addStyleModol.XMLviewNameTxt = $rootScope.addStyleModol.XMLviewName ? $rootScope.addStyleModol.XMLviewName : $filter('translate')('views.Style.SelectLayer');

                    var srr = angular.fromJson(item.styleRenderRule);
                    //console.log(srr);
                    if (!!srr) {
                        item.styleInfo = {
                            isOutline: srr.IsOutline,//是否有边框
                            lineWidth: srr.LineWidth,//线宽度
                            outlineColor: srr.OutlineColor,//线条颜色
                            lineTransparency: srr.LineTransparency,//线条透明度
                            fillTransparency: srr.FillTransparency,//填充透明度
                            polygonColor: srr.PolygonColor,//填充颜色
                            isFill: srr.IsFill,//是否填充
                            lineType: srr.LineType,//线型
                            isIcon: srr.IsIcon,//是否填充图片
                            iconPath: srr.IconPath,//图片地址
                            iconSize: srr.IconSize,//图片大小
                            iconRotation: srr.IconRotation,//图片旋转角度
                            pointType: srr.PointType,//图形
                        };
                    }
                    
                    if (item.ruleDatas) {
                        var ruleDatas = [];
                        $.each(item.ruleDatas, function (i, o) {
                            var ps = angular.fromJson(o.polygonStyle);
                            var mark = ps.PolygonColor;
                            ruleDatas.push({ Title: o.title, Value: o.value, Count: o.count, PolygonStyle: o.polygonStyle, mark: mark, GUID: getRandom() });
                        });
                        $rootScope.addStyleModol.tableDataSource = ruleDatas;
                    }

                    //色带赋值
                    for (var iii in $rootScope.styleColorBarComData) {
                        if (item.styleRenderColorBand == $rootScope.styleColorBarComData[iii].colorName) {
                            $rootScope.styleColorBarSelect.selected = $rootScope.styleColorBarComData[iii];
                            break;
                        }
                    }

                    if (item.styleInfo) {
                        toSetModalData($rootScope.addStyleModol, item.styleInfo);
                        toSetModalData($rootScope.stylePresetMarkModal, item.styleInfo);
                        toSetModalData($rootScope.styleSingleMarkModal, item.styleInfo);
                    }

                    $rootScope.addStyleModol.onStyleTypeSelected();

                    //查询分类
                    $rootScope.addStyleModol.typePullDownTreeData = JSON.parse(JSON.stringify($rootScope.typeTreeData[0].children));

                    var breakException = "hasSelected";
                    try {
                        $rootScope.addStyleModol.typePullDownTreeData.forEach(function (each) {
                            if (each.id == item.styleType) {
                                $rootScope.addStyleModol.initialTypeTreeData = angular.copy(each);
                                throw breakException;
                            }

                            var parentNode = each.children;
                            parentNode.forEach(function (a) {
                                if (a.id == item.styleType) {
                                    $rootScope.addStyleModol.initialTypeTreeData = angular.copy(a);
                                    throw breakException;
                                }
                            });
                        });
                    } catch (e) {
                        //console.log(e);
                        //console.log($rootScope.inputModal.initialTypeTreeData);
                    }

                    //preStyle('', $rootScope.addStyleModol.currentLayer, content);

                    $rootScope.choseLayerModol.checkedData = {};
                    $rootScope.addStyleModol.addStyle();
                    init_ace();
                    $timeout(function () {
                        //console.log(editor1);
                        editor1.setValue(item.styleContent);
                        editor1.gotoLine(1);
                    });
                } else {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.EditingFailed'), $filter('translate')('views.Style.alertFun.RequestFailed'), 'warning', '#007AFF');
                }
            });
        }
        $rootScope.typeTreeData = [];
        $rootScope.openAddStyle = function () {
            $rootScope.addStyleModol.XMLviewNameTxt = $rootScope.addStyleModol.XMLviewName ? $rootScope.addStyleModol.XMLviewName : $filter('translate')('views.Style.SelectLayer');
            // console.log('selectedLayerInfo', $rootScope.addStyleModol.selectedLayerInfo.layerName)
            if ($rootScope.addStyleModol.selectedLayerInfo !== null) {
                $rootScope.addStyleModol.selectedLayerInfoTxt = $rootScope.addStyleModol.selectedLayerInfo.layerName ? $rootScope.addStyleModol.selectedLayerInfo.layerName : $filter('translate')('views.Style.SelectLayer');
            } else {
                $rootScope.addStyleModol.selectedLayerInfoTxt = $filter('translate')('views.Style.SelectLayer');
            }           
            // console.log('selectedLayerInfoTxt', $rootScope.addStyleModol.selectedLayerInfoTxt)
        };

        //样式管理弹窗的提交方法
        var styleInit, styleBackFun;
        $rootScope.submitAddStyleForm = function (modalInstance, form) {
            $rootScope.loginOut();
            var content,//xml内容
                _styleRenderRule;//JSON串

            var infos = $rootScope.addStyleModol;
            var styleDataTypeId = infos.styleTypeSelect.selected.id;
            var styleTypeName = infos.styleTypeSelect.selected.codeName;
            var styleInfo = getStyleInfoByCondition(styleTypeName, infos);

            //console.log(infos);

            if (!infos.styleName) {
                $rootScope.alertFun($filter('translate')('views.Style.alertFun.FillName'), '', 'warning', '#007AFF');
                return;
            }
            if (!infos.typePullDownTreeSelData.id) {
                $rootScope.alertFun($filter('translate')('views.Style.alertFun.ChooseType'), '', 'warning', '#007AFF');
                return;
            }

            //分类： 单一样式 1  分类渲染 2    导入样式 3 
            if (infos.threeToOne == 1) {

            } else if (infos.threeToOne == 2) {

                console.log('Debug:   ', infos.tableDataSource);
                if (infos.tableDataSource.length === 0) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.LoadCategory'), '', 'warning', '#007AFF');
                    return;
                }

                _styleRenderRule = toGetStyleRenderRuleJson(styleTypeName, infos);

                console.log('Debug:   ', _styleRenderRule);

            } else if (infos.threeToOne == 3) {
                content = $.trim(editor1.getValue());
                if (!content) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleEmpty'), '', 'warning', '#007AFF');
                    return;
                }
                var xmlError = editor1.env.document.$annotations;
                if (xmlError.length !== 0) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_1') + xmlError[0].row + $filter('translate')('views.Style.alertFun.StyleError_2'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                    return;
                }
                //正则匹配XML
                if (!(/<([^>]+)>([^<>]+)<\/\1>/m.exec(content))) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_4'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                    return;
                }
                if (!matchingXmlHeadAndEnd(content)) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleError_5'), $filter('translate')('views.Style.alertFun.StyleError_3'), 'warning', '#007AFF');
                    return;
                }
            }

            console.log('Debug: styleInfostyleInfostyleInfo  ', infos);

            var dto = {
                Id: infos.id,
                StyleName: infos.styleName,
                StyleDesc: infos.styleDesc,
                StyleContent: content,
                StyleType: infos.typePullDownTreeSelData.id,
                CreateBy: localStorage.getItem('infoearth_spacedata_userCode'),
                StyleDataType: styleDataTypeId,// 样式类型，1点，2线，3面
                StyleConfigType: infos.threeToOne,
                StyleDefaultLayer: infos.selectedLayerInfo ? infos.selectedLayerInfo.id : null,
                StyleRenderField: infos.fieldTypeSelect.selected ? infos.fieldTypeSelect.selected.id : '',
                StyleRenderFieldName: infos.fieldTypeSelect.selected ? infos.fieldTypeSelect.selected.attributeName : '',
                StyleRenderColorBand: $rootScope.styleColorBarSelect.selected ? $rootScope.styleColorBarSelect.selected.colorName : '',
                StyleRenderRule: _styleRenderRule,//预设符号JSON串   样式渲染预设规则

                StyleInfo: {
                    Type: styleDataTypeId,// 样式类型，1点，2线，3面
                    MinScaleDenominator: infos.minDistance,// 最小比例尺
                    MaxScaleDenominator: infos.maxDistance,// 最大比例尺

                    //点样式
                    IsIcon: styleInfo.isIcon,
                    IconPath: styleInfo.iconPath,
                    IconSize: styleInfo.iconSize,
                    IconRotation: styleInfo.iconRotation,
                    PointType: styleInfo.pointType,

                    //线样式
                    IsOutline: styleInfo.isOutline,
                    LineWidth: styleInfo.lineWidth,
                    OutlineColor: styleInfo.outlineColor,
                    LineTransparency: styleInfo.lineTransparency,

                    //线型
                    LineType: styleInfo.lineType,
                    IsFill: styleInfo.isFill,
                    FillTransparency: styleInfo.fillTransparency,
                    PolygonColor: styleInfo.polygonColor
                },
                RuleDatas: infos.tableDataSource
            };
            //console.log(dto);

            waitmask.onShowMask($filter('translate')('setting.waitText'), 200);

            if ($rootScope.addStyleModol.title == '编辑样式' || $rootScope.addStyleModol.title == 'Edit style') {
                //console.log(editStyleInfo);

                dataStyle.update(dto).success(function (data, status) {
                    if (styleBackFun && angular.isFunction(styleBackFun)) {
                        styleBackFun(data, status);
                    }

                    if (status == 200) {
                        console.log('Debug: styleInit  fun  ', angular.isFunction(styleInit));

                        if (styleInit && angular.isFunction(styleInit)) {
                            styleInit();
                        }
                        waitmask.onHideMask();
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.ModifySuccessfully'), '', 'success', '#007AFF');
                        modalInstance.close();
                    }
                }).error(function (data) {
                    if (styleBackFun && angular.isFunction(styleBackFun)) {
                        styleBackFun(data);
                    }
                    //console.log(data);
                    waitmask.onHideMask();
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.error'), data.message, 'error', '#007AFF');
                });
            } else if ($rootScope.addStyleModol.title == '新增样式' || $rootScope.addStyleModol.title == 'New style') {
                dataStyle.isExists(dto.StyleName).success(function (result, status) {
                    //console.log(data);
                    if (!!result) {
                        waitmask.onHideMask();
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.StyleNameRepeated'), $filter('translate')('views.Style.alertFun.InputAgin'), 'warning', '#007AFF');
                        return;
                    }

                    console.log('Debug:   ', dto);

                    dataStyle.insert(dto).success(function (data, status) {
                        if (styleBackFun && angular.isFunction(styleBackFun)) {
                            styleBackFun(data, status);
                        }
                        if (status == 200) {
                            if (styleInit && angular.isFunction(styleInit)) {
                                styleInit();
                            }
                            waitmask.onHideMask();
                            $rootScope.alertFun($filter('translate')('views.Style.alertFun.NewStyleSuccessful'), '', 'success', '#007AFF');
                            modalInstance.close();
                        }
                    }).error(function (data) {
                        //console.log(data);
                        waitmask.onHideMask();
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.error'), data.message, 'error', '#007AFF');
                    });

                }).error(function (data, status) {
                    if (styleBackFun && angular.isFunction(styleBackFun)) {
                        styleBackFun(data, status);
                    }
                    console.log(data);
                    waitmask.onHideMask();
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.error'), data.message, 'error', '#007AFF');
                });

            } else if ($rootScope.addStyleModol.title == '查看样式') {
                waitmask.onHideMask();
                modalInstance.close();
            }

        }

        /*--------------选择图层弹窗--------------start----*/
        $rootScope.choseLayerModol = {
            //标签查询树
            tagTreeData: [],
            tagTreeCtrl: {},
            tagSelected: {},
            onTagSelected: function (node) {
                $rootScope.choseLayerModol.pageIndex = 1;
                $rootScope.choseLayerModol.layerName = "";
                $rootScope.getAllLayer('', node.id, '', $rootScope.choseLayerModol.pageSize, $rootScope.choseLayerModol.pageIndex);
            },
            //分类查询树
            typeTreeData: [{ label: $filter('translate')('views.Layer.Query.class.all'), children: [], id: '' }],
            typeTreeCtrl: {},
            typeSelected: {},
            onTypeSelected: function (node) {
                $rootScope.choseLayerModol.pageIndex = 1;
                $rootScope.choseLayerModol.layerName = "";
                $rootScope.getAllLayer(node.id, '', '', $rootScope.choseLayerModol.pageSize, $rootScope.choseLayerModol.pageIndex);
                $rootScope.choseLayerModol.tagTreeCtrl.select_branch();
            },

            //切换树
            showtab: true,
            selectTab: function (state) {
                $rootScope.choseLayerModol.showtab = state;
                $rootScope.choseLayerModol.typeTreeCtrl.select_branch($rootScope.choseLayerModol.typeTreeData[0]);
                $rootScope.choseLayerModol.onTypeSelected($rootScope.choseLayerModol.typeTreeData[0]);
            },

            //搜索图层
            layerName: '',
            searchLayerByName: function () {
                $rootScope.pageIndex = 1;
                $rootScope.choseLayerModol.pageIndex = 1;
                var typeId = typeof ($rootScope.choseLayerModol.typeSelected.id) === "undefined" ? '' : $rootScope.choseLayerModol.typeSelected.id;
                var tagId = typeof ($rootScope.choseLayerModol.tagSelected.id) === "undefined" ? '' : $rootScope.choseLayerModol.tagSelected.id;
                $rootScope.getAllLayer(typeId, tagId, $rootScope.choseLayerModol.layerName, $rootScope.choseLayerModol.pageSize, $rootScope.choseLayerModol.pageIndex);
            },

            choseableLayer: [],
            checkedData: {},
            choiceTR: function (tr) {
                //console.log(tr);
                $rootScope.choseLayerModol.checkedData = {};
                if (tr.checked) {
                    for (var i in $rootScope.choseLayerModol.choseableLayer) {
                        if ($rootScope.choseLayerModol.choseableLayer[i].id !== tr.id) {
                            $rootScope.choseLayerModol.choseableLayer[i].checked = false;
                        }
                    }
                    $rootScope.choseLayerModol.checkedData = angular.copy(tr);
                }
                //console.log($rootScope.choseLayerModol.checkedData);
            },

            //样式表格分页
            maxSize: 4,//页码个数显示数
            goPage: 1,//转到多少页
            pageCounts: 0,//32;//总条数
            pageIndex: 1,//1;//起始页
            pageSize: 10,//10;//每页显示条数
            //分页的事件方法
            pageChanged: function (a, evt) {
                if (evt && evt.keyCode !== 13) { return; }//注：回车键为13
                if (a) {
                    a = parseInt(a);
                    if (isNaN(a) || a < 1 || a > $rootScope.choseLayerModol.totalPages) {
                        $rootScope.choseLayerModol.goPage = $rootScope.choseLayerModol.pageIndex;
                        return;
                    }
                    $rootScope.choseLayerModol.goPage = a;
                    $rootScope.choseLayerModol.pageIndex = a;
                }
                //console.log({ MapName: $rootScope.inputText, MapType: $rootScope.typeNodeId });
                //调用AJAX
                var typeId = typeof ($rootScope.choseLayerModol.typeSelected.id) === "undefined" ? '' : $rootScope.choseLayerModol.typeSelected.id;
                var tagId = typeof ($rootScope.choseLayerModol.tagSelected.id) === "undefined" ? '' : $rootScope.choseLayerModol.tagSelected.id;
                $rootScope.getAllLayer(typeId, tagId, $rootScope.choseLayerModol.layerName, $rootScope.choseLayerModol.pageSize, $rootScope.choseLayerModol.pageIndex);
            },

            openWin: function () { },
            submitForm: function (modalInstance, form) {
                //console.log($rootScope.choseLayerModol.checkedData);
                var content = $.trim(editor1.getValue());
                if (!content) {
                    content = $.trim(editor2.getValue());
                }
                $rootScope.addStyleModol.currentLayer = angular.copy($rootScope.choseLayerModol.checkedData);
                preStyle($rootScope.addStyleModol.styleName, $rootScope.addStyleModol.currentLayer, content);

                modalInstance.close();
            },
            cancelWin: function () { },
        };

        //获取图层列表
        $rootScope.getAllLayer = function (typeId, tagId, name, size, index) {
            $rootScope.choseLayerModol.choseableLayer = [];
            layerContent.getPageListByName({ LayerName: name, LayerType: typeId, LayerTag: tagId, CreateBy: localStorage.getItem('infoearth_spacedata_userCode')}, size, index).success(function (data, status) {
                //console.log(data);
                $rootScope.choseLayerModol.pageCounts = data.totalCount;
                $rootScope.choseLayerModol.totalPages = Math.ceil($rootScope.choseLayerModol.pageCounts / $rootScope.choseLayerModol.pageSize);
                $rootScope.choseLayerModol.choseableLayer = angular.copy(data.items);

                for (var i in $rootScope.choseLayerModol.choseableLayer) {
                    $rootScope.choseLayerModol.choseableLayer[i].checked = false;
                    if (!!$rootScope.choseLayerModol.checkedData.id) {
                        if ($rootScope.choseLayerModol.choseableLayer[i].id === $rootScope.choseLayerModol.checkedData.id) {
                            $rootScope.choseLayerModol.choseableLayer[i].checked = true;
                        }
                    }
                }
            });
        }

        //分类树
        function getLayerTypeTree() {
            $rootScope.choseLayerModol.typeTreeData[0].children = [];
            //分类查询树的数据
            dataType.getAllListByDataType(layerTypeID).success(function (data, statues) {
                //console.log(data);

                var arr = [];
                for (var i in data.items) {
                    var tempTypeData = {};
                    tempTypeData.id = data.items[i].id;
                    tempTypeData.dictCodeID = data.items[i].dictCodeID;
                    tempTypeData.label = data.items[i].typeName;
                    tempTypeData.typeDesc = data.items[i].typeDesc;
                    tempTypeData.parentID = data.items[i].parentID;
                    tempTypeData.children = [];
                    arr = arr.concat(tempTypeData);
                }
                arr.forEach(function (each) {
                    if (each.parentID == 0) {
                        $rootScope.choseLayerModol.typeTreeData[0].children.push(each);
                    }
                });
                $rootScope.choseLayerModol.typeTreeData[0].children.forEach(function (each) {
                    each.children = [];
                    arr.forEach(function (item) {
                        if (item.parentID == each.id) {
                            each.children.push(item);
                        }
                    })
                });
            });
        }
        //标签树
        function getLayerTagTree() {
            //标签查询树的数据
            dataTag.getAllListByDataType(layerTypeID).success(function (data, statues) {
                //console.log(data);
                for (var i in data.items) {
                    var tempTagData = {};
                    tempTagData.id = data.items[i].id;
                    tempTagData.dictCodeID = data.items[i].dictCodeID;
                    tempTagData.label = data.items[i].tagName;
                    tempTagData.tagDesc = data.items[i].tagDesc;
                    tempTagData.children = [];

                    $rootScope.choseLayerModol.tagTreeData = $rootScope.choseLayerModol.tagTreeData.concat(tempTagData);
                }
            });
        }
        /*--------------选择图层弹窗--------------end----*/


        /*--------------创建分类弹窗--------------start----*/
        //创建分类的弹窗model
        $rootScope.creatStyleTypeModal = {
            title:"",
            classAddPa: function () { },
            classAddSon: function () { },
            classEdit: function () { },
            classDel: function () { },
            classTreedata: [],
            classTreeSel: {},
            onClaSelected: function () { },
            html: '    <form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            '<div class="form-group">' +
            '                <div class="popwin-top" style="padding: 0 10px 10px 10px; text-align: center;">' +
            '                    <div class="btn-group" style="margin-bottom: 15px;">' +
            '                        <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classAddPa()" translate="setting.class.newParent">' +
            '                            新增父节点' +
            '                        </a>' +
            '                        <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classAddSon()" translate="setting.class.newChild">' +
            '                            新增子节点' +
            '                        </a>' +
            '                        <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classEdit()" translate="setting.edit">' +
            '                            编辑' +
            '                        </a>' +
            '                        <a class="btn btn-primary font-title-btn" href="javascript:;" ng-click="popwinmodal.classDel()" translate="setting.delete">' +
            '                            删除' +
            '                        </a>' +
            '                    </div>' +
            '                </div>' +
            '                <div class="popwin-body" style="padding: 0 10px; max-height: 300px; overflow-y: auto;">' +
            '                    <div style="border-top: solid 1px rgba(0, 0, 0, 0.07);"></div>' +
            '                    <abn-tree class="font-title-btn" tree-data="popwinmodal.classTreedata" initial-selection="{{popwinmodal.classTreedata[0].label}}" selected-data="popwinmodal.classTreeSel" icon-leaf="" on-select="popwinmodal.onClaSelected" expand-level="3"></abn-tree>' +
            '                    <div style="border-top: solid 1px rgba(0, 0, 0, 0.07);"></div>' +
            '                </div>' +
            '            </div>' +
            '        <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '            <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.sure">确定</button>' +
            '        </div>' +
            '    </form>'
        }
        //初始化分类树
        $rootScope.initCreatType = function () {
            $rootScope.creatStyleTypeModal.classTreedata = [];
            dataType.getAllListByDataType(dataTypeID).success(function (data, statues) {
                //console.log(data);
                var arr = [];
                for (var i in data.items) {
                    var tempTypeData = {};
                    tempTypeData.id = data.items[i].id;
                    tempTypeData.dictCodeID = data.items[i].dictCodeID;
                    tempTypeData.label = data.items[i].typeName;
                    tempTypeData.typeDesc = data.items[i].typeDesc;
                    tempTypeData.parentID = data.items[i].parentID;
                    tempTypeData.children = [];
                    arr.push(tempTypeData);
                }
                arr.forEach(function (each) {
                    if (each.parentID == 0) {
                        $rootScope.creatStyleTypeModal.classTreedata.push(each);
                    }
                });
                $rootScope.creatStyleTypeModal.classTreedata.forEach(function (each) {
                    each.children = [];
                    arr.forEach(function (item) {
                        if (item.parentID == each.id) {
                            each.children.push(item);
                        }
                    })
                });
                $rootScope.typeTreeData[0].children = angular.copy($rootScope.creatStyleTypeModal.classTreedata);
            });
        }
        //打开
        $rootScope.openCreateStyle = function () {
            $rootScope.creatStyleTypeModal.title = $filter('translate')('setting.class.newClassTitle');
        }
        //提交
        $rootScope.submitCreateStyleForm = function (modalInstance, form) {
            //console.log('9999   ',$rootScope.creatStyleTypeModal);
            $rootScope.loginOut();

            $rootScope.getTypeTree();

            $timeout(function () {
                $rootScope.addStyleModol.typePullDownTreeData = angular.copy($rootScope.typeTreeData[0].children);
            }, 500);

            //关闭弹出框
            modalInstance.close();
        }
        //取消
        $rootScope.cancelCreate = function () { }

        //新增父节点
        $rootScope.nodeType = '';
        $rootScope.creatStyleTypeModal.classAddPa = function () {
            $rootScope.openInputTextModal.typeName = '';
            $rootScope.openInputTextModal.title = $filter('translate')('setting.class.newParent');
            $rootScope.openInputTextModal.openInputText();
            $rootScope.nodeType = 'parent';
            getNodeInputFocus("openInputTextCls");
        }
        //新增子节点
        $rootScope.creatStyleTypeModal.classAddSon = function () {
            $rootScope.nodeType = 'children';
            if (!$rootScope.creatStyleTypeModal.classTreeSel.id) {
                $rootScope.alertFun($filter('translate')('views.Style.alertFun.SelectNode'), "", 'warning', '#007AFF');
            } else {
                if ($rootScope.creatStyleTypeModal.classTreeSel.parentID != "0") {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.SelectParNode'), "", 'warning', '#007AFF');
                }
                else {
                    $rootScope.openInputTextModal.typeName = '';
                    $rootScope.openInputTextModal.title = $filter('translate')('setting.class.newChild');
                    $rootScope.openInputTextModal.openInputText();
                    getNodeInputFocus("openInputTextCls");
                }
            }
        }

        //编辑
        $rootScope.creatStyleTypeModal.classEdit = function () {
            $rootScope.nodeType = 'edit';
            if (!$rootScope.creatStyleTypeModal.classTreeSel.id) {
                $rootScope.alertFun($filter('translate')('views.Style.alertFun.SelectNodeEdit'), "", 'warning', '#007AFF');
            }
            else {
                $rootScope.openInputTextModal.typeName = $rootScope.creatStyleTypeModal.classTreeSel.label;
                $rootScope.openInputTextModal.title = $filter('translate')('setting.edit');
                $rootScope.openInputTextModal.openInputText();
                getNodeInputFocus("openInputTextCls");
            }
        }
        //删除
        $rootScope.creatStyleTypeModal.classDel = function () {
            $rootScope.nodeType = 'delete';
            if (!$rootScope.creatStyleTypeModal.classTreeSel.id) {
                $rootScope.alertFun($filter('translate')('views.Style.alertFun.SelectDelete'), "", 'warning', '#007AFF');
            }
            else if ($rootScope.creatStyleTypeModal.classTreeSel.id == $rootScope.currentType) {
                $rootScope.alertFun($filter('translate')('views.Style.alertFun.NotBeDeleted'), "", 'warning', '#007AFF');
            }
            else {
                $rootScope.alertConfirm($filter('translate')('views.Style.alertFun.delete'), $filter('translate')('views.Style.alertFun.DeleteSelectedNode'), "warning", function () {
                    dataType.delete($rootScope.creatStyleTypeModal.classTreeSel.id).success(function (data, status) {
                        //console.log(data);
                        $rootScope.initCreatType();
                        $rootScope.creatStyleTypeModal.classTreeSel = {};
                    });
                });
            }
        }

        //输入节点的input框获取焦点
        function getNodeInputFocus(classname) {
            $timeout(function () {
                do {
                    $("." + classname + " .font-title-little input").focus();
                }
                while (!$("." + classname)[0]);
            }, 200);
        }

        //选中树节点执行方法
        $rootScope.creatStyleTypeModal.onClaSelected = function (node, getParentNodeBackFun) {
            //console.log("当前节点", node);
            var pnode = getParentNodeBackFun(node);
            //console.log("当前节点的父节点", pnode);
        }

        //填写分类弹窗model
        $rootScope.openInputTextModal = {
            typeName: '',
            labelTxt: '',
            title: '',
            openWin: function(){
                $rootScope.openInputTextModal.labelTxt = $filter('translate')('setting.class.inputNode');
            },
            submit: function (a) {
                $rootScope.loginOut();
                if (!$.trim($rootScope.openInputTextModal.typeName)) {
                    $rootScope.alertFun($filter('translate')('views.Style.alertFun.EnterContent'), "", 'warning', '#007AFF');
                    return;
                }
                dataType.getDetailByName($rootScope.openInputTextModal.typeName, dataTypeID).success(function (data, statues) {
                    if (!data) return;
                    if (data.items.length === 0) {
                        if ($rootScope.nodeType == 'parent') {
                            var data = { TypeName: $rootScope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: dataTypeID, ParentID: "0" }
                            //console.log(data);
                            dataType.insert(data).success(function (data, status) {
                                //console.log(data);
                                a.close();
                                $rootScope.initCreatType();
                            });
                        } else if ($rootScope.nodeType == 'children') {
                            var data = { TypeName: $rootScope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: dataTypeID, ParentID: $rootScope.creatStyleTypeModal.classTreeSel.id }
                            //console.log(data);
                            dataType.insert(data).success(function (data, status) {
                                //console.log(data);
                                a.close();
                                $rootScope.initCreatType();
                            })
                        } else if ($rootScope.nodeType == 'edit') {
                            var data = { Id: $rootScope.creatStyleTypeModal.classTreeSel.id, TypeName: $rootScope.openInputTextModal.typeName, TypeDesc: '', DictCodeID: dataTypeID, ParentID: $rootScope.creatStyleTypeModal.classTreeSel.parentID }
                            dataType.update(data).success(function (data, status) {
                                //console.log(data);
                                a.close();
                                $rootScope.initCreatType();
                            });
                        }
                        $rootScope.openInputTextModal.typeName = [];
                        $rootScope.creatStyleTypeModal.classTreeSel = {};
                    }
                    else {
                        $rootScope.alertFun($filter('translate')('views.Style.alertFun.CategoryExists'), "", 'warning', '#007AFF');
                    }
                });
            },
            html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            '    <div class="form-group">' +
            '        <div class="col-sm-11" style="padding-left: 25px;"><text-box class="font-title-little" text-model="popwinmodal.typeName" text-label="popwinmodal.labelTxt"  required="true" text-length="" style="height: 34px;"></text-box></div>' +
            '        <div class="col-sm-1"></div>' +
            '    </div>' +
            '    <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '        <button class="btn btn-wide btn-primary font-title-btn" type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" translate="setting.submit">提交</button>' +
            '        <a class="btn btn-wide btn-o btn-default font-title-btn" href="javascript:;" style="float: right; min-width: 80px;" ng-click="submitForm.cancel()" translate="setting.cancel">取消</a>' +
            '    </div>' +
            '</form>'
        }

        /*--------------创建分类弹窗--------------end----*/

        /*--------------样式预览弹窗--------------start----*/
        var legend = GeoServerWmsUrl + '?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=20&HEIGHT=20&LAYER=' + WorkSpace + ':';

        $rootScope.stylePreviewModal = {
            html: '<form role="form" class="form-horizontal" name="$popwin_form" novalidate ng-submit="submitForm.submit($popwin_form)">' +
            '    <div class="form-group">' +
            '        <div class="r-right-bottom" style="overflow: auto; max-height: 200px;">' +
            //'            <ul id="dowebok"><li><img ng-src="{{popwinmodal.legend}}" alt="" /></li></ul>' +
            '        </div>' +
            '    </div>' +
            '    <div class="form-group" style="margin-top: 50px;margin-bottom: 0px; margin-right: 0px;">' +
            '        <button type="submit" style="min-width: 80px; float: right; margin-left: 0.5em;" class="btn btn-wide btn-primary" translate="setting.sure">确定</button>' +
            '    </div>' +
            '</form>'
        };

        $rootScope.submitStylePreview = function (modalInstance, form) {
            modalInstance.close();
        }

        //检查样式内容的首尾
        function matchingXmlHeadAndEnd(content) {
            var firstStrs = content.substr(0, content.indexOf('>') + 1);
            var lastStrs = content.substr(content.lastIndexOf('<\/') + 2);
            if (firstStrs.indexOf('<') === 0 && lastStrs.indexOf('>') === lastStrs.length - 1) {
                return true;
            }
            return false;
        }
        //预览样式
        function preStyle(styleName, data, sld) {

            var style = {
                StyleName: styleName,
                StyleContent: sld
            };
            dataStyle.insertGeoServerStyle(style).success(function (result, status) {
                if (status == 200) {
                    $("#map").html("");
                    var format = 'image/png';
                    //var bounds = [$rootScope.addStyleModol.currentLayer.data.minX, $rootScope.addStyleModol.currentLayer.data.minY, $rootScope.addStyleModol.currentLayer.data.maxX, $rootScope.addStyleModol.currentLayer.data.maxY];//范围
                    var bounds = [data.minX, data.minY, data.maxX, data.maxY];//范围
                    var ImageMap = new ol.layer.Image({
                        source: new ol.source.ImageWMS({
                            ratio: 1,
                            url: GeoServerUrl + '/wms',
                            params: {
                                FORMAT: format,
                                VERSION: '1.1.0',
                                STYLES: styleName,
                                //SLD_BODY: sld,
                                LAYERS: WorkSpace + ":" + data.layerAttrTable,
                            }
                        })
                    });
                    var projection = new ol.proj.Projection({
                        code: 'EPSG:4326',//投影编码
                        units: 'degrees',
                        axisOrientation: 'neu'
                    });
                    var map = new ol.Map({
                        controls: ol.control.defaults({
                            attribution: false
                        }).extend([
                            new ol.control.ScaleLine()
                        ]),
                        target: 'map',
                        layers: [
                            ImageMap
                        ],
                        view: new ol.View({
                            projection: projection
                        })
                    });
                    map.getView().fit(bounds, map.getSize());
                }
            }).error(function (data) {

            });


            //console.log(map);
        }

        /************************************样式管理多域调用的公共部分------end***************************************************/

        //生成随机数
        function getRandom() {
            var s = [];
            var str = "0123456789abcdefghijklmnopqrstuvwxyz";
            for (var i = 0; i < 36; i++) {
                s[i] = str.substr(Math.floor(Math.random() * 0x10), 1);
            }
            s[14] = "4";
            s[19] = str.substr((s[19] & 0x3) | 0x8, 1);
            s[8] = s[13] = s[18] = s[23] = "-";
            var guid = s.join("");
            return guid;
        }

    }]);
