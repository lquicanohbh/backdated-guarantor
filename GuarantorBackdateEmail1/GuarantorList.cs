using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GuarantorList
/// </summary>
public class GuarantorList
{
    private Guarantor[] _Guarantors;

    public Guarantor[] Guarantors
    {
        get { return _Guarantors; }
        set { _Guarantors = value; }
    }
    private int _GuarantorPointer;

    public int GuarantorPointer
    {
        get { return _GuarantorPointer; }
        set { _GuarantorPointer = value; }
    }
    public GuarantorList(int size)
    {
        this.Guarantors = new Guarantor[size];
        this._GuarantorPointer = 0;
        //
        // TODO: Add constructor logic here
        //
    }
    public void AddGuarantor(Guarantor guarantor)
    {
        this.Guarantors[_GuarantorPointer] = guarantor;
        _GuarantorPointer++;
    }
    public Guarantor getGuarantor(int position)
    {
        return this.Guarantors[position];
    }
    public Guarantor getGuarantorByID(String ID)
    {
        Guarantor returnGuarantor = null;
        for (int i = 0; i < this._GuarantorPointer; i++)
        {
            if (this.Guarantors[i].ID.Equals(ID))
                returnGuarantor = this.Guarantors[i];
        }
        return returnGuarantor;
    }

    /// <summary>
    /// Custom toString() method that displays every guarantor object in the list nicely formatted.
    /// </summary>
    public String toString()
    {
        String tempString = "";
        for (int i = 0; i < this._GuarantorPointer; i++)
        {
            tempString += "Guarantor ID: " + this.Guarantors[i].ID + "\n";
            //tempString += "Guarantor Name: "+this.Guarantors[i].Name + "\n";
            tempString += "Contract Effective Date: " + this.Guarantors[i].ContractEffectiveDate + "\n";
            tempString += "Contract Expiration Date: " + this.Guarantors[i].ContractExpirationDate + "\n";
            tempString += "Coverage Effective Date: " + this.Guarantors[i].CoverageEffectiveDate + "\n";
            tempString += "Coverage Expiration Date: " + this.Guarantors[i].CoverageExpirationDate + "\n";
            tempString += "Back Dated: " + this.Guarantors[i].IsBackDated + "\n\n";
        }
        return tempString;
    }
}