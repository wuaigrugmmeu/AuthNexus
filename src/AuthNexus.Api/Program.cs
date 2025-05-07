using AuthNexus.Application;
using AuthNexus.Infrastructure;
using AuthNexus.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 添加应用层服务
builder.Services.AddApplicationServices();

// 添加基础设施层服务
builder.Services.AddInfrastructureServices(builder.Configuration);

// 添加控制器
builder.Services.AddControllers();

// 添加Swagger/OpenAPI文档
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthNexus API",
        Version = "v1",
        Description = "权限管理系统API",
        Contact = new OpenApiContact
        {
            Name = "AuthNexus Team"
        }
    });
    
    // 添加JWT认证设置
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
            Array.Empty<string>()
        }
    });
});

// 配置JWT认证
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };
});

// 添加授权
builder.Services.AddAuthorization();

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

app.UseHttpsRedirection();

// 添加认证和授权中间件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 初始化数据库并生成测试数据
await app.Services.InitializeDatabaseAsync();

app.Run();
