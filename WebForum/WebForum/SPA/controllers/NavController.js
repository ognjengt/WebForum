webForum.controller('NavController', function ($scope,$location) {
    $scope.checkActive = function (path) {
        return ($location.path().substr(0, path.length) === path) ? 'active' : '';
    }

});