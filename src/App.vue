<template>
  <div id="app" class="min-h-screen bg-twilight-blue">
    <!-- Conteúdo principal -->
    <main class="flex-1">
      <router-view />
    </main>
  </div>
</template>

<script>
export default {
  name: "App",
};
</script>
