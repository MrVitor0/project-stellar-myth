<template>
  <div class="blessing-showcase">
    <div class="text-center justify-center mb-8">
      <h2 class="text-3xl font-bold text-mist-white mb-3">Bênçãos Recentes</h2>
      <p class="text-mid-gray">Criadas pela comunidade</p>
      <div class="flex items-center justify-center mt-2">
        <div
          class="w-2 h-2 bg-brazil-green rounded-full animate-pulse mr-2"
        ></div>
        <span class="text-sm text-brazil-green"
          >{{ blessings.length }} novas bênçãos</span
        >
      </div>
    </div>

    <div
      class="space-y-4 max-h-96 mx-auto justify-center overflow-y-auto custom-scrollbar"
    >
      <BlessingCard
        v-for="blessing in blessings"
        :key="blessing.id"
        :blessing="blessing"
        class="animate-fade-in-up"
      />
    </div>

    <div class="text-center mt-6">
      <button
        class="btn-secondary text-sm py-2 px-4 inline-flex items-center justify-center"
      >
        <Plus class="w-4 h-4 mr-2" />
        Criar Bênção
      </button>
    </div>
  </div>
</template>

<script>
import { Plus } from "lucide-vue-next";
import BlessingCard from "./BlessingCard.vue";

export default {
  name: "BlessingShowcase",
  components: {
    BlessingCard,
    Plus,
  },
  props: {
    blessings: {
      type: Array,
      default: () => [],
      validator(blessings) {
        return blessings.every(
          (blessing) =>
            blessing.id &&
            blessing.name &&
            blessing.description &&
            blessing.gift &&
            blessing.patron &&
            blessing.type &&
            blessing.creator &&
            blessing.timeAgo
        );
      },
    },
  },
};
</script>

<style scoped>
.custom-scrollbar {
  scrollbar-width: thin;
  scrollbar-color: rgba(34, 197, 94, 0.3) rgba(15, 23, 42, 0.2);
}

.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: rgba(15, 23, 42, 0.2);
  border-radius: 3px;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: rgba(34, 197, 94, 0.3);
  border-radius: 3px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: rgba(34, 197, 94, 0.5);
}

.animate-fade-in-up {
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
