using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using bd2_first_api_202603.Data;
using bd2_first_api_202603.DTOs;
using bd2_first_api_202603.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace bd2_first_api_202603.Services;

public class AuthService : IAuthService
{
    private readonly TiendaDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(TiendaDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<int> RegisterAsync(RegisterDto dto)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var rol = "Cliente";

        var resultado = await _context.Database
            .SqlQuery<int>($"EXEC dbo.sp_Usuario_Registrar @Username = {dto.Username}, @Email = {dto.Email}, @PasswordHash = {hash}, @RolNombre = {rol}")
            .ToListAsync();

        return resultado.First();
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        // EXEC no es componible: se materializa primero y luego se toma el primero
        var usuario = (await _context.Set<Usuario>()
                .FromSql($"EXEC dbo.sp_Usuario_ObtenerPorUsername @Username = {dto.Username}")
                .AsNoTracking()
                .ToListAsync())
            .FirstOrDefault();

        // Mismo resultado (null) si no existe, está inactivo o la clave es incorrecta
        if (usuario is null || !usuario.Activo ||
            !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return null;

        var roles = await _context.Database
            .SqlQuery<string>($"EXEC dbo.sp_Usuario_ObtenerRoles @IdUsuario = {usuario.IdUsuario}")
            .ToListAsync();

        var (token, expira) = GenerarToken(usuario, roles);

        return new AuthResponseDto
        {
            Token = token,
            Expira = expira,
            Username = usuario.Username,
            Roles = roles
        };
    }

    public async Task AsignarRolAsync(AsignarRolDto dto)
    {
        await _context.Database.ExecuteSqlAsync(
            $"EXEC dbo.sp_Usuario_AsignarRol @IdUsuario = {dto.IdUsuario}, @RolNombre = {dto.Rol}");
    }

    private (string token, DateTime expira) GenerarToken(Usuario usuario, List<string> roles)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expira = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"]!));

        var claims = new List<Claim>
        {
            new("sub", usuario.IdUsuario.ToString()),
            new("name", usuario.Username),
            new("email", usuario.Email),
            new("jti", Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim("role", r)));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expira,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}