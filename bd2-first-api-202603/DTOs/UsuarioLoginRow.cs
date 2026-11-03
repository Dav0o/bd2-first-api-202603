namespace bd2_first_api_202603.DTOs
{
    public class UsuarioLoginRow
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
