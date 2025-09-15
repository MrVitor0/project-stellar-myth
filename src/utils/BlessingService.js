/**
 * Blessing Data Service
 * Responsible for managing blessing data and business logic
 * Following Single Responsibility Principle
 */

class BlessingService {
  constructor() {
    this.blessings = [
      {
        id: 1,
        name: "Fúria do Guerreiro",
        description:
          "Uma bênção poderosa que aumenta significativamente o dano de ataque, ideal para combatentes de primeira linha.",
        gift: "+25% Ataque",
        patron: "Moreira",
        type: "Ao atacar",
        creator: "DragonSlayer",
        timeAgo: "2 min",
      },
      {
        id: 2,
        name: "Agilidade Sombria",
        description:
          "Concede velocidade supernatural ao portador, permitindo movimentos mais rápidos em combate e exploração.",
        gift: "+30% Velocidade",
        patron: "Page",
        type: "Passivo",
        creator: "ShadowRunner",
        timeAgo: "5 min",
      },
      {
        id: 3,
        name: "Coração Valente",
        description:
          "Aumenta a resistência e pontos de vida, perfeita para tanques e defensores que lideram a linha de frente.",
        gift: "+40 Vida",
        patron: "Maria Bonita",
        type: "Passivo",
        creator: "IronGuard",
        timeAgo: "8 min",
      },
      {
        id: 4,
        name: "Lâmina Flamejante",
        description:
          "Adiciona dano flamejante a cada ataque, causando dano contínuo aos inimigos atingidos.",
        gift: "+15% Ataque + Queima",
        patron: "Moreira",
        type: "Ao atacar",
        creator: "FireMaster",
        timeAgo: "12 min",
      },
      {
        id: 5,
        name: "Espírito Livre",
        description:
          "Uma bênção que combina velocidade e regeneração, ideal para exploradores e scouts.",
        gift: "+20% Velocidade + Regen",
        patron: "Page",
        type: "Passivo",
        creator: "WildWanderer",
        timeAgo: "15 min",
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
