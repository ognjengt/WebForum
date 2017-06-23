webForum.controller('PodforumiController', function ($scope, PodforumiFactory) {

    function init() {
        PodforumiFactory.getAllPodforums().then(function (response) {
            $scope.podforumi = response.data;
        });
    }

    init();

});