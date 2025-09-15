<template>
  <div class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300" :class="cardClasses">
    <div v-if="hasImage" class="relative overflow-hidden rounded-t-lg">
      <slot name="image"></slot>
    </div>

    <div class="p-6">
      <!-- Header -->
      <div v-if="title || $slots.header" class="mb-4">
        <slot name="header">
          <h3 v-if="title" class="text-lg font-semibold text-gray-900">
            {{ title }}
          </h3>
        </slot>
      </div>

      <!-- Content -->
      <div class="mb-4">
        <slot></slot>
      </div>

      <!-- Footer -->
      <div v-if="$slots.footer" class="pt-4 border-t border-gray-200">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'BaseCard',
  props: {
    title: {
      type: String,
      default: ''
    },
    variant: {
      type: String,
      default: 'default',
      validator: value => ['default', 'bordered', 'elevated'].includes(value)
    }
  },
  computed: {
    cardClasses() {
      const variants = {
        default: '',
        bordered: 'border border-gray-200',
        elevated: 'shadow-xl'
      }
      return variants[this.variant]
    },
    hasImage() {
      return !!this.$slots.image
    }
  }
}
</script>