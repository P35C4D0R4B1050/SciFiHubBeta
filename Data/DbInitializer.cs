using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;
using SciFiHub.Infrastructure.Data;

namespace SciFiHub.Web.Data;

/// <summary>
/// Clase para inicializar datos de prueba en la base de datos
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(SciFiHubDbContext context)
    {
        // Asegurar que la base de datos existe
        await context.Database.EnsureCreatedAsync();

        // Si ya hay usuarios, no hacer nada
        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        // Crear usuario administrador
        var adminUser = new Usuario
        {
            Username = "admin",
            Email = "admin@scifihub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            NombreCompleto = "Administrador del Sistema",
            Telefono = "+1 234 567 8900",
            Rol = "Administrador",
            Estado = "Activo",
            FechaRegistro = DateTime.UtcNow
        };

        // Crear usuario vendedor
        var vendedorUser = new Usuario
        {
            Username = "vendedor1",
            Email = "vendedor1@scifihub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vendedor123!"),
            NombreCompleto = "Vendedor de Prueba",
            Telefono = "+1 234 567 8901",
            Rol = "Vendedor",
            Estado = "Activo",
            FechaRegistro = DateTime.UtcNow
        };

        // Crear usuario cliente
        var clienteUser = new Usuario
        {
            Username = "cliente1",
            Email = "cliente1@scifihub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente123!"),
            NombreCompleto = "Cliente de Prueba",
            Telefono = "+1 234 567 8902",
            Rol = "Cliente",
            Estado = "Activo",
            FechaRegistro = DateTime.UtcNow
        };

        context.Usuarios.AddRange(adminUser, vendedorUser, clienteUser);
        await context.SaveChangesAsync();

        // Crear categorías
        var categoriaCienciaFiccion = new Categoria
        {
            Nombre = "Ciencia Ficción",
            Descripcion = "Libros de ciencia ficción y futurismo",
            Estado = "Activo",
            Orden = 1
        };

        var categoriaFantasia = new Categoria
        {
            Nombre = "Fantasía",
            Descripcion = "Libros de fantasía épica y aventuras",
            Estado = "Activo",
            Orden = 2
        };

        var categoriaDistopia = new Categoria
        {
            Nombre = "Distopía",
            Descripcion = "Futuros oscuros y sociedades distópicas",
            Estado = "Activo",
            Orden = 3,
            CategoriaPadreId = categoriaCienciaFiccion.Id
        };

        context.Categorias.AddRange(categoriaCienciaFiccion, categoriaFantasia, categoriaDistopia);
        await context.SaveChangesAsync();

        // Crear libros de ejemplo
        var libros = new List<Libro>
        {
            new Libro
            {
                ISBN = "978-0-441-17271-9",
                Titulo = "Dune",
                Autor = "Frank Herbert",
                Editorial = "Ace Books",
                CategoriaId = categoriaCienciaFiccion.Id,
                AnioPublicacion = 1965,
                NumeroPaginas = 688,
                Idioma = "Español",
                Precio = 29.99m,
                Stock = 50,
                ImagenPortada = "https://upload.wikimedia.org/wikipedia/en/d/de/Dune-Frank_Herbert_%281965%29_First_edition.jpg",
                Sinopsis = "Una obra maestra de la ciencia ficción que narra la historia de Paul Atreides en el planeta desértico Arrakis.",
                Descripcion = "Considerada una de las mejores novelas de ciencia ficción de todos los tiempos.",
                Destacado = true,
                Estado = EstadoLibro.Disponible,
                FechaIngreso = DateTime.UtcNow
            },
            new Libro
            {
                ISBN = "978-0-345-39180-3",
                Titulo = "Fundación",
                Autor = "Isaac Asimov",
                Editorial = "Bantam Spectra",
                CategoriaId = categoriaCienciaFiccion.Id,
                AnioPublicacion = 1951,
                NumeroPaginas = 255,
                Idioma = "Español",
                Precio = 24.99m,
                PrecioOferta = 19.99m,
                Stock = 35,
                ImagenPortada = "https://upload.wikimedia.org/wikipedia/en/4/42/Foundation_gnome.jpg",
                Sinopsis = "La saga que narra el declive del Imperio Galáctico y el nacimiento de la Fundación.",
                Descripcion = "Primera parte de la legendaria trilogía de la Fundación.",
                Destacado = true,
                Estado = EstadoLibro.Disponible,
                FechaIngreso = DateTime.UtcNow.AddDays(-10)
            },
            new Libro
            {
                ISBN = "978-0-553-38034-1",
                Titulo = "1984",
                Autor = "George Orwell",
                Editorial = "Signet Classic",
                CategoriaId = categoriaDistopia.Id,
                AnioPublicacion = 1949,
                NumeroPaginas = 328,
                Idioma = "Español",
                Precio = 19.99m,
                Stock = 60,
                ImagenPortada = "https://upload.wikimedia.org/wikipedia/commons/c/c3/1984first.jpg",
                Sinopsis = "Una visión distópica de un futuro totalitario donde el Gran Hermano todo lo ve.",
                Descripcion = "Un clásico que ha definido el género distópico.",
                Destacado = true,
                Estado = EstadoLibro.Disponible,
                FechaIngreso = DateTime.UtcNow.AddDays(-20)
            },
            new Libro
            {
                ISBN = "978-0-345-41943-6",
                Titulo = "El Nombre del Viento",
                Autor = "Patrick Rothfuss",
                Editorial = "DAW Books",
                CategoriaId = categoriaFantasia.Id,
                AnioPublicacion = 2007,
                NumeroPaginas = 662,
                Idioma = "Español",
                Precio = 34.99m,
                PrecioOferta = 27.99m,
                Stock = 45,
                ImagenPortada = "https://upload.wikimedia.org/wikipedia/en/5/56/TheNameoftheWind_cover.jpg",
                Sinopsis = "La historia de Kvothe, un héroe legendario que narra su propia historia.",
                Descripcion = "Primera parte de la saga Crónica del Asesino de Reyes.",
                Destacado = true,
                Estado = EstadoLibro.Disponible,
                FechaIngreso = DateTime.UtcNow.AddDays(-5)
            },
            new Libro
            {
                ISBN = "978-0-7653-0633-4",
                Titulo = "Neuromante",
                Autor = "William Gibson",
                Editorial = "Ace Books",
                CategoriaId = categoriaCienciaFiccion.Id,
                AnioPublicacion = 1984,
                NumeroPaginas = 271,
                Idioma = "Español",
                Precio = 22.99m,
                Stock = 28,
                ImagenPortada = "https://upload.wikimedia.org/wikipedia/en/4/4b/Neuromancer_%28Book%29.jpg",
                Sinopsis = "La novela que definió el género cyberpunk.",
                Descripcion = "Ganadora del premio Hugo, Nébula y Philip K. Dick.",
                Destacado = false,
                Estado = EstadoLibro.Disponible,
                FechaIngreso = DateTime.UtcNow.AddDays(-15)
            },
            new Libro
            {
                ISBN = "978-0-316-01984-0",
                Titulo = "El Juego de Ender",
                Autor = "Orson Scott Card",
                Editorial = "Tor Books",
                CategoriaId = categoriaCienciaFiccion.Id,
                AnioPublicacion = 1985,
                NumeroPaginas = 324,
                Idioma = "Español",
                Precio = 26.99m,
                Stock = 40,
                ImagenPortada = "https://upload.wikimedia.org/wikipedia/en/e/e4/Ender%27s_game_cover_ISBN_0312932081.jpg",
                Sinopsis = "Ender Wiggin, un niño genio entrenado para salvar a la humanidad.",
                Descripcion = "Ganadora de los premios Hugo y Nébula.",
                Destacado = true,
                Estado = EstadoLibro.Disponible,
                FechaIngreso = DateTime.UtcNow.AddDays(-3)
            }
        };

        context.Libros.AddRange(libros);
        await context.SaveChangesAsync();
    }
}
