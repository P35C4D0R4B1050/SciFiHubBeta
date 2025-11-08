using AutoMapper;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.ValueObjects;
using SciFiHub.Web.DTOs.Usuario;
using SciFiHub.Web.DTOs.Libro;
using SciFiHub.Web.DTOs.Venta;
using SciFiHub.Web.DTOs.Carrito;
using SciFiHub.Web.DTOs.Categoria;
using SciFiHub.Web.DTOs.Common;

namespace SciFiHub.Web.Mappings;

/// <summary>
/// Perfil de AutoMapper para mapeo entre entidades y DTOs
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // ===================================
        // MAPEOS DE USUARIO
        // ===================================
        CreateMap<Usuario, UsuarioDTO>()
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => 
                Enum.Parse<Domain.Enums.RolUsuario>(src.Rol)))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => 
                Enum.Parse<Domain.Enums.EstadoUsuario>(src.Estado)))
            .ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol))
            .ForMember(dest => dest.EstadoNombre, opt => opt.MapFrom(src => src.Estado));

        CreateMap<CrearUsuarioDTO, Usuario>()
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol.ToString()))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Activo"))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<ActualizarUsuarioDTO, Usuario>()
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol.ToString()))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.Username, opt => opt.Ignore()) // No se puede cambiar username
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // ===================================
        // MAPEOS DE LIBRO
        // ===================================
        CreateMap<Libro, LibroDTO>()
            .ForMember(dest => dest.AñoPublicacion, opt => opt.MapFrom(src => src.AnioPublicacion))
            .ForMember(dest => dest.Paginas, opt => opt.MapFrom(src => src.NumeroPaginas))
            .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : ""));

        CreateMap<Libro, LibroCardDTO>()
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
            .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : ""))
            .ForMember(dest => dest.Disponible, opt => opt.MapFrom(src => src.Stock > 0 && src.Estado == Domain.Enums.EstadoLibro.Disponible));

        CreateMap<CrearLibroDTO, Libro>()
            .ForMember(dest => dest.AnioPublicacion, opt => opt.MapFrom(src => src.AñoPublicacion))
            .ForMember(dest => dest.NumeroPaginas, opt => opt.MapFrom(src => src.Paginas))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<ActualizarLibroDTO, Libro>()
            .ForMember(dest => dest.AnioPublicacion, opt => opt.MapFrom(src => src.AñoPublicacion))
            .ForMember(dest => dest.NumeroPaginas, opt => opt.MapFrom(src => src.Paginas))
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // ===================================
        // MAPEOS DE VENTA
        // ===================================
        CreateMap<Venta, VentaDTO>()
            .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente.NombreCompleto))
            .ForMember(dest => dest.VendedorNombre, opt => opt.MapFrom(src => src.Vendedor != null ? src.Vendedor.NombreCompleto : null))
            .ForMember(dest => dest.DireccionEnvio, opt => opt.MapFrom(src => src.Direccion != null ? new DireccionEnvioDTO
            {
                Calle = src.DireccionCalle!,
                Ciudad = src.DireccionCiudad!,
                Departamento = src.DireccionDepartamento!,
                Pais = src.DireccionPais!,
                CodigoPostal = src.DireccionCodigoPostal,
                Referencia = src.DireccionReferencia
            } : null))
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));

        CreateMap<DetalleVenta, DetalleVentaMostrarDTO>()
            .ForMember(dest => dest.LibroISBN, opt => opt.MapFrom(src => src.Libro.ISBN))
            .ForMember(dest => dest.LibroTitulo, opt => opt.MapFrom(src => src.Libro.Titulo))
            .ForMember(dest => dest.LibroAutor, opt => opt.MapFrom(src => src.Libro.Autor))
            .ForMember(dest => dest.LibroImagen, opt => opt.MapFrom(src => src.Libro.ImagenPortada));

        CreateMap<CrearVentaDTO, Venta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroVenta, opt => opt.Ignore()) // Se genera automáticamente
            .ForMember(dest => dest.FechaVenta, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.EstadoVenta, opt => opt.MapFrom(src => Domain.Enums.EstadoVenta.Pendiente))
            .ForMember(dest => dest.DireccionCalle, opt => opt.MapFrom(src => src.DireccionEnvioString))
            .ForMember(dest => dest.DireccionCiudad, opt => opt.MapFrom(src => "Lima"))
            .ForMember(dest => dest.DireccionDepartamento, opt => opt.MapFrom(src => "Lima"))
            .ForMember(dest => dest.DireccionPais, opt => opt.MapFrom(src => "Perú"))
            .ForMember(dest => dest.DireccionCodigoPostal, opt => opt.Ignore())
            .ForMember(dest => dest.DireccionReferencia, opt => opt.Ignore())
            .ForMember(dest => dest.Detalles, opt => opt.Ignore()) // Se manejan por separado
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.Vendedor, opt => opt.Ignore())
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore())
            .ForMember(dest => dest.Descuento, opt => opt.Ignore())
            .ForMember(dest => dest.IGV, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<DetalleVentaDTO, DetalleVenta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.VentaId, opt => opt.Ignore())
            .ForMember(dest => dest.Venta, opt => opt.Ignore())
            .ForMember(dest => dest.Libro, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // ===================================
        // MAPEOS DE CARRITO
        // ===================================
        CreateMap<CarritoCompra, CarritoDTO>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Detalles));

        CreateMap<DetalleCarrito, ItemCarritoDTO>()
            .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Libro.Titulo))
            .ForMember(dest => dest.Autor, opt => opt.MapFrom(src => src.Libro.Autor))
            .ForMember(dest => dest.ImagenPortada, opt => opt.MapFrom(src => src.Libro.ImagenPortada))
            .ForMember(dest => dest.StockDisponible, opt => opt.MapFrom(src => src.Libro.Stock));

        // ===================================
        // MAPEOS DE CATEGORÍA
        // ===================================
        CreateMap<Categoria, CategoriaDTO>()
            .ForMember(dest => dest.CategoriaPadreNombre, opt => opt.MapFrom(src => src.CategoriaPadre != null ? src.CategoriaPadre.Nombre : null))
            .ForMember(dest => dest.CantidadLibros, opt => opt.MapFrom(src => src.Libros.Count))
            .ForMember(dest => dest.CantidadSubCategorias, opt => opt.MapFrom(src => src.SubCategorias.Count));

        CreateMap<Categoria, CategoriaSimpleDTO>();

        CreateMap<CrearCategoriaDTO, Categoria>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CategoriaPadre, opt => opt.Ignore())
            .ForMember(dest => dest.SubCategorias, opt => opt.Ignore())
            .ForMember(dest => dest.Libros, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<ActualizarCategoriaDTO, Categoria>()
            .ForMember(dest => dest.CategoriaPadre, opt => opt.Ignore())
            .ForMember(dest => dest.SubCategorias, opt => opt.Ignore())
            .ForMember(dest => dest.Libros, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
