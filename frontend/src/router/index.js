import { createRouter, createWebHistory } from 'vue-router'
import Home from '../pages/home.vue'
import Login from '../pages/auth/login.vue'  // 修改为正确的登录页面路径
import Profile from '../pages/profile/index.vue'
import EditProfile from '../pages/profile/edit-profile.vue'
import ChangePassword from '../pages/profile/change-password.vue'
import Admin from '../pages/admin.vue'
import AdminSettings from '../pages/admin/settings.vue'
import UserManagement from '../pages/admin/user-management.vue'
import FolderManagement from '../pages/admin/folder-management.vue'
import ShortLinkManagement from '../pages/admin/shortlink-management.vue'  // 添加短链接管理页面
import SystemLogs from '../pages/admin/system-logs.vue'  // 添加系统日志页面
import Reports from '../pages/admin/reports.vue'  // 添加统计报表页面
import Share from '../pages/share/index.vue'  // 添加分享页面
import { useAuthStore } from '../stores/auth'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/profile',
    name: 'Profile',
    component: Profile
  },
  {
    path: '/profile/edit',
    name: 'EditProfile',
    component: EditProfile
  },
  {
    path: '/profile/change-password',
    name: 'ChangePassword',
    component: ChangePassword
  },
  {
    path: '/admin',
    name: 'Admin',
    component: Admin
  },
  {
    path: '/admin/settings',
    name: 'AdminSettings',
    component: AdminSettings
  },
  {
    path: '/admin/users',
    name: 'UserManagement',
    component: UserManagement
  },
  {
    path: '/admin/folders',
    name: 'FolderManagement',
    component: FolderManagement
  },
  {
    path: '/admin/shortlinks',  // 添加短链接管理路由
    name: 'ShortLinkManagement',
    component: ShortLinkManagement
  },
  {
    path: '/admin/logs',  // 添加系统日志路由
    name: 'SystemLogs',
    component: SystemLogs
  },
  {
    path: '/admin/reports',  // 添加统计报表路由
    name: 'Reports',
    component: Reports
  },
  {
    path: '/share/:shortCode',
    name: 'Share',
    component: Share,
    props: true
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 添加路由守卫
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isAuthenticated = authStore.checkAuth()
  
  // 白名单路径（无需登录即可访问）
  const whiteList = ['/login']
  
  // 检查是否为分享页面
  const isSharePage = to.path.startsWith('/share/')
  
  // 如果目标路径在白名单中，或者用户已认证，或者是分享页面，则允许访问
  if (whiteList.includes(to.path) || isAuthenticated || isSharePage) {
    next()
  } else {
    // 未认证且目标路径不在白名单中，重定向到登录页
    next('/login')
  }
})

export default router