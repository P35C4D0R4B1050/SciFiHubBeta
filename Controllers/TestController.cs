using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciFiHub.Infrastructure.Data;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly SciFiHubDbContext _context;
    private readonly ILogger<TestController> _logger;

    public TestController(SciFiHubDbContext context, ILogger<TestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            database = "connected"
        });
    }

    [HttpGet("database-status")]
    public async Task<IActionResult> DatabaseStatus()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

            return Ok(new
            {
                canConnect,
                pendingMigrations = pendingMigrations.ToList(),
                appliedMigrations = appliedMigrations.ToList(),
                totalUsuarios = await _context.Usuarios.CountAsync(),
                totalLibros = await _context.Libros.CountAsync(),
                totalCategorias = await _context.Categorias.CountAsync(),
                totalVentas = await _context.Ventas.CountAsync()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database status");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("fix-user-passwords")]
    public async Task<IActionResult> FixUserPasswords()
    {
        try
        {
            var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            var vendedorHash = BCrypt.Net.BCrypt.HashPassword("Vendedor123!");
            var clienteHash = BCrypt.Net.BCrypt.HashPassword("Cliente123!");

            var updates = new List<string>();
            var usuarios = await _context.Usuarios.ToListAsync();

            foreach (var usuario in usuarios)
            {
                switch (usuario.Rol)
                {
                    case "Administrador":
                        usuario.PasswordHash = adminHash;
                        updates.Add($"Admin: {usuario.Username} - Password: Admin123!");
                        break;
                    case "Vendedor":
                        usuario.PasswordHash = vendedorHash;
                        updates.Add($"Vendedor: {usuario.Username} - Password: Vendedor123!");
                        break;
                    case "Cliente":
                        usuario.PasswordHash = clienteHash;
                        updates.Add($"Cliente: {usuario.Username} - Password: Cliente123!");
                        break;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Passwords actualizadas correctamente",
                updates
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fixing passwords");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("initialize-all-data")]
    public async Task<IActionResult> InitializeAllData()
    {
        try
        {
            _logger.LogInformation("=== INICIO DE INICIALIZACIÓN ===");
            var steps = new List<string>();

            // PASO 1: Limpiar datos existentes
            steps.Add("1. Limpiando datos existentes...");
            _logger.LogInformation("Paso 1: Limpiando datos...");
            
            try
            {
                _context.DetallesCarrito.RemoveRange(_context.DetallesCarrito);
                _context.CarritoCompras.RemoveRange(_context.CarritoCompras);
                _context.DetallesVenta.RemoveRange(_context.DetallesVenta);
                _context.Ventas.RemoveRange(_context.Ventas);
                _context.AuditoriasInventario.RemoveRange(_context.AuditoriasInventario);
                _context.Libros.RemoveRange(_context.Libros);
                _context.Categorias.RemoveRange(_context.Categorias);
                _context.Usuarios.RemoveRange(_context.Usuarios);
                
                var deletedCount = await _context.SaveChangesAsync();
                steps.Add($"   OK - {deletedCount} registros eliminados");
                _logger.LogInformation($"Datos eliminados: {deletedCount} registros");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar datos");
                throw new Exception($"Error al limpiar datos: {ex.Message}", ex);
            }

            // PASO 2: Crear Usuarios
            steps.Add("2. Creando usuarios...");
            _logger.LogInformation("Paso 2: Creando usuarios...");
            
            try
            {
                var usuarios = new List<Domain.Entities.Usuario>
                {
                    new Domain.Entities.Usuario
                    {
                        Username = "admin",
                        Email = "admin@scifihub.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        NombreCompleto = "Administrador del Sistema",
                        Telefono = "+51 999 888 777",
                        Rol = "Administrador",
                        Estado = "Activo",
                        FechaRegistro = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Domain.Entities.Usuario
                    {
                        Username = "vendedor1",
                        Email = "vendedor1@scifihub.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vendedor123!"),
                        NombreCompleto = "Maria Torres Garcia",
                        Telefono = "+51 999 777 666",
                        Rol = "Vendedor",
                        Estado = "Activo",
                        FechaRegistro = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Domain.Entities.Usuario
                    {
                        Username = "cliente1",
                        Email = "cliente1@scifihub.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente123!"),
                        NombreCompleto = "Juan Perez Lopez",
                        Telefono = "+51 999 666 555",
                        Rol = "Cliente",
                        Estado = "Activo",
                        FechaRegistro = DateTime.UtcNow,
                        IsDeleted = false
                    }
                };
                
                await _context.Usuarios.AddRangeAsync(usuarios);
                await _context.SaveChangesAsync();
                steps.Add($"   OK - {usuarios.Count} usuarios creados");
                _logger.LogInformation($"Usuarios creados: {usuarios.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuarios");
                throw new Exception($"Error al crear usuarios: {ex.Message}", ex);
            }

            // PASO 3: Crear Categorías
            steps.Add("3. Creando categorias...");
            _logger.LogInformation("Paso 3: Creando categorías...");
            
            try
            {
                var categorias = new List<Domain.Entities.Categoria>
                {
                    new Domain.Entities.Categoria
                    {
                        Nombre = "Ciencia Ficcion",
                        Descripcion = "Novelas de ciencia ficcion",
                        Estado = "Activo",
                        Orden = 1,
                        IsDeleted = false
                    },
                    new Domain.Entities.Categoria
                    {
                        Nombre = "Fantasia",
                        Descripcion = "Mundos fantasticos",
                        Estado = "Activo",
                        Orden = 2,
                        IsDeleted = false
                    }
                };
                
                await _context.Categorias.AddRangeAsync(categorias);
                await _context.SaveChangesAsync();
                steps.Add($"   OK - {categorias.Count} categorias creadas");
                _logger.LogInformation($"Categorías creadas: {categorias.Count}");

                // PASO 4: Crear Libros
                steps.Add("4. Creando libros...");
                _logger.LogInformation("Paso 4: Creando libros...");
                
                var libros = new List<Domain.Entities.Libro>
                {
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-441-17271-9",
                        Titulo = "Dune",
                        Autor = "Frank Herbert",
                        Editorial = "Ace Books",
                        CategoriaId = categorias[0].Id,
                        AnioPublicacion = 1965,
                        Precio = 29.99m,
                        Stock = 50,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-345-39180-3",
                        Titulo = "Fundacion",
                        Autor = "Isaac Asimov",
                        Editorial = "Bantam",
                        CategoriaId = categorias[0].Id,
                        AnioPublicacion = 1951,
                        Precio = 24.99m,
                        PrecioOferta = 19.99m,
                        Stock = 35,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-10),
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-316-01984-0",
                        Titulo = "El Juego de Ender",
                        Autor = "Orson Scott Card",
                        Editorial = "Tor Books",
                        CategoriaId = categorias[0].Id,
                        AnioPublicacion = 1985,
                        Precio = 26.99m,
                        Stock = 40,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-3),
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-553-38034-1",
                        Titulo = "1984",
                        Autor = "George Orwell",
                        Editorial = "Signet Classic",
                        CategoriaId = categorias[0].Id,
                        AnioPublicacion = 1949,
                        Precio = 19.99m,
                        Stock = 60,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-20),
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-544-33641-9",
                        Titulo = "Un mundo feliz",
                        Autor = "Aldous Huxley",
                        Editorial = "Harper",
                        CategoriaId = categorias[0].Id,
                        AnioPublicacion = 1932,
                        Precio = 18.99m,
                        PrecioOferta = 14.99m,
                        Stock = 45,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-15),
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-441-56959-4",
                        Titulo = "Neuromante",
                        Autor = "William Gibson",
                        Editorial = "Ace Books",
                        CategoriaId = categorias[1].Id,
                        AnioPublicacion = 1984,
                        Precio = 22.99m,
                        Stock = 28,
                        Destacado = false,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-25),
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-575-09406-6",
                        Titulo = "Blade Runner",
                        Autor = "Philip K. Dick",
                        Editorial = "Gollancz",
                        CategoriaId = categorias[1].Id,
                        AnioPublicacion = 1968,
                        Precio = 21.99m,
                        PrecioOferta = 17.99m,
                        Stock = 32,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-12),
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-345-41943-6",
                        Titulo = "El Nombre del Viento",
                        Autor = "Patrick Rothfuss",
                        Editorial = "DAW Books",
                        CategoriaId = categorias[1].Id,
                        AnioPublicacion = 2007,
                        Precio = 34.99m,
                        PrecioOferta = 27.99m,
                        Stock = 45,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-5),
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-765-31178-5",
                        Titulo = "El Imperio Final",
                        Autor = "Brandon Sanderson",
                        Editorial = "Tor Books",
                        CategoriaId = categorias[1].Id,
                        AnioPublicacion = 2006,
                        Precio = 32.99m,
                        Stock = 38,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-8),
                        IsDeleted = false
                    },
                    new Domain.Entities.Libro
                    {
                        ISBN = "978-0-316-33836-3",
                        Titulo = "Leviathan Wakes",
                        Autor = "James S.A. Corey",
                        Editorial = "Orbit",
                        CategoriaId = categorias[1].Id,
                        AnioPublicacion = 2011,
                        Precio = 28.99m,
                        Stock = 30,
                        Destacado = true,
                        Estado = Domain.Enums.EstadoLibro.Disponible,
                        FechaIngreso = DateTime.UtcNow.AddDays(-2),
                        IsDeleted = false
                    }
                };
                
                await _context.Libros.AddRangeAsync(libros);
                await _context.SaveChangesAsync();
                steps.Add($"   OK - {libros.Count} libros creados");
                _logger.LogInformation($"Libros creados: {libros.Count}");

                // Verificación
                var verificacion = new
                {
                    usuarios = await _context.Usuarios.CountAsync(),
                    categorias = await _context.Categorias.CountAsync(),
                    libros = await _context.Libros.CountAsync()
                };

                _logger.LogInformation($"=== INICIALIZACIÓN COMPLETADA === Usuarios: {verificacion.usuarios}, Categorías: {verificacion.categorias}, Libros: {verificacion.libros}");

                return Ok(new
                {
                    success = true,
                    message = "Inicializacion completada",
                    steps,
                    data = verificacion,
                    credentials = new
                    {
                        admin = new { username = "admin", password = "Admin123!" },
                        vendedor = new { username = "vendedor1", password = "Vendedor123!" },
                        cliente = new { username = "cliente1", password = "Cliente123!" }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear libros");
                throw new Exception($"Error al crear libros: {ex.Message}", ex);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error GENERAL en InitializeAllData");
            return StatusCode(500, new 
            { 
                success = false,
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                stackTrace = ex.StackTrace
            });
        }
    }

    [HttpGet("diagnose-catalog")]
    public async Task<IActionResult> DiagnoseCatalog()
    {
        try
        {
            var todosLosLibros = await _context.Libros
                .IgnoreQueryFilters()
                .Select(l => new
                {
                    l.Id,
                    l.Titulo,
                    l.Stock,
                    l.IsDeleted
                })
                .ToListAsync();

            var librosVisibles = await _context.Libros.CountAsync();

            return Ok(new
            {
                totalLibrosEnBD = todosLosLibros.Count,
                librosVisibles,
                librosConIsDeletedTrue = todosLosLibros.Count(l => l.IsDeleted == true),
                librosConIsDeletedFalse = todosLosLibros.Count(l => l.IsDeleted == false),
                todosLosLibros = todosLosLibros.Take(10)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
