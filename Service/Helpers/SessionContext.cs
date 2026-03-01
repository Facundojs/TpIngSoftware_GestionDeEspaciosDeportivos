using Service.DTO;

namespace Service.Helpers
{
    public static class SessionContext
    {
        public static UsuarioDTO CurrentUser { get; set; }
    }
}
