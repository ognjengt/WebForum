webForum.controller('PodforumController', function ($scope, PodforumiFactory, TemeFactory, AccountFactory, $routeParams) {

    $scope.nazivPodforuma = $routeParams.naziv;

    function init() {
        $scope.dodavanjeTemePopupVisible = false;
        $scope.tema = {};
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

    $scope.filesChanged = function (elm) {
        $scope.files = elm.files;
        $scope.$apply();
    }

    $scope.dodajTemu = function (tema) {
        //validacije
        if (tema.naslov == null || tema.naslov == "") {
            alert('Popunite naslov teme');
            return;
        }
        else if (tema.tip == null || tema.tip == "") {
            alert('Popunite tip teme');
            return;
        }
        else if (tema.tip != 'Slika') {
            if (tema.sadrzaj == null || tema.sadrzaj == "") {
                alert('Popunite sadrzaj teme');
                return;
            }
            
        }

        tema.podforumKomePripada = $scope.podforum.Naziv;
        tema.autor = sessionStorage.getItem("username");

        if (tema.tip == 'Tekst' || tema.tip == 'Link') {
            // dodaj tekstualnu ili temu sa Linkom

            tema.sadrzaj = tema.sadrzaj.replace(/(\r\n|\n|\r)/gm, "{novired}");

            TemeFactory.dodajTemu(tema).then(function (response) {
                $scope.dodavanjeTemePopupVisible = false;
                if (response.data == null) {
                    alert('Tema sa ovim nazivom vec postoji');
                    return;
                }
                init();
            });
        }
        else if (tema.tip == 'Slika') {
            // dodaj temu sa slikom
            var fullPath = document.getElementById('slikaTeme').value;
            if (fullPath == null || fullPath == "") {
                alert('Niste odabrali sliku za upload');
                return;
            }
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
                izmenjenNazivSlike = tema.podforumKomePripada+'-'+tema.naslov + "." + spliter[1];
            }
            tema.sadrzaj = izmenjenNazivSlike;

            TemeFactory.dodajTemu(tema).then(function (response) {
                if (response.data == null) {
                    alert('Tema sa ovim nazivom vec postoji');
                    return;
                }
                init();
                // tema dodata, poziva se funkcija za dodavanje slike ukoliko je slika prikacena
                if (izmenjenNazivSlike != "") {
                    upload(izmenjenNazivSlike);
                }
                console.log(response.data);
            });
        }
    }

    function upload(nazivSlike) {
        var fd = new FormData();
        angular.forEach($scope.files, function (file) {
            fd.append('file', file);
        });

        TemeFactory.uploadImage(fd, nazivSlike).then(function (response) {
            // upload slike gotov
            console.log(response.data);
        });
    }

    $scope.sacuvajPodforum = function (nazivPodforuma, username) {
        AccountFactory.sacuvajPodforum(nazivPodforuma, username).then(function (response) {
            if (response.data == false) {
                alert('Vec pratite ovaj podforum!');
            }
            else alert('Podforum dodat u listu pracenih');
        });
    }

    $scope.thumbDown = function (tema) {
        //TODO
        alert('Sprzi mu thumb down');
    }

    $scope.thumbUp = function (tema) {
        // TODO
        alert('Sprzi mu thumb up');
        console.log(tema);
    }

});