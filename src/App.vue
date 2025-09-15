<template>
  <div id="app" class="min-h-screen bg-gray-50">
    <!-- Navegação -->
    <nav class="bg-white shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <router-link to="/" class="text-xl font-bold text-gray-800 hover:text-blue-600 transition-colors">
              Project Stellar Myth
            </router-link>
          </div>
          <div class="flex items-center space-x-4">
            <router-link to="/"
              class="text-gray-600 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors"
              :class="{ 'text-blue-600 font-semibold': $route.path === '/' }">
              Home
            </router-link>
            <router-link to="/about"
              class="text-gray-600 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors"
              :class="{ 'text-blue-600 font-semibold': $route.path === '/about' }">
              About
            </router-link>
          </div>
        </div>
      </div>
    </nav>

    <!-- Conteúdo principal -->
    <main class="flex-1">
      <router-view />
    </main>

    <!-- Footer -->
    <footer class="bg-gray-800 text-white py-8 mt-auto">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center">
          <p class="text-gray-300">
            © 2025 Project Stellar Myth. Feito com Vue 3 + Tailwind CSS.
          </p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script>
export default {
  name: 'App'
}
</script>