import * as StellarSdk from "@stellar/stellar-sdk";

/**
 * Service for interacting with Soroban smart contracts on Stellar
 */
class SorobanService {
  constructor() {
    // Configuração da rede Testnet
    this.server = new StellarSdk.Horizon.Server(
      "https://horizon-testnet.stellar.org"
    );
    this.sorobanServer = new StellarSdk.Horizon.Server(
      "https://soroban-testnet.stellar.org:443"
    );
    this.networkPassphrase = StellarSdk.Networks.TESTNET;
    this.contractAddress =
      "CAKHS4ZPT7YGOXDKXEXSH6G7VQPA5K3MUICQDBXZRAJSLOKRT67KY3S4";
  }

  /**
   * Cria um par de chaves Stellar para o usuário (para demonstração)
   * Em produção, você deve integrar com uma carteira como Freighter
   */
  generateKeyPair() {
    return StellarSdk.Keypair.random();
  }

  /**
   * Financia uma conta na Testnet usando o Friendbot
   */
  async fundAccount(publicKey) {
    try {
      const response = await fetch(
        `https://friendbot.stellar.org?addr=${publicKey}`
      );
      if (!response.ok) {
        throw new Error("Falha ao financiar a conta");
      }
      return await response.json();
    } catch (error) {
      console.error("Erro ao financiar conta:", error);
      throw error;
    }
  }

  /**
   * Converte um objeto JavaScript para os tipos Soroban
   */
  convertToSorobanTypes(option) {
    // Converte o objeto Option para os tipos nativos do Soroban
    const map = new Map();

    map.set(
      "optionName",
      StellarSdk.nativeToScVal(option.optionName, { type: "string" })
    );
    map.set(
      "description",
      StellarSdk.nativeToScVal(option.description, { type: "string" })
    );
    map.set(
      "stellarTransactionId",
      StellarSdk.nativeToScVal(option.stellarTransactionId, { type: "string" })
    );
    map.set(
      "title",
      StellarSdk.nativeToScVal(option.title, { type: "string" })
    );
    map.set("buff", StellarSdk.nativeToScVal(option.buff, { type: "string" }));
    map.set("icon", StellarSdk.nativeToScVal(option.icon, { type: "string" }));
    map.set(
      "optionType",
      StellarSdk.nativeToScVal(option.optionType, { type: "string" })
    );
    map.set("value", StellarSdk.nativeToScVal(option.value, { type: "u64" }));
    map.set(
      "rarity",
      StellarSdk.nativeToScVal(option.rarity, { type: "string" })
    );
    map.set("cost", StellarSdk.nativeToScVal(option.cost, { type: "u64" }));
    map.set(
      "owner",
      StellarSdk.nativeToScVal(option.owner, { type: "address" })
    );

    return StellarSdk.nativeToScVal(map, { type: "map" });
  }

  /**
   * Cria uma nova opção no contrato
   */
  async createOption(userKeypair, optionData) {
    try {
      // Obter informações da conta
      const account = await this.server.loadAccount(userKeypair.publicKey());

      // Preparar os argumentos para a função
      const userAddress = new StellarSdk.Address(userKeypair.publicKey());
      const optionScVal = this.convertToSorobanTypes({
        ...optionData,
        owner: userKeypair.publicKey(),
      });

      // Criar o contrato
      const contract = new StellarSdk.Contract(this.contractAddress);

      // Preparar a operação
      const operation = contract.call(
        "create_option",
        StellarSdk.nativeToScVal(userAddress, { type: "address" }),
        optionScVal
      );

      // Construir a transação
      const transaction = new StellarSdk.TransactionBuilder(account, {
        fee: StellarSdk.BASE_FEE,
        networkPassphrase: this.networkPassphrase,
      })
        .addOperation(operation)
        .setTimeout(300) // 5 minutos
        .build();

      // Simular a transação primeiro
      const simulationResponse = await this.sorobanServer.simulateTransaction(
        transaction
      );

      if (StellarSdk.SorobanRpc.Api.isSimulationError(simulationResponse)) {
        throw new Error(`Erro na simulação: ${simulationResponse.error}`);
      }

      // Preparar a transação com os recursos necessários
      const preparedTransaction = StellarSdk.SorobanRpc.assembleTransaction(
        transaction,
        simulationResponse
      );

      // Assinar a transação
      preparedTransaction.sign(userKeypair);

      // Enviar a transação
      const sendResponse = await this.sorobanServer.sendTransaction(
        preparedTransaction
      );

      if (sendResponse.status === "PENDING") {
        // Aguardar a confirmação
        let getResponse = await this.sorobanServer.getTransaction(
          sendResponse.hash
        );

        // Aguardar até a transação ser processada
        while (getResponse.status === "NOT_FOUND") {
          await new Promise((resolve) => setTimeout(resolve, 1000));
          getResponse = await this.sorobanServer.getTransaction(
            sendResponse.hash
          );
        }

        if (getResponse.status === "SUCCESS") {
          return {
            success: true,
            hash: sendResponse.hash,
            result: getResponse.returnValue,
          };
        } else {
          throw new Error(`Transação falhou: ${getResponse.resultXdr}`);
        }
      } else {
        throw new Error(`Falha ao enviar transação: ${sendResponse.errorXdr}`);
      }
    } catch (error) {
      console.error("Erro ao criar opção:", error);
      throw error;
    }
  }

  /**
   * Cria uma nova opção no contrato usando Freighter para assinar a transação
   */
  async createOptionWithFreighter(optionData) {
    try {
      // Importar FreighterService
      const FreighterService = await import("./FreighterService").then(
        (module) => module.default
      );

      // Verificar se o Freighter está conectado
      if (!FreighterService.getIsConnected()) {
        throw new Error("Freighter não está conectado");
      }

      // Obter a chave pública do usuário
      const userPublicKey = await FreighterService.getPublicKey();

      // Verificar a rede atual
      await FreighterService.verifyNetwork();

      // Obter informações da conta
      const account = await this.server.loadAccount(userPublicKey);

      // Preparar os argumentos para a função
      const userAddress = new StellarSdk.Address(userPublicKey);
      const optionScVal = this.convertToSorobanTypes({
        ...optionData,
        owner: userPublicKey,
      });

      // Criar o contrato
      const contract = new StellarSdk.Contract(this.contractAddress);

      // Preparar a operação
      const operation = contract.call(
        "create_option",
        StellarSdk.nativeToScVal(userAddress, { type: "address" }),
        optionScVal
      );

      // Construir a transação
      const transaction = new StellarSdk.TransactionBuilder(account, {
        fee: StellarSdk.BASE_FEE,
        networkPassphrase: this.networkPassphrase,
      })
        .addOperation(operation)
        .setTimeout(300) // 5 minutos
        .build();

      // Simular a transação primeiro
      const simulationResponse = await this.sorobanServer.simulateTransaction(
        transaction
      );

      if (StellarSdk.SorobanRpc.Api.isSimulationError(simulationResponse)) {
        throw new Error(`Erro na simulação: ${simulationResponse.error}`);
      }

      // Preparar a transação com os recursos necessários
      const preparedTransaction = StellarSdk.SorobanRpc.assembleTransaction(
        transaction,
        simulationResponse
      );

      // Converter para XDR para assinar com o Freighter
      const xdrTransaction = preparedTransaction.toXDR();

      // Assinar a transação com o Freighter
      const signedXDR = await FreighterService.signTransaction(
        xdrTransaction,
        this.networkPassphrase
      );

      // Converter de volta para TransactionBuilder
      const signedTransaction = StellarSdk.TransactionBuilder.fromXDR(
        signedXDR,
        this.networkPassphrase
      );

      // Enviar a transação
      const sendResponse = await this.sorobanServer.sendTransaction(
        signedTransaction
      );

      if (sendResponse.status === "PENDING") {
        // Aguardar a confirmação
        let getResponse = await this.sorobanServer.getTransaction(
          sendResponse.hash
        );

        // Aguardar até a transação ser processada
        while (getResponse.status === "NOT_FOUND") {
          await new Promise((resolve) => setTimeout(resolve, 1000));
          getResponse = await this.sorobanServer.getTransaction(
            sendResponse.hash
          );
        }

        if (getResponse.status === "SUCCESS") {
          return {
            success: true,
            hash: sendResponse.hash,
            result: getResponse.returnValue,
          };
        } else {
          throw new Error(`Transação falhou: ${getResponse.resultXdr}`);
        }
      } else {
        throw new Error(`Falha ao enviar transação: ${sendResponse.errorXdr}`);
      }
    } catch (error) {
      console.error("Erro ao criar opção com Freighter:", error);
      throw error;
    }
  }

  /**
   * Busca as últimas N opções do contrato
   */
  async getLastNOptions(n = 10) {
    try {
      // Criar uma conta temporária apenas para leitura
      const tempKeypair = StellarSdk.Keypair.random();
      let account;

      try {
        account = await this.server.loadAccount(tempKeypair.publicKey());
      } catch {
        // Se a conta não existir, criar uma conta "fantasma" para leitura
        account = new StellarSdk.Account(tempKeypair.publicKey(), "0");
      }

      // Criar o contrato
      const contract = new StellarSdk.Contract(this.contractAddress);

      // Preparar a operação de leitura
      const operation = contract.call(
        "get_last_n_options",
        StellarSdk.nativeToScVal(n, { type: "u32" })
      );

      // Construir a transação
      const transaction = new StellarSdk.TransactionBuilder(account, {
        fee: StellarSdk.BASE_FEE,
        networkPassphrase: this.networkPassphrase,
      })
        .addOperation(operation)
        .setTimeout(300)
        .build();

      // Simular a transação para obter o resultado
      const simulationResponse = await this.sorobanServer.simulateTransaction(
        transaction
      );

      if (StellarSdk.SorobanRpc.Api.isSimulationError(simulationResponse)) {
        throw new Error(`Erro na simulação: ${simulationResponse.error}`);
      }

      // Converter o resultado de volta para JavaScript
      if (simulationResponse.result?.retval) {
        const result = StellarSdk.scValToNative(
          simulationResponse.result.retval
        );
        return this.parseOptionsArray(result);
      }

      return [];
    } catch (error) {
      console.error("Erro ao buscar opções:", error);
      throw error;
    }
  }

  /**
   * Converte o array de opções do formato Soroban para JavaScript
   */
  parseOptionsArray(sorobanArray) {
    if (!Array.isArray(sorobanArray)) {
      return [];
    }

    return sorobanArray.map((option) => {
      if (option instanceof Map) {
        const parsedOption = {};
        for (const [key, value] of option.entries()) {
          parsedOption[key] = value;
        }
        return parsedOption;
      }
      return option;
    });
  }

  /**
   * Valida se um endereço Stellar é válido
   */
  isValidStellarAddress(address) {
    try {
      StellarSdk.StrKey.decodeEd25519PublicKey(address);
      return true;
    } catch {
      return false;
    }
  }
}

export default new SorobanService();
