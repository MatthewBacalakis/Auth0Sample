(()=>{
    'use strict';
    angular.module("app").service("a0sItemService", ItemsService);

    ItemsService.$inject = ['$http', 'itemServiceEndpoint', '$log', '$q'];

    function ItemsService($http, itemServiceEndpoint, $log, $q){
        return {
            CallItemService: CallItemService
        }

        function CallItemService(method) {
            //token attached to requests by tokenInterceptor
            return $q((resolve, reject)=>{
                $http({
                        method: method,
                        url: itemServiceEndpoint
                    })
                    .then(resolve)
                    .catch((err)=>{
                        $log.error(err);
                        reject(err);
                    });
            });
        }
    }
    


})();