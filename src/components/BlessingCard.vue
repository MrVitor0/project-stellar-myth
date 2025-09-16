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
        :is="getGiftIcon(blessing.buff || blessing.gift)"
        class="w-5 h-5 mr-2"
        :class="getGiftIconColor(blessing.buff || blessing.gift)"
      />
      <span class="text-brazil-green font-semibold">{{
        blessing.buff || blessing.gift
      }}</span>
    </div>

    <!-- Blockchain Data (if available) -->
    <div
      v-if="blessing.id && blessing.id.toString().startsWith('ST-')"
      class="mb-4 space-y-2"
    >
      <!-- Stellar Transaction ID -->
      <div
        class="p-2 bg-gradient-to-r from-brazil-green/10 to-deep-blue/10 rounded-lg border border-brazil-green/20"
      >
        <div class="flex items-center mb-1">
          <svg
            class="w-3 h-3 text-brazil-green mr-1"
            fill="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 17.93c-3.94-.49-7-3.85-7-7.93 0-.62.08-1.21.21-1.79L9 15v1c0 1.1.9 2 2 2v1.93zm6.9-2.54c-.26-.81-1-1.39-1.9-1.39h-1v-3c0-.55-.45-1-1-1H8v-2h2c.55 0 1-.45 1-1V7h2c1.1 0 2-.9 2-2v-.41c2.93 1.19 5 4.06 5 7.41 0 2.08-.8 3.97-2.1 5.39z"
            />
          </svg>
          <span class="text-xs font-medium text-brazil-green">STELLAR TX</span>
        </div>
        <div class="font-mono text-xs text-mist-white">
          {{ blessing.id }}
        </div>
      </div>

      <!-- Creator Account (if available) -->
      <div
        v-if="blessing.creator && blessing.creator.length > 40"
        class="p-2 bg-gradient-to-r from-deep-blue/10 to-brazil-green/10 rounded-lg border border-deep-blue/20"
      >
        <div class="flex items-center mb-1">
          <svg
            class="w-3 h-3 text-deep-blue mr-1"
            fill="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              d="M12 2C13.1 2 14 2.9 14 4C14 5.1 13.1 6 12 6C10.9 6 10 5.1 10 4C10 2.9 10.9 2 12 2ZM21 9V7L15 1H5C3.9 1 3 1.9 3 3V19C3 20.1 3.9 21 5 21H19C20.1 21 21 20.1 21 19V9M19 9H14V4H19V9Z"
            />
          </svg>
          <span class="text-xs font-medium text-deep-blue">CONTA</span>
        </div>
        <div class="font-mono text-xs text-mist-white break-all">
          {{ blessing.creator }}
        </div>
      </div>
    </div>

    <!-- Creator info -->
    <div
      class="flex items-center justify-between pt-4 border-t border-brazil-green/20"
    >
      <div class="flex items-center">
        <div
          class="w-6 h-6 rounded-full bg-gradient-to-br from-deep-blue to-slate-concrete mr-2"
        ></div>
        <span class="text-xs text-mid-gray"
          >Por
          {{
            blessing.creatorShort ||
            (blessing.creator && blessing.creator.length > 40
              ? blessing.creator.slice(0, 8) + "..."
              : blessing.creator)
          }}</span
        >
      </div>
      <div class="flex items-center text-xs">
        <Clock class="w-3 h-3 mr-1" />
        <span
          :class="
            blessing.id && blessing.id.toString().startsWith('ST-')
              ? 'text-brazil-green'
              : 'text-mid-gray'
          "
        >
          {{ blessing.timeAgo }}
        </span>
        <svg
          v-if="blessing.id && blessing.id.toString().startsWith('ST-')"
          class="w-3 h-3 ml-1 text-brazil-green/70"
          fill="currentColor"
          viewBox="0 0 24 24"
        >
          <circle cx="12" cy="12" r="2" />
          <circle
            cx="12"
            cy="12"
            r="8"
            fill="none"
            stroke="currentColor"
            stroke-width="1.5"
          />
        </svg>
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
          (value.gift || value.buff) &&
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
      if (!gift) return "Sword";
      const giftLower = gift.toLowerCase();
      if (giftLower.includes("ataque") || giftLower.includes("dano"))
        return "Sword";
      if (giftLower.includes("velocidade") || giftLower.includes("movimento"))
        return "Zap";
      if (giftLower.includes("defesa") || giftLower.includes("resistencia"))
        return "Shield";
      if (
        giftLower.includes("vida") ||
        giftLower.includes("cura") ||
        giftLower.includes("regenera")
      )
        return "Heart";
      return "Sword";
    },

    getGiftIconColor(gift) {
      if (!gift) return "text-brazil-green";
      const giftLower = gift.toLowerCase();
      if (giftLower.includes("ataque") || giftLower.includes("dano"))
        return "text-red-400";
      if (giftLower.includes("velocidade") || giftLower.includes("movimento"))
        return "text-yellow-400";
      if (giftLower.includes("defesa") || giftLower.includes("resistencia"))
        return "text-blue-400";
      if (
        giftLower.includes("vida") ||
        giftLower.includes("cura") ||
        giftLower.includes("regenera")
      )
        return "text-green-400";
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
