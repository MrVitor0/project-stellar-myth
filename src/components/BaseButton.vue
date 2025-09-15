<template>
  <component :is="tag" :to="to" :href="href" :type="buttonType" :disabled="disabled || loading"
    class="inline-flex items-center justify-center font-medium transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2"
    :class="[buttonClasses, sizeClasses, { 'opacity-50 cursor-not-allowed': disabled }]" @click="handleClick">
    <!-- Loading Spinner -->
    <svg v-if="loading" class="animate-spin -ml-1 mr-2 h-4 w-4" :class="{ 'mr-0': !$slots.default }" fill="none"
      viewBox="0 0 24 24">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
      <path class="opacity-75" fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
      </path>
    </svg>

    <!-- Icon (left) -->
    <span v-if="$slots.iconLeft" class="mr-2">
      <slot name="iconLeft"></slot>
    </span>

    <!-- Content -->
    <slot></slot>

    <!-- Icon (right) -->
    <span v-if="$slots.iconRight" class="ml-2">
      <slot name="iconRight"></slot>
    </span>
  </component>
</template>

<script>
export default {
  name: 'BaseButton',
  props: {
    variant: {
      type: String,
      default: 'primary',
      validator: value => ['primary', 'secondary', 'outline', 'ghost', 'danger'].includes(value)
    },
    size: {
      type: String,
      default: 'md',
      validator: value => ['sm', 'md', 'lg', 'xl'].includes(value)
    },
    disabled: {
      type: Boolean,
      default: false
    },
    loading: {
      type: Boolean,
      default: false
    },
    to: {
      type: [String, Object],
      default: null
    },
    href: {
      type: String,
      default: null
    },
    type: {
      type: String,
      default: 'button'
    }
  },
  computed: {
    tag() {
      if (this.to) return 'router-link'
      if (this.href) return 'a'
      return 'button'
    },
    buttonType() {
      return this.tag === 'button' ? this.type : null
    },
    buttonClasses() {
      const variants = {
        primary: 'bg-stellar-600 hover:bg-stellar-700 focus:ring-stellar-500 text-white border border-transparent',
        secondary: 'bg-gray-200 hover:bg-gray-300 focus:ring-gray-500 text-gray-900 border border-transparent',
        outline: 'bg-transparent hover:bg-stellar-50 focus:ring-stellar-500 text-stellar-700 border border-stellar-300',
        ghost: 'bg-transparent hover:bg-gray-100 focus:ring-gray-500 text-gray-700 border border-transparent',
        danger: 'bg-red-600 hover:bg-red-700 focus:ring-red-500 text-white border border-transparent'
      }
      return variants[this.variant]
    },
    sizeClasses() {
      const sizes = {
        sm: 'px-3 py-1.5 text-sm rounded-md',
        md: 'px-4 py-2 text-sm rounded-lg',
        lg: 'px-6 py-3 text-base rounded-lg',
        xl: 'px-8 py-4 text-lg rounded-xl'
      }
      return sizes[this.size]
    }
  },
  methods: {
    handleClick(event) {
      if (!this.disabled && !this.loading) {
        this.$emit('click', event)
      }
    }
  },
  emits: ['click']
}
</script>