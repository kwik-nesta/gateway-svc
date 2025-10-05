namespace KwikNesta.Gateway.Svc.Application.Commands.Identity
{
    public record LoginCommand
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
