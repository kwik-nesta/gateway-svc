using CSharpTypes.Extensions.Guid;
using CSharpTypes.Extensions.Object;
using CSharpTypes.Extensions.String;
using Hangfire.Console;
using Hangfire.Server;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Contracts.Models.Location;

namespace KwikNesta.Gateway.Svc.Infrastructure.Extensions
{
    public class NotificationHelpers
    {
        public static string LoadAccountActivationEmail(NotificationMessage notification,
                                                     PerformContext context,
                                                     string templateRoot)
        {
            if (!ValidatePayload(notification))
            {
                context.WriteLine("Invalid notification payload");
                return "";
            }

            var template = LoadTemplate(notification.Type, templateRoot);
            if (template.IsNullOrEmpty())
            {
                context.WriteLine("The template returned an empty string");
                return "";
            }

            return template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{OTP}}", notification.Otp?.Value)
                .Replace("{{validity}}", notification.Otp?.Span.ToString())
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

        }

        public static string LoadPasswordResetEmail(NotificationMessage notification,
                                                 PerformContext context,
                                                 string templateRoot)
        {
            if (!ValidatePayload(notification))
            {
                context.WriteLine("Invalid notification payload");
                return "";
            }

            var template = LoadTemplate(notification.Type, templateRoot);
            if (template.IsNullOrEmpty())
            {
                context.WriteLine("The template returned an empty string");
                return "";
            }

            return template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{OTP}}", notification.Otp?.Value)
                .Replace("{{validity}}", notification.Otp?.Span.ToString())
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
        }

        public static string LoadPasswordResetNotificationEmail(NotificationMessage notification,
                                                             PerformContext context,
                                                             string templateRoot)
        {
            if (!ValidatePayload(notification))
            {
                context.WriteLine("Invalid notification payload");
                return "";
            }

            var template = LoadTemplate(notification.Type, templateRoot);
            if (template.IsNullOrEmpty())
            {
                context.WriteLine("The template returned an empty string");
                return "";
            }

            return template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
        }

        public static string LoadAccountDeactivationEmail(NotificationMessage notification,
                                                       PerformContext context,
                                                       string templateRoot)
        {
            if (!ValidatePayload(notification))
            {
                context.WriteLine("Invalid notification payload");
                return "";
            }

            var template = LoadTemplate(notification.Type, templateRoot);
            if (template.IsNullOrEmpty())
            {
                context.WriteLine("The template returned an empty string");
                return "";
            }

            var now = DateTime.UtcNow;
            return template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{DeactivationDate}}", now.ToString("d"))
                .Replace("{{Year}}", now.Year.ToString());
        }

        public static string LoadAccountReactivationEmail(NotificationMessage notification,
                                                       PerformContext context,
                                                       string templateRoot)
        {
            if (!ValidatePayload(notification))
            {
                context.WriteLine("Invalid notification payload");
                return "";
            }

            var template = LoadTemplate(notification.Type, templateRoot);
            if (template.IsNullOrEmpty())
            {
                context.WriteLine("The template returned an empty string");
                return "";
            }

            var now = DateTime.UtcNow;
            return template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{OTP}}", notification.Otp?.Value)
                .Replace("{{validity}}", notification.Otp?.Span.ToString())
                .Replace("{{Year}}", now.Year.ToString());
        }

        public static string LoadAccountReactivationNotificationEmail(NotificationMessage notification,
                                                                   PerformContext context,
                                                                   string templateRoot)
        {
            if (!ValidatePayload(notification))
            {
                context.WriteLine("Invalid notification payload");
                return "";
            }

            var template = LoadTemplate(notification.Type, templateRoot);
            if (template.IsNullOrEmpty())
            {
                context.WriteLine("The template returned an empty string");
                return "";
            }

            return template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
        }

        public static string LoadAccountSuspensionEmail(NotificationMessage notification,
                                                     PerformContext context,
                                                     string templateRoot)
        {
            if (!ValidatePayload(notification))
            {
                context.WriteLine("Invalid notification payload");
                return "";
            }

            var template = LoadTemplate(notification.Type, templateRoot);
            if (template.IsNullOrEmpty())
            {
                context.WriteLine("The template returned an empty string");
                return "";
            }

            return template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{Reason}}", notification.Reason)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
        }

        public static string LoadAdminRectivationNotificationEmail(NotificationMessage notification,
                                                                PerformContext context,
                                                                string templateRoot)
        {
            if (!ValidatePayload(notification))
            {
                context.WriteLine("Invalid notification payload");
                return "";
            }

            var template = LoadTemplate(notification.Type, templateRoot);
            if (template.IsNullOrEmpty())
            {
                context.WriteLine("The template returned an empty string");
                return "";
            }

            return template.Replace("{{FirstName}}", notification.ReceipientName)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
        }

        public static KwikNestaLocationCountry Map(KwikNestaCountry country)
        {
            return new KwikNestaLocationCountry
            {
                Id = country.Id,
                Name = country.Name,
                ISO2 = country.ISO2,
                ISO3 = country.ISO3,
                NumericCode = country.NumericCode,
                PhoneCode = country.PhoneCode,
                Capital = country.Capital,
                Currency = country.Currency,
                CurrencyName = country.CurrencyName,
                CurrencySymbol = country.CurrencySymbol,
                TLD = country.TLD,
                Region = country.Region,
                SubRegion = country.SubRegion,
                Native = country.Native,
                Nationality = country.Nationality,
                Longitude = country.Longitude,
                Latitude = country.Latitude,
                Emoji = country.Emoji,
                EmojiUnicode = country.EmojiUnicode,
                TimeZones = country.TimeZones
                    .Select(c => Map(country.Id, c)).ToList()
            };
        }

        public static KwikNestaLocationState Map(KwikNestaState state)
        {
            return new KwikNestaLocationState
            {
                Id = state.Id,
                CountryId = state.CountryId,
                Name = state.Name,
                CountryCode = state.CountryCode,
                ISO2 = state.ISO2,
                Longitude = state.Longitude,
                Latitude = state.Latitude,
                Type = state.Type
            };
        }

        public static KwikNestaLocationCity Map(KwikNestaCity city)
        {
            return new KwikNestaLocationCity
            {
                Id = city.Id,
                StateId = city.StateId,
                CountryId = city.CountryId,
                Name = city.Name,
                Longitude = city.Longitude,
                Latitude = city.Latitude
            };
        }

        public static KwikNestaLocationTimeZone Map(Guid countryId, KwikNestaTimeZone tz)
        {
            return new KwikNestaLocationTimeZone
            {
                CountryId = countryId,
                ZoneName = tz.ZoneName,
                GMTOffset = tz.GMTOffset,
                GMTOffsetName = tz.GMTOffsetName,
                Abbreviation = tz.Abbreviation,
                TZName = tz.TZName
            };
        }

        public static bool ValidatePayload(AuditLog audit)
        {
            if (audit == null)
            {
                return false;
            }

            if (audit.DomainId.IsEmpty() || audit.PerformedBy.IsNullOrEmpty() || audit.PerformedOnProfileId.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }

        #region Private Methods
        private static string GetTemplateName(EmailType type)
        {
            return type switch
            {
                EmailType.AccountActivation => "account-activation",
                EmailType.AccountDeactivation => "account-deactivation",
                EmailType.AccountReactivation => "account-reactivation",
                EmailType.AccountReactivationNotification => "account-reactivation-notification",
                EmailType.AccountSuspension => "account-suspension",
                EmailType.AdminAccountReactivation => "admin-account-reactivation",
                EmailType.PasswordReset => "password-reset",
                EmailType.PasswordResetNotification => "password-reset-notification",
                _ => throw new NotImplementedException()
            };
        }

        private static string LoadTemplate(EmailType templateType, string templateRootDirectory)
        {
            var path = Path.Combine(templateRootDirectory, $"{GetTemplateName(templateType)}.html");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return string.Empty;
        }

        private static bool ValidatePayload(NotificationMessage notification)
        {
            if (notification == null)
            {
                return false;
            }

            if (notification.ReceipientName.IsNullOrEmpty() || notification.EmailAddress.IsNullOrEmpty())
            {
                return false;
            }

            if (notification.Type is EmailType.AccountActivation or EmailType.PasswordReset or EmailType.AccountReactivation)
            {
                if (notification.Otp.IsNull() || (notification.Otp != null && notification.Otp.Value.IsNullOrEmpty()))
                {
                    return false;
                }
            }

            if (notification.Type is EmailType.AccountSuspension && string.IsNullOrWhiteSpace(notification.Reason))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
