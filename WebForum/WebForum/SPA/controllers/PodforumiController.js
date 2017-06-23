webForum.controller('PodforumiController', function ($scope, PodforumiFactory) {

    $scope.dodavanjePopupVisible = false;

    function init() {
        PodforumiFactory.getAllPodforums().then(function (response) {
            $scope.podforumi = response.data;
        });
    }

    init();

});