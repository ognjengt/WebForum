webForum.controller('TemaController', function ($scope, PodforumiFactory, TemeFactory, KomentariFactory, AccountFactory, $routeParams, $rootScope, $window) {

    $scope.nazivPodforuma = $routeParams.naziv;
    $scope.nazivTeme = $routeParams.tema;

    function init() {
        $scope.tema = {};
        $scope.komentarZaIzmenu = '';
        $scope.podkomentarZaIzmenu = '';

        $scope.izmenaModalWindowVisible = false;

        TemeFactory.getTemaByNaziv($scope.nazivTeme, $scope.nazivPodforuma).then(function (response) {
            $scope.tema = response.data;
            $scope.tema.DatumKreiranja = new Date($scope.tema.DatumKreiranja).toLocaleDateString();

            // dodeljivanje putanje za sliku
            if ($scope.tema.Sadrzaj.includes('.jpg') || $scope.tema.Sadrzaj.includes('.png')) {
                var spliter = $scope.tema.Sadrzaj.split('.');
                $scope.tema.Sadrzaj = $scope.tema.Sadrzaj + "." + spliter[1];
            }

            // parsiranje stringa za noveredove
            $scope.tema.Sadrzaj = $scope.tema.Sadrzaj.replace(new RegExp('{novired}', 'g'), '\n');

            KomentariFactory.getKomentariZaTemu($scope.nazivPodforuma, $scope.nazivTeme).then(function (response) {
                console.log(response.data);
                $scope.komentari = response.data;
                $scope.komentari.forEach(function (komentar) {
                    komentar.DatumKomentara = new Date(komentar.DatumKomentara).toLocaleDateString();
                    komentar.Tekst = komentar.Tekst.replace(new RegExp('{novired}', 'g'), '\n');
                    komentar.Podkomentari.forEach(function (podkomentar) {
                        podkomentar.DatumKomentara = new Date(podkomentar.DatumKomentara).toLocaleDateString();
                        podkomentar.Tekst = podkomentar.Tekst.replace(new RegExp('{novired}', 'g'), '\n');
                    });
                });

                //Uzmi podatke od podforuma u kom se tema nalazi da bih mogao da radim sa moderatorima itd
                PodforumiFactory.getPodforumByNaziv($scope.nazivPodforuma).then(function (response) {
                    console.log(response.data);
                    $scope.JeUListiModeratora = false;
                    $scope.podforumUKomeSeTemaNalazi = response.data;
                    $scope.podforumUKomeSeTemaNalazi.Moderatori.forEach(function (moderator) {
                        if (moderator == sessionStorage.getItem("username")) {
                            $scope.JeUListiModeratora = true;
                        }
                    });
                });

            });
        });
    }

    init();

    // -------------------------------------------------------------------------------- Komentarisanje

    $scope.komentarisiTemu = function (podforum, naslovTeme, tekstKomentara) {
        if (tekstKomentara == null || tekstKomentara == "") {
            alert('Popunite sadrzaj komentara');
            return;
        }
        tekstKomentara = tekstKomentara.replace(/(\r\n|\n|\r)/gm, "{novired}");
        var username = sessionStorage.getItem("username");
        KomentariFactory.ostaviKomentarNaTemu(podforum, naslovTeme, tekstKomentara, username).then(function (response) {
            console.log(response.data);
            $scope.tekstKomentara = "";
            init();
        });
    }

    $scope.dodajPodkomentar = function (IdRoditelja, tekstPodkomentara, podforum, tema) {
        if (tekstPodkomentara == null || tekstPodkomentara == "") {
            alert('Popunite sadrzaj komentara');
            return;
        }
        tekstPodkomentara = tekstPodkomentara.replace(/(\r\n|\n|\r)/gm, "{novired}");
        var autor = sessionStorage.getItem("username");
        var temaKojojPripada = podforum + '-' + tema;
        KomentariFactory.dodajPodkomentar(IdRoditelja, tekstPodkomentara, autor, temaKojojPripada).then(function (response) {
            init();
        });
    }

    // -------------------------------------------------------------------------------- Cuvanje entiteta

    $scope.zapratiTemu = function (username) {
        var naslovTeme = $scope.nazivPodforuma + '-' + $scope.nazivTeme;
        AccountFactory.zapratiTemu(naslovTeme, username).then(function (response) {
            if (response.data == false) {
                alert('Vec pratite ovu temu!');
            }
            else alert('Tema dodata u listu pracenih');
        });
    }

    $scope.sacuvajKomentar = function (idKomentara, username) {
        AccountFactory.sacuvajKomentar(idKomentara, username).then(function (response) {
            if (response.data == false) {
                alert('Vec ste sacuvali ovaj komentar!');
            } else alert('Komentar uspesno sacuvan!');
            console.log(response.data);
        });
    }

    // -------------------------------------------------------------------------------- Lajkovanje

    $scope.thumbsUp = function (komentar) {
        if (!$rootScope.ulogovan) {
            alert('Ulogujte se da bi ste dali glas komentaru!');
            return;
        }
        KomentariFactory.ThumbsUp(komentar, sessionStorage.getItem("username")).then(function (response) {
            console.log(response.data);
            if (response.data == false) {
                alert('Vec ste dali pozitivan glas ovom komentaru');
            }
            else init();
        });
    }

    $scope.thumbsDown = function (komentar) {
        if (!$rootScope.ulogovan) {
            alert('Ulogujte se da bi ste dali glas komentaru!');
            return;
        }
        KomentariFactory.ThumbsDown(komentar, sessionStorage.getItem("username")).then(function (response) {
            console.log(response.data);
            if (response.data == false) {
                alert('Vec ste dali negativan glas ovom komentaru');
            }
            else init();
        });
    }

    // -------------------------------------------------------------------------------- Brisanje

    $scope.obrisiPodkomentar = function (podkomentar) {
        KomentariFactory.obrisiPodkomentar(podkomentar).then(function (response) {
            console.log(response.data);
            init();
        });
    }

    $scope.obrisiKomentar = function (komentar) {
        KomentariFactory.obrisiKomentar(komentar).then(function (response) {
            console.log(response.data);
            init();
        });
    }

    $scope.obrisiTemu = function (tema) {
        TemeFactory.obrisiTemu(tema).then(function (response) {
            console.log(response.data);

            $window.location.href = "#!/podforumi"
        });
    }

    // -------------------------------------------------------------------------------- Izmena

    $scope.showIzmenaPopupWindow = function () {
        $scope.izmenaModalWindowVisible = !$scope.izmenaModalWindowVisible;
        $scope.noviSadrzajTeme = $scope.tema.Sadrzaj;
    }

    $scope.setKomentarZaIzmenu = function (komentar) {
        $scope.komentarZaIzmenu = komentar.Id;
        $scope.tekstKomentaraZaIzmenu = komentar.Tekst;
    }

    $scope.setPodkomentarZaIzmenu = function (komentar) {
        $scope.podkomentarZaIzmenu = komentar.Id;
        $scope.tekstPodkomentaraZaIzmenu = komentar.Tekst;
    }

    $scope.izmeniKomentar = function (idKomentara, tekstKomentara, tipKorisnika) {

        var prikaziDaJeIzmenjeno = false;
        // prodji kroz listu na sesiji za koje je taj korisnik odgovorni moderator, ako u toj listi nigde ne postoji ime ovog podforuma,
        // stavi prikaziDaJeIzmenjeno = true, ili ako mu je uloga obican korisnik
        if (tipKorisnika == 'Korisnik') {
            prikaziDaJeIzmenjeno = true;
        }
        tekstKomentara = tekstKomentara.replace(/(\r\n|\n|\r)/gm, "{novired}");
        KomentariFactory.izmeniKomentar(idKomentara, tekstKomentara, prikaziDaJeIzmenjeno).then(function (response) {
            console.log(response.data);
            init();
        });
    }

    $scope.izmeniPodkomentar = function (idKomentara, tekstKomentara, tipKorisnika) {

        var prikaziDaJeIzmenjeno = false;
        // prodji kroz listu na sesiji za koje je taj korisnik odgovorni moderator, ako u toj listi nigde ne postoji ime ovog podforuma,
        // stavi prikaziDaJeIzmenjeno = true, ili ako mu je uloga obican korisnik
        if (tipKorisnika == 'Korisnik') {
            prikaziDaJeIzmenjeno = true;
        }
        tekstKomentara = tekstKomentara.replace(/(\r\n|\n|\r)/gm, "{novired}");
        KomentariFactory.izmeniPodkomentar(idKomentara, tekstKomentara,prikaziDaJeIzmenjeno).then(function (response) {
            console.log(response.data);
            init();
        });
    }

    $scope.izmeniTemu = function (tema, noviSadrzaj) {
        noviSadrzaj = noviSadrzaj.replace(/(\r\n|\n|\r)/gm, "{novired}");
        TemeFactory.izmeniTemu(tema, noviSadrzaj).then(function (response) {
            console.log(response.data);
            init();
        });
    }

});