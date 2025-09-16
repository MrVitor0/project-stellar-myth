<template>
  <div class="blessing-showcase">
    <div class="text-center justify-center mb-8">
      <h2 class="text-3xl font-bold text-mist-white mb-3">Recent Blessings</h2>
      <p class="text-mid-gray">Created by the community</p>
      <div class="flex items-center justify-center mt-2">
        <div
          class="w-2 h-2 bg-brazil-green rounded-full animate-pulse mr-2"
        ></div>
        <span class="text-sm text-brazil-green"
          >{{ blessings.length }} new blessings</span
        >
      </div>
    </div>

    <div
      class="space-y-4 max-h-96 mx-auto justify-center overflow-y-auto custom-scrollbar"
    >
      <BlessingCard
        v-for="blessing in blessings"
        :key="blessing.id"
        :blessing="blessing"
        class="animate-fade-in-up"
      />
    </div>

    <div class="text-center mt-6">
      <button
        class="btn-secondary text-sm py-2 px-4 inline-flex items-center justify-center"
        @click="openCreateBlessingModal"
        :disabled="!isAuthenticated"
      >
        <Plus class="w-4 h-4 mr-2" />
        Create Blessing
      </button>
      <p v-if="!isAuthenticated" class="text-sm text-mid-gray mt-2">
        <router-link
          to="/login"
          class="text-brazil-green hover:text-brazil-yellow"
          >Login</router-link
        >
        to create your own blessing
      </p>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import { Plus } from "lucide-vue-next";
import BlessingCard from "./BlessingCard.vue";
import { BlessingFirestoreService } from "@/services/BlessingFirestoreService";
import { useAuth } from "@/composables/useAuth";

export default {
  name: "BlessingShowcase",
  components: {
    BlessingCard,
    Plus,
  },
  props: {
    initialBlessings: {
      type: Array,
      default: () => [],
      validator(blessings) {
        return blessings.every(
          (blessing) =>
            blessing.id &&
            blessing.name &&
            blessing.description &&
            blessing.gift &&
            blessing.patron &&
            blessing.type &&
            blessing.creatorName
        );
      },
    },
    limit: {
      type: Number,
      default: 10,
    },
  },
  setup(props) {
    const blessings = ref([]);
    const isLoading = ref(true);
    const { currentUser, isAuthenticated } = useAuth();

    onMounted(async () => {
      // Usar dados iniciais (se fornecidos) ou buscar do Firestore
      if (props.initialBlessings && props.initialBlessings.length > 0) {
        blessings.value = props.initialBlessings;
        isLoading.value = false;
      } else {
        await fetchBlessings();
      }
    });

    const fetchBlessings = async () => {
      try {
        isLoading.value = true;
        const fetchedBlessings = await BlessingFirestoreService.getBlessings(
          props.limit
        );

        // Formatar os dados para compatibilidade com o componente
        blessings.value = fetchedBlessings.map((blessing) => {
          return {
            id: blessing.id,
            name: blessing.name,
            description: blessing.description,
            gift: blessing.gift,
            patron: blessing.patron,
            type: blessing.type,
            creator: blessing.creatorName,
            creatorId: blessing.creatorId,
            timeAgo: BlessingFirestoreService.formatRelativeTime(
              blessing.createdAt
            ),
          };
        });
      } catch (error) {
        console.error("Erro ao buscar bênçãos:", error);
      } finally {
        isLoading.value = false;
      }
    };

    const openCreateBlessingModal = () => {
      // Implementar abertura de modal para criar bênção
      console.log("Abrir modal de criação de bênção");
      // Aqui você poderia emitir um evento para abrir um modal
    };

    return {
      blessings,
      isLoading,
      isAuthenticated,
      openCreateBlessingModal,
    };
  },
};
</script>

<style scoped>
.custom-scrollbar {
  scrollbar-width: thin;
  scrollbar-color: rgba(34, 197, 94, 0.3) rgba(15, 23, 42, 0.2);
}

.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: rgba(15, 23, 42, 0.2);
  border-radius: 3px;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: rgba(34, 197, 94, 0.3);
  border-radius: 3px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: rgba(34, 197, 94, 0.5);
}

.animate-fade-in-up {
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
