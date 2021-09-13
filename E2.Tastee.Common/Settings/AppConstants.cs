using System.Collections.Generic;

namespace E2.Tastee.Common
{
    public class AppConstants
    {
        public const string VERSION_HEADER_NAME = "zw-version";
        public const string VERSION_NUMBER = "0.24.0";
        public const string CACHE_CONTROL_HEADER_NAME = "Cache-Control";
        public const string PRAGMA_HEADER_NAME = "Pragma";
        public const string EXPIRES_HEADER_NAME = "Expires";
        public const int MAX_SMS_MESSAGE_LENGTH = 1600;
        public const string TWILIO_SMS_NUMBER_FORMAT_REGEX = @"^\+[1-9]\d{1,14}$"; //E.164 spec format
        public const string CLAIM_TYPE_USER_ID = "UserId";
        public const string DATE_FORMAT = "MM/dd/yy";
        public const string DATETIME_FORMAT = "MM/dd/yy hh:mm tt";
        public const string DATA_EXPORT_DATE_FORMAT = "yyyy-MM-dd";
        public const string DATA_EXPORT_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const int UNBOUNDED_RESULT_ROW_COUNT = 100000;
        public const int MAX_FAILED_LOGON_ATTEMPTS = 4;
        public const int LOGON_SELF_HEAL_MINUTES = 15;
        public const int RESOURCE_USER_ID = -1;
        public const int DEFAULT_TENANT_ID = 1;
        public const string DEFAULT_TIME_ZONE = "Eastern Standard Time";
        public const string LINUX_DEFAULT_TIME_ZONE_STRING = "America/New_York";
        public const string ZB_AUTH_TYPE = "ZB2 Authentication";
        public const string AUTH_HEADER_KEY = "Authorization";
        public const int ANY_ID_PLACEHOLDER = -1;
        public const string YMDHMS_FORMAT = "yyyyMMddHHmmss";
        public const decimal DEFAULT_SEARCH_RADIUS_MILES = 30m;
        public const string DEFAULT_TARGET_WORKSHEET_NAME = "ReportData";
        public const string CLAIM_TYPE_PARTICIPANT_ID = "ParticipantId";

        public static class ORG_CONTEXT_NAMES
        {
            public const string CG = "CG";
            public const string Category = "Category";
            public const string Subcategory = "Subcategory";
        }

        public static class REFERENCE_LIST_NAMES
        {
            public const string GENDER = "GENDER";
            public const string RACE = "RACE";
            public const string ETHNICITY = "ETHNICITY";
        }

        public static class POLICIES
        {
            public const string AnyUser = "Users";
            public const string TenantOrSystemAdmin = "SysAdmin";
            public const string AnyAdmin = "AnyAdmin";
            public const string Participant = "Participant";
        }

        public static UserDto ResourceUser() => new UserDto()
            {
                Id = RESOURCE_USER_ID,
                FirstName = "Resource",
                LastName = "Account"
            };

        // TODO:
        public const string PASSWORD_COMPLEXITY_REGEX = ".{8,}";
        public const string PASSWORD_COMPLEXITY_DSC = "at least 8 characters in length, at least 1 number and at least 1 of the following characters * ! % _ $ +";

        public static class MimeTypes
        {
            public const string TEXT_PLAIN = "text/plain";
            public const string JSON = "application/json";
            public const string HTML = "text/html";
            public const string OCTET_STREAM = "application/octet-stream";
            public const string PDF = "application/pdf";
            public const string JPG = "image/jpeg";
            public const string GIF = "image/gif";
            public const string PNG = "image/png";
            public const string CSV = "text/csv";
            public const string XLS = "application/vnd.ms-excel";
            public const string ZIP = "application/zip";
            public const string XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        public static class ErrorMessages
        {
            public const string AccessDenied = "Access Denied!";
            public const string ItemNotFound = "The requested item could not be found";
            public const string WouldBeDuplicate = "Saving this item would result in a duplicate";
            public const string EmptySmsMessage = "The SMS message cannot be empty";
            public static string SmsMessageTooLong = $"The SMS message cannot exceed {AppConstants.MAX_SMS_MESSAGE_LENGTH} characters";
            public const string SmsInvalidToNumber = "The SMS to number is not in a valid format (ex: +18045551212, 18045551212, 8045551212)";
            public const string SmsFromNumberOrNumbersNotSetOrInvalid = "The SMS from number or numbers must be set and valid";
        }
    }
}