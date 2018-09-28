using CRM.Core.Model;
using CRM.Core.ViewModel;
using CRM.Core.ViewModel.Claim;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CRM.Core.Repository.Claims
{
    public interface IClaimRepository : IRepository<Claim>, IDisposable
    {
        IEnumerable<Claim> GetAllByPortalId(int portalId);

        IEnumerable<CRM.Core.ViewModel.Claim.ActivityLogViewModel> GetActivityLogByClaimId(int claimId);
        IEnumerable<CRM.Core.ViewModel.Claim.ActivityLogViewModel> GetActivityLogByClaimIDClaimantID(int claimId, int? claimantId);

        bool PolicyandInsurerClaimNumberExists(string policynumber, string claimnumber, int? claimtype);
        ClaimViewModel ClaimDetails(int claimid);

        #region Claim Edit

        IEnumerable<ClaimListVM> GetAllClaimsList(Claim claim, IEnumerable<GetUserAssignedClaim_Result> result = null);
        IEnumerable<ClaimListVM> GetAllClaimsList_Allclaim(Claim claim, int page, int pgsz, IEnumerable<GetUserAssignedClaim_Result> result = null,int? type=null, int? Status = null);      
        void InsertAutoDiaries(Claim claim);

        #endregion
        PortalViewModel GetStartingAdjuster(int portalId);

        PortalViewModel GetLastAdjuster(int portalId);

        bool IsClaimNoExists(string ClaimNo);
        bool IsPolicyNoExists(string PolicyNo);
        bool IsClaimNoExistsOnEdit(int ClaimID,string ClientClaimNumber);
        bool IsPolicyNoExistsOnEdit(int ClaimID, string PolicyNo);
        int? GetAdjusterID(int claimid);
        AdjusterDetail GetAdjuster(int claimid);
        int? GetUserBranchCodeByUserID(int UserID);
        ClaimViewModel GetClaimAjusterNo(int ClaimID);
        List<ClaimStatusListViewModel> ClaimStatusList(int ClaimID);
        List<ClaimStatusListViewModel> ClaimantStatusList(int ClaimantID);
        List<sp_EmailToPastDueDate_Result> PastDueDateEmail();
        string getMaxInsurerClaimNumber();
       
    }
}
