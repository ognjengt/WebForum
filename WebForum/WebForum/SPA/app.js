var webForum = angular.module('webForum',['ngRoute']);

webForum.config(function ($routeProvider) {
    $routeProvider.when('/',
	{
	    redirectTo: '/pocetna'

	}).when('/pocetna', {

	    controller: 'PocetnaController',
	    templateUrl: 'SPA/partials/pocetna.html',
	    activetab: 'pocetna'

	}).when('/podforumi',
	{
	    controller: 'PodforumiController',
	    templateUrl: 'SPA/partials/podforumi/podforumi.html',
	    activeTab: 'podforumi'

	}).when('/login',
    {
        controller: 'LoginController',
        templateUrl: 'SPA/partials/login.html',
        activeTab: 'login'

    }).when('/register',
    {
        controller: 'LoginController',
        templateUrl: 'SPA/partials/register.html',
        activeTab: 'register'

    }).when('/podforumi/:naziv',
    {
        controller: 'PodforumController',
        templateUrl: 'SPA/partials/podforumi/pregledPodforuma.html',
        activetab: 'nijedan'

    }).when('/podforumi/:naziv/:tema',
    {
        controller: 'TemaController',
        templateUrl: 'SPA/partials/teme/pregledTeme.html',
        activetab: 'nijedan'

    }).when('/profil/:username', {

        controller: 'ProfilController',
        templateUrl: 'SPA/partials/profil.html',
        activetab: 'nijedan'

    }).when('/pretraga', {

        controller: 'PretragaController',
        templateUrl: 'SPA/partials/pretraga.html',
        activetab: 'pretraga'

    }).when('/promenaTipa', {

        controller: 'PromenaTipaController',
        templateUrl: 'SPA/partials/promenaTipa.html',
        activetab: 'promenaTipa'

    }).when('/zalbe', {

        controller: 'ZalbeController',
        templateUrl: 'SPA/partials/zalbe.html',
        activetab: 'zalbe'

    })

});
