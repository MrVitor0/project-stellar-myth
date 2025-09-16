<template>
  <div id="app" class="min-h-screen bg-twilight-blue">
    <!-- Conteúdo principal -->
    <main class="flex-1">
      <router-view />
    </main>

    <!-- Footer -->
    <footer class="text-white py-8 mt-auto">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center">
          <p class="text-gray-300">
            © 2025 Project Stellar Myth - Meridian Hack
          </p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script>
export default {
  name: "App",
};
</script>
