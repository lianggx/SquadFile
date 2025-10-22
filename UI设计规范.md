# SquadFile UI/UX设计规范

## 1. 设计理念

### 1.1 核心原则
- **简洁易用**: 界面简洁，操作直观，降低学习成本
- **一致性**: 保持设计语言统一，提供一致的用户体验
- **可访问性**: 支持无障碍访问，兼容不同设备和屏幕
- **响应式**: 适配多种设备，提供最佳浏览体验

### 1.2 设计目标
- 新用户5分钟内完成核心操作
- 界面加载时间不超过2秒
- 支持键盘导航和屏幕阅读器
- 移动端和桌面端体验一致

## 2. 颜色规范

### 2.1 主色调
```scss
// 主色调 - 蓝色系
$primary-50: #E3F2FD;
$primary-100: #BBDEFB;
$primary-200: #90CAF9;
$primary-300: #64B5F6;
$primary-400: #42A5F5;
$primary-500: #2196F3;  // 主色
$primary-600: #1E88E5;
$primary-700: #1976D2;
$primary-800: #1565C0;
$primary-900: #0D47A1;

// 辅助色 - 绿色系（成功）
$success-50: #E8F5E8;
$success-500: #4CAF50;
$success-700: #388E3C;

// 警告色 - 橙色系
$warning-50: #FFF3E0;
$warning-500: #FF9800;
$warning-700: #F57C00;

// 错误色 - 红色系
$error-50: #FFEBEE;
$error-500: #F44336;
$error-700: #D32F2F;
```

### 2.2 中性色
```scss
// 文字颜色
$text-primary: #212121;      // 主要文字
$text-secondary: #757575;    // 次要文字
$text-disabled: #BDBDBD;     // 禁用文字
$text-hint: #9E9E9E;         // 提示文字

// 背景颜色
$background-default: #FAFAFA;  // 默认背景
$background-paper: #FFFFFF;   // 卡片背景
$background-level1: #F5F5F5;  // 一级背景
$background-level2: #EEEEEE;  // 二级背景

// 边框颜色
$border-light: #E0E0E0;      // 浅色边框
$border-medium: #BDBDBD;     // 中等边框
$border-dark: #757575;       // 深色边框
```

### 2.3 功能色彩
```scss
// 文件类型色彩
$file-document: #2196F3;     // 文档类
$file-image: #4CAF50;        // 图片类
$file-video: #FF5722;        // 视频类
$file-audio: #9C27B0;        // 音频类
$file-archive: #795548;      // 压缩包类
$file-other: #607D8B;        // 其他类型

// 权限等级色彩
$permission-view: #2196F3;    // 查看权限
$permission-edit: #FF9800;    // 编辑权限
$permission-manage: #F44336;  // 管理权限
```

## 3. 字体规范

### 3.1 字体家族
```scss
// 中文字体
$font-family-chinese: 'PingFang SC', 'Hiragino Sans GB', 'Microsoft YaHei', 
                      'WenQuanYi Micro Hei', sans-serif;

// 英文字体
$font-family-english: 'Inter', 'Roboto', 'Helvetica Neue', Arial, sans-serif;

// 等宽字体
$font-family-mono: 'JetBrains Mono', 'Fira Code', 'Monaco', 'Consolas', 
                   'Courier New', monospace;

// 默认字体栈
$font-family-base: $font-family-english, $font-family-chinese;
```

### 3.2 字体大小
```scss
// 标题字体
$font-size-h1: 2.125rem;  // 34px
$font-size-h2: 1.75rem;   // 28px
$font-size-h3: 1.5rem;    // 24px
$font-size-h4: 1.25rem;   // 20px
$font-size-h5: 1.125rem;  // 18px
$font-size-h6: 1rem;      // 16px

// 正文字体
$font-size-body1: 1rem;      // 16px
$font-size-body2: 0.875rem;  // 14px
$font-size-caption: 0.75rem; // 12px

// 按钮字体
$font-size-button: 0.875rem; // 14px

// 代码字体
$font-size-code: 0.875rem;   // 14px
```

### 3.3 行高
```scss
$line-height-tight: 1.2;    // 紧密行高
$line-height-normal: 1.5;   // 正常行高
$line-height-relaxed: 1.7;  // 宽松行高
```

## 4. 间距规范

### 4.1 间距系统
```scss
// 基础间距单位（8px系统）
$spacing-0: 0;
$spacing-1: 0.25rem;  // 4px
$spacing-2: 0.5rem;   // 8px
$spacing-3: 0.75rem;  // 12px
$spacing-4: 1rem;     // 16px
$spacing-5: 1.25rem;  // 20px
$spacing-6: 1.5rem;   // 24px
$spacing-8: 2rem;     // 32px
$spacing-10: 2.5rem;  // 40px
$spacing-12: 3rem;    // 48px
$spacing-16: 4rem;    // 64px
$spacing-20: 5rem;    // 80px
```

### 4.2 组件间距
```scss
// 页面边距
$page-padding-mobile: $spacing-4;   // 移动端页面边距
$page-padding-tablet: $spacing-6;   // 平板端页面边距
$page-padding-desktop: $spacing-8;  // 桌面端页面边距

// 卡片间距
$card-padding: $spacing-6;          // 卡片内边距
$card-margin: $spacing-4;           // 卡片外边距

// 列表间距
$list-item-padding: $spacing-4;     // 列表项边距
$list-gap: $spacing-2;              // 列表项间隙
```

## 5. 圆角规范

### 5.1 圆角系统
```scss
$border-radius-sm: 0.25rem;   // 4px - 小圆角
$border-radius-md: 0.5rem;    // 8px - 中圆角
$border-radius-lg: 0.75rem;   // 12px - 大圆角
$border-radius-xl: 1rem;      // 16px - 超大圆角
$border-radius-full: 50%;     // 圆形
```

### 5.2 组件圆角
```scss
// 按钮圆角
$button-border-radius: $border-radius-md;

// 输入框圆角
$input-border-radius: $border-radius-sm;

// 卡片圆角
$card-border-radius: $border-radius-lg;

// 模态框圆角
$modal-border-radius: $border-radius-xl;
```

## 6. 阴影规范

### 6.1 阴影系统
```scss
// 阴影层级
$shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
$shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
$shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
$shadow-xl: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
$shadow-2xl: 0 25px 50px -12px rgba(0, 0, 0, 0.25);

// 特殊阴影
$shadow-inner: inset 0 2px 4px 0 rgba(0, 0, 0, 0.06);
$shadow-none: none;
```

### 6.2 组件阴影
```scss
// 卡片阴影
$card-shadow: $shadow-md;
$card-shadow-hover: $shadow-lg;

// 按钮阴影
$button-shadow: $shadow-sm;
$button-shadow-hover: $shadow-md;

// 模态框阴影
$modal-shadow: $shadow-2xl;

// 下拉菜单阴影
$dropdown-shadow: $shadow-lg;
```

## 7. 组件设计规范

### 7.1 按钮组件
```scss
// 按钮尺寸
.btn {
  &--small {
    padding: $spacing-2 $spacing-3;
    font-size: $font-size-caption;
    height: 2rem;
  }
  
  &--medium {
    padding: $spacing-3 $spacing-4;
    font-size: $font-size-button;
    height: 2.5rem;
  }
  
  &--large {
    padding: $spacing-4 $spacing-6;
    font-size: $font-size-body1;
    height: 3rem;
  }
}

// 按钮类型
.btn {
  &--primary {
    background-color: $primary-500;
    color: white;
    &:hover { background-color: $primary-600; }
  }
  
  &--secondary {
    background-color: transparent;
    color: $primary-500;
    border: 1px solid $primary-500;
    &:hover { background-color: $primary-50; }
  }
  
  &--text {
    background-color: transparent;
    color: $primary-500;
    &:hover { background-color: $primary-50; }
  }
}
```

### 7.2 输入框组件
```scss
.input {
  &--text {
    padding: $spacing-3 $spacing-4;
    border: 1px solid $border-light;
    border-radius: $input-border-radius;
    font-size: $font-size-body2;
    
    &:focus {
      border-color: $primary-500;
      box-shadow: 0 0 0 2px rgba($primary-500, 0.2);
    }
    
    &:disabled {
      background-color: $background-level1;
      color: $text-disabled;
    }
  }
}
```

### 7.3 卡片组件
```scss
.card {
  background-color: $background-paper;
  border-radius: $card-border-radius;
  box-shadow: $card-shadow;
  padding: $card-padding;
  
  &:hover {
    box-shadow: $card-shadow-hover;
  }
  
  &__header {
    padding-bottom: $spacing-4;
    border-bottom: 1px solid $border-light;
    margin-bottom: $spacing-4;
  }
  
  &__title {
    font-size: $font-size-h5;
    font-weight: 600;
    color: $text-primary;
  }
  
  &__content {
    color: $text-secondary;
  }
}
```

## 8. 图标规范

### 8.1 图标系统
- **图标库**: 使用 Heroicons、Feather Icons 或 Material Icons
- **图标尺寸**: 16px、20px、24px、32px、48px
- **图标颜色**: 继承文字颜色或使用主题色

### 8.2 文件类型图标
```scss
// 文件类型图标映射
$file-type-icons: (
  'pdf': 'document-text',
  'doc': 'document',
  'docx': 'document',
  'xls': 'table',
  'xlsx': 'table',
  'ppt': 'presentation-chart-bar',
  'pptx': 'presentation-chart-bar',
  'jpg': 'photograph',
  'jpeg': 'photograph',
  'png': 'photograph',
  'gif': 'photograph',
  'mp4': 'video-camera',
  'mov': 'video-camera',
  'mp3': 'music-note',
  'wav': 'music-note',
  'zip': 'archive',
  'rar': 'archive',
  'txt': 'document-text',
  'md': 'document-text'
);
```

## 9. 响应式设计

### 9.1 断点系统
```scss
// 断点定义
$breakpoint-sm: 640px;   // 小屏设备
$breakpoint-md: 768px;   // 平板设备
$breakpoint-lg: 1024px;  // 笔记本
$breakpoint-xl: 1280px;  // 桌面显示器
$breakpoint-2xl: 1536px; // 大屏显示器

// 媒体查询 Mixin
@mixin mobile-only {
  @media (max-width: #{$breakpoint-sm - 1px}) { @content; }
}

@mixin tablet-up {
  @media (min-width: $breakpoint-md) { @content; }
}

@mixin desktop-up {
  @media (min-width: $breakpoint-lg) { @content; }
}
```

### 9.2 布局适配
```scss
// 文件列表布局
.file-list {
  display: grid;
  gap: $spacing-4;
  
  // 移动端：单列
  grid-template-columns: 1fr;
  
  // 平板端：双列
  @include tablet-up {
    grid-template-columns: repeat(2, 1fr);
  }
  
  // 桌面端：多列
  @include desktop-up {
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  }
}

// 侧边栏适配
.sidebar {
  // 移动端：隐藏，通过菜单展开
  @include mobile-only {
    position: fixed;
    top: 0;
    left: -100%;
    transition: left 0.3s ease;
    
    &.is-open {
      left: 0;
    }
  }
  
  // 桌面端：固定显示
  @include desktop-up {
    position: relative;
    width: 250px;
  }
}
```

## 10. 交互设计规范

### 10.1 动画时长
```scss
// 动画时长
$duration-fast: 150ms;     // 快速动画
$duration-normal: 300ms;   // 正常动画
$duration-slow: 500ms;     // 慢速动画

// 缓动函数
$ease-in: cubic-bezier(0.4, 0, 1, 1);
$ease-out: cubic-bezier(0, 0, 0.2, 1);
$ease-in-out: cubic-bezier(0.4, 0, 0.2, 1);
```

### 10.2 状态反馈
```scss
// 悬停状态
.interactive {
  transition: all $duration-fast $ease-out;
  
  &:hover {
    transform: translateY(-1px);
    box-shadow: $shadow-lg;
  }
  
  &:active {
    transform: translateY(0);
  }
}

// 加载状态
.loading {
  position: relative;
  
  &::after {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 20px;
    height: 20px;
    margin: -10px 0 0 -10px;
    border: 2px solid $primary-200;
    border-top-color: $primary-500;
    border-radius: 50%;
    animation: spin 1s linear infinite;
  }
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
```

### 10.3 手势支持
```scss
// 移动端手势
.swipeable {
  touch-action: pan-y;
  
  &.swipe-left {
    transform: translateX(-100px);
    transition: transform $duration-normal $ease-out;
  }
  
  &.swipe-right {
    transform: translateX(100px);
    transition: transform $duration-normal $ease-out;
  }
}
```

## 11. 可访问性规范

### 11.1 颜色对比度
- 正常文字与背景对比度 ≥ 4.5:1
- 大文字与背景对比度 ≥ 3:1
- 非文字元素与背景对比度 ≥ 3:1

### 11.2 键盘导航
```scss
// 焦点样式
:focus {
  outline: 2px solid $primary-500;
  outline-offset: 2px;
}

// 跳过链接
.skip-link {
  position: absolute;
  top: -40px;
  left: 6px;
  background: $primary-500;
  color: white;
  padding: 8px;
  text-decoration: none;
  z-index: 1000;
  
  &:focus {
    top: 6px;
  }
}
```

### 11.3 屏幕阅读器支持
```html
<!-- 语义化HTML -->
<main role="main">
  <section aria-labelledby="files-heading">
    <h2 id="files-heading">文件列表</h2>
    <!-- 文件列表内容 -->
  </section>
</main>

<!-- ARIA属性 -->
<button aria-label="上传文件" aria-describedby="upload-help">
  <span class="icon">+</span>
</button>
<div id="upload-help" class="sr-only">
  支持拖拽上传或点击选择文件
</div>
```

## 12. 主题定制

### 12.1 CSS变量系统
```scss
:root {
  // 颜色变量
  --color-primary: #{$primary-500};
  --color-success: #{$success-500};
  --color-warning: #{$warning-500};
  --color-error: #{$error-500};
  
  // 文字变量
  --text-primary: #{$text-primary};
  --text-secondary: #{$text-secondary};
  
  // 背景变量
  --bg-default: #{$background-default};
  --bg-paper: #{$background-paper};
  
  // 间距变量
  --spacing-unit: 8px;
}

// 暗色主题
[data-theme="dark"] {
  --color-primary: #{$primary-400};
  --text-primary: #E0E0E0;
  --text-secondary: #BDBDBD;
  --bg-default: #121212;
  --bg-paper: #1E1E1E;
}
```

### 12.2 主题切换
```scss
.theme-toggle {
  background: none;
  border: 1px solid var(--text-secondary);
  border-radius: $border-radius-full;
  width: 40px;
  height: 40px;
  cursor: pointer;
  
  .icon {
    transition: transform $duration-normal $ease-in-out;
  }
  
  [data-theme="dark"] & .icon {
    transform: rotate(180deg);
  }
}
```

这套UI/UX设计规范确保了SquadFile在各个平台上的视觉一致性和用户体验的连贯性，同时支持主题定制和无障碍访问。