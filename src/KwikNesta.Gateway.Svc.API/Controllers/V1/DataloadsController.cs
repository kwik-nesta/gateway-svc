using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using CSharpTypes.Extensions.Guid;
using KwikNesta.Contracts.Commands;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KwikNesta.Gateway.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/dataloads")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class DataloadsController : ControllerBase
    {
        private readonly IRabbitMQPubSub _pubSub;

        public DataloadsController(IRabbitMQPubSub pubSub)
        {
            _pubSub = pubSub;
        }

        [HttpPost("location")]
        public async Task<IActionResult> RunLocation()
        {
            var loggedInUserEmail = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
          
            await _pubSub.PublishAsync(new DataLoadRequest
            {
                Type = DataLoadType.Location,
                RequesterEmail = loggedInUserEmail ?? string.Empty
            }, routingKey: MQRoutingKey.DataLoad.GetDescription());

            await _pubSub.PublishAsync(new AuditCommand
            {
                Domain = AuditDomain.System,
                DomainId = loggedInUserId.ToGuid(),
                Action = AuditAction.DataLoadRequest,
                PerformedBy = loggedInUserId,
                TargetId = loggedInUserId
            }, routingKey: MQRoutingKey.AuditTrails.GetDescription());

            return Ok();
        }
    }
}