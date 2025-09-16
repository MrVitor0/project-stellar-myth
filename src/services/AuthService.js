import {
  createUserWithEmailAndPassword,
  signInWithEmailAndPassword,
  signOut,
  updateProfile,
  GoogleAuthProvider,
  signInWithPopup,
  FacebookAuthProvider,
  sendPasswordResetEmail,
  onAuthStateChanged as firebaseAuthStateChanged,
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
   * Escutar mudanças no estado de autenticação
   * @param {Function} callback - Função de callback a ser executada quando o estado mudar
   * @returns {Function} - Função para parar de escutar
   */
  onAuthStateChanged(callback) {
    return firebaseAuthStateChanged(auth, callback);
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
        return "This email is already being used by another account.";
      case "auth/invalid-email":
        return "Invalid email.";
      case "auth/weak-password":
        return "Weak password. Use at least 6 characters.";
      case "auth/user-not-found":
        return "User not found. Check your email.";
      case "auth/wrong-password":
        return "Incorrect password.";
      case "auth/too-many-requests":
        return "Access temporarily disabled due to too many failed attempts. Try again later.";
      case "auth/user-disabled":
        return "This account has been disabled.";
      case "auth/account-exists-with-different-credential":
        return "An account already exists with this email using another login method.";
      case "auth/popup-closed-by-user":
        return "Operation canceled by user.";
      default:
        return "An error occurred during authentication. Please try again.";
    }
  },
};

export default AuthService;
