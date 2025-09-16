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
}

export default new FreighterService();
