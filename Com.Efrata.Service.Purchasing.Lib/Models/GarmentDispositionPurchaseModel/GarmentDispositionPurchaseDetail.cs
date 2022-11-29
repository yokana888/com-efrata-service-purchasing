using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Models.GarmentDispositionPurchaseModel
{
    public class GarmentDispositionPurchaseDetail : StandardEntity
    {
        public int ROId { get; set; }
        public string RONo { get; set; }
        public string IPONo { get; set; }
        public int IPOId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public double QTYOrder { get; set; }
        public string QTYUnit { get; set; }
        public double QTYRemains { get; set; }
        public double PricePerQTY { get; set; }
        public double PriceTotal { get; set; }
        public double QTYPaid { get; set; }
        public double PaidPrice { get; set; }
        public double PercentageOverQTY { get; set; }
        public int EPO_POId { get; set; }
        public double DispositionAmountPaid { get; set; }
        public double DispositionAmountCreated { get; set; }
        public double DispositionQuantityCreated { get; set; }
        public double DispositionQuantityPaid { get; set; }

        public int GarmentDispositionPurchaseItemId { get; set; }
        [ForeignKey("GarmentDispositionPurchaseItemId")]
        public GarmentDispositionPurchaseItem GarmentDispositionPurchaseItem { get; set; }
        /// <summary>
        /// Only use for Update Garment Disposition
        /// </summary>
        /// <param name="modelReplace"></param>
        public void Update(GarmentDispositionPurchaseDetail modelReplace)
        {
            ROId = modelReplace.ROId;
            RONo = modelReplace.RONo;
            IPONo = modelReplace.IPONo;
            IPOId = modelReplace.IPOId;
            ProductId = modelReplace.ProductId;
            ProductName = modelReplace.ProductName;
            UnitId = modelReplace.UnitId;
            UnitName = modelReplace.UnitName;
            UnitCode = modelReplace.UnitCode;
            QTYOrder = modelReplace.QTYOrder;
            QTYUnit = modelReplace.QTYUnit;
            QTYRemains = modelReplace.QTYRemains;
            PricePerQTY = modelReplace.PricePerQTY;
            PriceTotal = modelReplace.PriceTotal;
            QTYPaid = modelReplace.QTYPaid;
            PaidPrice = modelReplace.PaidPrice;
            PercentageOverQTY = modelReplace.PercentageOverQTY;
            EPO_POId = modelReplace.EPO_POId;
            DispositionAmountPaid = modelReplace.DispositionAmountPaid;
            DispositionAmountCreated = modelReplace.DispositionAmountCreated;
            DispositionQuantityCreated = modelReplace.DispositionQuantityCreated;
            DispositionQuantityPaid = modelReplace.DispositionQuantityPaid;
        }

        public void SetAudit(GarmentDispositionPurchase modelReplace)
        {
            Active = modelReplace.Active;
            CreatedUtc = modelReplace.CreatedUtc;
            CreatedBy = modelReplace.CreatedBy;
            CreatedAgent = modelReplace.CreatedAgent;
            LastModifiedUtc = modelReplace.LastModifiedUtc;
            LastModifiedBy = modelReplace.LastModifiedBy;
            LastModifiedAgent = modelReplace.LastModifiedAgent;
            IsDeleted = modelReplace.IsDeleted;
            DeletedUtc = modelReplace.DeletedUtc;
            DeletedBy = modelReplace.DeletedBy;
            DeletedAgent = modelReplace.DeletedAgent;
        }

    }
}
