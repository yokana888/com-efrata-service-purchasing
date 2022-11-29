using Com.Efrata.Service.Purchasing.Lib.Enums;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentInternNoteFacades
{
    public class GarmentInternNoteFacades : IGarmentInternNoteFacade
    {
        private readonly PurchasingDbContext dbContext;
        private readonly DbSet<GarmentInternNote> dbSet;
        private readonly DbSet<GarmentExternalPurchaseOrderItem> dbSetExternalPurchaseOrderItem;
        public readonly IServiceProvider serviceProvider;
        private readonly IGarmentDebtBalanceService _garmentDebtBalanceService;
        private string USER_AGENT = "Facade";

        public GarmentInternNoteFacades(PurchasingDbContext dbContext, IServiceProvider serviceProvider)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<GarmentInternNote>();
            dbSetExternalPurchaseOrderItem = dbContext.Set<GarmentExternalPurchaseOrderItem>();
            this.serviceProvider = serviceProvider;
            _garmentDebtBalanceService = serviceProvider.GetService<IGarmentDebtBalanceService>();
        }

        public async Task<int> Create(GarmentInternNote m, bool isImport, string user, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(m, user, USER_AGENT);

                    m.INNo = await GenerateNo(m, isImport, clientTimeZoneOffset);
                    m.INDate = DateTimeOffset.Now;

                    foreach (var item in m.Items)
                    {
                        GarmentInvoice garmentInvoice = this.dbContext.GarmentInvoices.FirstOrDefault(s => s.Id == item.InvoiceId);
                        if (garmentInvoice != null)
                            garmentInvoice.HasInternNote = true;
                        EntityExtension.FlagForCreate(item, user, USER_AGENT);
                        foreach (var detail in item.Details)
                        {
                            GarmentDeliveryOrder garmentDeliveryOrder = this.dbContext.GarmentDeliveryOrders.FirstOrDefault(s => s.Id == detail.DOId);
                            GarmentInternalPurchaseOrder internalPurchaseOrder = this.dbContext.GarmentInternalPurchaseOrders.FirstOrDefault(s => s.RONo.Equals(detail.RONo));
                            if (internalPurchaseOrder != null)
                            {
                                detail.UnitId = internalPurchaseOrder.UnitId;
                                detail.UnitCode = internalPurchaseOrder.UnitCode;
                                detail.UnitName = internalPurchaseOrder.UnitName;
                            }
                            if (garmentDeliveryOrder != null)
                            {
                                garmentDeliveryOrder.InternNo = m.INNo;
                            }
                            EntityExtension.FlagForCreate(detail, user, USER_AGENT);

                            await _garmentDebtBalanceService.UpdateFromInternalNote((int)detail.DOId, new InternalNoteFormDto((int)m.Id, m.INNo));
                        }
                    }

                    this.dbSet.Add(m);

                    Created = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        public int Delete(int id, string username)
        {
            int Deleted = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var model = this.dbSet
                                .Include(m => m.Items)
                                .ThenInclude(i => i.Details)
                                .SingleOrDefault(m => m.Id == id && !m.IsDeleted);

                    EntityExtension.FlagForDelete(model, username, USER_AGENT);
                    foreach (var item in model.Items)
                    {
                        GarmentInvoice garmentInvoice = this.dbContext.GarmentInvoices.FirstOrDefault(s => s.Id == item.InvoiceId);

                        if (garmentInvoice != null)
                        {
                            garmentInvoice.HasInternNote = false;
                        }
                        EntityExtension.FlagForDelete(item, username, USER_AGENT);
                        foreach (var detail in item.Details)
                        {
                            GarmentDeliveryOrder garmentDeliveryOrder = this.dbContext.GarmentDeliveryOrders.FirstOrDefault(s => s.Id == detail.DOId);
                            if (garmentDeliveryOrder != null)
                            {
                                garmentDeliveryOrder.InternNo = null;
                            }
                            EntityExtension.FlagForDelete(detail, username, USER_AGENT);

                            var result = _garmentDebtBalanceService.EmptyInternalNote((int)garmentDeliveryOrder.Id).Result;
                        }
                    }

                    Deleted = dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Deleted;
        }

        public Tuple<List<GarmentInternNote>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentInternNote> Query = dbSet;

            List<string> searchAttributes = new List<string>()
            {
                "INNo", "SupplierName", "Items.InvoiceNo"
            };

            Query = QueryHelper<GarmentInternNote>.ConfigureSearch(Query, searchAttributes, Keyword);

            Query = Query.Select(s => new GarmentInternNote
            {
                Id = s.Id,
                INNo = s.INNo,
                INDate = s.INDate,
                SupplierName = s.SupplierName,
                CreatedBy = s.CreatedBy,
                LastModifiedUtc = s.LastModifiedUtc,
                Position = s.Position,
                Items = s.Items.Select(i => new GarmentInternNoteItem
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceNo = i.InvoiceNo,
                    InvoiceDate = i.InvoiceDate,
                    Details = i.Details.Select(d => new GarmentInternNoteDetail
                    {
                        DOId = d.DOId
                    }).ToList()
                }).ToList()
            });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<GarmentInternNote>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<GarmentInternNote>.ConfigureOrder(Query, OrderDictionary);

            Pageable<GarmentInternNote> pageable = new Pageable<GarmentInternNote>(Query, Page - 1, Size);
            List<GarmentInternNote> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public GarmentInternNote ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                .Include(m => m.Items)
                    .ThenInclude(i => i.Details)
                .FirstOrDefault();
            return model;
        }

        public HashSet<long> GetGarmentInternNoteId(long id)
        {
            return new HashSet<long>(dbContext.GarmentInternNoteItems.Where(d => d.InternNote.Id == id).Select(d => d.Id));
        }

        public async Task<int> Update(int id, GarmentInternNote m, string user, int clientTimeZoneOffset = 7)
        {
            int Updated = 0;

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (m.Items != null)
                    {
                        double total = 0;
                        HashSet<long> detailIds = GetGarmentInternNoteId(id);
                        foreach (var itemId in detailIds)
                        {
                            GarmentInternNoteItem data = m.Items.FirstOrDefault(prop => prop.Id.Equals(itemId));
                            if (data == null)
                            {
                                GarmentInternNoteItem dataItem = dbContext.GarmentInternNoteItems.FirstOrDefault(prop => prop.Id.Equals(itemId));
                                EntityExtension.FlagForDelete(dataItem, user, USER_AGENT);
                                var Details = dbContext.GarmentInternNoteDetails.Where(prop => prop.GarmentItemINId.Equals(itemId)).ToList();
                                GarmentInvoice garmentInvoices = dbContext.GarmentInvoices.FirstOrDefault(s => s.Id.Equals(dataItem.InvoiceId));
                                if (garmentInvoices != null)
                                {
                                    garmentInvoices.HasInternNote = false;
                                }
                                foreach (GarmentInternNoteDetail detail in Details)
                                {
                                    GarmentDeliveryOrder garmentDeliveryOrder = this.dbContext.GarmentDeliveryOrders.FirstOrDefault(s => s.Id == detail.DOId);
                                    garmentDeliveryOrder.InternNo = null;

                                    EntityExtension.FlagForDelete(detail, user, USER_AGENT);
                                    await _garmentDebtBalanceService.EmptyInternalNote((int)detail.DOId);
                                }
                            }
                            else
                            {
                                EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                            }

                            foreach (GarmentInternNoteItem item in m.Items)
                            {
                                total += item.TotalAmount;
                                if (item.Id <= 0)
                                {
                                    GarmentInvoice garmentInvoice = this.dbContext.GarmentInvoices.FirstOrDefault(s => s.Id == item.InvoiceId);
                                    if (garmentInvoice != null)
                                        garmentInvoice.HasInternNote = true;
                                    EntityExtension.FlagForCreate(item, user, USER_AGENT);

                                }
                                else
                                {
                                    EntityExtension.FlagForUpdate(item, user, USER_AGENT);
                                }

                                foreach (GarmentInternNoteDetail detail in item.Details)
                                {
                                    if (item.Id <= 0)
                                    {
                                        GarmentDeliveryOrder garmentDeliveryOrder = this.dbContext.GarmentDeliveryOrders.FirstOrDefault(s => s.Id == detail.DOId);
                                        GarmentInternalPurchaseOrder internalPurchaseOrder = this.dbContext.GarmentInternalPurchaseOrders.FirstOrDefault(s => s.RONo == detail.RONo);
                                        if (internalPurchaseOrder != null)
                                        {
                                            detail.UnitId = internalPurchaseOrder.UnitId;
                                            detail.UnitCode = internalPurchaseOrder.UnitCode;
                                            detail.UnitName = internalPurchaseOrder.UnitName;
                                        }
                                        if (garmentDeliveryOrder != null)
                                        {
                                            garmentDeliveryOrder.InternNo = m.INNo;
                                        }
                                        EntityExtension.FlagForCreate(detail, user, USER_AGENT);

                                        await _garmentDebtBalanceService.UpdateFromInternalNote((int)garmentDeliveryOrder.Id, new InternalNoteFormDto((int)m.Id, m.INNo));
                                    }
                                    else
                                    {
                                        EntityExtension.FlagForUpdate(detail, user, USER_AGENT);
                                    }
                                }
                            }
                        }
                    }
                    EntityExtension.FlagForUpdate(m, user, USER_AGENT);
                    this.dbSet.Update(m);
                    Updated = await dbContext.SaveChangesAsync();
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Updated;
        }
        async Task<string> GenerateNo(GarmentInternNote model, bool isImport, int clientTimeZoneOffset)
        {
            DateTimeOffset dateTimeOffsetNow = DateTimeOffset.Now;
            string Month = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string Year = dateTimeOffsetNow.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Supplier = isImport ? "I" : "L";

            string no = $"NI{Year}{Month}";
            int Padding = 4;

            var lastNo = await this.dbSet.Where(w => w.INNo.StartsWith(no) && w.INNo.EndsWith(Supplier) && !w.IsDeleted).OrderByDescending(o => o.INNo).FirstOrDefaultAsync();

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0') + Supplier;
            }
            else
            {
                //int lastNoNumber = Int32.Parse(lastNo.INNo.Replace(no, "")) + 1;
                int.TryParse(lastNo.INNo.Replace(no, "").Replace(Supplier, ""), out int lastno1);
                int lastNoNumber = lastno1 + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0') + Supplier;

            }
        }
        #region Monitoring
        public Tuple<List<GarmentInternNoteReportViewModel>, int> GetReport(string no, string supplierCode, string curencyCode, string invoiceNo, string npn, string doNo, string billNo, string paymentBill, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetReportInternNote(no, supplierCode, curencyCode, invoiceNo, npn, doNo, billNo, paymentBill, dateFrom, dateTo, offset, page, size);
            //Console.WriteLine(Query);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.iNDate).ToList();
                Query = Query.OrderByDescending(b => b.invoiceNo).ToList();
            }
            //Pageable<GarmentInternNoteReportViewModel> pageable = new Pageable<GarmentInternNoteReportViewModel>(Query, page - 1, size);
            //List<GarmentInternNoteReportViewModel> Data = pageable.Data.ToList<GarmentInternNoteReportViewModel>();
            //int TotalData = pageable.TotalCount;

            return Tuple.Create(Query, TotalCountReport);
        }
        public int TotalCountReport { get; set; } = 0;
        public List<GarmentInternNoteReportViewModel> GetReportInternNote(string no, string supplierCode, string curencyCode, string invoiceNo, string npn, string doNo, string billNo, string paymentBill, DateTime? dateFrom, DateTime? dateTo, int offset, int page, int size)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
            //int i = 1;
            List<GarmentInternNoteReportViewModel> list = new List<GarmentInternNoteReportViewModel>();
            var Query = (from a in dbContext.GarmentInternNotes
                         join b in dbContext.GarmentInternNoteItems on a.Id equals b.GarmentINId
                         join c in dbContext.GarmentInternNoteDetails on b.Id equals c.GarmentItemINId
                         join d in dbContext.GarmentDeliveryOrders on c.DOId equals d.Id
                         join e in dbContext.GarmentInvoices on b.InvoiceId equals e.Id
                         where a.IsDeleted == false
                         && b.IsDeleted == false
                         && c.IsDeleted == false
                         && d.IsDeleted == false
                         && e.IsDeleted == false
                         && a.INNo == (string.IsNullOrWhiteSpace(no) ? a.INNo : no)
                         && b.InvoiceNo == (string.IsNullOrWhiteSpace(invoiceNo) ? b.InvoiceNo : invoiceNo)
                         //&& npn != null ? e.NPN == npn : false 
                         //&& e.NPN == (string.IsNullOrWhiteSpace(npn) ? e.NPN : npn)
                         && c.DONo == (string.IsNullOrWhiteSpace(doNo) ? c.DONo : doNo)
                         && d.BillNo == (string.IsNullOrWhiteSpace(billNo) ? d.BillNo : billNo)
                         && d.PaymentBill == (string.IsNullOrWhiteSpace(paymentBill) ? d.PaymentBill : paymentBill)
                         && a.SupplierCode == (string.IsNullOrWhiteSpace(supplierCode) ? a.SupplierCode : supplierCode)
                         && a.INDate.AddHours(offset).Date >= DateFrom.Date
                         && a.INDate.AddHours(offset).Date <= DateTo.Date
                         && a.CurrencyCode == (string.IsNullOrWhiteSpace(curencyCode) ? a.CurrencyCode : curencyCode)
                         group new { a, b, c, d, e } by new { c.DONo } into pg
                         let firstproduct = pg.FirstOrDefault()
                         let IN = firstproduct.a
                         let InItem = firstproduct.b
                         let InDetail = firstproduct.c
                         let Do = firstproduct.d
                         let Inv = firstproduct.e
                         select new
                         {
                             InternNoteId = IN.Id,
                             internNoteItemId = InItem.Id,
                             internNoteDetailId = InDetail.Id,
                             deliveryOrderId = Do.Id,
                             invoiceId = Inv.Id,
                             priceTotal = pg.Sum(m => m.c.PriceTotal),
                             INDate = IN.INDate,
                             NPN = Inv.NPN
                         });
            //  select new
            //{
            //    InternNoteId = a.Id,
            //    internNoteItemId = b.Id,
            //    internNoteDetailId = c.Id,
            //    deliveryOrderId = d.Id,
            //    invoiceId = e.Id,
            //    //priceTotal = pg.Sum(m => m.c.PriceTotal),
            //    INDate = a.INDate
            //});


            if (npn != null)
            {
                Query = Query.Where(x => x.NPN == npn);
            }
            //var Data = Query.GroupBy(s => s.doNo);
            TotalCountReport = Query.Distinct().Count();
            var queryResult = Query.Distinct().OrderByDescending(o => o.INDate).Skip((page - 1) * size).Take(size).ToList();
            var internnoteIds = queryResult.Distinct().Select(x => x.InternNoteId).ToList();
            var internnotes = dbContext.GarmentInternNotes.Where(x => internnoteIds.Contains(x.Id)).Select(x => new { x.Id, x.INNo, x.INDate, x.CurrencyCode, x.SupplierName, x.SupplierCode, x.CreatedBy }).ToList();
            var internnoteitemIds = queryResult.Distinct().Select(x => x.internNoteItemId).ToList();
            var internnoteitems = dbContext.GarmentInternNoteItems.Where(x => internnoteitemIds.Contains(x.Id)).Select(x => new { x.Id, x.InvoiceNo, x.InvoiceDate }).ToList();
            var internnotedetailIds = queryResult.Distinct().Select(x => x.internNoteDetailId).ToList();
            var internnotedetails = dbContext.GarmentInternNoteDetails.Where(x => internnotedetailIds.Contains(x.Id)).Select(x => new { x.Id, x.DONo, x.DODate, x.ProductName }).ToList();
            var deliveryorderIds = queryResult.Distinct().Select(x => x.deliveryOrderId).ToList();
            var deliveryorders = dbContext.GarmentDeliveryOrders.Where(x => deliveryorderIds.Contains(x.Id)).Select(x => new { x.Id, x.BillNo, x.PaymentBill, x.DOCurrencyRate, x.PaymentMethod }).ToList();
            var invoiceIds = queryResult.Distinct().Select(x => x.invoiceId).ToList();
            var invoices = dbContext.GarmentInvoices.Where(x => invoiceIds.Contains(x.Id)).Select(x => new { x.Id, x.NPN, x.VatNo }).ToList();

            foreach (var item in queryResult)
            {
                var internnote = internnotes.FirstOrDefault(x => x.Id.Equals(item.InternNoteId));
                var internnoteitem = internnoteitems.FirstOrDefault(x => x.Id.Equals(item.internNoteItemId));
                var internnotedetail = internnotedetails.FirstOrDefault(x => x.Id.Equals(item.internNoteDetailId));
                var deliveryorder = deliveryorders.FirstOrDefault(x => x.Id.Equals(item.deliveryOrderId));
                var invoice = invoices.FirstOrDefault(x => x.Id.Equals(item.invoiceId));
                var data1 = GetInvoice(item.invoiceId);

                list.Add(new GarmentInternNoteReportViewModel
                {
                    inNo = internnote.INNo,
                    iNDate = internnote.INDate,
                    currencyCode = internnote.CurrencyCode,
                    supplierName = internnote.SupplierName,
                    invoiceNo = internnoteitem.InvoiceNo,
                    invoiceDate = internnoteitem.InvoiceDate,
                    NPN = invoice.NPN,
                    VatNo = invoice.VatNo,
                    //pOSerialNumber = String.Join(",",pg.Select(m=>m.c.POSerialNumber)),//InDetail.POSerialNumber,
                    priceTotal = item.priceTotal,//InDetail.PriceTotal,
                    doId = deliveryorder.Id,
                    doNo = internnotedetail.DONo,
                    doDate = internnotedetail.DODate,
                    ProductName = internnotedetail.ProductName,
                    supplierCode = internnote.SupplierCode,
                    createdBy = internnote.CreatedBy,
                    billNo = deliveryorder.BillNo,
                    paymentBill = deliveryorder.PaymentBill,
                    doCurrencyRate = deliveryorder.DOCurrencyRate,
                    paymentType = deliveryorder.PaymentMethod,
                    paymentDoc = data1 == null ? "-" : data1.ExpenditureNoteNo,
                    paymentDate = data1 == null ? new DateTime(1970, 1, 1) : data1.ExpenditureDate
                });
            }
            //return list;
            var Query1 = list.AsQueryable();

            var gcn = dbContext.GarmentCorrectionNotes.Where(x => x.IsDeleted == false).AsEnumerable();
            var result = (from i in Query1
                          join f in gcn on i.doId equals f.DOId into cn
                          from gcnn in cn.DefaultIfEmpty()
                          select new GarmentInternNoteReportViewModel
                          {
                              inNo = i.inNo,
                              iNDate = i.iNDate,
                              currencyCode = i.currencyCode,
                              supplierName = i.supplierName,
                              invoiceNo = i.invoiceNo,
                              invoiceDate = i.invoiceDate,
                              NPN = i.NPN,
                              VatNo = i.VatNo,
                              priceTotal = i.priceTotal,
                              doId = i.doId,
                              doNo = i.doNo,
                              doDate = i.doDate,
                              ProductName = i.ProductName,
                              supplierCode = i.supplierCode,
                              createdBy = i.createdBy,
                              billNo = i.billNo,
                              paymentBill = i.paymentBill,
                              doCurrencyRate = i.doCurrencyRate,
                              paymentType = i.paymentType,
                              paymentDoc = i.paymentDoc,
                              paymentDate = i.paymentDate,
                              cnNo = gcnn == null ? "-" : gcnn.CorrectionNo,
                              cnDate = gcnn == null ? new DateTime(1970, 1, 1) : gcnn.CorrectionDate,
                              cnAmount = gcnn == null ? 0 : gcnn.TotalCorrection
                          });

            return result.ToList();
        }

        public MemoryStream GenerateExcelIn(string no, string supplierCode, string curencyCode, string invoiceNo, string npn, string doNo, string billNo, string paymentBill, DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportInternNote(no, supplierCode, curencyCode, invoiceNo, npn, doNo, billNo, paymentBill, dateFrom, dateTo, offset, 1, int.MaxValue);
            Query = Query.OrderByDescending(b => b.iNDate).ToList();
            Query = Query.OrderByDescending(c => c.invoiceNo).ToList();
            DataTable result = new DataTable();

            //result.Columns.Add(new DataColumn());
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota Intern", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Supplier", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Surat Jalan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon Kecil", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nominal", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Nota Pajak", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Faktur", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Rate", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Nota Koreksi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nominal Koreksi", DataType = typeof(Decimal) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe Bayar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pembayaran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Pembayaran", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "poserialnumber", DataType = typeof(String) });

            if (Query.Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", 0, "", "", "", "", 0, "", "", 0, "","","");
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string date = item.iNDate == null ? "-" : item.iNDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string paymentdate = item.paymentDate == new DateTime(1970, 1, 1) ? "-" : item.paymentDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string DueDate = item.paymentDueDate == null ? "-" : item.paymentDueDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MM yyyy", new CultureInfo("id-ID"));
                    string invoDate = item.invoiceDate == null ? "-" : item.invoiceDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string Dodate = item.doDate == null ? "-" : item.doDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //var price = item.priceTotal.ToString();
                    string priceTotal = string.Format("{0:N2}", item.priceTotal);
                    //double totalHarga = item.pricePerDealUnit * item.quantity;
                    string corrDate = item.cnDate == new DateTime(1970, 1, 1) ? "-" : item.cnDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string corrAmt = string.Format("{0:N2}", item.cnAmount);
                    //result.Rows.Add(index, item.inNo, date, item.currencyCode, item.supplierName, item.paymentMethod, item.paymentType, DueDate, item.invoiceNo, invoDate, item.doNo, Dodate, item.pOSerialNumber, item.rONo, item.productCode, item.productName, item.quantity, item.uOMUnit, item.pricePerDealUnit, totalHarga);
                    result.Rows.Add(index, item.inNo, date, item.supplierCode, item.supplierName, item.invoiceNo, invoDate, item.doNo, Dodate, item.billNo, item.paymentBill, priceTotal, item.NPN, item.VatNo, item.ProductName, item.currencyCode, item.doCurrencyRate, item.cnNo, corrDate, corrAmt, item.paymentType, item.paymentDoc, paymentdate);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>> { (new KeyValuePair<DataTable, string>(result, "Nota Intern")) }, true);

        }

        public List<GarmentInternalNoteDto> BankExpenditureReadInternalNotes(string currencyCode, int supplierId)
        {
            var query = dbContext.GarmentInternNotes.Where(entity => entity.Position == PurchasingGarmentExpeditionPosition.AccountingAccepted || entity.Position == PurchasingGarmentExpeditionPosition.CashierAccepted);

            if (!string.IsNullOrEmpty(currencyCode))
                query = query.Where(entity => entity.CurrencyCode.Contains(currencyCode));

            if (supplierId > 0)
                query = query.Where(entity => entity.SupplierId.GetValueOrDefault() == supplierId);

            var result = new List<GarmentInternalNoteDto>();
            if (query.Count() > 0)
            {
                var internalNotes = query.Select(entity => new { entity.Id, entity.INNo, entity.INDate, entity.SupplierId, entity.SupplierCode, entity.SupplierName, entity.CurrencyCode, entity.CurrencyId, entity.CurrencyRate });
                var internalNoteIds = internalNotes.Select(entity => entity.Id).ToList();
                var internalNoteItems = dbContext.GarmentInternNoteItems.Where(entity => internalNoteIds.Contains(entity.GarmentINId)).Select(entity => new { entity.Id, entity.GarmentINId, entity.InvoiceId });
                var internalNoteItemIds = internalNoteItems.Select(entity => entity.Id).ToList();
                var internalNoteDetails = dbContext.GarmentInternNoteDetails.Where(entity => internalNoteItemIds.Contains(entity.GarmentItemINId)).Select(entity => new { entity.Id, entity.DOId, entity.GarmentItemINId, entity.EPOId, entity.PaymentDueDate });

                var invoiceIds = internalNoteItems.Select(internalNoteItem => internalNoteItem.InvoiceId).ToList();

                var garmentInvoices = dbContext.GarmentInvoices.Where(entity => invoiceIds.Contains(entity.Id) && !entity.DPPVATIsPaid).Select(entity => new { entity.Id, entity.InvoiceNo, entity.InvoiceDate, entity.UseVat, entity.IsPayVat, entity.UseIncomeTax, entity.IsPayTax, entity.TotalAmount, entity.IncomeTaxRate }).ToList();
                var garmentInvoiceIds = garmentInvoices.Select(element => element.Id).ToList();
                var garmentInvoiceItems = dbContext.GarmentInvoiceItems.Where(entity => garmentInvoiceIds.Contains(entity.InvoiceId)).Select(entity => new { entity.Id, entity.InvoiceId, entity.DeliveryOrderId });
                var garmentInvoiceItemIds = garmentInvoiceItems.Select(element => element.Id).ToList();
                var garmentInvoiceDetails = dbContext.GarmentInvoiceDetails.Where(entity => garmentInvoiceItemIds.Contains(entity.InvoiceItemId)).Select(entity => new { entity.Id, entity.InvoiceItemId, entity.ProductName }).ToList();
                var deliveryOrderIds = garmentInvoiceItems.Select(element => element.DeliveryOrderId).ToList();
                var deliveryOrders = dbContext.GarmentDeliveryOrders.Where(entity => deliveryOrderIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.DONo, entity.PaymentBill, entity.BillNo, entity.TotalAmount,entity.DOCurrencyRate }).ToList();

                var corrections = dbContext.GarmentCorrectionNotes.Where(entity => deliveryOrderIds.Contains(entity.DOId)).Select(entity => new { entity.Id, entity.TotalCorrection, entity.CorrectionType, entity.DOId });
                var correctionIds = corrections.Select(element => element.Id).ToList();
                var correctionItems = dbContext.GarmentCorrectionNoteItems.Where(entity => correctionIds.Contains(entity.GCorrectionId)).Select(entity => new { entity.Id, entity.PricePerDealUnitAfter, entity.Quantity, entity.GCorrectionId });


                var externalPurchaseOrderIds = internalNoteDetails.Select(detail => detail.EPOId).ToList();
                var externalPurchaseOrders = dbContext.GarmentExternalPurchaseOrders.Where(entity => externalPurchaseOrderIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.PaymentMethod }).ToList();
                var externalPurchaseOrderItems = dbContext.GarmentExternalPurchaseOrderItems.Where(entity => externalPurchaseOrderIds.Contains(entity.GarmentEPOId)).Select(entity => new { entity.POId, entity.Id, entity.GarmentEPOId }).ToList();
                var internalPurchaseOrderIds = externalPurchaseOrderItems.Select(externalPurchaseOrderItem => (long)externalPurchaseOrderItem.POId).ToList();
                var internalPurchaseOrderItems = dbContext.GarmentInternalPurchaseOrderItems.Where(internalPurchaseOrderItem => internalPurchaseOrderIds.Contains(internalPurchaseOrderItem.GPOId)).Select(entity => new { entity.CategoryId, entity.CategoryName, entity.Id, entity.GPOId }).ToList();

                foreach (var internalNote in internalNotes)
                {
                    var selectedInternalNoteItems = internalNoteItems.Where(element => element.GarmentINId == internalNote.Id).ToList();
                    var internalNoteInvoiceIds = selectedInternalNoteItems.Select(item => item.InvoiceId).ToList();

                    var internalNoteDOIds = garmentInvoiceItems.Where(element => internalNoteInvoiceIds.Contains(element.InvoiceId)).Select(element => element.DeliveryOrderId).ToList();
                    var internalNoteDeliveryOrders = deliveryOrders.Where(element => internalNoteDOIds.Contains(element.Id)).ToList();
                    var selectedInternalNoteItemIds = selectedInternalNoteItems.Where(element => element.GarmentINId == internalNote.Id).Select(element => element.Id).ToList();
                    var selectedEPOIds = internalNoteDetails.Where(element => selectedInternalNoteItemIds.Contains(element.GarmentItemINId)).Select(element => element.EPOId).ToList();
                    var externalPurchaseOrder = externalPurchaseOrders.FirstOrDefault(element => selectedEPOIds.Contains(element.Id));
                    var selectedIPOIds = externalPurchaseOrderItems.Where(element => selectedEPOIds.Contains(element.GarmentEPOId)).Select(element => (long)element.POId).ToList();
                    var internalPurchaseOrderItem = internalPurchaseOrderItems.FirstOrDefault(element => selectedIPOIds.Contains(element.GPOId));

                    //var internalNoteInvoices = garmentInvoices.Where(invoice => internalNoteInvoiceIds.Contains(invoice.Id)).ToList();
                    var internalNoteInvoices = garmentInvoices.Where(invoice => internalNoteInvoiceIds.Contains(invoice.Id)).Select(s =>
                    {
                        var invoiceItems = garmentInvoiceItems.Where(element => element.InvoiceId == s.Id).ToList();
                        var invoiceItemIds = invoiceItems.Select(element => element.Id).ToList();
                        var invoiceDetails = garmentInvoiceDetails.Where(element => invoiceItemIds.Contains(element.InvoiceItemId)).ToList();
                        var selectedDeliveryOrderIds = invoiceItems.Select(item => item.DeliveryOrderId).ToList();
                        var selectedDeliveryOrders = deliveryOrders.Where(element => selectedDeliveryOrderIds.Contains(element.Id)).ToList();
                        var selectedDeliveryOrderLists = selectedDeliveryOrders.Select(element => new DeliveryOrderDto(element.DONo, element.TotalAmount, element.PaymentBill, element.BillNo, element.Id,element.DOCurrencyRate.GetValueOrDefault())).ToList();


                        var selectedCorrections = corrections.Where(element => selectedDeliveryOrderIds.Contains(element.DOId)).ToList();
                        var correctionAmount = selectedCorrections.Sum(element =>
                        {
                            var selectedCorrectionItems = correctionItems.Where(item => item.GCorrectionId == element.Id);

                            var total = 0.0;
                            if (element.CorrectionType.ToUpper() == "RETUR")
                                total = (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity);
                            else
                                total = (double)element.TotalCorrection;

                            return total;
                        });

                        int.TryParse(internalPurchaseOrderItem.CategoryId, out var categoryId);
                        
                        return new InternalNoteInvoiceDto(s.InvoiceNo, s.InvoiceDate, string.Join("\n", invoiceDetails.Select(element => $"- {element.ProductName}").Distinct()), categoryId, internalPurchaseOrderItem.CategoryName, externalPurchaseOrder.PaymentMethod, (int)s.Id, string.Join("\n", selectedDeliveryOrders.Select(element => $"- {element.DONo}").Distinct()), string.Join("\n", selectedDeliveryOrders.Select(element => $"- {element.BillNo}").Distinct()), string.Join("\n", selectedDeliveryOrders.Select(element => $"- {element.PaymentBill}").Distinct()), s.TotalAmount, s.UseVat, s.IsPayVat, s.UseIncomeTax, s.IsPayTax, s.IncomeTaxRate, correctionAmount,selectedDeliveryOrderLists);

                    }).ToList();

                    if (internalNoteInvoices.Count > 0)
                    {
                        result.Add(new GarmentInternalNoteDto((int)internalNote.Id, internalNote.INNo, internalNote.INDate, internalNote.INDate, (int)internalNote.SupplierId, internalNote.SupplierName, internalNote.SupplierCode, (int)internalNote.CurrencyId, internalNote.CurrencyCode, internalNote.CurrencyRate, internalNoteInvoices));
                    }
                }

                //result = internalNotes
                //    .Select(internalNote =>
                //    {
                //        var internalNoteInvoiceIds = internalNote.Items.Select(item => item.InvoiceId).ToList();
                //        var internalNoteInvoices = garmentInvoices.Where(invoice => internalNoteInvoiceIds.Contains(invoice.Id)).ToList();

                //        return new GarmentInternalNoteDto(internalNote, internalNoteInvoices);
                //    })
                //    .ToList();
            }

            return result;
        }
        /// <summary>
        /// Optimized for BankExpenditureReadInternalNotes
        /// Indexing:
        /// GarmentInterNoteItems : GarmentINId
        /// GarmentInterNotes : Position
        /// GarmentInterNoteDetails : GarmentItemINId
        /// GarmentInvoices: DPPVATIsPaid
        /// GarmentInvoiceItems : InvoiceId
        /// GarmentInvoiceDetails : InvoiceItemId
        /// </summary>
        /// <param name="currencyId"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public List<GarmentInternalNoteDto> BankExpenditureReadInternalNotesOptimized(int currencyId, int supplierId)
        {
            var query = dbContext.GarmentInternNotes.Include(entity=> entity.Items).ThenInclude(item => item.Details).Where(entity => entity.Position == PurchasingGarmentExpeditionPosition.AccountingAccepted || entity.Position == PurchasingGarmentExpeditionPosition.CashierAccepted);

            if (currencyId > 0)
                query = query.Where(entity => entity.CurrencyId.GetValueOrDefault() == currencyId);

            if (supplierId > 0)
                query = query.Where(entity => entity.SupplierId.GetValueOrDefault() == supplierId);

            var result = new List<GarmentInternalNoteDto>();
            if (query.Count() > 0)
            {
                var internalNotes = query.Select(entity => new { entity.Id, entity.INNo, entity.INDate, entity.SupplierId, entity.SupplierCode, entity.SupplierName, entity.CurrencyCode, entity.CurrencyId, entity.CurrencyRate });               
                var internalNoteItems = query.SelectMany(entity=> entity.Items).Select(entity => new { entity.Id, entity.GarmentINId, entity.InvoiceId });
                var internalNoteDetails = query.SelectMany(entity=> entity.Items).SelectMany(item=> item.Details).Select(entity => new { entity.Id, entity.DOId, entity.GarmentItemINId, entity.EPOId, entity.PaymentDueDate });

                var garmentInvoices = dbContext.GarmentInvoices.Include(entity => entity.Items).ThenInclude(item => item.Details)
                    .Join(internalNoteItems,
                    garmentInvoice => garmentInvoice.Id,
                    invoice => invoice.InvoiceId,
                    (entity, invoice) => new
                    {
                        entity.Id,
                        entity.InvoiceNo,
                        entity.InvoiceDate,
                        entity.UseVat,
                        entity.IsPayVat,
                        entity.UseIncomeTax,
                        entity.IsPayTax,
                        entity.TotalAmount,
                        entity.IncomeTaxRate,
                        entity.DPPVATIsPaid,
                        Items = entity.Items.Select(item => new
                        {
                            item.Id,
                            item.InvoiceId,
                            item.DeliveryOrderId,
                            Details = item.Details.Select(detail => new
                            {
                                detail.Id,
                                detail.InvoiceItemId,
                                detail.ProductName
                            })
                        })
                    }
                    ).Where(entity=> !entity.DPPVATIsPaid).ToList();
                var garmentInvoiceItems = garmentInvoices.SelectMany(entity=> entity.Items).Select(entity => new { entity.Id, entity.InvoiceId, entity.DeliveryOrderId });
                var garmentInvoiceDetails = garmentInvoices.SelectMany(entity => entity.Items).SelectMany(item=> item.Details).Select(entity => new { entity.Id, entity.InvoiceItemId, entity.ProductName });

                var deliveryOrders = dbContext.GarmentDeliveryOrders//.Where(entity => deliveryOrderIds.Contains(entity.Id))
                    .Join(garmentInvoiceItems,
                    deliveryOrder => deliveryOrder.Id,
                    garmentInvoice => garmentInvoice.DeliveryOrderId,
                    (entity, garmentInvoice) =>
                    new { entity.Id, entity.DONo, entity.PaymentBill, entity.BillNo ,entity.TotalAmount,entity.DOCurrencyRate}
                    ).ToList();

                var externalPurchaseOrders = dbContext.GarmentExternalPurchaseOrders.Include(entity=> entity.Items)
                    .Join(internalNoteDetails,
                    Epo => Epo.Id,
                    internalNoteDetail => internalNoteDetail.EPOId,
                    (entity, internalNoteDetail) => new
                    {
                        entity.Id,
                        entity.PaymentMethod,
                        Items = entity.Items.Select(item=> new {
                            item.POId,
                            item.Id,
                            item.GarmentEPOId
                        })
                    }).ToList();
                var externalPurchaseOrderItems = externalPurchaseOrders.SelectMany(entity=> entity.Items).Select(entity => new { entity.POId, entity.Id, entity.GarmentEPOId });
                var internalPurchaseOrderItems = dbContext.GarmentInternalPurchaseOrderItems
                    .Join(externalPurchaseOrderItems,
                    internalPO => internalPO.GPOId,
                    externalPO => (long)externalPO.POId,
                    (entity, externalPO) => new {
                        entity.CategoryId,
                        entity.CategoryName,
                        entity.Id,
                        entity.GPOId
                    }).ToList();

                var deliveryOrderIds = garmentInvoiceItems.Select(element => element.DeliveryOrderId).ToList();
                //var deliveryOrders = dbContext.GarmentDeliveryOrders.Where(entity => deliveryOrderIds.Contains(entity.Id)).Select(entity => new { entity.Id, entity.DONo, entity.PaymentBill, entity.BillNo }).ToList();

                var corrections = dbContext.GarmentCorrectionNotes.Where(entity => deliveryOrderIds.Contains(entity.DOId)).Select(entity => new { entity.Id, entity.TotalCorrection, entity.CorrectionType, entity.DOId });
                var correctionIds = corrections.Select(element => element.Id).ToList();
                var correctionItems = dbContext.GarmentCorrectionNoteItems.Where(entity => correctionIds.Contains(entity.GCorrectionId)).Select(entity => new { entity.Id, entity.PricePerDealUnitAfter, entity.Quantity, entity.GCorrectionId });

                foreach (var internalNote in internalNotes)
                {
                    var internalNoteInvoiceIds = internalNoteItems.Select(item => item.InvoiceId).ToList();

                    var internalNoteDOIds = garmentInvoiceItems.Where(element => internalNoteInvoiceIds.Contains(element.InvoiceId)).Select(element => element.DeliveryOrderId).ToList();
                    var internalNoteDeliveryOrders = deliveryOrders.Where(element => internalNoteDOIds.Contains(element.Id)).ToList();
                    var selectedInternalNoteItemIds = internalNoteItems.Where(element => element.GarmentINId == internalNote.Id).Select(element => element.Id).ToList();
                    var selectedEPOIds = internalNoteDetails.Where(element => selectedInternalNoteItemIds.Contains(element.GarmentItemINId)).Select(element => element.EPOId).ToList();
                    var externalPurchaseOrder = externalPurchaseOrders.FirstOrDefault(element => selectedEPOIds.Contains(element.Id));
                    var selectedIPOIds = externalPurchaseOrderItems.Where(element => selectedEPOIds.Contains(element.GarmentEPOId)).Select(element => (long)element.POId).ToList();
                    var internalPurchaseOrderItem = internalPurchaseOrderItems.FirstOrDefault(element => selectedIPOIds.Contains(element.GPOId));
                    //var internalNoteInvoices = garmentInvoices.Where(invoice => internalNoteInvoiceIds.Contains(invoice.Id)).ToList();
                    var internalNoteInvoices = garmentInvoices.Where(invoice => internalNoteInvoiceIds.Contains(invoice.Id)).Select(s =>
                    {
                        var invoiceItems = garmentInvoiceItems.Where(element => element.InvoiceId == s.Id).ToList();
                        var invoiceItemIds = invoiceItems.Select(element => element.Id).ToList();
                        var invoiceDetails = garmentInvoiceDetails.Where(element => invoiceItemIds.Contains(element.InvoiceItemId)).ToList();
                        var selectedDeliveryOrderIds = invoiceItems.Select(item => item.DeliveryOrderId).ToList();
                        var selectedDeliveryOrders = deliveryOrders.Where(element => selectedDeliveryOrderIds.Contains(element.Id)).ToList();
                        var selectedDeliveryOrderLists = selectedDeliveryOrders.Select(element => new DeliveryOrderDto(element.DONo,element.TotalAmount,element.PaymentBill,element.BillNo,element.Id,element.DOCurrencyRate.GetValueOrDefault())).ToList();
                        var productNames = string.Join("\n", invoiceDetails.Select(element => $"- {element.ProductName}").Distinct());
                        int.TryParse(internalPurchaseOrderItem.CategoryId, out var categoryId);
                        var selectedCorrections = corrections.Where(element => selectedDeliveryOrderIds.Contains(element.DOId)).ToList();

                        var correctionAmount = selectedCorrections.Sum(element =>
                        {
                            var selectedCorrectionItems = correctionItems.Where(item => item.GCorrectionId == element.Id);

                            var total = 0.0;
                            if (element.CorrectionType.ToUpper() == "RETUR")
                                total = (double)selectedCorrectionItems.Sum(item => item.PricePerDealUnitAfter * item.Quantity);
                            else
                                total = (double)element.TotalCorrection;

                            return total;
                        });
                        return new InternalNoteInvoiceDto(s.InvoiceNo,
                            s.InvoiceDate,
                            //string.Join("\n", invoiceDetails.Select(element => $"- {element.ProductName}").Distinct()),
                            productNames,
                            categoryId,
                            internalPurchaseOrderItem.CategoryName,
                            externalPurchaseOrder.PaymentMethod,
                            (int)s.Id,
                            string.Join("\n", selectedDeliveryOrders.Select(element => $"- {element.DONo}").Distinct()),
                            string.Join("\n", selectedDeliveryOrders.Select(element => $"- {element.BillNo}").Distinct()),
                            string.Join("\n", selectedDeliveryOrders.Select(element => $"- {element.PaymentBill}").Distinct()),
                            s.TotalAmount,
                            s.UseVat,
                            s.IsPayVat,
                            s.UseIncomeTax,
                            s.IsPayTax,
                            s.IncomeTaxRate, 
                            correctionAmount, selectedDeliveryOrderLists);

                    }).ToList();

                    if (internalNoteInvoices.Count > 0)
                    {
                        result.Add(new GarmentInternalNoteDto((int)internalNote.Id, internalNote.INNo, internalNote.INDate, internalNote.INDate, (int)internalNote.SupplierId, internalNote.SupplierName, internalNote.SupplierCode, (int)internalNote.CurrencyId, internalNote.CurrencyCode, internalNote.CurrencyRate, internalNoteInvoices));
                    }
                }

                //result = internalNotes
                //    .Select(internalNote =>
                //    {
                //        var internalNoteInvoiceIds = internalNote.Items.Select(item => item.InvoiceId).ToList();
                //        var internalNoteInvoices = garmentInvoices.Where(invoice => internalNoteInvoiceIds.Contains(invoice.Id)).ToList();

                //        return new GarmentInternalNoteDto(internalNote, internalNoteInvoices);
                //    })
                //    .ToList();
            }

            return result;
        }

        public async Task<int> BankExpenditureUpdateIsPaidInternalNoteAndInvoiceNote(bool dppVATIsPaid, int bankExpenditureNoteId, string bankExpenditureNoteNo, string internalNoteIds = "[]", string invoiceNoteIds = "[]")
        {
            var parsedInternalNoteIds = JsonConvert.DeserializeObject<List<long>>(internalNoteIds);
            var parsedInvoiceNoteIds = JsonConvert.DeserializeObject<List<long>>(invoiceNoteIds);

            var internalNotes = dbContext
                .GarmentInternNotes
                .Where(entity => parsedInternalNoteIds.Contains(entity.Id))
                .ToList()
                .Select(element =>
                {
                    element.DPPVATIsPaid = dppVATIsPaid;
                    EntityExtension.FlagForUpdate(element, element.CreatedBy, USER_AGENT);

                    return element;
                })
                .ToList();
            dbContext.GarmentInternNotes.UpdateRange(internalNotes);

            var invoiceNotes = dbContext
                .GarmentInvoices
                .Where(entity => parsedInvoiceNoteIds.Contains(entity.Id))
                .ToList()
                .Select(element =>
                {
                    element.DPPVATIsPaid = dppVATIsPaid;
                    EntityExtension.FlagForUpdate(element, element.CreatedBy, USER_AGENT);

                    return element;
                })
                .ToList();
            dbContext.GarmentInvoices.UpdateRange(invoiceNotes);

            var existingInvoiceNoteIds = invoiceNotes.Select(element => element.Id).ToList();

            var invoiceNoteItems = dbContext
                .GarmentInvoiceItems
                .Where(entity => existingInvoiceNoteIds.Contains(entity.InvoiceId))
                .ToList();

            foreach (var invoiceNoteItem in invoiceNoteItems)
            {
                var deliveryOrder = dbContext.GarmentDeliveryOrders.FirstOrDefault(entity => entity.Id == invoiceNoteItem.DeliveryOrderId);
                var invoiceNote = invoiceNotes.FirstOrDefault(element => element.Id == invoiceNoteItem.InvoiceId);

                if (deliveryOrder != null)
                {
                    if (dppVATIsPaid)
                    {
                        var amount = 0.0;
                        var currencyAmount = 0.0;
                        var vatAmount = 0.0;
                        var currencyVATAmount = 0.0;
                        var incomeTaxAmount = 0.0;
                        var currencyIncomeTaxAmount = 0.0;

                        if (invoiceNote.CurrencyCode == "IDR")
                        {
                            amount = invoiceNoteItem.TotalAmount;
                            if (invoiceNote.IsPayVat)
                            {
                                vatAmount = invoiceNoteItem.TotalAmount * 0.1;
                            }

                            if (invoiceNote.IsPayTax)
                            {
                                incomeTaxAmount = invoiceNoteItem.TotalAmount * invoiceNote.IncomeTaxRate / 100;
                            }
                        }
                        else
                        {
                            amount = invoiceNoteItem.TotalAmount * deliveryOrder.DOCurrencyRate.GetValueOrDefault();
                            currencyAmount = invoiceNoteItem.TotalAmount;
                            if (invoiceNote.IsPayVat)
                            {
                                vatAmount = amount * 0.1;
                                currencyVATAmount = invoiceNoteItem.TotalAmount * 0.1;
                            }

                            if (invoiceNote.IsPayTax)
                            {
                                incomeTaxAmount = amount * invoiceNote.IncomeTaxRate / 100;
                                currencyIncomeTaxAmount = invoiceNoteItem.TotalAmount * invoiceNote.IncomeTaxRate / 100;
                            }
                        }

                        amount = amount + vatAmount - incomeTaxAmount;
                        currencyAmount = currencyAmount + currencyVATAmount - currencyIncomeTaxAmount;
                        await _garmentDebtBalanceService.UpdateFromBankExpenditureNote((int)invoiceNoteItem.DeliveryOrderId, new BankExpenditureNoteFormDto(bankExpenditureNoteId, bankExpenditureNoteNo, amount, currencyAmount));
                    }
                    else
                    {
                        await _garmentDebtBalanceService.EmptyBankExpenditureNote((int)deliveryOrder.Id);
                    }

                }
            }

            return dbContext.SaveChanges();
        }
        #endregion

        public DPPVATBankExpenditureNoteViewModel GetInvoice(long InvoiceId)
        {
            string financeUri = "dpp-vat-bank-expenditure-notes/invoice";

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = httpClient.GetAsync($"{APIEndpoint.Finance}{financeUri}/{InvoiceId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                DPPVATBankExpenditureNoteViewModel viewModel;
                if (result.GetValueOrDefault("data") == null)
                {
                    viewModel = null;
                }
                else
                {
                    viewModel = JsonConvert.DeserializeObject<DPPVATBankExpenditureNoteViewModel>(result.GetValueOrDefault("data").ToString());
                }
                return viewModel;
            }
            else
            {
                return null;
            }
        }
    }
}
