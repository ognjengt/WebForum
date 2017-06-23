webForum.factory('PodforumiFactory', function ($http) {

    var factory = {};
    
    factory.getAllPodforums = function () {
        return $http.get('/api/Podforumi/GetAll');
    }

    return factory;

});