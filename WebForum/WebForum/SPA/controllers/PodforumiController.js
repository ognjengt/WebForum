webForum.controller('PodforumiController', function ($scope, PodforumiFactory) {

    $scope.dodavanjePopupVisible = false;

    function init() {
        PodforumiFactory.getAllPodforums().then(function (response) {
            $scope.podforumi = response.data;
        });
    }

    init();

    $scope.filesChanged = function (elm) {
        $scope.files = elm.files;
        $scope.$apply();
    }

    $scope.dodajPodforum = function (podforum) {

        ////var fullPath = document.getElementById('slikaPodforuma').value;
        ////if (fullPath) {
        ////    var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
        ////    var filename = fullPath.substring(startIndex);
        ////    if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
        ////        filename = filename.substring(1);
        ////    }
        ////    podforum.ikonica = fullPath;
        ////}
        //var f = document.getElementById('slikaPodforuma').files[0];
        //var r = new FileReader();

        //r.onloadend = function (e) {
        //    var data = e.target.result;
        //    //send your binary data via $http or $resource or do anything else with it
        //    podforum.ikonica = data;
        //    podforum.moderator = sessionStorage.getItem("username");
        //    console.log(podforum);
        //    //PodforumiFactory.dodajPodforum(podforum).then(function (response) {
        //    //    console.log(response.data);
        //    //});
        //}

        upload(podforum.naziv);
    }

    function upload(nazivPodforuma) {
        var fd = new FormData();
        angular.forEach($scope.files, function (file) {
            fd.append('file', file);
        });

        var fullPath = document.getElementById('slikaPodforuma').value;
        var nazivSlike;
        if (fullPath) {
            var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
            var filename = fullPath.substring(startIndex);
            if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                filename = filename.substring(1);
            }

            nazivSlike = filename;
        }
        var izmenjenNazivSlike;
        var spliter = nazivSlike.split('.');
        izmenjenNazivSlike = nazivPodforuma + "." + spliter[1];

        PodforumiFactory.uploadImage(fd,izmenjenNazivSlike).then(function (response) {
            console.log(response.data);
        });
    }

});