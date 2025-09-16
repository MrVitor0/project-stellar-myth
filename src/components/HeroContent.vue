<template>
  <div class="hero-content">
    <div class="text-left">
      <h1 class="hero-title">
        <span class="text-mist-white">{{ title.prefix }}</span>
        <br />
        <span class="stellar-gradient glow-text">{{ title.highlight }}</span>
      </h1>

      <p class="hero-description">
        {{ description }}
      </p>

      <div class="cta-buttons">
        <button
          @click="$router.push('/forge/myth')"
          class="btn-primary inline-flex items-center justify-center group"
        >
          <component
            :is="primaryButton.icon"
            class="mr-2 w-5 h-5 group-hover:scale-110 transition-transform"
          />
          {{ primaryButton.text }}
        </button>

        <button
          @click="launchGame"
          class="btn-secondary group inline-flex items-center justify-center"
        >
          {{ secondaryButton.text }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { Play, Wallet, Users, Coins, Trophy } from "lucide-vue-next";

export default {
  name: "HeroContent",
  components: {
    Play,
    Wallet,
    Users,
    Coins,
    Trophy,
  },
  methods: {
    launchGame() {
      // Emite um evento para o componente pai
      this.$emit("launch-game");
    },
  },
  props: {
    title: {
      type: Object,
      default: () => ({
        prefix: "Enter the",
        highlight: "Stellar Myth",
      }),
    },
    description: {
      type: String,
      default:
        "Experience the future of gaming where blockchain technology meets immersive gameplay. Build, trade, and conquer in a decentralized universe powered by your creativity.",
    },
    primaryButton: {
      type: Object,
      default: () => ({
        text: "Launch Game",
        icon: "Play",
      }),
    },
    secondaryButton: {
      type: Object,
      default: () => ({
        text: "Connect Wallet",
        icon: "Wallet",
      }),
    },
    stats: {
      type: Object,
      default: () => ({
        players: "1M+",
        volume: "$50M",
        nfts: "10K+",
      }),
    },
  },
};
</script>

<style scoped>
.hero-title {
  font-size: 4rem;
  font-weight: 900;
  margin-bottom: 2rem;
  line-height: 1.1;
  letter-spacing: -0.02em;
}

@media (min-width: 768px) {
  .hero-title {
    font-size: 6rem;
  }
}

.stellar-gradient {
  background: linear-gradient(
    135deg,
    #22c55e 0%,
    /* brazil-green */ #fbbf24 25%,
    /* brazil-yellow */ #06b6d4 50%,
    /* mystic-cyan */ #22c55e 75%,
    /* brazil-green */ #fbbf24 100% /* brazil-yellow */
  );
  background-size: 200% 200%;
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  animation: gradientShift 4s ease-in-out infinite;
}

@keyframes gradientShift {
  0%,
  100% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
}

.hero-description {
  font-size: 1.25rem;
  color: #9ca3af;
  margin-bottom: 3rem;
  max-width: 42rem;
  line-height: 1.6;
}

@media (min-width: 768px) {
  .hero-description {
    font-size: 1.5rem;
  }
}

.cta-buttons {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  margin-bottom: 3rem;
}

@media (min-width: 640px) {
  .cta-buttons {
    flex-direction: row;
  }
}

.stats-preview {
  display: flex;
  gap: 1.5rem;
  padding-top: 2rem;
  border-top: 1px solid rgba(34, 197, 94, 0.2);
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #f8fafc;
}

.glow-text {
  text-shadow: 0 0 10px rgba(34, 197, 94, 0.3), 0 0 20px rgba(34, 197, 94, 0.2),
    0 0 40px rgba(34, 197, 94, 0.1);
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 30px rgba(34, 197, 94, 0.3);
}

.btn-secondary:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 30px rgba(251, 191, 36, 0.2);
}
</style>
