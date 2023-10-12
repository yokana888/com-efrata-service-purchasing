using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Newtonsoft.Json;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.Utilities;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class UnitPaymentOrderPDFTemplate
    {
        public MemoryStream Generate(UnitPaymentOrder model, IUnitPaymentOrderFacade facade, SupplierViewModel supplier, int clientTimeZoneOffset = 7, string userName = null)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

            Document document = new Document(PageSize.A5.Rotate(), 15, 15, 15, 15);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            PdfPCell cellLeftNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellCenterNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            PdfPCell cellCenterTopNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_TOP };
            PdfPCell cellRightNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
            PdfPCell cellJustifyNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_JUSTIFIED };
            PdfPCell cellJustifyAllNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL };

            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            PdfPCell cellRightMerge = new PdfPCell() { Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_TOP, Padding = 5 };
            PdfPCell cellLeftMerge = new PdfPCell() { Border = Rectangle.NO_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_TOP, Padding = 5 };

            #region Header

            PdfPTable tableHeader = new PdfPTable(3);
            tableHeader.SetWidths(new float[] { 1f, 1f, 1f });

            PdfPCell cellHeaderContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellHeaderContentLeft.AddElement(new Phrase("PT. EFRATA GARMINDO UTAMA", header_font));
            cellHeaderContentLeft.AddElement(new Phrase("Kel. Banaran, Kec. Grogol, Kab.Sukoharjo, Jawa Tengah" + "\n" + "57552" + "\n" + "Telp (+62 271)719911, (+62 21)2900977", normal_font));
            tableHeader.AddCell(cellHeaderContentLeft);

            PdfPCell cellHeaderContentCenter = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellHeaderContentCenter.AddElement(new Paragraph("NOTA KREDIT", header_font) { Alignment = Element.ALIGN_CENTER });
            cellHeaderContentCenter.AddElement(new Paragraph(model.PaymentMethod.ToUpper().Trim().Equals("KREDIT") ? "" : model.PaymentMethod, normal_font) { Alignment = Element.ALIGN_CENTER });
            tableHeader.AddCell(cellHeaderContentCenter);

            PdfPCell cellHeaderContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER };
            cellHeaderContentRight.AddElement(new Phrase("FM-PB-00-06-014/R2", normal_font));
            cellHeaderContentRight.AddElement(new Phrase($"SUKOHARJO, {model.Date.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font));
            cellHeaderContentRight.AddElement(new Phrase($"( {model.SupplierCode} ) {model.SupplierName}", normal_font));
            cellHeaderContentRight.AddElement(new Phrase(model.SupplierAddress, normal_font));
            /* tambahan */
            if (supplier.npwp == "" || supplier.npwp == null)
            {
                supplier.npwp = "00.000.000.0-000.000";
                //cellHeaderContentRight.AddElement(new Phrase("NPWP / NIK : 00.000.000.0-000.000", normal_font));
            }
            //else
            //{
                cellHeaderContentRight.AddElement(new Phrase($"NPWP / NIK : {supplier.npwp}", normal_font));
            //}
            /* tambahan */
            tableHeader.AddCell(cellHeaderContentRight);

            PdfPCell cellHeader = new PdfPCell(tableHeader);
            tableHeader.ExtendLastRow = false;
            tableHeader.SpacingAfter = 15f;
            document.Add(tableHeader);

            #endregion

            #region Identity

            PdfPTable tableIdentity = new PdfPTable(3);
            tableIdentity.SetWidths(new float[] { 1.5f, 4.5f, 3f });

            cellLeftNoBorder.Phrase = new Phrase("Nota Pembelian", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.CategoryName}", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($"Nomor   {model.UPONo}", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("Untuk", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.DivisionName}", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingAfter = 15f;
            document.Add(tableIdentity);

            #endregion

            #region TableContent

            PdfPTable tableContent = new PdfPTable(10);
            tableContent.SetWidths(new float[] { 1.3f, 5f, 3f, 1.5f, 3.5f, 1.5f, 3.5f, 4f, 4f, 3f });

            cellCenter.Phrase = new Phrase("No.", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Nama Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Jumlah", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Harga Satuan", bold_font);
            cellCenter.Colspan = 2;
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Harga Total", bold_font);
            cellCenter.Colspan = 2;
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Nomor Order", bold_font);
            cellCenter.Colspan = 0;
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Nomor Bon Unit", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Unit", bold_font);
            tableContent.AddCell(cellCenter);

            int no = 0;
            double jumlah = 0;

            List<DateTimeOffset> DueDates = new List<DateTimeOffset>() { model.DueDate };
            List<DateTimeOffset> UnitReceiptNoteDates = new List<DateTimeOffset>() { DateTimeOffset.MinValue };

            //foreach (var f in new float[15])
            foreach (var item in model.Items)
            {
                var unitReceiptNote = facade.GetUnitReceiptNote(item.URNId);
                var unitReceiptNoteDate = unitReceiptNote.ReceiptDate;
                UnitReceiptNoteDates.Add(unitReceiptNoteDate);

                //var UnitName = unitReceiptNote.UnitId == "50" ? "WEAVING" : unitReceiptNote.UnitName;
                var UnitName = "";
                var unitId = unitReceiptNote.UnitId;
                if (unitId == "50")
                {
                    UnitName = "WEAVING";
                }
                else if (unitId == "35")
                {
                    UnitName = "SPINNING 1";
                }
                else
                {
                    UnitName = unitReceiptNote.UnitName;
                }

                foreach (var detail in item.Details)
                {
                    var PaymentDueDays = facade.GetExternalPurchaseOrder(detail.EPONo).PaymentDueDays;
                    DueDates.Add(unitReceiptNoteDate.AddDays(Double.Parse(PaymentDueDays ?? "0")));

                    cellCenter.Phrase = new Phrase($"{++no}", normal_font);
                    tableContent.AddCell(cellCenter);

                    cellLeft.Phrase = new Phrase(detail.ProductName, normal_font);
                    tableContent.AddCell(cellLeft);

                    cellCenter.Phrase = new Phrase(string.Format("{0:n2}", detail.ReceiptQuantity) +$" {detail.UomUnit}", normal_font);
                    tableContent.AddCell(cellCenter);

                    cellLeftMerge.Phrase = new Phrase($"{model.CurrencyCode}", normal_font);
                    tableContent.AddCell(cellLeftMerge);

                    cellRightMerge.Phrase = new Phrase(string.Format("{0:n4}", detail.PricePerDealUnit), normal_font);
                    tableContent.AddCell(cellRightMerge);

                    cellLeftMerge.Phrase = new Phrase($"{model.CurrencyCode}", normal_font);
                    tableContent.AddCell(cellLeftMerge);

                    cellRightMerge.Phrase = new Phrase(string.Format("{0:n2}", detail.PriceTotal), normal_font);
                    tableContent.AddCell(cellRightMerge);

                    cellCenter.Phrase = new Phrase($"{detail.PRNo}", normal_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{item.URNNo}", normal_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{UnitName}", normal_font);
                    tableContent.AddCell(cellCenter);

                    jumlah += detail.PriceTotal;
                }
            }

            PdfPCell cellContent = new PdfPCell(tableContent);
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 10f;
            document.Add(tableContent);

            #endregion

            #region Tax

            PdfPTable tableTax = new PdfPTable(3);
            tableTax.SetWidths(new float[] { 1f, 0.3f, 1f });

            var ppn = jumlah * (model.VatRate/100);
            var total = jumlah + (model.UseVat ? ppn : 0);
            var pph = jumlah * model.IncomeTaxRate / 100;
            var totalWithPph = total - pph;

            var withoutIncomeTax = true;


            if(model.UseIncomeTax && model.IncomeTaxBy == "Supplier")
            {
                withoutIncomeTax = false;
            }

            if (withoutIncomeTax)
            {
                tableTax.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            }
            else
            {
                PdfPTable tableIncomeTax = new PdfPTable(3);
                tableIncomeTax.SetWidths(new float[] { 5f, 2f, 3f });

                tableIncomeTax.AddCell(new PdfPCell(new Phrase(" ", normal_font)) { Border = Rectangle.NO_BORDER, Colspan = 3 });

                cellLeftNoBorder.Phrase = new Phrase($"PPh {model.IncomeTaxName} {model.IncomeTaxRate} %", normal_font);
                tableIncomeTax.AddCell(cellLeftNoBorder);

                cellLeftNoBorder.Phrase = new Phrase($":   {model.CurrencyCode}", normal_font);
                tableIncomeTax.AddCell(cellLeftNoBorder);

                cellRightNoBorder.Phrase = new Phrase($"{pph.ToString("n", new CultureInfo("id-ID"))}", normal_font);
                tableIncomeTax.AddCell(cellRightNoBorder);

                cellLeftNoBorder.Phrase = new Phrase("Jumlah dibayar Ke Supplier", normal_font);
                tableIncomeTax.AddCell(cellLeftNoBorder);

                cellLeftNoBorder.Phrase = new Phrase($":   {model.CurrencyCode}", normal_font);
                tableIncomeTax.AddCell(cellLeftNoBorder);

                cellRightNoBorder.Phrase = new Phrase($"{totalWithPph.ToString("n", new CultureInfo("id-ID"))}", normal_font);
                tableIncomeTax.AddCell(cellRightNoBorder);

                tableTax.AddCell(new PdfPCell(tableIncomeTax) { Border = Rectangle.NO_BORDER });
            }

            tableTax.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });

            PdfPTable tableVat = new PdfPTable(2);

            cellJustifyAllNoBorder.Phrase = new Phrase($"Jumlah . . . . . . . . . . . . . . .   {model.CurrencyCode}", normal_font);
            tableVat.AddCell(cellJustifyAllNoBorder);

            cellRightNoBorder.Phrase = new Phrase($"{jumlah.ToString("n", new CultureInfo("id-ID"))}", normal_font);
            tableVat.AddCell(cellRightNoBorder);

            if (model.UseVat)
            {
                cellJustifyAllNoBorder.Phrase = new Phrase($"PPn {model.VatRate} % . . . . . . . . . . . . . .   {model.CurrencyCode}", normal_font);
                tableVat.AddCell(cellJustifyAllNoBorder);
            }
            else
            {
                cellLeftNoBorder.Phrase = new Phrase(string.Concat("PPn %"), normal_font);
                tableVat.AddCell(cellLeftNoBorder);
            }

            cellRightNoBorder.Phrase = new Phrase(model.UseVat ? $"{ppn.ToString("n", new CultureInfo("id-ID"))}" : "-", normal_font);
            tableVat.AddCell(cellRightNoBorder);

            cellJustifyAllNoBorder.Phrase = new Phrase($"T O T A L. . . . . . . . . . . . . .   {model.CurrencyCode}", normal_font);
            tableVat.AddCell(cellJustifyAllNoBorder);

            cellRightNoBorder.Phrase = new Phrase($"{total.ToString("n", new CultureInfo("id-ID"))}", normal_font);
            tableVat.AddCell(cellRightNoBorder);

            tableTax.AddCell(new PdfPCell(tableVat) { Border = Rectangle.NO_BORDER });

            PdfPCell taxCell = new PdfPCell(tableTax);
            tableTax.ExtendLastRow = false;
            tableTax.SpacingAfter = 15f;
            document.Add(tableTax);

            #endregion

            Paragraph paragraphTerbilang = new Paragraph($"Terbilang : {NumberToTextIDN.terbilang(!withoutIncomeTax ? totalWithPph : total)} {model.CurrencyDescription.ToLower()}", bold_font) { SpacingAfter = 15f };
            document.Add(paragraphTerbilang);

            #region Footer

            PdfPTable tableFooter = new PdfPTable(3);
            tableFooter.SetWidths(new float[] { 1f, 0.3f, 1f });

            PdfPTable tableFooterLeft = new PdfPTable(3);
            tableFooterLeft.SetWidths(new float[] { 5f, 0.4f, 4.6f });

            cellLeftNoBorder.Phrase = new Phrase("Perjanjian Pembayaran", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($"{DueDates.Max().ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Invoice", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($"{model.InvoiceNo ?? "-"}, {model.InvoiceDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("No PIB", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($"{model.PibNo ?? "-"}", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Ket.", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($"{model.Remark ?? "-"}", normal_font);
            tableFooterLeft.AddCell(cellLeftNoBorder);

            tableFooter.AddCell(new PdfPCell(tableFooterLeft) { Border = Rectangle.NO_BORDER });

            tableFooter.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });

            PdfPTable tableFooterRight = new PdfPTable(3);
            tableFooterRight.SetWidths(new float[] { 5f, 0.5f, 6.8f });

            cellLeftNoBorder.Phrase = new Phrase("Barang Datang", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            var maxUnitReceiptNoteDate = UnitReceiptNoteDates.Max();
            cellLeftNoBorder.Phrase = new Phrase($"{maxUnitReceiptNoteDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Nomor Faktur Pajak PPN", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($"{model.VatNo ?? "-"}", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Pembayaran", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($"{model.PaymentMethod ?? "-"}", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            tableFooter.AddCell(new PdfPCell(tableFooterRight) { Border = Rectangle.NO_BORDER });

            PdfPCell taxFooter = new PdfPCell(tableFooter);
            tableFooter.ExtendLastRow = false;
            tableFooter.SpacingAfter = 20f;
            document.Add(tableFooter);

            #endregion

            #region TableSignature

            //PdfPTable tableSignature = new PdfPTable(5);

            //cellCenterTopNoBorder.Phrase = new Paragraph("Staff Pembelian\n\n\n\n\n\n\n\n(                                   )", normal_font);
            //tableSignature.AddCell(cellCenterTopNoBorder);
            //cellCenterTopNoBorder.Phrase = new Paragraph("Manager Pembelian\n\n\n\n\n\n\n\n(                                   )", normal_font);
            //tableSignature.AddCell(cellCenterTopNoBorder);
            //cellCenterTopNoBorder.Phrase = new Paragraph("Verifikasi\n\n\n\n\n\n\n\n(                                   )", normal_font);
            //tableSignature.AddCell(cellCenterTopNoBorder);
            //cellCenterTopNoBorder.Phrase = new Paragraph("Manager Keuangan\n\n\n\n\n\n\n\n(                                   )", normal_font);
            //tableSignature.AddCell(cellCenterTopNoBorder);
            //cellCenterTopNoBorder.Phrase = new Paragraph("Anggaran\n\n\n\n\n\n\n\n(                                   )", normal_font);
            //tableSignature.AddCell(cellCenterTopNoBorder);

            //PdfPCell cellSignature = new PdfPCell(tableSignature);
            //tableSignature.ExtendLastRow = false;
            //document.Add(tableSignature);

            #endregion

            #region signature
            PdfPTable tableSignature = new PdfPTable(5);
            tableSignature.SetWidths(new float[] { 5f, 5f, 4f, 4f, 3.5f });

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellSignatureContent.Phrase = new Phrase("", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("", normal_font);
            cellSignatureContent.Colspan = 3;
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Colspan = 0;
            cellSignatureContent.Phrase = new Phrase(" ", normal_font);
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Colspan = 2;
            cellSignatureContent.Phrase = new Phrase("Disetujui,", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Mengetahui,", normal_font);
            cellSignatureContent.Colspan = 2;
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Colspan = 0;
            cellSignatureContent.Phrase = new Phrase("Dibuat,", normal_font);
            tableSignature.AddCell(cellSignatureContent);

            if (total > 3000000)
            {
                cellSignatureContent.Colspan = 0;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n(Manager Akuntansi & Keu)", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n(Direktur Akuntansi & Keu)", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Colspan = 2;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n(Manager Pembelian)", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n(     Staff     )", normal_font);
                tableSignature.AddCell(cellSignatureContent);
            }
            else
            {
                cellSignatureContent.Colspan = 2;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n(Manager Akuntansi & Keu)", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Colspan = 2;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n(   Manager Pembelian    )", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Colspan = 0;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n(     Staff     )", normal_font);
                tableSignature.AddCell(cellSignatureContent);
            }


            //PdfPCell cellSignatureContentDir = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER,VerticalAlignment=Element.ALIGN_TOP };

            //cellSignatureContentDir.Phrase = new Phrase("\n\n\n\n\n\n\n    Direktur Keuangan   ", bold_font3);
            //tableSignature.AddCell(cellSignatureContentDir);
            //cellSignatureContentDir.Colspan = 4;
            //cellSignatureContentDir.Phrase = new Phrase("", bold_font3);
            //tableSignature.AddCell(cellSignatureContentDir);

            PdfPCell cellSignature = new PdfPCell(tableSignature); // dont remove
            tableSignature.ExtendLastRow = false;
            //tableSignature.SpacingBefore = 10f;
            //tableSignature.SpacingAfter = 20f;
            document.Add(tableSignature);
            #endregion

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}
