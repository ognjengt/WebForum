webForum.controller('PocetnaController', function ($scope, $rootScope, AccountFactory) {

    function init() {
        AccountFactory.getSacuvaneTeme(sessionStorage.getItem("username")).then(function (response) {
            $scope.snimljeneTeme = response.data;
        })
    }

    init();

});