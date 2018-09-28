using AutoMapper;
using CRM.Common;
using CRM.Common.Notification;
using CRM.Common.Security;
using CRM.Core.Model;
using CRM.Core.Repository;
using CRM.Core.Repository.Clients;
using CRM.Core.Repository.SystemSettings;
using CRM.Core.ViewModel.Claim;
using CRM.Core.ViewModel.Client;
using CRM.Data;
using CRM.Data.Repository;
using CRM.Data.Repository.Clients;
using CRM.Data.Repository.SystemSettings;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CRM.Core.Repository.Claims;
using CRM.Data.Repository.Claims;
using System.Transactions;
using System.IO;
using CRM.Core.ViewModel;
using CRM.Core.ViewModel.FTP;
using CRM.Common.FTPData;
using CRM.Core.Repository.FTP;
using CRM.Data.Repository.FTP;



namespace CRM.Web.Areas.Claims.Controllers
{
    public class ClaimController : Controller
    {
        private readonly IRepository<PortalBranch> _companyBranch;
        private readonly IRepository<Client> _client;
        private readonly IClientRepository _clientforbilling;
        private readonly IRepository<ClientBranch> _clientBranch;
        private readonly IRepository<ClientBillingProfile> _billingProfile;
        private readonly IClientContactRepository _clntcontacts;
        private readonly IRepository<ClientContact> _contact;
        private readonly IRepository<refState> _states;
        private readonly IRepository<refPolicyType> _policyType;
        private readonly ITypeOfLossRepository _typeOfLoss;
        private readonly IReserveCategoryRepository _reserveCategory;

        private readonly IRepository<Claim> _claim;
        private readonly IRepository<ClaimCoverage> _claimcoverage;
        private readonly IRepository<Insured> _insuredRepository;
        private readonly IRepository<ClaimReserve> _claimReserve;
        private readonly IClaimRepository _claimrepository;
        private readonly IRepository<LienHolder> _lienholder;
        private readonly IRepository<ClaimSubLimit> _claimsublimit;
        private readonly IClaimCoverageRepository _claimcoveragerepository;
        private readonly IRepository<ClaimActivityLog> _claimActivitylog;
        private readonly IClientRepository _clientrepository;
        private readonly IUserRepository _userrepository;
        private readonly IRepository<ReserveHistory> _reservehistory;
        private readonly IClaimLossTotalRepository _ClaimLossTotal;
        private readonly CRMDbContext _context;
        private readonly IRepository<ClaimDetail> _claimDetail;
        //-------------Xact Analysis------------------
        private readonly IXactImportRepository _XactImport;
        //--------------------------------------------
        public ClaimController()
        {
            var context = new CRMDbContext();
            _companyBranch = new Repository<PortalBranch>(context);
            _client = new Repository<Client>(context);
            _clientforbilling = new ClientRepository(context);
            _clientBranch = new Repository<ClientBranch>(context);
            _billingProfile = new Repository<ClientBillingProfile>(context);
            _clntcontacts = new ClientContactRepository(context);
            _contact = new Repository<ClientContact>(context);
            _states = new Repository<refState>(context);
            _policyType = new Repository<refPolicyType>(context);
            _typeOfLoss = new TypeOfLossRepository(context);
            _reserveCategory = new ReserveCategoryRepository(context);
            _claimDetail = new Repository<ClaimDetail>(context);
            _claim = new Repository<Claim>(context);
            _claimcoverage = new Repository<ClaimCoverage>(context);
            _insuredRepository = new Repository<Insured>(context);
            _claimReserve = new Repository<ClaimReserve>(context);
            _claimrepository = new ClaimRepository(context);
            _lienholder = new Repository<LienHolder>(context);
            _claimsublimit = new Repository<ClaimSubLimit>(context);
            _claimcoveragerepository = new ClaimCoverageRepository(context);
            _claimActivitylog = new Repository<ClaimActivityLog>(context);
            _clientrepository = new ClientRepository(context);
            _userrepository = new UserRepository(context);
            _reservehistory = new Repository<ReserveHistory>(context);
            _ClaimLossTotal = new ClaimLossTotalRepository(context);
            _XactImport = new XactImportRepository(new CRMDbContext());
            _context = new CRMDbContext();

        }

        //
        // GET: /Claims/Claim/
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeUser(AccessType = "Add", ModuleName = Common.Common.ClaimManagement)]
        public ActionResult CreateNew()
        {
            var directoryPath = Server.MapPath(Common.Constants.ClaimWizardPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            TempData.Remove("ClaimTabState");
            TempData.Remove("ClaimLibTabState");
            TempData.Remove("ClaimWCTabState");
            DeleteAllWizardFile();

            int s = countUserProperty();
            TempData["Claimcount"] = s;
            //s = 1;
            if (s == 1)
            {
                ClaimsType model = new ClaimsType();
                model.IsProperty = AuthUser.User.IsProperty;
                model.IsLib = AuthUser.User.IsLib;
                model.IsWc = AuthUser.User.IsWC;
                TempData["TempClaimType"] = model;
                #region
                var portaldetail = _claimrepository.GetStartingAdjuster((int)AuthUser.User.PortalId);

                if (string.IsNullOrEmpty(portaldetail.StartingAdjuster))
                {
                    this.ShowMessage(MessageType.Error, "Please create an Adjuster File", true);
                    // return View("ClaimType", model);//return to dashboard
                    // return View("ClaimType", model);
                    // DashBoard/Home                   
                    return RedirectToAction("Index", "Home", new { area = "Dashboard" });

                }

                string StartingAdjusterPortal = (from x in _context.Portals
                                                 where x.PortalID == AuthUser.User.PortalId
                                                 select x.StartingAdjuster).FirstOrDefault();

                string StartingAdjusterClaim = (from x in _context.Claims
                                                where x.PortalID == AuthUser.User.PortalId
                                                select x.AdjusterFile).FirstOrDefault();

                if (StartingAdjusterPortal == null && StartingAdjusterClaim == null)
                {
                    this.ShowMessage(MessageType.Error, "Please add an Adjuster ClaimNumber before creating Claim.", true);
                    // return View("ClaimType", model);
                    return RedirectToAction("Index", "Home", new { area = "Dashboard" });
                }
                WriteWizardData(model, Constants.ClaimWizardType.ClaimType);
                TempData["TempClaimType"] = model;
                if (model.IsProperty)
                {
                    ClearTempData(1);
                    return RedirectToAction("ClientDetail");
                }
                if (model.IsWc)
                {
                    ClearTempData(2);
                    return RedirectToAction("Index", "ClaimWC", new { model = model });
                }
                if (model.IsLib)
                {
                    ClearTempData(3);
                    return RedirectToAction("Index", "ClaimLib", new { model = model });
                }
                return RedirectToAction("ClientDetail");
                #endregion
            }
            else
            {
                return View("ClaimType");
            }
        }
        public int countUserProperty()
        {
            int count = 0;
            if (AuthUser.User.IsProperty == true)
                count += 1;
            if (AuthUser.User.IsLib == true)
                count += 1;
            if (AuthUser.User.IsWC == true)
                count += 1;
            return count;

        }

        /// <summary>
        /// For back button
        /// </summary>
        /// <param name="claimTypeID"> to check the radio button</param>
        /// <returns></returns>
        [AuthorizeUser(AccessType = "Add", ModuleName = Common.Common.ClaimManagement)]
        public ActionResult Back(int claimTypeID)
        {
            var directoryPath = Server.MapPath(Common.Constants.ClaimWizardPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            ViewBag.claimTypeID = claimTypeID;
            ViewBag.backpress = "true";
            TempData.Remove("ClaimTabState");
            TempData.Remove("ClaimLibTabState");
            TempData.Remove("ClaimWCTabState");
            DeleteAllWizardFile();
            ClaimsType model = new ClaimsType();
            if (claimTypeID == 1)
            {
                model.IsProperty = true;
                model.IsLib = false;
                model.IsWc = false;
            }
            else if (claimTypeID == 2)
            {
                model.IsProperty = false;
                model.IsLib = false;
                model.IsWc = true;
            }
            else if (claimTypeID == 3)
            {
                model.IsProperty = false;
                model.IsLib = true;
                model.IsWc = false;
            }
            int rs = countUserProperty();
            if (rs == 1)
                return RedirectToAction("Index", "Home", new { area = "Dashboard" });
            else
                return View("ClaimType", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClaimType(ClaimsType model)
        {
            if (!model.IsProperty && !model.IsWc && !model.IsLib)
            {
                this.ShowMessage(MessageType.Error, "Please select a claim type", true);
                return View("ClaimType", model);
            }

            var portaldetail = _claimrepository.GetStartingAdjuster((int)AuthUser.User.PortalId);

            if (string.IsNullOrEmpty(portaldetail.StartingAdjuster))
            {
                this.ShowMessage(MessageType.Error, "Please create an Adjuster File", true);
                return View("ClaimType", model);
            }

            string StartingAdjusterPortal = (from x in _context.Portals
                                             where x.PortalID == AuthUser.User.PortalId
                                             select x.StartingAdjuster).FirstOrDefault();

            string StartingAdjusterClaim = (from x in _context.Claims
                                            where x.PortalID == AuthUser.User.PortalId
                                            select x.AdjusterFile).FirstOrDefault();

            if (StartingAdjusterPortal == null && StartingAdjusterClaim == null)
            {
                this.ShowMessage(MessageType.Error, "Please add an Adjuster ClaimNumber before creating Claim.", true);
                return View("ClaimType", model);
            }
            WriteWizardData(model, Constants.ClaimWizardType.ClaimType);
            TempData["TempClaimType"] = model;
            if (model.IsProperty)
            {
                ClearTempData(1);
                return RedirectToAction("ClientDetail");
            }
            if (model.IsWc)
            {
                ClearTempData(2);
                return RedirectToAction("Index", "ClaimWC", new { model = model });
            }
            if (model.IsLib)
            {
                ClearTempData(3);
                return RedirectToAction("Index", "ClaimLib", new { model = model });
            }
            return RedirectToAction("ClientDetail");
        }

        #region ClientDetail

        private SelectList getClientList()
        {
            SelectList clients = null;
            if (AuthUser.User.Roles == "PortalAdmin")
            {
                clients = new SelectList(_client.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId && x.IsDeleted != true).OrderBy(x => x.ClientName), "ClientID", "ClientName");

            }
            else if (AuthUser.User.Roles != "PortalAdmin")
            {
                List<ClientAccessViewModel> clientlist = _clientrepository.ClientAccessList(AuthUser.User.UserId, AuthUser.User.PortalId, AuthUser.User.Roles, false);
                var userList = _userrepository.UserHierarchicalAccessList(AuthUser.User.UserId, false, AuthUser.User.UserId);
                foreach (var user in userList)
                {
                    List<ClientAccessViewModel> list = _clientrepository.ClientAccessList(user.UserID, AuthUser.User.PortalId, AuthUser.User.Roles, false);
                    foreach (var lst in list)
                    {
                        var exist = (from x in clientlist where x.ClientID == lst.ClientID select x.ClientID).FirstOrDefault();
                        if (exist == 0)
                            clientlist.Add(lst);
                    }

                }

                clients = new SelectList(clientlist.Where(x => x.PortalID == AuthUser.User.PortalId && x.IsDeleted != true).OrderBy(x => x.ClientName), "ClientID", "ClientName");
            }
            return clients;
        }
        private SelectList getClientListWithClaimType()
        {
            string claimtype = null;
            ClaimsType model = (ClaimsType)TempData.Peek("TempClaimType");
            SelectList clients = null;
            List<ClientAccessViewModel> clientlist = null;
            if (model.IsProperty)
            {
                claimtype = "Property";
            }
            else if (model.IsWc)
            {
                claimtype = "WC";
            }
            else if (model.IsLib)
            {
                claimtype = "Lib";
            }
            if (AuthUser.User.Roles == "PortalAdmin")
            {
                var clientlist2 = _client.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId && x.IsDeleted != true);
                if (claimtype == "Property")
                {
                    clients = new SelectList(clientlist2.Where(x => x.IsProperty == true).OrderBy(x => x.ClientName).OrderBy(x => x.ClientName), "ClientID", "ClientName");
                }
                else if (claimtype == "WC")
                {
                    clients = clients = new SelectList(clientlist2.Where(x => x.IsWC == true).OrderBy(x => x.ClientName).OrderBy(x => x.ClientName), "ClientID", "ClientName");
                }
                else if (claimtype == "Lib")
                {
                    clients = clients = new SelectList(clientlist2.Where(x => x.IsLib == true).OrderBy(x => x.ClientName).OrderBy(x => x.ClientName), "ClientID", "ClientName");
                }

            }
            else if (AuthUser.User.Roles != "PortalAdmin")
            {
                clientlist = _clientrepository.ClientAccessList(AuthUser.User.UserId, AuthUser.User.PortalId, AuthUser.User.Roles, false);
                var userList = _userrepository.UserHierarchicalAccessList(AuthUser.User.UserId, false, AuthUser.User.UserId);
                foreach (var user in userList)
                {
                    List<ClientAccessViewModel> list = _clientrepository.ClientAccessList(user.UserID, AuthUser.User.PortalId, AuthUser.User.Roles, false);
                    foreach (var lst in list)
                    {
                        var exist = (from x in clientlist where x.ClientID == lst.ClientID select x.ClientID).FirstOrDefault();
                        if (exist == 0)
                            clientlist.Add(lst);
                    }

                }

                clientlist = clientlist.Where(x => x.PortalID == AuthUser.User.PortalId && x.IsDeleted != true).ToList();
                if (claimtype == "Property")
                {
                    clients = new SelectList(clientlist.Where(x => x.IsProperty == true).OrderBy(x => x.ClientName).OrderBy(x => x.ClientName), "ClientID", "ClientName");
                }
                else if (claimtype == "WC")
                {
                    clients = clients = new SelectList(clientlist.Where(x => x.IsWC == true).OrderBy(x => x.ClientName).OrderBy(x => x.ClientName), "ClientID", "ClientName");
                }
                else if (claimtype == "Lib")
                {
                    clients = clients = new SelectList(clientlist.Where(x => x.IsLib == true).OrderBy(x => x.ClientName).OrderBy(x => x.ClientName), "ClientID", "ClientName");
                }
            }



            return clients;
        }

        public ActionResult ClientDetail()
        {
            ViewBag.ClientBranch = _claimrepository.GetUserBranchCodeByUserID(AuthUser.User.UserId);
            ViewBag.CompanyBranchID = new SelectList(_companyBranch.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId && x.IsDeleted != true), "PortalBranchId", "BranchCode", ViewBag.ClientBranch);
            //ViewBag.ClientID = getClientList();



            var obj = ReadWizardData<ClaimViewModel>(Constants.ClaimWizardType.ClientDetails);
            ViewBag.ClientID = getClientListWithClaimType();
            if (obj != null)
            {
                //ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == obj.ClientID), "CBranchId", "CBranchName");
                var clientbranchlist = _clientBranch.GetAll().Where(x => x.ClientID == obj.ClientID && x.IsDeleted != true).ToList();
                clientbranchlist.Insert(0, new ClientBranch() { CBranchId = 0, CBranchName = "--Select--" });
                ViewBag.ClientBranchID = new SelectList(clientbranchlist, "CBranchId", "CBranchName");// adding zero in condition so that selectlist has no entries
                ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == obj.ClientID), "BillProfileID", "ProgrameName");
            }
            else
            {
                //ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0), "CBranchId", "CBranchName");
                ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0), "CBranchId", "CBranchName");
                ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == 0), "BillProfileID", "ProgrameName");
            }
            return View("ClientDetail", obj);
        }

        public ActionResult FillClientBranch(int clientid)
        {
            TempData["clientid"] = clientid;
            TempData.Keep("clientid");
            var clientbranch = _clientBranch.GetAll().Where(m => m.ClientID == clientid && m.IsDeleted != true).ToList();
            return Json(clientbranch, JsonRequestBehavior.AllowGet);

            //return Json(new
            //{
            //    clientbranch = client_branch,
            //    billingProfile = billing_Profile
            //}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillClientAddress(int clientid)
        {
            TempData["clientid"] = clientid;
            TempData.Keep("clientid");

            var clientaddress = _clientforbilling.GetAll().Where(m => m.ClientID == clientid).ToList();
            return Json(clientaddress, JsonRequestBehavior.AllowGet);

            //return Json(new
            //{
            //    clientbranch = client_branch,
            //    billingProfile = billing_Profile
            //}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillBillingProfile(int clientid, int claimTypeID)
        {
            var billingprofile = _billingProfile.GetAll().Where(m => m.ClientID == clientid && m.ClaimTypeID == claimTypeID && m.IsDeleted != true);
            return this.Json((from obj in billingprofile where obj.ClientID == clientid orderby obj.DefaultBillingProfile descending select new { BillProfileID = obj.BillProfileID, ProgrameName = obj.ProgrameName }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ClientDetail(ClaimViewModel model)
        {
            if (_claimrepository.PolicyandInsurerClaimNumberExists(model.PolicyNumber, model.InsurerClaimNumber, 1) == false)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.CompanyBranchID = new SelectList(_companyBranch.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId), "PortalBranchId", "BranchCode");
                    ViewBag.ClientID = getClientListWithClaimType(); //new SelectList(_client.GetAll(), "ClientID", "ClientName");
                    ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0), "CBranchId", "CBranchName");
                    ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == 0), "BillProfileID", "ProgrameName");
                }
                model.UserID = AuthUser.User.UserId;
                model.PortalID = AuthUser.User.PortalId;
                TempData["policyNo"] = model.PolicyNumber;
                TempData.Keep("policyNo");
                WriteWizardData(model, Constants.ClaimWizardType.ClientDetails);
                return RedirectToAction("PolicyDetail");
            }
            else
            {
                this.ShowMessage(MessageType.Error, "PolicyNumber or Insurer ClaimNumber already exists", true);
                ViewBag.CompanyBranchID = new SelectList(_companyBranch.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId), "PortalBranchId", "BranchCode");

                ViewBag.ClientID = getClientListWithClaimType(); //new SelectList(_client.GetAll(), "ClientID", "ClientName");
                ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0 && x.IsDeleted != true), "CBranchId", "CBranchName");
                ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == 0), "BillProfileID", "ProgrameName");
            }
            return View("ClientDetail", model);
        }

        public ActionResult ContactList(int? page)
        {
            string clientid = TempData["clientid"].ToString();
            TempData.Keep("clientid");
            var ClientContacts = _clntcontacts.GetClientContactsListByClientId(Convert.ToInt32(clientid));
            if (ClientContacts == null)
            {
                return PartialView(new ClientContactViewModel { ClientContactId = 0, ClientID = Convert.ToInt32(clientid) });
            }
            var pageNumber = (page ?? 1);
            ViewBag.clientid = clientid;
            return PartialView("ContactList", ClientContacts.ToPagedList(pageNumber, Common.Constants.PageSize));
        }


        public ActionResult GetClientContact(int contactid)
        {
            ViewBag.CompanyBranchID = new SelectList(_companyBranch.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId), "PortalBranchId", "BranchCode");
            ViewBag.ClientID = getClientList(); //new SelectList(_client.GetAll(), "ClientID", "ClientName");
            var obj = ReadWizardData<ClaimViewModel>(Constants.ClaimWizardType.ClientDetails);
            if (obj != null)
            {
                ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == obj.ClientID), "CBranchId", "CBranchName");
                ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == obj.ClientID), "BillProfileID", "ProgrameName");
            }
            else
            {
                ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0), "CBranchId", "CBranchName");
                ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == 0), "BillProfileID", "ProgrameName");
            }
            obj.ReportTo = contactid;
            return View("ClientDetail", obj);
        }

        public ActionResult CreateClientContact(int? ClientContactID, int ClientID)
        {
            //var diary = new ClientContactViewModel() { ClientContactId = 0, ClientID = 1 };
            //return PartialView("CreateClientContact", diary);
            Mapper.CreateMap<ClientContact, ClientContactViewModel>();
            if (ClientContactID == 0)
            {
                ViewBag.CCStateID = new SelectList(_states.GetAll(), "StateID", "StateName");
                return PartialView(new ClientContactViewModel { ClientContactId = 0, ClientID = ClientID });
            }
            var ClientContact = _contact.Get(ClientContactID.GetValueOrDefault());
            ViewBag.CCStateID = new SelectList(_states.GetAll(), "StateID", "StateName");
            var ClientContactDta = Mapper.Map<ClientContact, ClientContactViewModel>(ClientContact);
            return PartialView(ClientContactDta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClientContact(ClientContactViewModel Collection)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(Collection);
            }
            Mapper.CreateMap<ClientContactViewModel, ClientContact>();
            var ClientContact = Mapper.Map<ClientContactViewModel, ClientContact>(Collection);
            if (Collection.ClientContactId == 0)
            {
                _clntcontacts.Add(ClientContact);
            }
            else
            {
                _clntcontacts.Update(ClientContact);
            }
            _clntcontacts.Save();


            return RedirectToAction("ContactList", new { clientid = Collection.ClientID });
            //return PartialView("CreateClientContact", new { ClientContactID = 0, clientid = Collection.ClientID });
        }

        #endregion

        #region PolicyDetails

        public ActionResult PolicyDetail()
        {

            ViewBag.PolicyTypeID = new SelectList(_policyType.GetAll().Where(x => (x.PortalID == AuthUser.User.PortalId || x.PortalID == null) && x.ClaimTypeID == 1), "ID", "PolicyType");

            var obj = ReadWizardData<PolicyDetailViewModel>(Constants.ClaimWizardType.PolicyDetails);

            var objNew = ReadWizardData<ClaimViewModel>(Constants.ClaimWizardType.ClientDetails);
            ViewBag.ClientID = getClientListWithClaimType();
            if (objNew != null)
            {
                TempData["policyNo"] = objNew.PolicyNumber;
            }

            if (obj.PolicyNumber == null)
                obj.PolicyNumber = TempData["policyNo"] == null ? "" : TempData["policyNo"].ToString();
            return View("PolicyDetail", obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PolicyDetail(PolicyDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.CoverageFirmID = new SelectList(_coverage.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId && x.ClaimTypeID == 1), "CoverageFirmID", "CoverageFirm");
                return View("PolicyDetail", model);
            }
            model.UserID = AuthUser.User.UserId;
            model.PortalID = AuthUser.User.PortalId;
            WriteWizardData(model, Constants.ClaimWizardType.PolicyDetails);

            //Save Coverage
            var coveragelist = (List<CreateClaimCoverageViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimCoverage.ToString()) ?? ReadWizardData<List<CreateClaimCoverageViewModel>>(Constants.ClaimWizardType.ClaimCoverage);
            WriteWizardData(coveragelist, Constants.ClaimWizardType.ClaimCoverage);

            var SubLimitlist = (List<CreateClaimSubLimitsViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimSubLimit.ToString()) ?? ReadWizardData<List<CreateClaimSubLimitsViewModel>>(Constants.ClaimWizardType.ClaimSubLimit);
            WriteWizardData(SubLimitlist, Constants.ClaimWizardType.ClaimSubLimit);

            var LieonHolderlist = (List<ClaimLienHolderViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimLienHolder.ToString()) ?? ReadWizardData<List<ClaimLienHolderViewModel>>(Constants.ClaimWizardType.ClaimLienHolder);
            WriteWizardData(LieonHolderlist, Constants.ClaimWizardType.ClaimLienHolder);

            return RedirectToAction("InsuredDetails");
        }

        #endregion

        #region Coverage
        public ActionResult AddCoverage()
        {
            var conSelecList = new List<SelectListItem>
            {
                new SelectListItem() {Text = "ACV", Value = "ACV"},
                new SelectListItem() {Text = "RCV", Value = "RCV"},
                new SelectListItem() {Text = "Incured", Value = "Incured"}
                
            };
            ViewBag.ACVRCV = conSelecList;
            //ViewBag.CoverageID = new SelectList(_coverage.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId && x.ClaimTypeID == 1), "CoverageFirmID", "CoverageFirm");
            return PartialView("AddClaimCoverage");
        }

        [HttpPost]
        public ActionResult AddCoverage(CreateClaimCoverageViewModel model, int? page)
        {
            model.ClaimID = 1;
            var obj = (List<CreateClaimCoverageViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimCoverage.ToString()) ?? ReadWizardData<List<CreateClaimCoverageViewModel>>(Constants.ClaimWizardType.ClaimCoverage);
            obj.Add(model);
            TempData[Constants.ClaimWizardType.ClaimCoverage.ToString()] = obj;
            return PartialView("_ClaimCoverageList", obj.ToPagedList((page ?? 1), Constants.PageSize));
        }

        public ActionResult ClaimCoverageList(int? page)
        {
            var coveragelist = (List<CreateClaimCoverageViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimCoverage.ToString()) ?? ReadWizardData<List<CreateClaimCoverageViewModel>>(Constants.ClaimWizardType.ClaimCoverage);
            return PartialView("_ClaimCoverageList", coveragelist.ToPagedList((page ?? 1), Constants.PageSize));
        }

        [HttpPost]
        public JsonResult DoesCoverageExist(string CoverageName)
        {
            var obj = (List<CreateClaimCoverageViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimCoverage.ToString()) ?? ReadWizardData<List<CreateClaimCoverageViewModel>>(Constants.ClaimWizardType.ClaimCoverage);

            if (obj.Count == 0)
                return Json(!false);

            if (obj.Count(c => c.CoverageName == CoverageName) > 0)
                return Json(!true);

            return Json(!false);
        }

        //[HttpPost]
        //public JsonResult IsNumber(string value)
        //{
        //    bool IsNum = false;
        //    try {
        //        if (value != null) 
        //        {
        //            decimal dec = decimal.Parse(value);
        //            IsNum = true;
        //        }

        //    }catch(Exception ex)
        //    {
        //        IsNum = false;
        //    }

        //    return Json(IsNum);
        //} 

        #endregion

        #region AddSubLimits

        public ActionResult AddSubLimits()
        {
            var CoverageFirmList = (List<CreateClaimCoverageViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimCoverage.ToString());
            if (CoverageFirmList != null)
            {
                ViewBag.CoverageFirmID = new SelectList(CoverageFirmList, "ClaimCovergaeID", "CoverageName");
            }
            else
            {
                this.ShowMessage(MessageType.Error, "Please add coverage first", true);
                var conSelecList = new List<SelectListItem>
            {
                new SelectListItem() {Text = "ACV", Value = "ACV"},
                new SelectListItem() {Text = "RCV", Value = "RCV"},
                new SelectListItem() {Text = "Incured", Value = "Incured"}
                
            };
                ViewBag.ACVRCV = conSelecList;
                return PartialView("AddClaimCoverage");
            }
            return PartialView("AddSubLimits");
        }

        [HttpPost]
        public ActionResult AddSubLimits(CreateClaimSubLimitsViewModel model, int? page)
        {
            model.ClaimID = 1;
            var obj = (List<CreateClaimSubLimitsViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimSubLimit.ToString()) ?? ReadWizardData<List<CreateClaimSubLimitsViewModel>>(Constants.ClaimWizardType.ClaimSubLimit);
            obj.Add(model);
            TempData[Constants.ClaimWizardType.ClaimSubLimit.ToString()] = obj;
            return PartialView("ClaimSubLimitList", obj.ToPagedList((page ?? 1), Constants.PageSize));
        }


        public ActionResult ClaimSubLimitList(int? page)
        {
            var coveragelist = (List<CreateClaimSubLimitsViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimSubLimit.ToString()) ?? ReadWizardData<List<CreateClaimSubLimitsViewModel>>(Constants.ClaimWizardType.ClaimSubLimit);
            return PartialView("ClaimSubLimitList", coveragelist.ToPagedList((page ?? 1), Constants.PageSize));
        }

        #endregion

        #region LienHolder
        public ActionResult AddLienHolder()
        {
            ViewBag.State = new SelectList(_states.GetAll(), "StateID", "StateName");
            return PartialView("AddLienHolder");
        }

        [HttpPost]
        public ActionResult AddLienHolder(ClaimLienHolderViewModel model, int? page)
        {
            model.ClaimID = 1;
            if (model.StateID > 0)
            {
                model.State = _states.Get((int)model.StateID).StateName;
            }
            var obj = (List<ClaimLienHolderViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimLienHolder.ToString()) ?? ReadWizardData<List<ClaimLienHolderViewModel>>(Constants.ClaimWizardType.ClaimLienHolder);
            obj.Add(model);
            TempData[Constants.ClaimWizardType.ClaimLienHolder.ToString()] = obj;
            return PartialView("ClaimLienHolderList", obj.ToPagedList((page ?? 1), Constants.PageSize));
        }

        public ActionResult ClaimLienHolderList(int? page)
        {
            var coveragelist = (List<ClaimLienHolderViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimLienHolder.ToString()) ?? ReadWizardData<List<ClaimLienHolderViewModel>>(Constants.ClaimWizardType.ClaimLienHolder);
            return PartialView("ClaimLienHolderList", coveragelist.ToPagedList((page ?? 1), Constants.PageSize));
        }

        public ActionResult DeleteLienHolder(int ID)
        {
            var obj = (List<ClaimLienHolderViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimLienHolder.ToString()) ?? ReadWizardData<List<ClaimLienHolderViewModel>>(Constants.ClaimWizardType.ClaimLienHolder);
            obj.RemoveAll(s => s.LienHolderID == ID);
            TempData[Constants.ClaimWizardType.ClaimLienHolder.ToString()] = obj;
            return RedirectToAction("ClaimLienHolderList", 1);
        }
        #endregion

        #region InsuredDetails

        public ActionResult InsuredDetails()
        {
            ViewBag.State = new SelectList(_states.GetAll(), "StateID", "StateName");
            var obj = ReadWizardData<InsuredViewModel>(Constants.ClaimWizardType.InsuredDetails);
            if (obj.LossCountry == "" || obj.LossCountry == null)
            {
                obj.LossCountry = "USA";
            }
            if (obj.MailingCountry == "" || obj.MailingCountry == null)
            {
                obj.MailingCountry = "USA";
            }
            return View("InsuredDetails", obj);
        }

        [HttpPost]
        public ActionResult InsuredDetails(InsuredViewModel model, string btnBack, string btnNext)
        {
            if (btnBack != null) //When back press
            {
                return RedirectToAction("PolicyDetail");
            }
            else //if (btnNext != null) when next press
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.State = new SelectList(_states.GetAll(), "StateID", "StateName");
                    return View("InsuredDetails", model);
                }
                WriteWizardData(model, Constants.ClaimWizardType.InsuredDetails);
                return RedirectToAction("ClaimDetails");
            }
        }

        #endregion

        #region ClaimDetails
        public ActionResult ClaimDetails()
        {
            ViewBag.TypeOfLoss = new SelectList(_typeOfLoss.GetAllWithClaimType(AuthUser.User.Roles, AuthUser.User.PortalId, 1), "TypeOfLossID", "TypeOfLoss");//z
            DDLTime();
            var obj = ReadWizardData<ClaimDetailsViewModel>(Constants.ClaimWizardType.ClaimDetails);
            return View("ClaimDetails", obj);
        }

        [HttpPost]
        public ActionResult ClaimDetails(ClaimDetailsViewModel model, string btnBack, string btnNext, bool assignAdjuster)
        {
            TempData["ClaimTypeId"] = model.ClaimTypeID;
            TempData.Keep("ClaimTypeId");

            if (btnBack != null) //When back press
            {
                return RedirectToAction("InsuredDetails");
            }
            else //if (btnNext != null) when next press
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.TypeOfLoss = new SelectList(_typeOfLoss.GetAllWithClaimType(AuthUser.User.Roles, AuthUser.User.PortalId, 1), "TypeOfLossID", "TypeOfLoss");//z
                    DDLTime();
                    return View("ClaimDetails", model);
                }
                WriteWizardData(model, Constants.ClaimWizardType.ClaimDetails);
                // return RedirectToAction("ReserveDetails");

                SaveClaim("Skip");
                this.ShowMessage(MessageType.Success, "Claim saved successfully.", true);
                //return RedirectToAction("Index", "ClaimEdit");


                var claimId = TempData["ClaimID"];
                var claimTypeId = TempData["ClaimTypeID"];
                ////----------------Code for XactAnalysis Integration------------------
                //if (claimTypeId.ToString() == "1")
                //{
                //    int ClaimID = Convert.ToInt32(claimId);
                //    ClaimSendToXactAnalysis(ClaimID);
                //}
                ////-------------------------------------------------------------------
                TempData.Remove("ClaimID"); TempData.Remove("ClaimTypeID");

                return Json(new { redirectTo = Url.Action("Edit", "ClaimEdit", new { id = claimId, ClaimTypeID = claimTypeId }), adjusterList = Url.Action("AdjusterList", "ClaimEdit", new { claimId = claimId }), assignAdjuster = assignAdjuster }); 
                //return RedirectToAction("Edit", "ClaimEdit", new { id = claimId, ClaimTypeID = claimTypeId });







            }
        }
        #endregion

        #region Reserve
        public ActionResult ReserveDetails()
        {
            ViewBag.ReserveCategoryID = new SelectList(_reserveCategory.GetAllWithClaimType(AuthUser.User.PortalId, AuthUser.User.Roles, 1), "ReserveCategoryId", "CategoryName");
            var obj = ReadWizardData<ClaimReserveViewModel>(Constants.ClaimWizardType.Reserve);
            return View("ReserveDetails", obj);
        }

        [HttpPost]
        public ActionResult ReserveDetails(ClaimReserveViewModel model, string btnBack, string btnNext)
        {
            if (btnBack != null) //When back press
            {

                return RedirectToAction("ClaimDetails");
            }
            else //if (btnNext != null) when next press
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ReserveCategoryID = new SelectList(_reserveCategory.GetAllWithClaimType(AuthUser.User.PortalId, AuthUser.User.Roles, 1), "ReserveCategoryId", "CategoryName");
                    return View("ReserveDetails", model);
                }
                WriteWizardData(model, Constants.ClaimWizardType.Reserve);
            }

            SaveClaim(btnNext);
            TempData.Remove("ClaimTabState");
            this.ShowMessage(MessageType.Success, "Claim saved successfully.", true);
            var claimId = TempData["ClaimID"];
            var claimTypeId = TempData["ClaimTypeID"];

            ////----------------Code for XactAnalysis Integration------------------
            //if (claimTypeId.ToString() == "1")
            //{
            //    int ClaimID = Convert.ToInt32(claimId);
            //    ClaimSendToXactAnalysis(ClaimID);
            //}
            ////-------------------------------------------------------------------
            TempData.Remove("ClaimID"); TempData.Remove("ClaimTypeID");
            return RedirectToAction("Edit", "ClaimEdit", new { id = claimId, ClaimTypeID = claimTypeId });
        }

        public ActionResult SkipReserveDetails(string Skipsave)
        {
            SaveClaim(Skipsave);
            this.ShowMessage(MessageType.Success, "Claim saved successfully.", true);
            //return RedirectToAction("Index", "ClaimEdit");


            var claimId = TempData["ClaimID"];
            var claimTypeId = TempData["ClaimTypeID"];
            ////----------------Code for XactAnalysis Integration------------------
            //if (claimTypeId.ToString() == "1")
            //{
            //    int ClaimID = Convert.ToInt32(claimId);
            //    ClaimSendToXactAnalysis(ClaimID);
            //}
            ////-------------------------------------------------------------------
            TempData.Remove("ClaimID"); TempData.Remove("ClaimTypeID");
            return RedirectToAction("Edit", "ClaimEdit", new { id = claimId, ClaimTypeID = claimTypeId });
        }

        #endregion

        #region FinalSave

        private void SaveClaim(string btnname)
        {
            string maxInsurerClaimNumber = getMaxInsurerClaimNumber();

            using (var transaction = new TransactionScope())
            {
                //Save Claim
                var client_obj = ReadWizardData<ClaimViewModel>(Constants.ClaimWizardType.ClientDetails);
                var policy_obj = ReadWizardData<PolicyDetailViewModel>(Constants.ClaimWizardType.PolicyDetails);
                var claim_obj = ReadWizardData<ClaimDetailsViewModel>(Constants.ClaimWizardType.ClaimDetails);

                client_obj.PolicyFormType = policy_obj.PolicyFormType;
                client_obj.PolicyTypeID = policy_obj.PolicyTypeID;
                client_obj.InitialCoverageDate = policy_obj.InitialCoverageDate;
                client_obj.EffectiveDate = policy_obj.EffectiveDate;
                client_obj.ExpirationDate = policy_obj.ExpirationDate;

                client_obj.CategoryID = claim_obj.CategoryId;
                client_obj.LossDate = claim_obj.LossDate;
                client_obj.DateReceived = claim_obj.DateReceived;
                client_obj.TypeofLoss = claim_obj.TypeofLoss;
                client_obj.LossDescription = claim_obj.LossDescription;
                client_obj.ClaimTypeID = 1;


                if (client_obj.ClientBranchID == 0)
                {
                    client_obj.ClientBranchID = null;
                }

                Mapper.CreateMap<ClaimViewModel, Claim>();
                Claim claim = Mapper.Map<ClaimViewModel, Claim>(client_obj);
                claim.DateEntered = DateTime.Now;
                claim.ReceivedFrom = claim_obj.ReceivedFrom;
                claim.TimeReceived = claim_obj.TimeReceived;
                claim.TimeReceivedPeriod = claim_obj.TimeReceivedPeriod;
                _claim.Add(claim);
                _claim.Save();
                if (TempData["ClaimID"] != null)
                {
                    TempData.Remove("ClaimID");
                }

                TempData.Add("ClaimID", claim.ClaimID);
                if (TempData["ClaimTypeID"] != null)
                {
                    TempData.Remove("ClaimTypeID");

                }
                TempData.Add("ClaimTypeID", claim.ClaimTypeID);
                //--------------------------Generate Adjuster File No---------------------------------------------------------------
                var portaldetail = _claimrepository.GetStartingAdjuster((int)AuthUser.User.PortalId);
                var data = _claimrepository.GetLastAdjuster((int)AuthUser.User.PortalId);

                //client_obj.AdjusterFileNumber = data != null ? data.AdjusterFile.Split('-')[0] + "-" + (Convert.ToInt32(data.AdjusterFile.Split('-')[1]) + 1) : portaldetail.StartingAdjuster;

                var AdjusterFileNumber = data != null ? data.AdjusterFile.Split('-')[0] + "-" + (Convert.ToInt32(data.AdjusterFile.Split('-')[1]) + 1) : portaldetail.StartingAdjuster;

                var maxAdjusterFileNumber = "";
                bool ClaimNumberExist = _claimrepository.IsClaimNoExists(AdjusterFileNumber);
                if (ClaimNumberExist)
                {
                    if (maxInsurerClaimNumber != null)
                    {
                        maxAdjusterFileNumber = maxInsurerClaimNumber.Split('-')[0] + "-" + (Convert.ToInt32(maxInsurerClaimNumber.Split('-')[1]) + 1);

                        AdjusterFileNumber = maxAdjusterFileNumber;
                    }

                }
                client_obj.AdjusterFileNumber = AdjusterFileNumber;

                //if (portaldetail.StartingAdjuster == null)
                //    client_obj.AdjusterFileNumber = portaldetail.Pid + "-" + claim.ClaimID;
                //else
                //    client_obj.AdjusterFileNumber = portaldetail.StartingAdjuster + claim.ClaimID;

                if (client_obj.InsurerClaimNumber == null)
                {
                    client_obj.InsurerClaimNumber = client_obj.AdjusterFileNumber;
                }

                if (client_obj.PolicyNumber == null)
                {
                    client_obj.PolicyNumber = client_obj.AdjusterFileNumber;
                }
                claim.PolicyNumber = client_obj.PolicyNumber;
                claim.InsurerClaimNumber = client_obj.InsurerClaimNumber;
                claim.AdjusterFile = client_obj.AdjusterFileNumber;
                _claim.UpdateFields(claim, new List<string> { "ClaimID", "PolicyNumber", "InsurerClaimNumber", "AdjusterFile" });
                _claim.Save();
                //-----------------------------End------------------------------------------------------------------------------------
                //ClaimLog
                SaveLog("Claim : " + client_obj.InsurerClaimNumber + " saved successfully", "Claim", AuthUser.User.UserId, claim.ClaimID);

                //Implement Automated Diary (by amit)
                _claimrepository.InsertAutoDiaries(claim);

                //Add Coverage
                var coverage_obj = ReadWizardData<List<CreateClaimCoverageViewModel>>(Constants.ClaimWizardType.ClaimCoverage);
                foreach (var cr in coverage_obj)
                {
                    Mapper.CreateMap<CreateClaimCoverageViewModel, ClaimCoverage>();
                    ClaimCoverage coverage = Mapper.Map<CreateClaimCoverageViewModel, ClaimCoverage>(cr);
                    coverage.ClaimID = claim.ClaimID;
                    _claimcoverage.Add(coverage);
                    //Coverage Log
                    SaveLog(coverage.CoverageName + " Added to Claim", "Policy", AuthUser.User.UserId, claim.ClaimID);
                }
                _claim.Save();//zi
                //Add LossTotal according to claims
                //fetch all coverages for this claimID
                //
                IEnumerable<ClaimCoverage> coverageList = _claimcoveragerepository.GetCoverageListByClaimID(claim.ClaimID);
                foreach (ClaimCoverage coverage in coverageList)
                {
                    ClaimLossTotal claimloss = new ClaimLossTotal();
                    claimloss.ClaimACVRCVIncured = coverage.ACVRCV;
                    claimloss.ClaimCATDeductible = coverage.CATdecuctible;
                    claimloss.ClaimDeductible = coverage.Deductible;
                    claimloss.ClaimCoverageID = coverage.ClaimCovergaeID;
                    claimloss.ClaimID = coverage.ClaimID;
                    _ClaimLossTotal.Add(claimloss);
                    _ClaimLossTotal.Save();
                }
                _claim.Save();
                //

                //Add Sublimit
                var sublimit_obj = ReadWizardData<List<CreateClaimSubLimitsViewModel>>(Constants.ClaimWizardType.ClaimSubLimit);
                foreach (var sub in sublimit_obj)
                {
                    //Get claimcoverageID
                    int coverageid = _claimcoveragerepository.GetClaimCoveragebyName(sub.SubLimitCoverage, claim.ClaimID);

                    Mapper.CreateMap<CreateClaimSubLimitsViewModel, ClaimSubLimit>();
                    ClaimSubLimit sublimit = Mapper.Map<CreateClaimSubLimitsViewModel, ClaimSubLimit>(sub);
                    sublimit.ClaimID = claim.ClaimID;
                    sublimit.ClaimCoverageID = coverageid;
                    _claimsublimit.Add(sublimit);
                    //Sublimit Log
                    SaveLog("Sublimit saved successfully", "Sublimit", AuthUser.User.UserId, claim.ClaimID);
                }
                _claim.Save();//zi
                //Add LienHolder
                var lien_obj = ReadWizardData<List<ClaimLienHolderViewModel>>(Constants.ClaimWizardType.ClaimLienHolder);
                foreach (var lh in lien_obj)
                {
                    Mapper.CreateMap<ClaimLienHolderViewModel, LienHolder>();
                    LienHolder lienholder = Mapper.Map<ClaimLienHolderViewModel, LienHolder>(lh);
                    lienholder.ClaimID = claim.ClaimID;
                    _lienholder.Add(lienholder);
                    //LienHolder Log
                    SaveLog("LienHolder : " + lienholder.LienHolderName + " saved successfully", "LienHolder", AuthUser.User.UserId, claim.ClaimID);
                }


                //Add Insured
                var insured_obj = ReadWizardData<InsuredViewModel>(Constants.ClaimWizardType.InsuredDetails);
                Mapper.CreateMap<InsuredViewModel, Insured>();
                Insured insured = Mapper.Map<InsuredViewModel, Insured>(insured_obj);
                insured.ClaimID = claim.ClaimID;
                _insuredRepository.Add(insured);

                //Insured Log
                SaveLog("Insured : " + insured.InsuredName + " saved successfully", "Insured", AuthUser.User.UserId, claim.ClaimID);

                if (btnname != "Skip")
                {
                    //Add reserve
                    var reserve_obj = ReadWizardData<ClaimReserveViewModel>(Constants.ClaimWizardType.Reserve);
                    Mapper.CreateMap<ClaimReserveViewModel, ClaimReserve>();
                    ClaimReserve claimreserve = Mapper.Map<ClaimReserveViewModel, ClaimReserve>(reserve_obj);
                    claimreserve.ClaimID = claim.ClaimID;

                    claimreserve.currentamount = reserve_obj.ReserveAmount;
                    claimreserve.lastchangeamount = 0;
                    claimreserve.claimdate = DateTime.Now;
                    claimreserve.AmountPaid = 0; //payment
                    claimreserve.SettlementType = null;
                    claimreserve.RecoveredAmount = 0;
                    claimreserve.SubrogationRecovery = 0;
                    claimreserve.RecoveryFees = 0;
                    claimreserve.TotalIncurred = claimreserve.currentamount;
                    claimreserve.Isincrease = true;
                    claimreserve.changedby = AuthUser.User.UserName;
                    claimreserve.lastpercentage = Convert.ToDecimal(claimreserve.currentamount);
                    claimreserve.ReserveBalance = Convert.ToDecimal(claimreserve.currentamount);
                    _claimReserve.Add(claimreserve);

                    //Add ReserveHistory
                    ReserveHistory rh = new ReserveHistory();
                    rh.ClaimReserveid = claimreserve.ClaimReserveID;
                    rh.AmountPaid = claimreserve.AmountPaid;
                    rh.Balanceamount = Convert.ToDecimal(claimreserve.currentamount);
                    rh.Change = "Increase";
                    rh.Changeamount = Convert.ToDecimal(claimreserve.currentamount);
                    rh.changedby = AuthUser.User.UserName;
                    rh.Initialamount = 0;
                    rh.Percentchange = Convert.ToDecimal(claimreserve.currentamount);
                    rh.Reason = claimreserve.Reason;
                    rh.RecoveredAmount = 0;
                    rh.RecoveryFees = 0;
                    rh.Savedate = DateTime.Now;
                    rh.SettlementType = null;
                    rh.SubrogationRecovery = 0;
                    rh.TotalIncurred = Convert.ToDecimal(claimreserve.currentamount);
                    _reservehistory.Add(rh);

                    _claimReserve.Add(claimreserve);
                    //ClaimReserve Log
                    SaveLog("Claim Reserve saved successfully", "Reserve", AuthUser.User.UserId, claim.ClaimID);
                }

                _claim.Save();

                //============Claim Detail===============
                ClaimDetail clmDetail = new ClaimDetail();
                clmDetail.Date = DateTime.Now;
                clmDetail.ClaimId = claim.ClaimID;
                clmDetail.Status = "Open";
                clmDetail.UserId = AuthUser.User.UserId;
                _claimDetail.Add(clmDetail);
                _claimDetail.Save();
                //===========End================

                TempData.Remove(Constants.ClaimWizardType.ClaimCoverage.ToString());
                TempData.Remove(Constants.ClaimWizardType.ClaimSubLimit.ToString());
                TempData.Remove(Constants.ClaimWizardType.ClaimLienHolder.ToString());

                //Delete xml files
                DeleteAllWizardFile();

                transaction.Complete();

            }
        }

        public string getMaxInsurerClaimNumber()
        {
            return _claimrepository.getMaxInsurerClaimNumber();
        }

        #endregion

        #region ActivityLog

        private void SaveLog(string activitydetail, string activitytitle, int userid, int claimid)
        {
            ClaimActivityLog activitylog = new ClaimActivityLog();
            activitylog.ActivityDate = DateTime.Now;
            activitylog.ActivityDetail = activitydetail;
            activitylog.ActivityTitle = activitytitle;
            activitylog.ActivityUserID = userid;
            activitylog.ClaimID = claimid;
            _claimActivitylog.Add(activitylog);
            _claimActivitylog.Save();
        }

        #endregion

        #region Private Methods
        //......This funtion is used to clear temp data in new claim creation process..........//
        private void ClearTempData(int claimTypeID)
        {
            switch (claimTypeID)
            {
                case 1:
                    TempData.Remove(Constants.ClaimWizardType.ClientDetails.ToString());
                    TempData.Remove(Constants.ClaimWizardType.PolicyDetails.ToString());
                    TempData.Remove(Constants.ClaimWizardType.InsuredDetails.ToString());
                    TempData.Remove(Constants.ClaimWizardType.ClaimDetails.ToString());
                    TempData.Remove(Constants.ClaimWizardType.Reserve.ToString());
                    TempData.Remove(Constants.ClaimWizardType.ClaimCoverage.ToString());
                    TempData.Remove(Constants.ClaimWizardType.ClaimSubLimit.ToString());
                    TempData.Remove(Constants.ClaimWizardType.ClaimLienHolder.ToString());
                    break;
                case 2:
                    TempData.Remove(Constants.ClaimWCWizardType.ClientDetails.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.EmployerDetails.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.PolicyDetails.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.ClaimantDetails.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.InjuryDetails.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.OshaDetails.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.Reserve.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.ClaimCoverage.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.ClaimExcessLimit.ToString());
                    TempData.Remove(Constants.ClaimWCWizardType.ClaimLienHolder.ToString());
                    break;
                default:
                    TempData.Remove(Constants.ClaimLibWizardType.ClientDetails.ToString());
                    TempData.Remove(Constants.ClaimLibWizardType.PolicyDetails.ToString());
                    TempData.Remove(Constants.ClaimLibWizardType.InsuredDetails.ToString());
                    TempData.Remove(Constants.ClaimLibWizardType.ClaimDetails.ToString());
                    TempData.Remove(Constants.ClaimLibWizardType.Reserve.ToString());
                    TempData.Remove(Constants.ClaimLibWizardType.ClaimCoverage.ToString());
                    TempData.Remove(Constants.ClaimLibWizardType.ClaimSubLimit.ToString());
                    TempData.Remove(Constants.ClaimLibWizardType.ClaimLienHolder.ToString());
                    TempData.Remove(Constants.ClaimLibWizardType.ClaimantDetails.ToString());
                    break;
            }

        }
        private T ReadWizardData<T>(Constants.ClaimWizardType formType)
        {
            var paths = Server.MapPath(Constants.ClaimWizardPath + AuthUser.User.UserId + "_" + formType + ".xml");
            return !System.IO.File.Exists(paths) ? (T)Activator.CreateInstance(typeof(T)) : Common.Common.ReadFromXmlFile<T>(paths);
        }
        private void WriteWizardData<T>(T obj, Constants.ClaimWizardType formType) where T : new()
        {
            var path = Server.MapPath(Constants.ClaimWizardPath + AuthUser.User.UserId + "_" + formType + ".xml");

            Common.Common.WriteToXmlFile(path, obj);
        }
        private void DeleteAllWizardFile()
        {
            var directoryPath = Server.MapPath(Common.Constants.ClaimWizardPath);
            var dir = new System.IO.DirectoryInfo(directoryPath);

            foreach (var file in dir.EnumerateFiles(AuthUser.User.UserId + "_*.xml"))
            {
                file.Delete();
            }

        }

        # endregion

        #region ActivityLog

        public ActionResult ActivityLogIndex(int? page, int claimid)
        {
            Mapper.CreateMap<ClaimActivityLog, ActivityLogViewModel>();
            var activityLogList = _claimrepository.GetActivityLogByClaimId(claimid);
            var pageNumber = (page ?? 1);
            ViewBag.claimid = claimid;
            return PartialView("ActivityLogIndex", activityLogList.ToPagedList(pageNumber, Common.Constants.PageSize));
        }

        #endregion

        #region check Policy No Duplicacy
        [HttpPost]
        public JsonResult DoesPolicyNoExist(string PolicyNumber)
        {
            bool IsExists = _claimrepository.IsPolicyNoExists(PolicyNumber);
            return Json(!IsExists);
        }
        #endregion

        #region Check Claim No Duplicacy
        [HttpPost]
        public JsonResult DoesClaimNoExist(string InsurerClaimNumber)
        {
            bool IsExists = _claimrepository.IsClaimNoExists(InsurerClaimNumber);
            return Json(!IsExists);
        }
        #endregion

        #region XactAnalysisIntegration
        private void ClaimSendToXactAnalysis(int ClaimID)
        {
            try
            {
                IEnumerable<XactImportViewModel> ClaimData = _XactImport.GetClaimData(ClaimID).ToList().Take(1);
                XactFTPWrite XactFTP = new XactFTPWrite();
                foreach (XactImportViewModel Claim in ClaimData)
                {
                    string TransactionID = XactFTP.SendClaim(Claim, Claim.ClaimID);

                    if (TransactionID != "")
                    {
                        if (TransactionID != "XML not generated.")
                        {
                            //SaveLog("Claim of Claim ID " + ClaimID + " Send to XactAnalysis successfully.XactAnalysis Transaction ID :" + TransactionID, "XactAnalysis", AuthUser.User.UserId, ClaimID);

                            //--------------Update Transaction ID-----------------------
                            var getClaimIDData = _claim.Get(ClaimID);
                            var XactTransactionID = getClaimIDData.XactTransactionID;
                            if (XactTransactionID == null)
                            {
                                getClaimIDData.ClaimID = ClaimID;
                                getClaimIDData.XactTransactionID = TransactionID;
                                _claim.Update(getClaimIDData);
                                _claim.Save();
                            }
                            else
                            {
                                //SaveLog("Transaction ID (" + XactTransactionID + ") of XactAnalysis already exist of Claim ID " + ClaimID + ".", "XactAnalysis", AuthUser.User.UserId, ClaimID);
                            }
                        }
                        else
                        {
                            //SaveLog("XML of Claim ID " + ClaimID + " not created.", "XactAnalysis", AuthUser.User.UserId, ClaimID);
                        }
                    }
                    else
                    {
                        //SaveLog("Claim of claim ID " + ClaimID + "  send to XactAnalysis but not get Successful response from XactAnalysis.", "XactAnalysis", AuthUser.User.UserId, ClaimID);

                        // ClaimEditViewModel claim = _claim.Get(Convert.ToInt32(claimId));
                        // _claim.UpdateFields(claim, new List<string> { "ClaimID", "PolicyNumber", "InsurerClaimNumber", "AdjusterFile" });
                        // _claim.Save();
                        //----------------------------------------------------------
                    }
                    break;
                }
            }
            catch(Exception ex)
            {
              var msg=  ex.Message;
                //SaveLog("Claim of Claim ID " + ClaimID + " not send to XactAnalysis due to problem", "XactAnalysis", AuthUser.User.UserId, ClaimID);
            }
        }
        #endregion

        [HttpPost]
        public ActionResult XClaim(ClaimsType model)
        {

            if (!model.IsProperty && !model.IsWc && !model.IsLib)
            {
                this.ShowMessage(MessageType.Error, "Please select a claim type", true);
                return View("ClaimType", model);
            }

            var portaldetail = _claimrepository.GetStartingAdjuster((int)AuthUser.User.PortalId);

            if (string.IsNullOrEmpty(portaldetail.StartingAdjuster))
            {
                this.ShowMessage(MessageType.Error, "Please create an Adjuster File", true);
                return View("ClaimType", model);
            }

            string StartingAdjusterPortal = (from x in _context.Portals
                                             where x.PortalID == AuthUser.User.PortalId
                                             select x.StartingAdjuster).FirstOrDefault();

            string StartingAdjusterClaim = (from x in _context.Claims
                                            where x.PortalID == AuthUser.User.PortalId
                                            select x.AdjusterFile).FirstOrDefault();

            if (StartingAdjusterPortal == null && StartingAdjusterClaim == null)
            {
                this.ShowMessage(MessageType.Error, "Please add an Adjuster ClaimNumber before creating Claim.", true);
                return View("ClaimType", model);
            }
            WriteWizardData(model, Constants.ClaimWizardType.ClaimType);
            TempData["TempClaimType"] = model;
            if (model.IsProperty)
            {
                ClearTempData(1);


                return RedirectToAction("XClientDetail", "Claim", "Claims");
            }
            if (model.IsWc)
            {
                ClearTempData(2);
                return RedirectToAction("Index", "ClaimWC", new { model = model });
            }
            if (model.IsLib)
            {
                ClearTempData(3);
                return RedirectToAction("Index", "ClaimLib", new { model = model });
            }
            return RedirectToAction("XClientDetail", "Claim", "Claims");
        }
        public ActionResult XClientDetail()
        {
            ViewBag.ClientBranch = _claimrepository.GetUserBranchCodeByUserID(AuthUser.User.UserId);
            ViewBag.CompanyBranchID = new SelectList(_companyBranch.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId), "PortalBranchId", "BranchCode", ViewBag.ClientBranch);
            //ViewBag.ClientID = getClientList();


            var obj = ReadWizardData<XactClaimViewModel>(Constants.ClaimWizardType.ClientDetails);
            ViewBag.ClientID = getClientListWithClaimType();
            if (obj != null)
            {
                //ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == obj.ClientID), "CBranchId", "CBranchName");
                ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == obj.ClientID), "CBranchId", "CBranchName");// adding zero in condition so that selectlist has no entries
                ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == obj.ClientID), "BillProfileID", "ProgrameName");
            }
            else
            {
                //ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0), "CBranchId", "CBranchName");
                ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0), "CBranchId", "CBranchName");
                ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == 0), "BillProfileID", "ProgrameName");
            }
            return View("XClientDetail", obj);
        }

        [HttpPost]
        public ActionResult XClientDetail(XactClaimViewModel model)
        {
            //if (btnBack != null) //When back press
            //{
            //    return RedirectToAction("CreateNew");
            //}
            //else //if (btnNext != null) when next press
            //{

            if (_claimrepository.PolicyandInsurerClaimNumberExists(model.PolicyNumber, model.InsurerClaimNumber, 1) == false)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.CompanyBranchID = new SelectList(_companyBranch.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId), "PortalBranchId", "BranchCode");
                    ViewBag.ClientID = getClientListWithClaimType(); //new SelectList(_client.GetAll(), "ClientID", "ClientName");
                    ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0), "CBranchId", "CBranchName");
                    ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == 0), "BillProfileID", "ProgrameName");
                }
                model.UserID = AuthUser.User.UserId;
                model.PortalID = AuthUser.User.PortalId;
                TempData["policyNo"] = model.PolicyNumber;
                TempData.Keep("policyNo");
                WriteWizardData(model, Constants.ClaimWizardType.ClientDetails);
                return RedirectToAction("XPolicyDetail");
            }
            else
            {
                this.ShowMessage(MessageType.Error, "PolicyNumber or Insurer ClaimNumber already exists", true);
                ViewBag.CompanyBranchID = new SelectList(_companyBranch.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId), "PortalBranchId", "BranchCode");

                ViewBag.ClientID = getClientListWithClaimType(); //new SelectList(_client.GetAll(), "ClientID", "ClientName");
                ViewBag.ClientBranchID = new SelectList(_clientBranch.GetAll().Where(x => x.ClientID == 0 && x.IsDeleted != true), "CBranchId", "CBranchName");
                ViewBag.BillingProfileID = new SelectList(_billingProfile.GetAll().Where(x => x.ClientID == 0), "BillProfileID", "ProgrameName");
            }
            //}
            return View("XClientDetail", model);
        }

        #region PolicyDetails and Send to Xact

        public ActionResult XPolicyDetail()
        {

            ViewBag.PolicyTypeID = new SelectList(_policyType.GetAll().Where(x => (x.PortalID == AuthUser.User.PortalId || x.PortalID== null) && x.ClaimTypeID == 1), "ID", "PolicyType");

            var obj = ReadWizardData<XactPolicyDetailViewModel>(Constants.ClaimWizardType.PolicyDetails);

            var objNew = ReadWizardData<XactClaimViewModel>(Constants.ClaimWizardType.ClientDetails);
            ViewBag.ClientID = getClientListWithClaimType();
            if (objNew != null)
            {
                TempData["policyNo"] = objNew.PolicyNumber;
            }

            if (obj.PolicyNumber == null)
                obj.PolicyNumber = TempData["policyNo"] == null ? "" : TempData["policyNo"].ToString();
            return View("XPolicyDetail", obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XPolicyDetail(XactPolicyDetailViewModel model, string btnBack, string btnNext)
        {
            if (btnBack != null) //When back press
            {
                return RedirectToAction("XClientDetail");
            }
            else //if (btnNext != null) when next press
            {
                if (!ModelState.IsValid)
                {
                    //ViewBag.CoverageFirmID = new SelectList(_coverage.GetAll().Where(x => x.PortalID == AuthUser.User.PortalId && x.ClaimTypeID == 1), "CoverageFirmID", "CoverageFirm");
                    return View("XPolicyDetail", model);
                }
                model.UserID = AuthUser.User.UserId;
                model.PortalID = AuthUser.User.PortalId;
                WriteWizardData(model, Constants.ClaimWizardType.PolicyDetails);

                //Save Coverage
                var coveragelist = (List<CreateClaimCoverageViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimCoverage.ToString()) ?? ReadWizardData<List<CreateClaimCoverageViewModel>>(Constants.ClaimWizardType.ClaimCoverage);
                WriteWizardData(coveragelist, Constants.ClaimWizardType.ClaimCoverage);

                var SubLimitlist = (List<CreateClaimSubLimitsViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimSubLimit.ToString()) ?? ReadWizardData<List<CreateClaimSubLimitsViewModel>>(Constants.ClaimWizardType.ClaimSubLimit);
                WriteWizardData(SubLimitlist, Constants.ClaimWizardType.ClaimSubLimit);

                var LieonHolderlist = (List<ClaimLienHolderViewModel>)TempData.Peek(Constants.ClaimWizardType.ClaimLienHolder.ToString()) ?? ReadWizardData<List<ClaimLienHolderViewModel>>(Constants.ClaimWizardType.ClaimLienHolder);
                WriteWizardData(LieonHolderlist, Constants.ClaimWizardType.ClaimLienHolder);
            }
            return RedirectToAction("XInsuredDetails");
        }

        #endregion

        #region InsuredDetails and Send to Xact

        public ActionResult XInsuredDetails()
        {
            ViewBag.State = new SelectList(_states.GetAll(), "StateID", "StateName");
            var obj = ReadWizardData<XactInsuredViewModel>(Constants.ClaimWizardType.InsuredDetails);
            if (obj.LossCountry == "" || obj.LossCountry == null)
            {
                obj.LossCountry = "US";
            }
            if (obj.MailingCountry == "" || obj.MailingCountry == null)
            {
                obj.MailingCountry = "US";
            }
            return View("XInsuredDetails", obj);
        }

        [HttpPost]
        public ActionResult XInsuredDetails(XactInsuredViewModel model, string btnBack, string btnNext)
        {
            if (btnBack != null) //When back press
            {
                return RedirectToAction("XPolicyDetail");
            }
            else //if (btnNext != null) when next press
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.State = new SelectList(_states.GetAll(), "StateID", "StateName");
                    return View("XInsuredDetails", model);
                }
                WriteWizardData(model, Constants.ClaimWizardType.InsuredDetails);
                return RedirectToAction("XClaimDetails");
            }
        }

        #endregion



        #region ClaimDetails and Send to Xact
        public ActionResult XClaimDetails()
        {
            ViewBag.TypeOfLoss = new SelectList(_typeOfLoss.GetAllWithClaimType(AuthUser.User.Roles, AuthUser.User.PortalId, 1), "TypeOfLossID", "TypeOfLoss");//z
            DDLTime();
            var obj = ReadWizardData<XactClaimDetailsViewModel>(Constants.ClaimWizardType.ClaimDetails);
            return View("XClaimDetails", obj);
        }

        [HttpPost]
        public ActionResult XClaimDetails(XactClaimDetailsViewModel model, string btnBack, string btnNext, bool assignAdjuster)
        {
            if (btnBack != null) //When back press
            {
                return RedirectToAction("XInsuredDetails");
            }
            else //if (btnNext != null) when next press
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.TypeOfLoss = new SelectList(_typeOfLoss.GetAllWithClaimType(AuthUser.User.Roles, AuthUser.User.PortalId, 1), "TypeOfLossID", "TypeOfLoss");//z
                    DDLTime();
                    return View("XClaimDetails", model);
                }
                WriteWizardData(model, Constants.ClaimWizardType.ClaimDetails);
                XactSaveClaim("Skip");
                this.ShowMessage(MessageType.Success, "Claim saved successfully.", true);
                var claimId = TempData["ClaimID"];
                var claimTypeId = TempData["ClaimTypeID"];
                //----------------Code for XactAnalysis Integration------------------
                if (claimTypeId.ToString() == "1")
                {
                    int ClaimID = Convert.ToInt32(claimId);
                    ClaimSendToXactAnalysis(ClaimID);
                }
                //-------------------------------------------------------------------
                TempData.Remove("ClaimID"); TempData.Remove("ClaimTypeID");
               // return RedirectToAction("Edit", "ClaimEdit", new { id = claimId, ClaimTypeID = claimTypeId });

                return Json(new { redirectTo = Url.Action("Edit", "ClaimEdit", new { id = claimId, ClaimTypeID = claimTypeId }), adjusterList = Url.Action("AdjusterList", "ClaimEdit", new { claimId = claimId }), assignAdjuster = assignAdjuster }); 

                //return RedirectToAction("XReserveDetails");
            }
        }
        #endregion


        #region Reserve and Claim Send to Xact
        public ActionResult XReserveDetails()
        {
            ViewBag.ReserveCategoryID = new SelectList(_reserveCategory.GetAllWithClaimType(AuthUser.User.PortalId, AuthUser.User.Roles, 1), "ReserveCategoryId", "CategoryName");
            var obj = ReadWizardData<ClaimReserveViewModel>(Constants.ClaimWizardType.Reserve);
            return View("XReserveDetails", obj);
        }

        [HttpPost]
        public ActionResult XReserveDetails(ClaimReserveViewModel model, string btnBack, string btnNext)
        {
            if (btnBack != null) //When back press
            {

                return RedirectToAction("XClaimDetails");
            }
            else //if (btnNext != null) when next press
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ReserveCategoryID = new SelectList(_reserveCategory.GetAllWithClaimType(AuthUser.User.PortalId, AuthUser.User.Roles, 1), "ReserveCategoryId", "CategoryName");
                    return View("XReserveDetails", model);
                }
                WriteWizardData(model, Constants.ClaimWizardType.Reserve);
            }

            XactSaveClaim(btnNext);
            TempData.Remove("ClaimTabState");
            this.ShowMessage(MessageType.Success, "Claim saved successfully.", true);
            var claimId = TempData["ClaimID"];
            var claimTypeId = TempData["ClaimTypeID"];

            //----------------Code for XactAnalysis Integration------------------
            if (claimTypeId.ToString() == "1")
            {
                int ClaimID = Convert.ToInt32(claimId);
                ClaimSendToXactAnalysis(ClaimID);
            }
            //-------------------------------------------------------------------
            TempData.Remove("ClaimID"); TempData.Remove("ClaimTypeID");
            return RedirectToAction("Edit", "ClaimEdit", new { id = claimId, ClaimTypeID = claimTypeId });
        }

        public ActionResult XSkipReserveDetails(string Skipsave)
        {
            XactSaveClaim(Skipsave);
            this.ShowMessage(MessageType.Success, "Claim saved successfully.", true);
            //return RedirectToAction("Index", "ClaimEdit");


            var claimId = TempData["ClaimID"];
            var claimTypeId = TempData["ClaimTypeID"];
            //----------------Code for XactAnalysis Integration------------------
            if (claimTypeId.ToString() == "1")
            {
                int ClaimID = Convert.ToInt32(claimId);
                ClaimSendToXactAnalysis(ClaimID);
            }
            //-------------------------------------------------------------------
            TempData.Remove("ClaimID"); TempData.Remove("ClaimTypeID");
            return RedirectToAction("Edit", "ClaimEdit", new { id = claimId, ClaimTypeID = claimTypeId });
        }

        #endregion


        #region FinalSave and send to Xact

        private void XactSaveClaim(string btnname)
        {
            using (var transaction = new TransactionScope())
            {
                //Save Claim
                var client_obj = ReadWizardData<XactClaimViewModel>(Constants.ClaimWizardType.ClientDetails);
                var policy_obj = ReadWizardData<XactPolicyDetailViewModel>(Constants.ClaimWizardType.PolicyDetails);
                var claim_obj = ReadWizardData<XactClaimDetailsViewModel>(Constants.ClaimWizardType.ClaimDetails);

                client_obj.PolicyFormType = policy_obj.PolicyFormType;
                client_obj.PolicyTypeID = policy_obj.PolicyTypeID;
                client_obj.InitialCoverageDate = policy_obj.InitialCoverageDate;
                client_obj.EffectiveDate = policy_obj.EffectiveDate;
                client_obj.ExpirationDate = policy_obj.ExpirationDate;

                client_obj.CategoryID = claim_obj.CategoryId;
                client_obj.LossDate = claim_obj.LossDate;
                client_obj.DateReceived = claim_obj.DateReceived;
                client_obj.TypeofLoss = claim_obj.TypeofLoss;
                client_obj.LossDescription = claim_obj.LossDescription;
                client_obj.ClaimTypeID = 1;

                if (client_obj.ClientBranchID == 0)
                {
                    client_obj.ClientBranchID = null;
                }

                Mapper.CreateMap<XactClaimViewModel, Claim>();
                Claim claim = Mapper.Map<XactClaimViewModel, Claim>(client_obj);
                claim.DateEntered = DateTime.Now;
                claim.ReceivedFrom = claim_obj.ReceivedFrom;
                claim.TimeReceived = claim_obj.TimeReceived;
                claim.TimeReceivedPeriod = claim_obj.TimeReceivedPeriod;
                claim.isSendToXact = true;
                _claim.Add(claim);
                _claim.Save();
                if (TempData["ClaimID"] != null)
                {
                    TempData.Remove("ClaimID");

                }
                TempData.Add("ClaimID", claim.ClaimID);
                if (TempData["ClaimTypeID"] != null)
                {
                    TempData.Remove("ClaimTypeID");

                }
                TempData.Add("ClaimTypeID", claim.ClaimTypeID);
                //--------------------------Generate Adjuster File No---------------------------------------------------------------
                var portaldetail = _claimrepository.GetStartingAdjuster((int)AuthUser.User.PortalId);
                var data = _claimrepository.GetLastAdjuster((int)AuthUser.User.PortalId);

                client_obj.AdjusterFileNumber = data != null ? data.AdjusterFile.Split('-')[0] + "-" + (Convert.ToInt32(data.AdjusterFile.Split('-')[1]) + 1) : portaldetail.StartingAdjuster;

                //if (portaldetail.StartingAdjuster == null)
                //    client_obj.AdjusterFileNumber = portaldetail.Pid + "-" + claim.ClaimID;
                //else
                //    client_obj.AdjusterFileNumber = portaldetail.StartingAdjuster + claim.ClaimID;

                if (client_obj.InsurerClaimNumber == null)
                    client_obj.InsurerClaimNumber = client_obj.AdjusterFileNumber;


                if (client_obj.PolicyNumber == null)
                    client_obj.PolicyNumber = client_obj.AdjusterFileNumber;

                claim.PolicyNumber = client_obj.PolicyNumber;
                claim.InsurerClaimNumber = client_obj.InsurerClaimNumber;
                claim.AdjusterFile = client_obj.AdjusterFileNumber;
                _claim.UpdateFields(claim, new List<string> { "ClaimID", "PolicyNumber", "InsurerClaimNumber", "AdjusterFile" });
                _claim.Save();
                //-----------------------------End------------------------------------------------------------------------------------
                //ClaimLog
                SaveLog("Claim : " + client_obj.InsurerClaimNumber + " saved successfully", "Claim", AuthUser.User.UserId, claim.ClaimID);

                //Implement Automated Diary (by amit)
                _claimrepository.InsertAutoDiaries(claim);

                //Add Coverage
                var coverage_obj = ReadWizardData<List<CreateClaimCoverageViewModel>>(Constants.ClaimWizardType.ClaimCoverage);
                foreach (var cr in coverage_obj)
                {
                    Mapper.CreateMap<CreateClaimCoverageViewModel, ClaimCoverage>();
                    ClaimCoverage coverage = Mapper.Map<CreateClaimCoverageViewModel, ClaimCoverage>(cr);
                    coverage.ClaimID = claim.ClaimID;
                    _claimcoverage.Add(coverage);
                    //Coverage Log
                    SaveLog(coverage.CoverageName + " Added to Claim", "Policy", AuthUser.User.UserId, claim.ClaimID);
                }
                _claim.Save();//zi
                //Add LossTotal according to claims
                //fetch all coverages for this claimID
                //
                IEnumerable<ClaimCoverage> coverageList = _claimcoveragerepository.GetCoverageListByClaimID(claim.ClaimID);
                foreach (ClaimCoverage coverage in coverageList)
                {
                    ClaimLossTotal claimloss = new ClaimLossTotal();
                    claimloss.ClaimACVRCVIncured = coverage.ACVRCV;
                    claimloss.ClaimCATDeductible = coverage.CATdecuctible;
                    claimloss.ClaimDeductible = coverage.Deductible;
                    claimloss.ClaimCoverageID = coverage.ClaimCovergaeID;
                    claimloss.ClaimID = coverage.ClaimID;
                    _ClaimLossTotal.Add(claimloss);
                    _ClaimLossTotal.Save();
                }
                _claim.Save();
                //

                //Add Sublimit
                var sublimit_obj = ReadWizardData<List<CreateClaimSubLimitsViewModel>>(Constants.ClaimWizardType.ClaimSubLimit);
                foreach (var sub in sublimit_obj)
                {
                    //Get claimcoverageID
                    int coverageid = _claimcoveragerepository.GetClaimCoveragebyName(sub.SubLimitCoverage, claim.ClaimID);

                    Mapper.CreateMap<CreateClaimSubLimitsViewModel, ClaimSubLimit>();
                    ClaimSubLimit sublimit = Mapper.Map<CreateClaimSubLimitsViewModel, ClaimSubLimit>(sub);
                    sublimit.ClaimID = claim.ClaimID;
                    sublimit.ClaimCoverageID = coverageid;
                    _claimsublimit.Add(sublimit);
                    //Sublimit Log
                    SaveLog("Sublimit saved successfully", "Sublimit", AuthUser.User.UserId, claim.ClaimID);
                }
                _claim.Save();//zi
                //Add LienHolder
                var lien_obj = ReadWizardData<List<ClaimLienHolderViewModel>>(Constants.ClaimWizardType.ClaimLienHolder);
                foreach (var lh in lien_obj)
                {
                    Mapper.CreateMap<ClaimLienHolderViewModel, LienHolder>();
                    LienHolder lienholder = Mapper.Map<ClaimLienHolderViewModel, LienHolder>(lh);
                    lienholder.ClaimID = claim.ClaimID;
                    _lienholder.Add(lienholder);
                    //LienHolder Log
                    SaveLog("LienHolder : " + lienholder.LienHolderName + " saved successfully", "LienHolder", AuthUser.User.UserId, claim.ClaimID);
                }


                //Add Insured
                var insured_obj = ReadWizardData<XactInsuredViewModel>(Constants.ClaimWizardType.InsuredDetails);
                Mapper.CreateMap<XactInsuredViewModel, Insured>();
                Insured insured = Mapper.Map<XactInsuredViewModel, Insured>(insured_obj);
                insured.ClaimID = claim.ClaimID;
                _insuredRepository.Add(insured);

                //Insured Log
                SaveLog("Insured : " + insured.InsuredName + " saved successfully", "Insured", AuthUser.User.UserId, claim.ClaimID);

                if (btnname != "Skip")
                {
                    //Add reserve
                    var reserve_obj = ReadWizardData<ClaimReserveViewModel>(Constants.ClaimWizardType.Reserve);
                    Mapper.CreateMap<ClaimReserveViewModel, ClaimReserve>();
                    ClaimReserve claimreserve = Mapper.Map<ClaimReserveViewModel, ClaimReserve>(reserve_obj);
                    claimreserve.ClaimID = claim.ClaimID;

                    claimreserve.currentamount = reserve_obj.ReserveAmount;
                    claimreserve.lastchangeamount = 0;
                    claimreserve.claimdate = DateTime.Now;
                    claimreserve.AmountPaid = 0; //payment
                    claimreserve.SettlementType = null;
                    claimreserve.RecoveredAmount = 0;
                    claimreserve.SubrogationRecovery = 0;
                    claimreserve.RecoveryFees = 0;
                    claimreserve.TotalIncurred = claimreserve.currentamount;
                    claimreserve.Isincrease = true;
                    claimreserve.changedby = AuthUser.User.UserName;
                    claimreserve.lastpercentage = Convert.ToDecimal(claimreserve.currentamount);
                    claimreserve.ReserveBalance = Convert.ToDecimal(claimreserve.currentamount);
                    _claimReserve.Add(claimreserve);

                    //Add ReserveHistory
                    ReserveHistory rh = new ReserveHistory();
                    rh.ClaimReserveid = claimreserve.ClaimReserveID;
                    rh.AmountPaid = claimreserve.AmountPaid;
                    rh.Balanceamount = Convert.ToDecimal(claimreserve.currentamount);
                    rh.Change = "Increase";
                    rh.Changeamount = Convert.ToDecimal(claimreserve.currentamount);
                    rh.changedby = AuthUser.User.UserName;
                    rh.Initialamount = 0;
                    rh.Percentchange = Convert.ToDecimal(claimreserve.currentamount);
                    rh.Reason = claimreserve.Reason;
                    rh.RecoveredAmount = 0;
                    rh.RecoveryFees = 0;
                    rh.Savedate = DateTime.Now;
                    rh.SettlementType = null;
                    rh.SubrogationRecovery = 0;
                    rh.TotalIncurred = Convert.ToDecimal(claimreserve.currentamount);
                    _reservehistory.Add(rh);

                    _claimReserve.Add(claimreserve);
                    //ClaimReserve Log
                    SaveLog("Claim Reserve saved successfully", "Reserve", AuthUser.User.UserId, claim.ClaimID);
                }

                _claim.Save();
                //============Claim Detail===============
                ClaimDetail clmDetail = new ClaimDetail();
                clmDetail.Date = DateTime.Now;
                clmDetail.ClaimId = claim.ClaimID;
                clmDetail.Status = "Open";
                clmDetail.UserId = AuthUser.User.UserId;
                _claimDetail.Add(clmDetail);
                _claimDetail.Save();
                //===========End================
                TempData.Remove(Constants.ClaimWizardType.ClaimCoverage.ToString());
                TempData.Remove(Constants.ClaimWizardType.ClaimSubLimit.ToString());
                TempData.Remove(Constants.ClaimWizardType.ClaimLienHolder.ToString());

                //Delete xml files
                DeleteAllWizardFile();

                transaction.Complete();

            }

        }
        #endregion

        #region loop for time field
        public void DDLTime()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            for (int hr = 0; hr <= 11; hr++)
            {
                for (int min = 0; min <= 59; min++)
                {
                    listItems.Add(new SelectListItem { Text = hr.ToString("00") + ":" + min.ToString("00"), Value = hr.ToString("00") + ":" + min.ToString("00") });
                }
            }
            listItems.Add(new SelectListItem { Text = "12:00", Value = "12:00" });
            listItems.RemoveAll(a => a.Text == "00:00");
            ViewBag.Time = new SelectList(listItems, "Value", "Text", selectedValue: "09:00");

            var AMPM = new List<SelectListItem> 
            {
                new SelectListItem { Text = "AM", Value = "AM" },
                new SelectListItem { Text = "PM", Value = "PM" }
            };
            ViewBag.AMPM = new SelectList(AMPM, "Value", "Text");
        }
        #endregion

    }
}