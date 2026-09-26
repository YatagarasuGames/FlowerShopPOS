import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import LoginView from '../views/LoginView.vue'
import PosView from '../views/PosView.vue'
import AdminView from '../views/AdminView.vue'

const routes = [
  { path: '/login', component: LoginView },
  {
    path: '/pos',
    component: PosView,
    meta: { requiresAuth: true, roles: ['Cashier', 'Owner', 'Admin'] }
  },
  {
    path: '/admin',
    component: AdminView,
    meta: { requiresAuth: true, roles: ['Owner', 'Admin'] }
  },
  { path: '/:pathMatch(.*)*', redirect: '/pos' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next('/login')
  }

  if (to.meta.roles && !to.meta.roles.includes(authStore.role)) {
    // Если у кассира нет прав на /admin — перенаправляем на кассу
    return next('/pos')
  }

  next()
})

export default router