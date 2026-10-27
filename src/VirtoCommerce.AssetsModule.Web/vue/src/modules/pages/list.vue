<template>
  <VcBlade
    :title="$t('ASSETS.PAGES.LIST.TITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:search-value="searchValue"
      v-model:selection="selection"
      selection-mode="multiple"
      :items="pageItems"
      :total-count="items.length"
      :pagination="pagination"
      :searchable="true"
      state-key="ASSETS"
      :active-item-id="activeItemId"
      :row-actions="rowActions"
      :empty-state="{ icon: 'lucide-folder-open', title: $t('ASSETS.PAGES.LIST.EMPTY') }"
      @row-click="onRowClick"
      @pagination-click="pagination.goToPage"
    >
      <template #header>
        <div class="assets-list__crumbs">
          <VcBreadcrumbs :items="breadcrumbs" />
        </div>
      </template>
      <VcColumn
        id="preview"
        :title="$t('ASSETS.PAGES.LIST.COLUMNS.PICTURE')"
        width="96px"
        always-visible
      >
        <template #body="{ data: asset }">
          <span class="assets-list__thumb">
            <img
              v-if="isImage(asset)"
              :src="previewUrl(asset)"
              :alt="asset.name"
              loading="lazy"
            />
            <VcIcon
              v-else
              :icon="asset.type === 'folder' ? 'lucide-folder' : 'lucide-file'"
              size="l"
            />
          </span>
        </template>
      </VcColumn>
      <VcColumn
        id="name"
        :title="$t('ASSETS.PAGES.LIST.COLUMNS.NAME')"
        body-class="assets-list__name"
        always-visible
      />
      <VcColumn
        id="size"
        :title="$t('ASSETS.PAGES.LIST.COLUMNS.SIZE')"
        align="end"
      >
        <template #body="{ data: asset }">{{ formatSize(asset.size) }}</template>
      </VcColumn>
      <VcColumn
        id="modifiedDate"
        :title="$t('ASSETS.PAGES.LIST.COLUMNS.MODIFIED')"
        type="date"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import {
  useApiClient,
  useAsync,
  useBlade,
  useDataTablePagination,
  useFunctions,
  useLoading,
  usePermissions,
  usePopup,
  type Breadcrumbs,
  type IBladeToolbar,
  type TableAction,
} from "@vc-shell/framework";
import { VcBlade, VcBreadcrumbs, VcColumn, VcDataTable, VcIcon } from "@vc-shell/framework/ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { AssetsClient } from "../../api_client/virtocommerce.assets";
import { download, errorText, formatSize, isImage, previewUrl, useAssetActions, type Asset } from "../utils";

defineBlade({
  url: "/assets",
  name: "AssetsBrowser",
  isWorkspace: true,
  // opened from another module (a store's folder) its root comes through options: a reload restores that module's
  // blade instead of a storage-root browser (the workspace itself still restores from its url)
  routable: false,
  menuItem: {
    title: "ASSETS.MENU.TITLE",
    icon: "lucide-image",
    priority: 50,
  },
});

const { t } = useI18n({ useScope: "global" });
// Another module can open the browser on one folder (options.folderUrl, e.g. a store's "/stores/{id}"): that folder
// is then the root the trail starts and stops at, as the AngularJS list opened with a currentEntity was
const { options, openBlade, closeChildren, exposeToChildren } = useBlade<{ folderUrl?: string; title?: string }>();
const { showConfirmation, showError } = usePopup();
const { hasAccess } = usePermissions();
const { debounce } = useFunctions();
const { copyLink, prompt } = useAssetActions();
const { getApiClient } = useApiClient(AssetsClient);

const PAGE_SIZE = 50;

// the folder trail below the root; the last entry is the open folder (empty = the root)
type Trail = { name: string; url: string }[];
const path = ref<Trail>([]);
const folderUrl = computed(() => path.value[path.value.length - 1]?.url ?? options.value?.folderUrl);

const items = ref<Asset[]>([]);
const selection = ref<Asset[]>([]);
const activeItemId = ref<string>();
const searchValue = ref<string>();
const totalCount = computed(() => items.value.length);
const pagination = useDataTablePagination({ pageSize: PAGE_SIZE, totalCount });
const pageItems = computed(() => items.value.slice(pagination.skip, pagination.skip + PAGE_SIZE));

const { loading: listLoading, action: load } = useAsync(async () => {
  const client = await getApiClient();
  const result = await client.searchAssetItems(folderUrl.value, searchValue.value || undefined);
  // the table keys rows by id; blobs have none, the url is unique
  items.value = ((result.results ?? []) as Asset[]).map((x) => ({ ...x, id: x.url }));
  selection.value = [];
});

// deleteAssets shows its own error popup
const { loading: deleteLoading, action: remove } = useAsync<Asset[]>(
  async (assets) => {
    const client = await getApiClient();
    await client.deleteBlobs((assets ?? []).map((x) => x.url!));
  },
  { notify: false },
);

const loading = useLoading(listLoading, deleteLoading);

const breadcrumbs = computed<Breadcrumbs[]>(() => [
  { id: "", title: options.value?.title ?? t("ASSETS.PAGES.LIST.ROOT"), icon: "lucide-house", clickHandler: () => goTo([]) },
  ...path.value.map((x, i) => ({ id: x.url, title: x.name, clickHandler: () => goTo(path.value.slice(0, i + 1)) })),
]);

async function goTo(trail: Trail) {
  await closeChildren();
  path.value = trail;
  pagination.setPage(1);
  if (searchValue.value) searchValue.value = undefined; // the search watcher reloads
  else await load();
}

const enterFolder = (folder: Asset) => goTo([...path.value, { name: folder.name ?? "", url: folder.url ?? "" }]);

function openDetails(asset: Asset) {
  activeItemId.value = asset.id ?? undefined;
  openBlade({
    name: "AssetsFileDetails",
    options: { asset, folderUrl: folderUrl.value },
    onClose() {
      activeItemId.value = undefined;
    },
  });
}

function onRowClick(event: { data: Asset }) {
  if (event.data.type === "folder") enterFolder(event.data);
  else openDetails(event.data);
}

async function deleteAssets(assets: Asset[]) {
  if (!(await showConfirmation(t("ASSETS.ALERTS.DELETE_CONFIRMATION", { count: assets.length })))) return;
  await closeChildren();
  try {
    await remove(assets);
  } catch (e) {
    showError(errorText(e));
  }
  await load();
}

async function newFolder() {
  const name = await prompt(t("ASSETS.PROMPT.NEW_FOLDER"), t("ASSETS.PROMPT.FOLDER_NAME"));
  if (!name) return;
  try {
    const client = await getApiClient();
    await client.createBlobFolder({ name, parentUrl: folderUrl.value });
  } catch (e) {
    showError(errorText(e));
  }
  await load();
}

function upload() {
  if (!folderUrl.value) {
    showError(t("ASSETS.ALERTS.ROOT_UPLOAD"));
    return;
  }
  openBlade({
    name: "AssetsUpload",
    options: { folderUrl: folderUrl.value, folderName: path.value[path.value.length - 1]?.name ?? options.value?.title },
  });
}

const rowActions = (asset: Asset): TableAction[] => {
  const isFolder = asset.type === "folder";
  return [
    { id: "open", icon: "lucide-folder-open", title: t("ASSETS.ACTIONS.OPEN"), type: "info", hidden: !isFolder, clickHandler: () => enterFolder(asset) },
    { id: "download", icon: "lucide-download", title: t("ASSETS.ACTIONS.DOWNLOAD"), type: "info", hidden: isFolder, clickHandler: () => download(asset) },
    { id: "copy", icon: "lucide-link", title: t("ASSETS.ACTIONS.COPY_LINK"), type: "info", clickHandler: () => copyLink(asset) },
    {
      id: "delete",
      icon: "lucide-trash-2",
      title: t("ASSETS.ACTIONS.DELETE"),
      type: "danger",
      variant: "danger",
      hidden: !hasAccess("platform:asset:delete"),
      clickHandler: () => deleteAssets([asset]),
    },
  ];
};

const debouncedLoad = debounce(load, 300);
watch(searchValue, () => {
  pagination.setPage(1);
  debouncedLoad();
});
load();

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("ASSETS.PAGES.LIST.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  {
    id: "newFolder",
    title: t("ASSETS.PAGES.LIST.TOOLBAR.NEW_FOLDER"),
    icon: "lucide-folder-plus",
    permissions: "platform:asset:create",
    clickHandler: () => newFolder(),
  },
  {
    id: "upload",
    title: t("ASSETS.PAGES.LIST.TOOLBAR.UPLOAD"),
    icon: "lucide-upload",
    permissions: "platform:asset:create",
    disabled: computed(() => !folderUrl.value),
    clickHandler: () => upload(),
  },
  {
    id: "delete",
    title: t("ASSETS.PAGES.LIST.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "platform:asset:delete",
    disabled: computed(() => !selection.value.length),
    clickHandler: () => deleteAssets(selection.value),
  },
]);

exposeToChildren({ reload: () => load() });
</script>

<style lang="scss">
.assets-list__crumbs {
  padding: 12px 16px 4px;
}
.assets-list__thumb {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 6px;
  overflow: hidden;
  background: var(--neutrals-100);
  color: var(--neutrals-500);
  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
}
.assets-list__name {
  color: var(--neutrals-900);
}
</style>
