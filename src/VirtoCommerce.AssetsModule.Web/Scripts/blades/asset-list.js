angular.module('virtoCommerce.assetsModule')
    .controller('virtoCommerce.assetsModule.assetListController', ['$scope', '$translate', 'platformWebApp.assets.api',
        'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper',
        function ($scope, $translate, assets, bladeNavigationService, dialogService, bladeUtils, uiGridHelper) {
            var blade = $scope.blade;
            blade.title = 'platform.blades.asset-list.title';
            if (!blade.currentEntity) {
                blade.currentEntity = {};
            }

            blade.refresh = function () {
                blade.isLoading = true;
                assets.search(
                    {
                        keyword: blade.searchKeyword,
                        folderUrl: blade.currentEntity.url
                    },
                    function (data) {
                        $scope.pageSettings.totalItems = data.totalCount;
                        _.each(data.results, function (x) {
                            x.isImage = x.contentType && x.contentType.startsWith('image/');
                            if (x.isImage) {
                                const delimiter = x.url?.contains('?') ? '&' : '?';
                                x.noCacheUrl = `${x.url}${delimiter}t=${x.modifiedDate}`;
                            }
                        });
                        $scope.listEntries = data.results;
                        blade.isLoading = false;

                        //Set navigation breadcrumbs
                        setBreadcrumbs();
                    });
            };

            //Breadcrumbs
            function setBreadcrumbs() {
                blade.breadcrumbs = blade.breadcrumbs
                    ? getUniqueBreadcrumbs()
                    : [generateBreadcrumb(blade.currentEntity.url, 'platform.blades.asset-list.bread-crumb-top')];
            }

            function getUniqueBreadcrumbs() {
                //Clone array (angular.copy leaves the same reference)
                var breadcrumbs = blade.breadcrumbs.slice(0);

                //prevent duplicate items
                if (blade.currentEntity.url && _.all(breadcrumbs,
                    function (x) {
                        return x.id !== blade.currentEntity.url
                    })) {
                    var breadCrumb = generateBreadcrumb(blade.currentEntity.url, blade.currentEntity.name);
                    breadcrumbs.push(breadCrumb);
                }
                return breadcrumbs;
            }

            function generateBreadcrumb(id, name) {
                return {
                    id: id,
                    name: name,
                    blade: blade,
                    navigate: function (breadcrumb) {
                        bladeNavigationService.closeBlade(blade,
                            function () {
                                blade.disableOpenAnimation = true;
                                bladeNavigationService.showBlade(blade, blade.parentBlade);
                            });
                    }
                }
            }

            function newFolder() {
                var tooltip = $translate.instant('platform.dialogs.create-folder.title');
                var result = prompt(tooltip + "\n\nEnter folder name:");
                if (result != null) {
                    assets.createFolder({ name: result, parentUrl: blade.currentEntity.url }, blade.refresh);
                }
            }

            $scope.copyUrl = function (data) {
                window.prompt("Copy to clipboard: Ctrl+C, Enter", data.url);
            };

            $scope.downloadUrl = function (data) {
                try {
                    var link = document.createElement('a');
                    link.href = data.url;
                    link.download = data.name || 'download'; 
                    document.body.appendChild(link);

                    link.click();

                    document.body.removeChild(link);
                } catch (err) {
                    console.warn('Download fallback: opening in new tab due to browser restriction', err);
                    // Fallback: open in new tab
                    window.open(data.url, '_blank');
                }
            };

            function isItemsChecked() {
                return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
            }

            $scope.delete = function (data) {
                deleteList([data]);
            };

            function deleteList(selection) {
                bladeNavigationService.closeChildrenBlades(blade, function () {
                    var dialog = {
                        id: "confirmDeleteItem",
                        title: "platform.dialogs.folders-delete.title",
                        message: "platform.dialogs.folders-delete.message",
                        callback: function (remove) {
                            if (remove) {
                                var listEntryIds = _.pluck(selection, 'url');
                                assets.remove({ urls: listEntryIds },
                                    blade.refresh,
                                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                            }
                        }
                    }
                    dialogService.showConfirmationDialog(dialog);
                });
            }

            $scope.selectNode = function (listItem) {
                if (listItem.type === 'folder') {
                    var newBlade = {
                        id: blade.id,
                        breadcrumbs: blade.breadcrumbs,
                        currentEntity: listItem,
                        disableOpenAnimation: true,
                        controller: blade.controller,
                        template: blade.template,
                        isClosingDisabled: blade.isClosingDisabled
                    };

                    bladeNavigationService.showBlade(newBlade, blade.parentBlade);
                }
            };

            blade.headIcon = 'fa fa-folder-o';

            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh", icon: 'fa fa-refresh',
                    title: "platform.commands.titles.refresh",
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.new-folder", icon: 'fa fa-folder-o',
                    title: "platform.commands.titles.new-folder",
                    executeMethod: function () { newFolder(); },
                    canExecuteMethod: function () {
                        return true;
                    },
                    permission: 'platform:asset:create'
                },
                {
                    name: "platform.commands.upload", icon: 'fa fa-upload',
                    title: "platform.commands.titles.upload",
                    executeMethod: function () {
                        var newBlade = {
                            id: "assetUpload",
                            currentEntityId: blade.currentEntity.url,
                            title: 'platform.blades.asset-upload.title',
                            controller: 'virtoCommerce.assetsModule.assetUploadController',
                            template: 'Modules/$(VirtoCommerce.Assets)/Scripts/blades/asset-upload.tpl.html'
                        };
                        bladeNavigationService.showBlade(newBlade, blade);
                    },
                    canExecuteMethod: function () {
                        return true;
                    },
                    permission: 'platform:asset:create'
                },
                {
                    name: "platform.commands.delete", icon: 'fas fa-trash-alt',
                    executeMethod: function () { deleteList($scope.gridApi.selection.getSelectedRows()); },
                    canExecuteMethod: isItemsChecked,
                    permission: 'platform:asset:delete'
                }
            ];

            // ui-grid
            $scope.setGridOptions = function (gridOptions) {
                uiGridHelper.initialize($scope, gridOptions,
                    function (gridApi) {
                        $scope.$watch('pageSettings.currentPage', gridApi.pagination.seek);
                    });
            };
            bladeUtils.initializePagination($scope, true);

            blade.refresh();
        }]);
