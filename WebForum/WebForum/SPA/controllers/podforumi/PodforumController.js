webForum.controller('PodforumController', function ($scope, PodforumiFactory, TemeFactory, $routeParams) {

    $scope.nazivPodforuma = $routeParams.naziv;

    function init() {
        $scope.dodavanjeTemePopupVisible = false;
        PodforumiFactory.getPodforumByNaziv($scope.nazivPodforuma).then(function (response) {
            $scope.podforum = response.data;
            if ($scope.podforum.Ikonica.includes('.jpg') || $scope.podforum.Ikonica.includes('.png')) {
                var spliter = $scope.podforum.Ikonica.split('.');
                $scope.podforum.Ikonica = $scope.podforum.Ikonica + "." + spliter[1];
                
            } else {
                $scope.podforum.Ikonica = "noimage.png";
            }
            console.log(response.data);
            
            $scope.podforum.Opis = $scope.podforum.Opis.replace(new RegExp('{novired}', 'g'), '\n');
            $scope.podforum.SpisakPravila = $scope.podforum.SpisakPravila.replace(new RegExp('{novired}', 'g'), '\n');

            TemeFactory.getTemeZaPodforum($scope.nazivPodforuma).then(function (response) {
                $scope.temePodforuma = response.data;
                $scope.temePodforuma.forEach(function (tema) {
                    tema.DatumKreiranja = new Date(tema.DatumKreiranja).toLocaleDateString();
                })
                console.log(response.data);
            });
        });
    }

    init();

});