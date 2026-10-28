using CapitalBraineAndSpine.Data.Abstracts;

namespace CapitalBraineAndSpine.Data.Entities.LogsEntities
{
    public class ExceptionLog : BaseEntity
    {
        public string UserId { get; set; }
        public int LineNumber { get; set; }
        public string FileName { get; set; }
        public string Method { get; set; }
        public string Class { get; set; }
        public int ColumnNumber { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerExMessage { get; set; }
        public string IPAddress { get; set; }
    }
}
