/**
 * Unity WebGL Configuration
 * Centralized configuration for Unity WebGL integration
 */

export const UnityConfig = {
  // Build paths
  buildPath: "/webgl/Build",

  // Unity instance configuration
  config: {
    companyName: "Stellar Myth Studios",
    productName: "Stellar Myth",
    productVersion: "1.0.0",
    streamingAssetsUrl: "StreamingAssets",
  },

  // Canvas configuration
  canvas: {
    id: "unity-canvas",
    backgroundColor: "#1e293b",
  },

  // Loading configuration
  loading: {
    showProgress: true,
    messages: {
      loading: "Loading WebGL Game...",
      blockchain: "Initializing blockchain connection",
      error: "Failed to load game. Falling back to basic mode.",
    },
  },

  // Game Objects and Methods for Unity communication
  gameObjects: {
    gameManager: "WebGLCommunicator",
    blockchainManager: "WebGLCommunicator",
    uiManager: "UIManager",
  },

  methods: {
    startGame: "StartGame",
    connectWallet: "ConnectWallet",
    updateBlessings: "UpdateBlessings",
    setPlayerData: "SetPlayerData",
    receiveParameter: "ReceiveParameter",
    updateShopOptions: "UpdateShopOptions",
    sendShopData: "OnShopOptionsReceivedFromJS",
  },

  // Error handling
  errorHandling: {
    fallbackToBasicWebGL: true,
    maxRetries: 3,
    retryDelay: 2000,
  },
};

export default UnityConfig;
