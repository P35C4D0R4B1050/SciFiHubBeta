using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad Usuario
/// </summary>
public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<Usuario>> GetClientesActivosAsync(CancellationToken cancellationToken = default);
}
