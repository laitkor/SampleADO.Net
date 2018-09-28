using CRM.Core.Model;
using CRM.Core.Repository.Claims;
using CRM.Core.ViewModel.Claim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CRM.Core.ViewModel;
using CRM.Common.Security;
using System.Linq.Expressions;

namespace CRM.Data.Repository.Claims
{
    public class ClaimRepository : Repository<Claim>, IClaimRepository
    {
        private readonly CRMDbContext _context;
        public ClaimRepository(CRMDbContext context)
            : base(context)
        {
            _context = context;
        }
        public IEnumerable<Claim> GetAllByPortalId(int portalId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ActivityLogViewModel> GetActivityLogByClaimId(int ClaimID)
        {
            var ActivityLog = from t1 in _context.ClaimActivityLogs
                              join t2 in _context.Users on t1.ActivityUserID equals t2.UserID
                              where t1.ClaimID == ClaimID && t1.IsDeleted != true
                              orderby t1.ClaimActivityLogID descending
                              select new ActivityLogViewModel
                             {
                                 ActivityDate = t1.ActivityDate,
                                 ActivityDetail = t1.ActivityDetail,
                                 ActivityTitle = t1.ActivityTitle,
                                 ActivityUserName = t2.UserName,
                                 ClaimActivityLogID = t1.ClaimActivityLogID
                             };
            return ActivityLog;
        }

        public IEnumerable<ActivityLogViewModel> GetActivityLogByClaimIDClaimantID(int claimID, int? claimantID)
        {
            var ActivityLog = from t1 in _context.ClaimActivityLogs
                              join t2 in _context.Users on t1.ActivityUserID equals t2.UserID
                              where t1.ClaimID == claimID && t1.ClaimantID == claimantID && t1.IsDeleted != true
                              orderby t1.ClaimActivityLogID descending
                              select new ActivityLogViewModel
                              {
                                  ActivityDate = t1.ActivityDate,
                                  ActivityDetail = t1.ActivityDetail,
                                  ActivityTitle = t1.ActivityTitle,
                                  ActivityUserName = t2.UserName,
                                  ClaimActivityLogID = t1.ClaimActivityLogID
                              };
            return ActivityLog;
        }

        public PortalViewModel GetStartingAdjuster(int portalId)
        {
            PortalViewModel ret = (from p in _context.Portals
                                   where p.PortalID == portalId && p.IsDeleted != true
                                   select new PortalViewModel
                          {
                              StartingAdjuster = p.StartingAdjuster,
                              Pid = p.Pid
                          }).FirstOrDefault();

            return ret;
            //return _context.Portals.Where(x => x.PortalID == portalId).Select(x => new Portal { StartingAdjuster = x.StartingAdjuster,Pid=x.Pid}).FirstOrDefault();
        }

        public PortalViewModel GetLastAdjuster(int portalId)
        {
            var qry = _context.Claims
                .Where(m => m.PortalID == portalId && (m.IsDeleted ?? false) == false)
                .OrderByDescending(m => m.ClaimID)
                .Select(x => new PortalViewModel
                {
                    AdjusterFile = x.AdjusterFile
                })
                .Skip(1).Take(1).FirstOrDefault();
            return qry;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool PolicyandInsurerClaimNumberExists(string policynumber, string claimnumber, int? claimtype)
        {
            bool isexist = false;
            if (policynumber == null && claimnumber != null)
            {
                Claim claim = _context.Claims.Where(x => x.ClaimTypeID == claimtype && (x.InsurerClaimNumber == claimnumber) && x.IsDeleted != true).FirstOrDefault();
                if (claim != null)
                    isexist = true;

            }
            else if (policynumber != null && claimnumber == null)
            {
                Claim claim = _context.Claims.Where(x => x.ClaimTypeID == claimtype && (x.PolicyNumber == policynumber) && x.IsDeleted != true).FirstOrDefault();
                if (claim != null)
                    isexist = true;
            }
            else if (policynumber == null && claimnumber == null)
            {
                isexist = false;

            }
            else
            {
                Claim claim = _context.Claims.Where(x => x.ClaimTypeID == claimtype && (x.PolicyNumber == policynumber || x.InsurerClaimNumber == claimnumber) && x.IsDeleted != true).FirstOrDefault();
                if (claim != null)
                    isexist = true;

            }


            return isexist;
        }

        #region Claim Edit Repo

        //Update Watchist
        public IEnumerable<ClaimListVM> GetAllClaimsList(Claim claim, IEnumerable<GetUserAssignedClaim_Result> result = null)
        {
            var res = _context.Claims
                .Include("Insured")
                .Include("User")
                .Include("ClaimEmployerDetail")
                .Include(x => x.UserAdjuster)
                .Include("refPolicyType")
                .Include("refClaimType")
                .Include("Client")
                .Include("ClaimInjuryDetails")
                .Include(x => x.ClientBranch)
                .Include(x => x.Insured.LossState)
                .Include(x => x.ClaimEmployerDetails)
                .Include(x => x.ClaimDiaries)
                .Include(x => x.ClaimInjuryDetails);
            if (result != null)
            {
                List<int> AllAssignedClaim = new List<int>();
                List<GetUserAssignedClaim_Result> Resultcpy = new List<GetUserAssignedClaim_Result>(result);
                AllAssignedClaim = Resultcpy.Select(x => x.ClaimId).ToList();
                res = res.Where(x => AllAssignedClaim.Contains(x.ClaimID) && (x.IsDeleted ?? false) == false);
            }
            else
            {

                if (AuthUser.User.Roles == "PortalAdmin")
                {
                    res = res.Where(x => x.PortalID == claim.PortalID && (x.IsDeleted ?? false) == false).OrderByDescending(x => x.ClaimID);
                }
                else
                {
                    List<int> AllAssignedClaim = new List<int>();
                    List<GetUserAssignedClaim_Result> Resultcpy = new List<GetUserAssignedClaim_Result>(result);
                    AllAssignedClaim = Resultcpy.Select(x => x.ClaimId).ToList();
                    res = res.Where(x => AllAssignedClaim.Contains(x.ClaimID) && (x.IsDeleted ?? false) == false).OrderByDescending(x => x.ClaimID);
                }
            }
            var ret = res.Select(x => new ClaimListVM()
                {
                    InsuredName = x.Insured.InsuredName,
                    PolicyType = x.refPolicyType.PolicyType,
                    AdjusterName = x.UserAdjuster.FirstName + " " + x.UserAdjuster.LastName,
                    DateEntered = x.DateEntered,
                    LossDate = x.LossDate,
                    InsurerClaim = x.InsurerClaimNumber,
                    AdjusterFile = x.AdjusterFile,
                    ClientName = x.Client.ClientName,
                    InsurerBranch = x.ClientBranch.CBranchName,
                    LossCity = x.Insured.LossCity,
                    LossState = x.Insured.LossState.StateName,
                    DiaryStatus = x.UserAdjuster == null ? "Assign Adjuster" :
                    x.IsClaimClosed ? "File Closed" : x.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted != true).OrderBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName ?? x.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
                    LossType = "",
                    ClaimID = x.ClaimID,
                    ClaimTypeID = x.ClaimTypeID ?? 0,
                    EmployerName = x.ClaimEmployerDetails.Where(y => y.ClaimID == x.ClaimID).FirstOrDefault().Employer,
                    WCLossDate = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryDate,
                    WCLossTime = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryTime,
                    WCInjuryPeriod = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryPeriod,
                    UserID = x.UserID,
                    PolicyNumber = x.PolicyNumber,
                    EventType = x.EventType,
                    EventName = x.EventName,
                    CatID = x.CategoryId,
                    IsClaimClosed = x.IsClaimClosed,
                    ClaimCloseDate = x.ClaimCloseDate,
                    IsClaimReOpened = x.IsClaimReOpened,
                    ClaimReOpeneDate = x.ClaimReOpeneDate,
                    isWatchList = x.isWathcList,
                    ClientID = x.ClientID,
                    ClientBranchID = x.ClientBranchID ?? 0,
                    TimeReceived = x.TimeReceived,
                    TimeReceivedPeriod = x.TimeReceivedPeriod,
                    AdjusterFirstName = x.UserAdjuster.FirstName,
                    AdjusterLastName = x.UserAdjuster.LastName,
                    ClientBranchZip = x.ClientBranch.CBrnahcZip,
                    ClientBranchState = _context.refStates.Where(y => y.StateID == x.ClientBranch.CBranchStateID).FirstOrDefault().StateName,
                    ClientBranchCity = x.ClientBranch.CBranchCity,
                    ClientBranchAdd1 = x.ClientBranch.CBranchAddress,
                    ClientBranchAdd2 = x.ClientBranch.CBranchAddress2,
                    ClientBranchName = x.ClientBranch.CBranchName,
                    ClientFax = x.Client.FaxNumber,
                    ClientEmail = x.Client.ReportingEmail,
                    ClientZip = x.Client.BillingZipCode,
                    ClientPhone = x.Client.Phone,
                    ClientAdd1 = x.Client.BillingAdd1,
                    ClientAdd2 = x.Client.BillingAdd2,
                    ClientState = _context.refStates.Where(y => y.StateID == x.Client.BillingStateID).FirstOrDefault().StateName,
                    ClientCity = x.Client.BillingCity,
                    PortalBranchName = x.PortalBranch.BranchName,
                    PortalBranchCode = x.PortalBranch.BranchCode
                }).ToList();
            return ret;

        }

        public IEnumerable<ClaimListVM> GetAllClaimsList_Allclaim(Claim claim, int page, int pgsze, IEnumerable<GetUserAssignedClaim_Result> result = null, int? Typeid = null, int? Status = null)
        {
            var res = _context.Claims
                .Include("Insured")
                .Include("User")
                .Include("ClaimEmployerDetail")
                .Include(x => x.UserAdjuster)
                .Include("refPolicyType")
                .Include("refClaimType")
                .Include("Client")
                .Include("ClaimInjuryDetails")
                .Include(x => x.ClientBranch)
                .Include(x => x.Insured.LossState)
                .Include(x => x.ClaimEmployerDetails)
                .Include(x => x.ClaimDiaries)
                .Include(x => x.ClaimInjuryDetails);
            if (result != null)
            {
                List<int> AllAssignedClaims = new List<int>();
                List<GetUserAssignedClaim_Result> Resultcpy = new List<GetUserAssignedClaim_Result>(result);
                AllAssignedClaims = Resultcpy.Select(x => x.ClaimId).ToList();
                res = res.Where(x => AllAssignedClaims.Contains(x.ClaimID));
            }
            else
            {
                if (AuthUser.User.Roles == "PortalAdmin")
                {
                    res = res.Where(x => x.PortalID == claim.PortalID && (x.IsDeleted ?? false) == false).OrderByDescending(x => x.ClaimID);
                }
                else
                {
                    List<int> AllAssignedClaims = new List<int>();
                    List<GetUserAssignedClaim_Result> Resultcpy = new List<GetUserAssignedClaim_Result>(result);
                    AllAssignedClaims = Resultcpy.Select(x => x.ClaimId).ToList();
                    res = res.Where(x => AllAssignedClaims.Contains(x.ClaimID) && (x.IsDeleted ?? false) == false).OrderByDescending(x => x.ClaimID);
                }
            }
            var clmcount = 0;
            if (Typeid != null || Status != null)
            {

                if (Typeid == 1 && Status == 0)
                {


                    clmcount = res.Where(x => x.ClaimTypeID == 1).Count();
                    res = res.Where(x => x.ClaimTypeID == 1);
                }
                else if (Typeid == 1 && Status == 1)
                {


                    clmcount = res.Where(x => x.ClaimTypeID == 1 && (x.IsClaimClosed == false || x.IsClaimReOpened == true) && x.IsDeleted != true).Count();
                    res = res.Where(x => x.ClaimTypeID == 1 && (x.IsClaimClosed == false || x.IsClaimReOpened == true) && x.IsDeleted != true);
                }
                else if (Typeid == 1 && Status == 2)
                {


                    clmcount = res.Where(x => x.ClaimTypeID == 1 && (x.IsClaimClosed == true) && x.IsDeleted != true).Count();
                    res = res.Where(x => x.ClaimTypeID == 1 && (x.IsClaimClosed == true) && x.IsDeleted != true);

                }
                else if (Typeid == 2 && Status == 0)
                {

                    clmcount = res.Where(x => x.ClaimTypeID == 2 && x.IsDeleted != true).Count();
                    res = res.Where(x => x.ClaimTypeID == 2 && x.IsDeleted != true);
                }
                else if (Typeid == 2 && Status == 1)
                {


                    clmcount = res.Where(x => x.ClaimTypeID == 2 && (x.IsClaimClosed == false || x.IsClaimReOpened == true) && x.IsDeleted != true).Count();
                    res = res.Where(x => x.ClaimTypeID == 2 && (x.IsClaimClosed == false || x.IsClaimReOpened == true) && x.IsDeleted != true);
                }
                else if (Typeid == 2 && Status == 2)
                {

                    clmcount = res.Where(x => x.ClaimTypeID == 2 && (x.IsClaimClosed == true) && x.IsDeleted != true).Count();
                    res = res.Where(x => x.ClaimTypeID == 2 && (x.IsClaimClosed == true) && x.IsDeleted != true);
                }
                else if (Typeid == 3 && Status == 0)
                {

                    clmcount = res.Where(x => x.ClaimTypeID == 3 && x.IsDeleted != true).Count();
                    res = res.Where(x => x.ClaimTypeID == 3 && x.IsDeleted != true);
                }
                else if (Typeid == 3 && Status == 1)
                {


                    clmcount = res.Where(x => x.ClaimTypeID == 3 && (x.IsClaimClosed == false || x.IsClaimReOpened == true) && x.IsDeleted != true).Count();
                    res = res.Where(x => x.ClaimTypeID == 3 && (x.IsClaimClosed == false || x.IsClaimReOpened == true) && x.IsDeleted != true);
                }
                else if (Typeid == 3 && Status == 2)
                {

                    clmcount = res.Where(x => x.ClaimTypeID == 3 && (x.IsClaimClosed == true) && x.IsDeleted != true).Count();
                    res = res.Where(x => x.ClaimTypeID == 3 && (x.IsClaimClosed == true) && x.IsDeleted != true);
                }
                else
                {
                    if (Status == 1)
                    {
                        clmcount = res.Where(x => (x.IsClaimClosed == false || x.IsClaimReOpened == true) && x.IsDeleted != true).Count();
                        res = res.Where(x => (x.IsClaimClosed == false || x.IsClaimReOpened == true) && x.IsDeleted != true);

                    }
                    else if (Status == 2)
                    {
                        clmcount = res.Where(x => (x.IsClaimClosed == true) && x.IsDeleted != true).Count();
                        res = res.Where(x => (x.IsClaimClosed == true) && x.IsDeleted != true);
                    }
                    else
                    {
                        clmcount = res.Count();
                    }


                }


            }
            else
            {
                clmcount = res.Count();
            }

            var ret = new List<ClaimListVM>();
            if (AuthUser.User.Roles == "PortalAdmin")
            {
                ret = res.Select(x => new ClaimListVM()
                     {
                         InsuredName = x.Insured.InsuredName,
                         PolicyType = x.refPolicyType.PolicyType,
                         AdjusterName = x.UserAdjuster.FirstName + " " + x.UserAdjuster.LastName,
                         DateEntered = x.DateEntered,
                         LossDate = x.LossDate,
                         InsurerClaim = x.InsurerClaimNumber,
                         AdjusterFile = x.AdjusterFile,
                         ClientName = x.Client.ClientName,
                         InsurerBranch = x.ClientBranch.CBranchName,
                         LossCity = x.Insured.LossCity,
                         LossState = x.Insured.LossState.StateName,
                         DiaryStatus = x.UserAdjuster == null ? "Assign Adjuster" :
                         x.IsClaimClosed ? "File Closed" : x.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted != true).OrderBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName ?? x.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
                         //DiaryStatus = _context.ClaimDiaries.Include(y => y.refDiaryStatu).Where(y => (y.IsDeleted ?? false) == false && y.ClaimID == x.ClaimID).OrderByDescending(y => y.IsCompleted).ThenBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
                         LossType = "",
                         ClaimID = x.ClaimID,
                         ClaimTypeID = x.ClaimTypeID ?? 0,
                         EmployerName = x.ClaimEmployerDetails.Where(y => y.ClaimID == x.ClaimID).FirstOrDefault().Employer,
                         WCLossDate = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryDate,
                         WCLossTime = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryTime,
                         WCInjuryPeriod = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryPeriod,
                         UserID = x.UserID,
                         PolicyNumber = x.PolicyNumber,
                         EventType = x.EventType,
                         EventName = x.EventName,
                         CatID = x.CategoryId,
                         IsClaimClosed = x.IsClaimClosed,
                         ClaimCloseDate = x.ClaimCloseDate,
                         IsClaimReOpened = x.IsClaimReOpened,
                         ClaimReOpeneDate = x.ClaimReOpeneDate,
                         isWatchList = x.isWathcList,
                         ClientID = x.ClientID,
                         ClientBranchID = x.ClientBranchID ?? 0,
                         clmcount = clmcount,
                         TimeReceived = x.TimeReceived,
                         TimeReceivedPeriod = x.TimeReceivedPeriod,
                         AdjusterId = x.AdjusterID,
                         SupervisorId = x.SupervisorID
                     }).Skip((page - 1) * pgsze).Take(pgsze).ToList();
            }
            else
            {
                ret = res.Select(x => new ClaimListVM()
                    {
                        InsuredName = x.Insured.InsuredName,
                        PolicyType = x.refPolicyType.PolicyType,
                        AdjusterName = x.UserAdjuster.FirstName + " " + x.UserAdjuster.LastName,
                        DateEntered = x.DateEntered,
                        LossDate = x.LossDate,
                        InsurerClaim = x.InsurerClaimNumber,
                        AdjusterFile = x.AdjusterFile,
                        ClientName = x.Client.ClientName,
                        InsurerBranch = x.ClientBranch.CBranchName,
                        LossCity = x.Insured.LossCity,
                        LossState = x.Insured.LossState.StateName,
                        DiaryStatus = x.UserAdjuster == null ? "Assign Adjuster" :
                        x.IsClaimClosed ? "File Closed" : x.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted != true).OrderBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName ?? x.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
                        //DiaryStatus = _context.ClaimDiaries.Include(y => y.refDiaryStatu).Where(y => (y.IsDeleted ?? false) == false && y.ClaimID == x.ClaimID).OrderByDescending(y => y.IsCompleted).ThenBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
                        LossType = "",
                        ClaimID = x.ClaimID,
                        ClaimTypeID = x.ClaimTypeID ?? 0,
                        EmployerName = x.ClaimEmployerDetails.Where(y => y.ClaimID == x.ClaimID).FirstOrDefault().Employer,
                        WCLossDate = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryDate,
                        WCLossTime = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryTime,
                        WCInjuryPeriod = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryPeriod,
                        UserID = x.UserID,
                        PolicyNumber = x.PolicyNumber,
                        EventType = x.EventType,
                        EventName = x.EventName,
                        CatID = x.CategoryId,
                        IsClaimClosed = x.IsClaimClosed,
                        IsClaimReOpened = x.IsClaimReOpened,
                        isWatchList = x.isWathcList,
                        ClientID = x.ClientID,
                        ClientBranchID = x.ClientBranchID ?? 0,
                        clmcount = clmcount,
                        AdjusterId = x.AdjusterID,
                        SupervisorId = x.SupervisorID
                    }).OrderByDescending(y => y.ClaimID).Skip((page - 1) * pgsze).Take(pgsze).ToList();

            }

            return ret;

        }
        public void InsertAutoDiaries(Claim claim)
        {
            var datereceived = claim.DateReceived.Value.Add(To24HrTime(claim.TimeReceived + " " + claim.TimeReceivedPeriod));
            _context.Proc_AutomatedDiary(claim.ClaimID, claim.ClientID, claim.ClaimTypeID, claim.UserID, datereceived);
        }
        #endregion


        public ClaimViewModel ClaimDetails(int claimid)
        {
            var clientdata = (from x in _context.Claims
                              where x.ClaimID == claimid && x.IsDeleted != true
                              select new ClaimViewModel()
                              {
                                  PolicyNumber = x.PolicyNumber,
                                  InsurerClaimNumber = x.InsurerClaimNumber,
                                  DateReceived = x.DateReceived,
                                  AdjusterFileNumber = x.AdjusterFile,
                                  BillingProfileID = x.BillingProfileID,
                                  CompanyBranchID = x.CompanyBranchID
                              }).FirstOrDefault();

            return clientdata;
        }

        public bool IsClaimNoExists(string ClaimNo)
        {
            int isExist = 0;
            isExist = _context.Claims.Where(x => x.InsurerClaimNumber == ClaimNo && x.IsDeleted != true).Count();
            if (isExist != 0)
                return true;
            else
                return false;
        }
        public bool IsPolicyNoExists(string PolicyNo)
        {
            int isExist = 0;
            isExist = _context.Claims.Where(x => x.PolicyNumber == PolicyNo && x.IsDeleted != true).Count();
            if (isExist != 0)
                return true;
            else
                return false;
        }

        public bool IsClaimNoExistsOnEdit(int ClaimID, string ClaimNo)
        {
            int isExist = 0;
            isExist = _context.Claims.Where(x => x.InsurerClaimNumber == ClaimNo && x.ClaimID != ClaimID && x.PortalID == AuthUser.User.PortalId && x.IsDeleted != true).Count();
            if (isExist != 0)
                return true;
            else
                return false;
        }

        public bool IsPolicyNoExistsOnEdit(int ClaimID, string PolicyNo)
        {
            int isExist = 0;
            isExist = _context.Claims.Where(x => x.PolicyNumber == PolicyNo && x.ClaimID != ClaimID && x.PortalID == AuthUser.User.PortalId && x.IsDeleted != true).Count();
            if (isExist != 0)
                return true;
            else
                return false;
        }

        public int? GetAdjusterID(int claimid)
        {
            return _context.Claims.SingleOrDefault(x => x.ClaimID == claimid && x.IsDeleted != true).AdjusterID;
        }

        public AdjusterDetail GetAdjuster(int claimid)
        {
            AdjusterDetail data = (from x in _context.Users
                                   join y in _context.Claims on x.UserID equals y.AdjusterID
                                   where y.ClaimID == claimid && x.IsDeleted != true
                                   select new AdjusterDetail
                                   {
                                       AdjusterID = x.UserID,
                                       Adjustername = x.FirstName + " " + x.LastName
                                   }).FirstOrDefault();

            return data;
        }

        public int? GetUserBranchCodeByUserID(int UserID)
        {
            return _context.Users.Where(m => m.UserID == UserID && m.IsDeleted != true).Select(m => m.PortalBranchID).SingleOrDefault();
        }


        public ClaimViewModel GetClaimAjusterNo(int ClaimID)
        {
            ClaimViewModel claim = _context.Claims.Where(x => x.ClaimID == ClaimID && x.IsDeleted != true).Select(x => new ClaimViewModel
            {
                InsurerClaimNumber = x.InsurerClaimNumber,
                AdjusterFileNumber = x.AdjusterFile
            }).FirstOrDefault();
            return claim;
        }


        public List<ClaimStatusListViewModel> ClaimStatusList(int ClaimID)
        {
            var result = (from cd in _context.ClaimDetails
                          join u in _context.Users on cd.UserId equals u.UserID
                          where cd.ClaimId == ClaimID && cd.IsDeleted != true
                          select new ClaimStatusListViewModel()
                          {
                              Claimid = cd.ClaimId,
                              Date = cd.Date,
                              Status = cd.Status,
                              Userid = cd.UserId ?? 0,
                              UserName = u.UserName
                          }).ToList();

            return result;

        }

        public List<ClaimStatusListViewModel> ClaimantStatusList(int ClaimantID)
        {
            var result = (from cd in _context.ClaimantOpenCloseStatus
                          join u in _context.Users on cd.UserID equals u.UserID
                          where cd.ClaimantID == ClaimantID && cd.IsDeleted != true
                          select new ClaimStatusListViewModel()
                          {
                              ClaimantId = cd.ClaimantID,
                              Date = cd.Date,
                              Status = cd.Status,
                              Userid = cd.UserID,
                              UserName = u.UserName
                          }).ToList();

            return result;
        }

        //private Expression<Func<ClaimListVM, bool>> BuildPredicate(Claim claim)
        //{


        //    var res = _context.Claims
        //        .Include("Insured")
        //        .Include("User")
        //        .Include("ClaimEmployerDetail")
        //        .Include(x => x.UserAdjuster)
        //        .Include("refPolicyType")
        //        .Include("refClaimType")
        //        .Include("Client")
        //        .Include("ClaimInjuryDetails")
        //        .Include(x => x.ClientBranch)
        //        .Include(x => x.Insured.LossState)
        //        .Include(x => x.ClaimEmployerDetails)
        //        .Include(x => x.ClaimDiaries)
        //        .Include(x => x.ClaimInjuryDetails);
        //    if (AuthUser.User.Roles == "PortalAdmin")
        //    {
        //        res = res.Where(x => x.PortalID == claim.PortalID && (x.IsDeleted ?? false) == false).OrderByDescending(x => x.ClaimID);
        //    }
        //    else
        //    {
        //        if (claim.ClientID != 0)
        //            res = res.Where(x => x.PortalID == claim.PortalID && x.ClientID == claim.ClientID && (x.IsDeleted ?? false) == false).OrderByDescending(x => x.ClaimID);
        //        else if (claim.UserID != 0)
        //            res = res.Where(x => x.PortalID == claim.PortalID && x.UserID == claim.UserID && (x.IsDeleted ?? false) == false).OrderByDescending(x => x.ClaimID);
        //    }
        //    var ret = res.Select(x => new ClaimListVM()
        //    {
        //        InsuredName = x.Insured.InsuredName,
        //        PolicyType = x.refPolicyType.PolicyType,
        //        AdjusterName = x.UserAdjuster.FirstName + " " + x.UserAdjuster.LastName,
        //        DateEntered = x.DateEntered,
        //        LossDate = x.LossDate,
        //        InsurerClaim = x.InsurerClaimNumber,
        //        AdjusterFile = x.AdjusterFile,
        //        ClientName = x.Client.ClientName,
        //        InsurerBranch = x.ClientBranch.CBranchName,
        //        LossCity = x.Insured.LossCity,
        //        LossState = x.Insured.LossState.StateName,
        //        DiaryStatus = x.UserAdjuster == null ? "Assign Adjuster" :
        //        x.IsClaimClosed ? "File Closed" : x.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted != true).OrderBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName ?? x.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
        //        //DiaryStatus = _context.ClaimDiaries.Include(y => y.refDiaryStatu).Where(y => (y.IsDeleted ?? false) == false && y.ClaimID == x.ClaimID).OrderByDescending(y => y.IsCompleted).ThenBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
        //        LossType = "",
        //        ClaimID = x.ClaimID,
        //        ClaimTypeID = x.ClaimTypeID ?? 0,
        //        EmployerName = x.ClaimEmployerDetails.Where(y => y.ClaimID == x.ClaimID).FirstOrDefault().Employer,
        //        WCLossDate = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryDate,
        //        WCLossTime = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryTime,
        //        WCInjuryPeriod = x.ClaimInjuryDetails.Where(y => y.ClaimID == x.ClaimID && x.ClaimTypeID == 2).FirstOrDefault().InjuryPeriod
        //    }).ToList();


        //    var queryBuilder = Linq.Func((Expression<Func<ClaimListVM, bool>> selector, string val) => res);

        //    return queryBuilder;
        //}

        public TimeSpan To24HrTime(string time)
        {

            char[] delimiters = new char[] { ':', ' ' };
            string[] spltTime = time.Split(delimiters);

            int hour = Convert.ToInt32(spltTime[0]);
            int minute = Convert.ToInt32(spltTime[1]);
            int seconds = 0;

            string amORpm = spltTime[2];

            if (amORpm.ToUpper() == "PM")
            {
                hour = (hour % 12) + 12;
            }

            return new TimeSpan(hour, minute, seconds);
        }


        public List<sp_EmailToPastDueDate_Result> PastDueDateEmail()
        {
            var list = _context.sp_EmailToPastDueDate().ToList();
            return list;
        }
        public string getMaxInsurerClaimNumber()
        {

            var ret = (from t1 in _context.Claims

                       where t1.PortalID == AuthUser.User.PortalId && t1.UserID == AuthUser.User.UserId && t1.IsDeleted != true
                       orderby t1.ClaimID descending
                       select new ClaimViewModel
                      {
                          InsurerClaimNumber = t1.InsurerClaimNumber,
                          ClaimID = t1.ClaimID
                      });

            return (ret == null || ret.ToList().Count == 0) ? "0" : ret.ToList()[0].InsurerClaimNumber;
        }

    }
}
