<template>
  <div class="min-h-screen blockchain-pattern py-12">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="text-center mb-12">
        <h1
          class="text-4xl md:text-5xl font-bold text-mist-white mb-6 glow-text"
        >
          About Stellar Myth
        </h1>
        <p class="text-xl text-mid-gray max-w-2xl mx-auto">
          Discover the revolutionary fusion of blockchain technology and
          immersive gaming experiences.
        </p>
      </div>

      <!-- Content -->
      <div class="gaming-card neon-glow">
        <div class="p-8 md:p-12">
          <!-- Project Description -->
          <div class="mb-8">
            <h2 class="text-2xl font-bold text-mist-white mb-4">
              What is Stellar Myth?
            </h2>
            <p class="text-mid-gray leading-relaxed mb-4">
              Stellar Myth represents the next generation of blockchain-powered
              gaming platforms. By combining cutting-edge Web3 technologies with
              immersive gameplay mechanics, we're creating a universe where
              players truly own their digital assets and experiences.
            </p>
            <p class="text-mid-gray leading-relaxed">
              Our platform leverages the power of decentralized finance (DeFi)
              and non-fungible tokens (NFTs) to create a player-driven economy
              where every action has real value and lasting impact.
            </p>
          </div>

          <!-- Technology Stack -->
          <div class="mb-8">
            <h2 class="text-2xl font-bold text-mist-white mb-6">
              Technology Stack
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div
                class="bg-twilight-blue/50 p-4 rounded-lg border border-mystic-cyan/30"
              >
                <h3 class="font-semibold text-mystic-cyan mb-2">Frontend</h3>
                <ul class="text-mid-gray text-sm space-y-1">
                  <li>• Vue 3 Composition API</li>
                  <li>• Vue Router 4</li>
                  <li>• Tailwind CSS 3</li>
                  <li>• WebGL & Three.js</li>
                </ul>
              </div>
              <div
                class="bg-twilight-blue/50 p-4 rounded-lg border border-mystic-cyan/30"
              >
                <h3 class="font-semibold text-mystic-cyan mb-2">Blockchain</h3>
                <ul class="text-mid-gray text-sm space-y-1">
                  <li>• Ethereum Network</li>
                  <li>• Smart Contracts</li>
                  <li>• Web3 Integration</li>
                  <li>• IPFS Storage</li>
                </ul>
              </div>
            </div>
          </div>

          <!-- Features -->
          <div class="mb-8">
            <h2 class="text-2xl font-bold text-mist-white mb-6">
              Core Features
            </h2>
            <div class="space-y-4">
              <div class="flex items-start space-x-3">
                <div
                  class="w-6 h-6 bg-mystic-cyan/20 rounded-full flex-shrink-0 mt-0.5"
                >
                  <div
                    class="w-2 h-2 bg-mystic-cyan rounded-full mx-auto mt-2"
                  ></div>
                </div>
                <div>
                  <h4 class="font-semibold text-mist-white">
                    NFT Asset Ownership
                  </h4>
                  <p class="text-mid-gray text-sm">
                    True ownership of in-game items, characters, and land
                    through blockchain-verified NFTs.
                  </p>
                </div>
              </div>

              <div class="flex items-start space-x-3">
                <div
                  class="w-6 h-6 bg-lamp-yellow/20 rounded-full flex-shrink-0 mt-0.5"
                >
                  <div
                    class="w-2 h-2 bg-lamp-yellow rounded-full mx-auto mt-2"
                  ></div>
                </div>
                <div>
                  <h4 class="font-semibold text-mist-white">
                    Play-to-Earn Economy
                  </h4>
                  <p class="text-mid-gray text-sm">
                    Earn cryptocurrency rewards through gameplay, tournaments,
                    and community participation.
                  </p>
                </div>
              </div>

              <div class="flex items-start space-x-3">
                <div
                  class="w-6 h-6 bg-night-forest/40 rounded-full flex-shrink-0 mt-0.5"
                >
                  <div
                    class="w-2 h-2 bg-night-forest rounded-full mx-auto mt-2"
                  ></div>
                </div>
                <div>
                  <h4 class="font-semibold text-mist-white">
                    Decentralized Governance
                  </h4>
                  <p class="text-mid-gray text-sm">
                    Community-driven decision making through DAO voting
                    mechanisms.
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Stats -->
          <div class="mb-8">
            <h2 class="text-2xl font-bold text-mist-white mb-6">
              Platform Statistics
            </h2>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div class="text-center p-4 bg-slate-concrete/50 rounded-lg">
                <div class="text-2xl font-bold text-mystic-cyan">1M+</div>
                <div class="text-xs text-mid-gray">Users</div>
              </div>
              <div class="text-center p-4 bg-slate-concrete/50 rounded-lg">
                <div class="text-2xl font-bold text-lamp-yellow">$50M</div>
                <div class="text-xs text-mid-gray">Volume</div>
              </div>
              <div class="text-center p-4 bg-slate-concrete/50 rounded-lg">
                <div class="text-2xl font-bold text-mystic-cyan">10K</div>
                <div class="text-xs text-mid-gray">NFTs</div>
              </div>
              <div class="text-center p-4 bg-slate-concrete/50 rounded-lg">
                <div class="text-2xl font-bold text-lamp-yellow">99.9%</div>
                <div class="text-xs text-mid-gray">Uptime</div>
              </div>
            </div>
          </div>

          <!-- Call to Action -->
          <div class="text-center">
            <router-link
              to="/"
              class="btn-primary inline-flex items-center justify-center mr-4"
            >
              Return Home
              <svg
                class="ml-2 w-4 h-4"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M10 19l-7-7m0 0l7-7m-7 7h18"
                ></path>
              </svg>
            </router-link>
            <a
              href="#"
              class="text-night-forest hover:text-mystic-cyan transition-colors"
            >
              Join Community →
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "About",
};
</script>
