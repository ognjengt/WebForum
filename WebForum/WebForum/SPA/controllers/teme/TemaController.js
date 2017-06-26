webForum.controller('TemaController', function ($scope, PodforumiFactory, TemeFactory, KomentariFactory, $routeParams) {

    $scope.nazivPodforuma = $routeParams.naziv;
    $scope.nazivTeme = $routeParams.tema;

    function init() {
        $scope.tema = {};

        TemeFactory.getTemaByNaziv($scope.nazivTeme, $scope.nazivPodforuma).then(function (response) {
            $scope.tema = response.data;
            $scope.tema.DatumKreiranja = new Date($scope.tema.DatumKreiranja).toLocaleDateString();

            KomentariFactory.getKomentariZaTemu($scope.nazivPodforuma, $scope.nazivTeme).then(function (response) {
                $scope.komentari = response.data;
                $scope.komentari.forEach(function (komentar) {
                    komentar.DatumKomentara = new Date($scope.tema.DatumKreiranja).toLocaleDateString();
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

});