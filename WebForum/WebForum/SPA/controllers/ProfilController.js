webForum.controller('ProfilController', function ($scope, AccountFactory, TemeFactory, PorukeFactory, $routeParams) {

    function init() {
        console.log('Profil kontroler inicijalizovan');
        $scope.showPorukaPopup = false;

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

                                    // uzmi poruke ako sam ja taj koji je ulogovan
                                    if ($scope.userProfile.Username == sessionStorage.getItem("username")) {
                                        PorukeFactory.getAllUserMessages(sessionStorage.getItem("username")).then(function (response) {
                                            console.log(response.data);
                                            $scope.primljenePoruke = response.data;
                                        });
                                    }
                                    

                                });

                            });

                        });

                    });

                });

            });
        });

        
    }

    init();

    $scope.posaljiPoruku = function (poruka) {
        if (poruka.sadrzaj == "" || poruka.sadrzaj == null) {
            alert('Popunite sadrzaj poruke za slanje!');
            return;
        }
        poruka.sadrzaj = poruka.sadrzaj.replace(/(\r\n|\n|\r)/gm, "{novired}");
        poruka.posiljalac = sessionStorage.getItem("username");
        poruka.primalac = $routeParams.username;
        PorukeFactory.posaljiPoruku(poruka).then(function (response) {
            console.log(response.data);
            $scope.poruka = {};
            $scope.showPorukaPopup = false;
            alert('Poruka uspesno poslata!');
        });
    }

    $scope.markirajKaoProcitano = function (poruka) {
        PorukeFactory.markirajKaoProcitano(poruka).then(function (response) {
            console.log(response.data);
            init();
        });
    }

});