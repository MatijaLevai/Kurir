using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Helpers
{
    public class ExceptionEmailModel
    {
        public IDictionary ExceptionData { get; set; }
        public StackFrame[] Frames { get; set; }
        public IHeaderDictionary HeaderDictionary { get; set; }
        public string HelpLink { get; set; }
        public int HResult { get; set; }
        public string InnerException { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; private set; }
        public string Source { get; set; }
        public string Stacktrace { get; set; }
        public string TargetSite { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
        public string Url { get; set; }

        public ExceptionEmailModel(Exception ex,HttpContext context)
        {
            var trace = new StackTrace(ex,true);
            Frames = trace.GetFrames();
            ExceptionData = ex.Data;
            HeaderDictionary = context.Request.Headers;
            HelpLink = ex.HelpLink;
            HResult = ex.HResult;
            InnerException = ex.InnerException?.Message??string.Empty;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            TargetSite = ex.TargetSite.ToString();
            Time = $"{DateTime.Now.ToLongDateString()}at{DateTime.Now.ToLongTimeString()}";
            Type = ex.GetBaseException().GetType().FullName;
            Url = $"{context.Request.Scheme}//{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";


        }
     }
}
