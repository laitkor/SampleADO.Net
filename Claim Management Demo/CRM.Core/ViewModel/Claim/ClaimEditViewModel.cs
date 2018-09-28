using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRM.Core.Model;
using System.Web.Mvc;
using System.ComponentModel;

namespace CRM.Core.ViewModel.Claim
{
    public class ClaimEditViewModel : ClaimDetailsViewModel
    {
        [Display(Name = "Company Branch Code")]
        public new int? CompanyBranchID { get; set; }
        // [Required]
        [StringLength(50)]
        [Display(Name = "Insurer Claim #")]
        [Remote("DoesClaimNoExist", "ClaimEdit", AdditionalFields = "ClaimID", HttpMethod = "POST", ErrorMessage = "This Claim No. Already exist")]
        public new string InsurerClaimNumber { get; set; }
        [StringLength(50)]
        [Display(Name = "Adjuster File #")]
        public string AdjusterFile { get; set; }
        [Display(Name = "Client")]
        [Required]
        public string ClientName { get; set; }
        [Display(Name = "Client Branch")]
        public new int? ClientBranchID { get; set; }
        [Display(Name = "Adjuster")]
        public string UserName { get; set; }
        public int? AdjusterUserID { get; set; }
        [Display(Name = "Report to")]
        public new int? ReportTo { get; set; }
        [Display(Name = "Supervisor")]
        public int? SupervisorID { get; set; }
        [Range(byte.MinValue, byte.MaxValue, ErrorMessage = "invalid {0}.")]
        public byte? Severity { get; set; }
        [Display(Name = "Event Type")]
        public string EventType { get; set; }
        [Display(Name = "Event Name")]
        public string EventName { get; set; }
        [Display(Name = "Cat ID #")]
        public string CatId { get; set; }
        [Display(Name = "Status")]
        public int? ClaimStatusId { get; set; }
        [Display(Name = "Sub Status")]
        public int? ClaimSubStatusId { get; set; }

        [DisplayName("Date Entered")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateEntered { get; set; }
        public string StatringAdjusterFile { get; set; }
        public int? LinkClaimID { get; set; }
        public string LinkClaimNo { get; set; }
        public string ReceivedFrom { get; set; }
        public bool IsWatchList { get; set; }
        [Display(Name = "Time Received")]
        public string TimeReceived { get; set; }
        public string TimeReceivedPeriod { get; set; }
        public List<ClaimStatusListViewModel> ClaimStatusList { get; set; }
        public int? UserID { get; set; }
    }

}
