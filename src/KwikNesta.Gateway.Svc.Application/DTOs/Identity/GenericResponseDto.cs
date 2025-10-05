namespace KwikNesta.Gateway.Svc.Application.DTOs.Identity
{
    public record GenericResponseDto(int Status, string Message, bool Successful = true);
}
