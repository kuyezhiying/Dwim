/* ========================================================================
 * @description
 *
 * Custom angularJS services which includes:
 *  == REST data service
 *
 * @dependency: angular-resource.js v1.4.6
 * ======================================================================== */

(function (angular) {
    'use strict';

    /**
     * REST data service
     * A simple service that can fetch data from backend 
     */
    var restDataService = angular.module('restDataService', ['ngResource']);

    var config = {
        restProject: window.location.origin + "/api/dwimApi/:action",
        restIngestionContext: "http://entityrepository.binginternal.com/Satori-production/?query=/IngestionDataSourceContext&responseFormat=JSON"
    };

    // Rest apis for project
    restDataService.factory('Project', [
        '$resource',
        function ($resource) {
            return $resource(config.restProject, {}, {

                // Correct Sentence
                sendRequestToMemory: { method: 'GET', params: { action: 'correctsentence' }, isArray: false },

                // Get Bing Entites
                getBingEntities: { method: 'GET', params: { action: 'getbingentities' }, isArray: false },
                recognizeBingEntitis: { method: 'GET', params: { action: 'recognizebingentities' }, isArray: false }
            });
        }
    ]);

}(angular))