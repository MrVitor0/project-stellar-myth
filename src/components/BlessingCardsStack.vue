<template>
  <div
    class="blessing-cards-stack relative flex items-center justify-center w-full h-full"
  >
    <!-- Cards Stack -->
    <div
      class="cards-container relative h-[450px] w-full max-w-xl flex items-center justify-center"
    >
      <!-- Card Left -->
      <div
        class="blessing-card card-left absolute top-1/3 left-1/2 -translate-y-1/2 bg-gradient-to-br from-slate-800 to-slate-900 rounded-2xl shadow-2xl border border-brazil-green/20 backdrop-blur-sm transform -rotate-12 -translate-x-[calc(50%+80px)] transition-all duration-300 group"
        v-if="blessings[0]"
      >
        <div class="p-6 h-full">
          <!-- Card Glow Effect -->
          <div
            class="absolute inset-0 bg-gradient-to-br from-brazil-green/5 to-transparent rounded-2xl opacity-0 group-hover:opacity-100 transition-opacity duration-300"
          ></div>

          <img
            v-if="blessings[2].image"
            src="@/assets/images/sepe.png"
            alt="Blessing Image"
            class="mx-auto h-64 w-64 mb-4 rounded-lg"
          />
          <!-- Content -->
          <div class="text-center relative z-10">
            <h3 class="text-xl font-bold text-mist-white mb-2">
              {{ blessings[2].name || "Bênção Dourada" }}
            </h3>

            <p class="text-sm text-mid-gray line-clamp-2 mb-3">
              {{
                blessings[2].description || "A bênção mais poderosa da coleção"
              }}
            </p>
          </div>
        </div>
      </div>

      <!-- Card Center (Top) -->
      <div
        class="blessing-card card-center absolute top-1/3 left-1/2 -translate-x-1/2 -translate-y-1/2 bg-gradient-to-br from-gray-800 to-gray-900 rounded-2xl shadow-2xl border border-brazil-yellow/30 backdrop-blur-sm transform z-10 cursor-pointer"
        v-if="blessings[1]"
        @click="$emit('create-blessing')"
      >
        <div class="p-6 h-full flex flex-col justify-between">
          <!-- Card Glow Effect -->
          <div
            class="absolute inset-0 bg-gradient-to-br from-brazil-yellow/10 to-transparent rounded-2xl opacity-0 group-hover:opacity-100 transition-opacity duration-500"
          ></div>

          <!-- Featured Badge -->
          <div
            class="absolute -top-2 left-1/2 transform -translate-x-1/2 px-3 py-1 bg-brazil-yellow text-dark-night text-xs font-bold rounded-full shadow-lg"
          >
            NEW
          </div>

          <img
            v-if="blessings[1].image"
            src="@/assets/images/moreira.png"
            alt="Blessing Image"
            class="mx-auto h-64 w-64 mb-4 rounded-lg"
          />
          <!-- Content -->
          <div class="text-center relative z-10">
            <h3 class="text-xl font-bold text-mist-white mb-2">
              {{ blessings[1].name || "Bênção Dourada" }}
            </h3>

            <p class="text-sm text-mid-gray line-clamp-2 mb-3">
              {{
                blessings[1].description || "A bênção mais poderosa da coleção"
              }}
            </p>
          </div>
        </div>
      </div>

      <!-- Card Right -->
      <div
        class="blessing-card card-right absolute top-1/2 left-1/2 -translate-y-1/2 bg-gradient-to-br from-slate-800 to-slate-900 rounded-2xl shadow-2xl border border-mystic-cyan/20 backdrop-blur-sm transform rotate-12 transition-all duration-300 group"
        v-if="blessings[2]"
      >
        <div class="p-6 h-full">
          <!-- Card Glow Effect -->
          <div
            class="absolute inset-0 bg-gradient-to-br from-mystic-cyan/5 to-transparent rounded-2xl opacity-0 group-hover:opacity-100 transition-opacity duration-300"
          ></div>

          <img
            v-if="blessings[2].image"
            src="@/assets/images/sepe.png"
            alt="Blessing Image"
            class="mx-auto h-64 w-64 mb-4 rounded-lg"
          />
          <!-- Content -->
          <div class="text-center relative z-10">
            <h3 class="text-xl font-bold text-mist-white mb-2">
              {{ blessings[2].name || "Bênção Dourada" }}
            </h3>

            <p class="text-sm text-mid-gray line-clamp-2 mb-3">
              {{
                blessings[2].description || "A bênção mais poderosa da coleção"
              }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "BlessingCardsStack",
  props: {
    blessings: {
      type: Array,
      default: () => [],
    },
    title: {
      type: String,
      default: "Bênçãos Mágicas",
    },
    description: {
      type: String,
      default: "Descubra o poder das bênçãos criadas pela comunidade",
    },
  },
  emits: ["create-blessing"],
};
</script>

<style scoped>
/* Blessing Cards Stack Styles */
.blessing-cards-stack {
  perspective: 1000px;
}

.cards-container {
  transform-style: preserve-3d;
}

.blessing-card {
  width: 320px;
  height: 380px;
  transform-origin: center;
  position: absolute;
  margin: 0 auto;
}

/* Individual Card Styles */
.card-left {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(34, 197, 94, 0.1),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

.card-left {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(34, 197, 94, 0.1),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
  opacity: 0.7;
}

.card-center {
  background: linear-gradient(135deg, #374151 0%, #1f2937 100%);
  box-shadow: 0 30px 60px rgba(0, 0, 0, 0.4), 0 0 0 1px rgba(251, 191, 36, 0.2),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

.card-right {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.3), 0 0 0 1px rgba(6, 182, 212, 0.1),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
  opacity: 0.7;
}

/* Card Content Styling */
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Ensure cards are visible in fullscreen mode */
@media (min-width: 1280px) {
  .cards-container {
    width: 90%;
    max-width: 1000px;
    height: 500px;
  }

  .blessing-card {
    width: 340px;
    height: 400px;
  }
}

.cards-container::before {
  content: "";
  position: absolute;
  top: -50px;
  left: -50px;
  right: -50px;
  bottom: -50px;
  background: radial-gradient(
    circle at center,
    rgba(34, 197, 94, 0.05) 0%,
    transparent 70%
  );
  border-radius: 50%;
  z-index: -1;
}

/* Featured Badge */
.card-center .absolute.-top-2 {
  box-shadow: 0 0 10px rgba(251, 191, 36, 0.6);
}

/* Responsive Adjustments */
@media (max-width: 768px) {
  .blessing-card {
    width: 260px;
    height: 320px;
  }

  .cards-container {
    height: 380px;
  }

  .card-left {
    transform: rotate(-12deg) translate(-50%, -50%);
    left: calc(50% - 40px);
  }

  .card-right {
    transform: rotate(12deg) translate(-50%, -50%);
    left: calc(50% + 40px);
  }

  .card-center {
    transform: translate(-50%, -50%);
  }
}
</style>
