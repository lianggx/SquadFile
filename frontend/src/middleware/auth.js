// 路由守卫 - 权限验证中间件

/**
 * 检查用户是否已登录
 * @returns {boolean} 是否已登录
 */
export function isAuthenticated() {
  // 检查localStorage中是否存在token
  const token = localStorage.getItem('token');
  return !!token;
}

/**
 * 路由守卫 - 检查页面访问权限
 * @param {string} pagePath - 页面路径
 * @returns {boolean} 是否允许访问
 */
export function checkPageAccess(pagePath) {
  // 白名单页面（无需登录即可访问的页面）
  const whiteList = [
    '/pages/auth/login',      // 登录页面（不同格式）
    '/pages/share/index'      // 分享页面（不同格式）
  ];
  
  // 如果是白名单页面，或者以/share/开头的路径，允许访问
  if (whiteList.includes(pagePath) || pagePath.startsWith('/share/')) {
    return true;
  }
  
  // 其他页面需要登录后才能访问
  return isAuthenticated();
}

/**
 * 重定向到登录页面
 */
export function redirectToLogin() {
  // 使用uni-app的路由跳转API
  uni.redirectTo({
    url: '/src/pages/auth/login'
  });
}

/**
 * 路由前置守卫
 * @param {string} url - 目标页面URL
 * @param {Function} next - 继续导航的回调函数
 */
export function beforeEach(url, next) {
  // 解析页面路径
  const pagePath = url.split('?')[0]; // 移除查询参数
  
  // 检查页面访问权限
  if (checkPageAccess(pagePath)) {
    // 允许访问
    next(true);
  } else {
    // 未登录且访问非白名单页面，重定向到登录页面
    redirectToLogin();
    next(false);
  }
}