<template>
  <div class="min-h-screen blockchain-pattern">
    <Navbar />
    <div class="container mx-auto px-4 py-12">
      <h1
        class="text-4xl font-bold text-brazil-yellow glow-text mb-6 text-center"
      >
        Myth Forge
      </h1>
      <p class="text-mist-white text-center mb-12">
        Create and customize your own myth's with our easy-to-use wizard, as
        soon it's created, all players will be able to eventually use it!
      </p>

      <div class="max-w-4xl mx-auto mt-12">
        <!-- Progress Bar -->
        <div class="mb-8">
          <div class="flex items-center justify-between mb-4">
            <div
              v-for="(step, index) in steps"
              :key="index"
              class="flex items-center"
              :class="{ 'opacity-50': index > currentStep }"
            >
              <div
                class="w-10 h-10 rounded-full flex items-center justify-center text-white font-bold"
                :class="{
                  'bg-brazil-green': index <= currentStep,
                  'bg-gray-600': index > currentStep,
                }"
              >
                {{ index + 1 }}
              </div>
              <span class="ml-2 text-mist-white font-medium">{{
                step.title
              }}</span>
              <div
                v-if="index < steps.length - 1"
                class="w-16 h-1 mx-4"
                :class="{
                  'bg-brazil-green': index < currentStep,
                  'bg-gray-600': index >= currentStep,
                }"
              ></div>
            </div>
          </div>
        </div>

        <!-- Wizard Content -->
        <div
          class="bg-gradient-to-br from-gray-800/80 to-gray-900/90 p-8 rounded-2xl border border-brazil-green/20"
        >
          <!-- Step 1: Basic Information -->
          <div v-if="currentStep === 0" class="space-y-6">
            <h2 class="text-2xl font-bold text-mist-white mb-6">
              Basic Information
            </h2>

            <div class="grid grid-cols-1 gap-6">
              <div>
                <label class="block text-mist-white font-medium mb-2"
                  >Item Title</label
                >
                <input
                  v-model="formData.title"
                  type="text"
                  class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-brazil-green"
                  placeholder="e.g., Celestial Vigor"
                  :class="{ 'border-red-500': $v.title.$error }"
                />
                <span v-if="$v.title.$error" class="text-red-500 text-sm"
                  >Title is required</span
                >
              </div>

              <div>
                <label class="block text-mist-white font-medium mb-2"
                  >Description</label
                >
                <textarea
                  v-model="formData.description"
                  rows="3"
                  class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-brazil-green"
                  placeholder="Describe what this item does..."
                  :class="{ 'border-red-500': $v.description.$error }"
                ></textarea>
                <span v-if="$v.description.$error" class="text-red-500 text-sm"
                  >Description is required</span
                >
              </div>
            </div>
          </div>

          <!-- Step 2: Item Properties -->
          <div v-if="currentStep === 1" class="space-y-6">
            <h2 class="text-2xl font-bold text-mist-white mb-6">
              Item Properties
            </h2>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-mist-white font-medium mb-2"
                  >Type Of Myth</label
                >
                <CustomSelect
                  v-model="formData.optionType"
                  :options="optionTypeOptions"
                  placeholder="Select item type..."
                  :has-error="$v.optionType.$error"
                  searchable
                  @change="updateBuffDescription"
                />
                <span v-if="$v.optionType.$error" class="text-red-500 text-sm"
                  >Option type is required</span
                >
              </div>

              <div>
                <label class="block text-mist-white font-medium mb-2"
                  >Rarity</label
                >
                <CustomSelect
                  v-model="formData.rarity"
                  :options="rarityOptions"
                  placeholder="Select rarity level..."
                  :has-error="$v.rarity.$error"
                  searchable
                />
                <span v-if="$v.rarity.$error" class="text-red-500 text-sm"
                  >Rarity is required</span
                >
              </div>

              <div class="md:col-span-2">
                <label class="block text-mist-white font-medium mb-2"
                  >Buff Amount</label
                >
                <input
                  v-model.number="formData.value"
                  @input="updateBuffDescription"
                  type="number"
                  step="0.1"
                  class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-brazil-green"
                  placeholder="e.g., 50 (amount of health/stamina/damage)"
                  :class="{ 'border-red-500': $v.value.$error }"
                />
                <span v-if="$v.value.$error" class="text-red-500 text-sm"
                  >Buff amount is required</span
                >
                <small class="text-gray-400 mt-1 block">
                  The amount this item will provide (e.g., 50 health points, 25
                  stamina, etc.)
                </small>
              </div>
            </div>
          </div>

          <!-- Step 3: Visual & Effects -->
          <div v-if="currentStep === 2" class="space-y-6">
            <h2 class="text-2xl font-bold text-mist-white mb-6">
              Visual & Effects
            </h2>

            <div class="grid grid-cols-1 gap-6">
              <div>
                <label class="block text-mist-white font-medium mb-2"
                  >Buff Description</label
                >
                <input
                  v-model="formData.buff"
                  type="text"
                  class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-brazil-green"
                  placeholder="e.g., +50 Max Health + Full Heal"
                  :class="{ 'border-red-500': $v.buff.$error }"
                />
                <span v-if="$v.buff.$error" class="text-red-500 text-sm"
                  >Buff description is required</span
                >
                <small class="text-gray-400 mt-1 block">
                  Short description of what the buff provides to the player
                </small>
              </div>
            </div>

            <!-- Special Effects (Mythic items only) -->
            <div v-if="formData.rarity === 'mythic'" class="space-y-4">
              <h3 class="text-xl font-bold text-mystic-cyan">
                Special Effects (Mythic Only)
              </h3>
              <div class="flex items-center mb-4">
                <input
                  v-model="formData.isSpecial"
                  type="checkbox"
                  class="mr-2"
                />
                <label class="text-mist-white">Enable special effects</label>
              </div>

              <div
                v-if="formData.isSpecial"
                class="grid grid-cols-1 md:grid-cols-3 gap-4"
              >
                <div>
                  <label class="block text-mist-white font-medium mb-2"
                    >Health Bonus</label
                  >
                  <input
                    v-model.number="formData.specialEffects.healthBonus"
                    type="number"
                    step="0.1"
                    class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-mystic-cyan"
                  />
                </div>
                <div>
                  <label class="block text-mist-white font-medium mb-2"
                    >Stamina Bonus</label
                  >
                  <input
                    v-model.number="formData.specialEffects.staminaBonus"
                    type="number"
                    step="0.1"
                    class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-mystic-cyan"
                  />
                </div>
                <div>
                  <label class="block text-mist-white font-medium mb-2"
                    >Damage Bonus</label
                  >
                  <input
                    v-model.number="formData.specialEffects.damageBonus"
                    type="number"
                    step="0.1"
                    class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-mystic-cyan"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Step 4: Review -->
          <div v-if="currentStep === 3" class="space-y-6">
            <h2 class="text-2xl font-bold text-mist-white mb-6">
              Review Your Item
            </h2>

            <div class="bg-gray-700/30 p-6 rounded-lg">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                <div>
                  <span class="text-brazil-green font-medium">Name:</span>
                  <span class="text-mist-white ml-2">{{ formData.title }}</span>
                </div>
                <div>
                  <span class="text-brazil-green font-medium">Title:</span>
                  <span class="text-mist-white ml-2">{{ formData.title }}</span>
                </div>
                <div class="md:col-span-2">
                  <span class="text-brazil-green font-medium"
                    >Description:</span
                  >
                  <span class="text-mist-white ml-2">{{
                    formData.description
                  }}</span>
                </div>
                <div>
                  <span class="text-brazil-green font-medium">Type:</span>
                  <span class="text-mist-white ml-2">{{
                    formData.optionType
                  }}</span>
                </div>
                <div>
                  <span class="text-brazil-green font-medium">Rarity:</span>
                  <span
                    class="text-mist-white ml-2 capitalize"
                    :class="getRarityColor(formData.rarity)"
                    >{{ formData.rarity }}</span
                  >
                </div>
                <div>
                  <span class="text-brazil-green font-medium"
                    >Buff Amount:</span
                  >
                  <span class="text-mist-white ml-2">{{ formData.value }}</span>
                </div>
                <div>
                  <span class="text-brazil-green font-medium">Buff:</span>
                  <span class="text-mist-white ml-2">{{ formData.buff }}</span>
                </div>
                <div v-if="formData.isSpecial" class="md:col-span-2">
                  <span class="text-mystic-cyan font-medium"
                    >Special Effects:</span
                  >
                  <div class="text-mist-white ml-2 mt-1">
                    Health: +{{ formData.specialEffects.healthBonus }}, Stamina:
                    +{{ formData.specialEffects.staminaBonus }}, Damage: +{{
                      formData.specialEffects.damageBonus
                    }}
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Navigation Buttons -->
          <div class="flex justify-between mt-8">
            <button
              v-if="currentStep > 0"
              @click="previousStep"
              class="px-6 py-3 bg-gray-600 text-white rounded-lg hover:bg-gray-500 transition-colors"
            >
              Previous
            </button>
            <div v-else></div>

            <button
              v-if="currentStep < steps.length - 1"
              @click="nextStep"
              :disabled="!isCurrentStepValid"
              class="px-6 py-3 bg-brazil-green text-white rounded-lg hover:bg-brazil-green/80 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Next
            </button>
            <button
              v-else
              @click="submitForm"
              class="px-6 py-3 bg-mystic-cyan text-white rounded-lg hover:bg-mystic-cyan/80 transition-colors"
            >
              Create Item
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useVuelidate } from "@vuelidate/core";
import { required, minValue } from "@vuelidate/validators";
import { v4 as uuidv4 } from "uuid";
import Swal from "sweetalert2";
import AuthService from "@/services/AuthService";
import Navbar from "@/components/Navbar.vue";
import CustomSelect from "@/components/CustomSelect.vue";

export default {
  name: "ForgeMythView",
  components: {
    Navbar,
    CustomSelect,
  },
  setup() {
    const router = useRouter();
    const currentUser = ref(null);
    const currentStep = ref(0);

    const steps = [
      { title: "Basic Info" },
      { title: "Properties" },
      { title: "Visual & Effects" },
      { title: "Review" },
    ];

    // Select options
    const optionTypeOptions = [
      {
        value: "HealthUpgrade",
        label: "Health Upgrade",
        description: "Permanently increases maximum health points",
        icon: `<svg class="w-5 h-5 text-red-400" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M3.172 5.172a4 4 0 015.656 0L10 6.343l1.172-1.171a4 4 0 115.656 5.656L10 17.657l-6.828-6.829a4 4 0 010-5.656z" clip-rule="evenodd" /></svg>`,
      },
      {
        value: "StaminaUpgrade",
        label: "Stamina Upgrade",
        description: "Permanently increases maximum stamina points",
        icon: `<svg class="w-5 h-5 text-blue-400" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd" /></svg>`,
      },
      {
        value: "HealOnly",
        label: "Heal Only",
        description: "Instantly restores health points without permanent bonus",
        icon: `<svg class="w-5 h-5 text-green-400" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-8.293l-3-3a1 1 0 00-1.414 0l-3 3a1 1 0 001.414 1.414L9 9.414V13a1 1 0 102 0V9.414l1.293 1.293a1 1 0 001.414-1.414z" clip-rule="evenodd" /></svg>`,
      },
      {
        value: "StaminaRestore",
        label: "Stamina Restore",
        description:
          "Instantly restores stamina points without permanent bonus",
        icon: `<svg class="w-5 h-5 text-cyan-400" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z" clip-rule="evenodd" /></svg>`,
      },
      {
        value: "DamageIncrease",
        label: "Damage Increase",
        description: "Permanently increases attack damage",
        icon: `<svg class="w-5 h-5 text-orange-400" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 2L3 7v11a1 1 0 001 1h12a1 1 0 001-1V7l-7-5zM6 9.5a.5.5 0 01.5-.5h7a.5.5 0 010 1h-7a.5.5 0 01-.5-.5zm.5 2.5a.5.5 0 000 1h7a.5.5 0 000-1h-7z" clip-rule="evenodd" /></svg>`,
      },
    ];

    const rarityOptions = [
      {
        value: "common",
        label: "Common",
        rarity: "common",
        description: "Basic item with standard effects",
      },
      {
        value: "rare",
        label: "Rare",
        rarity: "rare",
        description: "Enhanced item with improved effects",
      },
      {
        value: "epic",
        label: "Epic",
        rarity: "epic",
        description: "Powerful item with significant effects",
      },
      {
        value: "legendary",
        label: "Legendary",
        rarity: "legendary",
        description: "Exceptional item with major effects",
      },
      {
        value: "mythic",
        label: "Mythic",
        rarity: "mythic",
        description: "Ultimate item with extraordinary effects",
      },
    ];

    const formData = reactive({
      optionName: "",
      description: "",
      stellarTransactionId: "",
      title: "",
      buff: "",
      icon: "default_icon",
      optionType: "",
      value: 0,
      rarity: "",
      cost: 0,
      isSpecial: false,
      specialEffects: {
        healthBonus: 0,
        staminaBonus: 0,
        damageBonus: 0,
      },
    });

    // Validation rules for each step
    const rules = computed(() => {
      const baseRules = {
        description: { required },
        title: { required },
        buff: { required },
        optionType: { required },
        value: { required, minValue: minValue(0) },
        rarity: { required },
      };
      return baseRules;
    });

    const $v = useVuelidate(rules, formData);

    const isCurrentStepValid = computed(() => {
      switch (currentStep.value) {
        case 0:
          return !$v.value.title.$invalid && !$v.value.description.$invalid;
        case 1:
          return (
            !$v.value.optionType.$invalid &&
            !$v.value.rarity.$invalid &&
            !$v.value.value.$invalid
          );
        case 2:
          return !$v.value.buff.$invalid;
        case 3:
          return true;
        default:
          return false;
      }
    });

    const nextStep = async () => {
      await $v.value.$validate();
      if (isCurrentStepValid.value && currentStep.value < steps.length - 1) {
        currentStep.value++;
      }
    };

    const previousStep = () => {
      if (currentStep.value > 0) {
        currentStep.value--;
      }
    };

    const updateBuffDescription = () => {
      if (!formData.optionType || !formData.value) {
        formData.buff = "";
        return;
      }

      const buffTemplates = {
        HealthUpgrade: (value) => `+${value} Max Health + Full Heal`,
        StaminaUpgrade: (value) => `+${value} Max Stamina + Restore All`,
        HealOnly: (value) => `Heals ${value} HP Instantly`,
        StaminaRestore: (value) => `Restores ${value} Stamina`,
        DamageIncrease: (value) => `+${value} Attack Damage`,
      };

      const template = buffTemplates[formData.optionType];
      if (template) {
        formData.buff = template(formData.value);
      }
    };

    const generateIcon = (optionType) => {
      const iconMap = {
        HealthUpgrade: "health_upgrade_icon",
        StaminaUpgrade: "stamina_upgrade_icon",
        HealOnly: "heal_potion_icon",
        StaminaRestore: "stamina_restore_icon",
        DamageIncrease: "damage_boost_icon",
      };
      return iconMap[optionType] || "default_item_icon";
    };

    const generateTransactionId = () => {
      const typeMap = {
        HealthUpgrade: "HEALTH",
        StaminaUpgrade: "STAMINA",
        HealOnly: "HEAL",
        StaminaRestore: "STAMINA-RESTORE",
        DamageIncrease: "DAMAGE",
      };

      const typeCode = typeMap[formData.optionType] || "ITEM";
      const randomNum = Math.floor(Math.random() * 1000)
        .toString()
        .padStart(3, "0");
      return `ST-${typeCode}-${randomNum}`;
    };

    const getRarityColor = (rarity) => {
      const colors = {
        common: "text-gray-400",
        rare: "text-blue-400",
        epic: "text-purple-400",
        legendary: "text-yellow-400",
        mythic: "text-mystic-cyan",
      };
      return colors[rarity] || "text-white";
    };

    const getRarityColorName = (rarity) => {
      const colors = {
        common: "gray",
        rare: "blue",
        epic: "purple",
        legendary: "yellow",
        mythic: "cyan",
      };
      return colors[rarity] || "gray";
    };

    const getOptionTypeLabel = (optionType) => {
      const labels = {
        HealthUpgrade: "Health Upgrade",
        StaminaUpgrade: "Stamina Upgrade",
        HealOnly: "Heal Only",
        StaminaRestore: "Stamina Restore",
        DamageIncrease: "Damage Increase",
      };
      return labels[optionType] || optionType;
    };

    const submitForm = async () => {
      await $v.value.$validate();
      if (!$v.value.$invalid) {
        // Show loading alert
        Swal.fire({
          title: "Creating Your Myth...",
          html: `
            <div class="flex flex-col items-center space-y-4">
              <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
              <p class="text-gray-600">Forging your legendary item in the cosmic realm...</p>
            </div>
          `,
          allowOutsideClick: false,
          allowEscapeKey: false,
          showConfirmButton: false,
          background: "#1f2937",
          color: "#f9fafb",
          customClass: {
            popup: "border border-gray-600",
          },
        });

        // Simulate creation process (you can replace this with actual API call)
        await new Promise((resolve) => setTimeout(resolve, 3000));

        // Generate stellar transaction ID and icon
        formData.stellarTransactionId = generateTransactionId();
        formData.icon = generateIcon(formData.optionType);
        formData.optionName = formData.title; // Option name same as title

        // Create the final item object
        const newItem = {
          optionName: formData.title, // Use title as option name
          description: formData.description,
          stellarTransactionId: formData.stellarTransactionId,
          title: formData.title,
          buff: formData.buff,
          icon: formData.icon,
          optionType: formData.optionType,
          value: formData.value,
          rarity: formData.rarity,
          cost: 0, // Always 0 as requested
        };

        // Add special effects if mythic and special
        if (formData.rarity === "mythic" && formData.isSpecial) {
          newItem.isSpecial = true;
          newItem.specialEffects = {
            healthBonus: formData.specialEffects.healthBonus,
            staminaBonus: formData.specialEffects.staminaBonus,
            damageBonus: formData.specialEffects.damageBonus,
          };
        }

        // Output the created item
        console.log("🎮 New Item Created:", JSON.stringify(newItem, null, 2));

        // Close loading and show success
        Swal.fire({
          title: "Myth Created Successfully!",
          html: `
            <div class="space-y-4">
              <p class="text-lg font-semibold text-blue-300">${
                formData.title
              }</p>
              <p class="text-sm text-gray-400">${formData.description}</p>
              <div class="bg-gray-800 p-3 rounded-lg">
                <p class="text-sm"><strong>Type:</strong> ${getOptionTypeLabel(
                  formData.optionType
                )}</p>
                <p class="text-sm"><strong>Rarity:</strong> <span class="capitalize text-${getRarityColorName(
                  formData.rarity
                )}-400">${formData.rarity}</span></p>
                <p class="text-sm"><strong>Effect:</strong> ${formData.buff}</p>
                <p class="text-sm"><strong>Transaction ID:</strong> ${
                  formData.stellarTransactionId
                }</p>
              </div>
              <p class="text-xs text-gray-500">Your myth has been added to the cosmic registry and will be available for all players!</p>
            </div>
          `,
          icon: "success",
          confirmButtonText: "Return to Home",
          confirmButtonColor: "#10b981",
          background: "#1f2937",
          color: "#f9fafb",
          customClass: {
            popup: "border border-gray-600",
          },
        }).then((result) => {
          if (result.isConfirmed) {
            router.push("/");
          }
        });
      }
    };

    onMounted(() => {
      currentUser.value = AuthService.getCurrentUser();
      if (!currentUser.value) {
        router.push("/login");
      }
    });

    return {
      currentUser,
      currentStep,
      steps,
      optionTypeOptions,
      rarityOptions,
      formData,
      $v,
      isCurrentStepValid,
      nextStep,
      previousStep,
      updateBuffDescription,
      submitForm,
      getRarityColor,
    };
  },
};
</script>
