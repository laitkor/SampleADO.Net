using System;
using System.Collections.Generic;
using CRM.Core.Model;
using CRM.Core.ViewModel.Claim;
using CRM.Core.ViewModel.ClaimWC;

namespace CRM.Core.Repository.Claims
{
    public interface IClaimEditRepository : IRepository<Claim>, IDisposable
    {
        ClaimEditViewModel GetClaimDetilsByClaimId(int id);
        //Claim GetWithIncludeByClaimId(int claimId);
        Claim GetPortalDetailsByClaimId(int claimId);
        ClaimSideBarViewModel GetClaimDetailsForSidebar(int ClaimId);
    }
}
