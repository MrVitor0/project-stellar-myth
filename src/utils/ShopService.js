import shopOptionsData from "../data/shopOptions.json";

/**
 * Serviço para gerenciar as opções da loja
 * Fornece funcionalidades para carregar, filtrar e selecionar opções da loja
 */
class ShopService {
  constructor() {
    this.shopOptions = shopOptionsData.shopOptions || [];
    this.rarityWeights = shopOptionsData.rarityWeights || {};
    this.metadata = shopOptionsData.metadata || {};
  }

  /**
   * Obtém todas as opções da loja
   * @returns {Array} Array com todas as opções da loja
   */
  getAllOptions() {
    return [...this.shopOptions];
  }

  /**
   * Obtém opções filtradas por raridade
   * @param {string} rarity - Raridade desejada (common, rare, epic, legendary, mythic)
   * @returns {Array} Array com opções da raridade especificada
   */
  getOptionsByRarity(rarity) {
    return this.shopOptions.filter((option) => option.rarity === rarity);
  }

  /**
   * Obtém opções filtradas por tipo
   * @param {string} optionType - Tipo da opção (HealthUpgrade, StaminaUpgrade, etc.)
   * @returns {Array} Array com opções do tipo especificado
   */
  getOptionsByType(optionType) {
    return this.shopOptions.filter(
      (option) => option.optionType === optionType
    );
  }

  /**
   * Seleciona 3 opções aleatórias baseadas em pesos de raridade
   * @param {number} playerLevel - Nível do jogador (influencia raridade)
   * @returns {Array} Array com 3 opções selecionadas
   */
  getRandomOptions(playerLevel = 1) {
    const availableOptions = [...this.shopOptions];
    const selectedOptions = [];

    // Ajusta probabilidades baseado no nível do jogador
    const adjustedWeights = this.getAdjustedWeights(playerLevel);

    // Seleciona 3 opções diferentes
    while (selectedOptions.length < 3 && availableOptions.length > 0) {
      const selectedRarity = this.selectRandomRarity(adjustedWeights);
      const optionsOfRarity = availableOptions.filter(
        (option) => option.rarity === selectedRarity
      );

      if (optionsOfRarity.length > 0) {
        const randomIndex = Math.floor(Math.random() * optionsOfRarity.length);
        const selectedOption = optionsOfRarity[randomIndex];

        selectedOptions.push(selectedOption);

        // Remove a opção selecionada para evitar duplicatas
        const originalIndex = availableOptions.indexOf(selectedOption);
        availableOptions.splice(originalIndex, 1);
      }
    }

    // Se não conseguiu 3 opções, preenche com opções aleatórias restantes
    while (selectedOptions.length < 3 && availableOptions.length > 0) {
      const randomIndex = Math.floor(Math.random() * availableOptions.length);
      selectedOptions.push(availableOptions[randomIndex]);
      availableOptions.splice(randomIndex, 1);
    }

    return selectedOptions;
  }

  /**
   * Ajusta os pesos de raridade baseado no nível do jogador
   * @param {number} playerLevel - Nível do jogador
   * @returns {Object} Pesos ajustados
   */
  getAdjustedWeights(playerLevel) {
    const baseWeights = { ...this.rarityWeights };

    // Aumenta chance de itens raros conforme o nível
    if (playerLevel >= 5) {
      baseWeights.rare *= 1.2;
      baseWeights.common *= 0.9;
    }

    if (playerLevel >= 10) {
      baseWeights.epic *= 1.5;
      baseWeights.legendary *= 1.3;
      baseWeights.rare *= 1.1;
      baseWeights.common *= 0.8;
    }

    if (playerLevel >= 15) {
      baseWeights.mythic *= 2.0;
      baseWeights.legendary *= 1.4;
      baseWeights.epic *= 1.2;
      baseWeights.common *= 0.7;
    }

    return baseWeights;
  }

  /**
   * Seleciona uma raridade aleatória baseada nos pesos
   * @param {Object} weights - Pesos de raridade
   * @returns {string} Raridade selecionada
   */
  selectRandomRarity(weights) {
    const totalWeight = Object.values(weights).reduce(
      (sum, weight) => sum + weight,
      0
    );
    let random = Math.random() * totalWeight;

    for (const [rarity, weight] of Object.entries(weights)) {
      random -= weight;
      if (random <= 0) {
        return rarity;
      }
    }

    // Fallback para common se algo der errado
    return "common";
  }

  /**
   * Converte uma opção da loja para o formato JSON esperado pelo Unity
   * @param {Object} option - Opção da loja
   * @returns {Object} Opção formatada para Unity
   */
  formatOptionForUnity(option) {
    return {
      optionName: option.optionName,
      description: option.description,
      stellarTransactionId: option.stellarTransactionId,
      title: option.title,
      buff: option.buff,
      icon: option.icon,
      optionType: option.optionType,
      value: option.value,
      rarity: option.rarity,
      cost: option.cost || 0,
      isSpecial: option.isSpecial || false,
      specialEffects: option.specialEffects || null,
    };
  }

  /**
   * Converte múltiplas opções para o formato Unity
   * @param {Array} options - Array de opções
   * @returns {Array} Array de opções formatadas
   */
  formatOptionsForUnity(options) {
    return options.map((option) => this.formatOptionForUnity(option));
  }

  /**
   * Gera dados completos da loja para enviar ao Unity
   * @param {number} playerLevel - Nível do jogador
   * @param {Object} playerStats - Estatísticas do jogador
   * @returns {Object} Dados completos da loja
   */
  generateShopData(playerLevel = 1, playerStats = {}) {
    const randomOptions = this.getRandomOptions(playerLevel);
    const formattedOptions = this.formatOptionsForUnity(randomOptions);

    return {
      options: formattedOptions,
      playerLevel: playerLevel,
      playerStats: playerStats,
      shopMetadata: {
        version: this.metadata.version,
        timestamp: new Date().toISOString(),
        sessionId: this.generateSessionId(),
        totalAvailableOptions: this.shopOptions.length,
      },
    };
  }

  /**
   * Gera um ID de sessão único
   * @returns {string} ID de sessão
   */
  generateSessionId() {
    return `shop_${Math.random().toString(36).substr(2, 9)}_${Date.now()}`;
  }

  /**
   * Obtém estatísticas das opções da loja
   * @returns {Object} Estatísticas
   */
  getShopStats() {
    const rarityCount = {};
    const typeCount = {};

    this.shopOptions.forEach((option) => {
      rarityCount[option.rarity] = (rarityCount[option.rarity] || 0) + 1;
      typeCount[option.optionType] = (typeCount[option.optionType] || 0) + 1;
    });

    return {
      totalOptions: this.shopOptions.length,
      rarityDistribution: rarityCount,
      typeDistribution: typeCount,
      averageCost:
        this.shopOptions.reduce((sum, opt) => sum + (opt.cost || 0), 0) /
        this.shopOptions.length,
      averageValue:
        this.shopOptions.reduce((sum, opt) => sum + opt.value, 0) /
        this.shopOptions.length,
    };
  }

  /**
   * Valida uma opção da loja
   * @param {Object} option - Opção para validar
   * @returns {boolean} True se válida
   */
  validateOption(option) {
    const requiredFields = ["optionName", "description", "optionType", "value"];
    const validTypes = [
      "HealthUpgrade",
      "StaminaUpgrade",
      "HealOnly",
      "StaminaRestore",
      "DamageIncrease",
    ];
    const validRarities = ["common", "rare", "epic", "legendary", "mythic"];

    // Verifica campos obrigatórios
    for (const field of requiredFields) {
      if (
        !option.hasOwnProperty(field) ||
        option[field] === null ||
        option[field] === undefined
      ) {
        console.warn(
          `ShopService: Campo obrigatório '${field}' não encontrado na opção:`,
          option
        );
        return false;
      }
    }

    // Verifica tipo válido
    if (!validTypes.includes(option.optionType)) {
      console.warn(
        `ShopService: Tipo '${option.optionType}' não é válido. Tipos válidos:`,
        validTypes
      );
      return false;
    }

    // Verifica raridade válida
    if (option.rarity && !validRarities.includes(option.rarity)) {
      console.warn(
        `ShopService: Raridade '${option.rarity}' não é válida. Raridades válidas:`,
        validRarities
      );
      return false;
    }

    // Verifica valor numérico
    if (typeof option.value !== "number" || option.value <= 0) {
      console.warn(
        `ShopService: Valor '${option.value}' deve ser um número positivo`
      );
      return false;
    }

    return true;
  }

  /**
   * Obtém informações de debug do serviço
   * @returns {Object} Informações de debug
   */
  getDebugInfo() {
    return {
      service: "ShopService",
      version: "1.0.0",
      totalOptions: this.shopOptions.length,
      validOptions: this.shopOptions.filter((opt) => this.validateOption(opt))
        .length,
      metadata: this.metadata,
      stats: this.getShopStats(),
    };
  }
}

// Cria e exporta uma instância singleton
const shopService = new ShopService();

export default shopService;
