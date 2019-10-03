using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Helpers
{
    public interface IEmailService
    {
        Task SendEmailAsync(string fromDisplayName,string fromEmailAddress,string toName,string toEmailAddress,string subject,string message,params Attachment[]attachments);
        Task SendWelcomeEmailAsync( string toName, string toEmailAddress);

    }

}
