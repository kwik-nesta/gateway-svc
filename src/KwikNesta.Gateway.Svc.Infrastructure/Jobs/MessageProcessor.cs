using CSharpTypes.Extensions.Enumeration;
using DiagnosKit.Core.Logging.Contracts;
using DRY.MailJetClient.Library;
using EFCore.CrudKit.Library.Data.Interfaces;
using Hangfire.Console;
using Hangfire.Server;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Contracts.Models.Location;
using KwikNesta.Gateway.Svc.Application.Interfaces;
using KwikNesta.Gateway.Svc.Infrastructure.Extensions;
using KwikNesta.Gateway.Svc.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;

namespace KwikNesta.Gateway.Svc.Infrastructure.Jobs
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILoggerManager _logger;
        private readonly IEFCoreCrudKit _crudKit;
        private readonly string _templateRoot;
        private readonly IMailjetClientService _mailJet;
        private readonly ILocationClientService _locationClient;

        public MessageProcessor(ILoggerManager logger,
                              IMailjetClientService mailJet,
                              ILocationClientService locationClient,
                              IHostEnvironment env,
                              IEFCoreCrudKit crudKit)
        {
            _logger = logger;
            _crudKit = crudKit;
            _mailJet = mailJet;
            _locationClient = locationClient;
            _templateRoot = Path.Combine(env.ContentRootPath, "wwwroot", "templates");
        }

        public async Task HandleAsync(NotificationMessage message, PerformContext context)
        {
            var template = string.Empty;

            if (message != null)
            {
                switch (message.Type)
                {
                    case EmailType.AccountActivation:
                        template = NotificationHelpers.LoadAccountActivationEmail(message, context, _templateRoot);
                        break;
                    case EmailType.PasswordReset:
                        template = NotificationHelpers.LoadAccountActivationEmail(message, context, _templateRoot);
                        break;
                    case EmailType.PasswordResetNotification:
                        template = NotificationHelpers.LoadAccountActivationEmail(message, context, _templateRoot);
                        break;
                    case EmailType.AccountDeactivation:
                        template = NotificationHelpers.LoadAccountActivationEmail(message, context, _templateRoot);
                        break;
                    case EmailType.AccountReactivation:
                        template = NotificationHelpers.LoadAccountActivationEmail(message, context, _templateRoot);
                        break;
                    case EmailType.AccountSuspension:
                        template = NotificationHelpers.LoadAccountActivationEmail(message, context, _templateRoot);
                        break;
                    case EmailType.AccountReactivationNotification:
                        template = NotificationHelpers.LoadAccountActivationEmail(message, context, _templateRoot);
                        break;
                    case EmailType.AdminAccountReactivation:
                        template = NotificationHelpers.LoadAccountActivationEmail(message, context, _templateRoot);
                        break;
                }

                if (string.IsNullOrWhiteSpace(template))
                {
                    _logger.LogInfo("Template returned null");
                    context.WriteLine("Template returned null");
                    return;
                }
                else
                {
                    var isSent = await _mailJet.SendAsync(message.EmailAddress, template, message.Subject);
                    if (isSent)
                    {
                        _logger.LogInfo("Password reset notification email successfully sent to {EmailAddress}", message.EmailAddress);
                        context.WriteLine("Password reset notification email successfully sent to {EmailAddress}", message.EmailAddress);
                        return;
                    }
                    else
                    {
                        _logger.LogWarn("Password reset notification email failed for {EmailAddress}", message.EmailAddress);
                        context.WriteLine("Password reset notification email failed for {EmailAddress}", message.EmailAddress);
                        return;
                    }
                }
            }
            else
            {
                _logger.LogWarn($"Message content came null");
                context.WriteLine("Message content came null");
            }
        }

        public async Task HandleAsync(AuditLog message, PerformContext context)
        {
            try
            {
                if (message != null)
                {
                    if (!NotificationHelpers.ValidatePayload(message))
                    {
                        _logger.LogWarn("Invalid audit payload");
                        context.WriteLine("Invalid audit payolad");
                        return;
                    }

                    await _crudKit.InsertAsync(new KwikNestaAuditLog
                    {
                        PerformedBy = message.PerformedBy,
                        DomainId = message.DomainId,
                        Domain = message.Domain,
                        Action = message.Action,
                        PerformedOnProfileId = message.PerformedOnProfileId
                    });
                    _logger.LogInfo("Audit trail successfully added. Action Performed: {0}", message.Action.GetDescription());
                    context.WriteLine("Audit trail successfully added. Action Performed: {0}", message.Action.GetDescription());
                    return;
                }
                else
                {
                    _logger.LogWarn($"Message content came null");
                    context.WriteLine("Message content came null");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                context.WriteLine("An error occurred: {Message}", ex.Message);
                throw;
            }
        }

        public async Task HandleAsync(DataLoadRequest message, PerformContext context)
        {
            if (message != null)
            {
                switch (message.Type)
                {
                    case DataLoadType.Location:
                        await RunLocationDataLoad(context);
                        break;
                    default:
                        _logger.LogWarn($"Data load request type: {message.Type.GetDescription()}, not implemented.");
                        context.SetTextColor(ConsoleTextColor.Yellow);
                        context.WriteLine($"Data load request type: {message.Type.GetDescription()}, not implemented.");
                        break;
                }
            }
            else
            {
                _logger.LogWarn($"The {nameof(DataLoadRequest)} message came in null");
                context.SetTextColor(ConsoleTextColor.Yellow);
                context.WriteLine($"The {nameof(DataLoadRequest)} message came in null");
            }
        }

        #region Private methods
        private async Task RunLocationDataLoad(PerformContext context)
        {
            try
            {
                var countriesResponseData = await _locationClient.GetCountriesAsyncV1();
                if (countriesResponseData.IsSuccessStatusCode || countriesResponseData.Content == null)
                {
                    context.WriteLine(countriesResponseData.Error?.Message);
                    _logger.LogWarn(countriesResponseData.Error?.Message ?? "An error occurred while getting country data");
                    return;
                }

                foreach (var country in countriesResponseData.Content)
                {
                    _logger.LogInfo("Running data-load for country {0}.", country.Name);
                    context.WriteLine("Running data-load for country {0}.", country.Name);

                    var countryExists = await _crudKit.ExistsAsync<KwikNestaLocationCountry>(c => c.Id == country.Id);
                    if (!countryExists)
                    {
                        //// Itereate over each state
                        var statesToInsert = new List<KwikNestaLocationState>();
                        var cityCount = 0;
                        // Get the states for the country
                        var statesResponseData = await _locationClient.GetStatesForCountryAsyncV1(country.Id);
                        if (statesResponseData.IsSuccessStatusCode && statesResponseData.Content != null)
                        {
                            var states = statesResponseData.Content;
                            _logger.LogInfo($"{states.Count} states found for {country.Name}.");
                            context.WriteLine($"{states.Count} states found for {country.Name}.");

                            foreach (var state in states)
                            {
                                _logger.LogInfo($"Running data-load for the {country.Nationality} state, {state.Name}.");
                                context.WriteLine("Running data-load for the {0} state, {1}.", country.Nationality, state.Name);
                                var stateExists = await _crudKit.ExistsAsync<KwikNestaLocationState>(c => c.Id == country.Id);
                                if (!stateExists)
                                {
                                    var stateToAdd = NotificationHelpers.Map(state);
                                    // Get the cities for the state
                                    var citiesToAdd = new List<KwikNestaLocationCity>();
                                    var citiesResponseData = await _locationClient.GetCitiesForStateAsyncV1(country.Id, state.Id);
                                    if (citiesResponseData.IsSuccessStatusCode && citiesResponseData.Content != null)
                                    {

                                        citiesToAdd = citiesResponseData.Content.Select(NotificationHelpers.Map).ToList();
                                        stateToAdd.Cities = citiesToAdd;
                                        cityCount = citiesToAdd.Count;
                                    }
                                    else
                                    {
                                        context.WriteLine(citiesResponseData.Error?.Message);
                                    }

                                    statesToInsert.Add(stateToAdd);

                                }
                                else
                                {
                                    _logger.LogInfo($"State: {state.Name} in {country.Name} already exists in the database");
                                    context.WriteLine($"State: {state.Name} in {country.Name} already exists in the database");
                                }
                            }
                        }
                        else
                        {
                            context.WriteLine(statesResponseData.Error?.Message);
                        }

                        var countryToInsert = NotificationHelpers.Map(country);
                        countryToInsert.States = statesToInsert;
                        await _crudKit.InsertAsync(countryToInsert);
                        _logger.LogInfo("Data-load for country {0} is now done successfully.\nNo. of states {1}\nNo. of cities: {2}", country.Name, statesToInsert.Count, cityCount);
                        context.WriteLine("Data-load for country {0} is now done successfully.\nNo. of states {1}\nNo. of cities: {2}", country.Name, statesToInsert.Count, cityCount);
                    }
                    else
                    {
                        _logger.LogInfo("Country: {Name} already stateExists in the database", country.Name);
                        context.WriteLine("Country: {Name} already stateExists in the database", country.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion
    }
}