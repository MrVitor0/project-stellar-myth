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

  scValToNative(scVal) {
    return StellarSdk.xdr.ScVal.fromXDR(scVal.toXDR(), "base64").toXDRObject();
  }

  /**
   * Obtém as últimas N opções do contrato
   */
  async getLastNOptions(n = 10) {
    try {
      console.log(`🔍 Buscando as últimas ${n} opções do contrato...`);

      // Criar uma conta temporária para a simulação (sem usar saldo)
      const tempKeypair = StellarSdk.Keypair.random();
      const tempAccount = new StellarSdk.Account(
        tempKeypair.publicKey(),
        "0" // Sequence number 0 para simulação
      );

      // Criar operação de contrato para chamar get_last_n_options
      const contract = new StellarSdk.Contract(this.contractAddress);
      const operation = contract.call(
        "get_last_n_options",
        StellarSdk.xdr.ScVal.scvU32(n)
      );

      // Construir transação para simulação
      const transaction = new StellarSdk.TransactionBuilder(tempAccount, {
        fee: "0", // Taxa 0 para simulação
        networkPassphrase: this.networkPassphrase,
      })
        .addOperation(operation)
        .setTimeout(30)
        .build();

      // Simular transação (não consome recursos da rede)
      console.log("⚡ Simulando chamada do contrato...");
      const simulateResponse = await this.sorobanServer.simulateTransaction(
        transaction
      );

      // Verificar se houve erro na simulação
      if (StellarSdk.rpc.Api.isSimulationError(simulateResponse)) {
        console.error("❌ Erro na simulação:", simulateResponse.error);
        throw new Error(`Simulation error: ${simulateResponse.error}`);
      }

      // Extrair o resultado da simulação
      const result = simulateResponse.result.retval;

      // Método 1: Usar a função nativa do Stellar SDK (recomendado)
      // const nativeResult = StellarSdk.scValToNative(result);

      // Método 2: Usar nossa função personalizada (mantendo compatibilidade)
      const options = this.scValToJsOptions(result);
      return options;
    } catch (error) {
      console.error("❌ Erro ao buscar opções do contrato:", error);

      // Em caso de erro, retornar array vazio ou dados de fallback
      console.log("🔄 Retornando dados de fallback...");
      return [];
    }
  }

  /**
   * Obtém as últimas N opções do contrato usando scValToNative do SDK
   */
  async getLastNOptionsWithNativeConverter(n = 10) {
    try {
      console.log(
        `🔍 Buscando as últimas ${n} opções do contrato (usando scValToNative)...`
      );

      // Criar uma conta temporária para a simulação (sem usar saldo)
      const tempKeypair = StellarSdk.Keypair.random();
      const tempAccount = new StellarSdk.Account(
        tempKeypair.publicKey(),
        "0" // Sequence number 0 para simulação
      );

      // Criar operação de contrato para chamar get_last_n_options
      const contract = new StellarSdk.Contract(this.contractAddress);
      const operation = contract.call(
        "get_last_n_options",
        StellarSdk.xdr.ScVal.scvU32(n)
      );

      // Construir transação para simulação
      const transaction = new StellarSdk.TransactionBuilder(tempAccount, {
        fee: "0", // Taxa 0 para simulação
        networkPassphrase: this.networkPassphrase,
      })
        .addOperation(operation)
        .setTimeout(30)
        .build();

      // Simular transação (não consome recursos da rede)
      console.log("⚡ Simulando chamada do contrato...");
      const simulateResponse = await this.sorobanServer.simulateTransaction(
        transaction
      );

      // Verificar se houve erro na simulação
      if (StellarSdk.rpc.Api.isSimulationError(simulateResponse)) {
        console.error("❌ Erro na simulação:", simulateResponse.error);
        throw new Error(`Simulation error: ${simulateResponse.error}`);
      }

      // Extrair o resultado da simulação e converter usando a função nativa do SDK
      const result = simulateResponse.result.retval;
      console.log(
        "🔄 Convertendo resultado ScVal usando scValToNative...",
        result
      );

      // Usar scValToNative diretamente do Stellar SDK
      const nativeResult = StellarSdk.scValToNative(result);
      console.log("📋 Resultado nativo:", nativeResult);

      // Se o resultado é um array, retornar diretamente
      if (Array.isArray(nativeResult)) {
        console.log(`✅ ${nativeResult.length} opções carregadas do contrato`);
        return nativeResult;
      }

      // Se não é array, pode ser um único item
      const options = Array.isArray(nativeResult)
        ? nativeResult
        : [nativeResult];
      console.log(`✅ ${options.length} opções carregadas do contrato`);
      return options;
    } catch (error) {
      console.error("❌ Erro ao buscar opções do contrato:", error);
      return [];
    }
  }

  /**
   * Converte ScVal para array de opções JavaScript
   */
  scValToJsOptions(scVal) {
    try {
      console.log("🔄 Convertendo ScVal para opções JS...");

      // Verificar o tipo do ScVal
      const scValType = scVal.switch();
      console.log("📝 Tipo do ScVal:", scValType.name);

      if (scValType === StellarSdk.xdr.ScValType.scvVec()) {
        // É um vetor (array) de opções
        const vec = scVal.vec();
        return vec.map((item, index) => {
          console.log(`🔄 Convertendo item ${index}...`);
          return this.scValToJsOption(item);
        });
      } else if (scValType === StellarSdk.xdr.ScValType.scvMap()) {
        // É um mapa (object) - pode ser uma única opção
        return [this.scValToJsOption(scVal)];
      } else {
        console.log("⚠️ Tipo ScVal não reconhecido para conversão de opções");
        return [];
      }
    } catch (error) {
      console.error("❌ Erro ao converter ScVal para opções JS:", error);
      return [];
    }
  }

  /**
   * Converte um único ScVal para uma opção JavaScript
   */
  scValToJsOption(scVal) {
    try {
      const scValType = scVal.switch();

      if (scValType === StellarSdk.xdr.ScValType.scvMap()) {
        const map = scVal.map();
        const option = {};

        // Converter cada entrada do mapa
        map.forEach((entry) => {
          const key = this.scValToJsValue(entry.key());
          const value = this.scValToJsValue(entry.val());

          // Mapear os nomes das chaves do contrato para JavaScript
          switch (key) {
            case "buff":
              option.buff = value;
              break;
            case "cost":
              option.cost = typeof value === "bigint" ? Number(value) : value;
              break;
            case "description":
              option.description = value;
              break;
            case "icon":
              option.icon = value;
              break;
            case "option_name":
              option.optionName = value;
              break;
            case "option_type":
              option.optionType = value;
              break;
            case "owner":
              option.owner = value;
              break;
            case "rarity":
              option.rarity = value;
              break;
            case "stellar_transaction_id":
              option.stellarTransactionId = value;
              break;
            case "title":
              option.title = value;
              break;
            case "value":
              option.value = typeof value === "bigint" ? Number(value) : value;
              break;
            default:
              option[key] = value;
          }
        });

        return option;
      } else {
        console.log(
          "⚠️ ScVal não é um mapa para conversão de opção individual"
        );
        return {
          rawValue: this.scValToJsValue(scVal),
          type: scValType.name,
        };
      }
    } catch (error) {
      console.error("❌ Erro ao converter ScVal para opção JS:", error);
      return {
        error: error.message,
        rawScVal: scVal,
      };
    }
  }

  /**
   * Converte um ScVal para valor JavaScript básico
   */
  scValToJsValue(scVal) {
    try {
      const scValType = scVal.switch();

      switch (scValType.value) {
        case StellarSdk.xdr.ScValType.scvBool().value:
          return scVal.b();

        case StellarSdk.xdr.ScValType.scvVoid().value:
          return null;

        case StellarSdk.xdr.ScValType.scvU32().value:
          return scVal.u32();

        case StellarSdk.xdr.ScValType.scvI32().value:
          return scVal.i32();

        case StellarSdk.xdr.ScValType.scvU64().value:
          return BigInt(scVal.u64().toString());

        case StellarSdk.xdr.ScValType.scvI64().value:
          return BigInt(scVal.i64().toString());

        case StellarSdk.xdr.ScValType.scvString().value:
          return scVal.str().toString();

        case StellarSdk.xdr.ScValType.scvSymbol().value:
          return scVal.sym().toString();

        case StellarSdk.xdr.ScValType.scvBytes().value:
          return scVal.bytes();

        case StellarSdk.xdr.ScValType.scvAddress().value:
          return StellarSdk.Address.fromScAddress(scVal.address()).toString();

        case StellarSdk.xdr.ScValType.scvVec().value:
          return scVal.vec().map((item) => this.scValToJsValue(item));

        case StellarSdk.xdr.ScValType.scvMap().value:
          const result = {};
          scVal.map().forEach((entry) => {
            const key = this.scValToJsValue(entry.key());
            const value = this.scValToJsValue(entry.val());
            result[key] = value;
          });
          return result;

        default:
          console.log("⚠️ Tipo ScVal não suportado:", scValType.name);
          return scVal.toString();
      }
    } catch (error) {
      console.error("❌ Erro ao converter ScVal para valor JS:", error);
      return scVal.toString();
    }
  }
}

export default new SorobanService();
