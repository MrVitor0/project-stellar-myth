import {
  collection,
  doc,
  addDoc,
  getDoc,
  getDocs,
  updateDoc,
  deleteDoc,
  query,
  where,
  orderBy,
  limit,
  serverTimestamp,
} from "firebase/firestore";
import { db } from "@/firebase";

// Nome da coleção para bênçãos no Firestore
const BLESSINGS_COLLECTION = "blessings";

/**
 * Serviço para gerenciar as bênçãos no Firestore
 */
export const BlessingFirestoreService = {
  /**
   * Buscar todas as bênçãos
   * @param {number} limitCount - Limite de documentos (opcional)
   * @returns {Promise<Array>} - Lista de bênçãos
   */
  async getBlessings(limitCount = 20) {
    try {
      const q = query(
        collection(db, BLESSINGS_COLLECTION),
        orderBy("createdAt", "desc"),
        limit(limitCount)
      );

      const querySnapshot = await getDocs(q);
      return querySnapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    } catch (error) {
      console.error("Erro ao buscar bênçãos:", error);
      throw error;
    }
  },

  /**
   * Buscar bênçãos por tipo
   * @param {string} type - Tipo da bênção
   * @param {number} limitCount - Limite de documentos (opcional)
   * @returns {Promise<Array>} - Lista de bênçãos filtradas por tipo
   */
  async getBlessingsByType(type, limitCount = 20) {
    try {
      const q = query(
        collection(db, BLESSINGS_COLLECTION),
        where("type", "==", type),
        orderBy("createdAt", "desc"),
        limit(limitCount)
      );

      const querySnapshot = await getDocs(q);
      return querySnapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    } catch (error) {
      console.error(`Erro ao buscar bênçãos do tipo ${type}:`, error);
      throw error;
    }
  },

  /**
   * Buscar bênçãos por criador
   * @param {string} creatorId - ID do usuário criador
   * @param {number} limitCount - Limite de documentos (opcional)
   * @returns {Promise<Array>} - Lista de bênçãos do criador
   */
  async getBlessingsByCreator(creatorId, limitCount = 20) {
    try {
      const q = query(
        collection(db, BLESSINGS_COLLECTION),
        where("creatorId", "==", creatorId),
        orderBy("createdAt", "desc"),
        limit(limitCount)
      );

      const querySnapshot = await getDocs(q);
      return querySnapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    } catch (error) {
      console.error(`Erro ao buscar bênçãos do criador ${creatorId}:`, error);
      throw error;
    }
  },

  /**
   * Buscar uma bênção específica por ID
   * @param {string} blessingId - ID da bênção
   * @returns {Promise<Object|null>} - Dados da bênção ou null se não encontrada
   */
  async getBlessingById(blessingId) {
    try {
      const docRef = doc(db, BLESSINGS_COLLECTION, blessingId);
      const docSnap = await getDoc(docRef);

      if (docSnap.exists()) {
        return {
          id: docSnap.id,
          ...docSnap.data(),
        };
      } else {
        console.log(`Bênção com ID ${blessingId} não encontrada.`);
        return null;
      }
    } catch (error) {
      console.error(`Erro ao buscar bênção ${blessingId}:`, error);
      throw error;
    }
  },

  /**
   * Criar uma nova bênção
   * @param {Object} blessingData - Dados da bênção
   * @param {string} blessingData.name - Nome da bênção
   * @param {string} blessingData.description - Descrição da bênção
   * @param {string} blessingData.gift - Dádiva concedida pela bênção
   * @param {string} blessingData.patron - Patrono da bênção
   * @param {string} blessingData.type - Tipo da bênção (elementos, poder, etc)
   * @param {string} blessingData.creatorId - ID do usuário criador
   * @param {string} blessingData.creatorName - Nome do usuário criador
   * @returns {Promise<string>} - ID da bênção criada
   */
  async createBlessing(blessingData) {
    try {
      // Adiciona timestamp de criação e atualização
      const blessingWithTimestamp = {
        ...blessingData,
        createdAt: serverTimestamp(),
        updatedAt: serverTimestamp(),
      };

      const docRef = await addDoc(
        collection(db, BLESSINGS_COLLECTION),
        blessingWithTimestamp
      );
      console.log(`Bênção criada com ID: ${docRef.id}`);
      return docRef.id;
    } catch (error) {
      console.error("Erro ao criar bênção:", error);
      throw error;
    }
  },

  /**
   * Atualizar uma bênção existente
   * @param {string} blessingId - ID da bênção
   * @param {Object} updatedData - Dados atualizados
   * @returns {Promise<void>}
   */
  async updateBlessing(blessingId, updatedData) {
    try {
      const docRef = doc(db, BLESSINGS_COLLECTION, blessingId);

      // Adiciona timestamp de atualização
      const dataWithTimestamp = {
        ...updatedData,
        updatedAt: serverTimestamp(),
      };

      await updateDoc(docRef, dataWithTimestamp);
      console.log(`Bênção ${blessingId} atualizada com sucesso.`);
    } catch (error) {
      console.error(`Erro ao atualizar bênção ${blessingId}:`, error);
      throw error;
    }
  },

  /**
   * Excluir uma bênção
   * @param {string} blessingId - ID da bênção
   * @returns {Promise<void>}
   */
  async deleteBlessing(blessingId) {
    try {
      const docRef = doc(db, BLESSINGS_COLLECTION, blessingId);
      await deleteDoc(docRef);
      console.log(`Bênção ${blessingId} excluída com sucesso.`);
    } catch (error) {
      console.error(`Erro ao excluir bênção ${blessingId}:`, error);
      throw error;
    }
  },

  /**
   * Formata a data relativa para exibição (ex: "há 2 horas")
   * @param {Date} date - Data para formatar
   * @returns {string} - String formatada com tempo relativo
   */
  formatRelativeTime(timestamp) {
    if (!timestamp || !timestamp.toDate) {
      return "Agora";
    }

    const date = timestamp.toDate();
    const now = new Date();
    const diffInSeconds = Math.floor((now - date) / 1000);

    if (diffInSeconds < 60) {
      return "Agora";
    }

    const diffInMinutes = Math.floor(diffInSeconds / 60);
    if (diffInMinutes < 60) {
      return `${diffInMinutes} min atrás`;
    }

    const diffInHours = Math.floor(diffInMinutes / 60);
    if (diffInHours < 24) {
      return `${diffInHours}h atrás`;
    }

    const diffInDays = Math.floor(diffInHours / 24);
    if (diffInDays < 30) {
      return `${diffInDays}d atrás`;
    }

    const diffInMonths = Math.floor(diffInDays / 30);
    if (diffInMonths < 12) {
      return `${diffInMonths} meses atrás`;
    }

    const diffInYears = Math.floor(diffInMonths / 12);
    return `${diffInYears} anos atrás`;
  },
};

export default BlessingFirestoreService;
