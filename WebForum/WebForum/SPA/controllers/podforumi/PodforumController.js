webForum.controller('PodforumController', function ($scope, PodforumiFactory, $routeParams) {

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

        });
    }

    init();

});