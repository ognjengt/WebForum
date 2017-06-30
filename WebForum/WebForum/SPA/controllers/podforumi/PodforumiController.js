webForum.controller('PodforumiController', function ($scope, PodforumiFactory,$rootScope) {


    function init() {
        $rootScope.uspesnoRegistrovan = "";
        $scope.dodavanjePopupVisible = false;
        $scope.podforum = {};
        PodforumiFactory.getAllPodforums().then(function (response) {
            $scope.podforumi = response.data;
   
            $scope.podforumi.forEach(function (podforum) {
                // dodeljivanje putanje za sliku
                if (podforum.Ikonica.includes('.jpg') || podforum.Ikonica.includes('.png')) {
                    var spliter = podforum.Ikonica.split('.');
                    podforum.Ikonica = podforum.Ikonica + "." + spliter[1];
                }
                else {
                    podforum.Ikonica = "noimage.png";
                }
                
                // parsiranje stringa za noveredove
                podforum.Opis = podforum.Opis.replace(new RegExp('{novired}', 'g'), '\n');
                podforum.SpisakPravila = podforum.SpisakPravila.replace(new RegExp('{novired}', 'g'), '\n');
            });
        });
    }

    init();

    $scope.filesChanged = function (elm) {
        $scope.files = elm.files;
        $scope.$apply();
    }

    $scope.dodajPodforum = function (podforum) {
        //validacije
        if (podforum.naziv == "" || podforum.naziv == null) {
            alert('Popunite naziv podforuma');
            return;
        }
        else if (podforum.opis == "" || podforum.opis == null) {
            alert('Popunite opis podforuma');
            return;
        }
        else if (podforum.spisakPravila == "" || podforum.spisakPravila == null) {
            alert('Popunite spisak pravila podforuma');
            return;
        }

        
        podforum.opis = podforum.opis.replace(/(\r\n|\n|\r)/gm, "{novired}");
        podforum.spisakPravila = podforum.spisakPravila.replace(/(\r\n|\n|\r)/gm, "{novired}");


        // Zastita da samo administratori i moderatori mogu da dodaju nove podforume
        if (sessionStorage.getItem('uloga') == null) {
            return;
            if (!sessionStorage.getItem('uloga').includes('Administrator') || !sessionStorage.getItem('uloga').includes('Moderator')) {
                return;
            }
        }

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
        var izmenjenNazivSlike = "";
        if (nazivSlike != "") {
            var spliter = nazivSlike.split('.');
            izmenjenNazivSlike = podforum.naziv + "." + spliter[1];
        }

        podforum.ikonica = izmenjenNazivSlike;
        podforum.moderator = sessionStorage.getItem("username");

        PodforumiFactory.dodajPodforum(podforum).then(function (response) {
            if (response.data == null) {
                alert('Podforum sa ovim nazivom vec postoji');
                return;
            }
            init();
            // podforum dodat, poziva se funkcija za dodavanje slike ukoliko je slika prikacena
            if (izmenjenNazivSlike != "") {
                upload(izmenjenNazivSlike);
            }
            //else {
            //    $window.location.href = "#!/podforumi";
            //}
        }); 
    }

    function upload(nazivSlike) {
        var fd = new FormData();
        angular.forEach($scope.files, function (file) {
            fd.append('file', file);
        });

        PodforumiFactory.uploadImage(fd, nazivSlike).then(function (response) {
            // upload slike gotov
            console.log(response.data);
        });
    }

});