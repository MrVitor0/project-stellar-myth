<template>
  <div class="min-h-screen py-12">
    <!-- Hero Section -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center">
        <h1 class="text-4xl md:text-6xl font-bold text-gray-900 mb-6">
          Bem-vindo ao
          <span class="text-stellar-600">Stellar Myth</span>
        </h1>
        <p class="text-xl text-gray-600 mb-8 max-w-3xl mx-auto">
          Um projeto desenvolvido com Vue 3, Vue Router e Tailwind CSS.
          Explore as funcionalidades e descubra as possibilidades infinitas.
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <router-link to="/about" class="btn-primary inline-flex items-center justify-center">
            Saiba Mais
            <svg class="ml-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
            </svg>
          </router-link>
          <button class="btn-secondary">
            Ver Documentação
          </button>
        </div>
      </div>
    </div>

    <!-- Features Section -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 mt-20">
      <div class="text-center mb-16">
        <h2 class="text-3xl font-bold text-gray-900 mb-4">Tecnologias Utilizadas</h2>
        <p class="text-lg text-gray-600">Stack moderna para desenvolvimento web</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <!-- Vue 3 -->
        <div class="bg-white p-6 rounded-xl shadow-lg hover:shadow-xl transition-shadow">
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center mb-4">
            <svg class="w-6 h-6 text-green-600" fill="currentColor" viewBox="0 0 24 24">
              <path d="M2,3L5,3L12,15L19,3L22,3L12,21L2,3Z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold text-gray-900 mb-2">Vue 3</h3>
          <p class="text-gray-600">
            Framework progressivo para construir interfaces de usuário modernas e reativas.
          </p>
        </div>

        <!-- Vue Router -->
        <div class="bg-white p-6 rounded-xl shadow-lg hover:shadow-xl transition-shadow">
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
            <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6">
              </path>
            </svg>
          </div>
          <h3 class="text-xl font-semibold text-gray-900 mb-2">Vue Router</h3>
          <p class="text-gray-600">
            Sistema de roteamento oficial do Vue.js para Single Page Applications.
          </p>
        </div>

        <!-- Tailwind CSS -->
        <div class="bg-white p-6 rounded-xl shadow-lg hover:shadow-xl transition-shadow">
          <div class="w-12 h-12 bg-cyan-100 rounded-lg flex items-center justify-center mb-4">
            <svg class="w-6 h-6 text-cyan-600" fill="currentColor" viewBox="0 0 24 24">
              <path
                d="M12,2C13.75,2 15.25,3 16,4.5C15.25,4 13.75,4.5 12,4.5C10.25,4.5 8.75,4 8,4.5C8.75,3 10.25,2 12,2Z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold text-gray-900 mb-2">Tailwind CSS</h3>
          <p class="text-gray-600">
            Framework CSS utility-first para desenvolvimento rápido e customizável.
          </p>
        </div>
      </div>
    </div>

    <!-- Stats Section -->
    <div class="bg-stellar-50 mt-20 py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
          <div>
            <div class="text-4xl font-bold text-stellar-600 mb-2">100%</div>
            <div class="text-gray-600">Responsivo</div>
          </div>
          <div>
            <div class="text-4xl font-bold text-stellar-600 mb-2">⚡</div>
            <div class="text-gray-600">Performance</div>
          </div>
          <div>
            <div class="text-4xl font-bold text-stellar-600 mb-2">🚀</div>
            <div class="text-gray-600">Moderno</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Home'
}
</script>