import * as StellarSdk from "@stellar/stellar-sdk";
import SorobanService from "./SorobanService.js";

/**
 * Service for managing user wallet and Stellar keypairs
 */
class WalletService {
  constructor() {
    this.currentKeypair = null;
    this.isConnected = false;
    this.storageKey = "stellar_keypair";
    this.walletType = null; // Adicionar tipo de carteira: 'freighter' ou 'keypair'
  }

  /**
   * Gera um novo par de chaves e o salva no localStorage
   */
  async generateNewWallet() {
    try {
      const keypair = SorobanService.generateKeyPair();

      // Financiar a conta na testnet
      await SorobanService.fundAccount(keypair.publicKey());

      // Salvar no localStorage (em produção, use um método mais seguro)
      this.saveKeypair(keypair);

      this.currentKeypair = keypair;
      this.isConnected = true;

      return {
        publicKey: keypair.publicKey(),
        secret: keypair.secret(),
      };
    } catch (error) {
      console.error("Erro ao gerar carteira:", error);
      throw error;
    }
  }

  /**
   * Conecta usando uma chave secreta existente
   */
  async connectWithSecret(secretKey) {
    try {
      const keypair = StellarSdk.Keypair.fromSecret(secretKey);

      // Verificar se a conta existe, se não, financiar
      try {
        const server = new StellarSdk.Horizon.Server(
          "https://horizon-testnet.stellar.org"
        );
        await server.loadAccount(keypair.publicKey());
      } catch (error) {
        // Se a conta não existir, financiar
        await SorobanService.fundAccount(keypair.publicKey());
      }

      this.saveKeypair(keypair);
      this.currentKeypair = keypair;
      this.isConnected = true;

      return {
        publicKey: keypair.publicKey(),
        secret: keypair.secret(),
      };
    } catch (error) {
      console.error("Erro ao conectar carteira:", error);
      throw error;
    }
  }

  /**
   * Conecta usando o Freighter
   */
  async connectWithFreighter() {
    try {
      const freighterService = await import("./FreighterService").then(
        (module) => module.default
      );

      // Verificar explicitamente se o Freighter está disponível
      const isAvailable = await freighterService.checkFreighterAvailability();
      if (!isAvailable) {
        throw new Error(
          "Freighter não está instalado ou disponível no navegador"
        );
      }

      const result = await freighterService.connectWallet();

      if (result && result.publicKey) {
        // Salvar no localStorage
        localStorage.setItem(
          this.storageKey,
          JSON.stringify({
            publicKey: result.publicKey,
            type: "freighter",
          })
        );

        this.isConnected = true;
        this.walletType = "freighter";

        return {
          publicKey: result.publicKey,
          type: "freighter",
        };
      } else {
        throw new Error("Falha ao conectar com Freighter");
      }
    } catch (error) {
      console.error("Erro ao conectar com Freighter:", error);
      throw error;
    }
  }

  /**
   * Carrega o par de chaves ou conexão Freighter do localStorage
   */
  loadSavedWallet() {
    try {
      const saved = localStorage.getItem(this.storageKey);
      if (saved) {
        const data = JSON.parse(saved);

        // Verificar se é uma conexão Freighter
        if (data.type === "freighter") {
          // Verificar se o Freighter ainda está conectado
          return this.loadFreighterWallet();
        }

        // Se não, assumir que é um keypair normal
        if (data.secret) {
          const keypair = StellarSdk.Keypair.fromSecret(data.secret);
          this.currentKeypair = keypair;
          this.isConnected = true;
          this.walletType = "keypair";
          return {
            publicKey: keypair.publicKey(),
            secret: keypair.secret(),
            type: "keypair",
          };
        }
      }
      return null;
    } catch (error) {
      console.error("Erro ao carregar carteira salva:", error);
      return null;
    }
  }

  /**
   * Carrega uma carteira Freighter salva
   */
  async loadFreighterWallet() {
    try {
      const freighterService = await import("./FreighterService").then(
        (module) => module.default
      );
      const connection = await freighterService.checkExistingConnection();

      if (connection && connection.publicKey) {
        this.isConnected = true;
        this.walletType = "freighter";
        return {
          publicKey: connection.publicKey,
          type: "freighter",
        };
      }

      return null;
    } catch (error) {
      console.error("Erro ao carregar carteira Freighter:", error);
      return null;
    }
  }

  /**
   * Salva o par de chaves no localStorage
   */
  saveKeypair(keypair) {
    try {
      const walletData = {
        publicKey: keypair.publicKey(),
        secret: keypair.secret(),
        type: "keypair",
      };
      localStorage.setItem(this.storageKey, JSON.stringify(walletData));
      this.walletType = "keypair";
    } catch (error) {
      console.error("Erro ao salvar carteira:", error);
    }
  }

  /**
   * Remove a carteira do localStorage
   */
  async disconnect() {
    try {
      if (this.walletType === "freighter") {
        const freighterService = await import("./FreighterService").then(
          (module) => module.default
        );
        freighterService.disconnect();
      }

      localStorage.removeItem(this.storageKey);
      this.currentKeypair = null;
      this.isConnected = false;
      this.walletType = null;
    } catch (error) {
      console.error("Erro ao desconectar carteira:", error);
    }
  }

  /**
   * Retorna o par de chaves atual
   */
  getCurrentKeypair() {
    return this.currentKeypair;
  }

  /**
   * Retorna a chave pública atual
   */
  async getCurrentPublicKey() {
    if (this.walletType === "freighter") {
      try {
        const freighterService = await import("./FreighterService").then(
          (module) => module.default
        );
        return freighterService.getPublicKey();
      } catch (error) {
        console.error("Erro ao obter chave pública do Freighter:", error);
        return null;
      }
    }
    return this.currentKeypair ? this.currentKeypair.publicKey() : null;
  }

  /**
   * Retorna o tipo de carteira atual
   */
  getWalletType() {
    return this.walletType;
  }

  /**
   * Verifica se a carteira está conectada
   */
  getIsConnected() {
    return this.isConnected;
  }

  /**
   * Obtém o saldo da conta (XLM)
   */
  async getAccountBalance() {
    if (!this.currentKeypair) {
      throw new Error("Nenhuma carteira conectada");
    }

    try {
      const server = new StellarSdk.Horizon.Server(
        "https://horizon-testnet.stellar.org"
      );
      const account = await server.loadAccount(this.currentKeypair.publicKey());

      // Encontrar o saldo XLM nativo
      const nativeBalance = account.balances.find(
        (balance) => balance.asset_type === "native"
      );
      return nativeBalance ? parseFloat(nativeBalance.balance) : 0;
    } catch (error) {
      console.error("Erro ao obter saldo:", error);
      return 0;
    }
  }

  /**
   * Formata a chave pública para exibição (mostra apenas os primeiros e últimos caracteres)
   */
  formatPublicKey(publicKey = null) {
    // Se uma chave pública for fornecida diretamente, usá-la
    if (publicKey) {
      return `${publicKey.substring(0, 8)}...${publicKey.substring(
        publicKey.length - 8
      )}`;
    }

    // Se tivermos uma keypair disponível (carteira local)
    if (this.currentKeypair) {
      const key = this.currentKeypair.publicKey();
      return `${key.substring(0, 8)}...${key.substring(key.length - 8)}`;
    }

    // Se não temos chave pública disponível
    return "...";
  }

  /**
   * Copia a chave pública para o clipboard
   */
  async copyPublicKey() {
    try {
      const publicKey = await this.getCurrentPublicKey();
      if (publicKey) {
        try {
          await navigator.clipboard.writeText(publicKey);
          return true;
        } catch (error) {
          console.error("Erro ao copiar chave pública:", error);
          return false;
        }
      }

      // Se estamos usando Freighter mas não conseguimos obter a chave
      if (this.walletType === "freighter") {
        const freighterService = await import("./FreighterService").then(
          (module) => module.default
        );
        const freighterKey = freighterService.currentPublicKey;
        if (freighterKey) {
          await navigator.clipboard.writeText(freighterKey);
          return true;
        }
      }

      return false;
    } catch (error) {
      console.error("Erro ao copiar chave pública:", error);
      return false;
    }
  }
}

export default new WalletService();
