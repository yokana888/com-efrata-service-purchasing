using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal
{
    public class SPBDtoItem
    {
        public SPBDtoItem(GarmentInternNote element, GarmentInternNoteItem item)
        {
            Date = element.INDate;
            Remark = element.Remark;
            Amount = item.TotalAmount;
        }

        public SPBDtoItem(UnitPaymentOrder element, UnitPaymentOrderItem item)
        {
            Date = element.Date;
            Remark = element.Remark;
            Amount = item.Details.Sum(detail => detail.PricePerDealUnit);
            UseVat = element.UseVat;
            UseIncomeTax = element.UseIncomeTax;
            IncomeTax = new IncomeTaxDto(element.IncomeTaxId, element.IncomeTaxName, element.IncomeTaxRate);
            IncomeTaxBy = element.IncomeTaxBy;
        }

        public DateTimeOffset Date { get; private set; }
        public string Remark { get; private set; }
        public double Amount { get; private set; }
        public bool UseVat { get; private set; }
        public bool UseIncomeTax { get; private set; }
        public IncomeTaxDto IncomeTax { get; private set; }
        public string IncomeTaxBy { get; private set; }
    }
}
