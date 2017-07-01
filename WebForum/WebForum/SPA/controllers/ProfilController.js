webForum.controller('ProfilController', function ($scope, AccountFactory, TemeFactory, PorukeFactory, KomentariFactory, $routeParams) {

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
                        $scope.sacuvaniKomentari.forEach(function (komentar) {
                            komentar.TemaKojojPripada = komentar.TemaKojojPripada.replace("-", "/");
                        });
                        console.log(response.data);

                        AccountFactory.getSacuvaniPodkomentari($routeParams.username).then(function (response) {
                            var listaPodkomentara = response.data;
                            listaPodkomentara.forEach(function (podkomentar) {
                                podkomentar.TemaKojojPripada = podkomentar.TemaKojojPripada.replace("-", "/");
                                $scope.sacuvaniKomentari.push(podkomentar);
                            })
                            console.log(response.data);
                            
                            TemeFactory.getLajkovaneTeme($routeParams.username).then(function (response) {
                                var listaLajkovanih = response.data;
                                $scope.listaLajkovanihTema = [];
                                listaLajkovanih.forEach(function (tema) {
                                    tema = tema.replace("-", "/");
                                    $scope.listaLajkovanihTema.push(tema);
                                });
                                console.log(response.data);

                                TemeFactory.getDislajkovaneTeme($routeParams.username).then(function (response) {
                                    var listaDislajkovanih = response.data;
                                    $scope.listaDislajkovanihTema = [];
                                    listaDislajkovanih.forEach(function (tema) {
                                        tema = tema.replace("-", "/");
                                        $scope.listaDislajkovanihTema.push(tema);
                                    });
                                    console.log(response.data);

                                    KomentariFactory.getLajkovaniKomentari($routeParams.username).then(function (response) {
                                        $scope.listaLajkovanihKomentara = response.data;
                                        $scope.listaLajkovanihKomentara.forEach(function (komentar) {
                                            komentar.TemaKojojPripada = komentar.TemaKojojPripada.replace("-", "/");
                                        });
                                        console.log(response.data);

                                        KomentariFactory.getDislajkovaniKomentari($routeParams.username).then(function (response) {
                                            $scope.listaDislajkovanihKomentara = response.data;
                                            $scope.listaDislajkovanihKomentara.forEach(function (komentar) {
                                                komentar.TemaKojojPripada = komentar.TemaKojojPripada.replace("-", "/");
                                            });
                                            console.log(response.data);

                                            KomentariFactory.getLajkovaniPodkomentari($routeParams.username).then(function (response) {
                                                var listaLajkovanihPodkomentara = response.data;
                                                listaLajkovanihPodkomentara.forEach(function (podkomentar) {
                                                    podkomentar.TemaKojojPripada = podkomentar.TemaKojojPripada.replace("-", "/");
                                                    $scope.listaLajkovanihKomentara.push(podkomentar);
                                                })
                                                
                                                console.log(response.data);

                                                KomentariFactory.getDislajkovaniPodkomentari($routeParams.username).then(function (response) {
                                                    var listaDislajkovanihPodkomentara = response.data;
                                                    listaDislajkovanihPodkomentara.forEach(function (podkomentar) {
                                                        podkomentar.TemaKojojPripada = podkomentar.TemaKojojPripada.replace("-", "/");
                                                        $scope.listaDislajkovanihKomentara.push(podkomentar);
                                                    })
                                                    console.log(response.data);

                                                    // uzmi poruke ako sam ja taj koji je ulogovan
                                                    if ($scope.userProfile.Username == sessionStorage.getItem("username")) {
                                                        PorukeFactory.getAllUserMessages(sessionStorage.getItem("username")).then(function (response) {
                                                            console.log(response.data);
                                                            $scope.primljenePoruke = response.data;
                                                            $scope.primljenePoruke.forEach(function (poruka) {
                                                                poruka.Sadrzaj = poruka.Sadrzaj.replace(new RegExp('{novired}', 'g'), '\n');
                                                            });
                                                        });
                                                    }


                                                });

                                            });

                                        });

                                    });

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