using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Facades.DebtAndDispositionSummary;
using Com.Efrata.Service.Purchasing.Lib.Models.BudgetCashflowWorstCaseModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Moonlay.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public class BudgetCashflowService : IBudgetCashflowService
    {
        private const string UserAgent = "purchasing-service";
        private readonly PurchasingDbContext _dbContext;
        private readonly IdentityService _identityService;
        private readonly List<BudgetingCategoryDto> _budgetingCategories;
        private readonly List<CategoryDto> _categories;
        private readonly List<UnitDto> _units;

        public BudgetCashflowService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<PurchasingDbContext>();
            _identityService = serviceProvider.GetService<IdentityService>();

            var cache = serviceProvider.GetService<IDistributedCache>();
            var jsonBudgetingCategories = cache.GetString(MemoryCacheConstant.BudgetingCategories);
            _budgetingCategories = JsonConvert.DeserializeObject<List<BudgetingCategoryDto>>(jsonBudgetingCategories, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            var jsonCategories = cache.GetString(MemoryCacheConstant.Categories);
            _categories = JsonConvert.DeserializeObject<List<CategoryDto>>(jsonCategories, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            var jsonUnits = cache.GetString(MemoryCacheConstant.Units);
            _units = JsonConvert.DeserializeObject<List<UnitDto>>(jsonUnits, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }

        public async Task<int> UpsertWorstCaseBudgetCashflowUnit(WorstCaseBudgetCashflowFormDto form)
        {
            var month = form.DueDate.AddHours(_identityService.TimezoneOffset).AddMonths(1).Month;
            var year = form.DueDate.AddHours(_identityService.TimezoneOffset).AddMonths(1).Year;
            var model = _dbContext.BudgetCashflowWorstCases.FirstOrDefault(entity => entity.UnitId == form.UnitId && entity.Month == month && entity.Year == year);

            if (model == null)
            {
                model = new BudgetCashflowWorstCase(form.DueDate.AddHours(_identityService.TimezoneOffset), form.UnitId);
                EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
                _dbContext.BudgetCashflowWorstCases.Add(model);
                await _dbContext.SaveChangesAsync();
            }

            var items = _dbContext.BudgetCashflowWorstCaseItems.Where(entity => entity.BudgetCashflowWorstCaseId == model.Id);

            foreach (var formItem in form.Items)
            {
                var currencyId = formItem.Currency != null ? formItem.Currency.Id : 0;
                var existingItem = items.FirstOrDefault(item => item.CurrencyId == currencyId && item.LayoutOrder == formItem.LayoutOrder && item.BudgetCashflowWorstCaseId == model.Id);

                if (existingItem != null)
                {
                    existingItem.UpdateNominal(formItem.CurrencyNominal, formItem.Nominal, formItem.ActualNominal);
                    EntityExtension.FlagForUpdate(existingItem, _identityService.Username, UserAgent);
                    _dbContext.BudgetCashflowWorstCaseItems.Update(existingItem);
                }
                else
                {
                    var item = new BudgetCashflowWorstCaseItem(formItem.LayoutOrder, currencyId, formItem.CurrencyNominal, formItem.Nominal, formItem.ActualNominal, model.Id, form.UnitId);
                    EntityExtension.FlagForCreate(item, _identityService.Username, UserAgent);
                    _dbContext.BudgetCashflowWorstCaseItems.Add(item);
                }
            }
            await _dbContext.SaveChangesAsync();

            //var model = new BudgetCashflowWorstCase(form.Date.AddHours(_identityService.TimezoneOffset), form.UnitId);
            //EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            //_dbContext.BudgetCashflowWorstCases.Add(model);
            //await _dbContext.SaveChangesAsync();

            //var items = form.Items.Select(item => new BudgetCashflowWorstCaseItem(item.LayoutOrder, item.Currency.Id, item.CurrencyNominal, item.Nominal, model.Id));
            //_dbContext.BudgetCashflowWorstCaseItems.AddRange(items);
            //await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        public List<BudgetCashflowItemDto> GetBudgetCashflowWorstCase(DateTimeOffset dueDate, int unitId)
        {
            var model = _dbContext.BudgetCashflowWorstCases.FirstOrDefault(entity => entity.Year == dueDate.AddMonths(1).Year && entity.Month == dueDate.AddMonths(1).Month && entity.UnitId == unitId);

            var result = new List<BudgetCashflowItemDto>();
            if (model != null)
            {
                result = _dbContext
                    .BudgetCashflowWorstCaseItems
                    .Where(entity => entity.BudgetCashflowWorstCaseId == model.Id)
                    .OrderBy(entity => entity.LayoutOrder)
                    .Select(entity => new BudgetCashflowItemDto(entity.Id, entity.CurrencyId, entity.CurrencyNominal, entity.Nominal, entity.ActualNominal, entity.LayoutOrder))
                    .ToList();
            }
            return result;
        }

        public List<BudgetCashflowItemDto> GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder layoutOrder, int unitId, DateTimeOffset dueDate)
        {
            var queryResult = GetDebtAndDispositionSummary(layoutOrder, unitId, dueDate, 0);

            var result = queryResult
                .GroupBy(element => new { element.CurrencyId, element.UnitId, element.DivisionId })
                .Select(element => new BudgetCashflowItemDto(
                    element.Key.CurrencyId,
                    element.FirstOrDefault().CurrencyCode,
                    element.FirstOrDefault().CurrencyRate,
                    element.Sum(s => s.Total),
                    layoutOrder,
                    element.Key.UnitId,
                    element.Key.DivisionId
                    ))
                .ToList();
            if (result.Count <= 0)
            {
                result = new List<BudgetCashflowItemDto>() { new BudgetCashflowItemDto(0, 0, 0, 0, 0, layoutOrder) };
            }


            return result;
        }

        public List<BudgetCashflowItemDto> GetBudgetCashflowByCategoryAndUnitId(List<int> categoryIds, int unitId, DateTimeOffset dueDate, int divisionId, bool isImport)
        {
            var queryResult = GetDebtAndDispositionSummaryByCategory(categoryIds, unitId, dueDate, divisionId, isImport);

            var result = queryResult
                .GroupBy(element => new { element.CurrencyId, element.UnitId, element.DivisionId })
                .Select(element => new BudgetCashflowItemDto(
                    element.Key.CurrencyId,
                    element.FirstOrDefault().CurrencyCode,
                    element.FirstOrDefault().CurrencyRate,
                    element.Sum(s => s.Total),
                    0,
                    element.Key.UnitId,
                    element.Key.DivisionId
                    ))
                .ToList();
            if (result.Count <= 0)
            {
                result = new List<BudgetCashflowItemDto>() { new BudgetCashflowItemDto(0, 0, 0, 0, 0, 0) };
            }


            return result;
        }

        private IQueryable<DebtAndDispositionSummaryDto> GetDispositionQuery(List<int> budgetingCategoryIds, int unitId, DateTimeOffset dueDate, bool isImport, int divisionId, bool isRawMaterial)
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

            query = query.Where(entity => !entity.IsPaid && entity.DueDate <= dueDate);

            if (isRawMaterial)
            {
                query = query.Where(entity => entity.IsImport == isImport);
            }

            if (budgetingCategoryIds.Count > 0)
            {
                var categoryIds = _categories.Where(element => budgetingCategoryIds.Contains(element.BudgetingCategoryId)).Select(element => element.Id.ToString()).ToList();

                if (categoryIds.Count > 0)
                {
                    query = query.Where(entity => categoryIds.Contains(entity.CategoryId));
                }
            }

            if (unitId > 0)
            {
                query = query.Where(entity => entity.UnitId == unitId.ToString());
            }

            if (divisionId > 0)
            {
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());
            }

            return query;
        }

        private IQueryable<DebtAndDispositionSummaryDto> GetDebtQuery(List<int> budgetingCategoryIds, int unitId, DateTimeOffset dueDate, bool isImport, int divisionId, bool isRawMaterial)
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

            query = query.Where(entity => !entity.IsPaid && entity.DueDate <= dueDate);

            if (isRawMaterial)
            {
                query = query.Where(entity => entity.IsImport == isImport);
            }

            if (budgetingCategoryIds.Count > 0)
            {
                var categoryIds = _categories.Where(element => budgetingCategoryIds.Contains(element.BudgetingCategoryId)).Select(element => element.Id.ToString()).ToList();

                if (categoryIds.Count > 0)
                {
                    query = query.Where(entity => categoryIds.Contains(entity.CategoryId));
                }
            }

            if (unitId > 0)
            {
                query = query.Where(entity => entity.UnitId == unitId.ToString());
            }

            if (divisionId > 0)
            {
                query = query.Where(entity => entity.DivisionId == divisionId.ToString());
            }

            return query;
        }

        private List<DebtAndDispositionSummaryDto> GetDebtDispositionSummary(List<int> budgetCategoryIds, int unitId, DateTimeOffset dueDate, bool isImport, int divisionId, bool isRawMaterial)
        {
            var debtQuery = GetDebtQuery(budgetCategoryIds, unitId, dueDate, isImport, divisionId, isRawMaterial);
            var dispositionQuery = GetDispositionQuery(budgetCategoryIds, unitId, dueDate, isImport, divisionId, isRawMaterial);

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
                    CategoryLayoutIndex = categoryLayoutIndex
                };
            })
                .OrderBy(element => element.CategoryLayoutIndex)
                .ToList();

            return result;
        }

        public Task<int> UpdateWorstCaseBudgetCashflowUnit(int year, int month, int unitId, WorstCaseBudgetCashflowFormDto form)
        {
            throw new NotImplementedException();
        }

        private List<DebtAndDispositionSummaryDto> GetDebtAndDispositionSummary(BudgetCashflowCategoryLayoutOrder layoutOrder, int unitId, DateTimeOffset dueDate, int divisionId)
        {
            var budgetingCategoryNames = new List<string>();
            var budgetingCategoryIds = new List<int>();

            var result = new List<DebtAndDispositionSummaryDto>();
            switch (layoutOrder)
            {
                case BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial:
                    budgetingCategoryNames = new List<string>() { "BAHAN BAKU" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, true, divisionId, true);
                case BudgetCashflowCategoryLayoutOrder.LocalRawMaterial:
                    budgetingCategoryNames = new List<string>() { "BAHAN BAKU" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, true);
                case BudgetCashflowCategoryLayoutOrder.AuxiliaryMaterial:
                    budgetingCategoryNames = new List<string>() { "BAHAN PEMBANTU" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.Embalage:
                    budgetingCategoryNames = new List<string>() { "EMBALAGE" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.Coal:
                    budgetingCategoryNames = new List<string>() { "BATU BARA" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.FuelOil:
                    budgetingCategoryNames = new List<string>() { "BBM & PELUMAS" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.SparePartsMachineMaintenance:
                    budgetingCategoryNames = new List<string>() { "SPARE PART & PEMELIHARAAN MESIN" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeBuildingMaintenance:
                    budgetingCategoryNames = new List<string>() { "PEMELIHARAAN GEDUNG" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeStationary:
                    budgetingCategoryNames = new List<string>() { "ALAT TULIS" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeCorporateHousehold:
                    budgetingCategoryNames = new List<string>() { "URTP" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeVehicleCost:
                    budgetingCategoryNames = new List<string>() { "KENDARAAN (BEBAN)" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeOthersCost:
                    budgetingCategoryNames = new List<string>() { "LAIN-LAIN" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.MachineryPurchase:
                    budgetingCategoryNames = new List<string>() { "MESIN" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.VehiclePurchase:
                    budgetingCategoryNames = new List<string>() { "KENDARAAN" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.InventoryPurchase:
                    budgetingCategoryNames = new List<string>() { "INVENTARIS" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.ComputerToolsPurchase:
                    budgetingCategoryNames = new List<string>() { "ALAT KOMPUTER" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                //case BudgetCashflowCategoryLayoutOrder.ProductionToolsMaterialsPurchase:
                //    budgetingCategoryNames = new List<string>() { "ALAT DAN BAHAN PRODUKSI" };
                //    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                //    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                case BudgetCashflowCategoryLayoutOrder.ProjectPurchase:
                    budgetingCategoryNames = new List<string>() { "PROYEK" };
                    budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
                    return GetDebtDispositionSummary(budgetingCategoryIds, unitId, dueDate, false, divisionId, false);
                default:
                    return result;
            }
        }

        private List<DebtAndDispositionSummaryDto> GetDebtAndDispositionSummaryByCategory(List<int> categoryIds, int unitId, DateTimeOffset dueDate, int divisionId, bool isImport)
        {
            var budgetingCategoryNames = new List<string>();
            var budgetingCategoryIds = new List<int>();

            budgetingCategoryIds = _budgetingCategories.Where(element => budgetingCategoryNames.Contains(element.Name.ToUpper())).Select(element => element.Id).ToList();
            return GetDebtDispositionSummary(categoryIds, unitId, dueDate, isImport, divisionId, isImport);

        }

        public BudgetCashflowDivisionDto GetBudgetCashflowDivision(BudgetCashflowCategoryLayoutOrder layoutOrder, int divisionId, DateTimeOffset dueDate)
        {
            //var queryResult = GetDebtAndDispositionSummary(layoutOrder, 0, dueDate, divisionId);

            //var result = queryResult
            //    .GroupBy(element => new { element.CurrencyId, element.UnitId })
            //    .Select(element => new BudgetCashflowDivisionItemDto(
            //        element.Key.CurrencyId,
            //        element.FirstOrDefault().CurrencyCode,
            //        element.FirstOrDefault().CurrencyRate,
            //        element.FirstOrDefault().DivisionId,
            //        element.Key.UnitId,
            //        element.Sum(s => s.Total),
            //        layoutOrder
            //        ))
            //    .ToList();

            //if (result.Count <= 0)
            //{
            //    result = new List<BudgetCashflowDivisionItemDto>() { new BudgetCashflowDivisionItemDto("0", "", 0, "0", "0", 0, layoutOrder) };
            //}

            //var unitIds = result.Where(element => element.UnitId != 0).Select(element => element.UnitId).Distinct().ToList();


            //return new BudgetCashflowDivisionDto(unitIds, result);

            var unitIds = _units.Where(element => element.DivisionId == divisionId).Select(element => element.Id).ToList();
            var query = _dbContext.BudgetCashflowWorstCases.Where(entity => entity.Year == dueDate.AddMonths(1).Year && entity.Month == dueDate.AddMonths(1).Month);
            if (divisionId > 0)
            {
                query = query.Where(entity => unitIds.Contains(entity.UnitId));
            }

            unitIds = query.Select(entity => entity.UnitId).Distinct().ToList();

            var models = query.ToList();

            var result = new List<BudgetCashflowDivisionItemDto>();
            if (models.Count > 0)
            {
                var modelIds = models.Select(model => model.Id).ToList();
                result = _dbContext
                    .BudgetCashflowWorstCaseItems
                    .Where(entity => modelIds.Contains(entity.BudgetCashflowWorstCaseId) && entity.LayoutOrder == layoutOrder)
                    .GroupBy(element => new { element.CurrencyId, element.UnitId })
                    .Select(element => new BudgetCashflowDivisionItemDto(
                        element.Key.CurrencyId,
                        divisionId,
                        element.Key.UnitId,
                        element.Sum(s => s.Nominal),
                        element.Sum(s => s.CurrencyNominal),
                        element.Sum(s => s.ActualNominal),
                        layoutOrder))
                    .OrderBy(entity => entity.LayoutOrder)
                    .ToList();
            }

            if (result.Count <= 0)
            {
                result = new List<BudgetCashflowDivisionItemDto>() { new BudgetCashflowDivisionItemDto("0", "", 0, "0", "0", 0, layoutOrder) };
            }

            return new BudgetCashflowDivisionDto(unitIds, result);
        }

        public List<BudgetCashflowItemDto> GetCashInOperatingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            return new List<BudgetCashflowItemDto>() { new BudgetCashflowItemDto(0, 0, 0, 0, 0, 0, 0, 0) };
        }

        public List<BudgetCashflowItemDto> GetCashOutOperatingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            var layoutOrders = new List<BudgetCashflowCategoryLayoutOrder>()
            {
                BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial,
                BudgetCashflowCategoryLayoutOrder.LocalRawMaterial,
                BudgetCashflowCategoryLayoutOrder.AuxiliaryMaterial,
                BudgetCashflowCategoryLayoutOrder.Embalage,
                BudgetCashflowCategoryLayoutOrder.Coal,
                BudgetCashflowCategoryLayoutOrder.FuelOil,
                BudgetCashflowCategoryLayoutOrder.SparePartsMachineMaintenance,
                BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeBuildingMaintenance,
                BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeStationary,
                BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeCorporateHousehold,
                BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeVehicleCost,
                BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeOthersCost
            };

            var importRawMaterial = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.ImportedRawMaterial, unitId, dueDate);
            var localRawMaterial = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.LocalRawMaterial, unitId, dueDate);
            var auxiliaryMaterial = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.AuxiliaryMaterial, unitId, dueDate);
            var embalage = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.Embalage, unitId, dueDate);
            var coal = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.Coal, unitId, dueDate);
            var fuelOil = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.FuelOil, unitId, dueDate);
            var sparePart = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.SparePartsMachineMaintenance, unitId, dueDate);
            var buildingMaintenance = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeBuildingMaintenance, unitId, dueDate);
            var stationary = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeStationary, unitId, dueDate);
            var corporateHousehold = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeCorporateHousehold, unitId, dueDate);
            var vehicleCost = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeVehicleCost, unitId, dueDate);
            var othersCost = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.GeneralAdministrativeOthersCost, unitId, dueDate);

            var worstCases = GetBudgetCashflowWorstCase(dueDate, unitId);

            var bestCases = new List<BudgetCashflowItemDto>();
            bestCases.AddRange(importRawMaterial);
            bestCases.AddRange(localRawMaterial);
            bestCases.AddRange(auxiliaryMaterial);
            bestCases.AddRange(embalage);
            bestCases.AddRange(coal);
            bestCases.AddRange(fuelOil);
            bestCases.AddRange(sparePart);
            bestCases.AddRange(buildingMaintenance);
            bestCases.AddRange(stationary);
            bestCases.AddRange(corporateHousehold);
            bestCases.AddRange(vehicleCost);
            bestCases.AddRange(othersCost);

            bestCases = bestCases
                .GroupBy(element => element.CurrencyId)
                .Select(element => new BudgetCashflowItemDto(
                    element.Key,
                    0,
                    0,
                    0,
                    element.Sum(s => s.CurrencyNominal),
                    element.Sum(s => s.Nominal),
                    element.Sum(s => s.ActualNominal),
                    0
                    ))
                .ToList();

            worstCases = worstCases
                .Where(element => layoutOrders.Contains(element.LayoutOrder))
                .GroupBy(element => element.CurrencyId)
                .Select(element => new BudgetCashflowItemDto(
                    element.Key,
                    element.Sum(s => s.CurrencyNominal),
                    element.Sum(s => s.Nominal),
                    element.Sum(s => s.ActualNominal),
                    0,
                    0,
                    0,
                    0
                    ))
                .ToList();

            var result = new List<BudgetCashflowItemDto>();
            foreach (var bestCase in bestCases)
            {
                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == bestCase.CurrencyId);
                if (worstCase != null)
                {
                    result.Add(new BudgetCashflowItemDto(bestCase.CurrencyId, worstCase.CurrencyNominal, worstCase.Nominal, worstCase.ActualNominal, bestCase.BestCaseCurrencyNominal, bestCase.BestCaseNominal, bestCase.BestCaseActualNominal, 0));
                }
                else
                {
                    result.Add(new BudgetCashflowItemDto(bestCase.CurrencyId, 0, 0, 0, bestCase.BestCaseCurrencyNominal, bestCase.BestCaseNominal, bestCase.BestCaseActualNominal, 0));
                }
            }

            if (result.Count > 1)
                result = result.Where(element => element.CurrencyId > 0).ToList();

            return result;
        }

        public List<BudgetCashflowItemDto> GetDiffOperatingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            var cashOutItems = GetCashOutOperatingActivitiesByUnit(unitId, dueDate);
            var cashInItems = GetCashInOperatingActivitiesByUnit(unitId, dueDate);

            var result = new List<BudgetCashflowItemDto>();
            foreach (var cashOutItem in cashOutItems)
            {
                var cashInItem = cashInItems.FirstOrDefault(element => element.CurrencyId == cashOutItem.CurrencyId);
                if (cashInItem == null)
                    cashInItem = new BudgetCashflowItemDto(0, 0, 0, 0, 0, 0, 0, 0);

                result.Add(new BudgetCashflowItemDto(
                    cashOutItem.CurrencyId,
                    cashInItem.CurrencyNominal - cashOutItem.CurrencyNominal,
                    cashInItem.Nominal - cashOutItem.Nominal,
                    cashInItem.ActualNominal - cashOutItem.ActualNominal,
                    cashInItem.BestCaseCurrencyNominal - cashOutItem.BestCaseCurrencyNominal,
                    cashInItem.BestCaseNominal - cashOutItem.BestCaseNominal,
                    cashInItem.BestCaseActualNominal - cashOutItem.BestCaseActualNominal,
                    cashOutItem.LayoutOrder
                    ));
            }

            if (result.Count > 1)
                result = result.Where(element => element.CurrencyId > 0).ToList();

            return result;
        }

        public List<BudgetCashflowItemDto> GetCashInInvestingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            return new List<BudgetCashflowItemDto>() { new BudgetCashflowItemDto(0, 0, 0, 0, 0, 0, 0, 0) };
        }

        public List<BudgetCashflowItemDto> GetCashOutInvestingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            var layoutOrders = new List<BudgetCashflowCategoryLayoutOrder>()
            {
                BudgetCashflowCategoryLayoutOrder.MachineryPurchase,
                BudgetCashflowCategoryLayoutOrder.VehiclePurchase,
                BudgetCashflowCategoryLayoutOrder.InventoryPurchase,
                BudgetCashflowCategoryLayoutOrder.ComputerToolsPurchase,
                //BudgetCashflowCategoryLayoutOrder.ProductionToolsMaterialsPurchase,
                BudgetCashflowCategoryLayoutOrder.ProjectPurchase
            };

            var machine = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.MachineryPurchase, unitId, dueDate);
            var vehicle = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.VehiclePurchase, unitId, dueDate);
            var inventory = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.InventoryPurchase, unitId, dueDate);
            var computerTools = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.ComputerToolsPurchase, unitId, dueDate);
            //var productionToolsMaterial = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.ProductionToolsMaterialsPurchase, unitId, dueDate);
            var project = GetBudgetCashflowUnit(BudgetCashflowCategoryLayoutOrder.ProjectPurchase, unitId, dueDate);

            var worstCases = GetBudgetCashflowWorstCase(dueDate, unitId);

            var bestCases = new List<BudgetCashflowItemDto>();
            bestCases.AddRange(machine);
            bestCases.AddRange(vehicle);
            bestCases.AddRange(inventory);
            bestCases.AddRange(computerTools);
            //bestCases.AddRange(productionToolsMaterial);
            bestCases.AddRange(project);

            bestCases = bestCases
                .GroupBy(element => element.CurrencyId)
                .Select(element => new BudgetCashflowItemDto(
                    element.Key,
                    0,
                    0,
                    0,
                    element.Sum(s => s.CurrencyNominal),
                    element.Sum(s => s.Nominal),
                    element.Sum(s => s.ActualNominal),
                    0
                    ))
                .ToList();

            worstCases = worstCases
                .Where(element => layoutOrders.Contains(element.LayoutOrder))
                .GroupBy(element => element.CurrencyId)
                .Select(element => new BudgetCashflowItemDto(
                    element.Key,
                    element.Sum(s => s.CurrencyNominal),
                    element.Sum(s => s.Nominal),
                    element.Sum(s => s.ActualNominal),
                    0,
                    0,
                    0,
                    0
                    ))
                .ToList();

            var result = new List<BudgetCashflowItemDto>();
            foreach (var bestCase in bestCases)
            {
                var worstCase = worstCases.FirstOrDefault(element => element.CurrencyId == bestCase.CurrencyId);
                if (worstCase != null)
                {
                    result.Add(new BudgetCashflowItemDto(bestCase.CurrencyId, worstCase.CurrencyNominal, worstCase.Nominal, worstCase.ActualNominal, bestCase.BestCaseCurrencyNominal, bestCase.BestCaseNominal, bestCase.BestCaseActualNominal, 0));
                }
                else
                {
                    result.Add(new BudgetCashflowItemDto(bestCase.CurrencyId, 0, 0, 0, bestCase.BestCaseCurrencyNominal, bestCase.BestCaseNominal, bestCase.BestCaseActualNominal, 0));
                }
            }

            if (result.Count > 1)
                result = result.Where(element => element.CurrencyId > 0).ToList();

            return result;
        }

        public List<BudgetCashflowItemDto> GetDiffInvestingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            var cashOutItems = GetCashOutInvestingActivitiesByUnit(unitId, dueDate);
            var cashInItems = GetCashInInvestingActivitiesByUnit(unitId, dueDate);

            var result = new List<BudgetCashflowItemDto>();
            foreach (var cashOutItem in cashOutItems)
            {
                var cashInItem = cashInItems.FirstOrDefault(element => element.CurrencyId == cashOutItem.CurrencyId);
                if (cashInItem == null)
                    cashInItem = new BudgetCashflowItemDto(0, 0, 0, 0, 0, 0, 0, 0);

                result.Add(new BudgetCashflowItemDto(
                    cashOutItem.CurrencyId,
                    cashInItem.CurrencyNominal - cashOutItem.CurrencyNominal,
                    cashInItem.Nominal - cashOutItem.Nominal,
                    cashInItem.ActualNominal - cashOutItem.ActualNominal,
                    cashInItem.BestCaseCurrencyNominal - cashOutItem.BestCaseCurrencyNominal,
                    cashInItem.BestCaseNominal - cashOutItem.BestCaseNominal,
                    cashInItem.BestCaseActualNominal - cashOutItem.BestCaseActualNominal,
                    cashOutItem.LayoutOrder
                    ));
            }

            if (result.Count > 1)
                result = result.Where(element => element.CurrencyId > 0).ToList();

            return result;
        }

        public List<BudgetCashflowItemDto> GetCashInFinancingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            return new List<BudgetCashflowItemDto>() { new BudgetCashflowItemDto(0, 0, 0, 0, 0, 0, 0, 0) };
        }

        public List<BudgetCashflowItemDto> GetCashOutFinancingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            return new List<BudgetCashflowItemDto>() { new BudgetCashflowItemDto(0, 0, 0, 0, 0, 0, 0, 0) };
        }

        public List<BudgetCashflowItemDto> GetDiffFinancingActivitiesByUnit(int unitId, DateTimeOffset dueDate)
        {
            return new List<BudgetCashflowItemDto>() { new BudgetCashflowItemDto(0, 0, 0, 0, 0, 0, 0, 0) };
        }
    }
}
