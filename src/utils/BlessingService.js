/**
 * Blessing Data Service
 * Responsible for managing blessing data and business logic
 * Following Single Responsibility Principle
 */

class BlessingService {
  constructor() {
    this.blessings = [
      {
        id: "ST-12047-445",
        name: "Fúria do Guerreiro",
        description:
          "Uma bênção poderosa que aumenta significativamente o dano de ataque, ideal para combatentes de primeira linha.",
        power: 25,
        patron: "Moreira",
        type: "Ataque",
        creator: "GABC7XHELLOWORLD12345EXAMPLE67890STELLAR1234567890",
        creatorShort: "GABC7XHE...",
        timeAgo: "2 min",
        rarity: "legendary",
        buff: "+25% Dano de Ataque",
      },
      {
        id: "ST-12046-332",
        name: "Agilidade Sombria",
        description:
          "Concede velocidade supernatural ao portador, permitindo movimentos mais rápidos em combate e exploração.",
        power: 30,
        patron: "Page",
        type: "Suporte",
        creator: "GDEF456STELLAR789ACCOUNT012345EXAMPLE6789012345678",
        creatorShort: "GDEF456S...",
        timeAgo: "5 min",
        rarity: "epic",
        buff: "+30% Velocidade de Movimento",
      },
      {
        id: "ST-12045-189",
        name: "Coração Valente",
        description:
          "Aumenta a resistência e pontos de vida, perfeita para tanques e defensores que lideram a linha de frente.",
        power: 40,
        patron: "Maria Bonita",
        type: "Defesa",
        creator: "GHIJ789BLOCKCHAIN012STELLAR345EXAMPLE678901234567890",
        creatorShort: "GHIJ789B...",
        timeAgo: "8 min",
        rarity: "rare",
        buff: "+40 Pontos de Vida Máxima",
      },
      {
        id: "ST-12044-067",
        name: "Lâmina Flamejante",
        description:
          "Adiciona dano flamejante a cada ataque, causando dano contínuo aos inimigos atingidos.",
        power: 15,
        patron: "Moreira",
        type: "Ataque",
        creator: "GKLM012NEXUS345STELLAR678ACCOUNT901234567890123456",
        creatorShort: "GKLM012N...",
        timeAgo: "12 min",
        rarity: "common",
        buff: "+15% Dano + Efeito Queimadura",
      },
      {
        id: "ST-12043-821",
        name: "Espírito Livre",
        description:
          "Uma bênção que combina velocidade e regeneração, ideal para exploradores e scouts.",
        power: 20,
        patron: "Page",
        type: "Suporte",
        creator: "GNOP345FREEDOM678STELLAR901ACCOUNT234567890123456",
        creatorShort: "GNOP345F...",
        timeAgo: "15 min",
        rarity: "uncommon",
        buff: "+20% Velocidade + Regeneração de Vida",
      },
    ];
  }

  /**
   * Get all blessings
   * @returns {Array} Array of blessing objects
   */
  getAllBlessings() {
    return [...this.blessings];
  }

  /**
   * Get recent blessings (last 5)
   * @returns {Array} Array of recent blessing objects
   */
  getRecentBlessings() {
    return this.blessings.slice(0, 5);
  }

  /**
   * Get blessings by patron
   * @param {string} patron - Patron name
   * @returns {Array} Filtered blessing objects
   */
  getBlessingsByPatron(patron) {
    return this.blessings.filter(
      (blessing) => blessing.patron.toLowerCase() === patron.toLowerCase()
    );
  }

  /**
   * Get blessings by type
   * @param {string} type - Blessing type
   * @returns {Array} Filtered blessing objects
   */
  getBlessingsByType(type) {
    return this.blessings.filter((blessing) => blessing.type === type);
  }

  /**
   * Add new blessing (simulated)
   * @param {Object} blessingData - New blessing data
   * @returns {Object} Created blessing
   */
  createBlessing(blessingData) {
    const newBlessing = {
      id: this.blessings.length + 1,
      ...blessingData,
      timeAgo: "Agora",
    };

    this.blessings.unshift(newBlessing);
    return newBlessing;
  }

  /**
   * Get blessing statistics
   * @returns {Object} Statistics object
   */
  getStatistics() {
    const patronCount = {};
    const typeCount = {};

    this.blessings.forEach((blessing) => {
      patronCount[blessing.patron] = (patronCount[blessing.patron] || 0) + 1;
      typeCount[blessing.type] = (typeCount[blessing.type] || 0) + 1;
    });

    return {
      total: this.blessings.length,
      byPatron: patronCount,
      byType: typeCount,
    };
  }
}

// Singleton pattern for global access
const blessingService = new BlessingService();

export default blessingService;
