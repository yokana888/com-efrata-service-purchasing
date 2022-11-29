using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.GarmentReports
{
    public class ROFeatureFacade : IROFeatureFacade
    {
        private readonly PurchasingDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<GarmentDeliveryOrder> dbSet;

        public ROFeatureFacade(IServiceProvider serviceProvider, PurchasingDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<GarmentDeliveryOrder>();
        }

        public Tuple<List<ROFeatureViewModel>, int> GetROReport(int offset, string RO, int page, int size, string Order)
        {
            //var Query = GetStockQuery(tipebarang, unitcode, dateFrom, dateTo, offset);
            //Query = Query.OrderByDescending(x => x.SupplierName).ThenBy(x => x.Dono);
            List<ROFeatureViewModel> Data = GetRO(RO, offset);
            Data = Data.OrderByDescending(x => x.KodeBarang).ToList();
            //int TotalData = Data.Count();
            return Tuple.Create(Data, Data.Count());
        }

        public List<ROFeatureViewModel> GetRO(string RO, int offset)
        {

            List<ROFeatureViewModel> final = new List<ROFeatureViewModel>();

            var penerimaan = (from a in dbContext.GarmentUnitReceiptNoteItems
                              join b in dbContext.GarmentUnitReceiptNotes on a.URNId equals b.Id
                              join c in dbContext.GarmentPurchaseRequests on a.RONo equals c.RONo
                              where a.RONo == (string.IsNullOrWhiteSpace(RO) ? a.RONo : RO) && a.IsDeleted == false && b.IsDeleted == false
                              select new ROFeatureTemp
                              {
                                  KodeBarang = a.ProductCode,
                                  NamaBarang = a.ProductName,
                                  PO = a.POSerialNumber,
                                  Article = c.Article.ToUpper(),
                                  QtyTerima = Math.Round((double)(a.ReceiptQuantity * a.Conversion), 2),
                                  QtyKeluar = 0,
                                  RONo = a.RONo,
                                  UomMasuk = a.SmallUomUnit,
                                  UomKeluar = a.SmallUomUnit,
                                  Unitcode = b.UnitCode
                              }).ToList();
            //var pengeluaran = (from a in dbContext.GarmentUnitExpenditureNoteItems
            //                   join b in dbContext.GarmentUnitExpenditureNotes on a.UENId equals b.Id
            //                   join c in dbContext.GarmentUnitDeliveryOrderItems on a.UnitDOItemId equals c.Id
            //                   join d in dbContext.GarmentUnitDeliveryOrders on c.UnitDOId equals d.Id
            //                   join e in dbContext.GarmentPurchaseRequests on a.RONo equals e.RONo
            //                   join f in penerimaan on c.URNNo equals f.NoBukti
            //                   //join c in dbContext.GarmentInternalPurchaseOrderItems on a.POItemId equals c.Id
            //                   //join d in dbContext.GarmentInternalPurchaseOrders on c.GPOId equals d.Id
            //                   where a.RONo == (string.IsNullOrWhiteSpace(RO) ? a.RONo : RO) && a.IsDeleted == false && b.IsDeleted == false
            //                   select new ROFeatureTemp
            //                   {
            //                       KodeBarang = a.ProductCode,
            //                       NoBukti = b.UENNo,
            //                       NamaBarang = a.ProductName,
            //                       Article = d.Article.ToUpper(),
            //                       PO = a.POSerialNumber,
            //                       QtyTerima = 0,
            //                       QtyKeluar = a.Quantity,
            //                       RONo = a.RONo,
            //                       UomMasuk = f.UomMasuk,
            //                       UomKeluar = f.UomKeluar
            //                   }).ToList();

            //var report = penerimaan.ToList();
            var datas = penerimaan.GroupBy(a => new { a.KodeBarang, a.NamaBarang, a.Article, a.PO, a.RONo, a.UomMasuk, a.UomKeluar, a.Unitcode }, (key, groupdata) => new ROFeatureTemp
            {
                KodeBarang = key.KodeBarang,
                NamaBarang = key.NamaBarang,
                Article = key.Article,
                PO = key.PO,
                QtyTerima = groupdata.Sum(x => x.QtyTerima),
                QtyKeluar = groupdata.Sum(x => x.QtyKeluar),
                RONo = key.RONo,
                UomMasuk = key.UomMasuk,
                UomKeluar = key.UomKeluar,
                Unitcode = key.Unitcode
            }).ToList();
            //var datas = (from a in report
            //            group a by new { a.KodeBarang, a.NamaBarang, a.NoBukti, a.Article, a.PO, a.RONo, a.UomMasuk, a.UomKeluar, a.Unitcode} into groupdata
            //            select new ROFeatureTemp
            //            {
            //                KodeBarang = groupdata.Key.KodeBarang,
            //                NoBukti = groupdata.Key.NoBukti,
            //                NamaBarang = groupdata.Key.NamaBarang,
            //                Article = groupdata.Key.Article,
            //                PO = groupdata.Key.PO,
            //                QtyTerima = groupdata.Sum(x => x.QtyTerima),
            //                QtyKeluar = groupdata.Sum(x => x.QtyKeluar),
            //                RONo = groupdata.Key.RONo,
            //                UomMasuk = groupdata.Key.UomMasuk,
            //                UomKeluar = groupdata.Key.UomKeluar,
            //                Unitcode = groupdata.Key.Unitcode
            //            }).ToList();

            foreach (var data in datas)
            {

                var masuk = (from a in dbContext.GarmentUnitReceiptNotes
                            join b in dbContext.GarmentUnitReceiptNoteItems on a.Id equals b.URNId
                            where b.ProductCode == data.KodeBarang && b.ProductName == data.NamaBarang && b.POSerialNumber == data.PO && b.RONo == data.RONo && a.UnitCode == data.Unitcode
                            select new {
                                a.ReceiptDate,
                                a.URNNo,
                                b.POSerialNumber,
                                b.ProductCode,
                                b.ProductName,
                                Qty = Math.Round(b.ReceiptQuantity * b.Conversion, 2),
                                b.SmallUomUnit,
                                b.RONo
                            }).GroupBy(x=> new { x.ReceiptDate, x.POSerialNumber, x.ProductCode, x.ProductName, x.RONo, x.SmallUomUnit, x.URNNo },(key, group) => new {
                                ReceiptDate = key.ReceiptDate,
                                URNNo = key.URNNo,
                                POSerialNumber = key.POSerialNumber,
                                ProductCode = key.ProductCode,
                                ProductName = key.ProductName,
                                Qty = group.Sum(x=>x.Qty),
                                SmallUomUnit = key.SmallUomUnit,
                                RONo = key.RONo

                            }).ToList();
                var keluar = (from a in dbContext.GarmentUnitExpenditureNotes
                              join b in dbContext.GarmentUnitExpenditureNoteItems on a.Id equals b.UENId
                              join c in dbContext.GarmentUnitDeliveryOrderItems on b.UnitDOItemId equals c.Id
                              join d in dbContext.GarmentUnitDeliveryOrders on c.UnitDOId equals d.Id
                              //where b.ProductCode == data.KodeBarang && b.POSerialNumber == data.PO && b.RONo == data.RONo
                              where b.ProductCode == data.KodeBarang && b.POSerialNumber == data.PO && b.RONo == data.RONo && a.UnitSenderCode == data.Unitcode
                              select new
                             {
                                 a.ExpenditureDate,
                                 RO = b.RONo,
                                 a.UENNo,
                                 b.POSerialNumber,
                                 b.ProductCode,
                                 data.NamaBarang,
                                 Qty = b.Quantity,
                                 UomKeluar = b.UomUnit,
                                 d.RONo,
                                 c.Quantity,
                                 c.UomUnit,
                                 d.UnitDONo,
                                 a.ExpenditureType
                             }).GroupBy(x=> new { x.ExpenditureDate, x.RO, x.UENNo, x.POSerialNumber, x.ProductCode, x.NamaBarang, x.UomKeluar, x.RONo, x.UomUnit, x.UnitDONo, x.ExpenditureType },(key, group) => new {
                                 ExpenditureDate = key.ExpenditureDate,
                                 RO = key.RONo,
                                 UENNo = key.UENNo,
                                 POSerialNumber = key.POSerialNumber,
                                 ProductCode = key.ProductCode,
                                 ProductName = key.NamaBarang,
                                 Qty = group.Sum(x=>x.Qty),
                                 UomKeluar = key.UomKeluar,
                                 RONo = key.RONo,
                                 Quantity = group.Sum(x=>x.Quantity),
                                 UomUnit = key.UomUnit,
                                 UnitDONo = key.UnitDONo,
                                 ExpenditureType = key.ExpenditureType

                             }).ToList();

                List<RODetailMasukViewModel> masukdata = new List<RODetailMasukViewModel>();
                List<RODetailViewModel> keluardata = new List<RODetailViewModel>();

                foreach (var i in masuk) {
                    masukdata.Add(new RODetailMasukViewModel
                    {
                        ReceiptDate = i.ReceiptDate.DateTime,
                        KodeBarang = i.ProductCode,
                        NamaBarang = i.ProductName,
                        PO = i.POSerialNumber,
                        RONo = i.RONo,
                        NoBukti = i.URNNo,
                        Qty = (double)i.Qty,
                        Uom = i.SmallUomUnit,
                    });
                }

                foreach (var i in keluar) {
                    keluardata.Add(new RODetailViewModel
                    {
                        KodeBarang = i.ProductCode,
                        NamaBarang = i.ProductName,
                        PO = i.POSerialNumber,
                        NoBukti = i.UENNo,
                        Qty = i.Qty,
                        Uom = i.UomKeluar,
                        UomDO = i.UomUnit,
                        JumlahDO = i.Quantity,
                        RO = i.RONo,
                        RONo = i.RO,
                        TanggalKeluar = i.ExpenditureDate.DateTime,
                        Tipe = i.ExpenditureType,
                        UnitDONo = i.UnitDONo
                    });
                }

                final.Add(new ROFeatureViewModel
                {
                    KodeBarang = data.KodeBarang,
                    NamaBarang = data.NamaBarang,
                    Article = data.Article,
                    PO = data.PO,
                    QtyKeluar = Math.Round(keluardata.Sum(x=>x.Qty), 2),
                    QtyTerima = Math.Round(masukdata.Sum(x => x.Qty), 2),
                    RONo = data.RONo,
                    UomKeluar = data.UomKeluar,
                    UomMasuk = data.UomMasuk,
                    Unitcode = data.Unitcode,
                    items = new ROItemViewModel
                    {
                        Masuk = masukdata,
                        Keluar = keluardata
                    }
                });
            }

            return final;
        }
    }
}
