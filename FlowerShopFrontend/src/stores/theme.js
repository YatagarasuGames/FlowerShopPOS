import { defineStore } from 'pinia'
import { ref, watchEffect } from 'vue'

function getCookie(name) {
  const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'))
  return match ? decodeURIComponent(match[2]) : null
}

function setCookie(name, value, days = 365) {
  const maxAge = days * 24 * 60 * 60
  document.cookie = `${name}=${encodeURIComponent(value)}; path=/; max-age=${maxAge}; SameSite=Lax`
}

export const useThemeStore = defineStore('theme', () => {
  const savedCookieTheme = typeof document !== 'undefined' ? getCookie('theme') : null
  const savedLocalTheme = typeof localStorage !== 'undefined' ? localStorage.getItem('theme') : null
  const prefersDark = typeof window !== 'undefined' && window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches

  const initialTheme = savedCookieTheme || savedLocalTheme || (prefersDark ? 'dark' : 'light')
  const isDark = ref(initialTheme === 'dark')

  const toggleTheme = () => {
    isDark.value = !isDark.value
  }

  watchEffect(() => {
    if (typeof document !== 'undefined') {
      const root = document.documentElement
      const themeValue = isDark.value ? 'dark' : 'light'

      if (isDark.value) {
        root.classList.add('dark')
      } else {
        root.classList.remove('dark')
      }

      setCookie('theme', themeValue)
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem('theme', themeValue)
      }
    }
  })

  return { isDark, toggleTheme }
})