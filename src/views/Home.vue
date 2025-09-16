<template>
  <div class="min-h-screen blockchain-pattern">
    <!-- Navigation -->
    <nav class="relative z-50 py-6">
      <div class="mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center">
          <div class="text-2xl font-bold text-brazil-yellow">Stellar Myth</div>
          <div class="hidden md:flex space-x-8">
            <router-link
              to="/about"
              class="text-mid-gray hover:text-brazil-green transition-colors duration-300"
            >
              About
            </router-link>
            <a
              href="#"
              class="text-mid-gray hover:text-brazil-green transition-colors duration-300"
            >
              Blockchain
            </a>
            <a
              href="#"
              class="text-mid-gray hover:text-brazil-green transition-colors duration-300"
            >
              Gaming
            </a>
          </div>
        </div>
      </div>
    </nav>

    <!-- Hero Section -->
    <div class="relative py-20">
      <div class="mx-auto px-4 sm:px-6 lg:px-8">
        <div
          class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center min-h-[80vh]"
        >
          <!-- Left Column - Hero Content -->
          <div class="order-2 lg:order-1">
            <HeroContent
              :title="heroData.title"
              :description="heroData.description"
              :primary-button="heroData.primaryButton"
              :secondary-button="heroData.secondaryButton"
              :stats="heroData.stats"
              @launch-game="handleLaunchGame"
              @connect-wallet="handleConnectWallet"
            />
          </div>

          <!-- Right Column - Blessing Cards Showcase -->
          <div class="order-1 lg:order-2">
            <BlessingCardsStack :blessings="customCharacters" />
          </div>
        </div>

        <!-- Floating Elements -->
        <div
          class="absolute top-20 left-10 w-20 h-20 border border-brazil-green/30 rounded-lg transform rotate-45 animate-pulse hidden lg:block"
        ></div>
        <div
          class="absolute top-40 right-16 w-12 h-12 border border-brazil-yellow/40 rounded-full animate-bounce hidden lg:block"
        ></div>
        <div
          class="absolute bottom-20 left-1/4 w-16 h-16 border border-mystic-cyan/20 rounded-xl transform rotate-12 hidden lg:block"
        ></div>
      </div>
    </div>

    <!-- WebGL Game Container -->
    <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 mb-20">
      <div class="gaming-card">
        <div class="text-center mb-8">
          <h2 class="text-3xl font-bold text-mist-white mb-4">Game Portal</h2>
          <p class="text-mid-gray">
            Experience the immersive world of Stellar Myth
          </p>
        </div>

        <!-- Unity Parameter Tester -->
        <UnityParameterTester
          :unity-service="unityService"
          @parameter-sent="handleParameterSent"
        />

        <div class="webgl-container aspect-video relative">
          <div
            class="absolute inset-0 flex items-center justify-center z-10"
            v-if="!gameLoaded"
          >
            <div class="text-center">
              <div
                class="animate-spin rounded-full h-16 w-16 border-b-2 border-brazil-green mb-4 mx-auto"
              ></div>
              <p class="text-mist-white text-lg">Loading WebGL Game...</p>
              <p class="text-mid-gray text-sm mt-2">
                Initializing blockchain connection
              </p>
              <div class="mt-4 w-64 bg-gray-700 rounded-full h-2 mx-auto">
                <div
                  class="bg-brazil-green h-2 rounded-full transition-all duration-500"
                  :style="`width: ${loadingProgress}%`"
                ></div>
              </div>
              <p class="text-xs text-mid-gray mt-2">{{ loadingProgress }}%</p>
            </div>
          </div>

          <!-- Fullscreen Button -->
          <button
            v-if="gameLoaded"
            @click="toggleFullscreen"
            class="absolute top-4 right-4 z-20 p-2 bg-dark-night/80 border border-brazil-green/30 rounded-lg hover:bg-brazil-green/20 transition-colors backdrop-blur-sm"
            title="Tela Cheia (F11 ou F)"
          >
            <svg
              v-if="!isFullscreen"
              class="w-5 h-5 text-brazil-green"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 8V4m0 0h4M4 4l5 5m11-1V4m0 0h-4m4 0l-5 5M4 16v4m0 0h4m-4 0l5-5m11 5l-5-5m5 5v-4m0 4h-4"
              />
            </svg>
            <svg
              v-else
              class="w-5 h-5 text-brazil-green"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M9 9V4.5M9 9H4.5M9 9L3.5 3.5M15 9h4.5M15 9V4.5M15 9l5.5-5.5M9 15v4.5M9 15H4.5M9 15l-5.5 5.5M15 15h4.5M15 15v4.5m0-4.5l5.5 5.5"
              />
            </svg>
          </button>

          <!-- WebGL Canvas Container -->
          <div
            id="unity-container"
            ref="unityContainer"
            class="w-full h-full rounded-lg overflow-hidden"
            :class="{ 'opacity-100': gameLoaded, 'opacity-0': !gameLoaded }"
          >
            <canvas
              id="unity-canvas"
              class="w-full h-full block"
              style="background: #1e293b"
            ></canvas>

            <!-- Fullscreen hint -->
            <div v-if="gameLoaded && !isFullscreen" class="fullscreen-hint">
              Pressione F ou F11 para tela cheia
            </div>
          </div>
        </div>

        <div class="mt-6 flex justify-between items-center">
          <div class="flex space-x-4">
            <span class="text-sm text-mid-gray">Status:</span>
            <span class="text-sm text-brazil-green">● Connected</span>
          </div>
          <div class="flex space-x-2">
            <button
              class="p-2 bg-twilight-blue border border-brazil-green/30 rounded-lg hover:bg-brazil-green/20 transition-colors"
            >
              <svg
                class="w-4 h-4 text-brazil-green"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z"
                ></path>
              </svg>
            </button>
            <button
              class="p-2 bg-twilight-blue border border-brazil-green/30 rounded-lg hover:bg-brazil-green/20 transition-colors"
            >
              <svg
                class="w-4 h-4 text-brazil-green"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
                ></path>
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Features Grid -->
    <div class="mx-auto px-4 sm:px-6 lg:px-8 mb-20">
      <div class="text-center mb-16">
        <h2 class="text-4xl font-bold text-mist-white mb-4">
          Gaming Revolution
        </h2>
        <p class="text-lg text-mid-gray">
          Powered by cutting-edge blockchain technology
        </p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <!-- NFT Gaming -->
        <div class="gaming-card neon-glow">
          <div
            class="w-16 h-16 bg-gradient-to-br from-brazil-green to-deep-blue rounded-xl flex items-center justify-center mb-6 mx-auto"
          >
            <svg
              class="w-8 h-8 text-white"
              fill="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                d="M12,2A3,3 0 0,1 15,5V11A3,3 0 0,1 12,14A3,3 0 0,1 9,11V5A3,3 0 0,1 12,2M19,11C19,14.53 16.39,17.44 13,17.93V21H11V17.93C7.61,17.44 5,14.53 5,11H7A5,5 0 0,0 12,16A5,5 0 0,0 17,11H19Z"
              />
            </svg>
          </div>
          <h3 class="text-xl font-semibold text-mist-white mb-4 text-center">
            NFT Assets
          </h3>
          <p class="text-mid-gray text-center">
            Own unique in-game items as NFTs. Trade, collect, and showcase your
            digital treasures across the metaverse.
          </p>
        </div>

        <!-- DeFi Integration -->
        <div class="gaming-card neon-glow">
          <div
            class="w-16 h-16 bg-gradient-to-br from-brazil-yellow to-deep-blue rounded-xl flex items-center justify-center mb-6 mx-auto"
          >
            <svg
              class="w-8 h-8 text-dark-night"
              fill="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                d="M7,15H9C9,16.08 10.37,17 12,17C13.63,17 15,16.08 15,15C15,13.9 13.96,13.5 11.76,12.97C9.64,12.44 7,11.78 7,9C7,7.21 8.47,5.69 10.5,5.18V3H13.5V5.18C15.53,5.69 17,7.21 17,9H15C15,7.92 13.63,7 12,7C10.37,7 9,7.92 9,9C9,10.1 10.04,10.5 12.24,11.03C14.36,11.56 17,12.22 17,15C17,16.79 15.53,18.31 13.5,18.82V21H10.5V18.82C8.47,18.31 7,16.79 7,15Z"
              />
            </svg>
          </div>
          <h3 class="text-xl font-semibold text-mist-white mb-4 text-center">
            DeFi Gaming
          </h3>
          <p class="text-mid-gray text-center">
            Earn cryptocurrency while playing. Stake tokens, provide liquidity,
            and participate in governance decisions.
          </p>
        </div>

        <!-- Metaverse -->
        <div class="gaming-card neon-glow">
          <div
            class="w-16 h-16 bg-gradient-to-br from-brazil-green to-brazil-yellow rounded-xl flex items-center justify-center mb-6 mx-auto"
          >
            <svg
              class="w-8 h-8 text-dark-night"
              fill="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                d="M12,2A2,2 0 0,1 14,4C14,4.74 13.6,5.39 13,5.73V7H14A7,7 0 0,1 21,14H22A1,1 0 0,1 23,15V18A1,1 0 0,1 22,19H21V20A2,2 0 0,1 19,22H5A2,2 0 0,1 3,20V19H2A1,1 0 0,1 1,18V15A1,1 0 0,1 2,14H3A7,7 0 0,1 10,7H11V5.73C10.4,5.39 10,4.74 10,4A2,2 0 0,1 12,2M7.5,13A2.5,2.5 0 0,0 5,15.5A2.5,2.5 0 0,0 7.5,18A2.5,2.5 0 0,0 10,15.5A2.5,2.5 0 0,0 7.5,13M16.5,13A2.5,2.5 0 0,0 14,15.5A2.5,2.5 0 0,0 16.5,18A2.5,2.5 0 0,0 19,15.5A2.5,2.5 0 0,0 16.5,13Z"
              />
            </svg>
          </div>
          <h3 class="text-xl font-semibold text-mist-white mb-4 text-center">
            Metaverse
          </h3>
          <p class="text-mid-gray text-center">
            Explore vast virtual worlds with friends. Build communities, attend
            events, and shape the digital future.
          </p>
        </div>
      </div>
    </div>

    <!-- Bênçãos Recentes Section -->
    <div
      class="bg-gradient-to-r from-deep-blue/50 to-slate-concrete/80 backdrop-blur-sm py-20"
    >
      <div class="mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-4xl font-bold text-mist-white mb-4">
            Bênçãos Recentes
          </h2>
          <p class="text-lg text-mid-gray">
            Descubra as bênçãos mais poderosas criadas pela comunidade
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <div
            v-for="blessing in recentBlessings.slice(0, 6)"
            :key="blessing.id"
            class="gaming-card neon-glow group"
          >
            <!-- Ícone da Bênção -->
            <div class="flex items-center justify-center mb-6">
              <div
                class="w-20 h-20 bg-gradient-to-br from-brazil-green to-deep-blue rounded-xl flex items-center justify-center relative group-hover:scale-110 transition-transform duration-300"
              >
                <!-- Ícone baseado no tipo da bênção -->
                <svg
                  v-if="blessing.type === 'Ataque'"
                  class="w-10 h-10 text-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12,2A2,2 0 0,1 14,4A2,2 0 0,1 12,6A2,2 0 0,1 10,4A2,2 0 0,1 12,2M21,9V7L15,1H5A2,2 0 0,0 3,3V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V9M19,9H14V4H19V9Z"
                  />
                </svg>
                <svg
                  v-else-if="blessing.type === 'Defesa'"
                  class="w-10 h-10 text-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12,1L3,5V11C3,16.55 6.84,21.74 12,23C17.16,21.74 21,16.55 21,11V5L12,1M12,7C13.4,7 14.8,7.6 15.6,8.7C16.4,9.8 16.4,11.2 15.6,12.3C14.8,13.4 13.4,14 12,14C10.6,14 9.2,13.4 8.4,12.3C7.6,11.2 7.6,9.8 8.4,8.7C9.2,7.6 10.6,7 12,7Z"
                  />
                </svg>
                <svg
                  v-else-if="blessing.type === 'Cura'"
                  class="w-10 h-10 text-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12.8,2.11C14.2,2.39 15.5,3 16.6,3.9L15.5,5.3C14.7,4.6 13.7,4.2 12.6,4L12.8,2.11M7.4,3.9C8.5,3 9.8,2.39 11.2,2.11L11.4,4C10.3,4.2 9.3,4.6 8.5,5.3L7.4,3.9M18.1,7.4L19.5,6.3C20.4,7.4 21,8.7 21.28,10.1L19.39,10.3C19.19,9.2 18.79,8.2 18.1,7.4M5.9,7.4C5.21,8.2 4.81,9.2 4.61,10.3L2.72,10.1C3,8.7 3.6,7.4 4.5,6.3L5.9,7.4M2.11,11.8L4,11.6C4.2,12.7 4.6,13.7 5.3,14.5L3.9,15.6C3,14.5 2.39,13.2 2.11,11.8M21.89,11.8C21.61,13.2 21,14.5 20.1,15.6L18.7,14.5C19.4,13.7 19.8,12.7 20,11.6L21.89,11.8M6.3,19.5C7.4,20.4 8.7,21 10.1,21.28L9.9,19.39C8.8,19.19 7.8,18.79 7,18.1L6.3,19.5M17.7,19.5L17,18.1C17.8,17.4 18.2,16.4 18.4,15.3L20.28,15.5C20,16.9 19.4,18.2 18.5,19.3L17.7,19.5M11.8,21.89C13.2,21.61 14.5,21 15.6,20.1L14.5,18.7C13.7,19.4 12.7,19.8 11.6,20L11.8,21.89Z"
                  />
                </svg>
                <svg
                  v-else-if="blessing.type === 'Suporte'"
                  class="w-10 h-10 text-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M16,12A2,2 0 0,1 18,10A2,2 0 0,1 20,12A2,2 0 0,1 18,14A2,2 0 0,1 16,12M10,12A2,2 0 0,1 12,10A2,2 0 0,1 14,12A2,2 0 0,1 12,14A2,2 0 0,1 10,12M4,12A2,2 0 0,1 6,10A2,2 0 0,1 8,12A2,2 0 0,1 6,14A2,2 0 0,1 4,12Z"
                  />
                </svg>
                <svg
                  v-else
                  class="w-10 h-10 text-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12,2L13.09,8.26L22,9L13.09,9.74L12,16L10.91,9.74L2,9L10.91,8.26L12,2Z"
                  />
                </svg>
              </div>
            </div>

            <h3 class="text-xl font-semibold text-mist-white mb-2 text-center">
              {{ blessing.name }}
            </h3>

            <p class="text-mid-gray text-sm mb-4 line-clamp-3 text-center">
              {{ blessing.description }}
            </p>

            <div class="flex justify-between items-center mb-4">
              <div class="text-brazil-yellow font-semibold">
                {{ blessing.power }} Poder
              </div>
              <div class="text-xs text-mid-gray">
                por {{ blessing.creator }}
              </div>
            </div>

            <div class="pt-4 border-t border-brazil-green/20">
              <div class="flex justify-between text-xs text-mid-gray">
                <span class="flex items-center">
                  <span
                    class="w-2 h-2 rounded-full bg-brazil-green/60 mr-2"
                  ></span>
                  {{ blessing.type }}
                </span>
                <span>{{ blessing.dateCreated }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Ver Todas as Bênçãos -->
        <div class="text-center mt-12">
          <button @click="handleCreateBlessing" class="btn-primary mr-4">
            Criar Nova Bênção
          </button>
          <button class="btn-secondary">Ver Todas as Bênçãos</button>
        </div>
      </div>
    </div>

    <!-- Call to Action -->
    <div class="py-20">
      <div class="max-w-4xl mx-auto text-center px-4 sm:px-6 lg:px-8">
        <h2 class="text-4xl font-bold text-mist-white mb-6">
          Ready to Begin Your Journey?
        </h2>
        <p class="text-xl text-mid-gray mb-8">
          Join thousands of players in the ultimate blockchain gaming experience
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <button class="btn-primary">Start Playing Now</button>
          <button class="btn-secondary">
            <a
              href="#"
              class="text-brazil-green hover:text-mystic-cyan transition-colors"
            >
              Learn More
            </a>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import HeroContent from "../components/HeroContent.vue";
import BlessingShowcase from "../components/BlessingShowcase.vue";
import BlessingCardsStack from "../components/BlessingCardsStack.vue";
import UnityParameterTester from "../components/UnityParameterTester.vue";
import blessingService from "../utils/BlessingService.js";
import unityService from "../utils/UnityService.js";
import shopService from "../utils/ShopService.js";

export default {
  name: "Home",
  components: {
    HeroContent,
    BlessingShowcase,
    BlessingCardsStack,
    UnityParameterTester,
  },
  data() {
    return {
      gameLoaded: false,
      loadingProgress: 0,
      isFullscreen: false,
      unityService: unityService, // Adiciona referência ao serviço
      shopService: shopService, // Adiciona referência ao serviço da loja
      playerLevel: 1, // Nível do jogador para selecionar opções da loja
      lastShopOptionIds: [], // IDs das últimas opções da loja para evitar repetição
      customCharacters: [
        // Exemplos de personagens personalizados
        {
          id: 1,
          name: "Archer of Light",
          image: "https://via.placeholder.com/150x200?text=Archer+of+Light",
        },
        {
          id: 2,
          name: "Moreira, The Immortal Trickster",
          image: "@/assets/images/moreira.png",
          description: "A charismatic and cunning rogue",
        },
        {
          id: 3,
          name: "Sepé, the Southern Sentinel",
          image: "https://via.placeholder.com/150x200?text=Mystic+Mage",
          description: "A stoic and powerful Guarani warrior",
        },
      ],
      heroData: {
        title: {
          prefix: "Forge the Myth. Shape the Game.",
          highlight: "Stellar Myth",
        },
        description:
          "Your creation becomes their adventure. A collaborative Hack and Slash where every power-up is forged by a player, immortalized on-chain, and shared with the entire community.",
        primaryButton: {
          text: "Launch Game",
          icon: "Play",
        },
        secondaryButton: {
          text: "Connect Wallet",
          icon: "Wallet",
        },
        stats: {
          players: "1M+",
          volume: "$50M",
          nfts: "10K+",
        },
      },
      recentBlessings: [],
    };
  },
  mounted() {
    // Load recent blessings
    this.loadRecentBlessings();

    // Initialize Unity WebGL game
    this.initUnityWebGL();

    // Setup Unity service event listeners
    this.setupUnityEventListeners();

    // Setup keyboard shortcuts
    this.setupKeyboardShortcuts();

    // Setup fullscreen change listeners
    this.setupFullscreenListeners();
  },

  beforeUnmount() {
    // Cleanup Unity instance
    unityService.quit();

    // Remove keyboard event listeners
    this.removeKeyboardShortcuts();

    // Remove fullscreen event listeners
    this.removeFullscreenListeners();
  },
  methods: {
    loadRecentBlessings() {
      this.recentBlessings = blessingService.getRecentBlessings();
    },

    handleLaunchGame() {
      console.log("Launching game...");
      // Use Unity service to start the game
      if (unityService.isUnityLoaded()) {
        const success = unityService.startGame();
        if (!success) {
          console.log("Game is already running or GameManager not found");
        }
      } else {
        console.log("Unity is still loading...");
      }
    },

    handleConnectWallet() {
      console.log("Connecting wallet...");
      // Use Unity service to connect wallet
      if (unityService.isUnityLoaded()) {
        const walletData = {
          address: "0x1234567890abcdef",
          network: "ethereum",
          balance: "100.5 ETH",
        };
        const success = unityService.connectWallet(walletData);
        if (!success) {
          console.log("BlockchainManager not found in Unity or not ready");
        }
      } else {
        console.log("Unity is still loading...");
      }
    },

    handleCreateBlessing() {
      console.log("Opening blessing creation modal...");
      // Add blessing creation logic here
    },

    setupUnityEventListeners() {
      // Listen to Unity loading progress
      unityService.on("progress", (progress) => {
        this.loadingProgress = progress;
      });

      // Listen to Unity loaded event
      unityService.on("loaded", (instance) => {
        this.gameLoaded = true;
        console.log("Unity WebGL game loaded successfully");

        // Send initial data to Unity
        this.sendDataToUnity();
      });

      // Listen to Unity errors
      unityService.on("error", (error) => {
        console.error("Unity WebGL loading failed:", error);
        this.handleWebGLError();
      });

      // Setup global function for Unity to request new shop options
      this.setupShopOptionsRequestHandler();
    },

    /**
     * Configura o handler para solicitações de novas opções da loja do Unity
     */
    setupShopOptionsRequestHandler() {
      // Função global que o Unity pode chamar
      window.requestNewShopOptions = () => {
        console.log("Unity solicitou novas opções da loja");

        if (unityService.isUnityLoaded()) {
          // Gera novas opções aleatórias, evitando as últimas selecionadas
          const shopData = shopService.generateNewShopData(
            this.playerLevel,
            this.heroData.stats,
            this.lastShopOptionIds
          );

          // Atualiza o controle das últimas opções selecionadas
          this.lastShopOptionIds = shopData.options.map(
            (opt) => opt.stellarTransactionId
          );

          // Envia para o Unity
          unityService.sendShopOptionsToWebGL(shopData);

          console.log("Novas opções da loja enviadas para Unity:", shopData);
          console.log("Últimas opções armazenadas:", this.lastShopOptionIds);
        } else {
          console.warn(
            "Unity não está carregado, não é possível enviar novas opções"
          );
        }
      };

      // Também configura uma função de backup para atualização manual
      window.updateShopOptions = () => {
        this.updateShopOptions();
      };
    },

    /**
     * Método para atualizar manualmente as opções da loja
     */
    updateShopOptions() {
      if (unityService.isUnityLoaded()) {
        const shopData = shopService.generateNewShopData(
          this.playerLevel,
          this.heroData.stats,
          this.lastShopOptionIds
        );

        // Atualiza o controle das últimas opções selecionadas
        this.lastShopOptionIds = shopData.options.map(
          (opt) => opt.stellarTransactionId
        );

        unityService.sendShopOptionsToWebGL(shopData);
        console.log("Opções da loja atualizadas manualmente:", shopData);
      }
    },

    sendDataToUnity() {
      // Send blessings data to Unity
      unityService.updateBlessings(this.recentBlessings);

      // Generate and send NEW random shop options to Unity every time
      const shopData = shopService.generateNewShopData(
        this.playerLevel,
        this.heroData.stats,
        this.lastShopOptionIds
      );

      // Atualiza o controle das últimas opções selecionadas
      this.lastShopOptionIds = shopData.options.map(
        (opt) => opt.stellarTransactionId
      );

      unityService.sendShopOptionsToWebGL(shopData);

      // Send player stats to Unity
      const playerData = {
        stats: this.heroData.stats,
        blessingsCount: this.recentBlessings.length,
        playerLevel: this.playerLevel,
      };
      unityService.setPlayerData(playerData);

      // Log das informações enviadas
      console.log(
        "Novas opções da loja geradas e enviadas para Unity:",
        shopData
      );
      console.log("IDs das opções enviadas:", this.lastShopOptionIds);
      console.log("Dados do jogador enviados:", playerData);
    },

    initUnityWebGL() {
      const canvas = document.querySelector("#unity-canvas");
      if (!canvas) {
        console.error("Unity canvas not found");
        return;
      }

      // Prepare initial parameters to send to Unity
      const initParams = {
        timestamp: Date.now(),
        sessionId: this.generateSessionId(),
        gameVersion: "1.0.0",
        playerSettings: {
          language: "pt-BR",
          difficulty: "normal",
          sound: true,
        },
        vueAppData: {
          stats: this.heroData.stats,
          blessingsCount: this.recentBlessings.length,
        },
      };

      console.log("Inicializando Unity com parâmetros:", initParams);

      // Initialize Unity using the service with initial parameters
      unityService
        .initializeUnity(
          canvas,
          (progress) => {
            this.loadingProgress = progress;
          },
          initParams // Pass initial parameters
        )
        .catch((error) => {
          console.error("Failed to initialize Unity:", error);
          this.handleWebGLError();
        });
    },

    generateSessionId() {
      return (
        "session_" + Math.random().toString(36).substr(2, 9) + "_" + Date.now()
      );
    },

    handleParameterSent(parameter) {
      console.log("Parameter sent from Unity Parameter Tester:", parameter);
      // You can handle the sent parameter here if needed
    },

    /**
     * Método para atualizar o nível do jogador
     * @param {number} newLevel - Novo nível do jogador
     */
    updatePlayerLevel(newLevel) {
      this.playerLevel = Math.max(1, newLevel);

      // Se Unity estiver carregado, envia novos dados da loja
      if (unityService.isUnityLoaded()) {
        const shopData = shopService.generateNewShopData(
          this.playerLevel,
          this.heroData.stats,
          this.lastShopOptionIds
        );

        // Atualiza o controle das últimas opções selecionadas
        this.lastShopOptionIds = shopData.options.map(
          (opt) => opt.stellarTransactionId
        );

        unityService.sendShopOptionsToWebGL(shopData);

        console.log(
          `Nível do jogador atualizado para ${this.playerLevel}, novas opções da loja enviadas`,
          shopData
        );
      }
    },

    /**
     * Método para obter informações de debug dos serviços
     */
    getDebugInfo() {
      const debugInfo = {
        unity: {
          isLoaded: unityService.isUnityLoaded(),
          loadingProgress: this.loadingProgress,
        },
        shop: shopService.getDebugInfo(),
        blessings: {
          count: this.recentBlessings.length,
          data: this.recentBlessings,
        },
        player: {
          level: this.playerLevel,
          stats: this.heroData.stats,
        },
      };

      console.log("Debug Info:", debugInfo);
      return debugInfo;
    },

    handleWebGLError() {
      // Fallback to basic WebGL scene if Unity fails to load
      setTimeout(() => {
        this.initBasicWebGL();
        this.gameLoaded = true;
        this.loadingProgress = 100;
      }, 1000);
    },

    initBasicWebGL() {
      const canvas = document.querySelector("#unity-canvas");
      if (canvas) {
        const gl =
          canvas.getContext("webgl") || canvas.getContext("experimental-webgl");
        if (gl) {
          console.log("Basic WebGL context initialized as fallback");
          this.setupBasicWebGLScene(gl);
        }
      }
    },

    setupBasicWebGLScene(gl) {
      // Basic WebGL setup as fallback
      gl.clearColor(0.1, 0.15, 0.25, 1.0);
      gl.clear(gl.COLOR_BUFFER_BIT);

      // Animation loop
      const animate = () => {
        gl.clear(gl.COLOR_BUFFER_BIT);
        requestAnimationFrame(animate);
      };
      animate();
    },

    // Fullscreen methods
    toggleFullscreen() {
      if (!this.isFullscreen) {
        this.enterFullscreen();
      } else {
        this.exitFullscreen();
      }
    },

    enterFullscreen() {
      const container = this.$refs.unityContainer;
      if (container) {
        if (container.requestFullscreen) {
          container.requestFullscreen();
        } else if (container.webkitRequestFullscreen) {
          container.webkitRequestFullscreen();
        } else if (container.msRequestFullscreen) {
          container.msRequestFullscreen();
        } else if (container.mozRequestFullScreen) {
          container.mozRequestFullScreen();
        }
      }
    },

    exitFullscreen() {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      } else if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
      } else if (document.msExitFullscreen) {
        document.msExitFullscreen();
      } else if (document.mozCancelFullScreen) {
        document.mozCancelFullScreen();
      }
    },

    // Keyboard shortcuts
    setupKeyboardShortcuts() {
      this.handleKeydown = (event) => {
        // F11 or F key for fullscreen (only when game is loaded)
        if (
          this.gameLoaded &&
          (event.key === "F11" || event.key === "f" || event.key === "F")
        ) {
          event.preventDefault();
          this.toggleFullscreen();
        }

        // ESC to exit fullscreen
        if (event.key === "Escape" && this.isFullscreen) {
          event.preventDefault();
          this.exitFullscreen();
        }
      };

      document.addEventListener("keydown", this.handleKeydown);
    },

    removeKeyboardShortcuts() {
      if (this.handleKeydown) {
        document.removeEventListener("keydown", this.handleKeydown);
      }
    },

    // Fullscreen change listeners
    setupFullscreenListeners() {
      this.handleFullscreenChange = () => {
        const newFullscreenState = !!(
          document.fullscreenElement ||
          document.webkitFullscreenElement ||
          document.msFullscreenElement ||
          document.mozFullScreenElement
        );

        this.isFullscreen = newFullscreenState;

        // Notify Unity about fullscreen change
        if (unityService.isUnityLoaded()) {
          unityService.setFullscreenMode(newFullscreenState);
        }
      };

      // Add listeners for different browsers
      document.addEventListener(
        "fullscreenchange",
        this.handleFullscreenChange
      );
      document.addEventListener(
        "webkitfullscreenchange",
        this.handleFullscreenChange
      );
      document.addEventListener(
        "msfullscreenchange",
        this.handleFullscreenChange
      );
      document.addEventListener(
        "mozfullscreenchange",
        this.handleFullscreenChange
      );
    },

    removeFullscreenListeners() {
      if (this.handleFullscreenChange) {
        document.removeEventListener(
          "fullscreenchange",
          this.handleFullscreenChange
        );
        document.removeEventListener(
          "webkitfullscreenchange",
          this.handleFullscreenChange
        );
        document.removeEventListener(
          "msfullscreenchange",
          this.handleFullscreenChange
        );
        document.removeEventListener(
          "mozfullscreenchange",
          this.handleFullscreenChange
        );
      }
    },
  },
};
</script>

<style scoped>
#unity-container {
  background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
  border: 1px solid rgba(34, 197, 94, 0.2);
  transition: all 0.3s ease;
}

#unity-container:hover {
  border-color: rgba(34, 197, 94, 0.4);
  box-shadow: 0 0 20px rgba(34, 197, 94, 0.1);
}

/* Fullscreen styles */
#unity-container:fullscreen {
  background: #000;
  border: none;
  border-radius: 0;
}

#unity-container:-webkit-full-screen {
  background: #000;
  border: none;
  border-radius: 0;
}

#unity-container:-moz-full-screen {
  background: #000;
  border: none;
  border-radius: 0;
}

#unity-container:-ms-fullscreen {
  background: #000;
  border: none;
  border-radius: 0;
}

/* Canvas styles in fullscreen */
#unity-container:fullscreen #unity-canvas,
#unity-container:-webkit-full-screen #unity-canvas,
#unity-container:-moz-full-screen #unity-canvas,
#unity-container:-ms-fullscreen #unity-canvas {
  border-radius: 0;
}

#unity-canvas {
  cursor: crosshair;
}

.webgl-container {
  background: radial-gradient(
    circle at center,
    rgba(34, 197, 94, 0.05) 0%,
    transparent 70%
  );
}

/* Fullscreen button styles */
.webgl-container button {
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.webgl-container button:hover {
  transform: scale(1.05);
  box-shadow: 0 0 15px rgba(34, 197, 94, 0.3);
}

/* Responsive adjustments for Unity container */
@media (max-width: 768px) {
  .webgl-container {
    aspect-ratio: 16/10; /* Slightly taller on mobile for better gameplay */
  }
}

/* Loading animation improvements */
.animate-spin {
  animation: spin 2s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* Bênçãos Recentes Section Styles */
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Blessing card hover effects */
.gaming-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 25px rgba(34, 197, 94, 0.15);
}

/* Blessing icon container */
.gaming-card .bg-gradient-to-br {
  background: linear-gradient(135deg, #22c55e 0%, #1e40af 100%);
  box-shadow: 0 4px 15px rgba(34, 197, 94, 0.2);
}

.gaming-card:hover .bg-gradient-to-br {
  box-shadow: 0 6px 20px rgba(34, 197, 94, 0.3);
}

/* Rarity badge styles */
.gaming-card .bg-brazil-green\/90 {
  background: rgba(34, 197, 94, 0.9);
  backdrop-filter: blur(4px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

/* Type indicator dot */
.w-2.h-2.rounded-full {
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%,
  100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

/* Line clamp utility kept for other parts of the Home */
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
