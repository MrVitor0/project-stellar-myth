import { ref, onMounted, onUnmounted } from "vue";
import { auth } from "@/firebase";
import { onAuthStateChanged } from "firebase/auth";

/**
 * Composable para gerenciar o estado de autenticação
 */
export function useAuth() {
  const currentUser = ref(null);
  const isAuthenticated = ref(false);
  const isLoading = ref(true);

  // Função para formatar dados do usuário para armazenar
  const formatUserData = (user) => {
    if (!user) return null;

    return {
      uid: user.uid,
      email: user.email,
      displayName: user.displayName || "Usuário",
      photoURL: user.photoURL,
      emailVerified: user.emailVerified,
      // Adicione mais campos conforme necessário
    };
  };

  // Monitorar mudanças no estado de autenticação
  let unsubscribe;

  onMounted(() => {
    isLoading.value = true;
    unsubscribe = onAuthStateChanged(auth, (user) => {
      currentUser.value = formatUserData(user);
      isAuthenticated.value = !!user;
      isLoading.value = false;
    });
  });

  onUnmounted(() => {
    if (unsubscribe) {
      unsubscribe();
    }
  });

  return {
    currentUser,
    isAuthenticated,
    isLoading,
  };
}

export default useAuth;
