angular.module('virtoCommerce.assetsModule')
.factory('platformWebApp.assets.api', ['$resource', function ($resource) {
    return $resource('api/assets', {}, {
        search: { method: 'GET', url: 'api/assets', isArray: false },
        createFolder: { method: 'POST', url: 'api/assets/folder' },
        move: { method: 'POST', url: 'api/assets/move' },
        uploadFromUrl: { method: 'POST', params: { url: '@url', folderUrl: '@folderUrl', name: '@name' }, isArray: true }
    });
}]);

