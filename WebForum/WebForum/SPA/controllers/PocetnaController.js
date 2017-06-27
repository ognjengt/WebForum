webForum.controller('PocetnaController', function ($scope, $rootScope, AccountFactory, $window) {

    if (!$rootScope.ulogovan) {
        $window.location.href = "#!/login";
    }

    function init() {
        AccountFactory.getSacuvaneTeme(sessionStorage.getItem("username")).then(function (response) {
            $scope.snimljeneTeme = response.data;
            console.log(response.data);
        })
    }

    init();

});