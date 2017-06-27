webForum.controller('TemaController', function ($scope, PodforumiFactory, TemeFactory, KomentariFactory, $routeParams) {

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
        var username = sessionStorage.getItem("username");
        KomentariFactory.ostaviKomentarNaTemu(podforum, naslovTeme, tekstKomentara, username).then(function (response) {
            console.log(response.data);
            $scope.tekstKomentara = "";
            init();
        });
    }

    $scope.dodajPodkomentar = function (IdRoditelja, tekstPodkomentara) {
        var autor = sessionStorage.getItem("username");
        KomentariFactory.dodajPodkomentar(IdRoditelja, tekstPodkomentara, autor).then(function (response) {
            init();
        });
    }

});