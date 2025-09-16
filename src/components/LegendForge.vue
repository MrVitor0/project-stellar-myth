<template>
  <section class="legend-forge-section pb-8 relative overflow-hidden">
    <!-- Background Elements -->
    <!-- Main Content -->
    <div class="container mx-auto px-4 relative z-10">
      <!-- Section Header -->
      <div class="text-center mb-16">
        <h2 class="text-4xl md:text-5xl font-bold text-mist-white mb-4">
          The Legend's Forge
        </h2>
        <p class="text-mid-gray max-w-2xl mx-auto">
          More than a game, Stellar Myth is a collaborative mythology. Here's
          how you can leave your mark on the universe in three simple steps.
        </p>
      </div>

      <!-- Steps Timeline -->
      <div
        class="steps-timeline grid grid-cols-1 md:grid-cols-3 gap-8 max-w-6xl mx-auto"
      >
        <!-- Step 1: Create -->
        <div
          class="step-card bg-gradient-to-br from-gray-800/80 to-gray-900/90 p-8 rounded-2xl border border-brazil-green/20 transform transition-all duration-300 hover:-translate-y-2 hover:shadow-lg hover:shadow-brazil-green/10 group"
        >
          <div class="icon-container mb-6 flex justify-center">
            <div
              class="rounded-full p-6 bg-gradient-to-br from-brazil-green/20 to-brazil-green/5 group-hover:from-brazil-green/30 group-hover:to-brazil-green/10 transition-all duration-300"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-12 w-12 text-brazil-green"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="1.5"
                  d="M12 4.5v15m7.5-7.5h-15"
                />
              </svg>
            </div>
          </div>
          <h3 class="text-2xl font-bold text-mist-white text-center mb-4">
            FORGE
          </h3>
          <p class="text-mid-gray text-center">
            Design a unique power-up with our simple tools and watch as our AI
            forges your concept into a true legend.
          </p>
        </div>

        <!-- Step 2: Register -->
        <div
          class="step-card bg-gradient-to-br from-gray-800/80 to-gray-900/90 p-8 rounded-2xl border border-brazil-yellow/20 transform transition-all duration-300 hover:-translate-y-2 hover:shadow-lg hover:shadow-brazil-yellow/10 group"
        >
          <div class="icon-container mb-6 flex justify-center">
            <div
              class="rounded-full p-6 bg-gradient-to-br from-brazil-yellow/20 to-brazil-yellow/5 group-hover:from-brazil-yellow/30 group-hover:to-brazil-yellow/10 transition-all duration-300"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-12 w-12 text-brazil-yellow"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="1.5"
                  d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
          </div>
          <h3 class="text-2xl font-bold text-mist-white text-center mb-4">
            IMMORTALIZE
          </h3>
          <p class="text-mid-gray text-center">
            Commit your creation to the Stellar blockchain, giving it a
            permanent and verifiable place in the game's history.
          </p>
        </div>

        <!-- Step 3: Impact -->
        <div
          class="step-card bg-gradient-to-br from-gray-800/80 to-gray-900/90 p-8 rounded-2xl border border-mystic-cyan/20 transform transition-all duration-300 hover:-translate-y-2 hover:shadow-lg hover:shadow-mystic-cyan/10 group"
        >
          <div class="icon-container mb-6 flex justify-center">
            <div
              class="rounded-full p-6 bg-gradient-to-br from-mystic-cyan/20 to-mystic-cyan/5 group-hover:from-mystic-cyan/30 group-hover:to-mystic-cyan/10 transition-all duration-300"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-12 w-12 text-mystic-cyan"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="1.5"
                  d="M14.25 6.087c0-.355.186-.676.401-.959.221-.29.349-.634.349-1.003 0-1.036-1.007-1.875-2.25-1.875s-2.25.84-2.25 1.875c0 .369.128.713.349 1.003.215.283.401.604.401.959v0a.64.64 0 01-.657.643 48.39 48.39 0 01-4.163-.3c.186 1.613.293 3.25.315 4.907a.656.656 0 01-.658.663v0c-.355 0-.676-.186-.959-.401a1.647 1.647 0 00-1.003-.349c-1.036 0-1.875 1.007-1.875 2.25s.84 2.25 1.875 2.25c.369 0 .713-.128 1.003-.349.283-.215.604-.401.959-.401v0c.31 0 .555.26.532.57a48.039 48.039 0 01-.642 5.056c1.518.19 3.058.309 4.616.354a.64.64 0 00.657-.643v0c0-.355-.186-.676-.401-.959a1.647 1.647 0 01-.349-1.003c0-1.035 1.008-1.875 2.25-1.875 1.243 0 2.25.84 2.25 1.875 0 .369-.128.713-.349 1.003-.215.283-.4.604-.4.959v0c0 .333.277.599.61.58a48.1 48.1 0 005.427-.63 48.05 48.05 0 00.582-4.717.532.532 0 00-.533-.57v0c-.355 0-.676.186-.959.401-.29.221-.634.349-1.003.349-1.035 0-1.875-1.007-1.875-2.25s.84-2.25 1.875-2.25c.37 0 .713.128 1.003.349.283.215.604.401.96.401v0a.656.656 0 00.658-.663 48.422 48.422 0 00-.37-5.36c-1.886.342-3.81.574-5.766.689a.578.578 0 01-.61-.58v0z"
                />
              </svg>
            </div>
          </div>
          <h3 class="text-2xl font-bold text-mist-white text-center mb-4">
            IMPACT
          </h3>
          <p class="text-mid-gray text-center">
            Unleash your creation into the world. Your Myth will now appear in
            other players' runs, shaping their adventure.
          </p>
        </div>
      </div>

      <!-- Animated Transition & CTA Section -->
      <div class="cta-container mt-20 mb-10 relative">
        <!-- CTA Text & Buttons -->
        <div class="text-center max-w-2xl mx-auto">
          <h3 class="text-3xl font-bold text-mist-white mb-4">
            Ready to forge your legend?
          </h3>
          <p class="text-mid-gray mb-8">
            Choose your path. Become a Creator and add a new power to the
            universe, or step into the fray as a Hero and wield the legends
            others have built.
          </p>

          <!-- CTA Buttons -->
          <div class="flex flex-col md:flex-row gap-4 justify-center">
            <button
              @click="$router.push('/forge/myth')"
              class="px-8 py-4 bg-gradient-to-r from-brazil-yellow to-brazil-yellow/80 text-dark-night font-bold rounded-lg transform transition-all duration-300 hover:scale-105 hover:shadow-lg hover:shadow-brazil-yellow/20 flex items-center justify-center"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 mr-2"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 9v3m0 0v3m0-3h3m-3 0H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              Forge a New Myth
            </button>

            <button
              @click="launchGame"
              class="px-8 py-4 bg-gradient-to-r from-mystic-cyan/90 to-mystic-cyan/70 text-dark-night font-bold rounded-lg transform transition-all duration-300 hover:scale-105 hover:shadow-lg hover:shadow-mystic-cyan/20 flex items-center justify-center"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 mr-2"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z"
                />
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              Launch Game
            </button>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script>
export default {
  name: "LegendForge",
  methods: {
    launchGame() {
      window.open('/webgl/index.html', '_blank', 'fullscreen=yes');
    }
  },
};
</script>

<style scoped>
.legend-forge-section {
  position: relative;
}

/* Connection lines styling removed */

.step-card {
  position: relative;
  backdrop-filter: blur(10px);
  overflow: hidden;
}

.step-card::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.2),
    transparent
  );
  opacity: 0;
  transition: opacity 0.3s ease;
}

.step-card:hover::before {
  opacity: 1;
}

/* Animation for cards on scroll */
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

.step-card {
  animation: fadeInUp 0.6s ease-out forwards;
  opacity: 0;
}

.step-card:nth-child(1) {
  animation-delay: 0.1s;
}

.step-card:nth-child(2) {
  animation-delay: 0.3s;
}

.step-card:nth-child(3) {
  animation-delay: 0.5s;
}

@media (max-width: 768px) {
  .step-card {
    margin-bottom: 2rem;
  }
}

/* Animated CTA Section Styles */
.decoration-container {
  height: 120px;
}

@keyframes ping-slow {
  0% {
    transform: scale(0.2);
    opacity: 0;
  }
  50% {
    opacity: 0.6;
  }
  100% {
    transform: scale(2);
    opacity: 0;
  }
}

@keyframes float {
  0% {
    transform: translateY(0px);
  }
  50% {
    transform: translateY(-10px);
  }
  100% {
    transform: translateY(0px);
  }
}

.animate-ping-slow {
  animation: ping-slow 3s cubic-bezier(0, 0, 0.2, 1) infinite;
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}

.cta-container button {
  box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.3);
  backdrop-filter: blur(4px);
}

.cta-container button:hover {
  box-shadow: 0 20px 35px -10px rgba(0, 0, 0, 0.4),
    0 0 15px rgba(251, 191, 36, 0.4);
  transform: translateY(-2px) scale(1.01);
}

.cta-container button svg {
  filter: drop-shadow(0 0 2px rgba(255, 255, 255, 0.5));
}
</style>
