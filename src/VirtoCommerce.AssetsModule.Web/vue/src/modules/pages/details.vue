<template>
  <VcBlade
    :title="asset.name ?? $t('ASSETS.PAGES.DETAILS.TITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcContainer>
      <VcForm class="assets-form tw-space-y-4">
        <VcCard
          v-if="isImage(asset)"
          :header="$t('ASSETS.PAGES.DETAILS.PREVIEW')"
        >
          <div class="assets-preview">
            <img
              :src="previewUrl(asset)"
              :alt="asset.name"
            />
          </div>
        </VcCard>
        <VcField
          :label="$t('ASSETS.PAGES.DETAILS.FIELDS.FILE_NAME')"
          :model-value="asset.name"
        />
        <VcField
          :label="$t('ASSETS.PAGES.DETAILS.FIELDS.URL')"
          :model-value="asset.url"
          copyable
        />
        <VcRow class="tw-gap-4">
          <VcCol>
            <VcField
              :label="$t('ASSETS.PAGES.DETAILS.FIELDS.SIZE')"
              :model-value="formatSize(asset.size)"
            />
          </VcCol>
          <VcCol>
            <VcField
              :label="$t('ASSETS.PAGES.DETAILS.FIELDS.TYPE')"
              :model-value="asset.contentType"
            />
          </VcCol>
        </VcRow>
        <VcField
          :label="$t('ASSETS.PAGES.DETAILS.FIELDS.MODIFIED')"
          :model-value="asset.modifiedDate"
          type="date"
        />
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useApiClient, useBlade, usePermissions, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcCard, VcCol, VcContainer, VcField, VcForm, VcRow } from "@vc-shell/framework/ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { AssetsClient } from "../../api_client/virtocommerce.assets";
import { download, errorText, formatSize, isImage, previewUrl, renameAsset, useAssetActions, type Asset } from "../utils";

defineBlade({
  url: "/asset-file",
  name: "AssetsFileDetails",
  // the asset comes from the list (options): nothing to restore after a reload
  routable: false,
});

const { t } = useI18n({ useScope: "global" });
const { options, callParent, closeSelf } = useBlade<{ asset?: Asset; folderUrl?: string }>();
const { showConfirmation, showError } = usePopup();
const { hasAccess } = usePermissions();
const { copyLink, prompt } = useAssetActions();
const { getApiClient } = useApiClient(AssetsClient);

const asset = ref<Asset>({ ...(options.value?.asset ?? {}) });
const loading = ref(false);

async function rename() {
  const name = await prompt(t("ASSETS.PROMPT.RENAME"), t("ASSETS.PROMPT.FILE_NAME"), asset.value.name);
  if (!name || name === asset.value.name) return;
  loading.value = true;
  try {
    // the file's own folder (the list's folder differs for search results, and is unset at the root)
    const url = asset.value.url ?? "";
    const folderUrl = url.slice(0, url.lastIndexOf("/"));
    const client = await getApiClient();
    const folder = await client.searchAssetItems(folderUrl, undefined);
    if (folder.results?.some((x) => x.name?.toLowerCase() === name.toLowerCase())) {
      showError(t("ASSETS.ALERTS.RENAME_EXISTS", { name }));
      return;
    }
    const created = await renameAsset(client, asset.value, folderUrl, name);
    asset.value = { ...asset.value, ...created, name, modifiedDate: new Date() };
    await callParent("reload");
  } catch (e) {
    showError(errorText(e));
  } finally {
    loading.value = false;
  }
}

async function remove() {
  if (!(await showConfirmation(t("ASSETS.ALERTS.DELETE_CONFIRMATION", { count: 1 })))) return;
  loading.value = true;
  try {
    await (await getApiClient()).deleteBlobs([asset.value.url ?? ""]);
    await callParent("reload");
    await closeSelf();
  } catch (e) {
    showError(errorText(e));
  } finally {
    loading.value = false;
  }
}

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "download", title: t("ASSETS.PAGES.DETAILS.TOOLBAR.DOWNLOAD"), icon: "lucide-download", clickHandler: () => download(asset.value) },
  { id: "copy", title: t("ASSETS.PAGES.DETAILS.TOOLBAR.COPY_LINK"), icon: "lucide-link", clickHandler: () => copyLink(asset.value) },
  {
    id: "rename",
    title: t("ASSETS.PAGES.DETAILS.TOOLBAR.RENAME"),
    icon: "lucide-pencil",
    permissions: ["platform:asset:create"],
    // a rename copies, then deletes the original: without delete it leaves two copies
    isVisible: computed(() => hasAccess("platform:asset:delete")),
    disabled: computed(() => loading.value),
    clickHandler: () => rename(),
  },
  {
    id: "delete",
    title: t("ASSETS.PAGES.DETAILS.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "platform:asset:delete",
    clickHandler: () => remove(),
  },
]);
</script>

<style lang="scss">
.assets-form {
  padding: 20px 22px 36px;
}
.assets-preview {
  display: flex;
  justify-content: center;
  padding: 16px;
  background: var(--neutrals-50);
  img {
    max-width: 100%;
    max-height: 420px;
    object-fit: contain;
  }
}
</style>
