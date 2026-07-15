using Constants;
using UnityEngine;

public class BillUI : MonoBehaviour
{
    [SerializeField]
    private GameObject Parent;

    public TMPro.TextMeshProUGUI EO ;     //"Equipment Owned"
    public TMPro.TextMeshProUGUI CpE;     //"Usage Cost per Equipment"
    public TMPro.TextMeshProUGUI CR ;     //"Customer Recived"
    public TMPro.TextMeshProUGUI IE ;     //"Income Earned"
    public TMPro.TextMeshProUGUI ITp;     //"Income Tax Percentage"
    public TMPro.TextMeshProUGUI   D;     //"Difficulty"
    public TMPro.TextMeshProUGUI DB ;     //"Difficulty Bonus"
    public TMPro.TextMeshProUGUI RGT;     //"Random govt. Tax"
    public TMPro.TextMeshProUGUI EF ;     //"Equipment Fee"
    public TMPro.TextMeshProUGUI TF ;     //"Tax Fee"
    public TMPro.TextMeshProUGUI DB1;     //"Difficulty Bonus"
    public TMPro.TextMeshProUGUI DD ;     //"Discount"
    public TMPro.TextMeshProUGUI RGT1;     //"Random govt. Tax"
    public TMPro.TextMeshProUGUI FA;      //"Final Amount "

    public void SetBill(Bill bill)
{
        var data=bill.GetBillData();    
        EO .text = string.Format(Constant.EO , data.EquipmentOwne);
        CpE.text = string.Format(Constant.CpE, data.CostPerEquipment);
        CR .text = string.Format(Constant.CR , data.TotalCustomerAttended);
        IE .text = string.Format(Constant.IE , data.TotalIncome);
        ITp.text = string.Format(Constant.ITp, data.IncomeTax);
          D.text = string.Format(Constant.  D, data.Difficulty);
        DB .text = string.Format(Constant.DB , data.DifficultyBonus);
        RGT.text = string.Format(Constant.RGT, data.RandomGovtTax);
        EF .text = string.Format(Constant.EF , bill.GetEquipmentsurgeFee());
        TF .text = string.Format(Constant.TF , bill.GetTaxsurgeFee());
        DD .text = string.Format(Constant.DD , bill.GetRandomDiscountAfterAVideo());
        RGT1.text = string.Format(Constant.RGT, data.RandomGovtTax);
        FA .text = string.Format(Constant.FA,  bill.GetTotal());
        Parent.SetActive(true);
    }

    public void CloseBilling()
    {
        Parent.SetActive(false);
    }

}
