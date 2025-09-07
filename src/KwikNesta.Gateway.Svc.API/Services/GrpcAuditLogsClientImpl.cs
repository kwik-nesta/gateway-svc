using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Extensions;
using KwikNesta.Gateway.Svc.API.DTOs;
using KwikNesta.Gateway.Svc.API.Services.Interfaces;
using KwikNesta.SystemSupport.Svc.Contracts;

namespace KwikNesta.Gateway.Svc.API.Services
{
    public class GrpcAuditLogsClientImpl : IGrpcAuditLogsClientImpl
    {
        private readonly AuditLogsService.AuditLogsServiceClient _logsServiceClient;

        public GrpcAuditLogsClientImpl(AuditLogsService.AuditLogsServiceClient logsServiceClient)
        {
            _logsServiceClient = logsServiceClient;
        }

        public async Task<AuditLogsPagedResponse> GetAuditLogsAsync(AuditLogPageQuery query, Metadata meta)
        {
            var requestQuery = new GetAuditLogsListRequest
            {
                Page = query.Page,
                Size = query.PageSize,
                Search = query.Search,
                StartTime = ToRpcTime(query.StartTime),
                EndTime = ToRpcTime(query.EndTime)
            };

            if (query.Domain.HasValue)
            {
                requestQuery.Domain = EnumMapper.Map<AuditDomain, GrpcAuditLogDomain>(query.Domain.Value);
            }
            if (query.Action.HasValue)
            {
                requestQuery.Action = EnumMapper.Map<AuditAction, GrpcAuditLogAction>(query.Action.Value);
            }

            var response = await _logsServiceClient
                .GetAuditLogsListAsync(requestQuery, meta);

            return new AuditLogsPagedResponse
            {
                PageMetaData = response.MetaData,
                AuditLogs = ToAuditLogs(response.AuditLogs)
            };
        }

        private static List<AuditLogsResponse> ToAuditLogs(RepeatedField<GrpcAuditLog> grpcAuditLogs)
        {
            var items = new List<AuditLogsResponse>();

            foreach (var item in grpcAuditLogs)
            {
                items.Add(ToAuditLog(item));
            }

            return items;
        }

        private static AuditLogsResponse ToAuditLog(GrpcAuditLog grpcAuditLog)
        {
            return new AuditLogsResponse
            {
                Action = grpcAuditLog.Action,
                Domain = grpcAuditLog.Domain,
                PerformedBy = grpcAuditLog.PerformedBy,
                PerformedOn = grpcAuditLog.PerformedOn,
                TimeStamp = ToDateTime(grpcAuditLog.TimeStamp)
            };
        }

        private static Timestamp ToRpcTime(DateTime timestamp)
        {
            return Timestamp.FromDateTime(new DateTime(timestamp.Ticks, DateTimeKind.Utc));
        }

        private static DateTime ToDateTime(Timestamp timestamp)
        {
            return timestamp.ToDateTime();
        }
    }
}
