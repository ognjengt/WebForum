﻿webForum.controller('TemaController', function ($scope, PodforumiFactory, TemeFactory, KomentariFactory, AccountFactory, $routeParams) {

    $scope.nazivPodforuma = $routeParams.naziv;
    $scope.nazivTeme = $routeParams.tema;

    function init() {
        $scope.tema = {};

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
                    komentar.Podkomentari.forEach(function (podkomentar) {
                        podkomentar.DatumKomentara = new Date(podkomentar.DatumKomentara).toLocaleDateString();
                    });
                })
            });
        });
    }

    init();

    $scope.komentarisiTemu = function (podforum, naslovTeme, tekstKomentara) {
        if (tekstKomentara == null || tekstKomentara == "") {
            alert('Popunite sadrzaj komentara');
            return;
        }
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
        var autor = sessionStorage.getItem("username");
        var temaKojojPripada = podforum + '-' + tema;
        KomentariFactory.dodajPodkomentar(IdRoditelja, tekstPodkomentara, autor, temaKojojPripada).then(function (response) {
            init();
        });
    }

    $scope.zapratiTemu = function (username) {
        var naslovTeme = $scope.nazivPodforuma + '-' + $scope.nazivTeme;
        AccountFactory.zapratiTemu(naslovTeme, username).then(function (response) {
            if (response.data == false) {
                alert('Vec pratite ovu temu!');
            }
            else alert('Tema dodata u listu pracenih');
        });
    }

});