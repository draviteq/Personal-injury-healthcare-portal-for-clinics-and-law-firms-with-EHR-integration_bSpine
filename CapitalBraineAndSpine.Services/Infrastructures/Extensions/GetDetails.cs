using System.Diagnostics;

namespace CapitalBraineAndSpine.Service.Infrastructures.Extensions
{
    public class ExceptionDetails
    {
        public int LineNumber { get; set; }
        public string FileName { get; set; }
        public string Method { get; set; }
        public string Class { get; set; }
        public int ColumnNumber { get; set; }
    }

    public static class GetDetails
    {
        public static ExceptionDetails GetExceptionDetails(this Exception ex)
        {
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            return st.GetFrames()         // get the frames
                  .Select(frame => new ExceptionDetails
                  {                   // get the info
                      FileName = frame.GetFileName(),
                      LineNumber = frame.GetFileLineNumber(),
                      ColumnNumber = frame.GetFileColumnNumber(),
                      Method = frame.GetMethod()?.ToString(),
                      Class = frame.GetMethod()?.DeclaringType?.ToString(),
                  }).FirstOrDefault();
        }
    }
}
