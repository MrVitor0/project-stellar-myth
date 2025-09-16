<template>
  <section class="legend-forge-section py-16 relative overflow-hidden">
    <!-- Background Elements -->
    <div class="absolute inset-0 bg-gradient-to-b from-dark-night/80 to-dark-night z-0"></div>
    <div class="absolute inset-0 bg-[radial-gradient(circle_at_center,rgba(251,191,36,0.05)_0%,transparent_70%)] z-0"></div>
    
    <!-- Main Content -->
    <div class="container mx-auto px-4 relative z-10">
      <!-- Section Header -->
      <div class="text-center mb-16">
        <h2 class="text-4xl md:text-5xl font-bold text-mist-white mb-4">A Forja de Lendas</h2>
        <p class="text-mid-gray max-w-2xl mx-auto">
          Transforme suas ideias em elementos que impactam diretamente o mundo do jogo
        </p>
      </div>
      
      <!-- Steps Timeline -->
      <div class="steps-timeline grid grid-cols-1 md:grid-cols-3 gap-8 max-w-6xl mx-auto">
        <!-- Step 1: Create -->
        <div class="step-card bg-gradient-to-br from-gray-800/80 to-gray-900/90 p-8 rounded-2xl border border-brazil-green/20 transform transition-all duration-300 hover:-translate-y-2 hover:shadow-lg hover:shadow-brazil-green/10 group">
          <div class="icon-container mb-6 flex justify-center">
            <div class="rounded-full p-6 bg-gradient-to-br from-brazil-green/20 to-brazil-green/5 group-hover:from-brazil-green/30 group-hover:to-brazil-green/10 transition-all duration-300">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-brazil-green" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 4.5v15m7.5-7.5h-15" />
              </svg>
            </div>
          </div>
          <h3 class="text-2xl font-bold text-mist-white text-center mb-4">Crie</h3>
          <p class="text-mid-gray text-center">
            Proponha novas bênçãos, personagens ou itens com nosso criador intuitivo
          </p>
        </div>

        <!-- Step 2: Register -->
        <div class="step-card bg-gradient-to-br from-gray-800/80 to-gray-900/90 p-8 rounded-2xl border border-brazil-yellow/20 transform transition-all duration-300 hover:-translate-y-2 hover:shadow-lg hover:shadow-brazil-yellow/10 group">
          <div class="icon-container mb-6 flex justify-center">
            <div class="rounded-full p-6 bg-gradient-to-br from-brazil-yellow/20 to-brazil-yellow/5 group-hover:from-brazil-yellow/30 group-hover:to-brazil-yellow/10 transition-all duration-300">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-brazil-yellow" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
          <h3 class="text-2xl font-bold text-mist-white text-center mb-4">Registre</h3>
          <p class="text-mid-gray text-center">
            Eternize sua criação na blockchain Stellar de forma segura e transparente
          </p>
        </div>

        <!-- Step 3: Impact -->
        <div class="step-card bg-gradient-to-br from-gray-800/80 to-gray-900/90 p-8 rounded-2xl border border-mystic-cyan/20 transform transition-all duration-300 hover:-translate-y-2 hover:shadow-lg hover:shadow-mystic-cyan/10 group">
          <div class="icon-container mb-6 flex justify-center">
            <div class="rounded-full p-6 bg-gradient-to-br from-mystic-cyan/20 to-mystic-cyan/5 group-hover:from-mystic-cyan/30 group-hover:to-mystic-cyan/10 transition-all duration-300">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-mystic-cyan" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M14.25 6.087c0-.355.186-.676.401-.959.221-.29.349-.634.349-1.003 0-1.036-1.007-1.875-2.25-1.875s-2.25.84-2.25 1.875c0 .369.128.713.349 1.003.215.283.401.604.401.959v0a.64.64 0 01-.657.643 48.39 48.39 0 01-4.163-.3c.186 1.613.293 3.25.315 4.907a.656.656 0 01-.658.663v0c-.355 0-.676-.186-.959-.401a1.647 1.647 0 00-1.003-.349c-1.036 0-1.875 1.007-1.875 2.25s.84 2.25 1.875 2.25c.369 0 .713-.128 1.003-.349.283-.215.604-.401.959-.401v0c.31 0 .555.26.532.57a48.039 48.039 0 01-.642 5.056c1.518.19 3.058.309 4.616.354a.64.64 0 00.657-.643v0c0-.355-.186-.676-.401-.959a1.647 1.647 0 01-.349-1.003c0-1.035 1.008-1.875 2.25-1.875 1.243 0 2.25.84 2.25 1.875 0 .369-.128.713-.349 1.003-.215.283-.4.604-.4.959v0c0 .333.277.599.61.58a48.1 48.1 0 005.427-.63 48.05 48.05 0 00.582-4.717.532.532 0 00-.533-.57v0c-.355 0-.676.186-.959.401-.29.221-.634.349-1.003.349-1.035 0-1.875-1.007-1.875-2.25s.84-2.25 1.875-2.25c.37 0 .713.128 1.003.349.283.215.604.401.96.401v0a.656.656 0 00.658-.663 48.422 48.422 0 00-.37-5.36c-1.886.342-3.81.574-5.766.689a.578.578 0 01-.61-.58v0z" />
              </svg>
            </div>
          </div>
          <h3 class="text-2xl font-bold text-mist-white text-center mb-4">Impacte</h3>
          <p class="text-mid-gray text-center">
            Veja sua criação ganhar vida e influenciar a experiência de todos os jogadores
          </p>
        </div>
      </div>
      
      <!-- Connection Lines (Hidden on Mobile) -->
      <div class="connection-lines hidden md:block absolute top-1/2 left-0 w-full z-0 px-20">
        <div class="h-0.5 bg-gradient-to-r from-brazil-green/30 via-brazil-yellow/30 to-mystic-cyan/30 w-full"></div>
      </div>
    </div>
  </section>
</template>

<script>
export default {
  name: "LegendForge"
};
</script>

<style scoped>
.legend-forge-section {
  position: relative;
}

.connection-lines {
  transform: translateY(-170px);
  pointer-events: none;
}

.step-card {
  position: relative;
  backdrop-filter: blur(10px);
  overflow: hidden;
}

.step-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.2), transparent);
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
</style>
