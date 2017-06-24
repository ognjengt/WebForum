webForum.controller('PodforumiController', function ($scope, PodforumiFactory) {

    $scope.dodavanjePopupVisible = false;

    function init() {
        PodforumiFactory.getAllPodforums().then(function (response) {
            $scope.podforumi = response.data;
            $scope.podforumi.forEach(function (podforum) {
                if (podforum.Ikonica.includes('.jpg') || podforum.Ikonica.includes('.png')) {
                    var spliter = podforum.Ikonica.split('.');
                    podforum.Ikonica = podforum.Ikonica + "." + spliter[1];
                }
                else {
                    podforum.Ikonica = "noimage.png";
                }
                
            });
        });
    }

    init();

    $scope.filesChanged = function (elm) {
        $scope.files = elm.files;
        $scope.$apply();
    }

    $scope.dodajPodforum = function (podforum) {

        // na osnovu id-a sa stranice uzima naziv i ekstenziju slike
        var fullPath = document.getElementById('slikaPodforuma').value;
        var nazivSlike = "";
        if (fullPath) {
            var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
            var filename = fullPath.substring(startIndex);
            if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                filename = filename.substring(1);
            }

            nazivSlike = filename;
        }
        // menjam naziv slike da se zove onako kako se zove podforum i pozivam factory
        var izmenjenNazivSlike;
        if (nazivSlike != null || nazivSlike != "") {
            var spliter = nazivSlike.split('.');
            izmenjenNazivSlike = podforum.naziv + "." + spliter[1];
        }
        else izmenjenNazivSlike = null;

        podforum.ikonica = izmenjenNazivSlike;
        podforum.moderator = sessionStorage.getItem("username");

        PodforumiFactory.dodajPodforum(podforum).then(function (response) {
            if (izmenjenNazivSlike != null) {
                upload(izmenjenNazivSlike);
            }
            
        });

        
    }

    function upload(nazivSlike) {
        var fd = new FormData();
        angular.forEach($scope.files, function (file) {
            fd.append('file', file);
        });

        PodforumiFactory.uploadImage(fd,nazivSlike).then(function (response) {
            console.log(response.data);
        });
    }

});