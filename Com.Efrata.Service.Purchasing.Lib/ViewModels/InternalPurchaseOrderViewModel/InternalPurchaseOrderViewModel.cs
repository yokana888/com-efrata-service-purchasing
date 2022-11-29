using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Microsoft.EntityFrameworkCore;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.InternalPurchaseOrderViewModel
{
    public class InternalPurchaseOrderViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string poNo { get; set; }
        public string isoNo { get; set; }
        public string prId { get; set; }
        public string prNo { get; set; }
        public DateTimeOffset prDate { get; set; }
        public DateTimeOffset expectedDeliveryDate { get; set; }
        public BudgetViewModel budget { get; set; }
        public DivisionViewModel division { get; set; }
        public UnitViewModel unit { get; set; }
        public CategoryViewModel category { get; set; }
        public string remark { get; set; }
        public bool isPosted { get; set; }
        public bool isClosed { get; set; }
        public string status { get; set; }
        public int countPRNo { get; set; }
        public List<InternalPurchaseOrderItemViewModel> items { get; set; }

        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            PurchasingDbContext dbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
            //InternalPurchaseOrder a =
            if (this.prNo == null)
            {
                yield return new ValidationResult("No. PR is required", new List<string> { "prNo" });
            }
            if (this.items.Count.Equals(0))
            {
                yield return new ValidationResult("Items is required", new List<string> { "itemscount" });
            }
            //InternalPurchaseOrder NewData = dbContext.InternalPurchaseOrders.Include(p => p.Items).FirstOrDefault(p => p.PONo == this.poNo);
            //var n = dbContext.InternalPurchaseOrders.Count(pr => pr.PRNo == prNo && !pr.IsDeleted);
            if(this.poNo != null)
            {
                InternalPurchaseOrder NewData = dbContext.InternalPurchaseOrders
                    .Select(s => new InternalPurchaseOrder
                    {
                        Id = s.Id,
                        UId = s.UId,
                        PONo = s.PONo,
                        PRNo = s.PRNo,
                        ExpectedDeliveryDate = s.ExpectedDeliveryDate,
                        UnitName = s.UnitName,
                        DivisionName = s.DivisionName,
                        CategoryName = s.CategoryName,
                        IsPosted = s.IsPosted,
                        CreatedBy = s.CreatedBy,
                        PRDate = s.PRDate,
                        LastModifiedUtc = s.LastModifiedUtc,
                        PRId = s.PRId,
                        Items = s.Items
                    .Select(
                        q => new InternalPurchaseOrderItem
                        {
                            Id = q.Id,
                            POId = q.POId,
                            PRItemId = q.PRItemId,
                            ProductId = q.ProductId,
                            ProductName = q.ProductName,
                            ProductCode = q.ProductCode,
                            UomId = q.UomId,
                            UomUnit = q.UomUnit,
                            Quantity = q.Quantity,
                            ProductRemark = q.ProductRemark,
                            Status = q.Status
                        }
                    )
                    .Where(t => t.POId.Equals(s.Id) && t.Quantity != 0)
                    .ToList()
                    })
                .FirstOrDefault(p => p.PRNo == this.prNo && p.PONo == this.poNo);
                ;
                var n = dbContext.InternalPurchaseOrderItems.Count(pr => pr.POId == NewData.Id && pr.Quantity!=0 && !pr.IsDeleted);
                var j = 0;
                var k = 0;
                foreach (InternalPurchaseOrderItemViewModel Item in items)
                {
                    foreach (var itemCreate in NewData.Items)
                    {
                        if (itemCreate.Quantity == Item.quantity && itemCreate.Id==Item._id)
                        {
                            k += 1;
                        }
                        if (Item.quantity > itemCreate.Quantity && itemCreate.Id == Item._id)
                        {
                            yield return new ValidationResult("Jumlah Pecah PO Tidak Boleh Lebih dari Jumlah Awal", new List<string> { "itemscount" });
                            j += 1;
                            k = 0;
                        }
                    }
                }
                if (k >= n && j == 0 && this.items.Count == n)
                {
                    yield return new ValidationResult("Data belum ada yang diubah", new List<string> { "itemscount" });
                }
            }
        }
    }
}