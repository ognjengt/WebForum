var webForum = angular.module('webForum',['ngRoute']);

webForum.config(function ($routeProvider, $locationProvider) {
    $routeProvider.when('/',
	{
	    redirectTo: '/podforumi'

	}).when('/podforumi',
	{
	    controller: 'HomeController',
	    templateUrl: 'SPA/partials/pocetna.html',
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
    })

});
