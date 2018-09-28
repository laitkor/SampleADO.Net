using System.Collections.Generic;

namespace CRM.Core.Network.Email
{
   public interface IEmailClient
   {
       void Send(IEmailModel model, string FileName="");
       void Send(IList<IEmailModel> model,string FileName="");
   }
}
