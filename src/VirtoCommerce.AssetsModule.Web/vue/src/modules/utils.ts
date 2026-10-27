import { reactive } from "vue";
import { notification, parseError, readableSize, usePopup } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import PromptPopup from "./components/PromptPopup.vue";
import type { AssetsClient, BlobEntry } from "../api_client/virtocommerce.assets";

// The generated BlobEntry omits BlobInfo's size/contentType, which the API does return
export type Asset = BlobEntry & { size?: number; contentType?: string };

// folders have no size
export const formatSize = (bytes?: number) => (bytes == null ? "" : readableSize(bytes));

export const isImage = (a: Asset) => a.type !== "folder" && !!a.contentType?.startsWith("image/");

// same cache-buster as the AngularJS list, so a replaced image doesn't show stale
export function previewUrl(a: Asset) {
  const url = a.url ?? "";
  return `${url}${url.includes("?") ? "&" : "?"}t=${a.modifiedDate ? new Date(a.modifiedDate).getTime() : ""}`;
}

export function download(a: Asset) {
  const link = document.createElement("a");
  link.href = a.url ?? "";
  link.download = a.name || "download";
  document.body.appendChild(link);
  link.click();
  link.remove();
}

export function useAssetActions() {
  const { t } = useI18n({ useScope: "global" });

  async function copyLink(a: Asset) {
    await navigator.clipboard.writeText(a.url ?? "");
    notification.success(t("ASSETS.ALERTS.LINK_COPIED"), { timeout: 3000 });
  }

  // Asks for a value in a popup; resolves undefined on cancel
  const promptProps = reactive({ title: "", label: "", initialValue: "" });
  let resolvePrompt: ((value?: string) => void) | undefined;
  const { open, close } = usePopup({
    component: PromptPopup,
    props: promptProps,
    emits: {
      onClose() {
        resolvePrompt?.();
        close();
      },
      onSubmit(value: string) {
        resolvePrompt?.(value);
        close();
      },
    },
  });

  function prompt(title: string, label: string, initialValue = ""): Promise<string | undefined> {
    Object.assign(promptProps, { title, label, initialValue });
    return new Promise((resolve) => {
      resolvePrompt = (v) => {
        resolvePrompt = undefined;
        resolve(v);
      };
      open();
    });
  }

  return { copyLink, prompt };
}

// "Upload from URL": the server fetches the file itself. Raw fetch: the generated uploadAsset requires a file.
export async function uploadFromUrl(folderUrl: string, url: string, name?: string) {
  const query = new URLSearchParams({ folderUrl, url, ...(name ? { name } : {}) });
  const response = await fetch(`/api/assets?${query}`, { method: "POST", headers: { Accept: "application/json" } });
  if (!response.ok) throw new Error(`${response.status} ${await response.text()}`);
  return (await response.json()) as Asset[];
}

// There's no rename/move endpoint: copy the blob server-side through "upload from URL" under the new name,
// then delete the original. Files only.
export async function renameAsset(client: AssetsClient, asset: Asset, folderUrl: string, newName: string) {
  const [created] = await uploadFromUrl(folderUrl, asset.url ?? "", newName);
  await client.deleteBlobs([asset.url ?? ""]);
  return created;
}

// The server's own message (e.g. folder-name validation) instead of the generated client's generic one
export const errorText = (e: unknown) => parseError(e).message;
