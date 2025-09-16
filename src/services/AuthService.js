import {
  createUserWithEmailAndPassword,
  signInWithEmailAndPassword,
  signOut,
  updateProfile,
  GoogleAuthProvider,
  signInWithPopup,
  FacebookAuthProvider,
  sendPasswordResetEmail,
} from "firebase/auth";
import { auth } from "@/firebase";

/**
 * Serviço de autenticação para gerenciar login, registro e perfil de usuário
 */
export const AuthService = {
  /**
   * Cadastrar um novo usuário com email e senha
   * @param {Object} userData - Dados do usuário
   * @param {string} userData.email - Email do usuário
   * @param {string} userData.password - Senha do usuário
   * @param {string} userData.name - Nome do usuário
   * @returns {Promise<Object>} - Usuário criado
   */
  async register({ email, password, name }) {
    try {
      const userCredential = await createUserWithEmailAndPassword(
        auth,
        email,
        password
      );

      // Atualiza o perfil do usuário com o nome fornecido
      await updateProfile(userCredential.user, {
        displayName: name,
      });

      return {
        user: userCredential.user,
        success: true,
      };
    } catch (error) {
      console.error("Erro ao registrar usuário:", error);
      return {
        error: this._handleAuthError(error),
        success: false,
      };
    }
  },

  /**
   * Fazer login com email e senha
   * @param {string} email - Email do usuário
   * @param {string} password - Senha do usuário
   * @returns {Promise<Object>} - Resultado do login
   */
  async login(email, password) {
    try {
      const userCredential = await signInWithEmailAndPassword(
        auth,
        email,
        password
      );
      return {
        user: userCredential.user,
        success: true,
      };
    } catch (error) {
      console.error("Erro ao fazer login:", error);
      return {
        error: this._handleAuthError(error),
        success: false,
      };
    }
  },

  /**
   * Fazer login com Google
   * @returns {Promise<Object>} - Resultado do login
   */
  async loginWithGoogle() {
    try {
      const provider = new GoogleAuthProvider();
      const userCredential = await signInWithPopup(auth, provider);
      return {
        user: userCredential.user,
        success: true,
      };
    } catch (error) {
      console.error("Erro ao fazer login com Google:", error);
      return {
        error: this._handleAuthError(error),
        success: false,
      };
    }
  },

  /**
   * Fazer login com Facebook
   * @returns {Promise<Object>} - Resultado do login
   */
  async loginWithFacebook() {
    try {
      const provider = new FacebookAuthProvider();
      const userCredential = await signInWithPopup(auth, provider);
      return {
        user: userCredential.user,
        success: true,
      };
    } catch (error) {
      console.error("Erro ao fazer login com Facebook:", error);
      return {
        error: this._handleAuthError(error),
        success: false,
      };
    }
  },

  /**
   * Fazer logout
   * @returns {Promise<boolean>} - Sucesso do logout
   */
  async logout() {
    try {
      await signOut(auth);
      return true;
    } catch (error) {
      console.error("Erro ao fazer logout:", error);
      return false;
    }
  },

  /**
   * Enviar email de redefinição de senha
   * @param {string} email - Email do usuário
   * @returns {Promise<Object>} - Resultado da operação
   */
  async resetPassword(email) {
    try {
      await sendPasswordResetEmail(auth, email);
      return {
        success: true,
        message: "Email de redefinição de senha enviado com sucesso!",
      };
    } catch (error) {
      console.error("Erro ao enviar email de redefinição:", error);
      return {
        success: false,
        error: this._handleAuthError(error),
      };
    }
  },

  /**
   * Pegar o usuário atual
   * @returns {Object|null} - Usuário atual ou null
   */
  getCurrentUser() {
    return auth.currentUser;
  },

  /**
   * Traduz e trata os erros de autenticação do Firebase
   * @param {Error} error - Erro do Firebase
   * @returns {string} - Mensagem de erro traduzida
   * @private
   */
  _handleAuthError(error) {
    const errorCode = error.code;
    switch (errorCode) {
      case "auth/email-already-in-use":
        return "Este email já está sendo usado por outra conta.";
      case "auth/invalid-email":
        return "Email inválido.";
      case "auth/weak-password":
        return "Senha fraca. Use pelo menos 6 caracteres.";
      case "auth/user-not-found":
        return "Usuário não encontrado. Verifique seu email.";
      case "auth/wrong-password":
        return "Senha incorreta.";
      case "auth/too-many-requests":
        return "Acesso temporariamente desativado devido a muitas tentativas falhas. Tente novamente mais tarde.";
      case "auth/user-disabled":
        return "Esta conta foi desativada.";
      case "auth/account-exists-with-different-credential":
        return "Já existe uma conta com este email usando outro método de login.";
      case "auth/popup-closed-by-user":
        return "Operação cancelada pelo usuário.";
      default:
        return "Ocorreu um erro durante a autenticação. Por favor, tente novamente.";
    }
  },
};

export default AuthService;
