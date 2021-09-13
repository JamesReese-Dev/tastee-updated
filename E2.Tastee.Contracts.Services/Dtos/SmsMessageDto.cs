namespace E2.Tastee.Contracts.Services.Dtos
{
    public class SmsMessageDto
    {
        public string ToNumber { get; set; }
        public string Message { get; set; }
        public string FromPhoneNumber { get; set; }
        public string FromSid { get; set; }
        public int? ParticipantId { get; set; }
        public int? SMSTemplateId { get; set; }
        public int? SentSMSId { get; set; }
    }
}