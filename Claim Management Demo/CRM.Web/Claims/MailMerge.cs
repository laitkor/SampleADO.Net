using CRM.Core.Model;
using CRM.Core.Repository.Claims;
using CRM.Data;
using CRM.Data.Repository.Claims;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CRM.Web.Areas.Claims
{
    public class MailMerge
    {
        private readonly IClaimRepository _claims;
        private readonly CRMDbContext _context;
        public MailMerge()
        {
            _context = new CRMDbContext();
            _claims = new ClaimRepository(_context);
        }

        public Document mergeDocument(Document document, int id)
        {
            var datadoc = getData(id);
            document = findandreplace(document, datadoc);
            return document;
        }

        private mailmergedocument getData(int id)
        {
            // Warning because variables where assigned but never used
            var ret = true;

            var ret1 = 1;

            var suffixID = _context.ClaimantDetails.Where(u => u.ClaimID == id).FirstOrDefault() == null  ? 0 :  _context.ClaimantDetails.Where(u => u.ClaimID == id).FirstOrDefault().SuffixID;
            var LienHoldersStateID = _context.LienHolders.Where(u => u.ClaimID == id).FirstOrDefault() == null ? 0 : _context.LienHolders.Where(u => u.ClaimID == id).FirstOrDefault().StateID;

            var objClaim = _claims.GetAllInclude(
                    x => x.Portal,
                    x => x.User,
                    x => x.User.refState,
                    x => x.Client,
                    x => x.Client.refState1,
                    x => x.Insured,
                    x => x.UserSupervisor,
                    x => x.UserAdjuster,
                    x => x.Portal.Users,
                    x => x.refTypeOfLoss,
                    x => x.ClientBranch,
                    x => x.LienHolders,
                    x => x.ClientContact,
                    x => x.UserSupervisor,
                    x => x.refPolicyType,
                    x => x.Insured.LossState,
                    x => x.Insured.refState,
                    x => x.ClaimantDetails,
                    x => x.ClientContact.refState

                    )
                    .Where(x => x.ClaimID == id).ToList().Select(y => new mailmergedocument
                    {
                        ADJUSTER_COMPANY_PHONE = y.UserAdjuster == null ? "" : y.UserAdjuster.Phone,
                        ADJUSTER_EMAIL = y.UserAdjuster == null ? "" : y.UserAdjuster.Email,
                        ADJUSTER_MOBILE_PHONE = y.UserAdjuster == null ? "" : y.UserAdjuster.FaxNumber,
                        //ADJUSTER_NAME = y.UserAdjuster == null ? "" : y.UserAdjuster.FirstName + " " + y.UserAdjuster == null ? "" : y.UserAdjuster.LastName,
                        ADJUSTER_NAME = y.UserAdjuster == null ? "" : string.Format("{0} {1}", y.UserAdjuster.FirstName, y.UserAdjuster.LastName),
                        ADJUSTER_FILE_NUMBER = y.AdjusterFile,
                        INSURER_CLAIM_NUMBER = y.InsurerClaimNumber,
                        CARRIER_CLIENT_CITY = y.Client == null ? "" : y.Client.PhysicalCity,
                        CARRIER_CLIENT_EMAIL = y.Client == null ? "" : y.Client.ReportingEmail,
                        CARRIER_CLIENT_FAXNUMBER = y.Client == null ? "" : y.Client.FaxNumber,
                        CARRIER_CLIENT_NAME = y.Client == null ? "" : y.Client.ClientName,
                        CARRIER_CLIENT_PHONE_NUMBER = y.Client == null ? "" : y.Client.Phone,

                        CARRIER_CLIENT_STATE = y.Client == null ? "" : y.Client.PhysicalStateID == null ? "" : _context.refStates.Where(s => s.StateID == y.Client.PhysicalStateID).Select(s => s.StateName).FirstOrDefault() == null ? "" : _context.refStates.Where(s => s.StateID == y.Client.PhysicalStateID).FirstOrDefault().StateName,
                        CARRIER_CLIENT_STREETADDRESS1 = y.Client == null ? "" : y.Client.PhysicalAdd1,
                        CARRIER_CLIENT_STREETADDRESS2 = y.Client == null ? "" : y.Client.PhysicalAdd2,
                        CARRIER_CLIENT_ZIPCODE = y.Client == null ? "" : y.Client.PhysicalZipCode,
                        COMPANY_ADDRESS = y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().Address1 + " " + y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().Address2,
                        COMPANY_CITY = y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().City,
                        COMPANY_EMAIL = y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().Email,
                        COMPANY_FAX = y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().FaxNumber,
                        COMPANY_NAME = y.Portal.CompanyName,
                        COMPANY_PHONE_NUMBER = y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().Phone,
                        //COMPANY_STATE = y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().refState == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().refState.StateName,
                        //COMPANY_STATE = y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().refState == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).Select(u => u.StateID).FirstOrDefault() == null ? "" : _context.refStates.Where(s => s.StateID == y.Portal.Users.Where(n => n.RoleID == 2).Select(u => u.StateID).FirstOrDefault()).Select(s => s.StateName).FirstOrDefault() ?? "",
                        COMPANY_ZIP = y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault() == null ? "" : y.Portal.Users.Where(n => n.RoleID == 2).FirstOrDefault().ZipCode,
                        COMPANY_LOGO = y.Portal.LogoPath,
                        FEDERAL_ID_NO = y.Portal.FederalIDNo,
                        DATE_ACKNOWLEDGED = y.DateAssigned == null ? "" : string.Format("{0:MM/dd/yyyy}", y.DateAssigned),
                        DATE_ASSIGNED = y.ClaimCloseDate == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimCloseDate),
                        DATE_CLIENT_APPROVED = y.DateEntered == null ? "" : string.Format("{0:MM/dd/yyyy}", y.DateEntered),
                        DATE_CLOSED = y.DateReceived == null ? "" : string.Format("{0:MM/dd/yyyy}", y.DateReceived),
                        EVENT_NAME = y.EventName ?? "",
                        EVENT_TYPE = y.EventType ?? "",
                        SEVERITY = y.Severity == null ? "false" : y.Severity == 1 ? "true" : "false",
                        STATUS = "",
                        CAT_ID = y.CategoryId,
                        //TYPE_OF_LOSS = y.refTypeOfLoss == null ? "" : y.refTypeOfLoss.TypeOfLoss,
                        TYPE_OF_LOSS = y.TypeofLoss == null ? "" : _context.refTypeOfLosses.FirstOrDefault(x => x.TypeOfLossID == (int)y.TypeofLoss).TypeOfLoss ?? "",
                        LOSS_DESCRIPTION = y.LossDescription,
                        POLICY_NO = y.PolicyNumber,
                        EXPIRATION_DATE = y.ExpirationDate == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ExpirationDate),
                        INITIAL_COVERAGE_DATE = y.InitialCoverageDate == null ? "" : string.Format("{0:MM/dd/yyyy}", y.InitialCoverageDate),
                        TYPE_OF_POLICY = y.refPolicyType == null ? "" : y.refPolicyType.PolicyType,
                        POLICY_FORM_TYPE = y.PolicyFormType,
                        INSURED_BUSINESS_PHONE = y.Insured == null ? "" : y.Insured.BusinessPhoneNumber,
                        INSURED_HOME_PHONE = y.Insured == null ? "" : y.Insured.HomePhoneNumber,
                        INSURED_MOBILE_PHONE = y.Insured == null ? "" : y.Insured.MobileNumber,
                        INSURED_NAME = y.Insured == null ? "" : y.Insured.InsuredName,
                        LOSS_ADDRESS = y.Insured == null ? "" : y.Insured.LossAddress1,
                        LOSS_ADDRESS2 = y.Insured == null ? "" : y.Insured.LossAddress2,
                        LOSS_CITY = y.Insured == null ? "" : y.Insured.LossCity,
                        LOSS_LOCATION = y.Insured == null ? "" : y.Insured.LossLocation,
                        LOSS_STATE = y.Insured == null ? "" : y.Insured.LossState == null ? "" : y.Insured.LossState.StateName,
                        LOSS_ZIPCODE = y.Insured == null ? "" : y.Insured.LossZip,
                        LOSS_DATE = y.LossDate == null ? "" : string.Format("{0:MM/dd/yyyy}", y.LossDate),
                        PRIMARY_EMAIL = y.Insured == null ? "" : y.Insured.PrimaryEmail,
                        MAILING_ADDRESS = y.Insured == null ? "" : y.Insured.MailingAddress1,
                        MAILING_ADDRESS2 = y.Insured == null ? "" : y.Insured.MailingAddress2,
                        MAILING_CITY = y.Insured == null ? "" : y.Insured.MailingCity,
                        // MAILING_STATE = y.Insured == null ? "" : y.Insured.refState == null ? "" : y.Insured.refState.StateName,
                        MAILING_STATE = y.Insured == null ? "" : y.Insured.refState == null ? "" : y.Insured.MailingStateID == null ? "" : _context.refStates.Where(s => s.StateID == y.Insured.MailingStateID).FirstOrDefault().StateName ?? "",
                        MAILING_ZIP = y.Insured == null ? "" : y.Insured.MailingZip,
                        INSURER_BRANCH_ADDRESS1 = y.ClientBranch == null ? "" : y.ClientBranch.CBranchAddress,
                        INSURER_BRANCH_ADDRESS2 = y.ClientBranch == null ? "" : y.ClientBranch.CBranchAddress2,
                        INSURER_BRANCH_CITY = y.ClientBranch == null ? "" : y.ClientBranch.CBranchCity,
                        INSURER_BRANCH_NAME = y.ClientBranch == null ? "" : y.ClientBranch.CBranchName,
                        // INSURER_BRANCH_STATE = y.ClientBranch == null ? "" : y.ClientBranch.refState == null ? "" : y.ClientBranch.refState.StateName,
                        INSURER_BRANCH_STATE = y.ClientBranch == null ? "" : y.ClientBranch.refState == null ? "" : y.ClientBranch.CBranchStateID == null ? "" : _context.refStates.Where(s => s.StateID == y.ClientBranch.CBranchStateID).FirstOrDefault().StateName ?? "",
                        INSURER_BRANCH_ZIPCODE = y.ClientBranch == null ? "" : y.ClientBranch.CBrnahcZip,
                        //---------------Comment by Khalda---------------------
                        //LOANNUMBER = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().LoanNumber,
                        //LIENHOLDER = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().LienHolderName,
                        //LIENHOLDER_ADDRESS1 = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().Address1,
                        //LIENHOLDER_ADDRESS2 = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().Address2,
                        //LIENHOLDER_CITY = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().City,
                        ////LIENHOLDER_STATE = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().refState == null ? "" : y.LienHolders.FirstOrDefault().refState.StateName,
                        //LIENHOLDER_STATE = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().refState == null ? "" : y.LienHolders.FirstOrDefault().StateID == null ? "" : _context.refStates.Where(s => s.StateID == y.LienHolders.FirstOrDefault().StateID).Select(s => s.StateName).FirstOrDefault() ?? "",
                        //LIENHOLDER_ZIP = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().PostalCode,
                        //LIENHOLDER_PHONE = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().PhoneNumber,
                        //------------------------------------------------------


                        //---------------By Khalda--------------
                        LOANNUMBER = y.LienHolders == null ? "" : (y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().LoanNumber),
                        LIENHOLDER = y.LienHolders == null ? "" : (y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().LienHolderName),
                        LIENHOLDER_ADDRESS1 = y.LienHolders == null ? "" : (y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().Address1),
                        LIENHOLDER_ADDRESS2 = y.LienHolders == null ? "" : (y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().Address2),
                        LIENHOLDER_CITY = y.LienHolders == null ? "" : (y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().City),
                        //LIENHOLDER_STATE = y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().refState == null ? "" : y.LienHolders.FirstOrDefault().refState.StateName,
                        LIENHOLDER_STATE = y.LienHolders == null ? "" : (y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().refState == null ? "" : y.LienHolders.FirstOrDefault().StateID == null ? "" : _context.refStates.Where(s => s.StateID == LienHoldersStateID).FirstOrDefault().StateName ?? ""),
                        LIENHOLDER_ZIP = y.LienHolders == null ? "" : (y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().PostalCode),
                        LIENHOLDER_PHONE = y.LienHolders == null ? "" : (y.LienHolders.FirstOrDefault() == null ? "" : y.LienHolders.FirstOrDefault().PhoneNumber),
                        //--------------------------------------

                        REPORT_TO = y.ClientContact == null ? "" : y.ClientContact.CCFirstName + " " + y.ClientContact == null ? "" : y.ClientContact.CCLastName,
                        REPORT_TO_ADDRESS1 = y.ClientContact == null ? "" : y.ClientContact.CCAddress1,
                        REPORT_TO_ADDRESS2 = y.ClientContact == null ? "" : y.ClientContact.CCAddress2,
                        REPORT_TO_CITY = y.ClientContact == null ? "" : y.ClientContact.CCCity,
                        // REPORT_TO_STATE = y.ClientContact == null ? "" : y.ClientContact.refState == null ? "" : y.ClientContact.refState.StateName,
                        REPORT_TO_STATE = y.ClientContact == null ? "" : y.ClientContact.refState == null ? "" : y.ClientContact.CCStateID == null ? "" : _context.refStates.Where(s => s.StateID == y.ClientContact.CCStateID).FirstOrDefault().StateName ?? "",
                        REPORT_TO_ZIP = y.ClientContact == null ? "" : y.ClientContact.CCZipCode,
                        REPORT_TO_PHONE = y.ClientContact == null ? "" : y.ClientContact.CCPhoneNumber,
                        REPORT_TO_FAX = y.ClientContact == null ? "" : y.ClientContact.CCFax,
                        REPORT_TO_EMAIL = y.ClientContact == null ? "" : y.ClientContact.CCEmail,
                        SUPERVISOR = y.UserSupervisor == null ? "" : y.UserSupervisor.FirstName + " " + y.UserSupervisor == null ? "" : y.UserSupervisor.LastName,
                        SUPERVISOR_PHONE = y.UserSupervisor == null ? "" : y.UserSupervisor.Phone,
                        SUPERVISOR_EMAIl = y.UserSupervisor == null ? "" : y.UserSupervisor.Email,
                        TODAYS_DATE = string.Format("{0:MM/dd/yyyy}", DateTime.Now),


                        // property
                        Date_of_Loss = y.LossDate == null ? "" : string.Format("{0:MM/dd/yyyy}", y.LossDate),
                        Date_Received = y.DateReceived == null ? "" : string.Format("{0:MM/dd/yyyy}", y.DateReceived),
                        Date_Entered = y.DateEntered == null ? "" : string.Format("{0:MM/dd/yyyy}", y.DateEntered),
                        Effective_Date = y.EffectiveDate == null ? "" : string.Format("{0:MM/dd/yyyy}", y.EffectiveDate),
                        Insured_Primary_Email = y.Insured == null ? "" : y.Insured.PrimaryEmail == null ? "" : y.Insured.PrimaryEmail,
                        Insured_Fax_Number = y.Insured == null ? "" : y.Insured.Fax == null ? "" : y.Insured.Fax,
                        Secondary_Email = y.Insured == null ? "" : y.Insured.SecondaryEmail == null ? "" : y.Insured.SecondaryEmail,
                        Policy_Coverage = y.ClaimCoverages == null ? "" : y.ClaimCoverages.FirstOrDefault() == null ? "" : y.ClaimCoverages.FirstOrDefault().CoverageName,
                        Policy_Coverage_Limit = y.ClaimCoverages == null ? "" : y.ClaimCoverages.FirstOrDefault() == null ? "" : y.ClaimCoverages.FirstOrDefault().Limit == null ? "" : y.ClaimCoverages.FirstOrDefault().Limit.ToString(),
                        Policy_Deductible = y.ClaimCoverages == null ? "" : y.ClaimCoverages.FirstOrDefault() == null ? "" : y.ClaimCoverages.FirstOrDefault().Deductible == null ? "" : y.ClaimCoverages.FirstOrDefault().Deductible.ToString(),
                        Adjuster_First_Name = y.UserAdjuster != null && !string.IsNullOrEmpty(y.UserAdjuster.FirstName) ? y.UserAdjuster.FirstName : "",

                        //end
                        ////wc
                        ////From the Employer tab page
                        Department = y.ClaimEmployerDetails == null ? "" : _context.refDepartments.Where(z => z.DepartmentID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().DepartmentID)).FirstOrDefault() == null ? "" : _context.refDepartments.Where(z => z.DepartmentID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().DepartmentID)).FirstOrDefault().DepartmentName,
                        Location = y.ClaimEmployerDetails == null ? "" : _context.refLocations.Where(z => z.LocationID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().LocationID)).FirstOrDefault() == null ? "" : _context.refLocations.Where(z => z.LocationID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().LocationID)).FirstOrDefault().LocationName,

                        Division = y.ClaimEmployerDetails == null ? "" : _context.refDivisions.Where(z => z.DivisionID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().DivisionID)).FirstOrDefault() == null ? "" : _context.refDivisions.Where(z => z.DivisionID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().DivisionID)).FirstOrDefault().DivisionName,

                        Plant = y.ClaimEmployerDetails == null ? "" : _context.refPlants.Where(z => z.PlantID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().PlantID)).FirstOrDefault() == null ? "" : _context.refPlants.Where(z => z.PlantID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().PlantID)).FirstOrDefault().PlantName,
                        Shift = y.ClaimEmployerDetails == null ? "" : _context.refWorkShifts.Where(z => z.WorkShiftID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().ShiftID)).FirstOrDefault() == null ? "" : _context.refWorkShifts.Where(z => z.WorkShiftID == (y.ClaimEmployerDetails.FirstOrDefault() == null ? 0 : y.ClaimEmployerDetails.FirstOrDefault().ShiftID)).FirstOrDefault().WorkShiftName,
                        Reported_By = y.ClaimEmployerDetails == null ? "" : y.ClaimEmployerDetails.FirstOrDefault() == null ? "" : y.ClaimEmployerDetails.FirstOrDefault().ReportedBy,
                        Reported_By_Phone = y.ClaimEmployerDetails == null ? "" : y.ClaimEmployerDetails.FirstOrDefault() == null ? "" : y.ClaimEmployerDetails.FirstOrDefault().ReportedByPhn,
                        Reported_By_Email = y.ClaimEmployerDetails == null ? "" : y.ClaimEmployerDetails.FirstOrDefault() == null ? "" : y.ClaimEmployerDetails.FirstOrDefault().ReportedByEmail,

                        //    //end
                        //From the Claimant tab page
                        Claimant_First_Name = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : (y.ClaimantDetails.FirstOrDefault().FirstName == null ? "" : y.ClaimantDetails.FirstOrDefault().FirstName),
                        Claimant_Last_Name = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : (y.ClaimantDetails.FirstOrDefault().LastName == null ? "" : y.ClaimantDetails.FirstOrDefault().LastName),
                        //Suffix = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().refSuffix.Suffix,
                       // Suffix = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().SuffixID == null ? "" : _context.refSuffixes.Where(su => su.SuffixID == y.ClaimantDetails.FirstOrDefault().SuffixID).FirstOrDefault().Suffix ?? "",

                        Suffix = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().SuffixID == null ? "" : _context.refSuffixes.Where(su => su.SuffixID == suffixID).FirstOrDefault().Suffix ?? "",
                        Employee_Number = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().EmployeeNumber,
                        Employment_Start_Date = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : (y.ClaimantDetails.FirstOrDefault().EmloymentStartDate == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimantDetails.FirstOrDefault().EmloymentStartDate)),


                        SSN = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().SSNNo,
                        Date_of_Birth = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : (y.ClaimantDetails.FirstOrDefault().DOB == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimantDetails.FirstOrDefault().DOB)),
                        Employee_Classification_Code = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().EmployeeClassificationNumber,
                        Classification_Description = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().ClassificationDescription,
                        Mailing_Address_1 = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().MailingAdd1,
                        Mailing_Address_2 = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().MailingAdd2,
                        City = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().City,
                        // State = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().refState.StateName,
                        //   State = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().StateID == null ? "" : _context.refStates.Where(s => s.StateID == y.ClaimantDetails.FirstOrDefault().StateID).Select(s => s.StateName).FirstOrDefault() ?? "",
                        Zip = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().ZipCode,
                        Home_Phone = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().HomePhnNo,
                        Business_Phone = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().BusinessPhnNo,
                        Cell_Phone = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().CellPhnNo,
                        Primary_Email = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().PrimaryEmail,
                        // Race_Ethnicity = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().refEthnicity.EthnicityName,
                        //  Race_Ethnicity = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().EthnicityID == null ? "" : _context.refEthnicities.Where(e => e.EthnicityID == y.ClaimantDetails.FirstOrDefault().EthnicityID).Select(e => e.EthnicityName).FirstOrDefault() ?? "",
                        Primary_Language = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().PrimaryLanguage,
                        Occupation = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().Occupation,
                        // Marital_Status = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().refMaritalStatu.MaritalStatus,
                        //    Marital_Status = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().MaritalStatusID == null ? "" : _context.refMaritalStatus.Where(m => m.MaritalStatusID == y.ClaimantDetails.FirstOrDefault().MaritalStatusID).Select(m => m.MaritalStatus).FirstOrDefault(),
                        Number_of_Dependents = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().NoOfDependents == null ? "" : y.ClaimantDetails.FirstOrDefault().NoOfDependents.ToString(),
                        Emergency_Contact = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().EmergencyContact,
                        Emergency_Contact_Phone = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().EmergencyContactPhoneNumber,
                        Emergency_Contact_Relationship = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().EmergencyContactRelationship,
                        //end
                        //From the Injury tab page

                        Date_of_Injury = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimInjuryDetails.FirstOrDefault().InjuryDate),
                        Time_of_Injury = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().InjuryTime,
                        Date_Injury_Reported_to_Insured = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimInjuryDetails.FirstOrDefault().InjuryDateReportedToInsured),
                        Time_Injury_Reported_to_Insured = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().InjuryTimeReportedToInsured,
                        Date_Opened_Reported_to_Insurance_Company = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().DateReopenedToInsuranceCo == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimInjuryDetails.FirstOrDefault().DateReopenedToInsuranceCo),
                        Time_Opened_Reported_to_Insurance_Company = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().TimeReopenedToInsuranceCo ?? "",
                        Injury_Reported_To = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().InjuryReportedTo,
                        Witness_to_Injury = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().WitnessToInjury,
                        Witness_Phone = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().WitnessPhoneNo ?? "",

                        Where_did_Injury_Occur = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().WitnessPhoneNo,

                        Incident_Type = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().IncidenttypeId == null ? "" : _context.refIncidentTypes.Where(i => i.ID == y.ClaimInjuryDetails.FirstOrDefault().IncidenttypeId).FirstOrDefault().IncedentTypeName ?? "",
                        Description_of_Injury_or_Illness = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().InjuryDescription,

                        Machine = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().Machine,

                        Injury_Illness_that_occurred = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().CauseOfInjuryID == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().refCauseOfInjury.Name,
                        Part_of_Body_Affected = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().BodyPartCodeID == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().refBodyPartCode.BodyPartCode,
                        Cause_of_Accident = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().AccidentalCauseID == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().refCauseOfAccident.Name,

                        Sub_Cause_of_Accident = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().SubCauseOfAccidentID == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().refSubCauseOfAccident.Name,

                        Claimant_Cause = y.ClaimInjuryDetails == null ? "" : y.ClaimInjuryDetails.FirstOrDefault() == null ? "" : y.ClaimInjuryDetails.FirstOrDefault().ClaimantCause,

                        //

                        //From the Medical tab page
                        Authorized_Treating_Vendor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().AuthorizedTreatingVendor == null ? "" : y.Medicals.FirstOrDefault().AuthorizedTreatingVendor.ToString(),
                        Authorized_Treating_Doctor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().AuthorizedTreatingDoctor,
                        Referral_Doctor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().ReferalDoctor,
                        Employers_IME_Vendor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().EmployersIMEVendor == null ? "" : y.Medicals.FirstOrDefault().EmployersIMEVendor.ToString(),
                        Employers_IME_Doctor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().EmployersIMEDoctor,
                        Employees_IME_Vendor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().EmployeesIMEVendor == null ? "" : y.Medicals.FirstOrDefault().EmployeesIMEVendor.ToString(),
                        Employees_IME_Doctor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().EmployeesIMEDoctor,
                        Physical_Therapist_Vendor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().PhysicalTherapistVendor == null ? "" : y.Medicals.FirstOrDefault().PhysicalTherapistVendor.ToString(),
                        Physical_Therapist = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().PhysicalTherapist,
                        Second_Opinion_Vendor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().SecondOpinionVendor == null ? "" : y.Medicals.FirstOrDefault().SecondOpinionVendor.ToString(),
                        Second_Opinion_Doctor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().SecondOpinionDoctor,
                        Has_Drug_Program = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().HasDrugProgram == null ? "" : y.Medicals.FirstOrDefault().HasDrugProgram.ToString(),
                        Is_Drug_Tested = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().IsDrugTested == null ? "" : y.Medicals.FirstOrDefault().IsDrugTested.ToString(),
                        Test_Result = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().TestResult == null ? "" : y.Medicals.FirstOrDefault().TestResult.ToString(),
                        Case_Manager_Vendor = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().CaseManagerVendor == null ? "" : y.Medicals.FirstOrDefault().CaseManagerVendor.ToString(),
                        Case_Manager = y.Medicals == null ? "" : y.Medicals.FirstOrDefault() == null ? "" : y.Medicals.FirstOrDefault().CaseManager,

                        //end

                        //From the Indemnity(Wages & Comp) tab page
                        Home_Company = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().HomeCompanyNum,
                        Home_Company_Division = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().HomeDivision,
                        Home_Company_Plant = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().HomePlant,
                        Home_Company_Department = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().HomeDepartment,
                        Home_Company_Shift = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().HomeShift == null ? "" : y.IndemnityWageComps.FirstOrDefault().HomeShift.ToString(),
                        Home_Supervisor_Name = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().HomeSupervisor,
                        Regular_Weekly_Pay = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().RegularWeeklyPay == null ? "" : y.IndemnityWageComps.FirstOrDefault().RegularWeeklyPay.ToString(),
                        Hourly_Pay_Rate = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().HourlyPayRate == null ? "" : y.IndemnityWageComps.FirstOrDefault().HourlyPayRate.ToString(),
                        Number_Hours_Per_Day = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().NumberHoursPerDay == null ? "" : y.IndemnityWageComps.FirstOrDefault().NumberHoursPerDay.ToString(),
                        Number_Hours_Per_Week = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().NumberHoursPerWeek == null ? "" : y.IndemnityWageComps.FirstOrDefault().NumberHoursPerWeek.ToString(),
                        Days_Worked_Per_Week = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().DaysWorkedPerWeek == null ? "" : y.IndemnityWageComps.FirstOrDefault().DaysWorkedPerWeek.ToString(),
                        Gross_Wages = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().GrossWage == null ? "" : y.IndemnityWageComps.FirstOrDefault().GrossWage.ToString(),
                        Income_Allowance = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().IncomeAllowance == null ? "" : y.IndemnityWageComps.FirstOrDefault().IncomeAllowance.ToString(),
                        Number_of_Weeks = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().NumberOfWeeks == null ? "" : y.IndemnityWageComps.FirstOrDefault().NumberOfWeeks.ToString(),
                        TPD_Compensation_Rate = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().TPDCompRate == null ? "" : y.IndemnityWageComps.FirstOrDefault().TPDCompRate.ToString(),
                        TTD_Compensation_Rate = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().TTDCompRate == null ? "" : y.IndemnityWageComps.FirstOrDefault().TTDCompRate.ToString(),
                        PPD_Rate = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().PPDRate == null ? "" : y.IndemnityWageComps.FirstOrDefault().PPDRate.ToString(),
                        PTD_Rate = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().PTDRate == null ? "" : y.IndemnityWageComps.FirstOrDefault().PTDRate.ToString(),
                        PPI_SM_or_WB = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().PPISM,
                        PPI_Rating = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().PPIRating == null ? "" : y.IndemnityWageComps.FirstOrDefault().PPIRating.ToString(),
                        Dpndt_Rate = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().DependentRate == null ? "" : y.IndemnityWageComps.FirstOrDefault().DependentRate.ToString(),
                        Second_Injury_Trust_Fund = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().SecondInjuryTrustFund == null ? "" : y.IndemnityWageComps.FirstOrDefault().SecondInjuryTrustFund.ToString(),
                        Lost_Time_Restricted = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().LostTimeRestricted,
                        Lost_Time_From = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:hh:mm}", y.IndemnityWageComps.FirstOrDefault().LostTimeFrom),
                        Lost_Time_Through = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:hh:mm}", y.IndemnityWageComps.FirstOrDefault().LostTimeThrough),
                        Beginning_Date = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().BeginningDate),
                        End_Date = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().EndDate),
                        Amount = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : y.IndemnityWageComps.FirstOrDefault().Amount == null ? "" : y.IndemnityWageComps.FirstOrDefault().Amount.ToString(),
                        first_Date_Out_of_Work = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().FirstDateOutOfWork),
                        Final_Return_to_Work_Date = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().FinalDateReturnToWork),
                        Date_Released = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().DateReleased),
                        Date_Released_to_Return_to_Work = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().DateReleasedRTW),
                        Anticipated_Date_Return_to_Work = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().DateAnticipatedRTW),
                        Anticipated_MMI_Date = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().DateAnticipatedMMI),
                        MMI_Date = y.IndemnityWageComps == null ? "" : y.IndemnityWageComps.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.IndemnityWageComps.FirstOrDefault().DateMMI),
                        //end
                        //From the Legal tab page
                        //Jurisdiction = y.JurisdictionID == null ? "" : y.refState.StateName,
                        Jurisdiction = y.JurisdictionID == null ? "" : _context.refStates.Where(s => s.StateID == y.JurisdictionID).FirstOrDefault().StateName,
                        Suit_Filed = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().SuitField == null ? "" : y.ClaimLegals.FirstOrDefault().SuitField.ToString(),
                        Other_Attorney = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().OtherAttorney == null ? "" : y.ClaimLegals.FirstOrDefault().OtherAttorney.ToString(),
                        Employers_Voc_Disa = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().EmployeersVocDisa,
                        Employers_Voc_Exp = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().EmployeersVocExp),
                        State_File = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().StateFileNo,
                        Defense_Attorney = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().DefenseAttorney == null ? "" : y.ClaimLegals.FirstOrDefault().DefenseAttorney.ToString(),
                        County_of_Court = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().CountyOfCourt,
                        Employees_Voc_Disa = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().EmployeesVocDisa,
                        Employees_Voc_Exp = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().EmployeesVocExp),
                        Claim_Denied = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().ClaimDenied == null ? "" : y.ClaimLegals.FirstOrDefault().ClaimDenied.ToString(),
                        Plaintiff_Attorney = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().PlaintiffAttorney == null ? "" : y.ClaimLegals.FirstOrDefault().PlaintiffAttorney.ToString(),
                        Docket_Number = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().Docketno,
                        Case_Name = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().CaseName,
                        Court_Name = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().CourtName,
                        Lawsuit_Type = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().LawsuitType,
                        Lawsuit_City = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().LawsuitCity,
                        // Lawsuit_State = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().refState.StateName,
                        Lawsuit_Service_Type = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().LawsuitServiceType,
                        Matter_Reference = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().MatterReferenceNo,
                        Judge = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().JudgeID == null ? "" : y.ClaimLegals.FirstOrDefault().JudgeID.ToString(),
                        Mediator = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().MediatorID == null ? "" : y.ClaimLegals.FirstOrDefault().MediatorID.ToString(),
                        Defense_Firm = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().DefenseFirmID == null ? "" : y.ClaimLegals.FirstOrDefault().DefenseFirmID.ToString(),
                        Plaintiff_Firm = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().PlaintiffFirmID == null ? "" : y.ClaimLegals.FirstOrDefault().PlaintiffFirmID.ToString(),
                        CoDefense_Firm = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().CoDefenseFirmID == null ? "" : y.ClaimLegals.FirstOrDefault().CoDefenseFirmID.ToString(),
                        Coverage_Firm = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().CoverageFirmID == null ? "" : y.ClaimLegals.FirstOrDefault().CoverageFirmID.ToString(),
                        Full_Final_Settlement = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().FinalSettlement == null ? "" : y.ClaimLegals.FirstOrDefault().FinalSettlement.ToString(),
                        Medicare_Set_Aside = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().Medicare == null ? "" : y.ClaimLegals.FirstOrDefault().Medicare.ToString(),
                        Annuity = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().Annuity == null ? "" : y.ClaimLegals.FirstOrDefault().Annuity.ToString(),
                        Open_Future_Medicals = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().FutureMedicals == null ? "" : y.ClaimLegals.FirstOrDefault().FutureMedicals.ToString(),
                        Settlement_Details = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : y.ClaimLegals.FirstOrDefault().SettlementDetails,
                        Insured_Notice_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().InsuredNoticeDate),
                        Agent_Notice_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().AgentNoticeDate),
                        TPA_Notice_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().TPANoticeDate),
                        Filing_of_Suit_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().FilingSuitDate),
                        Service_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().ServiceDate),
                        Trial_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().TrialDate),
                        Alt_Dispute_Resolution_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().DisputeResolutionDate),
                        Panel_Hearing_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().PanelHearingDate),
                        Pretrial_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().PreTrialDate),
                        Suit_Assigned_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().SuitAssignedDate),
                        Discovery_Cutoff_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().DiscoveryCutOffDate),
                        Mandatory_Settlement_Date = y.ClaimLegals == null ? "" : y.ClaimLegals.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimLegals.FirstOrDefault().MandatorySettlementDate),
                        //end
                        //From the State Filing tab page
                        // Required_yes_or_no =y.ClaimFROIs==null?"":y.ClaimFROIs.FirstOrDefault().
                        FROI_Date_Received = y.ClaimFROIs == null ? "" : y.ClaimFROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimFROIs.FirstOrDefault().ReceivedDate),
                        FROI_Date_Filed = y.ClaimFROIs == null ? "" : y.ClaimFROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimFROIs.FirstOrDefault().FiledDate),
                        Date_Acknowledged = y.ClaimFROIs == null ? "" : y.ClaimFROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimFROIs.FirstOrDefault().AcknowledgedDate),
                        Error_Msg_Date = y.ClaimFROIs == null ? "" : y.ClaimFROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimFROIs.FirstOrDefault().ErrorMsgDate),
                        Error_Response_Date = y.ClaimFROIs == null ? "" : y.ClaimFROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimFROIs.FirstOrDefault().ErrorResponseDate),
                        SROI_Type = y.ClaimSROIs == null ? "" : y.ClaimSROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimSROIs.FirstOrDefault().SROIType),
                        SROI_Date_Filed = y.ClaimSROIs == null ? "" : y.ClaimSROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimSROIs.FirstOrDefault().FiledDate),
                        SROI_Date_Acknowledged = y.ClaimSROIs == null ? "" : y.ClaimSROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimSROIs.FirstOrDefault().AcknowledgedDate),
                        SROI_Error_Msg_Date = y.ClaimSROIs == null ? "" : y.ClaimSROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimSROIs.FirstOrDefault().ErrorMsgDate),
                        SROI_Error_Response_Date = y.ClaimSROIs == null ? "" : y.ClaimSROIs.FirstOrDefault() == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimSROIs.FirstOrDefault().ErrorResponseDate),
                        //end
                        //lib
                        Claimant_Middle_Name = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().MiddleName,
                        Claimant_Date_of_Loss = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : (y.ClaimantDetails.FirstOrDefault().DateOfLoss == null ? "" : string.Format("{0:MM/dd/yyyy}", y.ClaimantDetails.FirstOrDefault().DateOfLoss)),
                        Type_of_Loss = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().LossID == null ? "" : y.ClaimantDetails.FirstOrDefault().LossID.ToString(),
                        Loss_Description = y.ClaimantDetails == null ? "" : y.ClaimantDetails.FirstOrDefault() == null ? "" : y.ClaimantDetails.FirstOrDefault().LossDesc
                        //end

                    }).FirstOrDefault();
            return objClaim;
        }

        private Document findandreplace(Document destDoc, mailmergedocument sourceDoc)
        {
            destDoc.Replace("«ADJUSTER_COMPANY_PHONE»", sourceDoc.ADJUSTER_COMPANY_PHONE ?? "", false, true);
            destDoc.Replace("«ADJUSTER_EMAIL»", sourceDoc.ADJUSTER_EMAIL ?? "", false, true);
            destDoc.Replace("«ADJUSTER_MOBILE_PHONE»", sourceDoc.ADJUSTER_MOBILE_PHONE ?? "", false, true);
            destDoc.Replace("«ADJUSTER_NAME»", sourceDoc.ADJUSTER_NAME ?? "", false, true);
            destDoc.Replace("«ADJUSTER_FILE_NUMBER»", sourceDoc.ADJUSTER_FILE_NUMBER ?? "", false, true);
            destDoc.Replace("«INSURER_CLAIM_NUMBER»", sourceDoc.INSURER_CLAIM_NUMBER ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_CITY»", sourceDoc.CARRIER_CLIENT_CITY ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_EMAIL»", sourceDoc.CARRIER_CLIENT_EMAIL ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_FAXNUMBER»", sourceDoc.CARRIER_CLIENT_FAXNUMBER ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_NAME»", sourceDoc.CARRIER_CLIENT_NAME ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_PHONE_NUMBER»", sourceDoc.CARRIER_CLIENT_PHONE_NUMBER ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_STATE»", sourceDoc.CARRIER_CLIENT_STATE ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_STREETADDRESS1»", sourceDoc.CARRIER_CLIENT_STREETADDRESS1 ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_STREETADDRESS2»", sourceDoc.CARRIER_CLIENT_STREETADDRESS2 ?? "", false, true);
            destDoc.Replace("«CARRIER_CLIENT_ZIPCODE»", sourceDoc.CARRIER_CLIENT_ZIPCODE ?? "", false, true);
            destDoc.Replace("«COMPANY_ADDRESS»", sourceDoc.COMPANY_ADDRESS ?? "", false, true);
            destDoc.Replace("«COMPANY_CITY»", sourceDoc.COMPANY_CITY ?? "", false, true);
            destDoc.Replace("«COMPANY_EMAIL»", sourceDoc.COMPANY_EMAIL ?? "", false, true);
            destDoc.Replace("«COMPANY_FAX»", sourceDoc.COMPANY_FAX ?? "", false, true);
            destDoc.Replace("«COMPANY_NAME»", sourceDoc.COMPANY_NAME ?? "", false, true);
            destDoc.Replace("«COMPANY_PHONE_NUMBER»", sourceDoc.COMPANY_PHONE_NUMBER ?? "", false, true);
            destDoc.Replace("«COMPANY_STATE»", sourceDoc.COMPANY_STATE ?? "", false, true);
            destDoc.Replace("«COMPANY_ZIP»", sourceDoc.COMPANY_ZIP ?? "", false, true);
            destDoc.Replace("«COMPANY_LOGO»", sourceDoc.COMPANY_LOGO ?? "", false, true);
            destDoc.Replace("«FEDERAL_ID_NO»", sourceDoc.FEDERAL_ID_NO ?? "", false, true);
            destDoc.Replace("«DATE_ACKNOWLEDGED»", sourceDoc.DATE_ACKNOWLEDGED ?? "", false, true);
            destDoc.Replace("«DATE_ASSIGNED»", sourceDoc.DATE_ASSIGNED ?? "", false, true);
            destDoc.Replace("«DATE_CLIENT_APPROVED»", sourceDoc.DATE_CLIENT_APPROVED ?? "", false, true);
            destDoc.Replace("«DATE_CLOSED»", sourceDoc.DATE_CLOSED ?? "", false, true);
            destDoc.Replace("«EVENT_NAME»", sourceDoc.EVENT_NAME ?? "", false, true);
            destDoc.Replace("«EVENT_TYPE»", sourceDoc.EVENT_TYPE ?? "", false, true);
            destDoc.Replace("«SEVERITY»", sourceDoc.SEVERITY ?? "", false, true);
            destDoc.Replace("«STATUS»", sourceDoc.STATUS ?? "", false, true);
            destDoc.Replace("«CAT_ID»", sourceDoc.CAT_ID ?? "", false, true);
            destDoc.Replace("«TYPE_OF_LOSS»", sourceDoc.TYPE_OF_LOSS ?? "", false, true);
            destDoc.Replace("«LOSS_DESCRIPTION»", sourceDoc.LOSS_DESCRIPTION ?? "", false, true);
            destDoc.Replace("«POLICY_NO»", sourceDoc.POLICY_NO ?? "", false, true);
            destDoc.Replace("«EXPIRATION_DATE»", sourceDoc.EXPIRATION_DATE ?? "", false, true);
            destDoc.Replace("«INITIAL_COVERAGE_DATE»", sourceDoc.INITIAL_COVERAGE_DATE ?? "", false, true);
            destDoc.Replace("«TYPE_OF_POLICY»", sourceDoc.TYPE_OF_POLICY ?? "", false, true);
            destDoc.Replace("«POLICY_FORM_TYPE»", sourceDoc.POLICY_FORM_TYPE ?? "", false, true);
            destDoc.Replace("«INSURED_BUSINESS_PHONE»", sourceDoc.INSURED_BUSINESS_PHONE ?? "", false, true);
            destDoc.Replace("«INSURED_HOME_PHONE»", sourceDoc.INSURED_HOME_PHONE ?? "", false, true);
            destDoc.Replace("«INSURED_MOBILE_PHONE»", sourceDoc.INSURED_MOBILE_PHONE ?? "", false, true);
            destDoc.Replace("«INSURED_NAME»", sourceDoc.INSURED_NAME ?? "", false, true);
            destDoc.Replace("«LOSS_ADDRESS»", sourceDoc.LOSS_ADDRESS ?? "", false, true);
            destDoc.Replace("«LOSS_ADDRESS2»", sourceDoc.LOSS_ADDRESS2 ?? "", false, true);
            destDoc.Replace("«LOSS_CITY»", sourceDoc.LOSS_CITY ?? "", false, true);
            destDoc.Replace("«LOSS_LOCATION»", sourceDoc.LOSS_LOCATION ?? "", false, true);
            destDoc.Replace("«LOSS_STATE»", sourceDoc.LOSS_STATE ?? "", false, true);
            destDoc.Replace("«LOSS_ZIPCODE»", sourceDoc.LOSS_ZIPCODE ?? "", false, true);
            destDoc.Replace("«LOSS_DATE»", sourceDoc.LOSS_DATE ?? "", false, true);
            destDoc.Replace("«PRIMARY_EMAIL»", sourceDoc.PRIMARY_EMAIL ?? "", false, true);
            destDoc.Replace("«MAILING_ADDRESS»", sourceDoc.MAILING_ADDRESS ?? "", false, true);
            destDoc.Replace("«MAILING_ADDRESS2»", sourceDoc.MAILING_ADDRESS2 ?? "", false, true);
            destDoc.Replace("«MAILING_CITY»", sourceDoc.MAILING_CITY ?? "", false, true);
            destDoc.Replace("«MAILING_STATE»", sourceDoc.MAILING_STATE ?? "", false, true);
            destDoc.Replace("«MAILING_ZIP»", sourceDoc.MAILING_ZIP ?? "", false, true);
            destDoc.Replace("«INSURER_BRANCH_ADDRESS1»", sourceDoc.INSURER_BRANCH_ADDRESS1 ?? "", false, true);
            destDoc.Replace("«INSURER_BRANCH_ADDRESS2»", sourceDoc.INSURER_BRANCH_ADDRESS2 ?? "", false, true);
            destDoc.Replace("«INSURER_BRANCH_CITY»", sourceDoc.INSURER_BRANCH_CITY ?? "", false, true);
            destDoc.Replace("«INSURER_BRANCH_NAME»", sourceDoc.INSURER_BRANCH_NAME ?? "", false, true);
            destDoc.Replace("«INSURER_BRANCH_STATE»", sourceDoc.INSURER_BRANCH_STATE ?? "", false, true);
            destDoc.Replace("«INSURER_BRANCH_ZIPCODE»", sourceDoc.INSURER_BRANCH_ZIPCODE ?? "", false, true);
            destDoc.Replace("«LOANNUMBER»", sourceDoc.LOANNUMBER ?? "", false, true);
            destDoc.Replace("«LIENHOLDER»", sourceDoc.LIENHOLDER ?? "", false, true);
            destDoc.Replace("«LIENHOLDER_ADDRESS1»", sourceDoc.LIENHOLDER_ADDRESS1 ?? "", false, true);
            destDoc.Replace("«LIENHOLDER_ADDRESS2»", sourceDoc.LIENHOLDER_ADDRESS2 ?? "", false, true);
            destDoc.Replace("«LIENHOLDER_CITY»", sourceDoc.LIENHOLDER_CITY ?? "", false, true);
            destDoc.Replace("«LIENHOLDER_STATE»", sourceDoc.LIENHOLDER_STATE ?? "", false, true);
            destDoc.Replace("«LIENHOLDER_ZIP»", sourceDoc.LIENHOLDER_ZIP ?? "", false, true);
            destDoc.Replace("«LIENHOLDER_PHONE»", sourceDoc.LIENHOLDER_PHONE ?? "", false, true);
            destDoc.Replace("«REPORT_TO»", sourceDoc.REPORT_TO ?? "", false, true);
            destDoc.Replace("«REPORT_TO_ADDRESS1»", sourceDoc.REPORT_TO_ADDRESS1 ?? "", false, true);
            destDoc.Replace("«REPORT_TO_ADDRESS2»", sourceDoc.REPORT_TO_ADDRESS2 ?? "", false, true);
            destDoc.Replace("«REPORT_TO_CITY»", sourceDoc.REPORT_TO_CITY ?? "", false, true);
            destDoc.Replace("«REPORT_TO_STATE»", sourceDoc.REPORT_TO_STATE ?? "", false, true);
            destDoc.Replace("«REPORT_TO_ZIP»", sourceDoc.REPORT_TO_ZIP ?? "", false, true);
            destDoc.Replace("«REPORT_TO_PHONE»", sourceDoc.REPORT_TO_PHONE ?? "", false, true);
            destDoc.Replace("«REPORT_TO_FAX»", sourceDoc.REPORT_TO_FAX ?? "", false, true);
            destDoc.Replace("«REPORT_TO_EMAIL»", sourceDoc.REPORT_TO_EMAIL ?? "", false, true);
            destDoc.Replace("«SUPERVISOR»", sourceDoc.SUPERVISOR ?? "", false, true);
            destDoc.Replace("«SUPERVISOR_PHONE»", sourceDoc.SUPERVISOR_PHONE ?? "", false, true);
            destDoc.Replace("«SUPERVISOR_EMAIl»", sourceDoc.SUPERVISOR_EMAIl ?? "", false, true);
            destDoc.Replace("«TODAYS_DATE»", sourceDoc.TODAYS_DATE ?? "", false, true);


            //for property specs 01-09-2017 - CR2 - CRM File Fixes WA
            destDoc.Replace("«Date_of_Loss»", sourceDoc.Date_of_Loss ?? "", false, true);
            destDoc.Replace("«Date_Received»", sourceDoc.Date_Received ?? "", false, true);
            destDoc.Replace("«Date_Entered»", sourceDoc.Date_Entered ?? "", false, true);
            destDoc.Replace("«Effective_Date»", sourceDoc.Effective_Date ?? "", false, true);
            destDoc.Replace("«Insured_Primary_Email»", sourceDoc.Insured_Primary_Email ?? "", false, true);
            destDoc.Replace("«Insured_Fax_Number»", sourceDoc.Insured_Fax_Number ?? "", false, true);
            destDoc.Replace("«Secondary_Email»", sourceDoc.Secondary_Email ?? "", false, true);
            destDoc.Replace("«Policy_Coverage»", sourceDoc.Policy_Coverage ?? "", false, true);
            destDoc.Replace("«Policy_Coverage_Limit»", sourceDoc.Policy_Coverage_Limit ?? "", false, true);
            destDoc.Replace("«Policy_Deductible»", sourceDoc.Policy_Deductible ?? "", false, true);
            destDoc.Replace("«Adjuster_First_Name»", sourceDoc.Adjuster_First_Name ?? "", false, true);
            // end
            //for WC specs 01-09-2017 - CR2 - CRM File Fixes WA
            //From the Employer tab page
            destDoc.Replace("«Department»", sourceDoc.Department ?? "", false, true);
            destDoc.Replace("«Location»", sourceDoc.Location ?? "", false, true);
            destDoc.Replace("«Division»", sourceDoc.Division ?? "", false, true);
            destDoc.Replace("«Plant»", sourceDoc.Plant ?? "", false, true);
            destDoc.Replace("«Shift»", sourceDoc.Shift ?? "", false, true);
            destDoc.Replace("«Reported_By»", sourceDoc.Reported_By ?? "", false, true);
            destDoc.Replace("«Reported_By_Phone»", sourceDoc.Reported_By_Phone ?? "", false, true);
            destDoc.Replace("«Reported_By_Email»", sourceDoc.Reported_By_Email ?? "", false, true);
            //end From the Employer tab page
            //From the Claimant tab page
            destDoc.Replace("«Claimant_First_Name»", sourceDoc.Claimant_First_Name ?? "", false, true);
            destDoc.Replace("«Claimant_Last_Name»", sourceDoc.Claimant_Last_Name ?? "", false, true);
            destDoc.Replace("«Claimant_Full_Name»", string.Format("{0} {1}", sourceDoc.Claimant_First_Name, sourceDoc.Claimant_Last_Name) ?? "", false, true);
            destDoc.Replace("«Suffix»", sourceDoc.Suffix ?? "", false, true);
            destDoc.Replace("«Employee_Number»", sourceDoc.Employee_Number ?? "", false, true);
            destDoc.Replace("«Employment_Start_Date»", sourceDoc.Employment_Start_Date ?? "", false, true);
            destDoc.Replace("«SSN»", sourceDoc.SSN ?? "", false, true);
            destDoc.Replace("«Date_of_Birth»", sourceDoc.Date_of_Birth ?? "", false, true);
            destDoc.Replace("«Sex»", sourceDoc.Sex ?? "", false, true);
            destDoc.Replace("«Employee_Classification_Code»", sourceDoc.Employee_Classification_Code ?? "", false, true);
            destDoc.Replace("«Classification_Description»", sourceDoc.Classification_Description ?? "", false, true);
            destDoc.Replace("«Mailing_Address_1»", sourceDoc.Mailing_Address_1 ?? "", false, true);
            destDoc.Replace("«Mailing_Address_2»", sourceDoc.Mailing_Address_2 ?? "", false, true);
            destDoc.Replace("«City»", sourceDoc.City ?? "", false, true);
            destDoc.Replace("«State»", sourceDoc.State ?? "", false, true);
            destDoc.Replace("«Zip»", sourceDoc.Zip ?? "", false, true);
            destDoc.Replace("«Home_Phone»", sourceDoc.Home_Phone ?? "", false, true);
            destDoc.Replace("«Business_Phone»", sourceDoc.Business_Phone ?? "", false, true);
            destDoc.Replace("«Cell_Phone»", sourceDoc.Cell_Phone ?? "", false, true);
            destDoc.Replace("«Primary_Email»", sourceDoc.Primary_Email ?? "", false, true);
            destDoc.Replace("«Secondary_Email»", sourceDoc.Secondary_Email ?? "", false, true);
            destDoc.Replace("«Race_Ethnicity»", sourceDoc.Race_Ethnicity ?? "", false, true);
            destDoc.Replace("«Primary_Language»", sourceDoc.Primary_Language ?? "", false, true);
            destDoc.Replace("«Occupation»", sourceDoc.Occupation ?? "", false, true);
            destDoc.Replace("«Marital_Status»", sourceDoc.Marital_Status ?? "", false, true);
            destDoc.Replace("«Number_of_Dependents»", sourceDoc.Number_of_Dependents ?? "", false, true);
            destDoc.Replace("«Emergency_Contact»", sourceDoc.Emergency_Contact ?? "", false, true);
            destDoc.Replace("«Emergency_Contact_Phone»", sourceDoc.Emergency_Contact_Phone ?? "", false, true);
            destDoc.Replace("«Emergency_Contact_Relationship»", sourceDoc.Emergency_Contact_Relationship ?? "", false, true);
            //end From the Claimant tab page
            //From the Injury tab page

            destDoc.Replace("«Date_of_Injury»", sourceDoc.Date_of_Injury ?? "", false, true);
            destDoc.Replace("«Time_of_Injury»", sourceDoc.Time_of_Injury ?? "", false, true);
            destDoc.Replace("«Date_Injury_Reported_to_Insured»", sourceDoc.Date_Injury_Reported_to_Insured ?? "", false, true);
            destDoc.Replace("«Time_Injury_Reported_to_Insured»", sourceDoc.Time_Injury_Reported_to_Insured ?? "", false, true);
            destDoc.Replace("«Date_Opened_Reported_to_Insurance_Company»", sourceDoc.Date_Opened_Reported_to_Insurance_Company ?? "", false, true);
            destDoc.Replace("«Time_Opened_Reported_to_Insurance_Company»", sourceDoc.Time_Opened_Reported_to_Insurance_Company ?? "", false, true);
            destDoc.Replace("«Injury_Reported_To»", sourceDoc.Injury_Reported_To ?? "", false, true);
            destDoc.Replace("«Witness_to_Injury»", sourceDoc.Witness_to_Injury ?? "", false, true);
            destDoc.Replace("«Witness_Phone»", sourceDoc.Witness_Phone ?? "", false, true);
            destDoc.Replace("«Where_did_Injury_Occur»", sourceDoc.Where_did_Injury_Occur ?? "", false, true);
            destDoc.Replace("«Incident_Type»", sourceDoc.Incident_Type ?? "", false, true);
            destDoc.Replace("«Description_of_Injury_or_Illness»", sourceDoc.Description_of_Injury_or_Illness ?? "", false, true);
            destDoc.Replace("«Machine»", sourceDoc.Machine ?? "", false, true);
            destDoc.Replace("«Injury_Illness_that_occurred»", sourceDoc.Injury_Illness_that_occurred ?? "", false, true);
            destDoc.Replace("«Part_of_Body_Affected»", sourceDoc.Part_of_Body_Affected ?? "", false, true);
            destDoc.Replace("«Cause_of_Accident»", sourceDoc.Cause_of_Accident ?? "", false, true);
            destDoc.Replace("«Sub_Cause_of_Accident»", sourceDoc.Sub_Cause_of_Accident ?? "", false, true);
            destDoc.Replace("«Claimant_Cause»", sourceDoc.Claimant_Cause ?? "", false, true);
            //end From the Injury tab page
            //From the Medical tab page
            destDoc.Replace("«Authorized_Treating_Vendor»", sourceDoc.Authorized_Treating_Vendor ?? "", false, true);
            destDoc.Replace("«Authorized_Treating_Doctor»", sourceDoc.Authorized_Treating_Doctor ?? "", false, true);
            destDoc.Replace("«Referral_Doctor»", sourceDoc.Referral_Doctor ?? "", false, true);
            destDoc.Replace("«Employers_IME_Vendor»", sourceDoc.Employers_IME_Vendor ?? "", false, true);
            destDoc.Replace("«Employers_IME_Doctor»", sourceDoc.Employers_IME_Doctor ?? "", false, true);
            destDoc.Replace("«Employees_IME_Vendor»", sourceDoc.Employees_IME_Vendor ?? "", false, true);
            destDoc.Replace("«Employees_IME_Doctor»", sourceDoc.Employees_IME_Doctor ?? "", false, true);
            destDoc.Replace("«Physical_Therapist_Vendor»", sourceDoc.Physical_Therapist_Vendor ?? "", false, true);
            destDoc.Replace("«Physical_Therapist»", sourceDoc.Physical_Therapist ?? "", false, true);
            destDoc.Replace("«Second_Opinion_Vendor»", sourceDoc.Second_Opinion_Vendor ?? "", false, true);
            destDoc.Replace("«Second_Opinion_Doctor»", sourceDoc.Second_Opinion_Doctor ?? "", false, true);
            destDoc.Replace("«Has_Drug_Program»", sourceDoc.Has_Drug_Program ?? "", false, true);
            destDoc.Replace("«Is_Drug_Tested»", sourceDoc.Is_Drug_Tested ?? "", false, true);
            destDoc.Replace("«Test_Result»", sourceDoc.Test_Result ?? "", false, true);
            destDoc.Replace("«Case_Manager_Vendor»", sourceDoc.Case_Manager_Vendor ?? "", false, true);
            destDoc.Replace("«Case_Manager»", sourceDoc.Case_Manager ?? "", false, true);
            //end From the Medical tab page
            //From the Indemnity(Wages & Comp) tab page
            destDoc.Replace("«Home_Company»", sourceDoc.Home_Company ?? "", false, true);
            destDoc.Replace("«Home_Company_Division»", sourceDoc.Home_Company_Division ?? "", false, true);
            destDoc.Replace("«Home_Company_Plant»", sourceDoc.Home_Company_Plant ?? "", false, true);
            destDoc.Replace("«Home_Company_Department»", sourceDoc.Home_Company_Department ?? "", false, true);
            destDoc.Replace("«Home_Company_Shift»", sourceDoc.Home_Company_Shift ?? "", false, true);
            destDoc.Replace("«Home_Supervisor_Name»", sourceDoc.Home_Supervisor_Name ?? "", false, true);
            destDoc.Replace("«Regular_Weekly_Pay»", sourceDoc.Regular_Weekly_Pay ?? "", false, true);
            destDoc.Replace("«Hourly_Pay_Rate»", sourceDoc.Hourly_Pay_Rate ?? "", false, true);
            destDoc.Replace("«Number_Hours_Per_Day»", sourceDoc.Number_Hours_Per_Day ?? "", false, true);
            destDoc.Replace("«Number_Hours_Per_Week»", sourceDoc.Number_Hours_Per_Week ?? "", false, true);
            destDoc.Replace("«Days_Worked_Per_Week»", sourceDoc.Days_Worked_Per_Week ?? "", false, true);
            destDoc.Replace("«Gross_Wages»", sourceDoc.Gross_Wages ?? "", false, true);
            destDoc.Replace("«Income_Allowance»", sourceDoc.Income_Allowance ?? "", false, true);
            destDoc.Replace("«Number_of_Weeks»", sourceDoc.Number_of_Weeks ?? "", false, true);
            destDoc.Replace("«TPD_Compensation_Rate»", sourceDoc.TPD_Compensation_Rate ?? "", false, true);
            destDoc.Replace("«TTD_Compensation_Rate»", sourceDoc.TTD_Compensation_Rate ?? "", false, true);
            destDoc.Replace("«PPD_Rate»", sourceDoc.PPD_Rate ?? "", false, true);
            destDoc.Replace("«PTD_Rate»", sourceDoc.PTD_Rate ?? "", false, true);
            destDoc.Replace("«PPI_SM_or_WB»", sourceDoc.PPI_SM_or_WB ?? "", false, true);
            destDoc.Replace("«PPI_Rating»", sourceDoc.PPI_Rating ?? "", false, true);
            destDoc.Replace("«Dpndt_Rate»", sourceDoc.Dpndt_Rate ?? "", false, true);
            destDoc.Replace("«Second_Injury_Trust_Fund»", sourceDoc.Second_Injury_Trust_Fund ?? "", false, true);
            destDoc.Replace("«Lost_Time_Restricted»", sourceDoc.Lost_Time_Restricted ?? "", false, true);
            destDoc.Replace("«Lost_Time_From»", sourceDoc.Lost_Time_From ?? "", false, true);
            destDoc.Replace("«Lost_Time_Through»", sourceDoc.Lost_Time_Through ?? "", false, true);
            destDoc.Replace("«Beginning_Date»", sourceDoc.Beginning_Date ?? "", false, true);
            destDoc.Replace("«End_Date»", sourceDoc.End_Date ?? "", false, true);
            destDoc.Replace("«Amount»", sourceDoc.Amount ?? "", false, true);
            destDoc.Replace("«first_Date_Out_of_Work»", sourceDoc.first_Date_Out_of_Work ?? "", false, true);
            destDoc.Replace("«Final_Return_to_Work_Date»", sourceDoc.Final_Return_to_Work_Date ?? "", false, true);
            destDoc.Replace("«Date_Released»", sourceDoc.Date_Released ?? "", false, true);
            destDoc.Replace("«Date_Released_to_Return_to_Work»", sourceDoc.Date_Released_to_Return_to_Work ?? "", false, true);
            destDoc.Replace("«Anticipated_Date_Return_to_Work»", sourceDoc.Anticipated_Date_Return_to_Work ?? "", false, true);
            destDoc.Replace("«Anticipated_MMI_Date»", sourceDoc.Anticipated_MMI_Date ?? "", false, true);
            destDoc.Replace("«MMI_Date »", sourceDoc.MMI_Date ?? "", false, true);
            //end From the Indemnity(Wages & Comp) tab page
            //From the Legal tab page

            destDoc.Replace("«Jurisdiction»", sourceDoc.Jurisdiction ?? "", false, true);
            destDoc.Replace("«Suit_Filed»", sourceDoc.Suit_Filed ?? "", false, true);
            destDoc.Replace("«Other_Attorney»", sourceDoc.Other_Attorney ?? "", false, true);
            destDoc.Replace("«Employers_Voc_Disa»", sourceDoc.Employers_Voc_Disa ?? "", false, true);
            destDoc.Replace("«Employers_Voc_Exp»", sourceDoc.Employers_Voc_Exp ?? "", false, true);
            destDoc.Replace("«State_File»", sourceDoc.State_File ?? "", false, true);
            destDoc.Replace("«Defense_Attorney»", sourceDoc.Defense_Attorney ?? "", false, true);
            destDoc.Replace("«County_of_Court»", sourceDoc.County_of_Court ?? "", false, true);
            destDoc.Replace("«Employees_Voc_Disa»", sourceDoc.Employees_Voc_Disa ?? "", false, true);
            destDoc.Replace("«Employees_Voc_Exp»", sourceDoc.Employees_Voc_Exp ?? "", false, true);
            destDoc.Replace("«Claim_Denied»", sourceDoc.Claim_Denied ?? "", false, true);
            destDoc.Replace("«Plaintiff_Attorney»", sourceDoc.Plaintiff_Attorney ?? "", false, true);
            destDoc.Replace("«Docket_Number»", sourceDoc.Docket_Number ?? "", false, true);
            destDoc.Replace("«Case_Name»", sourceDoc.Case_Name ?? "", false, true);
            destDoc.Replace("«Court_Name»", sourceDoc.Court_Name ?? "", false, true);
            destDoc.Replace("«Lawsuit_Type»", sourceDoc.Lawsuit_Type ?? "", false, true);
            destDoc.Replace("«Lawsuit_City»", sourceDoc.Lawsuit_City ?? "", false, true);
            destDoc.Replace("«Lawsuit_State»", sourceDoc.Lawsuit_State ?? "", false, true);
            destDoc.Replace("«Lawsuit_Service_Type»", sourceDoc.Lawsuit_Service_Type ?? "", false, true);
            destDoc.Replace("«Matter_Reference»", sourceDoc.Matter_Reference ?? "", false, true);
            destDoc.Replace("«Judge»", sourceDoc.Judge ?? "", false, true);
            destDoc.Replace("«Mediator»", sourceDoc.Mediator ?? "", false, true);
            destDoc.Replace("«Defense_Firm»", sourceDoc.Defense_Firm ?? "", false, true);
            destDoc.Replace("«Plaintiff_Firm»", sourceDoc.Plaintiff_Firm ?? "", false, true);
            destDoc.Replace("«CoDefense_Firm»", sourceDoc.CoDefense_Firm ?? "", false, true);
            destDoc.Replace("«Coverage_Firm»", sourceDoc.Coverage_Firm ?? "", false, true);
            destDoc.Replace("«Full_Final_Settlement»", sourceDoc.Full_Final_Settlement ?? "", false, true);
            destDoc.Replace("«Medicare_Set_Aside»", sourceDoc.Medicare_Set_Aside ?? "", false, true);
            destDoc.Replace("«Annuity»", sourceDoc.Annuity ?? "", false, true);
            destDoc.Replace("«Open_Future_Medicals»", sourceDoc.Open_Future_Medicals ?? "", false, true);
            destDoc.Replace("«Settlement_Details»", sourceDoc.Settlement_Details ?? "", false, true);
            destDoc.Replace("«Insured_Notice_Date»", sourceDoc.Insured_Notice_Date ?? "", false, true);
            destDoc.Replace("«Agent_Notice_Date»", sourceDoc.Agent_Notice_Date ?? "", false, true);
            destDoc.Replace("«TPA_Notice_Date»", sourceDoc.TPA_Notice_Date ?? "", false, true);
            destDoc.Replace("«Filing_of_Suit_Date»", sourceDoc.Filing_of_Suit_Date ?? "", false, true);
            destDoc.Replace("«Service_Date»", sourceDoc.Service_Date ?? "", false, true);
            destDoc.Replace("«Trial_Date»", sourceDoc.Trial_Date ?? "", false, true);
            destDoc.Replace("«Alt_Dispute_Resolution_Date»", sourceDoc.Alt_Dispute_Resolution_Date ?? "", false, true);
            destDoc.Replace("«Panel_Hearing_Date»", sourceDoc.Panel_Hearing_Date ?? "", false, true);
            destDoc.Replace("«Pretrial_Date»", sourceDoc.Pretrial_Date ?? "", false, true);
            destDoc.Replace("«Suit_Assigned_Date»", sourceDoc.Suit_Assigned_Date ?? "", false, true);
            destDoc.Replace("«Discovery_Cutoff_Date»", sourceDoc.Discovery_Cutoff_Date ?? "", false, true);
            destDoc.Replace("«Mandatory_Settlement_Date»", sourceDoc.Mandatory_Settlement_Date ?? "", false, true);
            //end From the Legal tab page
            //From the State Filing tab page
            destDoc.Replace("«Required_yes_or_no»", sourceDoc.Required_yes_or_no ?? "", false, true);
            destDoc.Replace("«FROI_Date_Received»", sourceDoc.FROI_Date_Received ?? "", false, true);
            destDoc.Replace("«FROI_Date_Filed»", sourceDoc.FROI_Date_Filed ?? "", false, true);
            destDoc.Replace("«Date_Acknowledged»", sourceDoc.Date_Acknowledged ?? "", false, true);
            destDoc.Replace("«Error_Msg_Date»", sourceDoc.Error_Msg_Date ?? "", false, true);
            destDoc.Replace("«Error_Response_Date»", sourceDoc.Error_Response_Date ?? "", false, true);
            destDoc.Replace("«SROI_Type»", sourceDoc.SROI_Type ?? "", false, true);
            destDoc.Replace("«SROI_Date_Filed»", sourceDoc.SROI_Date_Filed ?? "", false, true);
            destDoc.Replace("«SROI_Date_Acknowledged»", sourceDoc.SROI_Date_Acknowledged ?? "", false, true);
            destDoc.Replace("«SROI_Error_Msg_Date»", sourceDoc.SROI_Error_Msg_Date ?? "", false, true);
            destDoc.Replace("«SROI_Error_Response_Date»", sourceDoc.SROI_Error_Response_Date ?? "", false, true);
            //end From the State Filing tab page

            // end
            //for Lib specs 01-09-2017 - CR2 - CRM File Fixes WA
            //From the Claimant tab page
            destDoc.Replace("«Claimant_First_Name»", sourceDoc.Claimant_First_Name ?? "", false, true);
            destDoc.Replace("«Claimant_Last_Name»", sourceDoc.Claimant_Last_Name ?? "", false, true);
            destDoc.Replace("«Claimant_Middle_Name»", sourceDoc.Claimant_Middle_Name ?? "", false, true);
            destDoc.Replace("«Mailing_Address_1»", sourceDoc.Mailing_Address_1 ?? "", false, true);
            destDoc.Replace("«Mailing_Address_2»", sourceDoc.Mailing_Address_2 ?? "", false, true);
            destDoc.Replace("«City»", sourceDoc.City ?? "", false, true);
            destDoc.Replace("«State»", sourceDoc.State ?? "", false, true);
            destDoc.Replace("«Zip»", sourceDoc.Zip ?? "", false, true);
            destDoc.Replace("«Home_Phone»", sourceDoc.Home_Phone ?? "", false, true);
            destDoc.Replace("«Business_Phone»", sourceDoc.Business_Phone ?? "", false, true);
            destDoc.Replace("«Cell_Phone»", sourceDoc.Cell_Phone ?? "", false, true);
            destDoc.Replace("«Primary_Email»", sourceDoc.Primary_Email ?? "", false, true);
            destDoc.Replace("«Secondary_Email»", sourceDoc.Secondary_Email ?? "", false, true);
            destDoc.Replace("«Claimant_Date_of_Loss»", sourceDoc.Claimant_Date_of_Loss ?? "", false, true);
            destDoc.Replace("«Date_Received»", sourceDoc.Date_Received ?? "", false, true);
            destDoc.Replace("«Date_Entered»", sourceDoc.Date_Entered ?? "", false, true);
            destDoc.Replace("«Type_of_Loss»", sourceDoc.Type_of_Loss ?? "", false, true);
            destDoc.Replace("«Loss_Description»", sourceDoc.Loss_Description ?? "", false, true);
            // end

            return destDoc;
        }
        public void mergeDocViaIntrop(object newDocFile, int id)
        {
            Microsoft.Office.Interop.Word.Document aDoc = null;
            Microsoft.Office.Interop.Word.Application wordApp = null;
            try
            {
                var datadoc = getData(id);




                object missing = System.Reflection.Missing.Value;
                wordApp = new Microsoft.Office.Interop.Word.Application();
                var toDay = new DateTime(); // Warning since variables was assigned but never used (Dev Alfredo Morales)
                object readOnly = false;
                object isVisible = false;
                wordApp.Visible = false;
                aDoc = wordApp.Documents.Open(ref newDocFile, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref missing);

                aDoc.Activate();
                MergingTemplate(wordApp, datadoc);
                aDoc.Close();
                wordApp.Quit();
            }
            finally
            {
                // Release all Interop objects.
                if (aDoc != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(aDoc);
                if (wordApp != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                aDoc = null;
                wordApp = null;
                GC.Collect();
            }


        }
        public void MergingTemplate(Microsoft.Office.Interop.Word.Application wordApp, mailmergedocument sourceDoc)
        {

            FindAndReplace(wordApp, "«ADJUSTER_COMPANY_PHONE»", sourceDoc.ADJUSTER_COMPANY_PHONE ?? "", false, true);
            FindAndReplace(wordApp, "«ADJUSTER_EMAIL»", sourceDoc.ADJUSTER_EMAIL ?? "", false, true);
            FindAndReplace(wordApp, "«ADJUSTER_MOBILE_PHONE»", sourceDoc.ADJUSTER_MOBILE_PHONE ?? "", false, true);
            FindAndReplace(wordApp, "«ADJUSTER_NAME»", sourceDoc.ADJUSTER_NAME ?? "", false, true);
            FindAndReplace(wordApp, "«ADJUSTER_FILE_NUMBER»", sourceDoc.ADJUSTER_FILE_NUMBER ?? "", false, true);
            FindAndReplace(wordApp, "«INSURER_CLAIM_NUMBER»", sourceDoc.INSURER_CLAIM_NUMBER ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_CITY»", sourceDoc.CARRIER_CLIENT_CITY ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_EMAIL»", sourceDoc.CARRIER_CLIENT_EMAIL ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_FAXNUMBER»", sourceDoc.CARRIER_CLIENT_FAXNUMBER ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_NAME»", sourceDoc.CARRIER_CLIENT_NAME ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_PHONE_NUMBER»", sourceDoc.CARRIER_CLIENT_PHONE_NUMBER ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_STATE»", sourceDoc.CARRIER_CLIENT_STATE ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_STREETADDRESS1»", sourceDoc.CARRIER_CLIENT_STREETADDRESS1 ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_STREETADDRESS2»", sourceDoc.CARRIER_CLIENT_STREETADDRESS2 ?? "", false, true);
            FindAndReplace(wordApp, "«CARRIER_CLIENT_ZIPCODE»", sourceDoc.CARRIER_CLIENT_ZIPCODE ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_ADDRESS»", sourceDoc.COMPANY_ADDRESS ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_CITY»", sourceDoc.COMPANY_CITY ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_EMAIL»", sourceDoc.COMPANY_EMAIL ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_FAX»", sourceDoc.COMPANY_FAX ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_NAME»", sourceDoc.COMPANY_NAME ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_PHONE_NUMBER»", sourceDoc.COMPANY_PHONE_NUMBER ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_STATE»", sourceDoc.COMPANY_STATE ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_ZIP»", sourceDoc.COMPANY_ZIP ?? "", false, true);
            FindAndReplace(wordApp, "«COMPANY_LOGO»", sourceDoc.COMPANY_LOGO ?? "", false, true);
            FindAndReplace(wordApp, "«FEDERAL_ID_NO»", sourceDoc.FEDERAL_ID_NO ?? "", false, true);
            FindAndReplace(wordApp, "«DATE_ACKNOWLEDGED»", sourceDoc.DATE_ACKNOWLEDGED ?? "", false, true);
            FindAndReplace(wordApp, "«DATE_ASSIGNED»", sourceDoc.DATE_ASSIGNED ?? "", false, true);
            FindAndReplace(wordApp, "«DATE_CLIENT_APPROVED»", sourceDoc.DATE_CLIENT_APPROVED ?? "", false, true);
            FindAndReplace(wordApp, "«DATE_CLOSED»", sourceDoc.DATE_CLOSED ?? "", false, true);
            FindAndReplace(wordApp, "«EVENT_NAME»", sourceDoc.EVENT_NAME ?? "", false, true);
            FindAndReplace(wordApp, "«EVENT_TYPE»", sourceDoc.EVENT_TYPE ?? "", false, true);
            FindAndReplace(wordApp, "«SEVERITY»", sourceDoc.SEVERITY ?? "", false, true);
            FindAndReplace(wordApp, "«STATUS»", sourceDoc.STATUS ?? "", false, true);
            FindAndReplace(wordApp, "«CAT_ID»", sourceDoc.CAT_ID ?? "", false, true);
            FindAndReplace(wordApp, "«TYPE_OF_LOSS»", sourceDoc.TYPE_OF_LOSS ?? "", false, true);
            FindAndReplace(wordApp, "«LOSS_DESCRIPTION»", sourceDoc.LOSS_DESCRIPTION ?? "", false, true);
            FindAndReplace(wordApp, "«POLICY_NO»", sourceDoc.POLICY_NO ?? "", false, true);
            FindAndReplace(wordApp, "«EXPIRATION_DATE»", sourceDoc.EXPIRATION_DATE ?? "", false, true);
            FindAndReplace(wordApp, "«INITIAL_COVERAGE_DATE»", sourceDoc.INITIAL_COVERAGE_DATE ?? "", false, true);
            FindAndReplace(wordApp, "«TYPE_OF_POLICY»", sourceDoc.TYPE_OF_POLICY ?? "", false, true);
            FindAndReplace(wordApp, "«POLICY_FORM_TYPE»", sourceDoc.POLICY_FORM_TYPE ?? "", false, true);
            FindAndReplace(wordApp, "«INSURED_BUSINESS_PHONE»", sourceDoc.INSURED_BUSINESS_PHONE ?? "", false, true);
            FindAndReplace(wordApp, "«INSURED_HOME_PHONE»", sourceDoc.INSURED_HOME_PHONE ?? "", false, true);
            FindAndReplace(wordApp, "«INSURED_MOBILE_PHONE»", sourceDoc.INSURED_MOBILE_PHONE ?? "", false, true);
            FindAndReplace(wordApp, "«INSURED_NAME»", sourceDoc.INSURED_NAME ?? "", false, true);
            FindAndReplace(wordApp, "«LOSS_ADDRESS»", sourceDoc.LOSS_ADDRESS ?? "", false, true);
            FindAndReplace(wordApp, "«LOSS_ADDRESS2»", sourceDoc.LOSS_ADDRESS2 ?? "", false, true);
            FindAndReplace(wordApp, "«LOSS_CITY»", sourceDoc.LOSS_CITY ?? "", false, true);
            FindAndReplace(wordApp, "«LOSS_LOCATION»", sourceDoc.LOSS_LOCATION ?? "", false, true);
            FindAndReplace(wordApp, "«LOSS_STATE»", sourceDoc.LOSS_STATE ?? "", false, true);
            FindAndReplace(wordApp, "«LOSS_ZIPCODE»", sourceDoc.LOSS_ZIPCODE ?? "", false, true);
            FindAndReplace(wordApp, "«LOSS_DATE»", sourceDoc.LOSS_DATE ?? "", false, true);
            FindAndReplace(wordApp, "«PRIMARY_EMAIL»", sourceDoc.PRIMARY_EMAIL ?? "", false, true);
            FindAndReplace(wordApp, "«MAILING_ADDRESS»", sourceDoc.MAILING_ADDRESS ?? "", false, true);
            FindAndReplace(wordApp, "«MAILING_ADDRESS2»", sourceDoc.MAILING_ADDRESS2 ?? "", false, true);
            FindAndReplace(wordApp, "«MAILING_CITY»", sourceDoc.MAILING_CITY ?? "", false, true);
            FindAndReplace(wordApp, "«MAILING_STATE»", sourceDoc.MAILING_STATE ?? "", false, true);
            FindAndReplace(wordApp, "«MAILING_ZIP»", sourceDoc.MAILING_ZIP ?? "", false, true);
            FindAndReplace(wordApp, "«INSURER_BRANCH_ADDRESS1»", sourceDoc.INSURER_BRANCH_ADDRESS1 ?? "", false, true);
            FindAndReplace(wordApp, "«INSURER_BRANCH_ADDRESS2»", sourceDoc.INSURER_BRANCH_ADDRESS2 ?? "", false, true);
            FindAndReplace(wordApp, "«INSURER_BRANCH_CITY»", sourceDoc.INSURER_BRANCH_CITY ?? "", false, true);
            FindAndReplace(wordApp, "«INSURER_BRANCH_NAME»", sourceDoc.INSURER_BRANCH_NAME ?? "", false, true);
            FindAndReplace(wordApp, "«INSURER_BRANCH_STATE»", sourceDoc.INSURER_BRANCH_STATE ?? "", false, true);
            FindAndReplace(wordApp, "«INSURER_BRANCH_ZIPCODE»", sourceDoc.INSURER_BRANCH_ZIPCODE ?? "", false, true);
            FindAndReplace(wordApp, "«LOANNUMBER»", sourceDoc.LOANNUMBER ?? "", false, true);
            FindAndReplace(wordApp, "«LIENHOLDER»", sourceDoc.LIENHOLDER ?? "", false, true);
            FindAndReplace(wordApp, "«LIENHOLDER_ADDRESS1»", sourceDoc.LIENHOLDER_ADDRESS1 ?? "", false, true);
            FindAndReplace(wordApp, "«LIENHOLDER_ADDRESS2»", sourceDoc.LIENHOLDER_ADDRESS2 ?? "", false, true);
            FindAndReplace(wordApp, "«LIENHOLDER_CITY»", sourceDoc.LIENHOLDER_CITY ?? "", false, true);
            FindAndReplace(wordApp, "«LIENHOLDER_STATE»", sourceDoc.LIENHOLDER_STATE ?? "", false, true);
            FindAndReplace(wordApp, "«LIENHOLDER_ZIP»", sourceDoc.LIENHOLDER_ZIP ?? "", false, true);
            FindAndReplace(wordApp, "«LIENHOLDER_PHONE»", sourceDoc.LIENHOLDER_PHONE ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO»", sourceDoc.REPORT_TO ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO_ADDRESS1»", sourceDoc.REPORT_TO_ADDRESS1 ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO_ADDRESS2»", sourceDoc.REPORT_TO_ADDRESS2 ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO_CITY»", sourceDoc.REPORT_TO_CITY ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO_STATE»", sourceDoc.REPORT_TO_STATE ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO_ZIP»", sourceDoc.REPORT_TO_ZIP ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO_PHONE»", sourceDoc.REPORT_TO_PHONE ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO_FAX»", sourceDoc.REPORT_TO_FAX ?? "", false, true);
            FindAndReplace(wordApp, "«REPORT_TO_EMAIL»", sourceDoc.REPORT_TO_EMAIL ?? "", false, true);
            FindAndReplace(wordApp, "«SUPERVISOR»", sourceDoc.SUPERVISOR ?? "", false, true);
            FindAndReplace(wordApp, "«SUPERVISOR_PHONE»", sourceDoc.SUPERVISOR_PHONE ?? "", false, true);
            FindAndReplace(wordApp, "«SUPERVISOR_EMAIl»", sourceDoc.SUPERVISOR_EMAIl ?? "", false, true);
            FindAndReplace(wordApp, "«TODAYS_DATE»", sourceDoc.TODAYS_DATE ?? "", false, true);


        }

        private void FindAndReplace(Microsoft.Office.Interop.Word.Application wordApp, object findText, object replaceWithText, bool t1, bool t2)
        {
            object matchCase = false;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;
            //execute find and replace
            wordApp.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);
        }
    }
    public class mailmergedocument
    {
        public string ADJUSTER_COMPANY_PHONE { get; set; }
        public string ADJUSTER_EMAIL { get; set; }
        public string ADJUSTER_MOBILE_PHONE { get; set; }
        public string ADJUSTER_NAME { get; set; }
        public string ADJUSTER_FILE_NUMBER { get; set; }
        public string INSURER_CLAIM_NUMBER { get; set; }
        public string CARRIER_CLIENT_CITY { get; set; }
        public string CARRIER_CLIENT_EMAIL { get; set; }
        public string CARRIER_CLIENT_FAXNUMBER { get; set; }
        public string CARRIER_CLIENT_NAME { get; set; }
        public string CARRIER_CLIENT_PHONE_NUMBER { get; set; }
        public string CARRIER_CLIENT_STATE { get; set; }
        public string CARRIER_CLIENT_STREETADDRESS1 { get; set; }
        public string CARRIER_CLIENT_STREETADDRESS2 { get; set; }
        public string CARRIER_CLIENT_ZIPCODE { get; set; }
        public string COMPANY_ADDRESS { get; set; }
        public string COMPANY_CITY { get; set; }
        public string COMPANY_EMAIL { get; set; }
        public string COMPANY_FAX { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_PHONE_NUMBER { get; set; }
        public string COMPANY_STATE { get; set; }
        public string COMPANY_ZIP { get; set; }
        public string COMPANY_LOGO { get; set; }
        public string FEDERAL_ID_NO { get; set; }
        public string DATE_ACKNOWLEDGED { get; set; }
        public string DATE_ASSIGNED { get; set; }
        public string DATE_CLIENT_APPROVED { get; set; }
        public string DATE_CLOSED { get; set; }
        public string EVENT_NAME { get; set; }
        public string EVENT_TYPE { get; set; }
        public string SEVERITY { get; set; }
        public string STATUS { get; set; }
        public string CAT_ID { get; set; }
        public string TYPE_OF_LOSS { get; set; }
        public string LOSS_DESCRIPTION { get; set; }
        public string POLICY_NO { get; set; }
        public string EXPIRATION_DATE { get; set; }
        public string INITIAL_COVERAGE_DATE { get; set; }
        public string TYPE_OF_POLICY { get; set; }
        public string POLICY_FORM_TYPE { get; set; }
        public string INSURED_BUSINESS_PHONE { get; set; }
        public string INSURED_HOME_PHONE { get; set; }
        public string INSURED_MOBILE_PHONE { get; set; }
        public string INSURED_NAME { get; set; }
        public string LOSS_ADDRESS { get; set; }
        public string LOSS_ADDRESS2 { get; set; }
        public string LOSS_CITY { get; set; }
        public string LOSS_LOCATION { get; set; }
        public string LOSS_STATE { get; set; }
        public string LOSS_ZIPCODE { get; set; }
        public string LOSS_DATE { get; set; }
        public string PRIMARY_EMAIL { get; set; }
        public string MAILING_ADDRESS { get; set; }
        public string MAILING_ADDRESS2 { get; set; }
        public string MAILING_CITY { get; set; }
        public string MAILING_STATE { get; set; }
        public string MAILING_ZIP { get; set; }
        public string INSURER_BRANCH_ADDRESS1 { get; set; }
        public string INSURER_BRANCH_ADDRESS2 { get; set; }
        public string INSURER_BRANCH_CITY { get; set; }
        public string INSURER_BRANCH_NAME { get; set; }
        public string INSURER_BRANCH_STATE { get; set; }
        public string INSURER_BRANCH_ZIPCODE { get; set; }
        public string LOANNUMBER { get; set; }
        public string LIENHOLDER { get; set; }
        public string LIENHOLDER_ADDRESS1 { get; set; }
        public string LIENHOLDER_ADDRESS2 { get; set; }
        public string LIENHOLDER_CITY { get; set; }
        public string LIENHOLDER_STATE { get; set; }
        public string LIENHOLDER_ZIP { get; set; }
        public string LIENHOLDER_PHONE { get; set; }
        public string REPORT_TO { get; set; }
        public string REPORT_TO_ADDRESS1 { get; set; }
        public string REPORT_TO_ADDRESS2 { get; set; }
        public string REPORT_TO_CITY { get; set; }
        public string REPORT_TO_STATE { get; set; }
        public string REPORT_TO_ZIP { get; set; }
        public string REPORT_TO_PHONE { get; set; }
        public string REPORT_TO_FAX { get; set; }
        public string REPORT_TO_EMAIL { get; set; }
        public string SUPERVISOR { get; set; }
        public string SUPERVISOR_PHONE { get; set; }
        public string SUPERVISOR_EMAIl { get; set; }
        public string TODAYS_DATE { get; set; }

        //property
        public string Date_of_Loss { get; set; }
        public string Date_Received { get; set; }
        public string Date_Entered { get; set; }
        public string Effective_Date { get; set; }
        public string Insured_Primary_Email { get; set; }
        public string Insured_Fax_Number { get; set; }
        public string Secondary_Email { get; set; }
        public string Policy_Coverage { get; set; }
        public string Policy_Coverage_Limit { get; set; }
        public string Policy_Deductible { get; set; }
        public string Adjuster_First_Name { get; set; }
        //end
        //wc
        // From the Employer tab page
        public string Department { get; set; }
        public string Location { get; set; }
        public string Division { get; set; }
        public string Plant { get; set; }
        public string Shift { get; set; }
        public string Reported_By { get; set; }
        public string Reported_By_Phone { get; set; }
        public string Reported_By_Email { get; set; }
        //end
        //From the Claimant tab page
        public string Claimant_First_Name { get; set; }
        public string Claimant_Last_Name { get; set; }
        public string Suffix { get; set; }
        public string Employee_Number { get; set; }
        public string Employment_Start_Date { get; set; }
        public string SSN { get; set; }
        public string Date_of_Birth { get; set; }
        public string Sex { get; set; }
        public string Employee_Classification_Code { get; set; }
        public string Classification_Description { get; set; }
        public string Mailing_Address_1 { get; set; }
        public string Mailing_Address_2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Home_Phone { get; set; }
        public string Business_Phone { get; set; }
        public string Cell_Phone { get; set; }
        public string Primary_Email { get; set; }
        //   public string Secondary_Email { get; set; }
        public string Race_Ethnicity { get; set; }
        public string Primary_Language { get; set; }
        public string Occupation { get; set; }
        public string Marital_Status { get; set; }
        public string Number_of_Dependents { get; set; }
        public string Emergency_Contact { get; set; }
        public string Emergency_Contact_Phone { get; set; }
        public string Emergency_Contact_Relationship { get; set; }
        //end From the Claimant tab page
        //From the Injury tab page

        public string Date_of_Injury { get; set; }
        public string Time_of_Injury { get; set; }
        public string Date_Injury_Reported_to_Insured { get; set; }
        public string Time_Injury_Reported_to_Insured { get; set; }
        public string Date_Opened_Reported_to_Insurance_Company { get; set; }
        public string Time_Opened_Reported_to_Insurance_Company { get; set; }
        public string Injury_Reported_To { get; set; }
        public string Witness_to_Injury { get; set; }
        public string Witness_Phone { get; set; }
        public string Where_did_Injury_Occur { get; set; }
        public string Incident_Type { get; set; }
        public string Description_of_Injury_or_Illness { get; set; }
        public string Machine { get; set; }
        public string Injury_Illness_that_occurred { get; set; }
        public string Part_of_Body_Affected { get; set; }
        public string Cause_of_Accident { get; set; }
        public string Sub_Cause_of_Accident { get; set; }
        public string Claimant_Cause { get; set; }
        //end From the Injury tab page
        //From the Medical tab page
        public string Authorized_Treating_Vendor { get; set; }
        public string Authorized_Treating_Doctor { get; set; }
        public string Referral_Doctor { get; set; }
        public string Employers_IME_Vendor { get; set; }
        public string Employers_IME_Doctor { get; set; }
        public string Employees_IME_Vendor { get; set; }
        public string Employees_IME_Doctor { get; set; }
        public string Physical_Therapist_Vendor { get; set; }
        public string Physical_Therapist { get; set; }
        public string Second_Opinion_Vendor { get; set; }
        public string Second_Opinion_Doctor { get; set; }
        public string Has_Drug_Program { get; set; }
        public string Is_Drug_Tested { get; set; }
        public string Test_Result { get; set; }
        public string Case_Manager_Vendor { get; set; }
        public string Case_Manager { get; set; }
        //end From the Medical tab page
        //From the Indemnity(Wages & Comp) tab page
        public string Home_Company { get; set; }
        public string Home_Company_Division { get; set; }
        public string Home_Company_Plant { get; set; }
        public string Home_Company_Department { get; set; }
        public string Home_Company_Shift { get; set; }
        public string Home_Supervisor_Name { get; set; }
        public string Regular_Weekly_Pay { get; set; }
        public string Hourly_Pay_Rate { get; set; }
        public string Number_Hours_Per_Day { get; set; }
        public string Number_Hours_Per_Week { get; set; }
        public string Days_Worked_Per_Week { get; set; }
        public string Gross_Wages { get; set; }
        public string Income_Allowance { get; set; }
        public string Number_of_Weeks { get; set; }
        public string TPD_Compensation_Rate { get; set; }
        public string TTD_Compensation_Rate { get; set; }
        public string PPD_Rate { get; set; }
        public string PTD_Rate { get; set; }
        public string PPI_SM_or_WB { get; set; }
        public string PPI_Rating { get; set; }
        public string Dpndt_Rate { get; set; }
        public string Second_Injury_Trust_Fund { get; set; }
        public string Lost_Time_Restricted { get; set; }
        public string Lost_Time_From { get; set; }
        public string Lost_Time_Through { get; set; }
        public string Beginning_Date { get; set; }
        public string End_Date { get; set; }
        public string Amount { get; set; }

        public string first_Date_Out_of_Work { get; set; }
        public string Final_Return_to_Work_Date { get; set; }
        public string Date_Released { get; set; }
        public string Date_Released_to_Return_to_Work { get; set; }
        public string Anticipated_Date_Return_to_Work { get; set; }
        public string Anticipated_MMI_Date { get; set; }
        public string MMI_Date { get; set; }
        //end From the Indemnity(Wages & Comp) tab page
        //From the Legal tab page

        public string Jurisdiction { get; set; }
        public string Suit_Filed { get; set; }
        public string Other_Attorney { get; set; }
        public string Employers_Voc_Disa { get; set; }
        public string Employers_Voc_Exp { get; set; }
        public string State_File { get; set; }
        public string Defense_Attorney { get; set; }
        public string County_of_Court { get; set; }
        public string Employees_Voc_Disa { get; set; }
        public string Employees_Voc_Exp { get; set; }
        public string Claim_Denied { get; set; }
        public string Plaintiff_Attorney { get; set; }
        public string Docket_Number { get; set; }
        public string Case_Name { get; set; }
        public string Court_Name { get; set; }
        public string Lawsuit_Type { get; set; }
        public string Lawsuit_City { get; set; }
        public string Lawsuit_State { get; set; }
        public string Lawsuit_Service_Type { get; set; }
        public string Matter_Reference { get; set; }
        public string Judge { get; set; }
        public string Mediator { get; set; }
        public string Defense_Firm { get; set; }
        public string Plaintiff_Firm { get; set; }
        public string CoDefense_Firm { get; set; }
        public string Coverage_Firm { get; set; }
        public string Full_Final_Settlement { get; set; }
        public string Medicare_Set_Aside { get; set; }
        public string Annuity { get; set; }
        public string Open_Future_Medicals { get; set; }
        public string Settlement_Details { get; set; }
        public string Insured_Notice_Date { get; set; }
        public string Agent_Notice_Date { get; set; }
        public string TPA_Notice_Date { get; set; }
        public string Filing_of_Suit_Date { get; set; }
        public string Service_Date { get; set; }
        public string Trial_Date { get; set; }
        public string Alt_Dispute_Resolution_Date { get; set; }
        public string Panel_Hearing_Date { get; set; }
        public string Pretrial_Date { get; set; }
        public string Suit_Assigned_Date { get; set; }
        public string Discovery_Cutoff_Date { get; set; }
        public string Mandatory_Settlement_Date { get; set; }
        //end From the Legal tab page
        //From the State Filing tab page
        public string Required_yes_or_no { get; set; }
        public string FROI_Date_Received { get; set; }
        public string FROI_Date_Filed { get; set; }
        public string Date_Acknowledged { get; set; }
        public string Error_Msg_Date { get; set; }
        public string Error_Response_Date { get; set; }
        public string SROI_Type { get; set; }
        public string SROI_Date_Filed { get; set; }
        public string SROI_Date_Acknowledged { get; set; }
        public string SROI_Error_Msg_Date { get; set; }
        public string SROI_Error_Response_Date { get; set; }
        //end From the State Filing tab page

        // end
        //for Lib specs 01-09-2017 - CR2 - CRM File Fixes WA
        //From the Claimant tab page
        //public string Claimant_First_Name { get; set; }
        //public string Claimant_Last_Name { get; set; }
        public string Claimant_Middle_Name { get; set; }
        //public string Mailing_Address_1 { get; set; }
        //public string Mailing_Address_2 { get; set; }
        //public string City { get; set; }
        //public string State { get; set; }
        //public string Zip { get; set; }
        //public string Home_Phone { get; set; }
        //public string Business_Phone { get; set; }
        //public string Cell_Phone { get; set; }
        //public string Primary_Email { get; set; }
        //public string Secondary_Email { get; set; }
        public string Claimant_Date_of_Loss { get; set; }
        //public string Date_Received { get; set; }
        //public string Date_Entered { get; set; }
        public string Type_of_Loss { get; set; }
        public string Loss_Description { get; set; }
        // end

    }
}