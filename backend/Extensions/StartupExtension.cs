using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SquadFile.Data;
using SquadFile.Resources;
using SquadFile.Security;
using SquadFile.Services;
using System.Text;

namespace SquadFile.Extensions
{
    public static class StartupExtension
    {
        /// <summary>
        /// 初始化存储路径
        /// </summary>
        public static WebApplicationBuilder InitializeUploadPath(this WebApplicationBuilder builder)
        {
            var uploads = builder.Configuration["Uploads"];
            // 确保存储目录存在
            if (string.IsNullOrEmpty(uploads))
            {
                uploads = "uploads";
                builder.Configuration["Uploads"] = uploads;
            }

            // 获取应用程序根目录
            var appRoot = Directory.GetCurrentDirectory();
            // 构建完整的存储路径
            var fullPath = Path.Combine(appRoot, uploads);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return builder;
        }

        /// <summary>
        /// 添加自定义注入服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<LocalizationService>();
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<SystemSettingsService>();
            services.AddScoped<CaptchaService>();
            services.AddScoped<FileManagementService>();
            services.AddScoped<ShareService>();
            services.AddScoped<LogService>();
            services.AddSingleton<TemporaryDownloadTokenService>();
            services.Configure<JwtHelper>(configuration.GetSection("Jwt"));
            return services;
        }

        /// <summary>
        /// 添加自定义Jwt配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static IServiceCollection AddCustomJwt(this IServiceCollection services, IConfiguration configuration)
        {
            JwtHelper jwtHelper = new();
            configuration.Bind("Jwt", jwtHelper);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtHelper.Issuer,
                            ValidAudience = jwtHelper.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtHelper.Key ?? throw new KeyNotFoundException("Jwt:Key")))
                        };
                    });

            // 添加授权策略
            services.AddAuthorization(options =>
            {
                // 添加管理员策略，要求用户具有Admin角色
                options.AddPolicy("admin", policy => policy.RequireRole("Admin"));
            });

            return services;
        }

        /// <summary>
        /// 添加自定义本地化配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllers()
                 .AddDataAnnotationsLocalization(options =>
                 {
                     options.DataAnnotationLocalizerProvider = (type, factory) =>
                         factory.Create(typeof(SharedResource));
                 });

            // 配置支持的语言
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "zh-Hans", "en-US" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });

            return services;
        }

        /// <summary>
        /// 添加自定义Swagger配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SquadFile API", Version = "v1" });

                // 添加JWT认证支持
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }
    }
}
