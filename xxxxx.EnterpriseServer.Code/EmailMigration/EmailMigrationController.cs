using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidCP.EnterpriseServer
{
    public class EmailMigrationController
    {
        public static DataSet GetEmailMigrations(int userId, string sortColumn,
           int startRow, int maximumRows)
        {
            return DataProvider.GetEmailMigrations(userId, sortColumn,
                startRow, maximumRows);
        }
       
       
        public static int AddMigration(string MigrationName, string MigrationProtocol, 
            string MaximumConnectionLimit, string PathPrefix, string ExcludeFolderList, string BlackOutTime, string TimeZone, int UserID)
        {
            return DataProvider.AddMigration(MigrationName, MigrationProtocol,
                MaximumConnectionLimit, PathPrefix, ExcludeFolderList, BlackOutTime, TimeZone, UserID);
        }
        public static int EditMigration(int MigrationID, string MigrationName, string MigrationProtocol,
           string MaximumConnectionLimit, string PathPrefix, string ExcludeFolderList, string BlackOutTime, string TimeZone, int UserID)
        {
            return DataProvider.EditMigration(MigrationID, MigrationName, MigrationProtocol,
                MaximumConnectionLimit, PathPrefix, ExcludeFolderList, BlackOutTime, TimeZone, UserID);
        }

        public static void DeleteMigration(int migrationId)
        {
            DataProvider.DeleteMigration(migrationId);
        }

        public static DataSet GetEmailMigrationAccounts(int userId, int migrationId, string sortColumn,
           int startRow, int maximumRows)
        {
            return DataProvider.GetEmailMigrationAccounts(userId, migrationId, sortColumn,
                startRow, maximumRows);
        }
        public static DataSet GetMigrationItemlist(int userId, int MigrationAccountId, string sortColumn,
           int startRow, int maximumRows)
        {
            return DataProvider.GetMigrationItemlist(userId, MigrationAccountId, sortColumn,
                startRow, maximumRows);
        }
        
        public static DataSet GetMigrationDestinationServer()
        {
            return DataProvider.GetMigrationDestinationServer();
        }
       
        public static int AddMigrationAccount(string SourceUserName, string SourcePassword, string DestinationEmail, string Priority, bool IsImportant,
           bool IsStarred, bool IsExcludeInbox, string ExcludeFolderList, bool IsAllMails, string SpecificDateRange, bool IsAllFolders, string IncludeFolderList,
           int UserID, int MigrationID,
           string SourceSeverName, string SourceServerSecurity, string SourceServerPort, string SourceServerType, string DestinationPassword, string DestinationServerName, string DestinationPort, string DestinationServerSecurity)
        {
            return DataProvider.AddMigrationAccount(SourceUserName, SourcePassword, DestinationEmail, Priority, IsImportant,
                IsStarred, IsExcludeInbox, ExcludeFolderList, IsAllMails, SpecificDateRange, IsAllFolders, IncludeFolderList,
                UserID, MigrationID,
                SourceSeverName, SourceServerSecurity, SourceServerPort, SourceServerType, DestinationPassword,DestinationServerName,DestinationPort,DestinationServerSecurity);
        }
        public static int EditMigrationAccount(int MigrationAccountId,string SourceUserName, string SourcePassword, string DestinationEmail, string Priority, bool IsImportant,
          bool IsStarred, bool IsExcludeInbox, string ExcludeFolderList, bool IsAllMails, string SpecificDateRange, bool IsAllFolders, string IncludeFolderList,         
          string SourceSeverName, string SourceServerSecurity, string SourceServerPort, string SourceServerType, string DestinationPassword, string DestinationServerName, string DestinationPort, string DestinationServerSecurity)
        {
            return DataProvider.EditMigrationAccount(MigrationAccountId,SourceUserName, SourcePassword, DestinationEmail, Priority, IsImportant,
                IsStarred, IsExcludeInbox, ExcludeFolderList, IsAllMails, SpecificDateRange, IsAllFolders, IncludeFolderList,               
                SourceSeverName, SourceServerSecurity, SourceServerPort, SourceServerType, DestinationPassword, DestinationServerName, DestinationPort, DestinationServerSecurity);
        }
        public static void DeleteMigrationAccount(int migrationAccountID)
        {
            DataProvider.DeleteMigrationAccount(migrationAccountID);
        }

        public static DataSet GetEmailMigration(int userId, int migrationId)
        {
            return DataProvider.GetEmailMigration(userId, migrationId);
        }
        public static DataSet GetEmailMigrationAccount(int MigrationAccountId)
        {
            return DataProvider.GetEmailMigrationAccount(MigrationAccountId);
        }
        public static DataSet GetEmailMigrationErrors(int userId, int migrationId, string sortColumn,
          int startRow, int maximumRows)
        {
            return DataProvider.GetEmailMigrationErrors(userId, migrationId, sortColumn,
                startRow, maximumRows);
        }

        public static void UpdateDefaultserver(string servername, string serverport, string serversecurity)
        {
            DataProvider.UpdateDefaultserver(servername, serverport, serversecurity);
        }
        public static DataSet GetEmailMXRecord()
        {
           return DataProvider.GetEmailMXRecord();
        }
        public static void UpdateDomainMXStatus(int domainId, Boolean mxstatus, string recordtype)
        {
            DataProvider.UpdateDomainMXStatus(domainId, mxstatus, recordtype);
        }
        
       

    }
}
