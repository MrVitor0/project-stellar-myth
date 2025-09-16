import {
  isConnected,
  isAllowed,
  setAllowed,
  requestAccess,
  signTransaction,
  signBlob,
  getPublicKey,
  getNetwork,
  getNetworkDetails,
} from "@stellar/freighter-api";

/**
 * Service for integrating with Freighter Wallet
 */
class FreighterService {
  constructor() {
    this.isFreighterAvailable = false;
    this.currentPublicKey = null;
    this.isConnected = false;
  }

  /**
   * Verifica se a extensão Freighter está instalada
   */
  async checkFreighterAvailability() {
    try {
      // Verificação mais robusta: tenta usar a API importada
      try {
        // Primeiro tenta verificar se está conectado usando a API importada
        await isConnected();
        this.isFreighterAvailable = true;
        return true;
      } catch (apiError) {
        // Se falhar, verifica se o objeto window.freighter existe
        // (caso a API não seja carregada corretamente)
        if (typeof window !== "undefined" && window.freighter) {
          this.isFreighterAvailable = true;
          return true;
        }

        // Se chegou aqui, o Freighter realmente não está disponível
        console.log("Freighter API não está acessível:", apiError.message);
        this.isFreighterAvailable = false;
        return false;
      }
    } catch (error) {
      console.log("Freighter não está disponível:", error.message);
      this.isFreighterAvailable = false;
      return false;
    }
  }

  /**
   * Verifica se já existe uma conexão ativa com o Freighter
   */
  async checkExistingConnection() {
    if (!this.isFreighterAvailable) {
      await this.checkFreighterAvailability();
    }

    if (!this.isFreighterAvailable) {
      return false;
    }

    try {
      const connected = await isConnected();
      const allowed = await isAllowed();

      if (connected && allowed) {
        const publicKey = await getPublicKey();
        this.currentPublicKey = publicKey;
        this.isConnected = true;

        console.log("✅ Freighter já conectado:", publicKey);
        return {
          publicKey: publicKey,
          network: await this.getCurrentNetwork(),
        };
      }

      return false;
    } catch (error) {
      console.log("Erro ao verificar conexão existente:", error);
      return false;
    }
  }

  /**
   * Solicita acesso/conexão com o Freighter
   */
  async connectWallet() {
    if (!this.isFreighterAvailable) {
      const available = await this.checkFreighterAvailability();
      if (!available) {
        throw new Error(
          "Freighter não está instalado. Instale a extensão Freighter para continuar."
        );
      }
    }

    try {
      // Solicitar acesso se necessário
      const accessGranted = await requestAccess();

      if (accessGranted) {
        await setAllowed();
        const publicKey = await getPublicKey();

        this.currentPublicKey = publicKey;
        this.isConnected = true;

        console.log("✅ Freighter conectado com sucesso:", publicKey);

        return {
          publicKey: publicKey,
          network: await this.getCurrentNetwork(),
        };
      } else {
        throw new Error("Acesso negado pelo usuário");
      }
    } catch (error) {
      console.error("Erro ao conectar Freighter:", error);
      throw error;
    }
  }

  /**
   * Obtém a rede atual do Freighter
   */
  async getCurrentNetwork() {
    try {
      const network = await getNetwork();
      const networkDetails = await getNetworkDetails();

      return {
        network: network,
        details: networkDetails,
      };
    } catch (error) {
      console.log("Erro ao obter rede:", error);
      return { network: "TESTNET", details: null };
    }
  }

  /**
   * Desconecta do Freighter
   */
  disconnect() {
    this.currentPublicKey = null;
    this.isConnected = false;
  }

  /**
   * Assina uma transação usando o Freighter
   */
  async signTransaction(transactionXdr, networkPassphrase) {
    if (!this.isConnected) {
      throw new Error("Freighter não está conectado");
    }

    try {
      const signedTransaction = await signTransaction(transactionXdr, {
        network: networkPassphrase,
        accountToSign: this.currentPublicKey,
      });

      return signedTransaction;
    } catch (error) {
      console.error("Erro ao assinar transação:", error);
      throw error;
    }
  }

  /**
   * Assina um blob usando o Freighter
   */
  async signBlob(blob, accountToSign = null) {
    if (!this.isConnected) {
      throw new Error("Freighter não está conectado");
    }

    try {
      const signedBlob = await signBlob(blob, {
        accountToSign: accountToSign || this.currentPublicKey,
      });

      return signedBlob;
    } catch (error) {
      console.error("Erro ao assinar blob:", error);
      throw error;
    }
  }

  /**
   * Verifica se está usando a rede correta (Testnet para desenvolvimento)
   */
  async verifyNetwork() {
    try {
      const networkInfo = await this.getCurrentNetwork();
      const isTestnet =
        networkInfo.network === "TESTNET" ||
        (networkInfo.details &&
          networkInfo.details.networkPassphrase?.includes("Test"));

      if (!isTestnet) {
        console.warn("⚠️ Freighter não está na rede Testnet");
        return false;
      }

      return true;
    } catch (error) {
      console.log("Erro ao verificar rede:", error);
      return true; // Assume que está correto se não conseguir verificar
    }
  }

  /**
   * Formata a chave pública para exibição
   */
  formatPublicKey(publicKey = null) {
    const key = publicKey || this.currentPublicKey;
    if (!key) return "...";
    return `${key.substring(0, 8)}...${key.substring(key.length - 8)}`;
  }

  /**
   * Copia a chave pública para a área de transferência
   */
  async copyPublicKey() {
    if (!this.currentPublicKey) return false;

    try {
      await navigator.clipboard.writeText(this.currentPublicKey);
      return true;
    } catch (error) {
      console.error("Erro ao copiar chave:", error);
      return false;
    }
  }

  /**
   * Getters para acesso aos dados
   */
  async getPublicKey() {
    // Se já tivermos a chave pública em cache, retorne-a
    if (this.currentPublicKey) {
      return this.currentPublicKey;
    }

    // Caso contrário, tente obter do Freighter
    try {
      if (await this.checkFreighterAvailability()) {
        const publicKey = await getPublicKey();
        this.currentPublicKey = publicKey;
        return publicKey;
      }
    } catch (error) {
      console.error("Erro ao obter chave pública do Freighter:", error);
    }

    return null;
  }

  getIsConnected() {
    return this.isConnected;
  }

  getIsFreighterAvailable() {
    return this.isFreighterAvailable;
  }
}

export default new FreighterService();
