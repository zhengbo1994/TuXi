
app.controller('spaceWinCtrl', ["$scope", "$rootScope", "$modalInstance", "$modal", "fromWhere", "which", "SweetAlert",
    function ($scope, $rootScope, $modalInstance, $modal, fromWhere, which, SweetAlert) {
        $rootScope.loginOut();
    $scope.my_data = [{
        label: '地理坐标系',
        children: [{
            label: 'Asia',
            uid: 11,
            children: [{
                label: 'Beijing 1954',
                uid: 111
            }, {
                label: 'Xian 1980',
                uid: 112
            }, {
                label: 'China Geodetic Coordinate System 2000',
                uid: 113
            }]
        }, {
            label: 'World',
            uid: 12,
            children: [{
                label: 'WGS 1984',
                uid: 121
            }]
        }]
    }, {
        label: "投影坐标系",
        children: [{
            label: 'Gauss Kruger',
            uid: 21,
            children: [{
                label: 'Beijing 1954',
                uid: 211,
                children: [{
                    label: 'Beijing 1954 3 Degree GK CM 75E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 78E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 81E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 84E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 87E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 90E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 93E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 96E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 99E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 102E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 105E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 108E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 111E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 114E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 117E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 120E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 123E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 126E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 129E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 132E'
                }, {
                    label: 'Beijing 1954 3 Degree GK CM 135E'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 25'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 26'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 27'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 28'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 29'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 30'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 31'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 32'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 33'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 34'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 35'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 36'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 37'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 38'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 39'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 40'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 41'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 42'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 43'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 44'
                }, {
                    label: 'Beijing 1954 3 Degree GK Zone 45'
                }, {
                    label: 'Beijing 1954 3 GK Zone 13'
                }, {
                    label: 'Beijing 1954 3 GK Zone 13N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 14'
                }, {
                    label: 'Beijing 1954 3 GK Zone 14N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 15'
                }, {
                    label: 'Beijing 1954 3 GK Zone 15N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 16'
                }, {
                    label: 'Beijing 1954 3 GK Zone 16N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 17'
                }, {
                    label: 'Beijing 1954 3 GK Zone 17N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 18'
                }, {
                    label: 'Beijing 1954 3 GK Zone 18N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 19'
                }, {
                    label: 'Beijing 1954 3 GK Zone 19N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 20'
                }, {
                    label: 'Beijing 1954 3 GK Zone 20N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 21'
                }, {
                    label: 'Beijing 1954 3 GK Zone 21N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 22'
                }, {
                    label: 'Beijing 1954 3 GK Zone 22N'
                }, {
                    label: 'Beijing 1954 3 GK Zone 23'
                }, {
                    label: 'Beijing 1954 3 GK Zone 23N'
                }]
            }, {
                label: 'Xian 1980',
                uid: 212,
                children: [{
                    label: 'Xian 1980 3 Degree GK CM 75E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 78E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 81E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 84E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 87E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 90E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 93E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 96E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 99E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 102E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 105E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 108E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 111E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 114E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 117E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 120E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 123E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 126E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 129E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 132E'
                }, {
                    label: 'Xian 1980 3 Degree GK CM 135E'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 25'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 26'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 27'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 28'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 29'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 30'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 31'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 32'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 33'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 34'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 35'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 36'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 37'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 38'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 39'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 40'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 41'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 42'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 43'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 44'
                }, {
                    label: 'Xian 1980 3 Degree GK Zone 45'
                }, {
                    label: 'Xian 1980 GK CM 75E'
                }, {
                    label: 'Xian 1980 GK CM 81E'
                }, {
                    label: 'Xian 1980 GK CM 87E'
                }, {
                    label: 'Xian 1980 GK CM 93E'
                }, {
                    label: 'Xian 1980 GK CM 99E'
                }, {
                    label: 'Xian 1980 GK CM 105E'
                }, {
                    label: 'Xian 1980 GK CM 111E'
                }, {
                    label: 'Xian 1980 GK CM 117E'
                }, {
                    label: 'Xian 1980 GK CM 123E'
                }, {
                    label: 'Xian 1980 GK CM 129E'
                }, {
                    label: 'Xian 1980 GK CM 135E'
                }, {
                    label: 'Xian 1980 GK Zone 13'
                }, {
                    label: 'Xian 1980 GK Zone 14'
                }, {
                    label: 'Xian 1980 GK Zone 15'
                }, {
                    label: 'Xian 1980 GK Zone 16'
                }, {
                    label: 'Xian 1980 GK Zone 17'
                }, {
                    label: 'Xian 1980 GK Zone 18'
                }, {
                    label: 'Xian 1980 GK Zone 19'
                }, {
                    label: 'Xian 1980 GK Zone 20'
                }, {
                    label: 'Xian 1980 GK Zone 21'
                }, {
                    label: 'Xian 1980 GK Zone 22'
                }, {
                    label: 'Xian 1980 GK Zone 23'
                }]
            }, {
                label: 'CGCS 2000',
                uid: 213,
                children: [{
                    label: 'CGCS2000 3 Degree GK CM 75E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 78E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 81E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 84E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 87E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 90E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 93E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 96E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 99E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 102E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 105E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 108E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 111E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 114E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 117E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 120E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 123E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 126E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 129E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 132E'
                }, {
                    label: 'CGCS2000 3 Degree GK CM 135E'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 25'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 26'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 27'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 28'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 29'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 30'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 31'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 32'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 33'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 34'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 35'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 36'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 37'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 38'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 39'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 40'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 41'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 42'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 43'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 44'
                }, {
                    label: 'CGCS2000 3 Degree GK Zone 45'
                }, {
                    label: 'CGCS2000 GK CM 75E'
                }, {
                    label: 'CGCS2000 GK CM 81E'
                }, {
                    label: 'CGCS2000 GK CM 87E'
                }, {
                    label: 'CGCS2000 GK CM 93E'
                }, {
                    label: 'CGCS2000 GK CM 99E'
                }, {
                    label: 'CGCS2000 GK CM 105E'
                }, {
                    label: 'CGCS2000 GK CM 111E'
                }, {
                    label: 'CGCS2000 GK CM 117E'
                }, {
                    label: 'CGCS2000 GK CM 123E'
                }, {
                    label: 'CGCS2000 GK CM 129E'
                }, {
                    label: 'CGCS2000 GK CM 135E'
                }, {
                    label: 'CGCS2000 GK Zone 13'
                }, {
                    label: 'CGCS2000 GK Zone 14'
                }, {
                    label: 'CGCS2000 GK Zone 15'
                }, {
                    label: 'CGCS2000 GK Zone 16'
                }, {
                    label: 'CGCS2000 GK Zone 17'
                }, {
                    label: 'CGCS2000 GK Zone 18'
                }, {
                    label: 'CGCS2000 GK Zone 19'
                }, {
                    label: 'CGCS2000 GK Zone 20'
                }, {
                    label: 'CGCS2000 GK Zone 21'
                }, {
                    label: 'CGCS2000 GK Zone 22'
                }, {
                    label: 'CGCS2000 GK Zone 23'
                }]
            }]
        }, {
            label: 'UTM',
            uid: 22,
            children: [{
                label: 'WGS1984',
                uid: 221,
                children: [{
                    label: 'Northern Hemisphere',
                    uid: 2211,
                    children: [{
                        label: 'WGS 1984 BLM Zone 14N (US Feet)'
                    }, {
                        label: 'WGS 1984 BLM Zone 15N (US Feet)'
                    }, {
                        label: 'WGS 1984 BLM Zone 16N (US Feet)'
                    }, {
                        label: 'WGS 1984 BLM Zone 17N (US Feet)'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 20N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 21N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 22N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 23N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 24N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 25N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 26N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 27N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 28N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 29N'
                    }, {
                        label: 'WGS 1984 Complex UTM Zone 30N'
                    }, {
                        label: 'WGS 1984 UTM Zone 1N'
                    }, {
                        label: 'WGS 1984 UTM Zone 2N'
                    }, {
                        label: 'WGS 1984 UTM Zone 3N'
                    }, {
                        label: 'WGS 1984 UTM Zone 4N'
                    }, {
                        label: 'WGS 1984 UTM Zone 5N'
                    }, {
                        label: 'WGS 1984 UTM Zone 6N'
                    }, {
                        label: 'WGS 1984 UTM Zone 7N'
                    }, {
                        label: 'WGS 1984 UTM Zone 8N'
                    }, {
                        label: 'WGS 1984 UTM Zone 9N'
                    }, {
                        label: 'WGS 1984 UTM Zone 10N'
                    }, {
                        label: 'WGS 1984 UTM Zone 11N'
                    }, {
                        label: 'WGS 1984 UTM Zone 12N'
                    }, {
                        label: 'WGS 1984 UTM Zone 13N'
                    }, {
                        label: 'WGS 1984 UTM Zone 14N'
                    }, {
                        label: 'WGS 1984 UTM Zone 15N'
                    }, {
                        label: 'WGS 1984 UTM Zone 16N'
                    }, {
                        label: 'WGS 1984 UTM Zone 17N'
                    }, {
                        label: 'WGS 1984 UTM Zone 18N'
                    }, {
                        label: 'WGS 1984 UTM Zone 19N'
                    }, {
                        label: 'WGS 1984 UTM Zone 20N'
                    }, {
                        label: 'WGS 1984 UTM Zone 21N'
                    }, {
                        label: 'WGS 1984 UTM Zone 22N'
                    }, {
                        label: 'WGS 1984 UTM Zone 23N'
                    }, {
                        label: 'WGS 1984 UTM Zone 24N'
                    }, {
                        label: 'WGS 1984 UTM Zone 25N'
                    }, {
                        label: 'WGS 1984 UTM Zone 26N'
                    }, {
                        label: 'WGS 1984 UTM Zone 27N'
                    }, {
                        label: 'WGS 1984 UTM Zone 28N'
                    }, {
                        label: 'WGS 1984 UTM Zone 29N'
                    }, {
                        label: 'WGS 1984 UTM Zone 30N'
                    }, {
                        label: 'WGS 1984 UTM Zone 31N'
                    }, {
                        label: 'WGS 1984 UTM Zone 32N'
                    }, {
                        label: 'WGS 1984 UTM Zone 33N'
                    }, {
                        label: 'WGS 1984 UTM Zone 34N'
                    }, {
                        label: 'WGS 1984 UTM Zone 35N'
                    }, {
                        label: 'WGS 1984 UTM Zone 36N'
                    }, {
                        label: 'WGS 1984 UTM Zone 37N'
                    }, {
                        label: 'WGS 1984 UTM Zone 38N'
                    }, {
                        label: 'WGS 1984 UTM Zone 39N'
                    }, {
                        label: 'WGS 1984 UTM Zone 40N'
                    }, {
                        label: 'WGS 1984 UTM Zone 41N'
                    }, {
                        label: 'WGS 1984 UTM Zone 42N'
                    }, {
                        label: 'WGS 1984 UTM Zone 43N'
                    }, {
                        label: 'WGS 1984 UTM Zone 44N'
                    }, {
                        label: 'WGS 1984 UTM Zone 45N'
                    }, {
                        label: 'WGS 1984 UTM Zone 46N'
                    }, {
                        label: 'WGS 1984 UTM Zone 47N'
                    }, {
                        label: 'WGS 1984 UTM Zone 48N'
                    }, {
                        label: 'WGS 1984 UTM Zone 49N'
                    }, {
                        label: 'WGS 1984 UTM Zone 50N'
                    }, {
                        label: 'WGS 1984 UTM Zone 51N'
                    }, {
                        label: 'WGS 1984 UTM Zone 52N'
                    }, {
                        label: 'WGS 1984 UTM Zone 53N'
                    }, {
                        label: 'WGS 1984 UTM Zone 54N'
                    }, {
                        label: 'WGS 1984 UTM Zone 55N'
                    }, {
                        label: 'WGS 1984 UTM Zone 56N'
                    }, {
                        label: 'WGS 1984 UTM Zone 57N'
                    }, {
                        label: 'WGS 1984 UTM Zone 58N'
                    }, {
                        label: 'WGS 1984 UTM Zone 59N'
                    }, {
                        label: 'WGS 1984 UTM Zone 60N'
                    }]
                }, {
                    label: 'Southern Hemisphere',
                    uid: 2212,
                    children: [{
                        label: 'WGS 1984 UTM Zone 1S'
                    }, {
                        label: 'WGS 1984 UTM Zone 2S'
                    }, {
                        label: 'WGS 1984 UTM Zone 3S'
                    }, {
                        label: 'WGS 1984 UTM Zone 4S'
                    }, {
                        label: 'WGS 1984 UTM Zone 5S'
                    }, {
                        label: 'WGS 1984 UTM Zone 6S'
                    }, {
                        label: 'WGS 1984 UTM Zone 7S'
                    }, {
                        label: 'WGS 1984 UTM Zone 8S'
                    }, {
                        label: 'WGS 1984 UTM Zone 9S'
                    }, {
                        label: 'WGS 1984 UTM Zone 10S'
                    }, {
                        label: 'WGS 1984 UTM Zone 11S'
                    }, {
                        label: 'WGS 1984 UTM Zone 12S'
                    }, {
                        label: 'WGS 1984 UTM Zone 13S'
                    }, {
                        label: 'WGS 1984 UTM Zone 14S'
                    }, {
                        label: 'WGS 1984 UTM Zone 15S'
                    }, {
                        label: 'WGS 1984 UTM Zone 16S'
                    }, {
                        label: 'WGS 1984 UTM Zone 17S'
                    }, {
                        label: 'WGS 1984 UTM Zone 18S'
                    }, {
                        label: 'WGS 1984 UTM Zone 19S'
                    }, {
                        label: 'WGS 1984 UTM Zone 20S'
                    }, {
                        label: 'WGS 1984 UTM Zone 21S'
                    }, {
                        label: 'WGS 1984 UTM Zone 22S'
                    }, {
                        label: 'WGS 1984 UTM Zone 23S'
                    }, {
                        label: 'WGS 1984 UTM Zone 24S'
                    }, {
                        label: 'WGS 1984 UTM Zone 25S'
                    }, {
                        label: 'WGS 1984 UTM Zone 26S'
                    }, {
                        label: 'WGS 1984 UTM Zone 27S'
                    }, {
                        label: 'WGS 1984 UTM Zone 28S'
                    }, {
                        label: 'WGS 1984 UTM Zone 29S'
                    }, {
                        label: 'WGS 1984 UTM Zone 30S'
                    }, {
                        label: 'WGS 1984 UTM Zone 31S'
                    }, {
                        label: 'WGS 1984 UTM Zone 32S'
                    }, {
                        label: 'WGS 1984 UTM Zone 33S'
                    }, {
                        label: 'WGS 1984 UTM Zone 34S'
                    }, {
                        label: 'WGS 1984 UTM Zone 35S'
                    }, {
                        label: 'WGS 1984 UTM Zone 36S'
                    }, {
                        label: 'WGS 1984 UTM Zone 37S'
                    }, {
                        label: 'WGS 1984 UTM Zone 38S'
                    }, {
                        label: 'WGS 1984 UTM Zone 39S'
                    }, {
                        label: 'WGS 1984 UTM Zone 40S'
                    }, {
                        label: 'WGS 1984 UTM Zone 41S'
                    }, {
                        label: 'WGS 1984 UTM Zone 42S'
                    }, {
                        label: 'WGS 1984 UTM Zone 43S'
                    }, {
                        label: 'WGS 1984 UTM Zone 44S'
                    }, {
                        label: 'WGS 1984 UTM Zone 45S'
                    }, {
                        label: 'WGS 1984 UTM Zone 46S'
                    }, {
                        label: 'WGS 1984 UTM Zone 47S'
                    }, {
                        label: 'WGS 1984 UTM Zone 48S'
                    }, {
                        label: 'WGS 1984 UTM Zone 49S'
                    }, {
                        label: 'WGS 1984 UTM Zone 50S'
                    }, {
                        label: 'WGS 1984 UTM Zone 51S'
                    }, {
                        label: 'WGS 1984 UTM Zone 52S'
                    }, {
                        label: 'WGS 1984 UTM Zone 53S'
                    }, {
                        label: 'WGS 1984 UTM Zone 54S'
                    }, {
                        label: 'WGS 1984 UTM Zone 55S'
                    }, {
                        label: 'WGS 1984 UTM Zone 56S'
                    }, {
                        label: 'WGS 1984 UTM Zone 57S'
                    }, {
                        label: 'WGS 1984 UTM Zone 58S'
                    }, {
                        label: 'WGS 1984 UTM Zone 59S'
                    }, {
                        label: 'WGS 1984 UTM Zone 60S'
                    }]
                }]
            }]
        }]
    }];

    if (fromWhere === -1) {
        if (which.indexOf("投影") > 0) {
            if (which[0] === "G") {
                $scope.my_data.splice(0, 1);
            }
            if (which[0] === "P") {
                $scope.my_data.splice(1, 1);
            }
        }
        if (which.indexOf("坐标") > 0) {
            if (which[0] === "G") {
                $scope.my_data.splice(1, 1);
            }
            if (which[0] === "P") {
                $scope.my_data.splice(0, 1);
            }
        }
    }

    var branch_selected = "";
    $scope.selectedData = '';
    $scope.my_tree_handler = function () {       
        branch_selected = $scope.selectedData;
        console.log(branch_selected);
    }

    $scope.right = function () {
        if (branch_selected == "") {
            SweetAlert.swal({
                title: "没有选择任何一项！",
                type: "error",
                confirmButtonColor: "#007AFF"
            });
        }
        else if (branch_selected&&branch_selected.children.length > 0) {
            SweetAlert.swal({
                title: "请选择最底层节点！",
                type: "error",
                confirmButtonColor: "#007AFF"
            });
        }
        else {

            console.log(branch_selected);

            $scope.$emit('choseTheCoord', (branch_selected.label + "," + fromWhere));
            $modalInstance.dismiss('cancel');
        }
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}]);