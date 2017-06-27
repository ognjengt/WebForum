webForum.controller('ProfilController', function ($scope, AccountFactory, $routeParams) {

    function init() {
        console.log('Profil kontroler inicijalizovan');

        AccountFactory.getSacuvaniPodforumi(sessionStorage.getItem("username")).then(function (response) {
            $scope.sacuvaniPodforumi = response.data;
            console.log(response.data);

            AccountFactory.getSacuvaneTeme(sessionStorage.getItem("username")).then(function (response) {
                $scope.sacuvaneTeme = response.data;
                console.log(response.data);

                AccountFactory.getSacuvaniKomentari(sessionStorage.getItem("username")).then(function (response) {
                    $scope.sacuvaniKomentari = response.data;
                    console.log(response.data);

                    AccountFactory.getSacuvaniPodkomentari(sessionStorage.getItem("username")).then(function (response) {
                        $scope.sacuvaniPodkomentari = response.data;
                        console.log(response.data);

                        // TODO ispisati sve lajkovane i dislajkovane entitete

                    });

                });

            });

        });
    }

    init();

});