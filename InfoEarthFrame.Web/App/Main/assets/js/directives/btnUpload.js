'use strict';
/** 
  * 多媒体控件
  * by licx
*/

app.directive('btnUpload', function ($timeout, $filter) {
    return {
        restrict: 'E',
        template: function (elemt, attr) {
            return '<a class="btn btn-wide btn-o btn-primary btn-file" href="javascript:;">' +
                '   <i class="fa ti-upload"></i>&nbsp;' + elemt.text() +
                '   <input type="file" nv-file-select=""  uploader="myUploader" filters="customsFilter" ' + (angular.isDefined(attr.multiple) ? 'multiple' : '') + ' />' +
                '</a>';
        },
        scope: {
            formData: '=?',
            uploadUrl: '=',
            onSuccess: '=?',
        },
        replace: true,
        controller: ['$scope', '$element', 'FileUploader', 'SweetAlert', function ($scope, $element, FileUploader, SweetAlert) {
            if (!$scope.uploadUrl) {
                SweetAlert.swal({
                    title: 'upload-url属性不能为空',
                    text: '',
                    type: "error",
                    confirmButtonColor: "#007AFF"
                });
                return;
            }
            /***********************************************上传部分---start----******************************/
            $scope.myUploader = new FileUploader();
            //console.log('Debug:   ', $scope.myUploader);

            $scope.myUploader.url = $scope.uploadUrl;

            $scope.$watch('formData', function (val) {
                $scope.myUploader.formData = (!!val && val.length != 0) ? val : [];
            });
            

            // FILTERS(上传文件的条件过滤)
            $scope.myUploader.filters.push({
                name: 'customsFilter',
                fn: function (item) {//监测上传文件的类型、大小及上传文件的个数等条件 "image/png"  \'.jpg.png.jpeg.bmp.gif\'.indexOf(row.extension) > -1
                    
                    return true;
                }
            });
            
            $scope.myUploader.onAfterAddingFile = function (fileItem) {
                //console.log('Debug: formData =   ', $scope.myUploader.formData);
                fileItem.upload();
            };
            $scope.myUploader.onSuccessItem = function (fileItem, response, status, headers) {
                console.info('onSuccessItem', response);
                if (response.success === "true") {
                    if (!!$scope.onSuccess) {
                        $scope.onSuccess(response);
                    }
                    SweetAlert.swal({
                        title: $filter('translate')('views.Style.alertFun.UploadSuccessfully'),
                        text: "",
                        type: "success",
                        confirmButtonColor: "#007AFF"
                    });
                }
                else {
                    SweetAlert.swal({
                        title: $filter('translate')('views.Style.alertFun.uploadFailed'),
                        text: response.message,
                        type: "error",
                        confirmButtonColor: "#007AFF"
                    });

                }
            };
            /***********************************************上传部分---end----******************************/
        }]
    }
});
