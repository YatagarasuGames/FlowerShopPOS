import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import apiClient from '../api/axios'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || null)
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))

  const isAuthenticated = computed(() => !!token.value)
  const role = computed(() => user.value?.role || '')

  const isAdmin = computed(() => role.value === 'Admin')
  const isOwner = computed(() => role.value === 'Owner')
  const isCashier = computed(() => role.value === 'Cashier')

  // Доступ к панели управления имеют и Владелец, и Админ
  const canAccessAdminPanel = computed(() => isAdmin.value || isOwner.value)

  const login = async (username, password) => {
    const response = await apiClient.post('/auth/login', { username, password })
    const data = response.data

    token.value = data.token
    user.value = {
      id: data.id,
      username: data.username,
      role: data.role
    }

    localStorage.setItem('token', data.token)
    localStorage.setItem('user', JSON.stringify(user.value))
  }

  const logout = () => {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  return {
    token,
    user,
    role,
    isAuthenticated,
    isAdmin,
    isOwner,
    isCashier,
    canAccessAdminPanel,
    login,
    logout
  }
})