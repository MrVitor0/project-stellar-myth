<template>
  <div
    class="bg-gradient-to-br from-gray-800/80 to-gray-900/90 p-6 rounded-2xl border border-brazil-green/20"
  >
    <div v-if="!isConnected" class="space-y-4">
      <h3 class="text-xl font-bold text-mist-white mb-4">
        <svg
          class="w-6 h-6 inline-block mr-2"
          fill="currentColor"
          viewBox="0 0 20 20"
        >
          <path
            fill-rule="evenodd"
            d="M4 4a2 2 0 00-2 2v4a2 2 0 002 2V6h10a2 2 0 00-2-2H4zm2 6a2 2 0 012-2h8a2 2 0 012 2v4a2 2 0 01-2 2H8a2 2 0 01-2-2v-4zm6 4a2 2 0 100-4 2 2 0 000 4z"
            clip-rule="evenodd"
          />
        </svg>
        Connect Your Wallet
      </h3>

      <p class="text-gray-400 text-sm mb-6">
        In order to interact with the Stellar Testnet, you need to connect a
        wallet. You can either generate a new wallet or connect using an
        existing secret key.
      </p>
    </div>

    <div class="space-y-3">
      <!-- Freighter Connection Button (shown first if available) -->
      <BaseButton
        v-if="freighterStatus === 'available'"
        @click="connectWithFreighter"
        :loading="isConnectingFreighter"
        variant="primary"
        class="w-full"
      >
        <template #icon>
          <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
            <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </template>
        Connect with Freighter
      </BaseButton>

      <!-- Divider if Freighter is available -->
      <div v-if="freighterStatus === 'available'" class="relative">
        <div class="absolute inset-0 flex items-center">
          <div class="w-full border-t border-gray-600"></div>
        </div>
        <div class="relative flex justify-center text-sm">
          <span class="px-2 bg-gray-800 text-gray-400"
            >or use local wallet</span
          >
        </div>
      </div>

      <BaseButton
        @click="generateNewWallet"
        :loading="isGenerating"
        :variant="freighterStatus === 'available' ? 'secondary' : 'primary'"
        class="w-full"
      >
        <template #icon>
          <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
            <path
              fill-rule="evenodd"
              d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"
              clip-rule="evenodd"
            />
          </svg>
        </template>
        Generate New Local Wallet
      </BaseButton>

      <div class="relative">
        <div class="absolute inset-0 flex items-center">
          <div class="w-full border-t border-gray-600"></div>
        </div>
        <div class="relative flex justify-center text-sm">
          <span class="px-2 bg-gray-800 text-gray-400">or</span>
        </div>
      </div>

      <div class="space-y-2">
        <label class="block text-sm font-medium text-mist-white">
          Existing Secret Key
        </label>
        <div class="flex space-x-2">
          <input
            v-model="secretKey"
            type="password"
            placeholder="SXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
            class="flex-1 px-3 py-2 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-brazil-green focus:border-transparent"
            :class="{ 'border-red-500': secretKeyError }"
          />
          <BaseButton
            @click="connectWithSecret"
            :loading="isConnecting"
            :disabled="!secretKey.trim()"
            variant="secondary"
          >
            Connect
          </BaseButton>
        </div>
        <span v-if="secretKeyError" class="text-red-400 text-xs">{{
          secretKeyError
        }}</span>
      </div>
    </div>

    <!-- Wallet Connected State -->
    <div v-if="isConnected" class="space-y-4">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-3">
          <div
            class="w-8 h-8 bg-gradient-to-br from-brazil-green to-mystic-cyan rounded-full flex items-center justify-center"
          >
            <svg
              class="w-4 h-4 text-white"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fill-rule="evenodd"
                d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <div>
            <div class="flex items-center space-x-2">
              <h3 class="text-lg font-bold text-mist-white">
                Wallet Connected
              </h3>
              <span
                v-if="walletType === 'freighter'"
                class="px-2 py-1 bg-green-600/20 text-green-300 text-xs rounded-full border border-green-600/30"
              >
                Freighter
              </span>
              <span
                v-else
                class="px-2 py-1 bg-blue-600/20 text-blue-300 text-xs rounded-full border border-blue-600/30"
              >
                Local
              </span>
            </div>
            <p class="text-sm text-gray-400">{{ formattedPublicKey }}</p>
          </div>
        </div>
        <BaseButton @click="disconnect" variant="danger" size="sm">
          Disconnect
        </BaseButton>
      </div>

      <div class="grid grid-cols-2 gap-4 py-4 px-4 bg-gray-700/30 rounded-lg">
        <div class="text-center">
          <p class="text-xs text-gray-400 uppercase tracking-wide">
            XLM Balance
          </p>
          <p class="text-lg font-bold text-brazil-yellow">
            {{ balance.toFixed(2) }}
          </p>
        </div>
        <div class="text-center">
          <p class="text-xs text-gray-400 uppercase tracking-wide">Network</p>
          <p class="text-lg font-bold text-mystic-cyan">Testnet</p>
        </div>
      </div>

      <div class="flex space-x-2">
        <BaseButton
          @click="copyPublicKey"
          variant="secondary"
          size="sm"
          class="flex-1"
        >
          <template #icon>
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path d="M8 3a1 1 0 011-1h2a1 1 0 110 2H9a1 1 0 01-1-1z" />
              <path
                d="M6 3a2 2 0 00-2 2v11a2 2 0 002 2h8a2 2 0 002-2V5a2 2 0 00-2-2 3 3 0 01-3 3H9a3 3 0 01-3-3z"
              />
            </svg>
          </template>
          {{ copyButtonText }}
        </BaseButton>
        <BaseButton @click="refreshBalance" variant="secondary" size="sm">
          <template #icon>
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path
                fill-rule="evenodd"
                d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z"
                clip-rule="evenodd"
              />
            </svg>
          </template>
        </BaseButton>
      </div>

      <!-- Wallet Details Modal -->
      <div
        v-if="showDetails"
        class="mt-4 p-4 bg-gray-900/50 rounded-lg border border-gray-600"
      >
        <div class="flex justify-between items-center mb-3">
          <h4 class="font-semibold text-mist-white">Wallet Details</h4>
          <button
            @click="showDetails = false"
            class="text-gray-400 hover:text-white"
          >
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path
                fill-rule="evenodd"
                d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                clip-rule="evenodd"
              />
            </svg>
          </button>
        </div>
        <div class="space-y-2 text-xs">
          <div>
            <span class="text-gray-400">Public Key:</span>
            <p class="font-mono text-white break-all">{{ currentPublicKey }}</p>
          </div>
          <div>
            <span class="text-gray-400">Secret Key:</span>
            <div class="flex items-center space-x-2">
              <input
                :type="showSecretKey ? 'text' : 'password'"
                :value="currentSecretKey"
                readonly
                class="flex-1 px-2 py-1 bg-gray-800 text-white font-mono text-xs rounded border border-gray-600"
              />
              <button
                @click="showSecretKey = !showSecretKey"
                class="text-gray-400 hover:text-white"
              >
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path
                    fill-rule="evenodd"
                    d="M10 12a2 2 0 100-4 2 2 0 000 4z"
                    clip-rule="evenodd"
                  />
                  <path
                    fill-rule="evenodd"
                    d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                    clip-rule="evenodd"
                  />
                </svg>
              </button>
            </div>
          </div>
        </div>
        <div
          class="mt-3 p-2 bg-yellow-900/20 border border-yellow-600/30 rounded text-xs"
        >
          <div class="flex items-start space-x-2">
            <svg
              class="w-4 h-4 text-yellow-400 mt-0.5"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fill-rule="evenodd"
                d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
                clip-rule="evenodd"
              />
            </svg>
            <p class="text-yellow-200">
              <strong>Important:</strong> Keep your secret key secure. Anyone
              with access to it can control your funds.
            </p>
          </div>
        </div>
      </div>

      <BaseButton
        @click="showDetails = !showDetails"
        variant="ghost"
        size="sm"
        class="w-full"
      >
        {{ showDetails ? "Hide Details" : "View Wallet Details" }}
      </BaseButton>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import WalletService from "@/services/WalletService.js";
import FreighterService from "@/services/FreighterService.js";
import BaseButton from "@/components/BaseButton.vue";

export default {
  name: "WalletConnector",
  components: {
    BaseButton,
  },
  emits: ["wallet-connected", "wallet-disconnected"],
  setup(_, { emit }) {
    const isConnected = ref(false);
    const isGenerating = ref(false);
    const isConnecting = ref(false);
    const isConnectingFreighter = ref(false);
    const secretKey = ref("");
    const secretKeyError = ref("");
    const balance = ref(0);
    const showDetails = ref(false);
    const showSecretKey = ref(false);
    const copyButtonText = ref("Copiar Endereço");
    const freighterStatus = ref("checking"); // 'checking', 'available', 'not-installed'
    const walletType = ref(null);

    // Usar ref em vez de computed para lidar com método assíncrono
    const currentPublicKey = ref("");

    // Função para atualizar a chave pública
    const updatePublicKey = async () => {
      if (isConnected.value) {
        const publicKey = await WalletService.getCurrentPublicKey();
        currentPublicKey.value = publicKey || "";

        // Atualizar também a versão formatada
        if (publicKey) {
          formattedPublicKey.value = WalletService.formatPublicKey(publicKey);
        } else {
          formattedPublicKey.value = "";
        }
      } else {
        currentPublicKey.value = "";
        formattedPublicKey.value = "";
      }
    };

    const currentSecretKey = computed(() => {
      const keypair = WalletService.getCurrentKeypair();
      return keypair ? keypair.secret() : "";
    });

    // Usar ref para a chave pública formatada em vez de computed
    const formattedPublicKey = ref("");

    // Verifica disponibilidade do Freighter
    const checkFreighterAvailability = async () => {
      try {
        const available = await FreighterService.checkFreighterAvailability();
        freighterStatus.value = available ? "available" : "not-installed";
      } catch (error) {
        console.log("Freighter não disponível:", error);
        freighterStatus.value = "not-installed";
      }
    };

    const connectWithFreighter = async () => {
      isConnectingFreighter.value = true;
      secretKeyError.value = "";

      try {
        // Verificar novamente se o Freighter está disponível antes de tentar conectar
        const isAvailable = await FreighterService.checkFreighterAvailability();
        if (!isAvailable) {
          throw new Error(
            "Freighter não está instalado ou não está disponível no navegador"
          );
        }

        const wallet = await WalletService.connectWithFreighter();
        isConnected.value = true;
        walletType.value = wallet.type;
        await updatePublicKey();
        await refreshBalance();
        emit("wallet-connected", wallet.publicKey);
      } catch (error) {
        console.error("Erro ao conectar Freighter:", error);
        secretKeyError.value =
          error.message || "Erro ao conectar com Freighter.";

        // Atualizar o status do Freighter para refletir que não está disponível
        freighterStatus.value = "not-installed";
      } finally {
        isConnectingFreighter.value = false;
      }
    };

    const generateNewWallet = async () => {
      isGenerating.value = true;
      secretKeyError.value = "";

      try {
        const wallet = await WalletService.generateNewWallet();
        isConnected.value = true;
        walletType.value = "keypair";
        await updatePublicKey();
        await refreshBalance();
        emit("wallet-connected", wallet.publicKey);
      } catch (error) {
        console.error("Erro ao gerar carteira:", error);
        secretKeyError.value = "Erro ao gerar carteira. Tente novamente.";
      } finally {
        isGenerating.value = false;
      }
    };

    const connectWithSecret = async () => {
      if (!secretKey.value.trim()) return;

      isConnecting.value = true;
      secretKeyError.value = "";

      try {
        const wallet = await WalletService.connectWithSecret(
          secretKey.value.trim()
        );
        isConnected.value = true;
        walletType.value = "keypair";
        secretKey.value = "";
        await updatePublicKey();
        await refreshBalance();
        emit("wallet-connected", wallet.publicKey);
      } catch (error) {
        console.error("Erro ao conectar carteira:", error);
        secretKeyError.value = "Chave secreta inválida ou erro de conexão.";
      } finally {
        isConnecting.value = false;
      }
    };

    const disconnect = async () => {
      await WalletService.disconnect();
      isConnected.value = false;
      walletType.value = null;
      balance.value = 0;
      showDetails.value = false;
      currentPublicKey.value = "";
      emit("wallet-disconnected");
    };

    const refreshBalance = async () => {
      if (!isConnected.value) return;

      try {
        const newBalance = await WalletService.getAccountBalance();
        balance.value = newBalance;
      } catch (error) {
        console.error("Erro ao obter saldo:", error);
      }
    };

    const copyPublicKey = async () => {
      try {
        const success = await WalletService.copyPublicKey();
        if (success) {
          copyButtonText.value = "Copiado!";
          setTimeout(() => {
            copyButtonText.value = "Copiar Endereço";
          }, 2000);
        }
      } catch (error) {
        console.error("Erro ao copiar chave pública:", error);
        copyButtonText.value = "Erro ao copiar";
        setTimeout(() => {
          copyButtonText.value = "Copiar Endereço";
        }, 2000);
      }
    };

    onMounted(async () => {
      // Verificar disponibilidade do Freighter - tentamos mais de uma vez com um pequeno atraso
      // para garantir que as extensões do navegador foram carregadas corretamente
      freighterStatus.value = "checking";

      try {
        // Primeira tentativa
        const available = await FreighterService.checkFreighterAvailability();
        if (available) {
          freighterStatus.value = "available";
        } else {
          // Segunda tentativa após um breve atraso
          setTimeout(async () => {
            try {
              const secondCheck =
                await FreighterService.checkFreighterAvailability();
              freighterStatus.value = secondCheck
                ? "available"
                : "not-installed";
            } catch {
              freighterStatus.value = "not-installed";
            }
          }, 500);
        }
      } catch {
        freighterStatus.value = "not-installed";
      }

      // Tentar carregar carteira salva (Freighter tem prioridade)
      const savedWallet = await WalletService.loadSavedWallet();
      if (savedWallet) {
        isConnected.value = true;
        walletType.value = savedWallet.type;
        await updatePublicKey();
        refreshBalance();
        emit("wallet-connected", savedWallet.publicKey);
      }
    });

    return {
      isConnected,
      isGenerating,
      isConnecting,
      isConnectingFreighter,
      secretKey,
      secretKeyError,
      balance,
      showDetails,
      showSecretKey,
      copyButtonText,
      freighterStatus,
      walletType,
      currentPublicKey,
      currentSecretKey,
      formattedPublicKey,
      connectWithFreighter,
      generateNewWallet,
      connectWithSecret,
      disconnect,
      refreshBalance,
      copyPublicKey,
    };
  },
};
</script>
