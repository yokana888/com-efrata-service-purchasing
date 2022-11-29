using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary
{
    public class DebtAndDispositionSummaryService : IDebtAndDispositionSummaryService
    {
        private readonly PurchasingDbContext _dbContext;
        private readonly IdentityService _identityService;
        private readonly List<UnitDto> _units;
        //private readonly List<AccountingUnitDto> _units;

        private readonly List<AccountingUnitDto> _accountingUnits;
        private readonly List<CategoryDto> _categories;


        //private readonly IDistributedCache _cache;

        public DebtAndDispositionSummaryService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<PurchasingDbContext>();
            _identityService = serviceProvider.GetService<IdentityService>();
            var cache = serviceProvider.GetService<IDistributedCache>();
            var jsonUnits = cache.GetString(MemoryCacheConstant.Units);
            var jsonCategories = cache.GetString(MemoryCacheConstant.Categories);
            var jsonAccountingUnits = cache.GetString(MemoryCacheConstant.AccountingUnits);
            _units = JsonConvert.DeserializeObject<List<UnitDto>>(jsonUnits, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
            //_units = JsonConvert.DeserializeObject<List<AccountingUnitDto>>(jsonUnits, new JsonSerializerSettings
            //{
            //    MissingMemberHandling = MissingMemberHandling.Ignore
            //});

            _accountingUnits = JsonConvert.DeserializeObject<List<AccountingUnitDto>>(jsonAccountingUnits, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            _categories = JsonConvert.DeserializeObject<List<CategoryDto>>(jsonCategories, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }

        private IQueryable<DebtAndDispositionSummaryDto> GetDebtQuery(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency)
        {
            var unitReceiptNoteItems = _dbContext.UnitReceiptNoteItems.AsQueryable();
            var unitReceiptNotes = _dbContext.UnitReceiptNotes.AsQueryable();
            var unitPaymentOrderItems = _dbContext.UnitPaymentOrderItems.AsQueryable();
            var unitPaymentOrders = _dbContext.UnitPaymentOrders.AsQueryable();
            var externalPurchaseOrders = _dbContext.ExternalPurchaseOrders.AsQueryable();
            var purchaseRequests = _dbContext.PurchaseRequests.AsQueryable();

            var query = from unitReceiptNoteItem in unitReceiptNoteItems

                        join unitReceiptNote in unitReceiptNotes on unitReceiptNoteItem.URNId equals unitReceiptNote.Id into urnWithItems
                        from urnWithItem in urnWithItems.DefaultIfEmpty()

                        join unitPaymentOrderItem in unitPaymentOrderItems on urnWithItem.Id equals unitPaymentOrderItem.URNId into urnUPOItems
                        from urnUPOItem in urnUPOItems.DefaultIfEmpty()

                        join unitPaymentOrder in unitPaymentOrders on urnUPOItem.UPOId equals unitPaymentOrder.Id into upoWithItems
                        from upoWithItem in upoWithItems.DefaultIfEmpty()

                        join externalPurchaseOrder in externalPurchaseOrders on unitReceiptNoteItem.EPOId equals externalPurchaseOrder.Id into urnEPOs
                        from urnEPO in urnEPOs.DefaultIfEmpty()

                        join purchaseRequest in purchaseRequests on unitReceiptNoteItem.PRId equals purchaseRequest.Id into urnPRs
                        from urnPR in urnPRs.DefaultIfEmpty()

                        select new DebtAndDispositionSummaryDto
                        {
                            CurrencyId = urnEPO.CurrencyId,
                            CurrencyCode = urnEPO.CurrencyCode,
                            CurrencyRate = urnEPO.CurrencyRate,
                            CategoryId = urnPR.CategoryId,
                            CategoryCode = urnPR.CategoryCode,
                            CategoryName = urnPR.CategoryName,
                            UnitId = urnPR.UnitId,
                            UnitCode = urnPR.UnitCode,
                            UnitName = urnPR.UnitName,
                            DivisionId = urnPR.DivisionId,
                            DivisionCode = urnPR.DivisionCode,
                            DivisionName = urnPR.DivisionName,
                            IsImport = urnWithItem.SupplierIsImport,
                            IsPaid = upoWithItem != null && upoWithItem.IsPaid,
                            DebtPrice = unitReceiptNoteItem.PricePerDealUnit,
                            DebtQuantity = unitReceiptNoteItem.ReceiptQuantity,
                            DebtTotal = unitReceiptNoteItem.PricePerDealUnit * unitReceiptNoteItem.ReceiptQuantity,
                            DueDate = urnWithItem.ReceiptDate.AddDays(Convert.ToInt32(urnEPO.PaymentDueDays)),
                            IncomeTaxBy = urnEPO.IncomeTaxBy,
                            UseIncomeTax = urnEPO.UseIncomeTax,
                            IncomeTaxRate = urnEPO.IncomeTaxRate,
                            UseVat = urnEPO.UseVat
                        };

            query = query.Where(entity => !entity.IsPaid && (entity.IsImport == isImport) && entity.DueDate <= dueDate);

            if (categoryId > 0)
                query = query.Where(entity => entity.CategoryId == categoryId.ToString());

            if (accountingUnitId > 0)
            {
                var unitIds = _units.Where(unit => unit.Id == accountingUnitId).Select(unit => unit.Id.ToString()).ToList();
                if (unitIds.Count == 0)
                    // intentionally added to make the query returns empty data
                    unitIds.Add("0");
                query = query.Where(entity => unitIds.Contains(entity.UnitId));

            }

            if (divisionId > 0)
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());

            if (!isForeignCurrency && !isImport)
                query = query.Where(entity => entity.CurrencyCode.ToUpper() == "IDR" && !entity.IsImport);
            else if (isForeignCurrency)
                query = query.Where(entity => entity.CurrencyCode.ToUpper() != "IDR" && !entity.IsImport);
            else if (isImport)
                query = query.Where(entity => entity.IsImport);

            return query;
        }

        private IQueryable<DebtAndDispositionSummaryDto> GetDebtQuery(int unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, List<string> categoryIds)
        {
            var unitReceiptNoteItems = _dbContext.UnitReceiptNoteItems.AsQueryable();
            var unitReceiptNotes = _dbContext.UnitReceiptNotes.AsQueryable();
            var unitPaymentOrderItems = _dbContext.UnitPaymentOrderItems.AsQueryable();
            var unitPaymentOrders = _dbContext.UnitPaymentOrders.AsQueryable();
            var externalPurchaseOrders = _dbContext.ExternalPurchaseOrders.AsQueryable();
            var purchaseRequests = _dbContext.PurchaseRequests.AsQueryable();

            var query = from unitReceiptNoteItem in unitReceiptNoteItems

                        join unitReceiptNote in unitReceiptNotes on unitReceiptNoteItem.URNId equals unitReceiptNote.Id into urnWithItems
                        from urnWithItem in urnWithItems.DefaultIfEmpty()

                        join unitPaymentOrderItem in unitPaymentOrderItems on urnWithItem.Id equals unitPaymentOrderItem.URNId into urnUPOItems
                        from urnUPOItem in urnUPOItems.DefaultIfEmpty()

                        join unitPaymentOrder in unitPaymentOrders on urnUPOItem.UPOId equals unitPaymentOrder.Id into upoWithItems
                        from upoWithItem in upoWithItems.DefaultIfEmpty()

                        join externalPurchaseOrder in externalPurchaseOrders on unitReceiptNoteItem.EPOId equals externalPurchaseOrder.Id into urnEPOs
                        from urnEPO in urnEPOs.DefaultIfEmpty()

                        join purchaseRequest in purchaseRequests on unitReceiptNoteItem.PRId equals purchaseRequest.Id into urnPRs
                        from urnPR in urnPRs.DefaultIfEmpty()

                        select new DebtAndDispositionSummaryDto
                        {
                            CurrencyId = urnEPO.CurrencyId,
                            CurrencyCode = urnEPO.CurrencyCode,
                            CurrencyRate = urnEPO.CurrencyRate,
                            CategoryId = urnPR.CategoryId,
                            CategoryCode = urnPR.CategoryCode,
                            CategoryName = urnPR.CategoryName,
                            UnitId = urnPR.UnitId,
                            UnitCode = urnPR.UnitCode,
                            UnitName = urnPR.UnitName,
                            DivisionId = urnPR.DivisionId,
                            DivisionCode = urnPR.DivisionCode,
                            DivisionName = urnPR.DivisionName,
                            IsImport = urnWithItem.SupplierIsImport,
                            IsPaid = upoWithItem != null && upoWithItem.IsPaid,
                            DebtPrice = unitReceiptNoteItem.PricePerDealUnit,
                            DebtQuantity = unitReceiptNoteItem.ReceiptQuantity,
                            DebtTotal = unitReceiptNoteItem.PricePerDealUnit * unitReceiptNoteItem.ReceiptQuantity,
                            DueDate = urnWithItem.ReceiptDate.AddDays(Convert.ToInt32(urnEPO.PaymentDueDays)),
                            IncomeTaxBy = urnEPO.IncomeTaxBy,
                            UseIncomeTax = urnEPO.UseIncomeTax,
                            IncomeTaxRate = urnEPO.IncomeTaxRate,
                            UseVat = urnEPO.UseVat
                        };

            query = query.Where(entity => !entity.IsPaid && entity.DueDate <= date);

            if (categoryIds.Count > 0)
                query = query.Where(entity => categoryIds.Contains(entity.CategoryId));

            if (unitId > 0)
            {
                query = query.Where(entity => unitId.ToString() == entity.UnitId);
            }

            if (divisionId > 0)
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());

            return query;
        }

        private IQueryable<DebtAndDispositionSummaryDto> GetDebtQuery(List<string> unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, List<string> categoryIds)
        {
            var unitReceiptNoteItems = _dbContext.UnitReceiptNoteItems.AsQueryable();
            var unitReceiptNotes = _dbContext.UnitReceiptNotes.AsQueryable();
            var unitPaymentOrderItems = _dbContext.UnitPaymentOrderItems.AsQueryable();
            var unitPaymentOrders = _dbContext.UnitPaymentOrders.AsQueryable();
            var externalPurchaseOrders = _dbContext.ExternalPurchaseOrders.AsQueryable();
            var purchaseRequests = _dbContext.PurchaseRequests.AsQueryable();

            var query = from unitReceiptNoteItem in unitReceiptNoteItems

                        join unitReceiptNote in unitReceiptNotes on unitReceiptNoteItem.URNId equals unitReceiptNote.Id into urnWithItems
                        from urnWithItem in urnWithItems.DefaultIfEmpty()

                        join unitPaymentOrderItem in unitPaymentOrderItems on urnWithItem.Id equals unitPaymentOrderItem.URNId into urnUPOItems
                        from urnUPOItem in urnUPOItems.DefaultIfEmpty()

                        join unitPaymentOrder in unitPaymentOrders on urnUPOItem.UPOId equals unitPaymentOrder.Id into upoWithItems
                        from upoWithItem in upoWithItems.DefaultIfEmpty()

                        join externalPurchaseOrder in externalPurchaseOrders on unitReceiptNoteItem.EPOId equals externalPurchaseOrder.Id into urnEPOs
                        from urnEPO in urnEPOs.DefaultIfEmpty()

                        join purchaseRequest in purchaseRequests on unitReceiptNoteItem.PRId equals purchaseRequest.Id into urnPRs
                        from urnPR in urnPRs.DefaultIfEmpty()

                        select new DebtAndDispositionSummaryDto
                        {
                            CurrencyId = urnEPO.CurrencyId,
                            CurrencyCode = urnEPO.CurrencyCode,
                            CurrencyRate = urnEPO.CurrencyRate,
                            CategoryId = urnPR.CategoryId,
                            CategoryCode = urnPR.CategoryCode,
                            CategoryName = urnPR.CategoryName,
                            UnitId = urnPR.UnitId,
                            UnitCode = urnPR.UnitCode,
                            UnitName = urnPR.UnitName,
                            DivisionId = urnPR.DivisionId,
                            DivisionCode = urnPR.DivisionCode,
                            DivisionName = urnPR.DivisionName,
                            IsImport = urnWithItem.SupplierIsImport,
                            IsPaid = upoWithItem != null && upoWithItem.IsPaid,
                            DebtPrice = unitReceiptNoteItem.PricePerDealUnit,
                            DebtQuantity = unitReceiptNoteItem.ReceiptQuantity,
                            DebtTotal = unitReceiptNoteItem.PricePerDealUnit * unitReceiptNoteItem.ReceiptQuantity,
                            DueDate = urnWithItem.ReceiptDate.AddDays(Convert.ToInt32(urnEPO.PaymentDueDays)),
                            IncomeTaxBy = urnEPO.IncomeTaxBy,
                            UseIncomeTax = urnEPO.UseIncomeTax,
                            IncomeTaxRate = urnEPO.IncomeTaxRate,
                            UseVat = urnEPO.UseVat
                        };

            query = query.Where(entity => !entity.IsPaid && entity.DueDate <= date);

            if (categoryIds.Count > 0)
                query = query.Where(entity => categoryIds.Contains(entity.CategoryId));

            if (unitId.Where(s => s != "0").Count() > 0 && unitId.Count > 0)
            {
                //query = query.Where(entity => (entity.UnitId == null? false :  unitId.Contains(entity.UnitId.ToString())) && true );
                //var unitIdFirst = unitId.FirstOrDefault();
                //query = query.Where(entity => entity.UnitId != null && unitId.Contains(entity.UnitId.ToString()));
                query = query.Where(entity => unitId.Contains(entity.UnitId));


            }

            if (divisionId > 0)
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());

            return query;
        }

        private IQueryable<DebtAndDispositionSummaryDto> GetDispositionQuery(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency)
        {
            var externalPurchaseOrders = _dbContext.ExternalPurchaseOrders.AsQueryable();
            var purchasingDispositionDetails = _dbContext.PurchasingDispositionDetails.AsQueryable();
            var purchasingDispositionItems = _dbContext.PurchasingDispositionItems.AsQueryable();
            var purchasingDispositions = _dbContext.PurchasingDispositions.AsQueryable();

            var query = from purchasingDispositionDetail in purchasingDispositionDetails

                        join purchasingDispositionItem in purchasingDispositionItems on purchasingDispositionDetail.PurchasingDispositionItemId equals purchasingDispositionItem.Id into pdDetailItems
                        from pdDetailItem in pdDetailItems.DefaultIfEmpty()

                        join purchasingDisposition in purchasingDispositions on pdDetailItem.PurchasingDispositionId equals purchasingDisposition.Id into pdWithItems
                        from pdWithItem in pdWithItems.DefaultIfEmpty()

                        join externalPurchaseOrder in externalPurchaseOrders on pdDetailItem.EPOId equals externalPurchaseOrder.Id.ToString() into pdItemEPOs
                        from pdItemEPO in pdItemEPOs.DefaultIfEmpty()

                        select new DebtAndDispositionSummaryDto
                        {
                            CurrencyId = pdWithItem.CurrencyId,
                            CurrencyCode = pdWithItem.CurrencyCode,
                            CurrencyRate = pdWithItem.CurrencyRate,
                            CategoryId = pdWithItem.CategoryId,
                            CategoryCode = pdWithItem.CategoryCode,
                            CategoryName = pdWithItem.CategoryName,
                            UnitId = purchasingDispositionDetail.UnitId,
                            UnitCode = purchasingDispositionDetail.UnitCode,
                            UnitName = purchasingDispositionDetail.UnitName,
                            DivisionId = pdWithItem.DivisionId,
                            DivisionCode = pdWithItem.DivisionCode,
                            DivisionName = pdWithItem.DivisionName,
                            IsImport = pdItemEPO.SupplierIsImport,
                            IsPaid = pdWithItem.IsPaid,
                            DispositionPrice = purchasingDispositionDetail.PricePerDealUnit,
                            DispositionQuantity = purchasingDispositionDetail.PaidQuantity,
                            DispositionTotal = purchasingDispositionDetail.PaidPrice,
                            DueDate = pdWithItem.PaymentDueDate,
                            IncomeTaxBy = pdItemEPO.IncomeTaxBy,
                            UseIncomeTax = pdItemEPO.UseIncomeTax,
                            IncomeTaxRate = pdItemEPO.IncomeTaxRate,
                            UseVat = pdItemEPO.UseVat
                        };

            query = query.Where(entity => !entity.IsPaid && (entity.IsImport == isImport) && entity.DueDate <= dueDate);

            if (categoryId > 0)
                query = query.Where(entity => entity.CategoryId == categoryId.ToString());

            if (accountingUnitId > 0)
            {
                var unitIds = _units.Where(unit => unit.Id == accountingUnitId).Select(unit => unit.Id.ToString()).ToList();
                if (unitIds.Count == 0)
                    // intentionally added to make the query returns empty data
                    unitIds.Add("0");
                query = query.Where(entity => unitIds.Contains(entity.UnitId));

            }

            if (divisionId > 0)
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());

            if (!isForeignCurrency && !isImport)
                query = query.Where(entity => entity.CurrencyCode.ToUpper() == "IDR" && !entity.IsImport);
            else if (isForeignCurrency)
                query = query.Where(entity => entity.CurrencyCode.ToUpper() != "IDR" && !entity.IsImport);
            else if (isImport)
                query = query.Where(entity => entity.IsImport);

            return query;
        }

        private IQueryable<DebtAndDispositionSummaryDto> GetDispositionQuery(int unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, List<string> categoryIds)
        {
            var externalPurchaseOrders = _dbContext.ExternalPurchaseOrders.AsQueryable();
            var purchasingDispositionDetails = _dbContext.PurchasingDispositionDetails.AsQueryable();
            var purchasingDispositionItems = _dbContext.PurchasingDispositionItems.AsQueryable();
            var purchasingDispositions = _dbContext.PurchasingDispositions.AsQueryable();

            var query = from purchasingDispositionDetail in purchasingDispositionDetails

                        join purchasingDispositionItem in purchasingDispositionItems on purchasingDispositionDetail.PurchasingDispositionItemId equals purchasingDispositionItem.Id into pdDetailItems
                        from pdDetailItem in pdDetailItems.DefaultIfEmpty()

                        join purchasingDisposition in purchasingDispositions on pdDetailItem.PurchasingDispositionId equals purchasingDisposition.Id into pdWithItems
                        from pdWithItem in pdWithItems.DefaultIfEmpty()

                        join externalPurchaseOrder in externalPurchaseOrders on pdDetailItem.EPOId equals externalPurchaseOrder.Id.ToString() into pdItemEPOs
                        from pdItemEPO in pdItemEPOs.DefaultIfEmpty()

                        select new DebtAndDispositionSummaryDto
                        {
                            CurrencyId = pdWithItem.CurrencyId,
                            CurrencyCode = pdWithItem.CurrencyCode,
                            CurrencyRate = pdWithItem.CurrencyRate,
                            CategoryId = pdWithItem.CategoryId,
                            CategoryCode = pdWithItem.CategoryCode,
                            CategoryName = pdWithItem.CategoryName,
                            UnitId = purchasingDispositionDetail.UnitId,
                            UnitCode = purchasingDispositionDetail.UnitCode,
                            UnitName = purchasingDispositionDetail.UnitName,
                            DivisionId = pdWithItem.DivisionId,
                            DivisionCode = pdWithItem.DivisionCode,
                            DivisionName = pdWithItem.DivisionName,
                            IsImport = pdItemEPO.SupplierIsImport,
                            IsPaid = pdWithItem.IsPaid,
                            DispositionPrice = purchasingDispositionDetail.PricePerDealUnit,
                            DispositionQuantity = purchasingDispositionDetail.PaidQuantity,
                            DispositionTotal = purchasingDispositionDetail.PaidPrice,
                            DueDate = pdWithItem.PaymentDueDate,
                            IncomeTaxBy = pdItemEPO.IncomeTaxBy,
                            UseIncomeTax = pdItemEPO.UseIncomeTax,
                            IncomeTaxRate = pdItemEPO.IncomeTaxRate,
                            UseVat = pdItemEPO.UseVat
                        };

            query = query.Where(entity => !entity.IsPaid && entity.DueDate <= date);

            if (categoryIds.Count > 0)
                query = query.Where(entity => categoryIds.Contains(entity.CategoryId));

            if (unitId > 0)
            {
                query = query.Where(entity => unitId.ToString() == entity.UnitId);
            }

            if (divisionId > 0)
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());

            return query;
        }
        private IQueryable<DebtAndDispositionSummaryDto> GetDispositionQuery(List<string> unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, List<string> categoryIds)
        {
            var externalPurchaseOrders = _dbContext.ExternalPurchaseOrders.AsQueryable();
            var purchasingDispositionDetails = _dbContext.PurchasingDispositionDetails.AsQueryable();
            var purchasingDispositionItems = _dbContext.PurchasingDispositionItems.AsQueryable();
            var purchasingDispositions = _dbContext.PurchasingDispositions.AsQueryable();
            var query = from purchasingDispositionDetail in purchasingDispositionDetails

                        join purchasingDispositionItem in purchasingDispositionItems on purchasingDispositionDetail.PurchasingDispositionItemId equals purchasingDispositionItem.Id into pdDetailItems
                        from pdDetailItem in pdDetailItems.DefaultIfEmpty()

                        join purchasingDisposition in purchasingDispositions on pdDetailItem.PurchasingDispositionId equals purchasingDisposition.Id into pdWithItems
                        from pdWithItem in pdWithItems.DefaultIfEmpty()

                        join externalPurchaseOrder in externalPurchaseOrders on pdDetailItem.EPOId equals externalPurchaseOrder.Id.ToString() into pdItemEPOs
                        from pdItemEPO in pdItemEPOs.DefaultIfEmpty()

                        select new DebtAndDispositionSummaryDto
                        {
                            CurrencyId = pdWithItem.CurrencyId,
                            CurrencyCode = pdWithItem.CurrencyCode,
                            CurrencyRate = pdWithItem.CurrencyRate,
                            CategoryId = pdWithItem.CategoryId,
                            CategoryCode = pdWithItem.CategoryCode,
                            CategoryName = pdWithItem.CategoryName,
                            UnitId = purchasingDispositionDetail.UnitId,
                            UnitCode = purchasingDispositionDetail.UnitCode,
                            UnitName = purchasingDispositionDetail.UnitName,
                            DivisionId = pdWithItem.DivisionId,
                            DivisionCode = pdWithItem.DivisionCode,
                            DivisionName = pdWithItem.DivisionName,
                            IsImport = pdItemEPO.SupplierIsImport,
                            IsPaid = pdWithItem.IsPaid,
                            DispositionPrice = purchasingDispositionDetail.PricePerDealUnit,
                            DispositionQuantity = purchasingDispositionDetail.PaidQuantity,
                            DispositionTotal = purchasingDispositionDetail.PaidPrice,
                            DueDate = pdWithItem.PaymentDueDate,
                            IncomeTaxBy = pdItemEPO.IncomeTaxBy,
                            UseIncomeTax = pdItemEPO.UseIncomeTax,
                            IncomeTaxRate = pdItemEPO.IncomeTaxRate,
                            UseVat = pdItemEPO.UseVat
                        };

            query = query.Where(entity => !entity.IsPaid && entity.DueDate <= date);

            if (categoryIds.Count > 0)
                query = query.Where(entity => categoryIds.Contains(entity.CategoryId));

            if (unitId.Where(s => s != "0").Count() > 0 && unitId.Count > 0)
            {
                //query = query.Where(entity => (entity.UnitId == null ? false : unitId.Contains(entity.UnitId.ToString())) && true );
                //query = query.Where(entity => entity.UnitId != null && unitId.Contains(entity.UnitId.ToString()));
                query = query.Where(entity => unitId.Contains(entity.UnitId));

            }

            if (divisionId > 0)
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());

            return query;
        }


        public ReadResponse<DebtAndDispositionSummaryDto> GetReport(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency)
        {
            var debtQuery = GetDebtQuery(categoryId, accountingUnitId, divisionId, dueDate, isImport, isForeignCurrency);
            var dispositionQuery = GetDispositionQuery(categoryId, accountingUnitId, divisionId, dueDate, isImport, isForeignCurrency);

            var debts = debtQuery.ToList();
            var dispositions = dispositionQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(debts);
            result.AddRange(dispositions);


            result = result
                .Select(element =>
                {
                    double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                    var debtTotal = element.DebtTotal;
                    var dispositionTotal = element.DispositionTotal;

                    var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                    var categoryLayoutIndex = 0;
                    if (category != null)
                        categoryLayoutIndex = category.ReportLayoutIndex;

                    if (element.UseVat)
                    {
                        debtTotal += element.DebtTotal * 0.1;
                        dispositionTotal += element.DispositionTotal * 0.1;
                    }

                    if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                    {
                        debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                        dispositionTotal -= element.DispositionTotal * (incomeTaxRate / 100);
                    }

                    var accountingUnitName = "-";
                    var selectedAccountingUnitId = "";
                    var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                    if (unit != null)
                    {
                        var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.AccountingUnitId);
                        if (accountingUnit != null)
                        {
                            accountingUnitName = accountingUnit.Name;
                            selectedAccountingUnitId = accountingUnit.Id.ToString();
                        }
                    }

                    return new DebtAndDispositionSummaryDto()
                    {
                        CategoryCode = element.CategoryCode,
                        CategoryName = element.CategoryName,
                        CategoryLayoutIndex = categoryLayoutIndex,
                        CurrencyCode = element.CurrencyCode,
                        DebtTotal = debtTotal,
                        DispositionTotal = dispositionTotal,
                        Total = debtTotal + dispositionTotal,
                        DivisionId = element.DivisionId,
                        DivisionName = element.DivisionName,
                        DivisionCode = element.DivisionCode,
                        UnitCode = element.UnitCode,
                        UnitId = element.UnitId,
                        UnitName = element.UnitName,
                        AccountingUnitId = selectedAccountingUnitId,
                        AccountingUnitName = accountingUnitName
                    };
                })
                .GroupBy(element => new { element.CategoryCode, element.CurrencyCode })
                .Select(element =>
                {
                    var debtTotal = element.Sum(sum => sum.DebtTotal);
                    var dispositionTotal = element.Sum(sum => sum.DispositionTotal);


                    return new DebtAndDispositionSummaryDto()
                    {
                        CategoryCode = element.Key.CategoryCode,
                        CategoryName = element.FirstOrDefault().CategoryName,
                        CategoryLayoutIndex = element.FirstOrDefault().CategoryLayoutIndex,
                        CurrencyCode = element.Key.CurrencyCode,
                        DebtTotal = debtTotal,
                        DispositionTotal = dispositionTotal,
                        Total = debtTotal + dispositionTotal,
                        DivisionId = element.FirstOrDefault().DivisionId,
                        DivisionName = element.FirstOrDefault().DivisionName,
                        DivisionCode = element.FirstOrDefault().DivisionCode,
                        UnitCode = element.FirstOrDefault().UnitCode,
                        UnitId = element.FirstOrDefault().UnitId,
                        UnitName = element.FirstOrDefault().UnitName,
                        AccountingUnitId = element.FirstOrDefault().AccountingUnitId,
                        AccountingUnitName = element.FirstOrDefault().AccountingUnitName
                    };
                })
                .OrderBy(element => element.CategoryLayoutIndex)
                .ToList();

            return new ReadResponse<DebtAndDispositionSummaryDto>(result, result.Count, new Dictionary<string, string>());
        }

        public List<DebtAndDispositionSummaryDto> GetSummary(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency)
        {
            var debtQuery = GetDebtQuery(categoryId, accountingUnitId, divisionId, dueDate, isImport, isForeignCurrency);
            var dispositionQuery = GetDispositionQuery(categoryId, accountingUnitId, divisionId, dueDate, isImport, isForeignCurrency);

            var debts = debtQuery.ToList();
            var dispositions = dispositionQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(debts);
            result.AddRange(dispositions);

            result = result.Select(element =>
            {
                double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                var debtTotal = element.DebtTotal;
                var dispositionTotal = element.DispositionTotal;

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                var accountingUnitName = "-";
                var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                if (unit != null)
                {
                    var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.Id);
                    if (accountingUnit != null)
                        accountingUnitName = accountingUnit.Name;
                }

                if (element.UseVat)
                {
                    debtTotal += element.DebtTotal * 0.1;
                    dispositionTotal += element.DispositionTotal * 0.1;
                }

                if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                    dispositionTotal -= element.DispositionTotal * (incomeTaxRate / 100);
                }

                return new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.CategoryCode,
                    CategoryId = element.CategoryId,
                    CategoryName = element.CategoryName,
                    CurrencyCode = element.CurrencyCode,
                    CurrencyId = element.CurrencyId,
                    CurrencyRate = element.CurrencyRate,
                    DebtPrice = element.DebtPrice,
                    DebtQuantity = element.DebtQuantity,
                    DebtTotal = debtTotal,
                    DispositionPrice = element.DispositionPrice,
                    DispositionQuantity = element.DispositionQuantity,
                    DispositionTotal = dispositionTotal,
                    DivisionCode = element.DivisionCode,
                    DivisionId = element.DivisionId,
                    DivisionName = element.DivisionName,
                    DueDate = element.DueDate,
                    IncomeTaxBy = element.IncomeTaxBy,
                    IncomeTaxRate = element.IncomeTaxRate,
                    IsImport = element.IsImport,
                    IsPaid = element.IsPaid,
                    Total = debtTotal + dispositionTotal,
                    UnitCode = element.UnitCode,
                    UnitId = element.UnitId,
                    UnitName = element.UnitName,
                    UseIncomeTax = element.UseIncomeTax,
                    AccountingUnitName = accountingUnitName,
                    CategoryLayoutIndex = categoryLayoutIndex
                };
            })
                .OrderBy(element => element.CategoryLayoutIndex)
                .ToList();

            return result;
        }

        public ReadResponse<DebtAndDispositionSummaryDto> GetReportDebt(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency)
        {
            var debtQuery = GetDebtQuery(categoryId, accountingUnitId, divisionId, dueDate, isImport, isForeignCurrency);

            var debt = debtQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(debt);


            result = result
                .Select(element =>
                {
                    double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                    var debtTotal = element.DebtTotal;

                    var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                    var categoryLayoutIndex = 0;
                    if (category != null)
                        categoryLayoutIndex = category.ReportLayoutIndex;

                    if (element.UseVat)
                    {
                        debtTotal += element.DebtTotal * 0.1;
                    }

                    if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                    {
                        debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                    }

                    return new DebtAndDispositionSummaryDto()
                    {
                        CategoryCode = element.CategoryCode,
                        CategoryName = element.CategoryName,
                        CategoryLayoutIndex = categoryLayoutIndex,
                        CurrencyCode = element.CurrencyCode,
                        DebtTotal = debtTotal,
                        Total = debtTotal
                    };
                })
                .GroupBy(element => new { element.CategoryCode, element.CurrencyCode })
                .Select(element =>
                {
                    var debtTotal = element.Sum(sum => sum.DebtTotal);

                    return new DebtAndDispositionSummaryDto()
                    {
                        CategoryCode = element.Key.CategoryCode,
                        CategoryName = element.FirstOrDefault().CategoryName,
                        CategoryLayoutIndex = element.FirstOrDefault().CategoryLayoutIndex,
                        CurrencyCode = element.Key.CurrencyCode,
                        DebtTotal = debtTotal,
                        Total = debtTotal
                    };
                })
                .OrderBy(element => element.CategoryLayoutIndex)
                .ToList();

            return new ReadResponse<DebtAndDispositionSummaryDto>(result, result.Count, new Dictionary<string, string>());
        }

        public List<DebtAndDispositionSummaryDto> GetDebtSummary(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency)
        {
            var debtQuery = GetDebtQuery(categoryId, accountingUnitId, divisionId, dueDate, isImport, isForeignCurrency);

            var debt = debtQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(debt);

            result = result.Select(element =>
            {
                double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                var debtTotal = element.DebtTotal;

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                var accountingUnitName = "-";
                var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                if (unit != null)
                {
                    var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.Id);
                    if (accountingUnit != null)
                        accountingUnitName = accountingUnit.Name;
                }

                if (element.UseVat)
                {
                    debtTotal += element.DebtTotal * 0.1;
                }

                if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                }

                return new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.CategoryCode,
                    CategoryId = element.CategoryId,
                    CategoryName = element.CategoryName,
                    CurrencyCode = element.CurrencyCode,
                    CurrencyId = element.CurrencyId,
                    CurrencyRate = element.CurrencyRate,
                    DebtPrice = element.DebtPrice,
                    DebtQuantity = element.DebtQuantity,
                    DebtTotal = debtTotal,
                    DivisionCode = element.DivisionCode,
                    DivisionId = element.DivisionId,
                    DivisionName = element.DivisionName,
                    DueDate = element.DueDate,
                    IncomeTaxBy = element.IncomeTaxBy,
                    IncomeTaxRate = element.IncomeTaxRate,
                    IsImport = element.IsImport,
                    IsPaid = element.IsPaid,
                    Total = debtTotal,
                    UnitCode = element.UnitCode,
                    UnitId = element.UnitId,
                    UnitName = element.UnitName,
                    UseIncomeTax = element.UseIncomeTax,
                    CategoryLayoutIndex = categoryLayoutIndex,
                    AccountingUnitName = accountingUnitName
                };
            }).OrderBy(element => element.CategoryLayoutIndex).ToList();

            return result;
        }

        public ReadResponse<DebtAndDispositionSummaryDto> GetReportDisposition(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency)
        {
            var dispositionQuery = GetDispositionQuery(categoryId, accountingUnitId, divisionId, dueDate, isImport, isForeignCurrency);

            var dispositions = dispositionQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(dispositions);


            result = result
                .Select(element =>
                {
                    double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                    var dispositionTotal = element.DispositionTotal;

                    var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                    var categoryLayoutIndex = 0;
                    if (category != null)
                        categoryLayoutIndex = category.ReportLayoutIndex;

                    if (element.UseVat)
                    {
                        dispositionTotal += element.DispositionTotal * 0.1;
                    }

                    if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                    {
                        dispositionTotal -= element.DispositionTotal * (incomeTaxRate / 100);
                    }

                    return new DebtAndDispositionSummaryDto()
                    {
                        CategoryCode = element.CategoryCode,
                        CategoryName = element.CategoryName,
                        CategoryLayoutIndex = categoryLayoutIndex,
                        CurrencyCode = element.CurrencyCode,
                        DispositionTotal = dispositionTotal,
                        Total = dispositionTotal
                    };
                })
                .GroupBy(element => new { element.CategoryCode, element.CurrencyCode })
                .Select(element =>
                {
                    var dispositionTotal = element.Sum(sum => sum.DispositionTotal);

                    return new DebtAndDispositionSummaryDto()
                    {
                        CategoryCode = element.Key.CategoryCode,
                        CategoryName = element.FirstOrDefault().CategoryName,
                        CategoryLayoutIndex = element.FirstOrDefault().CategoryLayoutIndex,
                        CurrencyCode = element.Key.CurrencyCode,
                        DispositionTotal = dispositionTotal,
                        Total = dispositionTotal
                    };
                })
                .OrderBy(element => element.CategoryLayoutIndex)
                .ToList();

            return new ReadResponse<DebtAndDispositionSummaryDto>(result, result.Count, new Dictionary<string, string>());
        }

        public List<DebtAndDispositionSummaryDto> GetDispositionSummary(int categoryId, int accountingUnitId, int divisionId, DateTimeOffset dueDate, bool isImport, bool isForeignCurrency)
        {
            var dispositionQuery = GetDispositionQuery(categoryId, accountingUnitId, divisionId, dueDate, isImport, isForeignCurrency);

            var dispositions = dispositionQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(dispositions);

            result = result.Select(element =>
            {
                double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                var dispositionTotal = element.DispositionTotal;

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                var accountingUnitName = "-";
                var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                if (unit != null)
                {
                    var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.Id);
                    if (accountingUnit != null)
                        accountingUnitName = accountingUnit.Name;
                }

                if (element.UseVat)
                {
                    dispositionTotal += element.DispositionTotal * 0.1;
                }

                if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    dispositionTotal -= element.DispositionTotal * (incomeTaxRate / 100);
                }

                return new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.CategoryCode,
                    CategoryId = element.CategoryId,
                    CategoryName = element.CategoryName,
                    CurrencyCode = element.CurrencyCode,
                    CurrencyId = element.CurrencyId,
                    CurrencyRate = element.CurrencyRate,
                    DispositionPrice = element.DispositionPrice,
                    DispositionQuantity = element.DispositionQuantity,
                    DispositionTotal = dispositionTotal,
                    DivisionCode = element.DivisionCode,
                    DivisionId = element.DivisionId,
                    DivisionName = element.DivisionName,
                    DueDate = element.DueDate,
                    IncomeTaxBy = element.IncomeTaxBy,
                    IncomeTaxRate = element.IncomeTaxRate,
                    IsImport = element.IsImport,
                    IsPaid = element.IsPaid,
                    Total = dispositionTotal,
                    UnitCode = element.UnitCode,
                    UnitId = element.UnitId,
                    UnitName = element.UnitName,
                    UseIncomeTax = element.UseIncomeTax,
                    CategoryLayoutIndex = categoryLayoutIndex,
                    AccountingUnitName = accountingUnitName
                };
            })
                .OrderBy(element => element.CategoryLayoutIndex)
                .ToList();

            return result;
        }

        public List<DebtAndDispositionSummaryDto> GetSummary(int unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, string categoryIds = "[]")
        {
            var intCategoryIds = JsonConvert.DeserializeObject<List<int>>(categoryIds);
            var stringCategoryIds = intCategoryIds.Select(element => element.ToString()).ToList();

            var debtQuery = GetDebtQuery(unitId, divisionId, year, month, isImport, date, stringCategoryIds);
            var dispositionQuery = GetDispositionQuery(unitId, divisionId, year, month, isImport, date, stringCategoryIds);

            var debts = debtQuery.ToList();
            var dispositions = dispositionQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(debts);
            result.AddRange(dispositions);

            result = result.Select(element =>
            {
                double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                var debtTotal = element.DebtTotal;
                var dispositionTotal = element.DispositionTotal;

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                var accountingUnitName = "-";
                var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                if (unit != null)
                {
                    var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.Id);
                    if (accountingUnit != null)
                        accountingUnitName = accountingUnit.Name;
                }

                if (element.UseVat)
                {
                    debtTotal += element.DebtTotal * 0.1;
                    dispositionTotal += element.DispositionTotal * 0.1;
                }

                if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                    dispositionTotal -= element.DispositionTotal * (incomeTaxRate / 100);
                }

                return new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.CategoryCode,
                    CategoryId = element.CategoryId,
                    CategoryName = element.CategoryName,
                    CurrencyCode = element.CurrencyCode,
                    CurrencyId = element.CurrencyId,
                    CurrencyRate = element.CurrencyRate,
                    DebtPrice = element.DebtPrice,
                    DebtQuantity = element.DebtQuantity,
                    DebtTotal = debtTotal,
                    DispositionPrice = element.DispositionPrice,
                    DispositionQuantity = element.DispositionQuantity,
                    DispositionTotal = dispositionTotal,
                    DivisionCode = element.DivisionCode,
                    DivisionId = element.DivisionId,
                    DivisionName = element.DivisionName,
                    DueDate = element.DueDate,
                    IncomeTaxBy = element.IncomeTaxBy,
                    IncomeTaxRate = element.IncomeTaxRate,
                    IsImport = element.IsImport,
                    IsPaid = element.IsPaid,
                    Total = debtTotal + dispositionTotal,
                    UnitCode = element.UnitCode,
                    UnitId = element.UnitId,
                    UnitName = element.UnitName,
                    UseIncomeTax = element.UseIncomeTax,
                    AccountingUnitName = accountingUnitName,
                    CategoryLayoutIndex = categoryLayoutIndex
                };
            })
                .OrderBy(element => element.CategoryLayoutIndex)
                .ToList();

            return result;
        }

        public List<DebtAndDispositionSummaryDto> GetSummary(List<int> unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, string categoryIds = "[]")
        {
            var intCategoryIds = JsonConvert.DeserializeObject<List<int>>(categoryIds);
            var stringCategoryIds = intCategoryIds.Select(element => element.ToString()).ToList();
            var unitIdStr = unitId.Select(s => s.ToString()).ToList();

            var debtQuery = GetDebtQuery(unitIdStr, divisionId, year, month, isImport, date, stringCategoryIds);
            var dispositionQuery = GetDispositionQuery(unitIdStr, divisionId, year, month, isImport, date, stringCategoryIds);

            var debts = debtQuery.ToList();
            var dispositions = dispositionQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(debts);
            result.AddRange(dispositions);

            result = result.Select(element =>
            {
                double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                var debtTotal = element.DebtTotal;
                var dispositionTotal = element.DispositionTotal;

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                var accountingUnitName = "-";
                var accountingUnitId = "-";
                var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                if (unit != null)
                {
                    var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.AccountingUnitId);
                    if (accountingUnit != null)
                    {
                        accountingUnitName = accountingUnit.Name;
                        accountingUnitId = accountingUnit.Id.ToString();
                    }
                }

                if (element.UseVat)
                {
                    debtTotal += element.DebtTotal * 0.1;
                    dispositionTotal += element.DispositionTotal * 0.1;
                }

                if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                    dispositionTotal -= element.DispositionTotal * (incomeTaxRate / 100);
                }

                return new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.CategoryCode,
                    CategoryId = element.CategoryId,
                    CategoryName = element.CategoryName,
                    CurrencyCode = element.CurrencyCode,
                    CurrencyId = element.CurrencyId,
                    CurrencyRate = element.CurrencyRate,
                    DebtPrice = element.DebtPrice,
                    DebtQuantity = element.DebtQuantity,
                    DebtTotal = debtTotal,
                    DispositionPrice = element.DispositionPrice,
                    DispositionQuantity = element.DispositionQuantity,
                    DispositionTotal = dispositionTotal,
                    DivisionCode = element.DivisionCode,
                    DivisionId = element.DivisionId,
                    DivisionName = element.DivisionName,
                    DueDate = element.DueDate,
                    IncomeTaxBy = element.IncomeTaxBy,
                    IncomeTaxRate = element.IncomeTaxRate,
                    IsImport = element.IsImport,
                    IsPaid = element.IsPaid,
                    Total = debtTotal + dispositionTotal,
                    UnitCode = element.UnitCode,
                    UnitId = element.UnitId,
                    UnitName = element.UnitName,
                    //UnitId = element.AccountingUnitId,
                    //UnitName = element.AccountingUnitName,
                    UseIncomeTax = element.UseIncomeTax,
                    AccountingUnitName = accountingUnitName,
                    AccountingUnitId = accountingUnitId,
                    CategoryLayoutIndex = categoryLayoutIndex
                };
            })
                .OrderBy(element => element.CategoryLayoutIndex)
                .ToList();

            return result;
        }


        public List<DebtAndDispositionSummaryDto> GetDebtSummary(int unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, string categoryIds = "[]")
        {
            var intCategoryIds = JsonConvert.DeserializeObject<List<int>>(categoryIds);
            var stringCategoryIds = intCategoryIds.Select(element => element.ToString()).ToList();
            var debtQuery = GetDebtQuery(unitId, divisionId, year, month, isImport, date, stringCategoryIds);

            var debt = debtQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(debt);

            result = result.Select(element =>
            {
                double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                var debtTotal = element.DebtTotal;

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                var accountingUnitName = "-";
                var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                if (unit != null)
                {
                    var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.Id);
                    if (accountingUnit != null)
                        accountingUnitName = accountingUnit.Name;
                }

                if (element.UseVat)
                {
                    debtTotal += element.DebtTotal * 0.1;
                }

                if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                }

                return new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.CategoryCode,
                    CategoryId = element.CategoryId,
                    CategoryName = element.CategoryName,
                    CurrencyCode = element.CurrencyCode,
                    CurrencyId = element.CurrencyId,
                    CurrencyRate = element.CurrencyRate,
                    DebtPrice = element.DebtPrice,
                    DebtQuantity = element.DebtQuantity,
                    DebtTotal = debtTotal,
                    DivisionCode = element.DivisionCode,
                    DivisionId = element.DivisionId,
                    DivisionName = element.DivisionName,
                    DueDate = element.DueDate,
                    IncomeTaxBy = element.IncomeTaxBy,
                    IncomeTaxRate = element.IncomeTaxRate,
                    IsImport = element.IsImport,
                    IsPaid = element.IsPaid,
                    Total = debtTotal,
                    UnitCode = element.UnitCode,
                    UnitId = element.UnitId,
                    UnitName = element.UnitName,
                    UseIncomeTax = element.UseIncomeTax,
                    CategoryLayoutIndex = categoryLayoutIndex,
                    AccountingUnitName = accountingUnitName
                };
            }).OrderBy(element => element.CategoryLayoutIndex).ToList();

            return result;
        }

        public List<DebtAndDispositionSummaryDto> GetDebtSummary(List<int> unitId, int divisionId, int year, int month, bool isImport, DateTimeOffset date, string categoryIds = "[]")
        {
            var intCategoryIds = JsonConvert.DeserializeObject<List<int>>(categoryIds);
            var stringCategoryIds = intCategoryIds.Select(element => element.ToString()).ToList();
            var unitIdsStr = unitId.Select(s => s.ToString()).ToList();
            var debtQuery = GetDebtQuery(unitIdsStr, divisionId, year, month, isImport, date, stringCategoryIds);

            var debt = debtQuery.ToList();

            var result = new List<DebtAndDispositionSummaryDto>();
            result.AddRange(debt);

            result = result.Select(element =>
            {
                double.TryParse(element.IncomeTaxRate, NumberStyles.Any, CultureInfo.InvariantCulture, out var incomeTaxRate);
                var debtTotal = element.DebtTotal;

                var category = _categories.FirstOrDefault(_category => _category.Id.ToString() == element.CategoryId);
                var categoryLayoutIndex = 0;
                if (category != null)
                    categoryLayoutIndex = category.ReportLayoutIndex;

                var accountingUnitName = "-";
                var accountingUnitId = "-";
                var unit = _units.FirstOrDefault(_unit => _unit.Id.ToString() == element.UnitId);
                if (unit != null)
                {
                    var accountingUnit = _accountingUnits.FirstOrDefault(_accountingUnit => _accountingUnit.Id == unit.AccountingUnitId);
                    if (accountingUnit != null)
                    {
                        accountingUnitName = accountingUnit.Name;
                        accountingUnitId = accountingUnit.Id.ToString();
                    }
                }

                if (element.UseVat)
                {
                    debtTotal += element.DebtTotal * 0.1;
                }

                if (element.UseIncomeTax && element.IncomeTaxBy.ToUpper() == "SUPPLIER")
                {
                    debtTotal -= element.DebtTotal * (incomeTaxRate / 100);
                }

                return new DebtAndDispositionSummaryDto()
                {
                    CategoryCode = element.CategoryCode,
                    CategoryId = element.CategoryId,
                    CategoryName = element.CategoryName,
                    CurrencyCode = element.CurrencyCode,
                    CurrencyId = element.CurrencyId,
                    CurrencyRate = element.CurrencyRate,
                    DebtPrice = element.DebtPrice,
                    DebtQuantity = element.DebtQuantity,
                    DebtTotal = debtTotal,
                    DivisionCode = element.DivisionCode,
                    DivisionId = element.DivisionId,
                    DivisionName = element.DivisionName,
                    DueDate = element.DueDate,
                    IncomeTaxBy = element.IncomeTaxBy,
                    IncomeTaxRate = element.IncomeTaxRate,
                    IsImport = element.IsImport,
                    IsPaid = element.IsPaid,
                    Total = debtTotal,
                    UnitCode = element.UnitCode,
                    UnitId = element.UnitId,
                    UnitName = element.UnitName,
                    //UnitId = accountingUnitId,
                    //UnitName = accountingUnitName,
                    UseIncomeTax = element.UseIncomeTax,
                    CategoryLayoutIndex = categoryLayoutIndex,
                    AccountingUnitName = accountingUnitName,
                    AccountingUnitId = accountingUnitId
                };
            }).OrderBy(element => element.CategoryLayoutIndex).ToList();

            return result;
        }

    }
}
