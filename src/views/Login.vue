<template>
  <div class="min-h-screen blockchain-pattern">
    <Navbar />
    <div class="flex items-center justify-center min-h-[80vh] px-4">
      <div class="w-full max-w-5xl grid grid-cols-1 md:grid-cols-2 gap-12">
        <!-- Left column: Text and description -->
        <div class="flex flex-col justify-center">
          <h1
            class="text-4xl md:text-5xl lg:text-6xl font-bold text-brazil-yellow glow-text mb-6"
          >
            Join<br />
            <span class="text-mist-white">Stellar Myth</span>
          </h1>

          <p class="text-mid-gray text-lg mb-8">
            Connect your account to join a new era of collaborative gaming.
            Forge legends, build your on-chain reputation, and impact a universe
            powered by its community.
          </p>

          <div class="flex flex-wrap gap-4 mb-8">
            <div class="flex items-center">
              <div
                class="w-10 h-10 rounded-full bg-brazil-green/20 flex items-center justify-center mr-3"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5 text-brazil-green"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fill-rule="evenodd"
                    d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                    clip-rule="evenodd"
                  />
                </svg>
              </div>
              <span class="text-mist-white"
                >Access the Legend's Forge to create new in-game powers</span
              >
            </div>

            <div class="flex items-center">
              <div
                class="w-10 h-10 rounded-full bg-brazil-green/20 flex items-center justify-center mr-3"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5 text-brazil-green"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fill-rule="evenodd"
                    d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                    clip-rule="evenodd"
                  />
                </svg>
              </div>
              <span class="text-mist-white"
                >Build your On-Chain Legacy with permanent, verifiable
                achievements.</span
              >
            </div>

            <div class="flex items-center">
              <div
                class="w-10 h-10 rounded-full bg-brazil-green/20 flex items-center justify-center mr-3"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5 text-brazil-green"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fill-rule="evenodd"
                    d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                    clip-rule="evenodd"
                  />
                </svg>
              </div>
              <span class="text-mist-white"
                >Impact the game for every player with your unique
                creations.</span
              >
            </div>
          </div>
        </div>

        <!-- Right column: Login form -->
        <div class="flex justify-center items-center">
          <AuthForm
            @login-success="handleAuthSuccess"
            @register-success="handleAuthSuccess"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import AuthForm from "@/components/AuthForm.vue";
import Navbar from "@/components/Navbar.vue";
export default {
  name: "LoginView",
  components: {
    AuthForm,
    Navbar,
  },
  methods: {
    handleAuthSuccess() {
      // After successful login or registration, redirect to forge/myth page
      this.$router.push("/forge/myth");
    },
  },
};
</script>
