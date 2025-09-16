<template>
  <div
    class="unity-parameter-tester bg-dark-night border border-brazil-green/30 rounded-lg p-4 mb-4"
  >
    <h3 class="text-lg font-semibold text-brazil-green mb-3">
      Unity Parameter Tester
    </h3>

    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-4">
      <!-- Text Parameter Input -->
      <div>
        <label class="block text-sm font-medium text-mid-gray mb-2">
          Texto Simples:
        </label>
        <input
          v-model="textParameter"
          type="text"
          placeholder="Digite um parâmetro de texto..."
          class="w-full px-3 py-2 bg-twilight-blue border border-gray-600 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-brazil-green"
        />
        <button
          @click="sendTextParameter"
          :disabled="!canSendParameter || !textParameter.trim()"
          class="mt-2 w-full btn-sm bg-brazil-green hover:bg-brazil-green/80 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Enviar Texto
        </button>
      </div>

      <!-- JSON Parameter Input -->
      <div>
        <label class="block text-sm font-medium text-mid-gray mb-2">
          JSON Personalizado:
        </label>
        <textarea
          v-model="jsonParameter"
          rows="3"
          placeholder='{"message": "Hello Unity!", "value": 123}'
          class="w-full px-3 py-2 bg-twilight-blue border border-gray-600 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-brazil-green resize-none"
        ></textarea>
        <button
          @click="sendJsonParameter"
          :disabled="!canSendParameter || !isValidJson"
          class="mt-2 w-full btn-sm bg-mystic-cyan hover:bg-mystic-cyan/80 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Enviar JSON
        </button>
      </div>
    </div>

    <!-- Predefined Parameters -->
    <div class="mb-4">
      <label class="block text-sm font-medium text-mid-gray mb-2">
        Parâmetros Predefinidos:
      </label>
      <div class="flex flex-wrap gap-2">
        <button
          v-for="preset in presetParameters"
          :key="preset.name"
          @click="sendPresetParameter(preset)"
          :disabled="!canSendParameter"
          class="btn-sm bg-brazil-yellow text-dark-night hover:bg-brazil-yellow/80 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ preset.name }}
        </button>
      </div>
    </div>

    <!-- Status Display -->
    <div class="flex items-center justify-between text-sm">
      <div class="flex items-center space-x-2">
        <span class="text-mid-gray">Unity Status:</span>
        <span
          :class="{
            'text-brazil-green': canSendParameter,
            'text-brazil-yellow': !canSendParameter,
          }"
        >
          {{ unityStatus }}
        </span>
      </div>
      <div v-if="lastSentParameter" class="text-xs text-mid-gray">
        Último: {{ truncateParameter(lastSentParameter) }}
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "UnityParameterTester",

  props: {
    unityService: {
      type: Object,
      required: true,
    },
  },

  data() {
    return {
      textParameter: "",
      jsonParameter:
        '{\n  "message": "Hello from Vue!",\n  "timestamp": ' +
        Date.now() +
        ',\n  "user": "Player1"\n}',
      lastSentParameter: "",

      presetParameters: [
        {
          name: "Test",
          value: "test",
        },
        {
          name: "Hello",
          value: "hello",
        },
        {
          name: "Debug On",
          value: "debug",
        },
        {
          name: "Player Info",
          value: {
            playerId: "player_123",
            level: 5,
            score: 1250,
            timestamp: Date.now(),
          },
        },
        {
          name: "Game Config",
          value: {
            difficulty: "normal",
            sound: true,
            graphics: "high",
            language: "pt-BR",
          },
        },
      ],
    };
  },

  computed: {
    canSendParameter() {
      return this.unityService && this.unityService.isUnityLoaded();
    },

    unityStatus() {
      if (!this.unityService) return "Não conectado";
      if (this.unityService.isUnityLoaded()) return "Conectado";
      return "Carregando...";
    },

    isValidJson() {
      if (!this.jsonParameter.trim()) return false;
      try {
        JSON.parse(this.jsonParameter);
        return true;
      } catch {
        return false;
      }
    },
  },

  methods: {
    sendTextParameter() {
      if (!this.canSendParameter || !this.textParameter.trim()) return;

      const parameter = this.textParameter.trim();
      const success = this.unityService.sendParameter(parameter);

      if (success) {
        this.lastSentParameter = parameter;
        this.$emit("parameter-sent", parameter);
        console.log(
          `Unity Parameter Tester: Parâmetro de texto enviado: "${parameter}"`
        );
      } else {
        console.warn(
          "Unity Parameter Tester: Falha ao enviar parâmetro de texto"
        );
      }
    },

    sendJsonParameter() {
      if (!this.canSendParameter || !this.isValidJson) return;

      try {
        const jsonObj = JSON.parse(this.jsonParameter);
        const success = this.unityService.sendParameter(jsonObj);

        if (success) {
          this.lastSentParameter = JSON.stringify(jsonObj);
          this.$emit("parameter-sent", jsonObj);
          console.log(
            "Unity Parameter Tester: Parâmetro JSON enviado:",
            jsonObj
          );
        } else {
          console.warn(
            "Unity Parameter Tester: Falha ao enviar parâmetro JSON"
          );
        }
      } catch (error) {
        console.error("Unity Parameter Tester: Erro ao processar JSON:", error);
      }
    },

    sendPresetParameter(preset) {
      if (!this.canSendParameter) return;

      const success = this.unityService.sendParameter(preset.value);

      if (success) {
        this.lastSentParameter =
          typeof preset.value === "object"
            ? JSON.stringify(preset.value)
            : preset.value.toString();
        this.$emit("parameter-sent", preset.value);
        console.log(
          `Unity Parameter Tester: Parâmetro predefinido "${preset.name}" enviado:`,
          preset.value
        );
      } else {
        console.warn(
          `Unity Parameter Tester: Falha ao enviar parâmetro predefinido "${preset.name}"`
        );
      }
    },

    truncateParameter(param) {
      const str = typeof param === "string" ? param : JSON.stringify(param);
      return str.length > 30 ? str.substring(0, 30) + "..." : str;
    },
  },
};
</script>

<style scoped>
.btn-sm {
  @apply px-3 py-1 text-sm font-medium rounded-md transition-colors duration-200;
}
</style>
