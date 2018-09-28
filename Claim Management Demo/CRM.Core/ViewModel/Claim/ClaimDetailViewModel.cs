using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.ViewModel.Claim
{
    public class ClaimDetailViewModel
    {
        
        public int ClaimID;
        public int ClaimTypeID;
        public Nullable<bool> PBBankClientPay { get; set; }
        public Nullable<bool> PBChargeOff { get; set; }
        public Nullable<bool> PBBankAc { get; set; }
        public string InsuredName { get; set; }
        public string PolicyType { get; set; }
        public string AdjusterName { get; set; }
        public DateTime? DateEntered { get; set; }
        public DateTime? LossDate { get; set; }
        public string InsurerClaim { get; set; }
        public string AdjusterFile { get; set; }
        public string DiaryStatus { get; set; }
        public string ClientName { get; set; }
        public string InsurerBranch { get; set; }
        public string LossCity { get; set; }
        public string LossState { get; set; }
        public string LossType { get; set; }
        public string LossZip { get; set; }
        

        public string EmployerName { get; set; }
        public DateTime? WCLossDate { get; set; }

        public string WCLossTime { get; set; }

        public string WCInjuryPeriod { get; set; }
        public int UserID { get; set; }
        public string PolicyNumber { get; set; }
        public string EventType { get; set; }
        public string EventName { get; set; }
        public string CatID { get; set; }
        public string LossAddress { get; set; }
        public bool IsSelected { get; set; }
    }
}
