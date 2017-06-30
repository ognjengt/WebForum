webForum.controller('PreporukeController', function ($scope, $rootScope, PreporukeFactory, $window) {

    if (sessionStorage.getItem("username") == null) {
        alert('Ulogujte se da bi ste dobili preporuke');
        $window.location.href = "#!/login";
    }

    function init() {
        console.log('Preporuke controller startovan');

        PreporukeFactory.getPreporukeZaKorisnika(sessionStorage.getItem("username")).then(function (response) {
            console.log(response.data);
            $scope.preporuceneTeme = response.data;
        });
    }

    init();

});