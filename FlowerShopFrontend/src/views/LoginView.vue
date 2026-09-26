<template>
  <div class="min-h-screen flex items-center justify-center bg-radial from-emerald-50/80 via-zinc-100 to-zinc-200 dark:from-zinc-950 dark:via-zinc-900 dark:to-black p-4 transition-colors duration-300">
    <div class="w-full max-w-md bg-white/85 dark:bg-zinc-900/85 backdrop-blur-2xl border border-zinc-200/80 dark:border-zinc-800/80 rounded-3xl p-8 sm:p-10 shadow-2xl shadow-emerald-950/5 transition-all">
      <div class="text-center mb-8">
        <div class="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-emerald-500/10 dark:bg-emerald-500/15 text-emerald-600 dark:text-emerald-400 mb-4 ring-1 ring-emerald-500/20 shadow-inner">
          <!-- SVG: Flower Logo -->
          <svg class="w-8 h-8" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.75">
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 21a9.004 9.004 0 008.716-6.747M12 21a9.004 9.004 0 01-8.716-6.747M12 21V12m0 0a3 3 0 10-6 0 3 3 0 006 0zm0 0a3 3 0 106 0 3 3 0 00-6 0zm0 0V3m0 0a3 3 0 10-6 0 3 3 0 006 0zm0 0a3 3 0 106 0 3 3 0 00-6 0z" />
          </svg>
        </div>
        <h2 class="text-2xl font-black tracking-tight text-zinc-900 dark:text-zinc-50">FlowerShop POS</h2>
        <p class="text-xs text-zinc-500 dark:text-zinc-400 mt-1.5 font-medium">Система складского учета и кассовых продаж</p>
      </div>

      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label class="block text-[11px] font-bold uppercase tracking-wider text-zinc-600 dark:text-zinc-400 mb-1.5">Логин</label>
          <div class="relative">
            <span class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-zinc-400">
              <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
            </span>
            <input
              v-model="username"
              type="text"
              required
              autofocus
              placeholder="admin, owner или cashier"
              class="w-full pl-10 pr-4 py-3 rounded-xl border border-zinc-200 dark:border-zinc-800 bg-zinc-50/50 dark:bg-zinc-800/40 text-zinc-900 dark:text-zinc-100 placeholder-zinc-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50 focus:border-emerald-500 transition-all text-xs font-medium"
            />
          </div>
        </div>

        <div>
          <label class="block text-[11px] font-bold uppercase tracking-wider text-zinc-600 dark:text-zinc-400 mb-1.5">Пароль</label>
          <div class="relative">
            <span class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-zinc-400">
              <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
              </svg>
            </span>
            <input
              v-model="password"
              type="password"
              required
              placeholder="••••••••"
              class="w-full pl-10 pr-4 py-3 rounded-xl border border-zinc-200 dark:border-zinc-800 bg-zinc-50/50 dark:bg-zinc-800/40 text-zinc-900 dark:text-zinc-100 placeholder-zinc-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50 focus:border-emerald-500 transition-all text-xs font-medium"
            />
          </div>
        </div>

        <div v-if="error" class="p-3 rounded-xl bg-rose-50 dark:bg-rose-950/40 border border-rose-200 dark:border-rose-900/60 text-rose-600 dark:text-rose-400 text-xs font-medium flex items-center gap-2.5">
          <svg class="w-4 h-4 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
          <span>{{ error }}</span>
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full mt-2 py-3.5 px-4 bg-emerald-600 hover:bg-emerald-700 active:scale-[0.99] disabled:opacity-50 text-white text-xs font-bold rounded-xl shadow-lg shadow-emerald-600/20 transition-all flex items-center justify-center gap-2 cursor-pointer"
        >
          <span v-if="loading" class="animate-spin inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full"></span>
          <span>{{ loading ? 'Авторизация...' : 'Войти в терминал' }}</span>
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const handleLogin = async () => {
  error.value = ''
  loading.value = true
  try {
    await authStore.login(username.value, password.value)
    if (authStore.canAccessAdminPanel) {
      router.push('/admin')
    } else {
      router.push('/pos')
    }
  } catch (err) {
    error.value = err.response?.data?.message || 'Неверный логин или пароль'
  } finally {
    loading.value = false
  }
}
</script>