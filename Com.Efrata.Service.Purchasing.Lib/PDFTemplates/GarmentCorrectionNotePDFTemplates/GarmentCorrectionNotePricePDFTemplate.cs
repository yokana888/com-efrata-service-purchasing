using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates.GarmentCorrectionNotePDFTemplates
{
    public class GarmentCorrectionNotePricePDFTemplate
    {
        public static MemoryStream Generate(GarmentCorrectionNote model, IServiceProvider serviceProvider, int clientTimeZoneOffset = 7, string userName = "")
        {
            IGarmentDeliveryOrderFacade garmentDeliveryOrderFacade = (IGarmentDeliveryOrderFacade)serviceProvider.GetService(typeof(IGarmentDeliveryOrderFacade));
            IGarmentInternalPurchaseOrderFacade garmentInternalPurchaseOrderFacade = (IGarmentInternalPurchaseOrderFacade)serviceProvider.GetService(typeof(IGarmentInternalPurchaseOrderFacade));

            Document document = new Document(PageSize.A4, 10, 10, 10, 10);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            PdfPCell cellLeftNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellCenterNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            PdfPCell cellCenterTopNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_TOP };
            PdfPCell cellRightNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
            PdfPCell cellJustifyNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_JUSTIFIED };
            PdfPCell cellJustifyAllNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL };

            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            var garmentDeliveryOrder = garmentDeliveryOrderFacade.ReadById((int)model.DOId);

            #region Header
            //string addressString = "PT. EFRATA GARMINDO UTAMA\n" +
            //    "JL. Merapi No.23\n" +
            //    "Banaran, Grogol, Kab. Sukoharjo\n" +
            //    "Jawa Tengah 57552 - INDONESIA\n" +
            //    "PO.BOX 166 Solo 57100\n" +
            //    "Telp. (0271) 740888, 714400\n" +
            //    "Fax. (0271) 735222, 740777";

            string addressString = "PT EFRATA GARMINDO UTAMA" + "\n" + "Banaran, Grogol, Sukoharjo" + "\n" + "Jawa Tengah 57552 - INDONESIA" + "\n" + "Telp (+62 271)719911, (+62 21)2900977";
            Paragraph addressParagraph = new Paragraph(8f, addressString, bold_font) { Alignment = Element.ALIGN_LEFT };
            document.Add(addressParagraph);
            Paragraph headerParagraph = new Paragraph("NOTA KOREKSI", header_font) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 15f };
            document.Add(headerParagraph);
            #endregion

            #region Identity

            PdfPTable tableIdentity = new PdfPTable(2);
            tableIdentity.SetWidths(new float[] { 1f, 1f });

            PdfPTable tableIdentityLeft = new PdfPTable(2);
            tableIdentityLeft.SetWidths(new float[] { 3f, 5f });

            cellLeftNoBorder.Phrase = new Phrase("No. Nota Koreksi", normal_font);
            tableIdentityLeft.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.CorrectionNo}", normal_font);
            tableIdentityLeft.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("Kode Supplier", normal_font);
            tableIdentityLeft.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.SupplierCode}", normal_font);
            tableIdentityLeft.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("Nama Supplier", normal_font);
            tableIdentityLeft.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.SupplierName}", normal_font);
            tableIdentityLeft.AddCell(cellLeftNoBorder);

            PdfPTable tableIdentityRight = new PdfPTable(2);
            tableIdentityRight.SetWidths(new float[] { 3f, 5f });
            cellLeftNoBorder.Phrase = new Phrase("Tanggal", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.CorrectionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("No. Surat Jalan", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.DONo}", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("Tanggal Surat Jalan", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {garmentDeliveryOrder.DODate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);

            PdfPCell cellIdentityLeft = new PdfPCell(tableIdentityLeft) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            tableIdentity.AddCell(cellIdentityLeft);
            PdfPCell cellIdentityRight = new PdfPCell(tableIdentityRight) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            tableIdentity.AddCell(cellIdentityRight);

            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingAfter = 15f;
            document.Add(tableIdentity);

            #endregion

            #region TableContent

            PdfPTable tableContent = new PdfPTable(10);
            tableContent.SetWidths(new float[] { 1.1f, 1.2f, 1f, 1f, 0.9f, 0.7f, 1.2f, 0.8f, 1.2f, 1.3f });

            foreach (var columnName in new List<string>{ "Plan PO", "Artikel", "Kode Barang", "Nama Barang", "Jumlah SJ (Koreksi)", "Satuan", "Harga Satuan", "Jumlah Koreksi", "Harga/Satuan (Koreksi)", "Total Harga" })
            {
                cellCenter.Phrase = new Phrase(columnName, bold_font);
                tableContent.AddCell(cellCenter);
            }

            Dictionary<string, decimal> dictionaryUnitAmount = new Dictionary<string, decimal>();
            var totalAmount = 0m;
            foreach (var item in model.Items)
            {
                var garmentInternalPurchaseOrder = garmentInternalPurchaseOrderFacade.ReadById((int)item.POId);

                cellLeft.Phrase = new Phrase(item.POSerialNumber, normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(garmentInternalPurchaseOrder.Article, normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(item.ProductCode, normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(item.ProductName, normal_font);
                tableContent.AddCell(cellLeft);

                cellRight.Phrase = new Phrase(((double)item.Quantity).ToString(), normal_font);
                tableContent.AddCell(cellRight);

                cellLeft.Phrase = new Phrase(item.UomIUnit, normal_font);
                tableContent.AddCell(cellLeft);

                cellRight.Phrase = new Phrase(item.PricePerDealUnitBefore.ToString("n", new CultureInfo("id-ID")), normal_font);
                tableContent.AddCell(cellRight);

                cellRight.Phrase = new Phrase("-", normal_font);
                tableContent.AddCell(cellRight);

                var isTotalCorrection = (model.CorrectionType ?? "").ToUpper() == "HARGA TOTAL";

                cellRight.Phrase = new Phrase(isTotalCorrection ? "-" : (item.PricePerDealUnitAfter - item.PricePerDealUnitBefore).ToString("n", new CultureInfo("id-ID")), normal_font);
                tableContent.AddCell(cellRight);

                var totalHarga = isTotalCorrection ? (item.PriceTotalAfter - item.PriceTotalBefore) : (item.Quantity * (item.PricePerDealUnitAfter - item.PricePerDealUnitBefore));
                totalAmount += totalHarga;

                if (dictionaryUnitAmount.ContainsKey(garmentInternalPurchaseOrder.UnitCode))
                {
                    dictionaryUnitAmount[garmentInternalPurchaseOrder.UnitCode] += totalHarga;
                }
                else
                {
                    dictionaryUnitAmount.Add(garmentInternalPurchaseOrder.UnitCode, totalHarga);
                }

                cellRight.Phrase = new Phrase(totalHarga.ToString("n", new CultureInfo("id-ID")), normal_font);
                tableContent.AddCell(cellRight);
            }

            PdfPCell cellContent = new PdfPCell(tableContent);
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 10f;
            document.Add(tableContent);

            #endregion

            #region Footer

            PdfPTable tableFooter = new PdfPTable(2);
            tableFooter.SetWidths(new float[] { 1f, 1f });

            PdfPTable tableFooterLeft = new PdfPTable(2);
            tableFooterLeft.SetWidths(new float[] { 3f, 5f });

            foreach (var unitAmount in dictionaryUnitAmount)
            {
                cellLeftNoBorder.Phrase = new Phrase($"Total {unitAmount.Key}", normal_font);
                tableFooterLeft.AddCell(cellLeftNoBorder);
                cellLeftNoBorder.Phrase = new Phrase($":   {unitAmount.Value.ToString("n", new CultureInfo("id-ID"))}", normal_font);
                tableFooterLeft.AddCell(cellLeftNoBorder);
            }

            PdfPTable tableFooterRight = new PdfPTable(2);
            tableFooterRight.SetWidths(new float[] { 3f, 5f });

            cellLeftNoBorder.Phrase = new Phrase("Total Amount", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {totalAmount.ToString("n", new CultureInfo("id-ID"))}", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("Mata Uang", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.CurrencyCode}", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("Total Harga Pokok (Rp)", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {(totalAmount * (decimal)garmentDeliveryOrder.DOCurrencyRate).ToString("n", new CultureInfo("id-ID"))}", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            PdfPCell cellFooterLeft = new PdfPCell(tableFooterLeft) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            tableFooter.AddCell(cellFooterLeft);
            PdfPCell cellFooterRight = new PdfPCell(tableFooterRight) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            tableFooter.AddCell(cellFooterRight);

            PdfPCell cellFooter = new PdfPCell(tableFooter);
            tableFooter.ExtendLastRow = false;
            tableFooter.SpacingAfter = 15f;
            document.Add(tableFooter);

            #endregion

            #region TableSignature

            var signer = new List<string> { "Administrasi", "Staff Pembelian", "Verifikasi" };

            PdfPTable tableSignature = new PdfPTable(signer.Count);

            foreach (var columnName in signer)
            {
                cellCenter.Phrase = new Phrase(columnName, bold_font);
                tableSignature.AddCell(cellCenter);
            }

            for (int i = 0; i < signer.Count; i++)
            {
                cellCenter.Phrase = new Phrase("\n\n\n\n\n\n(Nama & Tanggal)", bold_font);
                tableSignature.AddCell(cellCenter);
            }

            PdfPCell cellSignature = new PdfPCell(tableSignature); // dont remove
            tableSignature.ExtendLastRow = false;
            tableSignature.SpacingBefore = 20f;
            tableSignature.SpacingAfter = 10f;
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
