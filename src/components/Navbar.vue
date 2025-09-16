<template>
  <nav class="relative z-50 py-6 bg-transparent">
    <div class="mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center">
        <router-link to="/" class="text-2xl font-bold text-brazil-yellow">
          Stellar Myth
        </router-link>
        <div class="hidden md:flex space-x-8 items-center">
          <router-link
            to="/forge/myth"
            class="text-mid-gray hover:text-brazil-green transition-colors duration-300"
          >
            Forge a Myth
          </router-link>
          <template v-if="currentUser">
            <button
              @click="handleLogout"
              class="px-4 py-2 rounded-md bg-red-500 hover:bg-red-600 text-white transition-colors duration-300"
            >
              Logout
            </button>
          </template>
          <router-link
            v-else
            to="/login"
            class="text-mid-gray hover:text-brazil-green transition-colors duration-300"
          >
            Join Community
          </router-link>
        </div>
      </div>
    </div>
  </nav>
</template>

<script>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import AuthService from "@/services/AuthService";

export default {
  name: "Navbar",
  setup() {
    const router = useRouter();
    const currentUser = ref(null);

    const checkAuthStatus = () => {
      currentUser.value = AuthService.getCurrentUser();
    };

    onMounted(() => {
      checkAuthStatus();
      // Add an auth state listener to update the navbar when the user logs in/out
      const unsubscribe = AuthService.onAuthStateChanged((user) => {
        currentUser.value = user;
      });
    });

    const handleLogout = async () => {
      try {
        await AuthService.logout();
        currentUser.value = null;
        router.push("/");
      } catch (error) {
        console.error("Error logging out:", error);
      }
    };

    return {
      currentUser,
      handleLogout,
    };
  },
};
</script>

<style scoped>
nav {
  backdrop-filter: blur(5px);
  -webkit-backdrop-filter: blur(5px);
}

/* Adicione outros estilos específicos do navbar aqui, se necessário */
</style>
