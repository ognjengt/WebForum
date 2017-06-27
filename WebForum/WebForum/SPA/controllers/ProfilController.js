webForum.controller('ProfilController', function ($scope, AccountFactory, $routeParams) {

    function init() {
        console.log('Profil kontroler inicijalizovan');

        AccountFactory.getUserByUsername($routeParams.username).then(function (response) {
            console.log(response.data);

            $scope.userProfile = response.data;

            $scope.userProfile.DatumRegistracije = new Date($scope.userProfile.DatumRegistracije).toLocaleDateString();

            AccountFactory.getSacuvaniPodforumi($routeParams.username).then(function (response) {
                $scope.sacuvaniPodforumi = response.data;
                console.log(response.data);

                AccountFactory.getSacuvaneTeme($routeParams.username).then(function (response) {
                    $scope.sacuvaneTeme = response.data;
                    console.log(response.data);

                    AccountFactory.getSacuvaniKomentari($routeParams.username).then(function (response) {
                        $scope.sacuvaniKomentari = response.data;
                        console.log(response.data);

                        AccountFactory.getSacuvaniPodkomentari($routeParams.username).then(function (response) {
                            var listaPodkomentara = response.data;
                            listaPodkomentara.forEach(function (podkomentar) {
                                $scope.sacuvaniKomentari.push(podkomentar);
                            })
                            console.log(response.data);

                            //$scope.sacuvaniKomentari.push($scope.sacuvaniPodkomentari);
                            // TODO ispisati sve lajkovane i dislajkovane entitete

                        });

                    });

                });

            });
        });

        
    }

    init();

});