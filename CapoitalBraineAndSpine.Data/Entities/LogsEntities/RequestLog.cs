using CapitalBraineAndSpine.Data.Abstracts;

namespace CapitalBraineAndSpine.Data.Entities.LogsEntities
{
    public class RequestLog : BaseEntity
    {
        public string UserId { get; set; }
        public string Method { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Url { get; set; }
        public string ReferUrl { get; set; }
        public string IpAddress { get; set; }

    }
}
