# SquadFile 网盘项目

## 项目结构

### 后端 (.NET 9.0)

```
SquadFile/
├── Program.cs                  # 程序入口点
├── appsettings.json           # 配置文件
├── Controllers/               # 控制器
│   ├── BaseController.cs     # 基类控制器
│   ├── AuthController.cs     # 认证相关接口
│   ├── UserController.cs     # 用户管理接口
│   └── SystemController.cs   # 系统设置接口
├── Services/                 # 业务服务
│   ├── AuthService.cs        # 认证服务
│   ├── UserService.cs        # 用户服务
│   └── SystemSettingsService.cs # 系统设置服务
├── Models/                   # 数据模型
│   ├── SysUser.cs            # 用户实体
│   ├── SystemSettings.cs     # 系统设置实体
│   └── DTOs/                 # 数据传输对象
│       ├── ChangePasswordRequest.cs
│       ├── CreateUserRequest.cs
│       ├── LoginRequest.cs
│       ├── LoginResponse.cs
│       ├── RefreshTokenRequest.cs
│       ├── ResetPasswordRequest.cs
│       └── LanguageSettingsDto.cs # 语言设置DTO
├── Data/                     # 数据访问
│   ├── ApplicationDbContext.cs # 数据库上下文
│   └── Migrations/           # 数据库迁移文件
├── Security/                 # 安全相关
│   ├── JwtHelper.cs          # JWT令牌生成与验证
│   └── PasswordHasher.cs     # 密码加密处理
├── Properties/               # 项目属性
│   └── launchSettings.json
├── wwwroot/                  # 静态文件
└── SquadFile.csproj          # 项目文件
```

### 前端 (Uniapp + Vue3)

```
frontend/
├── package.json               # 包管理文件
├── vite.config.js            # Vite配置
├── manifest.json             # 应用配置
├── pages.json                # 页面配置
├── App.vue                   # 应用根组件
├── main.js                   # 应用入口
├── src/
│   ├── pages/                # 页面目录
│   │   └── auth/            # 认证相关页面
│   │       └── login.vue    # 登录页面
│   │   └── home.vue         # 首页
│   ├── components/           # 组件目录
│   │   └── auth/            # 认证相关组件
│   │       └── LoginForm.vue # 登录表单组件
│   ├── stores/               # 状态管理(Pinia)
│   │   └── auth.js          # 认证状态管理
│   ├── services/             # API服务
│   │   ├── auth.js          # 认证相关API
│   │   └── request.js       # HTTP请求封装
│   ├── utils/                # 工具函数
│   │   └── crypto.js        # 加密工具
│   ├── locales/              # 国际化语言包
│   │   ├── zh-Hans.json      # 中文语言包
│   │   └── en-US.json       # 英文语言包
│   ├── plugins/              # 插件
│   │   └── i18n.js          # 国际化插件配置
│   └── styles/               # 样式文件
│       └── auth.css          # 认证相关样式
├── static/                   # 静态文件
└── dist/                     # 构建输出
```

## 运行项目

### 后端

1. 确保已安装 .NET 9.0 SDK
2. 在 `SquadFile` 目录下运行以下命令：

```bash
dotnet build
dotnet run
```

3. 后端服务将在 `http://localhost:8169` 启动

4. API文档可通过 `http://localhost:8169/swagger` 访问

### 前端

1. 确保已安装 Node.js 和 npm
2. 在 `frontend` 目录下运行以下命令安装依赖：

```bash
npm install
```

3. 启动开发服务器：

```bash
npm run dev
```

4. 前端服务将在 `http://localhost:8168` 启动

## API 接口

### 认证相关接口

- `POST /squadfile-api/auth/login` - 用户登录
- `POST /squadfile-api/auth/refresh` - 刷新令牌
- `POST /squadfile-api/auth/logout` - 用户登出
- `PUT /squadfile-api/auth/password` - 修改密码

### 用户管理接口

- `POST /squadfile-api/users` - 创建用户（管理员）
- `POST /squadfile-api/users/{userId}/reset-password` - 重置用户密码（管理员）

### 系统设置接口

- `GET /squadfile-api/system/language-settings` - 获取系统语言设置
- `PUT /squadfile-api/system/language-settings` - 更新系统默认语言设置
- `PUT /squadfile-api/users/{userId}/language` - 更新用户语言偏好

## 默认管理员账号

- 用户名: `admin`
- 密码: `admin123`

> 注意：首次登录后请立即修改密码