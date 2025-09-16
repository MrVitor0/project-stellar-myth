<template>
  <div class="relative w-full max-w-md">
    <div class="gaming-card neon-glow overflow-hidden">
      <!-- Login/Register navigation tabs -->
      <div class="flex mb-6 border-b border-brazil-green/30">
        <button
          @click="activeTab = 'login'"
          class="flex-1 py-3 px-4 text-center font-medium transition-colors duration-300"
          :class="
            activeTab === 'login'
              ? 'text-brazil-yellow border-b-2 border-brazil-yellow'
              : 'text-mid-gray hover:text-mist-white'
          "
        >
          Login
        </button>
        <button
          @click="activeTab = 'register'"
          class="flex-1 py-3 px-4 text-center font-medium transition-colors duration-300"
          :class="
            activeTab === 'register'
              ? 'text-brazil-yellow border-b-2 border-brazil-yellow'
              : 'text-mid-gray hover:text-mist-white'
          "
        >
          Register
        </button>
      </div>

      <!-- Login Form -->
      <div v-if="activeTab === 'login'" class="space-y-4">
        <h3 class="text-xl font-bold text-brazil-yellow glow-text mb-6">
          Welcome back!
        </h3>

        <!-- Mensagens de erro/sucesso -->
        <div
          v-if="errorMessage"
          class="p-3 bg-red-500/20 border border-red-500/30 rounded-lg text-sm text-white mb-2"
        >
          {{ errorMessage }}
        </div>
        <div
          v-if="successMessage"
          class="p-3 bg-brazil-green/20 border border-brazil-green/30 rounded-lg text-sm text-white mb-2"
        >
          {{ successMessage }}
        </div>

        <div class="space-y-2">
          <label
            for="login-email"
            class="block text-mist-white text-sm font-medium"
            >Email</label
          >
          <input
            id="login-email"
            v-model="loginForm.email"
            type="email"
            class="w-full bg-twilight-blue/50 border border-brazil-green/30 rounded-lg py-2 px-3 text-mist-white focus:outline-none focus:ring-2 focus:ring-brazil-green/50"
            placeholder="your@email.com"
          />
        </div>

        <div class="space-y-2">
          <label
            for="login-password"
            class="block text-mist-white text-sm font-medium"
            >Password</label
          >
          <input
            id="login-password"
            v-model="loginForm.password"
            type="password"
            class="w-full bg-twilight-blue/50 border border-brazil-green/30 rounded-lg py-2 px-3 text-mist-white focus:outline-none focus:ring-2 focus:ring-brazil-green/50"
            placeholder="********"
            @keyup.enter="handleLogin"
          />
        </div>

        <div class="flex justify-between items-center text-sm">
          <div class="flex items-center">
            <input
              id="remember-me"
              type="checkbox"
              v-model="loginForm.rememberMe"
              class="h-4 w-4 bg-twilight-blue border border-brazil-green/30 rounded focus:ring-brazil-green"
            />
            <label for="remember-me" class="ml-2 text-mist-white"
              >Remember me</label
            >
          </div>
          <a
            href="#"
            @click.prevent="handleResetPassword"
            class="text-brazil-green hover:text-brazil-yellow transition-colors"
            >Forgot password?</a
          >
        </div>

        <div class="pt-4">
          <BaseButton
            variant="primary"
            size="lg"
            class="w-full"
            :loading="isLoading"
            @click="handleLogin"
          >
            Sign In
          </BaseButton>

          <div class="mt-4 text-center">
            <p class="text-mid-gray">Or sign in with</p>
            <div class="flex justify-center space-x-4 mt-3">
              <button
                @click="handleGoogleLogin"
                class="p-2 rounded-full bg-twilight-blue hover:bg-brazil-green/20 border border-brazil-green/30 transition-colors"
              >
                <svg
                  class="w-5 h-5 text-mist-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M21.35 11.1h-9.17v2.73h6.51c-.33 3.81-3.5 5.44-6.5 5.44C8.36 19.27 5 16.25 5 12c0-4.1 3.2-7.27 7.2-7.27 3.09 0 4.9 1.97 4.9 1.97L19 4.72S16.56 2 12.1 2C6.42 2 2.03 6.8 2.03 12c0 5.05 4.13 10 10.22 10 5.35 0 9.25-3.67 9.25-9.09 0-1.15-.15-1.81-.15-1.81z"
                  />
                </svg>
              </button>
              <button
                @click="handleFacebookLogin"
                class="p-2 rounded-full bg-twilight-blue hover:bg-brazil-green/20 border border-brazil-green/30 transition-colors"
              >
                <svg
                  class="w-5 h-5 text-mist-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385h-3.047v-3.47h3.047v-2.642c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953h-1.514c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385c5.736-.9 10.126-5.864 10.126-11.854z"
                  />
                </svg>
              </button>
              <button
                class="p-2 rounded-full bg-twilight-blue hover:bg-brazil-green/20 border border-brazil-green/30 transition-colors"
              >
                <svg
                  class="w-5 h-5 text-mist-white"
                  fill="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"
                  />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Registration Form -->
      <div v-if="activeTab === 'register'" class="space-y-4">
        <h3 class="text-xl font-bold text-brazil-yellow glow-text mb-6">
          Join us!
        </h3>

        <!-- Mensagens de erro/sucesso -->
        <div
          v-if="errorMessage"
          class="p-3 bg-red-500/20 border border-red-500/30 rounded-lg text-sm text-white mb-2"
        >
          {{ errorMessage }}
        </div>
        <div
          v-if="successMessage"
          class="p-3 bg-brazil-green/20 border border-brazil-green/30 rounded-lg text-sm text-white mb-2"
        >
          {{ successMessage }}
        </div>

        <div class="space-y-2">
          <label
            for="register-name"
            class="block text-mist-white text-sm font-medium"
            >Full name</label
          >
          <input
            id="register-name"
            v-model="registerForm.name"
            type="text"
            class="w-full bg-twilight-blue/50 border border-brazil-green/30 rounded-lg py-2 px-3 text-mist-white focus:outline-none focus:ring-2 focus:ring-brazil-green/50"
            placeholder="Your name"
          />
        </div>

        <div class="space-y-2">
          <label
            for="register-email"
            class="block text-mist-white text-sm font-medium"
            >Email</label
          >
          <input
            id="register-email"
            v-model="registerForm.email"
            type="email"
            class="w-full bg-twilight-blue/50 border border-brazil-green/30 rounded-lg py-2 px-3 text-mist-white focus:outline-none focus:ring-2 focus:ring-brazil-green/50"
            placeholder="your@email.com"
          />
        </div>

        <div class="space-y-2">
          <label
            for="register-password"
            class="block text-mist-white text-sm font-medium"
            >Password</label
          >
          <input
            id="register-password"
            v-model="registerForm.password"
            type="password"
            class="w-full bg-twilight-blue/50 border border-brazil-green/30 rounded-lg py-2 px-3 text-mist-white focus:outline-none focus:ring-2 focus:ring-brazil-green/50"
            placeholder="********"
          />
        </div>

        <div class="space-y-2">
          <label
            for="register-confirm-password"
            class="block text-mist-white text-sm font-medium"
            >Confirm password</label
          >
          <input
            id="register-confirm-password"
            v-model="registerForm.confirmPassword"
            type="password"
            class="w-full bg-twilight-blue/50 border border-brazil-green/30 rounded-lg py-2 px-3 text-mist-white focus:outline-none focus:ring-2 focus:ring-brazil-green/50"
            placeholder="********"
          />
        </div>

        <div class="flex items-start mt-4">
          <div class="flex items-center h-5">
            <input
              id="terms"
              v-model="registerForm.acceptTerms"
              type="checkbox"
              class="h-4 w-4 bg-twilight-blue border border-brazil-green/30 rounded focus:ring-brazil-green"
            />
          </div>
          <div class="ml-3 text-sm">
            <label for="terms" class="text-mist-white">
              I accept the
              <a href="#" class="text-brazil-green hover:text-brazil-yellow"
                >Terms of Service</a
              >
              and
              <a href="#" class="text-brazil-green hover:text-brazil-yellow"
                >Privacy Policy</a
              >
            </label>
          </div>
        </div>

        <div class="pt-4">
          <BaseButton
            variant="primary"
            size="lg"
            class="w-full"
            :loading="isLoading"
            @click="handleRegister"
          >
            Register
          </BaseButton>
        </div>
      </div>

      <!-- Padrão de blockchain com partículas no background -->
      <div class="absolute inset-0 -z-10 overflow-hidden">
        <div
          class="absolute w-20 h-20 bg-brazil-green/10 rounded-full -top-10 -left-10 animate-pulse"
        ></div>
        <div
          class="absolute w-16 h-16 bg-brazil-yellow/5 rounded-full top-1/4 -right-8 animate-pulse"
          style="animation-delay: 1s"
        ></div>
        <div
          class="absolute w-24 h-24 bg-mystic-cyan/5 rounded-full -bottom-12 left-1/3 animate-pulse"
          style="animation-delay: 2s"
        ></div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import BaseButton from "./BaseButton.vue";
import AuthService from "@/services/AuthService";
import { useRouter } from "vue-router";

export default {
  name: "AuthForm",
  components: {
    BaseButton,
  },
  emits: ["login-success", "register-success"],
  setup(props, { emit }) {
    const router = useRouter();
    const activeTab = ref("login");
    const isLoading = ref(false);
    const errorMessage = ref("");
    const successMessage = ref("");

    const loginForm = ref({
      email: "",
      password: "",
      rememberMe: false,
    });

    const registerForm = ref({
      name: "",
      email: "",
      password: "",
      confirmPassword: "",
      acceptTerms: false,
    });

    // Função para lidar com o login
    const handleLogin = async () => {
      errorMessage.value = "";
      successMessage.value = "";

      if (!loginForm.value.email || !loginForm.value.password) {
        errorMessage.value = "Please fill in all fields";
        return;
      }

      isLoading.value = true;

      try {
        const result = await AuthService.login(
          loginForm.value.email,
          loginForm.value.password
        );

        if (result.success) {
          successMessage.value = "Login successful!";
          emit("login-success", result.user);

          // Redirecionar após login bem-sucedido
          router.push("/forge/myth");
        } else {
          errorMessage.value = result.error || "Login failed";
        }
      } catch (error) {
        console.error("Login error:", error);
        errorMessage.value = "An unexpected error occurred";
      } finally {
        isLoading.value = false;
      }
    };

    // Função para lidar com o login via Google
    const handleGoogleLogin = async () => {
      errorMessage.value = "";
      successMessage.value = "";
      isLoading.value = true;

      try {
        const result = await AuthService.loginWithGoogle();

        if (result.success) {
          successMessage.value = "Login successful!";
          emit("login-success", result.user);

          // Redirecionar após login bem-sucedido
          router.push("/forge/myth");
        } else {
          errorMessage.value = result.error || "Google login failed";
        }
      } catch (error) {
        console.error("Google login error:", error);
        errorMessage.value = "An unexpected error occurred";
      } finally {
        isLoading.value = false;
      }
    };

    // Função para lidar com o login via Facebook
    const handleFacebookLogin = async () => {
      errorMessage.value = "";
      successMessage.value = "";
      isLoading.value = true;

      try {
        const result = await AuthService.loginWithFacebook();

        if (result.success) {
          successMessage.value = "Login successful!";
          emit("login-success", result.user);

          // Redirecionar após login bem-sucedido
          router.push("/forge/myth");
        } else {
          errorMessage.value = result.error || "Facebook login failed";
        }
      } catch (error) {
        console.error("Facebook login error:", error);
        errorMessage.value = "An unexpected error occurred";
      } finally {
        isLoading.value = false;
      }
    };

    // Função para lidar com o registro
    const handleRegister = async () => {
      errorMessage.value = "";
      successMessage.value = "";

      if (!registerForm.value.acceptTerms) {
        errorMessage.value = "You must accept the terms to register";
        return;
      }

      if (registerForm.value.password !== registerForm.value.confirmPassword) {
        errorMessage.value = "Passwords do not match";
        return;
      }

      if (
        !registerForm.value.name ||
        !registerForm.value.email ||
        !registerForm.value.password
      ) {
        errorMessage.value = "Please fill in all required fields";
        return;
      }

      isLoading.value = true;

      try {
        const result = await AuthService.register({
          name: registerForm.value.name,
          email: registerForm.value.email,
          password: registerForm.value.password,
        });

        if (result.success) {
          successMessage.value = "Registration successful!";
          emit("register-success", result.user);

          // Redirecionar após registro bem-sucedido
          router.push("/forge/myth");
        } else {
          errorMessage.value = result.error || "Registration failed";
        }
      } catch (error) {
        console.error("Registration error:", error);
        errorMessage.value = "An unexpected error occurred";
      } finally {
        isLoading.value = false;
      }
    };

    // Função para lidar com o reset de senha
    const handleResetPassword = async () => {
      if (!loginForm.value.email) {
        errorMessage.value = "Please enter your email address";
        return;
      }

      isLoading.value = true;

      try {
        const result = await AuthService.resetPassword(loginForm.value.email);

        if (result.success) {
          successMessage.value = result.message;
        } else {
          errorMessage.value = result.error || "Failed to send reset email";
        }
      } catch (error) {
        console.error("Reset password error:", error);
        errorMessage.value = "An unexpected error occurred";
      } finally {
        isLoading.value = false;
      }
    };

    return {
      activeTab,
      isLoading,
      errorMessage,
      successMessage,
      loginForm,
      registerForm,
      handleLogin,
      handleRegister,
      handleGoogleLogin,
      handleFacebookLogin,
      handleResetPassword,
    };
  },
};
</script>

<style scoped>
/* Animação de pulsação */
@keyframes pulse {
  0% {
    opacity: 0.4;
    transform: scale(1);
  }
  50% {
    opacity: 0.7;
    transform: scale(1.05);
  }
  100% {
    opacity: 0.4;
    transform: scale(1);
  }
}

.animate-pulse {
  animation: pulse 4s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}
</style>
