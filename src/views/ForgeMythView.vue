<template>
  <div class="min-h-screen blockchain-pattern">
    <Navbar />
    <div class="container mx-auto px-4 py-12">
      <h1
        class="text-4xl font-bold text-brazil-yellow glow-text mb-6 text-center"
      >
        Item Forge Wizard
      </h1>

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
                  >Option Type</label
                >
                <select
                  v-model="formData.optionType"
                  @change="updateBuffDescription"
                  class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-brazil-green"
                  :class="{ 'border-red-500': $v.optionType.$error }"
                >
                  <option value="">Select type...</option>
                  <option value="HealthUpgrade">Health Upgrade</option>
                  <option value="StaminaUpgrade">Stamina Upgrade</option>
                  <option value="HealOnly">Heal Only</option>
                  <option value="StaminaRestore">Stamina Restore</option>
                  <option value="DamageIncrease">Damage Increase</option>
                </select>
                <span v-if="$v.optionType.$error" class="text-red-500 text-sm"
                  >Option type is required</span
                >
              </div>

              <div>
                <label class="block text-mist-white font-medium mb-2"
                  >Rarity</label
                >
                <select
                  v-model="formData.rarity"
                  class="w-full px-4 py-3 bg-gray-700/50 border border-gray-600 rounded-lg text-white focus:outline-none focus:border-brazil-green"
                  :class="{ 'border-red-500': $v.rarity.$error }"
                >
                  <option value="">Select rarity...</option>
                  <option value="common">Common</option>
                  <option value="rare">Rare</option>
                  <option value="epic">Epic</option>
                  <option value="legendary">Legendary</option>
                  <option value="mythic">Mythic</option>
                </select>
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
import AuthService from "@/services/AuthService";
import Navbar from "@/components/Navbar.vue";
import CustomSelect from "@/components/CustomSelect.vue";

export default {
  name: "ForgeMythView",
  components: {
    Navbar,
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

    const submitForm = async () => {
      await $v.value.$validate();
      if (!$v.value.$invalid) {
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

        // Show success message (you could add a toast notification here)
        alert("Item created successfully! Check the console for details.");
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
