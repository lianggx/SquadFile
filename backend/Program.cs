using Microsoft.Extensions.FileProviders;
using SquadFile.Data;
using SquadFile.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 添加日志配置
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 初始化上传路径
builder.InitializeUploadPath();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCustomService(builder.Configuration);

// 添加JWT认证
builder.Services.AddCustomJwt(builder.Configuration);

// 添加内存缓存（用于Session）
builder.Services.AddDistributedMemoryCache();

// 添加HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 添加本地化服务
builder.Services.AddCustomLocalization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 使用请求日志中间件
app.UseRequestLoggingMiddleware();

// 使用全局异常处理中间件
app.UseGlobalExceptionMiddleware();

// 使用请求本地化
app.UseRequestLocalization();

// 添加认证中间件
app.UseAuthentication();

app.UseAuthorization();

// 添加专门用于文件访问的静态文件中间件，设置固定的URL前缀
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, builder.Configuration["Uploads"])),
    RequestPath = "/files"
});

app.MapControllers();

app.Run();