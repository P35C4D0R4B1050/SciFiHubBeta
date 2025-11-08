using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using FluentValidation;
using FluentValidation.AspNetCore;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Infrastructure.Data;
using SciFiHub.Infrastructure.Repositories;
using SciFiHub.Web.Services;
using SciFiHub.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =============================
// CONFIGURACIÓN DE SERVICIOS
// =============================

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuración de DbContext con SQL Server
builder.Services.AddDbContext<SciFiHubDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            // Deshabilitar EnableRetryOnFailure para evitar conflictos con transacciones manuales
            // sqlOptions.EnableRetryOnFailure(
            //     maxRetryCount: 5,
            //     maxRetryDelay: TimeSpan.FromSeconds(30),
            //     errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(60);
        });

    // Habilitar logging de queries en desarrollo
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Registro de repositorios (Scoped - una instancia por request HTTP)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<ICarritoCompraRepository, CarritoCompraRepository>();

// Registro del Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Registro de servicios de aplicación (COMPLETO)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IPdfService, PdfService>();

// Configuración de AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Configuración de FluentValidation (SIN validación automática)
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// NO usar: builder.Services.AddFluentValidationAutoValidation();

// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "SciFiHub.Auth";
    });

// Configuración de autorización con políticas por rol
builder.Services.AddAuthorization(options =>
{
    // Política para solo Administradores
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Administrador"));

    // Política para Administrador o Vendedor
    options.AddPolicy("AdminOrVendedor", policy =>
        policy.RequireRole("Administrador", "Vendedor"));

    // Política para cualquier usuario autenticado
    options.AddPolicy("ClienteRegistrado", policy =>
        policy.RequireAuthenticatedUser());

    // Política para acceso al área de ventas
    options.AddPolicy("AccesoVentas", policy =>
        policy.RequireRole("Administrador", "Vendedor"));

    // Política para gestión de usuarios
    options.AddPolicy("GestionUsuarios", policy =>
        policy.RequireRole("Administrador"));

    // Política para gestión del catálogo
    options.AddPolicy("GestionCatalogo", policy =>
        policy.RequireRole("Administrador", "Vendedor"));
});

// Configuración de sesiones (para carrito temporal de usuarios no autenticados)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "SciFiHub.Session";
});

// HttpContextAccessor para acceder al contexto HTTP en servicios
builder.Services.AddHttpContextAccessor();

// =============================
// CONFIGURACIÓN DEL PIPELINE
// =============================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/dotnet-support-policy.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

// Habilitar sesiones (ANTES de autenticación)
app.UseSession();

// Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Rutas adicionales específicas por área
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "vendedor",
    pattern: "Vendedor/{controller=Ventas}/{action=Index}/{id?}");

// =============================
// APLICAR MIGRACIONES Y SEED (SOLO EN DESARROLLO)
// =============================

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<SciFiHubDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Verificando estado de la base de datos...");

        // Verificar si puede conectarse a la BD
        if (await context.Database.CanConnectAsync())
        {
            logger.LogInformation("Conexión a la base de datos exitosa");

            // Aplicar migraciones pendientes
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Aplicando migraciones pendientes...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migraciones aplicadas exitosamente");
            }
            else
            {
                logger.LogInformation("No hay migraciones pendientes");
            }

            // Inicializar datos de prueba
            logger.LogInformation("Inicializando datos de prueba...");
            await SciFiHub.Web.Data.DbInitializer.InitializeAsync(context);
            logger.LogInformation("Datos de prueba inicializados correctamente");
        }
        else
        {
            logger.LogWarning("No se pudo conectar a la base de datos");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos");
    }
}

app.Run();
