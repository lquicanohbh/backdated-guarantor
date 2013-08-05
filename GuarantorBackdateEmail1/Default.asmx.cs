using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Data.Odbc;
using System.Data;
using System.Net;
using System.Net.Mail;

/// <summary>
/// Summary description for Default
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.ComponentModel.ToolboxItem(false)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Default : System.Web.Services.WebService
{
    [WebMethod]
    public string GetVersion()
    {
        return "Version 1.0";
    }

    [WebMethod]
    public OptionObject RunScript(OptionObject optionObject, String scriptName)
    {
        OptionObject returnOptionObject = new OptionObject();
        //initialize the option object
        returnOptionObject.EntityID = optionObject.EntityID;
        returnOptionObject.OptionId = optionObject.OptionId;
        returnOptionObject.Facility = optionObject.Facility;
        returnOptionObject.SystemCode = optionObject.SystemCode;

        switch (scriptName)
        {
            case "SendEmailToBilling":
                returnOptionObject = SendEmailToBilling(optionObject);
                break;
            default:
                break;
        }
        return returnOptionObject;
    }
    private OptionObject SendEmailToBilling(OptionObject optionObject)
    {
        OptionObject returnOptionObject = new OptionObject();
        try
        {
            if (checkUserRole(optionObject.OptionUserId))
            {
                Boolean isInfoBackdated = false;
                FieldObject guarantorID = new FieldObject();
                guarantorID.FieldNumber = "680";
                FieldObject guarantorName = new FieldObject();
                guarantorName.FieldNumber = "239";
                FieldObject contractEffectiveDate = new FieldObject();
                contractEffectiveDate.FieldNumber = "1050";
                FieldObject contractExpirationDate = new FieldObject();
                contractExpirationDate.FieldNumber = "1051";
                FieldObject coverageEffectiveDate = new FieldObject();
                coverageEffectiveDate.FieldNumber = "712";
                FieldObject coverageExpirationDate = new FieldObject();
                coverageExpirationDate.FieldNumber = "713";
                Int32 numberOfGuarantors = 0;
                if (optionObject.Forms.Count() > 1)
                {
                    #region backdating
                    if (optionObject.Forms[1].OtherRows != null)
                        numberOfGuarantors = optionObject.Forms[1].OtherRows.Length;
                    Guarantor guarantor = new Guarantor();
                    GuarantorList guarantorList = new GuarantorList(numberOfGuarantors + 1);
                    foreach (var field in optionObject.Forms[1].CurrentRow.Fields)
                    {
                        if (field.FieldNumber.Equals(guarantorID.FieldNumber))
                            guarantorID.FieldValue = field.FieldValue;
                        if (field.FieldNumber.Equals(guarantorName.FieldNumber))
                            guarantorName.FieldValue = field.FieldValue;
                        if (field.FieldNumber.Equals(contractEffectiveDate.FieldNumber))
                            contractEffectiveDate.FieldValue = field.FieldValue;
                        if (field.FieldNumber.Equals(contractExpirationDate.FieldNumber))
                            contractExpirationDate.FieldValue = field.FieldValue;
                        if (field.FieldNumber.Equals(coverageEffectiveDate.FieldNumber))
                            coverageEffectiveDate.FieldValue = field.FieldValue;
                        if (field.FieldNumber.Equals(coverageExpirationDate.FieldNumber))
                            coverageExpirationDate.FieldValue = field.FieldValue;
                    }
                    guarantorList.AddGuarantor(new Guarantor(guarantorID.FieldValue, guarantorName.FieldValue,
                                                contractEffectiveDate.FieldValue, contractExpirationDate.FieldValue,
                                                coverageEffectiveDate.FieldValue, coverageExpirationDate.FieldValue));
                    if (optionObject.Forms[1].OtherRows != null)
                    {
                        foreach (var row in optionObject.Forms[1].OtherRows)
                        {
                            foreach (var field in row.Fields)
                            {
                                if (field.FieldNumber.Equals(guarantorID.FieldNumber))
                                    guarantorID.FieldValue = field.FieldValue;
                                if (field.FieldNumber.Equals(guarantorName.FieldNumber))
                                    guarantorName.FieldValue = field.FieldValue;
                                if (field.FieldNumber.Equals(contractEffectiveDate.FieldNumber))
                                    contractEffectiveDate.FieldValue = field.FieldValue;
                                if (field.FieldNumber.Equals(contractExpirationDate.FieldNumber))
                                    contractExpirationDate.FieldValue = field.FieldValue;
                                if (field.FieldNumber.Equals(coverageEffectiveDate.FieldNumber))
                                    coverageEffectiveDate.FieldValue = field.FieldValue;
                                if (field.FieldNumber.Equals(coverageExpirationDate.FieldNumber))
                                    coverageExpirationDate.FieldValue = field.FieldValue;
                            }
                            guarantorList.AddGuarantor(new Guarantor(guarantorID.FieldValue, guarantorName.FieldValue,
                                                    contractEffectiveDate.FieldValue, contractExpirationDate.FieldValue,
                                                    coverageEffectiveDate.FieldValue, coverageExpirationDate.FieldValue));
                        }
                    }
                    GuarantorList savedGuarantorInformation = getSavedGuarantorInformation(optionObject.EntityID, numberOfGuarantors + 1);
                    foreach (var currentGuarantor in guarantorList.Guarantors)
                    {
                        Guarantor tempGuarantor = savedGuarantorInformation.getGuarantorByID(currentGuarantor.ID);
                        if (tempGuarantor == null)
                        {
                            if (checkNewDates(currentGuarantor))
                            {
                                currentGuarantor.IsBackDated = true;
                                isInfoBackdated = true;
                            }
                        }
                        else if (!isGuarantorInfoTheSame(currentGuarantor, tempGuarantor))
                        {
                            if (isBackDate(currentGuarantor, tempGuarantor))
                            {
                                currentGuarantor.IsBackDated = true;
                                isInfoBackdated = true;
                            }
                        }
                    }
                    if (isInfoBackdated)
                        emailGuarantor(guarantorList, savedGuarantorInformation, optionObject.EntityID, optionObject.OptionStaffId);
                    #endregion
                }
            }
        }
        catch (Exception ex)
        {
        }
        
        returnOptionObject.EntityID = optionObject.EntityID;
        returnOptionObject.OptionId = optionObject.OptionId;
        returnOptionObject.Facility = optionObject.Facility;
        returnOptionObject.SystemCode = optionObject.SystemCode;
        return returnOptionObject;
    }
    private FieldObject[] createNewSingleField(String FieldNumber, String FieldValue)
    {
        var fields = new FieldObject[1];
        var field = new FieldObject();
        field.FieldNumber = FieldNumber;
        field.FieldValue = FieldValue;
        fields[0] = field;
        return fields;
    }
    private Boolean checkNewDates(Guarantor guarantor)
    {
        //if (checkDateFirstTime(guarantor.ContractEffectiveDate))
        //    return true;
        //else if (checkDateFirstTime(guarantor.ContractExpirationDate))
        //    return true;
        if (checkDateFirstTime(guarantor.CoverageEffectiveDate))
            return true;
        else if (checkDateFirstTime(guarantor.CoverageExpirationDate))
            return true;
        else
            return false;
    }
    private void emailGuarantor(GuarantorList newGuarantorInfo, GuarantorList oldGuarantorInfo, String clientID, String staffID)
    {
        MailMessage mailMessage = new MailMessage();
        SmtpClient smtpClient = new SmtpClient();
        smtpClient.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
        smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString());
        mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPFromEmailAddress"].ToString());
        mailMessage.To.Clear();
        mailMessage.To.Add(ConfigurationManager.AppSettings["SMTPToEmailAddress"].ToString());
        mailMessage.To.Add(ConfigurationManager.AppSettings["SMTPCreateSWTicket"].ToString());
        mailMessage.Subject = "Guarantor information was backdated";
        mailMessage.Body = "The following guarantor(s) information for client " + clientID + " was backdated by " + staffID + " :\n";
        foreach (var guarantor in newGuarantorInfo.Guarantors)
        {
            if (guarantor.IsBackDated)
            {
                mailMessage.Body += "New Information " + guarantor.toString() + "\n";
                if (oldGuarantorInfo.getGuarantorByID(guarantor.ID) != null)
                    mailMessage.Body += "Old Information\n " + oldGuarantorInfo.getGuarantorByID(guarantor.ID).toString() + "\n\n";
                else
                    mailMessage.Body += "Old Information\n No Old Information, new guarantor on file.\n\n";
            }
        }
        mailMessage.Body += ConfigurationManager.AppSettings["appSWAnywhere1"].ToString() + " \n" +
                            ConfigurationManager.AppSettings["appSWAnywhere2"].ToString() + " \n" +
                            ConfigurationManager.AppSettings["appSWAnywhere3"].ToString() + " \n";
        smtpClient.Send(mailMessage);
        mailMessage.Dispose();
    }
    private GuarantorList getSavedGuarantorInformation(String PATID, int listSize)
    {
        GuarantorList savedGuarantorListInformation = new GuarantorList(listSize);
        var connectionString = ConfigurationManager.ConnectionStrings["CacheODBC"].ConnectionString;
        var commandText = "SELECT billing_guar_emp_data_no_ep.PATID, " +
                            "billing_guar_emp_data_no_ep.GUARANTOR_ID, " +
                            "billing_guar_emp_data_no_ep.contract_effective_date, " +
                            "billing_guar_emp_data_no_ep.contract_expiration_date, " +
                            "billing_guar_emp_data_no_ep.cov_effective_date, " +
                            "billing_guar_emp_data_no_ep.cov_expiration_date " +
                            "FROM SYSTEM.billing_guar_emp_data_no_ep billing_guar_emp_data_no_ep " +
                            "WHERE billing_guar_emp_data_no_ep.PATID=?";
        using (var connection = new OdbcConnection(connectionString))
        {
            connection.Open();
            using (var dbcommand = new OdbcCommand(commandText, connection))
            {
                var dbparameter1 = new OdbcParameter("PATID", PATID);
                dbcommand.Parameters.Add(dbparameter1);
                using (var reader = dbcommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Guarantor guarantor = new Guarantor();
                        guarantor.ID = reader["GUARANTOR_ID"].ToString();
                        guarantor.ContractEffectiveDate = formatDateToString(reader["contract_effective_date"].ToString());
                        guarantor.ContractExpirationDate = formatDateToString(reader["contract_expiration_date"].ToString());
                        guarantor.CoverageEffectiveDate = formatDateToString(reader["cov_effective_date"].ToString());
                        guarantor.CoverageExpirationDate = formatDateToString(reader["cov_expiration_date"].ToString());
                        savedGuarantorListInformation.AddGuarantor(guarantor);
                    }
                }
            }
            connection.Close();
        }
        return savedGuarantorListInformation;
    }
    private Boolean isBackDate(Guarantor currentGuarantor, Guarantor savedGuarantor)
    {
        Boolean backDated = false;
        //if (!currentGuarantor.ContractEffectiveDate.Equals(savedGuarantor.ContractEffectiveDate))
        //    backDated = checkDate(currentGuarantor.ContractEffectiveDate);
        //if (!currentGuarantor.ContractExpirationDate.Equals(savedGuarantor.ContractExpirationDate))
        //    backDated = checkDate(currentGuarantor.ContractExpirationDate);
        if (!currentGuarantor.CoverageEffectiveDate.Equals(savedGuarantor.CoverageEffectiveDate))
            backDated = checkDate(currentGuarantor.CoverageEffectiveDate);
        if (!currentGuarantor.CoverageExpirationDate.Equals(savedGuarantor.CoverageExpirationDate))
            backDated = checkDate(currentGuarantor.CoverageExpirationDate);
        return backDated;
    }
    private Boolean checkDateFirstTime(String date)
    {
        int num = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfDaysForBackdate"].ToString()) * (-1);
        Boolean backDated = false;
        if (!date.Equals(String.Empty))
        {
            return DateTime.Today.AddDays(num) >= formatDateToDateTime(date);
        }
        return backDated;
    }
    private Boolean checkDate(String date)
    {
        int num = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfDaysForBackdate"].ToString()) * (-1);
        if (date.Equals(String.Empty))
        {
            return true;
        }
        else
        {
            return DateTime.Today.AddDays(num) >= formatDateToDateTime(date);
        }
    }
    private Boolean isGuarantorInfoTheSame(Guarantor guarantor1, Guarantor guarantor2)
    {
        Boolean isTheSame;
        if (guarantor1.ID.Equals(guarantor2.ID) &&
            //guarantor1.ContractEffectiveDate.Equals(guarantor2.ContractEffectiveDate) &&
            //guarantor1.ContractExpirationDate.Equals(guarantor2.ContractExpirationDate) &&
            guarantor1.CoverageEffectiveDate.Equals(guarantor2.CoverageEffectiveDate) &&
            guarantor1.CoverageExpirationDate.Equals(guarantor2.CoverageExpirationDate))
            isTheSame = true;
        else
            isTheSame = false;
        return isTheSame;
    }
    private DateTime formatDateToDateTime(String date)
    {
        String dayS, monthS, yearS;
        int dateOnly = date.IndexOf(" ");
        if (dateOnly != -1)
            date = date.Substring(0, dateOnly);
        int day;
        int month;
        month = date.IndexOf("/");
        monthS = date.Substring(0, month);
        date = date.Substring(month + 1, date.Length - month - 1);
        day = date.IndexOf("/");
        dayS = date.Substring(0, day);
        date = date.Substring(day + 1, date.Length - day - 1);
        yearS = date;
        date.ToString();
        DateTime dateTime = new DateTime(Convert.ToInt32(yearS), Convert.ToInt32(monthS), Convert.ToInt32(dayS));
        return dateTime;
    }
    private String formatDateToString(String date)
    {
        if (!date.Equals(String.Empty))
        {
            String dayS, monthS, yearS;
            int dateOnly = date.IndexOf(" ");
            if (dateOnly != -1)
                date = date.Substring(0, dateOnly);
            int day;
            int month;
            month = date.IndexOf("/");
            monthS = date.Substring(0, month);
            date = date.Substring(month + 1, date.Length - month - 1);
            day = date.IndexOf("/");
            dayS = date.Substring(0, day);
            date = date.Substring(day + 1, date.Length - day - 1);
            yearS = date;
            date.ToString();
            DateTime dateTime = new DateTime(Convert.ToInt32(yearS), Convert.ToInt32(monthS), Convert.ToInt32(dayS));
            return dateTime.ToString("MM/dd/yyyy");
        }
        else
            return "";
    }
    private Boolean checkUserRole(String UserID)
    {
        Boolean applyScriptLinkRules = true;
        var connectionString1 = ConfigurationManager.ConnectionStrings["CacheODBC"].ConnectionString;
        var commandText1 = "SELECT RADplus_users.USERID, " +
                            "RADplus_users.USERROLE " +
                            "FROM SYSTEM.RADplus_users RADplus_users " +
                            "WHERE RADplus_users.USERID=? " +
                            "AND (RADplus_users.USERROLE LIKE '%&ARBilling&%' " +
                            "OR RADplus_users.USERROLE LIKE '%&ITAll&%') ";
        using (var connection1 = new OdbcConnection(connectionString1))
        {
            connection1.Open();
            using (var dbcommand = new OdbcCommand(commandText1, connection1))
            {
                var parameter1 = new OdbcParameter("USERID", UserID);
                dbcommand.Parameters.Add(parameter1);
                using (var reader = dbcommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        applyScriptLinkRules = false;
                    }
                }
            }
            connection1.Close();
        }
        return applyScriptLinkRules;
    }
}