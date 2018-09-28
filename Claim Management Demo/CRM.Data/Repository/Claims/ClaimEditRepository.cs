using System.Collections.Generic;
using CRM.Core.Model;
using CRM.Core.Repository.Claims;
using CRM.Core.ViewModel.Claim;
using System.Linq;
using System.Data.Entity;
using CRM.Core.ViewModel.ClaimWC;
using System;


namespace CRM.Data.Repository.Claims
{
    public class ClaimEditRepository : Repository<Claim>, IClaimEditRepository
    {
        private readonly CRMDbContext _context;
        public ClaimEditRepository(CRMDbContext context)
            : base(context)
        {
            _context = context;
        }

        public ClaimEditViewModel GetClaimDetilsByClaimId(int id)
        {
            return _context.Claims.Include(x => x.UserAdjuster).Include(x => x.Client).Where(x => x.ClaimID == id && x.IsDeleted != true).Select(x => new ClaimEditViewModel
              {
                  CompanyBranchID = x.CompanyBranchID,
                  PolicyNumber = x.PolicyNumber,
                  InsurerClaimNumber = x.InsurerClaimNumber,
                  AdjusterFile = x.AdjusterFile,
                  ClientID = x.ClientID,
                  ClientName = x.Client.ClientName,
                  ClientBranchID = x.ClientBranchID,
                  AdjusterUserID = x.AdjusterID,
                  UserName = x.UserAdjuster.FirstName + " " + x.UserAdjuster.LastName,
                  ReportTo = x.ReportTo,
                  SupervisorID = x.SupervisorID,
                  Severity = x.Severity,
                  EventType = x.EventType,
                  EventName = x.EventName,
                  CategoryId = x.CategoryId,
                  CatId = x.CategoryId,
                  StatringAdjusterFile = (_context.Portals.Where(m => m.PortalID == (_context.Users.Where(n => n.UserID == x.UserID).Select(n => n.UserID).FirstOrDefault())).Select(m => m.StartingAdjuster).FirstOrDefault()),
                  TypeofLoss = x.TypeofLoss,
                  LossDescription = x.LossDescription,
                  LossDate = x.LossDate,
                  DateReceived = x.DateReceived,
                  DateEntered = x.DateEntered,
                  ClaimID = x.ClaimID,
                  LinkClaimID = x.LinkClaimID,
                  ReceivedFrom = x.ReceivedFrom,
                  ClaimTypeID = x.ClaimTypeID ?? 0,
                  IsWatchList = x.isWathcList,
                  TimeReceived = x.TimeReceived,
                  TimeReceivedPeriod = x.TimeReceivedPeriod,
                  UserID = x.UserID
              }).FirstOrDefault();
        }
        public Claim GetPortalDetailsByClaimId(int claimId)
        {
            return _context.Claims
                .Include(x => x.Portal)
                .Include(x => x.User).Include(x => x.User.refState)
                .Include(x => x.Client).Include(x => x.Client.refState)
                .Include(x => x.Insured).Include(x => x.XactAnalysi)
                .Include(x => x.UserAdjuster).Where(x => x.Portal.IsDeleted != true).FirstOrDefault(x => x.ClaimID == claimId);
        }

        public ClaimSideBarViewModel GetClaimDetailsForSidebar(int ClaimId)
        {
            ClaimSideBarViewModel details = null;
            ClaimSideBarViewModel diaryDetails = null;
            ClaimSideBarViewModel activeDiary = null;

            details = (from clm in _context.Claims
                       join cln in _context.Clients on clm.ClientID equals cln.ClientID
                       join clb in _context.ClientBranches on clm.ClientBranchID equals clb.CBranchId into joined
                       from clientBranch in joined.DefaultIfEmpty()
                       join us in _context.Users on clm.AdjusterID equals us.UserID into userjoin
                       from user in userjoin.DefaultIfEmpty()
                       join rct in _context.refClaimTypes on clm.ClaimTypeID equals rct.ClaimTypeID into claimjoin
                       from claim in claimjoin.DefaultIfEmpty()
                       join rcb in _context.ClientBillingProfiles on clm.BillingProfileID equals rcb.BillProfileID into billjoin
                       from billingprofile in billjoin.DefaultIfEmpty()
                       join cc in _context.ClientContacts on clm.ReportTo equals cc.ClientContactId into contactjoin
                       from clientContact in contactjoin.DefaultIfEmpty()
                       join loss in _context.refTypeOfLosses on clm.TypeofLoss equals loss.TypeOfLossID into lossjoin
                       from typeofloss in lossjoin.DefaultIfEmpty()
                       join client in _context.Clients on clm.ClientID equals client.ClientID
                       join claimInjury in _context.ClaimInjuryDetails on clm.ClaimID equals claimInjury.ClaimID into wcLossjoin
                       from WCInjury in wcLossjoin.DefaultIfEmpty()

                       where clm.ClaimID == ClaimId
                       select new ClaimSideBarViewModel()
                       {
                           Adjuster = user.FirstName + " " + user.LastName,
                           BillingPrograme = billingprofile.ProgrameName,
                           PolicyNumber = clm.PolicyNumber ?? clm.AdjusterFile,
                           InsurerClaimNumber = clm.InsurerClaimNumber,
                           CompanyBranchName = clientBranch.CBranchName,
                           ClaimType = claim.ClaimTypeName,
                           ClientBranchName = clientBranch.CBranchName,
                           ReportTo = clientContact.CCFirstName + " " + clientContact.CCLastName,
                           TypeOfLoss = clm.ClaimTypeID == 2 ? _context.refIncidentTypes.Where(y => y.ID == WCInjury.IncidenttypeId).FirstOrDefault().IncedentTypeName : typeofloss.TypeOfLoss,
                           ClientId = client.ClientID,
                           Client = client.ClientName,
                           LossDate = clm.ClaimTypeID == 2 ? WCInjury.InjuryDate : clm.LossDate,
                           AdjFileNumber = clm.AdjusterFile,
                           ClientContactName = clm.ClientContact.CCFirstName + " " + clm.ClientContact.CCLastName,
                           lossdescription = clm.LossDescription,
                           InsuredDetailSideBarViewModel = (from ins in _context.Insureds
                                                            where ins.ClaimID == ClaimId
                                                            select new InsuredDetailSideBarViewModel()
                                                            {
                                                                Name = ins.InsuredName,
                                                                BusinessPhone = ins.BusinessPhoneNumber,
                                                                HomePhone = ins.HomePhoneNumber,
                                                                MobilePhone = ins.MobileNumber,
                                                                PrimaryEmail = ins.PrimaryEmail,
                                                                SecondryEmail = ins.SecondaryEmail

                                                            }).FirstOrDefault(),
                           ClientDetailSideBarViewModel = (new ClientDetailSideBarViewModel()
                                                               {
                                                                   Address = clm.Client.BillingAdd1 + " " + clm.Client.BillingAdd2,
                                                                   Email = clm.Client.ReportingEmail,
                                                                   Name = clm.Client.ClientName,
                                                                   PhoneNumber = clm.Client.Phone,
                                                                   Contact = clm.ClientContact.CCFirstName + " " + clm.ClientContact.CCLastName,
                                                                   Fax = clm.Client.FaxNumber
                                                               }),
                           ReportToDetailSideBarViewModel = (
                           new ReportToDetailSideBarViewModel()
                           {
                               Name = clm.ClientContact.CCFirstName + " " + clm.ClientContact.CCLastName,
                               Address = clm.ClientContact.CCAddress1 + " " + clm.ClientContact.CCAddress2,
                               Email = clm.ClientContact.CCEmail,
                               Phone = clm.ClientContact.CCPhoneNumber

                           }
                           ),
                           AdjusterDetailSideBarViewModel = (from usr in _context.Users
                                                             where usr.UserID == clm.UserID && usr.IsAdjuster
                                                             select new AdjusterDetailSideBarViewModel()
                            {
                                Address = usr.Address1 + " " + usr.Address2,
                                CompanyPhone = (_context.Users.Where(x => x.PortalID == usr.PortalID && x.RoleID == _context.refRoles.Where(v => v.RoleName == "PortalAdmin").Select(m => m.RoleId).FirstOrDefault()).Select(y => y.Phone)).FirstOrDefault(),
                                Email = usr.Email,
                                Employee = usr.EmployeeNum,
                                Mobile = usr.Phone,
                                Name = usr.FirstName + " " + usr.LastName,
                            }).FirstOrDefault()
                       }).FirstOrDefault();

            if (details == null)
            {
                details = new ClaimSideBarViewModel();
            }

            //diaryDetails = (from diary in _context.ClaimDiaries
            //                join rds in _context.refDiaryStatus on diary.DiaryStatusID equals rds.DiaryStatusID
            //                where diary.ClaimID == ClaimId && diary.IsCompleted != true && diary.IsDeleted != true
            //                orderby diary.DueDate
            //                select new ClaimSideBarViewModel()
            //                {
            //                    NextDiaryDue = rds.DiaryStatusName,
            //                    NextDiaryDueDate = diary.DueDate,

            //                }).FirstOrDefault();





            diaryDetails = (from c in _context.Claims

                            where c.ClaimID == ClaimId

                            select new ClaimSideBarViewModel()
                            {
                                // NextDiaryDue = claim.UserAdjuster == null ? "Assign Adjuster" : claim.IsClaimClosed ? "File Closed" : refDiary.DiaryStatusName ?? _context.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
                                NextDiaryDue = c.UserAdjuster == null ? "Assign Adjuster" : c.IsClaimClosed ? "File Closed" : c.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted != true).OrderBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName ?? c.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
                                NextDiaryDueDate = c.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted != true).OrderBy(y => y.DueDate).FirstOrDefault().DueDate == null ? c.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().DueDate : c.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().DueDate,


                            }).FirstOrDefault();

            //for Active diary
            if (_context.Claims.Any(x => x.ClaimID == ClaimId && (x.IsDeleted ?? false) == false && x.AdjusterID != null))
            {
                activeDiary = _context.ClaimDiaries.Include(x => x.refDiaryStatu).Where(x => x.ClaimID == ClaimId && (x.IsDeleted ?? false) == false && x.IsCompleted == false).OrderBy(x => x.DueDate).Select(x => new ClaimSideBarViewModel
                {
                    ActiveDiaryStatus = x.refDiaryStatu.DiaryStatusName,
                    ActiveDiaryStatusId = x.refDiaryStatu.DiaryStatusID
                }).FirstOrDefault();
            }
            else
            {
                activeDiary = new ClaimSideBarViewModel
                {
                    ActiveDiaryStatus = "Assign Adjuster",
                    ActiveDiaryStatusId = 0
                };
            }


            //if (insuredDetails != null)
            //{
            //    details.Insured = insuredDetails.Insured;

            //}
            //else
            //{
            //    details.Insured = "";
            //}
            if (diaryDetails != null)
            {
                details.NextDiaryDue = diaryDetails.NextDiaryDue;
                details.NextDiaryDueDate = diaryDetails.NextDiaryDueDate;
                if (diaryDetails.NextDiaryDueDate != null)
                {
                    System.DateTime currentdate = System.DateTime.Now;
                    System.TimeSpan ts = currentdate.Subtract((System.DateTime)diaryDetails.NextDiaryDueDate);
                    if (ts.TotalDays > 0)
                    {
                        details.Datelate = ts.TotalDays;
                    }
                    else
                    {
                        details.Datelate = 0;
                    }

                }

            }
            else
            {
                details.NextDiaryDue = null;
                details.NextDiaryDueDate = null;
                details.Datelate = 0;
            }
            decimal? totalreservebalance = (from r in _context.ClaimReserves
                                            where r.ClaimID == ClaimId
                                            select r.ReserveBalance).Sum();

            decimal? netClaimPayable = (from r in _context.ClaimLossTotals
                                        where r.ClaimID == ClaimId
                                        select r.ClaimGrossNetPayable).Sum();

            details.LossDateStr = details.LossDate.GetValueOrDefault().Date.ToShortDateString() == "1/1/0001" ? "" : details.LossDate.GetValueOrDefault().Date.ToShortDateString();
            details.NextDiaryDueDateStr = details.NextDiaryDueDate.GetValueOrDefault().ToShortDateString();

            details.OutstandingReserve = totalreservebalance == null ? "" : Convert.ToDecimal(totalreservebalance).ToString("C");
            details.NetClaimPayble = netClaimPayable;

            //assign active diary value
            if (activeDiary != null)
            {
                details.ActiveDiaryStatus = activeDiary.ActiveDiaryStatus;
                details.ActiveDiaryStatusId = activeDiary.ActiveDiaryStatusId;
            }

            return details;

        }


        public ClaimSideBarViewModel GetDiaryStatus(int ClaimId)
        {
            //ClaimSideBarViewModel DiaryDetails = (from diary in _context.ClaimDiaries
            //                                      join rds in _context.refDiaryStatus on diary.DiaryStatusID equals rds.DiaryStatusID
            //                                      where diary.ClaimID == ClaimId && diary.IsCompleted != true && diary.IsDeleted != true
            //                                      orderby diary.DueDate
            //                                      select new ClaimSideBarViewModel()
            //                                      {
            //                                          status = rds.DiaryStatusName,
            //                                          NextDiaryDueDate = diary.DueDate,

            //                                      }).FirstOrDefault();

            ClaimSideBarViewModel DiaryDetails = (from c in _context.Claims
                                                  where c.ClaimID == ClaimId
                                                  select new ClaimSideBarViewModel()
                                                  {
                                                      NextDiaryDue = c.UserAdjuster == null ? "Assign Adjuster" : c.IsClaimClosed ? "File Closed" : c.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted != true).OrderBy(y => y.DueDate).FirstOrDefault().refDiaryStatu.DiaryStatusName ?? c.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted).OrderByDescending(y => y.CompletedDate).FirstOrDefault().refDiaryStatu.DiaryStatusName,
                                                      NextDiaryDueDate = c.ClaimDiaries.Where(y => (y.IsDeleted ?? false) == false && y.IsCompleted != true).OrderBy(y => y.DueDate).FirstOrDefault().DueDate,

                                                  }).FirstOrDefault();
            return DiaryDetails;
        }


    }
}
