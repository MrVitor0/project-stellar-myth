<template>
  <div class="min-h-screen py-12">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="text-center mb-12">
        <h1 class="text-4xl md:text-5xl font-bold text-gray-900 mb-6">
          Sobre o Projeto
        </h1>
        <p class="text-xl text-gray-600 max-w-2xl mx-auto">
          Conheça mais sobre o desenvolvimento e as tecnologias por trás do Stellar Myth.
        </p>
      </div>

      <!-- Content -->
      <div class="bg-white rounded-2xl shadow-xl overflow-hidden">
        <div class="p-8 md:p-12">
          <!-- Project Description -->
          <div class="mb-8">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">O Que é o Stellar Myth?</h2>
            <p class="text-gray-600 leading-relaxed mb-4">
              O Stellar Myth é um projeto de demonstração que combina as melhores práticas
              de desenvolvimento front-end moderno. Utilizando Vue 3 como framework principal,
              Vue Router para navegação e Tailwind CSS para estilização, este projeto serve
              como base para aplicações web robustas e escaláveis.
            </p>
            <p class="text-gray-600 leading-relaxed">
              O objetivo é fornecer uma estrutura sólida e bem organizada que pode ser
              facilmente adaptada para diferentes tipos de projetos, desde landing pages
              até aplicações complexas de single-page.
            </p>
          </div>

          <!-- Technical Stack -->
          <div class="mb-8">
            <h2 class="text-2xl font-bold text-gray-900 mb-6">Stack Técnico</h2>
            <div class="space-y-4">
              <div class="flex items-start space-x-4">
                <div class="w-2 h-2 bg-stellar-500 rounded-full mt-2 flex-shrink-0"></div>
                <div>
                  <h3 class="font-semibold text-gray-900">Vue 3 (Composition API)</h3>
                  <p class="text-gray-600 text-sm">Framework progressivo com melhor performance e TypeScript support</p>
                </div>
              </div>
              <div class="flex items-start space-x-4">
                <div class="w-2 h-2 bg-stellar-500 rounded-full mt-2 flex-shrink-0"></div>
                <div>
                  <h3 class="font-semibold text-gray-900">Vue Router 4</h3>
                  <p class="text-gray-600 text-sm">Roteamento oficial com suporte a lazy loading e guards</p>
                </div>
              </div>
              <div class="flex items-start space-x-4">
                <div class="w-2 h-2 bg-stellar-500 rounded-full mt-2 flex-shrink-0"></div>
                <div>
                  <h3 class="font-semibold text-gray-900">Tailwind CSS 3</h3>
                  <p class="text-gray-600 text-sm">Framework CSS utility-first com design system customizável</p>
                </div>
              </div>
              <div class="flex items-start space-x-4">
                <div class="w-2 h-2 bg-stellar-500 rounded-full mt-2 flex-shrink-0"></div>
                <div>
                  <h3 class="font-semibold text-gray-900">Vite</h3>
                  <p class="text-gray-600 text-sm">Build tool moderna com Hot Module Replacement ultrarrápido</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Features -->
          <div class="mb-8">
            <h2 class="text-2xl font-bold text-gray-900 mb-6">Características</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="bg-gray-50 p-6 rounded-lg">
                <h3 class="font-semibold text-gray-900 mb-2">📱 Responsivo</h3>
                <p class="text-gray-600 text-sm">Design que se adapta perfeitamente a todos os tamanhos de tela</p>
              </div>
              <div class="bg-gray-50 p-6 rounded-lg">
                <h3 class="font-semibold text-gray-900 mb-2">⚡ Performance</h3>
                <p class="text-gray-600 text-sm">Otimizado para carregamento rápido e experiência fluida</p>
              </div>
              <div class="bg-gray-50 p-6 rounded-lg">
                <h3 class="font-semibold text-gray-900 mb-2">🎨 Design System</h3>
                <p class="text-gray-600 text-sm">Paleta de cores personalizada e componentes reutilizáveis</p>
              </div>
              <div class="bg-gray-50 p-6 rounded-lg">
                <h3 class="font-semibold text-gray-900 mb-2">🔧 Configurável</h3>
                <p class="text-gray-600 text-sm">Estrutura flexível e fácil de personalizar</p>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex flex-col sm:flex-row gap-4 justify-center pt-8 border-t border-gray-200">
            <router-link to="/" class="btn-primary inline-flex items-center justify-center">
              <svg class="mr-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
              </svg>
              Voltar ao Início
            </router-link>
            <button class="btn-secondary">
              Ver no GitHub
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'About'
}
</script>