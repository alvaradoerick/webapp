<script setup>
import { defineProps } from 'vue'

defineProps({
    items: {
        type: Array,
        required: true
    },
    active: {
        type: Number,
        required: true
    }
})
</script>

<template>
    <div class="stepper">
        <div class="stepper-item-container" v-for="(item, index) in items" :key="item.id">
            <div class="stepper-item">
                <div class="stepper-item-circle" :class="{ 'stepper-item-circle--active': active === item.id }">
                    {{ item.id }}
                </div>
                <div class="stepper-item-label">{{ item.label }}</div>
            </div>
            <!-- Add the line only if it's not the last item -->
            <div v-if="index !== items.length - 1" class="stepper-item-line"></div>
        </div>
    </div>
</template>
<style scoped="scoped">
.stepper {
    display: flex;
    justify-content: space-between;
}

.stepper-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
}

.stepper-item-container {
    display: flex;
    align-items: center;
}

.stepper-item-circle {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    background-color: #e0e0e0;
    display: flex;
    justify-content: center;
    align-items: center;
    color: #fff;
    font-weight: bold;
}

.stepper-item-circle--active {
    background-color: #3f51b5;
}

.stepper-item-label {
    font-size: 0.8rem;
    margin-top: 0.5rem;
}

.stepper-item-line {
  flex-grow: 1;
  height: 2px;
  background-color: #852f2f;
  color: #3f51b5;
  margin-top: 0.5rem;
  margin-left: 15px;
}
</style>
