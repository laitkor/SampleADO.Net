// Copyright (c) 2016, SolidCP
// SolidCP is distributed under the Creative Commons Share-alike license
// 
// SolidCP is a fork of WebsitePanel:
// Copyright (c) 2015, Outercurve Foundation.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  Outercurve Foundation  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Microsoft.ApplicationBlocks.Data;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SolidCP.EnterpriseServer
{
    public static class SignupDataProvider
    {
        static string EnterpriseServerRegistryPath = "SOFTWARE\\SolidCP\\EnterpriseServer";
        private static string ConnectionString
        {
            get
            {
                string ConnectionKey = ConfigurationManager.AppSettings["SolidCP.AltConnectionString"];
                string value = string.Empty;

                if (!string.IsNullOrEmpty(ConnectionKey))
                {
                    RegistryKey root = Registry.LocalMachine;
                    RegistryKey rk = root.OpenSubKey(EnterpriseServerRegistryPath);
                    if (rk != null)
                    {
                        value = (string)rk.GetValue(ConnectionKey, null);
                        rk.Close();
                    }
                }

                if (!string.IsNullOrEmpty(value))
                    return value;
                else
                    return ConfigurationManager.ConnectionStrings["EnterpriseServer"].ConnectionString;
            }
        }
        private static string ObjectQualifier
        {
            get
            {
                return "";
            }
        }
        public static string AddUser(UserInfo User, bool check, string password, int plandId, string domainName)
        {
            SqlParameter prmErrormsg = new SqlParameter("@errorMsg", SqlDbType.VarChar, 100);
            prmErrormsg.Direction = ParameterDirection.Output;

            SqlParameter prmUID = new SqlParameter("@UID", SqlDbType.Int);
            prmUID.Direction = ParameterDirection.Output;

            // add user to SolidCP Users table
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "Signup",
                prmErrormsg, prmUID,
                //prmErrormsg,
                new SqlParameter("@RoleID", 3),
                new SqlParameter("@StatusId", User.StatusId),
                new SqlParameter("@SubscriberNumber", User.SubscriberNumber),
                new SqlParameter("@LoginStatusId", User.LoginStatusId),
                new SqlParameter("@IsDemo", User.IsDemo),
                new SqlParameter("@IsPeer", User.IsPeer),
                new SqlParameter("@Comments", User.Comments),
                new SqlParameter("@username", User.Username),
                new SqlParameter("@password", CryptoUtils.Encrypt(password)),
                new SqlParameter("@firstName", User.FirstName),
                new SqlParameter("@lastName", User.LastName),
                new SqlParameter("@email", User.Email),
                new SqlParameter("@secondaryEmail", User.SecondaryEmail),
                new SqlParameter("@address", User.Address),
                new SqlParameter("@city", User.City),
                new SqlParameter("@country", User.Country),
                new SqlParameter("@state", User.State),
                new SqlParameter("@zip", User.Zip),
                new SqlParameter("@primaryPhone", User.PrimaryPhone),
                new SqlParameter("@secondaryPhone", User.SecondaryPhone),
                new SqlParameter("@fax", User.Fax),
                new SqlParameter("@instantMessenger", User.InstantMessenger),
                new SqlParameter("@htmlMail", User.HtmlMail),
                new SqlParameter("@CompanyName", User.CompanyName),
                new SqlParameter("@PlanId", plandId),
                new SqlParameter("@DomainName", domainName),
                new SqlParameter("@NoOfuser", User.NoOfuser),
                new SqlParameter("@EcommerceEnabled", User.EcommerceEnabled));

            return Convert.ToString(prmErrormsg.Value + "&" + prmUID.Value);
        }
        public static int VerifyEmail(int userId)
        {
            SqlParameter ret = new SqlParameter("@ret", SqlDbType.Int);
            ret.Direction = ParameterDirection.Output;


            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "VerifyEmail",
                ret,
                //prmErrormsg,
                new SqlParameter("@userId", userId));

            //string errormsg = Convert.ToString(prmErrormsg.Value);
            return Convert.ToInt32(ret.Value);
        }

        public static int checkExit(string name, string controlName)
        {
            int cond = 0;
            if (controlName == "ctl22_ctl01_ctl00_txtUsername")
                cond = 1;
            if (controlName == "ctl22_ctl01_ctl00_txtDomainName")
                cond = 2;
            SqlParameter ret = new SqlParameter("@ret", SqlDbType.Int);
            ret.Direction = ParameterDirection.Output;


            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "checkExit",
                ret,
                new SqlParameter("@cond", cond),
                new SqlParameter("@name", name));

            //string errormsg = Convert.ToString(prmErrormsg.Value);
            return Convert.ToInt32(ret.Value);
        }


        public static int UpdateSignupusers(int userId, string txnId, string paymentGateway, decimal amount)
        {
            SqlParameter ret = new SqlParameter("@ret", SqlDbType.Int);
            ret.Direction = ParameterDirection.Output;

            // add user to SolidCP Users table
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "UpdateSignupusers",
                ret,
                //prmErrormsg,
                new SqlParameter("@userId", userId),
                  new SqlParameter("@amt", amount),
                new SqlParameter("@txnId", txnId),
                new SqlParameter("@paymentGateway", paymentGateway));

            //string errormsg = Convert.ToString(prmErrormsg.Value);
            return Convert.ToInt32(ret.Value);
        }
        public static int UpdateNoOfusers(int userId, int NoOfusers)
        {
            SqlParameter ret = new SqlParameter("@ret", SqlDbType.Int);
            ret.Direction = ParameterDirection.Output;

            // add user to SolidCP Users table
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "UpdateNoOfusers",
                ret,
                //prmErrormsg,
                new SqlParameter("@userId", userId),
                  new SqlParameter("@NoOfusers", NoOfusers));

            //string errormsg = Convert.ToString(prmErrormsg.Value);
            return Convert.ToInt32(ret.Value);
        }
        public static int PayPalTransaction(int userId, string txnId, decimal amount, string TranType, string paymentGateway)
        {
            SqlParameter ret = new SqlParameter("@ret", SqlDbType.Int);
            ret.Direction = ParameterDirection.Output;

            // add user to SolidCP Users table
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "PayPalTransaction",
                ret,
                //prmErrormsg,
                new SqlParameter("@userId", userId),
                new SqlParameter("@amt", amount),
                new SqlParameter("@tranType", TranType),
                new SqlParameter("@txnId", txnId),
                new SqlParameter("@paymentGateway", paymentGateway));

            //string errormsg = Convert.ToString(prmErrormsg.Value);
            return Convert.ToInt32(ret.Value);
        }
        public static System.Data.DataSet getDomainVerificationStatus(int userId)
        {
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "getDomainVerificationStatus",
                new SqlParameter("@userId", userId));


        }

        public static DataSet GetSignupAvailableHostingPlans(int userId, string source)
        {
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "GetSignupAvailableHostingPlans",
                new SqlParameter("@userId", userId),
                 new SqlParameter("@source", source));
        }
        public static int UpdatePopupInfo(PopupInfo pi)
        {
            SqlParameter ret = new SqlParameter("@ret", SqlDbType.Int);
            ret.Direction = ParameterDirection.Output;

            // add user to SolidCP Users table
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure,
                ObjectQualifier + "UpdatePopupInfo",
                ret,
                //prmErrormsg,
                new SqlParameter("@Id", pi.Id),
                  new SqlParameter("@alerttext", pi.AlertText),
                new SqlParameter("@popuptext", pi.PopupText));

            //string errormsg = Convert.ToString(prmErrormsg.Value);
            return Convert.ToInt32(ret.Value);
        }
    }
}
