using KwikNesta.Contracts.Enums;

namespace KwikNesta.Gateway.Svc.API.DTOs
{
    public class AuditLogPageQuery : PagedDataQuery
    {
        private DateTime _minTime = DateTime.MinValue;
        private DateTime _maxTime = DateTime.MaxValue;

        public AuditAction? Action { get; set; }
        public AuditDomain? Domain { get; set; }
        public DateTime StartTime 
        {
            get
            {
                return _minTime;
            }
            set
            {
                _minTime = _minTime > value || value > EndTime ? _minTime : value;
            }
        }
        public DateTime EndTime 
        {
            get
            {
                return _maxTime;
            }
            set
            {
                _maxTime = _maxTime < value || value < StartTime ? _maxTime : value;
            } 
        }
    }
}
