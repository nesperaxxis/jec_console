using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SBOHelper.Class;
using SBOHelper.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TAIDII_SAP_CONSOLE.Class
{
    public class ConstantClass
    {
        //--------------------------------------------SAP B1 SETTINGS---------------------------------------------------------------------//
        public static string SBOServer = System.Configuration.ConfigurationManager.AppSettings["SBOServer"].ToString();
        public static string ServerUN = System.Configuration.ConfigurationManager.AppSettings["ServerUN"].ToString();
        public static string ServerPW = System.Configuration.ConfigurationManager.AppSettings["ServerPW"].ToString();
        public static string ServerVersion = System.Configuration.ConfigurationManager.AppSettings["ServerVersion"].ToString();
        public static string SAPUser = System.Configuration.ConfigurationManager.AppSettings["SAPUsername"].ToString();
        public static string SAPPassword = System.Configuration.ConfigurationManager.AppSettings["SAPPassword"].ToString();
        public static string AppSetting = System.Configuration.ConfigurationManager.AppSettings["AppStart"].ToString();
        //--------------------------------------------SAP B1 SETTINGS---------------------------------------------------------------------//

        //--------------------------------------------TAIDII SETTING----------------------------------------------------------------------//
        public static string APIBaseURL = "";
        public static string APIKey = "";
        public static string APIClient = "";
        public static string APIFormat = System.Configuration.ConfigurationManager.AppSettings["APIFormat"].ToString();
        public static string APILastTimeStamp = System.Configuration.ConfigurationManager.AppSettings["APILastTimeStamp"].ToString();
        public static string APILastDate = System.Configuration.ConfigurationManager.AppSettings["APILastDate"].ToString();
        public static string ERRORPATH = System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"].ToString();
        //--------------------------------------------TAIDII SETTING----------------------------------------------------------------------//


    }
}
