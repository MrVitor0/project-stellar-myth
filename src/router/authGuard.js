import { auth } from "@/firebase";

/**
 * Guarda de rota para verificar se o usuário está autenticado
 * @param {Object} to - Rota de destino
 * @param {Object} from - Rota de origem
 * @param {Function} next - Função de callback
 */
export default async function authGuard(to, from, next) {
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth);

  // Verifica o status de autenticação do usuário
  const isAuthenticated = auth.currentUser;

  // Se a rota requer autenticação e o usuário não está autenticado,
  // redireciona para a página de login
  if (requiresAuth && !isAuthenticated) {
    next({ path: "/login", query: { redirect: to.fullPath } });
  }
  // Se o usuário já está autenticado e tenta acessar login/registro,
  // redireciona para a página inicial
  else if (
    (to.path === "/login" || to.path === "/register") &&
    isAuthenticated
  ) {
    next({ path: "/forge/myth" });
  }
  // Em qualquer outro caso, permite o acesso à rota
  else {
    next();
  }
}
