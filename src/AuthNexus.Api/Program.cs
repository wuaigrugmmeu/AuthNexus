using AuthNexus.Api.Extensions;
using AuthNexus.Api.Filters;
using AuthNexus.Application;
using AuthNexus.Infrastructure;
using AuthNexus.Infrastructure.Data;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// 添加应用层服务
builder.Services.AddApplicationServices();

// 添加基础设施层服务
builder.Services.AddInfrastructureServices(builder.Configuration);

// 添加数据库初始化服务
builder.Services.AddDbInitializer();

// 添加JWT认证
builder.Services.AddJwtAuthentication(builder.Configuration);

// 添加授权
builder.Services.AddAuthorization();

// 添加控制器，并配置全局过滤器
builder.Services.AddControllers(options => 
{
    // 添加模型验证过滤器
    options.Filters.Add<ModelValidationFilter>();
});

// 添加日志
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 添加Swagger配置
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthNexus API v1");
        // 设置Swagger UI为根路径
        c.RoutePrefix = string.Empty;
    });
}

// 添加重定向，将根路径访问重定向到Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// 使用自定义中间件
app.UseCustomMiddlewares();

app.UseHttpsRedirection();

// 添加认证和授权中间件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 初始化数据库并生成测试数据
await app.Services.InitializeDatabaseAsync();

app.Run();
