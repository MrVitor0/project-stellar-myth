/**
 * Unity WebGL Service
 * Service responsible for Unity WebGL integration and communication
 * Following Single Responsibility Principle
 */

import UnityConfig from "./UnityConfig.js";

class UnityService {
  constructor() {
    this.instance = null;
    this.isLoaded = false;
    this.loadingProgress = 0;
    this.eventListeners = new Map();
  }

  /**
   * Initialize Unity WebGL instance
   * @param {HTMLCanvasElement} canvas - Canvas element
   * @param {Function} progressCallback - Progress callback function
   * @returns {Promise<Object>} Unity instance
   */
  async initializeUnity(canvas, progressCallback = null) {
    try {
      // Load Unity loader script
      await this.loadUnityScript();

      // Create Unity instance
      const config = this.buildConfig();

      this.instance = await window.createUnityInstance(
        canvas,
        config,
        (progress) => {
          this.loadingProgress = Math.round(progress * 100);
          if (progressCallback) {
            progressCallback(this.loadingProgress);
          }
          this.emit("progress", this.loadingProgress);
        }
      );

      this.isLoaded = true;
      this.emit("loaded", this.instance);
      return this.instance;
    } catch (error) {
      this.emit("error", error);
      throw error;
    }
  }

  /**
   * Load Unity loader script
   * @returns {Promise<void>}
   */
  loadUnityScript() {
    return new Promise((resolve, reject) => {
      if (window.createUnityInstance) {
        resolve();
        return;
      }

      const script = document.createElement("script");
      script.src = `${UnityConfig.buildPath}/build.loader.js`;
      script.onload = resolve;
      script.onerror = reject;
      document.head.appendChild(script);
    });
  }

  /**
   * Build Unity configuration object
   * @returns {Object} Unity configuration
   */
  buildConfig() {
    const buildPath = UnityConfig.buildPath;
    return {
      dataUrl: `${buildPath}/build.data`,
      frameworkUrl: `${buildPath}/build.framework.js`,
      codeUrl: `${buildPath}/build.wasm`,
      streamingAssetsUrl: UnityConfig.config.streamingAssetsUrl,
      companyName: UnityConfig.config.companyName,
      productName: UnityConfig.config.productName,
      productVersion: UnityConfig.config.productVersion,
    };
  }

  /**
   * Send message to Unity GameObject
   * @param {string} gameObject - GameObject name
   * @param {string} method - Method name
   * @param {*} value - Value to send (optional)
   */
  sendMessage(gameObject, method, value = "") {
    if (!this.instance || !this.isLoaded) {
      console.warn("Unity instance not loaded yet");
      return false;
    }

    try {
      this.instance.SendMessage(gameObject, method, value);
      return true;
    } catch (error) {
      console.error(
        `Failed to send message to ${gameObject}.${method}:`,
        error
      );
      return false;
    }
  }

  /**
   * Start the game
   */
  startGame() {
    return this.sendMessage(
      UnityConfig.gameObjects.gameManager,
      UnityConfig.methods.startGame
    );
  }

  /**
   * Connect wallet to the game
   * @param {Object} walletData - Wallet connection data
   */
  connectWallet(walletData = {}) {
    return this.sendMessage(
      UnityConfig.gameObjects.blockchainManager,
      UnityConfig.methods.connectWallet,
      JSON.stringify(walletData)
    );
  }

  /**
   * Update blessings in the game
   * @param {Array} blessings - Array of blessing objects
   */
  updateBlessings(blessings) {
    return this.sendMessage(
      UnityConfig.gameObjects.gameManager,
      UnityConfig.methods.updateBlessings,
      JSON.stringify(blessings)
    );
  }

  /**
   * Set player data in the game
   * @param {Object} playerData - Player data object
   */
  setPlayerData(playerData) {
    return this.sendMessage(
      UnityConfig.gameObjects.gameManager,
      UnityConfig.methods.setPlayerData,
      JSON.stringify(playerData)
    );
  }

  /**
   * Cleanup Unity instance
   */
  quit() {
    if (this.instance) {
      try {
        this.instance.Quit();
      } catch (error) {
        console.warn("Error quitting Unity instance:", error);
      }
      this.instance = null;
      this.isLoaded = false;
      this.loadingProgress = 0;
    }
  }

  /**
   * Add event listener
   * @param {string} event - Event name
   * @param {Function} callback - Callback function
   */
  on(event, callback) {
    if (!this.eventListeners.has(event)) {
      this.eventListeners.set(event, []);
    }
    this.eventListeners.get(event).push(callback);
  }

  /**
   * Remove event listener
   * @param {string} event - Event name
   * @param {Function} callback - Callback function
   */
  off(event, callback) {
    if (this.eventListeners.has(event)) {
      const listeners = this.eventListeners.get(event);
      const index = listeners.indexOf(callback);
      if (index > -1) {
        listeners.splice(index, 1);
      }
    }
  }

  /**
   * Emit event
   * @param {string} event - Event name
   * @param {*} data - Event data
   */
  emit(event, data) {
    if (this.eventListeners.has(event)) {
      this.eventListeners.get(event).forEach((callback) => {
        try {
          callback(data);
        } catch (error) {
          console.error(`Error in event listener for ${event}:`, error);
        }
      });
    }
  }

  /**
   * Get loading progress
   * @returns {number} Loading progress (0-100)
   */
  getLoadingProgress() {
    return this.loadingProgress;
  }

  /**
   * Check if Unity is loaded
   * @returns {boolean} Is loaded
   */
  isUnityLoaded() {
    return this.isLoaded;
  }

  /**
   * Get Unity instance
   * @returns {Object|null} Unity instance
   */
  getInstance() {
    return this.instance;
  }
}

// Singleton pattern
const unityService = new UnityService();

export default unityService;
