using Bookstore.API.Middlewares;
using Bookstore.Application.Interfaces;
using Bookstore.Application.Services;
using Bookstore.Domain.Interfaces;
using Bookstore.Infrastructure.Authentication;
using Bookstore.Infrastructure.Data;
using Bookstore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Setup básico do Serilog jogando pro console
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Conexão com o Postgres
// TODO: A connection string tá hardcoded pra rodar o teste fácil, mas temos que mover pro appsettings.json (ou var de ambiente) antes de subir pra prod!
var connectionString = "Host=localhost;Port=5432;Database=BookstoreDb;Username=postgres;Password=admin123";
builder.Services.AddDbContext<BookstoreDbContext>(options =>
    options.UseNpgsql(connectionString));

// Resolvendo a Injeção de Dependências (DI)
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Config do JWT
// TODO: Essa key gigante tem que sair daqui e ir pro cofre de senhas
var secretKey = "SuperSecretKeyForBookstoreApiThatMustBeVeryLong!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = false, // TODO: Validar o domínio certinho quando for pra prod
            ValidateAudience = false
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger maroto com suporte pra injetar o token JWT na interface
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bookstore API", Version = "v1" });

    // Puxando os XML comments pra documentar os endpoints na tela
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);

    // Setup do botãozinho "Authorize" no header
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu_token}",
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- Pipeline HTTP (Atenção com a ordem dos middlewares aqui!) ---

// Nosso tratador de erros global pra não vazar exception feia (stack trace) pro client
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Autenticação sempre ANTES da Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();