using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel
{
    public class GarmentUnitExpenditureNoteViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string UENNo { get; set; }
        public DateTimeOffset ExpenditureDate { get; set; }
        public string ExpenditureType { get; set; }
        public string ExpenditureTo { get; set; }
        public long UnitDOId { get; set; }
        public string UnitDONo { get; set; }

        public UnitViewModel UnitRequest { get; set; }

        public UnitViewModel UnitSender { get; set; }
        public IntegrationViewModel.StorageViewModel Storage { get; set; }
        public IntegrationViewModel.StorageViewModel StorageRequest { get; set; }
        public bool IsPreparing { get; set; }
        public bool IsTransfered { get; set; }
        public bool IsReceived { get; set; }
        public DateTimeOffset UnitDODate { get; set; }

        public List<GarmentUnitExpenditureNoteItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IGarmentUnitDeliveryOrderFacade unitDeliveryOrderFacade = validationContext == null ? null : (IGarmentUnitDeliveryOrderFacade)validationContext.GetService(typeof(IGarmentUnitDeliveryOrderFacade));

            if (ExpenditureDate.Equals(DateTimeOffset.MinValue) || ExpenditureDate == null)
            {
                yield return new ValidationResult("Tanggal Pengeluaran Diperlukan", new List<string> { "ExpenditureDate" });
            }
            else if(UnitDODate > ExpenditureDate)
            {
                yield return new ValidationResult($"Tanggal Pengeluaran Tidak boleh kurang dari {UnitDODate.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID"))}", new List<string> { "ExpenditureDate" });
            }
            if (UnitDONo == null)
            {
                yield return new ValidationResult("Nomor Delivery Order Diperlukan", new List<string> { "UnitDONo" });
            }

            int itemErrorCount = 0;

            if (this.Items == null || Items.Count(i => i.IsSave) <= 0)
            {
                yield return new ValidationResult("Item is required", new List<string> { "ItemsCount" });
            }
            else
            {
                string itemError = "[";
                
                foreach (var item in Items)
                {
                    itemError += "{";

                    if (item.IsSave)
                    {
                        var unitDO = unitDeliveryOrderFacade.ReadById((int)UnitDOId);
                        if (unitDO != null)
                        {
                            var unitDOItem = unitDO.Items.Where(s => s.Id == item.UnitDOItemId).FirstOrDefault();
                            if (unitDOItem != null)
                            {
                                if ((double)item.Quantity > unitDOItem.Quantity)
                                {
                                    itemErrorCount++;
                                    itemError += "Quantity: 'Jumlah tidak boleh lebih dari " + unitDOItem.Quantity + "', ";
                                }
                            }
                        }


                        PurchasingDbContext dbContext = (PurchasingDbContext)validationContext.GetService(typeof(PurchasingDbContext));
                        var UENItem = dbContext.GarmentUnitExpenditureNoteItems.AsNoTracking().FirstOrDefault(x => x.Id == item.Id);
                        if (UENItem != null)
                        {
                            if ((double)item.Quantity > UENItem.Quantity)
                            {
                                itemErrorCount++;
                                itemError += "Quantity: 'Jumlah tidak boleh lebih dari " + UENItem.Quantity + "', ";
                            }

                        }
                        if (item.Quantity <= 0)
                        {
                            itemErrorCount++;
                            itemError += "Quantity: 'Jumlah harus lebih dari 0', ";
                        }
                    }
                    
                    itemError += "}, ";
                }

                itemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(itemError, new List<string> { "Items" });
            }
        }
    }
}
