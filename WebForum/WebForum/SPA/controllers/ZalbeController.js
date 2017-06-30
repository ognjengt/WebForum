webForum.controller('ZalbeController', function ($scope, $window, $rootScope, ZalbeFactory, PorukeFactory, PodforumiFactory, TemeFactory) {

    function init() {
        console.log('Zalbe inicijalizovane');

        ZalbeFactory.getSveZalbe(sessionStorage.getItem("username")).then(function (response) {
            console.log(response.data);
            $scope.zalbe = response.data;
            $scope.zalbe.forEach(function (zalba) {
                zalba.DatumZalbe = new Date(zalba.DatumZalbe).toLocaleDateString();
                zalba.Tekst = zalba.Tekst.replace(new RegExp('{novired}', 'g'), '\n');
            });
        });
    }

    init();


    $scope.odbijZalbu = function (zalba) {
        var tekstPoruke = "";
        if (zalba.TipEntiteta == 'Podforum') {
            tekstPoruke = "Vasa zalba na podforum " + zalba.Entitet + " je odbijena."
        }
        else if (zalba.TipEntiteta == 'Tema') {
            tekstPoruke = "Vasa zalba na temu " + zalba.Entitet + " je odbijena."
        }
        else if (zalba.Entitet == 'Komentar') {
            tekstPoruke = "Vasa zalba na komentar " + zalba.Entitet + " je odbijena."
        }


        var poruka = {
            posiljalac: sessionStorage.getItem("username"),
            primalac: zalba.KorisnikKojiJeUlozio,
            sadrzaj: tekstPoruke
        }
        PorukeFactory.posaljiPoruku(poruka).then(function (response) {
            console.log(response.data);
            ZalbeFactory.obrisiZalbu(zalba).then(function (response) {
                console.log(response.data);
                alert('Zalba uspesno odbijena');
                init();
            });
        });
    }

    $scope.upozoriAutoraEntiteta = function (zalba) {
        var tekstPorukeZaAutoraZalbe = "";
        if (zalba.TipEntiteta == 'Podforum') {
            tekstPorukeZaAutoraZalbe = "Postovani, upozorili smo korisnika " + zalba.AutorZaljenogEntiteta + " zbog vase zalbe na podforum " + zalba.Entitet;
        }
        else if (zalba.TipEntiteta == 'Tema') {
            tekstPorukeZaAutoraZalbe = "Postovani, upozorili smo korisnika " + zalba.AutorZaljenogEntiteta + " zbog vase zalbe na temu " + zalba.Entitet;
        }
        else if (zalba.Entitet == 'Komentar') {
            tekstPorukeZaAutoraZalbe = "Postovani, upozorili smo korisnika " + zalba.AutorZaljenogEntiteta + " zbog vase zalbe na komentar " + zalba.Entitet;
        }

        var tekstPorukeZaAutoraZaljenogEntiteta = "";
        if (zalba.TipEntiteta == 'Podforum') {
            tekstPorukeZaAutoraZaljenogEntiteta = "Upozoravamo vas da je vas podforum " + zalba.Entitet + " bio prijavljen u zalbi.";
        }
        else if (zalba.TipEntiteta == 'Tema') {
            tekstPorukeZaAutoraZaljenogEntiteta = "Upozoravamo vas da je vasa tema " + zalba.Entitet + " bila prijavljena u zalbi.";
        }
        else if (zalba.Entitet == 'Komentar') {
            tekstPorukeZaAutoraZaljenogEntiteta = "Upozoravamo vas da je vas komentar " + zalba.Entitet + " bio prijavljen u zalbi.";
        }

        var porukaZaAutoraZalbe = {
            posiljalac: sessionStorage.getItem("username"),
            primalac: zalba.KorisnikKojiJeUlozio,
            sadrzaj: tekstPorukeZaAutoraZalbe
        };

        var porukaZaAutoraZaljenogEntiteta = {
            posiljalac: sessionStorage.getItem("username"),
            primalac: zalba.AutorZaljenogEntiteta,
            sadrzaj: tekstPorukeZaAutoraZaljenogEntiteta
        };

        PorukeFactory.posaljiPoruku(porukaZaAutoraZalbe).then(function (response) {
            console.log(response.data);
            
            PorukeFactory.posaljiPoruku(porukaZaAutoraZaljenogEntiteta).then(function (response) {
                ZalbeFactory.obrisiZalbu(zalba).then(function (response) {
                    console.log(response.data);
                    alert('Korisnici uspesno obavesteni');
                    init();
                });
            });
        });

    }

    $scope.obrisiEntitet = function (zalba) {

        var tekstPorukeZaAutoraZalbe = "";
        if (zalba.TipEntiteta == 'Podforum') {
            tekstPorukeZaAutoraZalbe = "Postovani, obrisali smo podforum "+ zalba.Entitet + " na koji ste se zalili.";
        }
        else if (zalba.TipEntiteta == 'Tema') {
            tekstPorukeZaAutoraZalbe = "Postovani, obrisali smo temu " + zalba.Entitet + " na koju ste se zalili.";
        }
        else if (zalba.Entitet == 'Komentar') {
            tekstPorukeZaAutoraZalbe = "Postovani, obrisali smo komentar " + zalba.Entitet + " na koji ste se zalili.";
        }

        var tekstPorukeZaAutoraZaljenogEntiteta = "";
        if (zalba.TipEntiteta == 'Podforum') {
            tekstPorukeZaAutoraZaljenogEntiteta = "Obavestavamo vas da je vas podforum " + zalba.Entitet + " obrisan zbog zalbi.";
        }
        else if (zalba.TipEntiteta == 'Tema') {
            tekstPorukeZaAutoraZaljenogEntiteta = "Obavestavamo vas da je vasa tema " + zalba.Entitet + " obrisana zbog zalbi.";
        }
        else if (zalba.Entitet == 'Komentar') {
            tekstPorukeZaAutoraZaljenogEntiteta = "Obavestavamo vas da je vas komentar " + zalba.Entitet + " obrisan zbog zalbi.";
        }

        var porukaZaAutoraZalbe = {
            posiljalac: sessionStorage.getItem("username"),
            primalac: zalba.KorisnikKojiJeUlozio,
            sadrzaj: tekstPorukeZaAutoraZalbe
        };

        var porukaZaAutoraZaljenogEntiteta = {
            posiljalac: sessionStorage.getItem("username"),
            primalac: zalba.AutorZaljenogEntiteta,
            sadrzaj: tekstPorukeZaAutoraZaljenogEntiteta
        };

        PorukeFactory.posaljiPoruku(porukaZaAutoraZalbe).then(function (response) {
            console.log(response.data);

            if (zalba.TipEntiteta == 'Podforum') {
                var podforum = {
                    Naziv: zalba.Entitet
                }
                PodforumiFactory.obrisiPodforum(podforum).then(function (response) {
                    if (response.data == true) {
                        PorukeFactory.posaljiPoruku(porukaZaAutoraZaljenogEntiteta).then(function (response) {
                            ZalbeFactory.obrisiZalbu(zalba).then(function (response) {
                                console.log(response.data);
                                alert('Entitet obrisan');
                                init();
                            });
                        });
                    }
                });
            }
            else if (zalba.TipEntiteta == 'Tema') {
                var podforumTema = zalba.Entitet.split('-');
                var tema = {
                    PodforumKomePripada: podforumTema[0],
                    Naslov: podforumTema[1]
                }

                TemeFactory.obrisiTemu(tema).then(function (response) {
                    if (response.data == true) {
                        PorukeFactory.posaljiPoruku(porukaZaAutoraZaljenogEntiteta).then(function (response) {
                            ZalbeFactory.obrisiZalbu(zalba).then(function (response) {
                                console.log(response.data);
                                alert('Entitet obrisan');
                                init();
                            });
                        });
                    }
                });

            }
            else if (zalba.TipEntiteta == 'Komentar') {

            }

            
        });

    }


});