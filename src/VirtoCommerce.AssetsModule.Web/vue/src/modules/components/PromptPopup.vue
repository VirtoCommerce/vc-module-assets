<template>
  <VcPopup
    :title="title"
    @close="$emit('close')"
  >
    <template #content>
      <form
        class="assets-prompt"
        @submit.prevent="submit"
      >
        <VcInput
          v-model="value"
          :label="label"
          required
        />
      </form>
    </template>
    <template #footer="{ close }">
      <!-- the framework's confirmation dialog layout: text-styled primary action, filled Cancel -->
      <div class="vc-popup-base__actions vc-popup-base__actions--confirm">
        <VcButton
          text
          class="vc-popup-base__confirm"
          :disabled="!value.trim()"
          @click="submit"
        >
          {{ $t("ASSETS.PROMPT.OK") }}
        </VcButton>
        <VcButton @click="close">{{ $t("ASSETS.PROMPT.CANCEL") }}</VcButton>
      </div>
    </template>
  </VcPopup>
</template>

<script setup lang="ts">
// Text prompt (the AngularJS blade used window.prompt for "new folder")
import { VcButton, VcInput, VcPopup } from "@vc-shell/framework/ui";
import { ref } from "vue";

const props = defineProps<{ title: string; label: string; initialValue?: string }>();
const emit = defineEmits<{ (e: "close"): void; (e: "submit", value: string): void }>();

const value = ref(props.initialValue ?? "");

function submit() {
  if (value.value.trim()) emit("submit", value.value.trim());
}
</script>

<style lang="scss">
.assets-prompt {
  width: 100%;
  min-width: 340px;
  padding: 4px 0 8px;
  .vc-input {
    width: 100%;
  }
}
</style>
