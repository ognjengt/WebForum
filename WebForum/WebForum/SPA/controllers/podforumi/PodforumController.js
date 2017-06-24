webForum.controller('PodforumController', function ($scope, PodforumiFactory, TemeFactory, $routeParams) {

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

    $scope.dodajTemu = function (tema) {
        //validacije
        if (tema.naslov == null || tema.naslov == "") {
            alert('Popunite naslov teme');
            return;
        }
        else if (tema.opis == null || tema.opis == "") {
            alert('Popunite opis teme');
            return;
        }
        else if (tema.tip == null || tema.tip == "") {
            alert('Popunite tip teme');
            return;
        }
        else if (tema.sadrzaj == null || tema.sadrzaj == "") {
            alert('Popunite sadrzaj teme');
            return;
        }

        tema.podforumKomePripada = $scope.podforum.Naziv;
        tema.autor = sessionStorage.getItem("username");

        if (tema.tip == 'Tekst' || tema.tip == 'Link') {
            // dodaj tekstualnu ili temu sa Linkom
            TemeFactory.dodajTemu(tema).then(function (response) {
                console.log(response.data);
            });
        }
        else if (tema.tip == 'Slika') {
            // dodaj temu sa slikom
        }
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