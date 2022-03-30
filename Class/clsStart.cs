using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBOHelper.Class;
using SBOHelper.Models;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Sap.Data.Hana;

namespace TAIDII_SAP_CONSOLE.Class
{
    public class clsStart
    {
        public bool GetStarted()
        {
            bool retBool = true;
            string FileName = string.Empty;
            string GlobalErrorLogPath = ConstantClass.ERRORPATH + "\\";

            string ErrorMessage = string.Empty;
            string ConnString = string.Empty;
            string SelectQuery = string.Empty;
            HanaConnection _HanaConnection = null;
            SqlConnection _SqlConnection = null;
            DataTable _DataTable = null;

            try
            {
                Console.WriteLine("API Connection is now Initializing!");
                //Call initialized constants      
                InitializedConstants();
                if (ConstantClass.AppSetting == "0")
                {
                    try
                    {
                        if (SBOConstantClass.ServerVersion == "dst_HANADB")
                        {
                            //Connection String for HANA
                            ConnString = "Server=" + SBOConstantClass.SBOServer + ";initial catalog=TAIDII_SAP;UserID=" + SBOConstantClass.ServerUN + ";Password=" + SBOConstantClass.ServerPW;
                            _HanaConnection = new HanaConnection(ConnString);
                            _HanaConnection.Open();

                            SelectQuery = "select *,TO_VARCHAR (TO_DATE(CURRENT_DATE),'YYYY-MM-DD') \"current_date\" from " + " \"TAIDII_SAP\"" + "." + "\"axxis_tb_IntegrationSetup\"";

                            HanaCommand _HanaCommand = new HanaCommand(SelectQuery, _HanaConnection);
                            HanaDataAdapter _HanaDataAdapter = new HanaDataAdapter();
                            _HanaDataAdapter.SelectCommand = _HanaCommand;
                            DataSet _DataSet = new DataSet();
                            _HanaDataAdapter.Fill(_DataSet);
                            _DataTable = _DataSet.Tables[0];
                        }
                        else
                        {
                            //Connection String for MSSQL
                            ConnString = "Data Source=" + SBOConstantClass.SBOServer + ";Initial Catalog=TAIDII_SAP; User ID=" + SBOConstantClass.ServerUN + ";Password=" + SBOConstantClass.ServerPW + ";Integrated Security=false;";
                            _SqlConnection = new SqlConnection(ConnString);
                            _SqlConnection.Open();

                            SelectQuery = "select *,CONVERT(VARCHAR, GETDATE(), 23) \"current_date\" from " + " \"TAIDII_SAP\"" + ".." + "\"axxis_tb_IntegrationSetup\"";

                            SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(SelectQuery, _SqlConnection);
                            DataSet _DataSet = new DataSet();
                            _SqlDataAdapter.Fill(_DataSet);
                            _DataTable = _DataSet.Tables[0];
                        }

                        if (_DataTable.Rows.Count > 0)
                        {
                            for (int i = 0; i <= _DataTable.Rows.Count - 1; i++)
                            {
                                SBOConstantClass.Database = _DataTable.Rows[i]["companyDB"].ToString();
                                string ErrorLogPath = ConstantClass.ERRORPATH + "\\" + SBOConstantClass.Database + "\\";

                                if (!Directory.Exists(ErrorLogPath))
                                {
                                    Directory.CreateDirectory(ErrorLogPath);
                                }

                                ConstantClass.APIBaseURL = _DataTable.Rows[i]["base_url"].ToString();
                                ConstantClass.APIKey = "?api_key=" + _DataTable.Rows[i]["api_key"].ToString();
                                ConstantClass.APIClient = "client=" + _DataTable.Rows[i]["clientName"].ToString();

                                if (ConstantClass.APILastDate == "")
                                {
                                    ConstantClass.APILastDate = _DataTable.Rows[i]["current_date"].ToString() + " 00:00:00";
                                }

                                Console.WriteLine("Company Database:" + SBOConstantClass.Database + ", Last Time Stamp:" + ConstantClass.APILastDate + " to fetch in TAIDII API.");
                                try
                                {
                                    //**** Customer List ****////
                                    TAIDII_SAP_CONSOLE.Class.clsCallAPI clsCallAPI_BP = new Class.clsCallAPI();
                                    clsCallAPI_BP.listBusinessParters = null;
                                    //Assign data of information
                                    Console.WriteLine("Getting List of Students");
                                    if (!clsCallAPI_BP.GetAPIList("student/list/", "Student").Result)
                                    {
                                        //Write to text file
                                        ErrorMessage += "List of Students: " + clsCallAPI_BP._ErrorMessage + Environment.NewLine;
                                        Console.WriteLine(clsCallAPI_BP._ErrorMessage);
                                        WriteLog(ErrorLogPath + "Student_", clsCallAPI_BP._ErrorMessage);
                                        retBool = false;
                                    }
                                    else
                                    {
                                        if (clsCallAPI_BP.listBusinessParters.Count != 0)
                                        {
                                            SBOHelper.Class.SBOClass clsSBOClass = new SBOHelper.Class.SBOClass();
                                            clsSBOClass.SBOPostBusinessPartners(clsCallAPI_BP.listBusinessParters, ConstantClass.APILastDate);
                                        }
                                        else
                                            Console.WriteLine("No records in Student List!");

                                    }
                                    ////**** Customer List ****////
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ErrorLogPath + "Student_", "Getting List of Students Method - " + ex.ToString());
                                }

                                try
                                {
                                    ////**** Invoice List ****////
                                    TAIDII_SAP_CONSOLE.Class.clsCallAPI clsCallAPI_INVOICE = new Class.clsCallAPI();
                                    clsCallAPI_INVOICE.listInvoice = null;
                                    //Assign data of information
                                    Console.WriteLine("Getting List of Invoices");
                                    if (!clsCallAPI_INVOICE.GetAPIList("invoice/list/", "Invoice").Result)
                                    {
                                        //Write to text file
                                        ErrorMessage += "List of Invoices: " + clsCallAPI_INVOICE._ErrorMessage + Environment.NewLine;
                                        Console.WriteLine(clsCallAPI_INVOICE._ErrorMessage);
                                        WriteLog(ErrorLogPath + "Invoices_", clsCallAPI_INVOICE._ErrorMessage);
                                        retBool = false;
                                    }
                                    else
                                    {
                                        if (clsCallAPI_INVOICE.listInvoice.Count != 0)
                                        {
                                            SBOHelper.Class.SBOClass clsSBOClass = new SBOHelper.Class.SBOClass();
                                            clsSBOClass.SBOPostInvoice(clsCallAPI_INVOICE.listInvoice, ConstantClass.APILastDate);
                                        }
                                        else
                                            Console.WriteLine("No records in Invoice List!");
                                    }
                                    ////**** Invoice List ****////
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ErrorLogPath + "Invoices_", "Getting List of Invoices Method - " + ex.ToString());
                                }

                                try
                                {
                                    ////**** Credit Note List ****////
                                    TAIDII_SAP_CONSOLE.Class.clsCallAPI clsCallAPI_CREDITNOTE = new Class.clsCallAPI();
                                    clsCallAPI_CREDITNOTE.listCreditNote = null;
                                    //Assign data of information
                                    Console.WriteLine("Getting List of Credit Notes");
                                    if (!clsCallAPI_CREDITNOTE.GetAPIList("creditnotes/list/", "CreditNote").Result)
                                    {
                                        //Write to text file
                                        ErrorMessage += "List of Credit Notes: " + clsCallAPI_CREDITNOTE._ErrorMessage + Environment.NewLine;
                                        Console.WriteLine(clsCallAPI_CREDITNOTE._ErrorMessage);
                                        WriteLog(ErrorLogPath + "CreditNotes_", clsCallAPI_CREDITNOTE._ErrorMessage);
                                        retBool = false;
                                    }
                                    else
                                    {
                                        if (clsCallAPI_CREDITNOTE.listCreditNote.Count != 0)
                                        {
                                            SBOHelper.Class.SBOClass clsSBOClass = new SBOHelper.Class.SBOClass();
                                            clsSBOClass.SBOPostCreditNote(clsCallAPI_CREDITNOTE.listCreditNote, ConstantClass.APILastDate);
                                        }
                                        else
                                            Console.WriteLine("No records in Credit Note List!");
                                    }
                                    ////**** Credit Note List ****////
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ErrorLogPath + "CreditNotes_", "Getting List of Credit Notes Method - " + ex.ToString());
                                }

                                try
                                {
                                    //**** Receipt List ****////
                                    TAIDII_SAP_CONSOLE.Class.clsCallAPI clsCallAPI_RECEIPT = new Class.clsCallAPI();
                                    clsCallAPI_RECEIPT.listReceipt = null;
                                    //Assign data of information
                                    Console.WriteLine("Getting List of Receipts");
                                    if (!clsCallAPI_RECEIPT.GetAPIList("receipt/list/", "Receipt").Result)
                                    {
                                        //Write to text file
                                        ErrorMessage += "List of Receipts: " + clsCallAPI_RECEIPT._ErrorMessage + Environment.NewLine;
                                        Console.WriteLine(clsCallAPI_RECEIPT._ErrorMessage);
                                        WriteLog(ErrorLogPath + "Receipts_", clsCallAPI_RECEIPT._ErrorMessage);
                                        retBool = false;
                                    }
                                    else
                                    {
                                        if (clsCallAPI_RECEIPT.listReceipt.Count != 0)
                                        {
                                            SBOHelper.Class.SBOClass clsSBOClass = new SBOHelper.Class.SBOClass();
                                            clsSBOClass.SBOPostReceipt(clsCallAPI_RECEIPT.listReceipt, ConstantClass.APILastDate);
                                        }
                                        else
                                            Console.WriteLine("No records in Payment List!");
                                    }
                                    ////**** Receipt List ****////
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ErrorLogPath + "Receipts_", "Getting List of Receipts Method - " + ex.ToString());
                                }

                                try
                                {
                                    ////**** Credit Refund List ****////
                                    TAIDII_SAP_CONSOLE.Class.clsCallAPI clsCallAPI_CREDITREFUND = new Class.clsCallAPI();
                                    clsCallAPI_CREDITREFUND.listCreditRefund = null;
                                    //Assign data of information
                                    Console.WriteLine("Getting List of Credit Refunds");
                                    if (!clsCallAPI_CREDITREFUND.GetAPIList("refund/list/", "CreditRefund").Result)
                                    {
                                        //Write to text file
                                        ErrorMessage += "List of Credit Refunds: " + clsCallAPI_CREDITREFUND._ErrorMessage + Environment.NewLine;
                                        Console.WriteLine(clsCallAPI_CREDITREFUND._ErrorMessage);
                                        WriteLog(ErrorLogPath + "CreditRefunds_", clsCallAPI_CREDITREFUND._ErrorMessage);
                                        retBool = false;
                                    }
                                    else
                                    {
                                        if (clsCallAPI_CREDITREFUND.listCreditRefund.Count != 0)
                                        {
                                            SBOHelper.Class.SBOClass clsSBOClass = new SBOHelper.Class.SBOClass();
                                            clsSBOClass.SBOPostCreditRefund(clsCallAPI_CREDITREFUND.listCreditRefund, ConstantClass.APILastDate);
                                        }
                                        else
                                            Console.WriteLine("No records in Credit Refund List!");
                                    }
                                    ////**** Credit Refund List ****////
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ErrorLogPath + "CreditRefunds_", "Getting List of Credit Refunds Method - " + ex.ToString());
                                }

                                ////**** Post Item Master Data ****////
                                try
                                {
                                    SBOHelper.Class.SBOClass clsSBOClassItem = new SBOHelper.Class.SBOClass();
                                    if (clsSBOClassItem.ItemMasterData(ConstantClass.APILastDate) == false)
                                    {
                                        if (clsSBOClassItem.ItemMasterModel.Count == 0)
                                        {
                                            Console.WriteLine("No records in Product List!");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ErrorLogPath + "Products_", "Getting List of Products Method - " + ex.ToString());
                                }
                                ////**** Post Item Master Data ****////
                            }
                        }

                        if (SBOConstantClass.ServerVersion == "dst_HANADB")
                        { _HanaConnection.Close(); }
                        else
                        { }
                    }
                    catch (Exception ex)
                    {
                        string Message = ex.Message;
                        retBool = false;

                        ErrorMessage += "Exception Error : " + ex.Message + Environment.NewLine;
                        retBool = false;
                        //write to text;
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(ErrorMessage))
                        {
                            WriteLog(GlobalErrorLogPath, ErrorMessage);
                        }
                    }
                }
                else
                {
                    try
                    {
                        ////**** Instantiate Order ****////
                        Console.WriteLine("SAP Config is starting!");
                        Console.WriteLine("SAP Connection is now Initializing!");

                        SBOClass sbo = new SBOClass();
                        Console.WriteLine("SAP Config is starting!");
                        if (!string.IsNullOrEmpty(sbo.LastErrorMessage))
                        {
                            ErrorMessage = sbo.LastErrorMessage;
                            retBool = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog(GlobalErrorLogPath, ex.ToString());
                    }
                }
                return retBool;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void WriteLog(string filename, string msg)
        {
            try
            {
                string fname = filename + DateTime.Now.ToString("dd.MM.yyyy HHmm") + ".txt";
                using (StreamWriter writer = new StreamWriter(fname, true))
                {
                    if (msg == "----------------------------------------------------------------")
                    {
                        writer.WriteLine(msg);
                    }
                    else
                    {
                        writer.WriteLine(msg + " || timestamp: " + DateTime.Now.ToString("HH:mm:ss"));
                    }

                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void InitializedConstants()
        {
            SBOConstantClass.SBOServer = ConstantClass.SBOServer;
            SBOConstantClass.ServerVersion = ConstantClass.ServerVersion;
            SBOConstantClass.SAPUser = ConstantClass.SAPUser;
            SBOConstantClass.SAPPassword = ConstantClass.SAPPassword;
            SBOConstantClass.ServerUN = ConstantClass.ServerUN;
            SBOConstantClass.ServerPW = ConstantClass.ServerPW;
        }
    }
}
