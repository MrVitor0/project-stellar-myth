<template>
  <div class="min-h-screen blockchain-pattern">
    <!-- Navigation -->
    <nav class="relative z-50 py-6">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
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
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
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

          <!-- Right Column - Blessings Showcase -->
          <div class="order-1 lg:order-2">
            <BlessingShowcase
              :blessings="recentBlessings"
              @create-blessing="handleCreateBlessing"
            />
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

          <!-- WebGL Canvas Container -->
          <div 
            id="unity-container" 
            class="w-full h-full rounded-lg overflow-hidden"
            :class="{ 'opacity-100': gameLoaded, 'opacity-0': !gameLoaded }"
          >
            <canvas 
              id="unity-canvas" 
              class="w-full h-full block"
              style="background: #1e293b;"
            ></canvas>
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
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 mb-20">
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

    <!-- Stats Section -->
    <div
      class="bg-gradient-to-r from-deep-blue/50 to-slate-concrete/80 backdrop-blur-sm py-20"
    >
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-8 text-center">
          <div class="group">
            <div
              class="text-5xl font-bold text-brazil-green mb-2 glow-text group-hover:scale-110 transition-transform duration-300"
            >
              1M+
            </div>
            <div class="text-mid-gray">Active Players</div>
          </div>
          <div class="group">
            <div
              class="text-5xl font-bold text-brazil-yellow mb-2 group-hover:scale-110 transition-transform duration-300"
            >
              $50M
            </div>
            <div class="text-mid-gray">Trading Volume</div>
          </div>
          <div class="group">
            <div
              class="text-5xl font-bold text-brazil-green mb-2 glow-text group-hover:scale-110 transition-transform duration-300"
            >
              10K+
            </div>
            <div class="text-mid-gray">NFT Items</div>
          </div>
          <div class="group">
            <div
              class="text-5xl font-bold text-brazil-yellow mb-2 group-hover:scale-110 transition-transform duration-300"
            >
              24/7
            </div>
            <div class="text-mid-gray">Live Blockchain</div>
          </div>
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
import blessingService from "../utils/BlessingService.js";

export default {
  name: "Home",
  components: {
    HeroContent,
    BlessingShowcase,
  },
  data() {
    return {
      gameLoaded: false,
      heroData: {
        title: {
          prefix: "Enter the",
          highlight: "Stellar Myth",
        },
        description:
          "Experience the future of gaming where blockchain technology meets immersive gameplay. Build, trade, and conquer in a decentralized universe powered by your creativity.",
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

    // Simulate WebGL game loading
    setTimeout(() => {
      this.gameLoaded = true;
    }, 2000);

    // Initialize WebGL context when component mounts
    this.initWebGL();
  },
  methods: {
    loadRecentBlessings() {
      this.recentBlessings = blessingService.getRecentBlessings();
    },

    handleLaunchGame() {
      console.log("Launching game...");
      // Add game launch logic here
    },

    handleConnectWallet() {
      console.log("Connecting wallet...");
      // Add wallet connection logic here
    },

    handleCreateBlessing() {
      console.log("Opening blessing creation modal...");
      // Add blessing creation logic here
    },

    initWebGL() {
      const canvas = document.getElementById("game-canvas");
      if (canvas) {
        const gl =
          canvas.getContext("webgl") || canvas.getContext("experimental-webgl");
        if (gl) {
          console.log("WebGL context initialized successfully");
          // Here you would integrate your actual WebGL game
          this.setupWebGLScene(gl);
        }
      }
    },

    setupWebGLScene(gl) {
      // Basic WebGL setup - replace with your actual game initialization
      gl.clearColor(0.1, 0.15, 0.25, 1.0);
      gl.clear(gl.COLOR_BUFFER_BIT);

      // Animation loop placeholder
      const animate = () => {
        gl.clear(gl.COLOR_BUFFER_BIT);
        requestAnimationFrame(animate);
      };
      animate();
    },
  },
};
</script>
