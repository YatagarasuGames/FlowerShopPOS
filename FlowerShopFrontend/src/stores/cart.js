import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import apiClient from '../api/axios'

export const useCartStore = defineStore('cart', () => {
  const singleItems = ref([])
  const compositions = ref([])

  const getDiscountedPrice = (basePrice, discountType, discountValue) => {
    if (discountType === 1) { // Процент
      const pct = Math.min(100, Math.max(0, Number(discountValue) || 0))
      return Math.max(0, Math.round(basePrice * (1 - pct / 100)))
    }
    if (discountType === 2) { // Фикс цена
      return Math.max(0, Number(discountValue) || 0)
    }
    return basePrice
  }

  const calcItemPrice = (item) => {
    const unitPrice = getDiscountedPrice(item.sellingPrice, item.discountType, item.discountValue)
    return unitPrice * item.quantity
  }

  const singleItemsTotal = computed(() => {
    return singleItems.value.reduce((sum, item) => sum + calcItemPrice(item), 0)
  })

  const compositionsTotal = computed(() => {
    return compositions.value.reduce((sum, comp) => {
      const itemsTotal = comp.items.reduce((iSum, item) => iSum + calcItemPrice(item), 0)
      return sum + (Number(comp.assemblyPrice) || 0) + itemsTotal
    }, 0)
  })

  const totalAmount = computed(() => {
    return singleItemsTotal.value + compositionsTotal.value
  })

  const totalItemsCount = computed(() => {
    const singleCount = singleItems.value.reduce((sum, i) => sum + i.quantity, 0)
    const compCount = compositions.value.reduce((sum, c) => sum + c.items.reduce((iSum, i) => iSum + i.quantity, 0), 0)
    return singleCount + compCount
  })

  // Добавление товара с авто-распределением по доступным партиям (FIFO)
  const addSingleItem = (product) => {
    if (!product.activeBatches || product.activeBatches.length === 0) {
      // Фолбэк, если партий нет
      const existing = singleItems.value.find(i => i.productId === product.id)
      if (existing) {
        if (existing.quantity < product.stock) existing.quantity++
      } else if (product.stock > 0) {
        singleItems.value.push({
          cartItemId: `${product.id}_default`,
          productId: product.id,
          productBatchId: null,
          name: product.name,
          sellingPrice: product.price,
          maxStock: product.stock,
          quantity: 1,
          discountType: 0,
          discountValue: 0
        })
      }
      return
    }

    // Ищем первую партию, в которой еще есть не зарезервированный остаток
    for (const batch of product.activeBatches) {
      const cartItem = singleItems.value.find(i => i.productBatchId === batch.batchId)
      const currentQtyInCart = cartItem ? cartItem.quantity : 0

      if (currentQtyInCart < batch.remainingQuantity) {
        if (cartItem) {
          cartItem.quantity++
        } else {
          singleItems.value.push({
            cartItemId: `${product.id}_${batch.batchId}`,
            productId: product.id,
            productBatchId: batch.batchId,
            name: product.name,
            sellingPrice: batch.sellingPrice,
            purchasePrice: batch.purchasePrice,
            maxStock: batch.remainingQuantity,
            quantity: 1,
            discountType: 0,
            discountValue: 0
          })
        }
        return
      }
    }
  }

  const updateSingleQuantity = (cartItemId, delta) => {
    const item = singleItems.value.find(i => i.cartItemId === cartItemId)
    if (!item) return

    item.quantity += delta
    if (item.quantity <= 0) {
      removeSingleItem(cartItemId)
    } else if (item.quantity > item.maxStock) {
      item.quantity = item.maxStock
    }
  }

  const setSingleItemDiscount = (cartItemId, type, value) => {
    const item = singleItems.value.find(i => i.cartItemId === cartItemId)
    if (item) {
      item.discountType = type
      item.discountValue = value
    }
  }

  const removeSingleItem = (cartItemId) => {
    singleItems.value = singleItems.value.filter(i => i.cartItemId !== cartItemId)
  }

  const addComposition = (compData) => {
    compositions.value.push({
      id: crypto.randomUUID ? crypto.randomUUID() : Date.now().toString(),
      name: compData.name || 'Сборный букет',
      assemblyPrice: Number(compData.assemblyPrice) || 0,
      items: compData.items.map(i => ({
        ...i,
        discountType: i.discountType || 0,
        discountValue: Number(i.discountValue) || 0
      }))
    })
  }

  const removeComposition = (compId) => {
    compositions.value = compositions.value.filter(c => c.id !== compId)
  }

  const clear = () => {
    singleItems.value = []
    compositions.value = []
  }

  const checkout = async () => {
    if (singleItems.value.length === 0 && compositions.value.length === 0) return

    const payload = {
      singleItems: singleItems.value.map(i => ({
        productId: i.productId,
        productBatchId: i.productBatchId,
        quantity: i.quantity,
        discountType: i.discountType,
        discountValue: Number(i.discountValue) || 0
      })),
      compositions: compositions.value.map(c => ({
        name: c.name,
        assemblyPrice: Number(c.assemblyPrice) || 0,
        items: c.items.map(i => ({
          productId: i.productId,
          productBatchId: i.productBatchId || null,
          quantity: i.quantity,
          discountType: i.discountType,
          discountValue: Number(i.discountValue) || 0
        }))
      }))
    }

    const res = await apiClient.post('/orders', payload)
    clear()
    return res.data
  }

  return {
    singleItems,
    compositions,
    totalAmount,
    totalItemsCount,
    getDiscountedPrice,
    calcItemPrice,
    addSingleItem,
    updateSingleQuantity,
    setSingleItemDiscount,
    removeSingleItem,
    addComposition,
    removeComposition,
    clear,
    checkout
  }
})