using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Guarantor
/// </summary>
public class Guarantor
{
    private Boolean _isBackDated;

    public Boolean IsBackDated
    {
        get { return _isBackDated; }
        set { _isBackDated = value; }
    }

    private String _ID;

    public String ID
    {
        get { return _ID; }
        set { _ID = value; }
    }
    private String _Name;

    public String Name
    {
        get { return _Name; }
        set { _Name = value; }
    }
    private String _ContractEffectiveDate;

    public String ContractEffectiveDate
    {
        get { return _ContractEffectiveDate; }
        set { _ContractEffectiveDate = value; }
    }
    private String _ContractExpirationDate;

    public String ContractExpirationDate
    {
        get { return _ContractExpirationDate; }
        set { _ContractExpirationDate = value; }
    }
    private String _CoverageEffectiveDate;

    public String CoverageEffectiveDate
    {
        get { return _CoverageEffectiveDate; }
        set { _CoverageEffectiveDate = value; }
    }
    private String _CoverageExpirationDate;

    public String CoverageExpirationDate
    {
        get { return _CoverageExpirationDate; }
        set { _CoverageExpirationDate = value; }
    }
    public Guarantor()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public Guarantor(String ID, String Name, String ContractEffectiveDate, String ContractExpirationDate, String CoverageEffectiveDate, String CoverageExpirationDate)
    {
        this.ID = ID;
        this.Name = Name;
        this.ContractEffectiveDate = ContractEffectiveDate;
        this.ContractExpirationDate = ContractExpirationDate;
        this.CoverageEffectiveDate = CoverageEffectiveDate;
        this.CoverageExpirationDate = CoverageExpirationDate;
        this.IsBackDated = false;
    }
    /// <summary>
    /// Custom toString() method that displays (this) guarantor and all its properties
    /// </summary>
    public String toString()
    {
        return "Guarantor ID: " + this.ID + "\n" +
                "Contract Effective Date: " + this.ContractEffectiveDate + "\n" +
                "Contract Expiration Date: " + this.ContractExpirationDate + "\n" +
                "Coverage Effective Date: " + this.CoverageEffectiveDate + "\n" +
                "Coverage Expiration Date: " + this.CoverageExpirationDate + "\n";
    }
}