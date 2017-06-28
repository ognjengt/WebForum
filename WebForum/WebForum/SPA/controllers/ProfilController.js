webForum.controller('ProfilController', function ($scope, AccountFactory, TemeFactory, $routeParams) {

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
                            
                            TemeFactory.getLajkovaneTeme($routeParams.username).then(function (response) {
                                $scope.listaLajkovanihTema = response.data;
                                console.log(response.data);

                                TemeFactory.getDislajkovaneTeme($routeParams.username).then(function (response) {
                                    $scope.listaDislajkovanihTema = response.data;
                                    console.log(response.data);
                                });

                            });

                        });

                    });

                });

            });
        });

        
    }

    init();

});