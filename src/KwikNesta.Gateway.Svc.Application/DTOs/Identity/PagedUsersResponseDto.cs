namespace KwikNesta.Gateway.Svc.Application.DTOs.Identity
{
    public class PagedUsersResponseDto
    {
        public PageMetaDto Meta { get; set; } = new();
        public List<CurrentUserDto> Users { get; set; } = [];
    }
}
