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

    // Usando o endpoint correto do Soroban RPC
    this.sorobanServer = new StellarSdk.rpc.Server(
      "https://soroban-testnet.stellar.org",
      { allowHttp: false }
    );

    this.networkPassphrase = StellarSdk.Networks.TESTNET;
    this.contractAddress =
      "CAKHS4ZPT7YGOXDKXEXSH6G7VQPA5K3MUICQDBXZRAJSLOKRT67KY3S4";
  }

  /**
   * Gera um novo par de chaves Stellar
   */
  generateKeyPair() {
    return StellarSdk.Keypair.random();
  }

  /**
   * Financia uma conta na testnet usando o Friendbot
   */
  async fundAccount(publicKey) {
    try {
      const response = await fetch(
        `https://friendbot.stellar.org?addr=${encodeURIComponent(publicKey)}`
      );
      const responseJSON = await response.json();
      console.log("SUCCESS! You have a new account :)\n", responseJSON);
      return responseJSON;
    } catch (error) {
      console.error("ERROR! Unable to fund account", error);
      throw error;
    }
  }

  /**
   * Converte um objeto JavaScript Option para ScVal com validação rigorosa
   */
  jsOptionToScVal(option) {
    try {
      // Validação rigorosa dos tipos
      if (typeof option.buff !== "string") {
        throw new Error(`buff must be a string, got ${typeof option.buff}`);
      }
      if (typeof option.cost !== "number" || option.cost < 0) {
        throw new Error(
          `cost must be a non-negative number, got ${typeof option.cost}: ${
            option.cost
          }`
        );
      }
      if (typeof option.description !== "string") {
        throw new Error(
          `description must be a string, got ${typeof option.description}`
        );
      }
      if (typeof option.icon !== "string") {
        throw new Error(`icon must be a string, got ${typeof option.icon}`);
      }
      if (typeof option.optionName !== "string") {
        throw new Error(
          `optionName must be a string, got ${typeof option.optionName}`
        );
      }
      if (typeof option.optionType !== "string") {
        throw new Error(
          `optionType must be a string, got ${typeof option.optionType}`
        );
      }
      if (typeof option.owner !== "string") {
        throw new Error(`owner must be a string, got ${typeof option.owner}`);
      }
      if (typeof option.rarity !== "string") {
        throw new Error(`rarity must be a string, got ${typeof option.rarity}`);
      }
      if (typeof option.stellarTransactionId !== "string") {
        throw new Error(
          `stellarTransactionId must be a string, got ${typeof option.stellarTransactionId}`
        );
      }
      if (typeof option.title !== "string") {
        throw new Error(`title must be a string, got ${typeof option.title}`);
      }
      if (typeof option.value !== "number" || option.value < 0) {
        throw new Error(
          `value must be a non-negative number, got ${typeof option.value}: ${
            option.value
          }`
        );
      }

      // Validar se owner é um endereço válido
      try {
        StellarSdk.StrKey.decodeEd25519PublicKey(option.owner);
      } catch (error) {
        throw new Error(`Invalid owner address: ${option.owner}`);
      }

      const scValMap = [
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("buff"),
          val: StellarSdk.xdr.ScVal.scvString(option.buff),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("cost"),
          val: StellarSdk.xdr.ScVal.scvU64(
            new StellarSdk.xdr.Uint64(option.cost)
          ),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("description"),
          val: StellarSdk.xdr.ScVal.scvString(option.description),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("icon"),
          val: StellarSdk.xdr.ScVal.scvString(option.icon),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("option_name"),
          val: StellarSdk.xdr.ScVal.scvString(option.optionName),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("option_type"),
          val: StellarSdk.xdr.ScVal.scvString(option.optionType),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("owner"),
          val: new StellarSdk.Address(option.owner).toScVal(),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("rarity"),
          val: StellarSdk.xdr.ScVal.scvString(option.rarity),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("stellar_transaction_id"),
          val: StellarSdk.xdr.ScVal.scvString(option.stellarTransactionId),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("title"),
          val: StellarSdk.xdr.ScVal.scvString(option.title),
        },
        {
          key: StellarSdk.xdr.ScVal.scvSymbol("value"),
          val: StellarSdk.xdr.ScVal.scvU64(
            new StellarSdk.xdr.Uint64(option.value)
          ),
        },
      ];

      // Criar o mapa de entradas
      const mapEntries = scValMap.map(
        (entry) =>
          new StellarSdk.xdr.ScMapEntry({
            key: entry.key,
            val: entry.val,
          })
      );

      // Retornar como um mapa ScVal
      return StellarSdk.xdr.ScVal.scvMap(mapEntries);
    } catch (error) {
      console.error("Error converting JS Option to ScVal:", error);
      throw error;
    }
  }

  /**
   * Cria uma nova opção usando keypair local
   */
  async createOption(optionData, keypair) {
    try {
      console.log("🚀 Creating option with keypair:", optionData);

      // Carregar conta do usuário
      const account = await this.server.loadAccount(keypair.publicKey());

      // Construir argumentos para o contrato
      const userAddress = new StellarSdk.Address(keypair.publicKey());

      // Adicionar o owner ao optionData antes de converter
      const completeOptionData = {
        ...optionData,
        owner: keypair.publicKey(),
      };

      const optionScVal = this.jsOptionToScVal(completeOptionData);

      // Criar operação do contrato
      const contract = new StellarSdk.Contract(this.contractAddress);
      const operation = contract.call(
        "create_option",
        userAddress.toScVal(),
        optionScVal
      );

      // Construir transação
      const transaction = new StellarSdk.TransactionBuilder(account, {
        fee: StellarSdk.BASE_FEE,
        networkPassphrase: this.networkPassphrase,
      })
        .addOperation(operation)
        .setTimeout(30)
        .build();

      // Simular transação primeiro
      const simulateResponse = await this.sorobanServer.simulateTransaction(
        transaction
      );

      if (StellarSdk.rpc.Api.isSimulationError(simulateResponse)) {
        throw new Error(`Simulation error: ${simulateResponse.error}`);
      }

      // Preparar transação com os recursos necessários
      const preparedTransactionBuilder = StellarSdk.rpc.assembleTransaction(
        transaction,
        simulateResponse
      );

      // Build a transação final
      const preparedTransaction = preparedTransactionBuilder.build();

      // Assinar transação
      preparedTransaction.sign(keypair);

      // Enviar transação
      const sendResponse = await this.sorobanServer.sendTransaction(
        preparedTransaction
      );

      if (sendResponse.status === "ERROR") {
        throw new Error(`Send error: ${sendResponse.errorResult}`);
      }

      // Aguardar resultado
      let getResponse = await this.sorobanServer.getTransaction(
        sendResponse.hash
      );

      // Aguardar até a transação ser processada
      while (getResponse.status === "NOT_FOUND") {
        console.log("⏳ Aguardando transação ser processada...");
        await new Promise((resolve) => setTimeout(resolve, 1000));
        getResponse = await this.sorobanServer.getTransaction(
          sendResponse.hash
        );
      }

      if (getResponse.status === "SUCCESS") {
        console.log("✅ Transação bem-sucedida!");
        return {
          success: true,
          hash: sendResponse.hash,
          result: getResponse.returnValue,
        };
      } else {
        throw new Error(`Transaction failed: ${getResponse.status}`);
      }
    } catch (error) {
      console.error("❌ Error creating option:", error);
      throw error;
    }
  }

  /**
   * Cria uma nova opção usando Freighter
   */
  async createOptionWithFreighter(optionData) {
    try {
      console.log("🚀 Creating option with Freighter:", optionData);

      // Importar FreighterService dinamicamente
      const FreighterService = await import("./FreighterService.js").then(
        (module) => module.default
      );

      // Obter chave pública do Freighter
      const publicKey = await FreighterService.getPublicKey();
      if (!publicKey) {
        throw new Error("Não foi possível obter a chave pública do Freighter");
      }

      // Carregar conta do usuário
      const account = await this.server.loadAccount(publicKey);

      // Construir argumentos para o contrato
      const userAddress = new StellarSdk.Address(publicKey);

      // Adicionar o owner ao optionData antes de converter
      const completeOptionData = {
        ...optionData,
        owner: publicKey,
      };

      const optionScVal = this.jsOptionToScVal(completeOptionData);

      // Criar operação do contrato
      const contract = new StellarSdk.Contract(this.contractAddress);
      const operation = contract.call(
        "create_option",
        userAddress.toScVal(),
        optionScVal
      );

      // Construir transação
      const transaction = new StellarSdk.TransactionBuilder(account, {
        fee: StellarSdk.BASE_FEE,
        networkPassphrase: this.networkPassphrase,
      })
        .addOperation(operation)
        .setTimeout(30)
        .build();

      // Simular transação primeiro
      const simulateResponse = await this.sorobanServer.simulateTransaction(
        transaction
      );

      if (StellarSdk.rpc.Api.isSimulationError(simulateResponse)) {
        throw new Error(`Simulation error: ${simulateResponse.error}`);
      }

      // Preparar transação com os recursos necessários
      const preparedTransactionBuilder = StellarSdk.rpc.assembleTransaction(
        transaction,
        simulateResponse
      );

      // Build a transação final
      const preparedTransaction = preparedTransactionBuilder.build();

      // Converter transação para XDR
      const transactionXdr = preparedTransaction.toXDR();

      // Assinar com Freighter
      const signedXdr = await FreighterService.signTransaction(
        transactionXdr,
        publicKey,
        this.networkPassphrase
      );

      // Criar transação assinada
      const signedTransaction = StellarSdk.TransactionBuilder.fromXDR(
        signedXdr,
        this.networkPassphrase
      );

      // Enviar transação
      const sendResponse = await this.sorobanServer.sendTransaction(
        signedTransaction
      );

      if (sendResponse.status === "ERROR") {
        throw new Error(`Send error: ${sendResponse.errorResult}`);
      }

      // Aguardar resultado
      let getResponse = await this.sorobanServer.getTransaction(
        sendResponse.hash
      );

      // Aguardar até a transação ser processada
      while (getResponse.status === "NOT_FOUND") {
        console.log("⏳ Aguardando transação ser processada...");
        await new Promise((resolve) => setTimeout(resolve, 1000));
        getResponse = await this.sorobanServer.getTransaction(
          sendResponse.hash
        );
      }

      if (getResponse.status === "SUCCESS") {
        console.log("✅ Transação bem-sucedida!");
        return {
          success: true,
          hash: sendResponse.hash,
          result: getResponse.returnValue,
        };
      } else {
        throw new Error(`Transaction failed: ${getResponse.status}`);
      }
    } catch (error) {
      console.error("❌ Error creating option with Freighter:", error);
      throw error;
    }
  }

  /**
   * Obtém as últimas N opções do contrato
   */
  async getLastNOptions(n = 10) {
    try {
      console.log(`🔍 Getting last ${n} options from contract`);

      // Criar operação de consulta
      const contract = new StellarSdk.Contract(this.contractAddress);
      const operation = contract.call(
        "get_last_n_options",
        StellarSdk.xdr.ScVal.scvU32(n)
      );

      // Criar transação temporária para simular
      const account = await this.server.loadAccount(
        StellarSdk.Keypair.random().publicKey()
      );
      const transaction = new StellarSdk.TransactionBuilder(account, {
        fee: StellarSdk.BASE_FEE,
        networkPassphrase: this.networkPassphrase,
      })
        .addOperation(operation)
        .setTimeout(30)
        .build();

      // Simular transação para obter resultado
      const simulateResponse = await this.sorobanServer.simulateTransaction(
        transaction
      );

      if (StellarSdk.rpc.Api.isSimulationError(simulateResponse)) {
        throw new Error(`Simulation error: ${simulateResponse.error}`);
      }

      // Processar resultado
      if (simulateResponse.result && simulateResponse.result.retval) {
        const options = this.scValToJsOptions(simulateResponse.result.retval);
        console.log(`✅ Retrieved ${options.length} options`);
        return options;
      }

      return [];
    } catch (error) {
      console.error("❌ Error getting options:", error);
      throw error;
    }
  }

  /**
   * Converte ScVal para array de opções JavaScript
   */
  scValToJsOptions(scVal) {
    try {
      // Esta é uma implementação simplificada
      // Em produção, você precisaria de uma conversão mais robusta
      if (scVal.instance && scVal.instance().vec) {
        return scVal
          .instance()
          .vec()
          .map((item) => ({
            // Conversão básica - você pode expandir isso conforme necessário
            optionName: "Converted Option",
            description: "Converted from contract",
            // ... outros campos
          }));
      }
      return [];
    } catch (error) {
      console.error("Error converting ScVal to JS options:", error);
      return [];
    }
  }
}

export default new SorobanService();
