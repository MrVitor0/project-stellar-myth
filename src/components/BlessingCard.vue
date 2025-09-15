<template>
  <div class="blessing-card gaming-card neon-glow p-6">
    <!-- Header with blessing name and type -->
    <div class="flex items-center justify-between mb-4">
      <h3 class="text-xl font-bold text-mist-white">{{ blessing.name }}</h3>
      <span
        class="px-3 py-1 text-xs rounded-full border"
        :class="getTypeClass(blessing.type)"
      >
        {{ blessing.type }}
      </span>
    </div>

    <!-- Patron section -->
    <div class="flex items-center mb-4">
      <div
        class="w-8 h-8 rounded-full bg-gradient-to-br from-brazil-green to-brazil-yellow flex items-center justify-center mr-3"
      >
        <Crown class="w-4 h-4 text-dark-night" />
      </div>
      <div>
        <p class="text-sm text-mid-gray">Patrono</p>
        <p class="text-brazil-yellow font-semibold">{{ blessing.patron }}</p>
      </div>
    </div>

    <!-- Description -->
    <p class="text-mid-gray text-sm mb-4 line-clamp-2">
      {{ blessing.description }}
    </p>

    <!-- Gift/Bonus -->
    <div class="flex items-center mb-4">
      <component
        :is="getGiftIcon(blessing.gift)"
        class="w-5 h-5 mr-2"
        :class="getGiftIconColor(blessing.gift)"
      />
      <span class="text-brazil-green font-semibold">{{ blessing.gift }}</span>
    </div>

    <!-- Creator info -->
    <div
      class="flex items-center justify-between pt-4 border-t border-brazil-green/20"
    >
      <div class="flex items-center">
        <div
          class="w-6 h-6 rounded-full bg-gradient-to-br from-deep-blue to-slate-concrete mr-2"
        ></div>
        <span class="text-xs text-mid-gray">Por {{ blessing.creator }}</span>
      </div>
      <div class="flex items-center text-xs text-brazil-green">
        <Clock class="w-3 h-3 mr-1" />
        {{ blessing.timeAgo }}
      </div>
    </div>
  </div>
</template>

<script>
import { Crown, Clock, Sword, Zap, Shield, Heart } from "lucide-vue-next";

export default {
  name: "BlessingCard",
  components: {
    Crown,
    Clock,
    Sword,
    Zap,
    Shield,
    Heart,
  },
  props: {
    blessing: {
      type: Object,
      required: true,
      validator(value) {
        return (
          value.name &&
          value.description &&
          value.gift &&
          value.patron &&
          value.type &&
          value.creator &&
          value.timeAgo
        );
      },
    },
  },
  methods: {
    getTypeClass(type) {
      const classes = {
        "Ao atacar": "bg-red-900/30 border-red-500/50 text-red-300",
        Passivo: "bg-blue-900/30 border-blue-500/50 text-blue-300",
      };
      return classes[type] || "bg-gray-900/30 border-gray-500/50 text-gray-300";
    },

    getGiftIcon(gift) {
      if (gift.includes("Ataque")) return "Sword";
      if (gift.includes("Velocidade")) return "Zap";
      if (gift.includes("Defesa")) return "Shield";
      if (gift.includes("Vida")) return "Heart";
      return "Sword";
    },

    getGiftIconColor(gift) {
      if (gift.includes("Ataque")) return "text-red-400";
      if (gift.includes("Velocidade")) return "text-yellow-400";
      if (gift.includes("Defesa")) return "text-blue-400";
      if (gift.includes("Vida")) return "text-green-400";
      return "text-brazil-green";
    },
  },
};
</script>

<style scoped>
.blessing-card {
  background: linear-gradient(
    135deg,
    rgba(15, 23, 42, 0.9),
    rgba(30, 41, 59, 0.8)
  );
  backdrop-filter: blur(10px);
  border: 1px solid rgba(34, 197, 94, 0.2);
  transition: all 0.3s ease;
}

.blessing-card:hover {
  transform: translateY(-2px);
  border-color: rgba(34, 197, 94, 0.4);
  box-shadow: 0 10px 40px rgba(34, 197, 94, 0.1);
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
