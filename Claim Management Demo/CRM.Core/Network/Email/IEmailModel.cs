using System.Collections.Generic;

namespace CRM.Core.Network.Email
{
    public interface IEmailModel
    {
        string From { get; set; }
        string To { get; set; }
        string FromDisplayName { get; set; }
        string Cc { get; set; }
        string Bcc { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        IEnumerable<string> Attachments { get; set; }
        string SiteLogoPath { get; set; }
    }
}
