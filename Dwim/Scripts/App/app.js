/* ========================================================================
 * @description
 *
 * Init angular app and also do followings:
 *  == Add errorHandlingInterceptor for http request
 *  
 * @dependency: angular-resource.js v1.4.6
 * ======================================================================== */


/**
* Init angular module
*/

var app = angular.module('app', ['restDataService']);

/**
* The definition of 'errorHandlingInterceptor'. 
* If the http request fails, the error will be recorded in $scope.errors.
* $scope.errors will be displayed at the top of each page. 
*/
app.factory('errorHandlingInterceptor', ['$q', '$rootScope',
    function ($q, $rootScope) {
        return {
            'responseError': function (rejection) {
                console.debug("Failed request:", rejection); // for debugging
                $rootScope.errors = []; // if errors object doesn't exist, create one
                $rootScope.errors.push(rejection); // append error
                return $q.reject(rejection);
            }
        };
    }]);

/**
* Regesiter 'errorHandlingInterceptor'
*/
app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('errorHandlingInterceptor');
}]);

/**
* app init
* 1. Add callback function for error event.
*/
app.run(function ($rootScope) {
    $rootScope.errors = [];
    $rootScope.$watchCollection('errors', function (newErrors, oldErrors) {
        if (newErrors !== oldErrors) {
            $('html,body').animate({ scrollTop: 0 }, 0);
        }
    });

    // jQuery events
    $("body").delegate('[data-toggle="popover"]', "mouseover", function (event) {
        $(this).popover('show');
    }).delegate('[data-toggle="popover"]', "mouseleave", function (event) {
        $(this).popover('hide');
    });

});