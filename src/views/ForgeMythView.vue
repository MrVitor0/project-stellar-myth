<template>
  <div class="min-h-screen blockchain-pattern">
    <div class="container mx-auto px-4 py-12">
      <h1 class="text-4xl font-bold text-brazil-yellow glow-text mb-6">
        Legend's Forge
      </h1>

      <div class="max-w-5xl mx-auto mt-12">
        <LegendForge />
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import AuthService from "@/services/AuthService";
import LegendForge from "@/components/LegendForge.vue";

export default {
  name: "ForgeMythView",
  components: {
    LegendForge,
  },
  setup() {
    const router = useRouter();
    const currentUser = ref(null);

    onMounted(() => {
      currentUser.value = AuthService.getCurrentUser();
      if (!currentUser.value) {
        router.push("/login");
      }
    });

    return {
      currentUser,
    };
  },
};
</script>
