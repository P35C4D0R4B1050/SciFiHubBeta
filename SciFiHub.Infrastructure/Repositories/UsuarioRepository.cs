using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Infrastructure.Data;

namespace SciFiHub.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de Usuario
/// </summary>
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(SciFiHubDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<Usuario?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol, CancellationToken cancellationToken = default)
    {
        string rolString = rol.ToString();
        
        return await _dbSet
            .Where(u => u.Rol == rolString)
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<IEnumerable<Usuario>> GetClientesActivosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.Rol == "Cliente" && u.Estado == "Activo")
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync(cancellationToken);
    }
}
