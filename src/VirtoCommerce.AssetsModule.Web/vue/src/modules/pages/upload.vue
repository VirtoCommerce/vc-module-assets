<template>
  <VcBlade
    :title="$t('ASSETS.PAGES.UPLOAD.TITLE')"
    :loading="loading"
    width="50%"
  >
    <VcContainer>
      <VcForm class="assets-form tw-space-y-4">
        <VcField
          :label="$t('ASSETS.PAGES.UPLOAD.FOLDER')"
          :model-value="options?.folderName"
        />
        <VcFileUpload
          :label="$t('ASSETS.PAGES.UPLOAD.SELECT_FILES')"
          multiple
          :loading="uploading"
          @upload="uploadFiles"
        />
        <div
          v-if="uploading"
          class="assets-upload__progress"
        >
          <span>{{ $t("ASSETS.PAGES.UPLOAD.PROGRESS", { done: progress.done, total: progress.total }) }}</span>
          <VcProgress :value="progress.total ? (progress.done / progress.total) * 100 : 0" />
        </div>
        <VcRow class="tw-gap-4 assets-upload__url">
          <VcCol>
            <VcInput
              v-model="externalUrl"
              type="url"
              :label="$t('ASSETS.PAGES.UPLOAD.FROM_URL')"
              :placeholder="$t('ASSETS.PAGES.UPLOAD.URL_PLACEHOLDER')"
            />
          </VcCol>
          <VcCol class="assets-upload__url-btn">
            <VcButton
              :disabled="!externalUrl || uploading"
              icon="lucide-plus"
              @click="addFromUrl"
            >
              {{ $t("ASSETS.PAGES.UPLOAD.ADD") }}
            </VcButton>
          </VcCol>
        </VcRow>
        <VcCard
          v-if="uploaded.length"
          :header="$t('ASSETS.PAGES.UPLOAD.UPLOADED')"
        >
          <ul class="assets-upload__done">
            <li
              v-for="file in uploaded"
              :key="file.url"
            >
              <VcIcon
                icon="lucide-circle-check"
                size="s"
              />
              {{ file.name }}
            </li>
          </ul>
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useApiClient, useBlade, usePopup } from "@vc-shell/framework";
import {
  VcBlade,
  VcButton,
  VcCard,
  VcCol,
  VcContainer,
  VcField,
  VcFileUpload,
  VcForm,
  VcIcon,
  VcInput,
  VcProgress,
  VcRow,
} from "@vc-shell/framework/ui";
import { reactive, ref } from "vue";
import { useI18n } from "vue-i18n";
import { AssetsClient, type BlobInfo } from "../../api_client/virtocommerce.assets";
import { errorText, uploadFromUrl } from "../utils";

defineBlade({
  url: "/assets-upload",
  name: "AssetsUpload",
  // the target folder comes from the list (options): nothing to restore after a reload
  routable: false,
});

const { t } = useI18n({ useScope: "global" });
const { options, callParent } = useBlade<{ folderUrl?: string; folderName?: string }>();
const { showConfirmation, showError } = usePopup();
const { getApiClient } = useApiClient(AssetsClient);

const uploading = ref(false);
const loading = ref(false);
const progress = reactive({ done: 0, total: 0 });
const uploaded = ref<BlobInfo[]>([]);
const externalUrl = ref("");
const folderUrl = options.value?.folderUrl ?? "";

// one request per file: the endpoint reads a single multipart section
async function uploadFiles(files: FileList) {
  const client = await getApiClient();
  // the target folder's own files, unfiltered (the list may show a search, which is recursive)
  let existing: string[];
  try {
    const folder = await client.searchAssetItems(folderUrl, undefined);
    existing = (folder.results ?? []).filter((x) => x.type !== "folder").map((x) => x.name?.toLowerCase() ?? "");
  } catch (e) {
    showError(errorText(e));
    return;
  }
  const queue: File[] = [];
  for (const file of Array.from(files)) {
    if (!existing.includes(file.name.toLowerCase()) || (await showConfirmation(t("ASSETS.ALERTS.OVERWRITE_CONFIRMATION", { name: file.name })))) {
      queue.push(file);
    }
  }
  if (!queue.length) return;
  uploading.value = true;
  Object.assign(progress, { done: 0, total: queue.length });
  try {
    for (const file of queue) {
      const result = await client.uploadAsset(folderUrl, undefined, undefined, { data: file, fileName: file.name });
      uploaded.value.push(...(result ?? []));
      progress.done++;
    }
  } catch (e) {
    showError(errorText(e));
  } finally {
    uploading.value = false;
    await callParent("reload");
  }
}

async function addFromUrl() {
  loading.value = true;
  try {
    uploaded.value.push(...(await uploadFromUrl(folderUrl, externalUrl.value)));
    externalUrl.value = "";
    await callParent("reload");
  } catch (e) {
    showError(errorText(e));
  } finally {
    loading.value = false;
  }
}
</script>

<style lang="scss">
.assets-upload__progress {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 13px;
  color: var(--neutrals-600);
}
.assets-upload__url {
  align-items: flex-end;
}
.assets-upload__url-btn {
  flex: 0 0 auto !important;
}
.assets-upload__done {
  margin: 0;
  padding: 12px 16px;
  list-style: none;
  font-size: 14px;
  li {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 4px 0;
    color: var(--neutrals-700);
  }
}
</style>
