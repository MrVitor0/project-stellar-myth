<template>
  <div class="relative" ref="selectContainer">
    <!-- Select Button -->
    <button
      @click="toggleDropdown"
      :class="[
        'w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-left text-white focus:outline-none transition-all duration-300 flex items-center justify-between',
        isOpen
          ? 'border-brazil-green shadow-lg shadow-brazil-green/20'
          : 'hover:border-gray-500',
        hasError ? 'border-red-500' : '',
        disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer',
      ]"
      :disabled="disabled"
    >
      <span :class="selectedOption ? 'text-white' : 'text-gray-400'">
        {{ selectedOption ? selectedOption.label : placeholder }}
      </span>

      <!-- Arrow Icon -->
      <svg
        :class="[
          'w-5 h-5 transition-transform duration-300',
          isOpen ? 'rotate-180 text-brazil-green' : 'text-gray-400',
        ]"
        fill="none"
        viewBox="0 0 24 24"
        stroke="currentColor"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M19 9l-7 7-7-7"
        />
      </svg>
    </button>

    <!-- Dropdown -->
    <transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="transform scale-95 opacity-0"
      enter-to-class="transform scale-100 opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="transform scale-100 opacity-100"
      leave-to-class="transform scale-95 opacity-0"
    >
      <div
        v-if="isOpen"
        class="absolute z-50 w-full mt-2 bg-gray-800/95 backdrop-blur-sm border border-gray-600 rounded-lg shadow-2xl shadow-black/50 max-h-60 overflow-y-auto"
      >
        <!-- Search Input (if searchable) -->
        <div v-if="searchable" class="p-3 border-b border-gray-600">
          <input
            ref="searchInput"
            v-model="searchQuery"
            type="text"
            class="w-full px-3 py-2 bg-gray-700/50 border border-gray-600 rounded-lg text-white text-sm focus:outline-none focus:border-brazil-green"
            :placeholder="`Search ${placeholder.toLowerCase()}...`"
            @click.stop
          />
        </div>

        <!-- Options List -->
        <div class="py-2">
          <div
            v-for="(option, index) in filteredOptions"
            :key="option.value"
            @click="selectOption(option)"
            :class="[
              'px-4 py-3 cursor-pointer transition-all duration-200 flex items-center',
              'hover:bg-brazil-green/20 hover:text-brazil-green',
              selectedOption && selectedOption.value === option.value
                ? 'bg-brazil-green/30 text-brazil-green border-l-4 border-brazil-green'
                : 'text-gray-300',
            ]"
          >
            <!-- Option Icon (if provided) -->
            <div
              v-if="option.icon"
              class="w-5 h-5 mr-3 flex-shrink-0"
              v-html="option.icon"
            ></div>

            <!-- Option Content -->
            <div class="flex-1">
              <div class="font-medium">{{ option.label }}</div>
              <div v-if="option.description" class="text-xs text-gray-400 mt-1">
                {{ option.description }}
              </div>
            </div>

            <!-- Rarity Badge (if rarity option) -->
            <div
              v-if="option.rarity"
              :class="[
                'px-2 py-1 rounded-full text-xs font-medium ml-2',
                getRarityColorClasses(option.rarity),
              ]"
            >
              {{ option.rarity }}
            </div>

            <!-- Check Icon for selected -->
            <svg
              v-if="selectedOption && selectedOption.value === option.value"
              class="w-4 h-4 ml-2 text-brazil-green"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fill-rule="evenodd"
                d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                clip-rule="evenodd"
              />
            </svg>
          </div>

          <!-- No options found -->
          <div
            v-if="filteredOptions.length === 0"
            class="px-4 py-3 text-gray-400 text-center"
          >
            No options found
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from "vue";

export default {
  name: "CustomSelect",
  props: {
    modelValue: {
      type: [String, Number, Object],
      default: null,
    },
    options: {
      type: Array,
      required: true,
      default: () => [],
    },
    placeholder: {
      type: String,
      default: "Select an option...",
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    searchable: {
      type: Boolean,
      default: false,
    },
    hasError: {
      type: Boolean,
      default: false,
    },
  },
  emits: ["update:modelValue", "change"],
  setup(props, { emit }) {
    const isOpen = ref(false);
    const searchQuery = ref("");
    const selectContainer = ref(null);
    const searchInput = ref(null);

    const selectedOption = computed(() => {
      if (!props.modelValue) return null;
      return props.options.find((option) => option.value === props.modelValue);
    });

    const filteredOptions = computed(() => {
      if (!props.searchable || !searchQuery.value) {
        return props.options;
      }

      const query = searchQuery.value.toLowerCase();
      return props.options.filter(
        (option) =>
          option.label.toLowerCase().includes(query) ||
          (option.description &&
            option.description.toLowerCase().includes(query))
      );
    });

    const toggleDropdown = () => {
      if (props.disabled) return;
      isOpen.value = !isOpen.value;
    };

    const selectOption = (option) => {
      emit("update:modelValue", option.value);
      emit("change", option);
      isOpen.value = false;
      searchQuery.value = "";
    };

    const closeDropdown = (event) => {
      if (
        selectContainer.value &&
        !selectContainer.value.contains(event.target)
      ) {
        isOpen.value = false;
        searchQuery.value = "";
      }
    };

    const getRarityColorClasses = (rarity) => {
      const colors = {
        common: "bg-gray-500/20 text-gray-300 border border-gray-500",
        rare: "bg-blue-500/20 text-blue-300 border border-blue-500",
        epic: "bg-purple-500/20 text-purple-300 border border-purple-500",
        legendary: "bg-yellow-500/20 text-yellow-300 border border-yellow-500",
        mythic: "bg-cyan-500/20 text-cyan-300 border border-cyan-500",
      };
      return colors[rarity] || colors.common;
    };

    // Focus search input when dropdown opens
    watch(isOpen, async (newVal) => {
      if (newVal && props.searchable) {
        await nextTick();
        searchInput.value?.focus();
      }
    });

    onMounted(() => {
      document.addEventListener("click", closeDropdown);
    });

    onUnmounted(() => {
      document.removeEventListener("click", closeDropdown);
    });

    return {
      isOpen,
      searchQuery,
      selectContainer,
      searchInput,
      selectedOption,
      filteredOptions,
      toggleDropdown,
      selectOption,
      getRarityColorClasses,
    };
  },
};
</script>
