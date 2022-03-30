using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TAIDII_SAP_CONSOLE.Models;
using System.Globalization;

using SBOHelper.Class;
using SBOHelper.Models;

namespace TAIDII_SAP_CONSOLE.Class
{
    public class clsCallAPI
    {
        public List<API_BusinessPartners> listBusinessParters { get; set; }
        public List<API_Invoice> listInvoice { get; set; }
        public List<API_CreditNote> listCreditNote { get; set; }
        public List<API_Receipt> listReceipt { get; set; }
        public List<API_CreditRefund> listCreditRefund { get; set; }
        public List<API_FinanceItem> listItem { get; set; }
        public SAPResponse responseResult { get; set; }
        public List<SBOHelper.Models.ResponseResultSuccess> listResponseResultSuccess { get; set; }
        public List<SBOHelper.Models.ResponseResultFailed> listResponseResultFailed { get; set; }
        public string _ErrorMessage { get; set; }

        public async Task<bool> GetAPIList(string APIMethod, string IntegrationPoint)
        {
            //Declarations
            string APIBaseURL = ConstantClass.APIBaseURL;
            string MethodURL = string.Empty;
            string jsonText = string.Empty;

            string retValue = string.Empty;
            _ErrorMessage = string.Empty;
            bool retBool = false;

            try
            {
                //Start initializing connection to WebAPI.
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                HttpResponseMessage iHttpResponseMessage;
                HttpClient iHttpClient = new HttpClient();
                iHttpClient.DefaultRequestHeaders.Clear();
                
                //Set Base Address for API Call
                iHttpClient.BaseAddress = new Uri(APIBaseURL);
                iHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //////API Assigning of method
                switch (IntegrationPoint)
                {
                    case "Student":
                        MethodURL = APIMethod + ConstantClass.APIKey + "&" + ConstantClass.APIClient + "&" + ConstantClass.APILastTimeStamp + ConstantClass.APILastDate;
                        break;
                    case "Invoice":
                        MethodURL = APIMethod + ConstantClass.APIKey + "&" + ConstantClass.APIClient + "&" + ConstantClass.APILastTimeStamp + ConstantClass.APILastDate;
                        break;
                    case "CreditNote":
                        MethodURL = APIMethod + ConstantClass.APIKey + "&" + ConstantClass.APIClient + "&" + ConstantClass.APILastTimeStamp + ConstantClass.APILastDate;
                        break;
                    case "Receipt":
                        MethodURL = APIMethod + ConstantClass.APIKey + "&" + ConstantClass.APIClient + "&" + ConstantClass.APILastTimeStamp + ConstantClass.APILastDate;
                        break;
                    case "CreditRefund":
                        MethodURL = APIMethod + ConstantClass.APIKey + "&" + ConstantClass.APIClient + "&" + ConstantClass.APILastTimeStamp + ConstantClass.APILastDate;
                        break;
                }

                //HttpRequestMessage iHttpRequestMessage = new HttpRequestMessage(HttpMethod.Get, iHttpClient.BaseAddress);
                iHttpResponseMessage = await iHttpClient.GetAsync(MethodURL);

                iHttpClient.Dispose();

                if (iHttpResponseMessage.IsSuccessStatusCode)
                {
                    retValue = iHttpResponseMessage.Content.ReadAsStringAsync().Result;

                    switch (IntegrationPoint)
                    {
                        case "Student":
                            List<API_BusinessPartners> lstBusinessPartners = Newtonsoft.Json.JsonConvert.DeserializeObject<List<API_BusinessPartners>>(retValue);
                            listBusinessParters = lstBusinessPartners;
                            break;
                        case "Invoice":
                            List<API_Invoice> lstInvoice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<API_Invoice>>(retValue);
                            listInvoice = lstInvoice;
                            break;
                        case "CreditNote":
                            List<API_CreditNote> lstCreditNote = Newtonsoft.Json.JsonConvert.DeserializeObject<List<API_CreditNote>>(retValue);
                            listCreditNote = lstCreditNote;
                            break;
                        case "Receipt":
                            List<API_Receipt> lstReceipt = Newtonsoft.Json.JsonConvert.DeserializeObject<List<API_Receipt>>(retValue);
                            listReceipt = lstReceipt;
                            break;
                        case "CreditRefund":
                            List<API_CreditRefund> lstCreditRefund = Newtonsoft.Json.JsonConvert.DeserializeObject<List<API_CreditRefund>>(retValue);
                            listCreditRefund = lstCreditRefund;
                            break;
                    }
                    retBool = true;
                }
                else
                {
                    retValue = iHttpResponseMessage.Content.ReadAsStringAsync().Result;

                    ////Error message response
                    ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(retValue);
                    SAPResponse result = new SAPResponse();
                    result.RecordStatus = "false";
                    result.Message = err.Message;
                    responseResult = result;
                    _ErrorMessage += "TAIDII API: Error Connection. Please check your connection." + Environment.NewLine;
                    retBool = false;
                }

                iHttpResponseMessage.Dispose();

                return await Task.FromResult(retBool);
            }
            catch (Exception ex)
            {
                _ErrorMessage += "TAIDII API Connect (EX): " + ex.Message + Environment.NewLine;
                return await Task.FromResult(false);
            }
        }

        public object iif(bool expression, object truePart, object falsePart)
        { return expression ? truePart : falsePart; }
    }
}
