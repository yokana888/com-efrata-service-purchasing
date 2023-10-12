using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentCorrectionNoteModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates.GarmentCorrectionNotePDFTemplates
{
    public class GarmentCorrectionNoteQuantityPphPDFTemplate
    {
        public static MemoryStream Generate(GarmentCorrectionNote model, IServiceProvider serviceProvider, int clientTimeZoneOffset = 7, string userName = "")
        {
            IGarmentDeliveryOrderFacade garmentDeliveryOrderFacade = (IGarmentDeliveryOrderFacade)serviceProvider.GetService(typeof(IGarmentDeliveryOrderFacade));
            IGarmentInvoice garmentInvoiceFacade = (IGarmentInvoice)serviceProvider.GetService(typeof(IGarmentInvoice));

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

            var deliveryOrder = garmentDeliveryOrderFacade.ReadById((int)model.DOId);
            var invoice = garmentInvoiceFacade.ReadByDOId((int)model.DOId);

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
            Paragraph headerParagraph = new Paragraph("NOTA KOREKSI PAJAK", header_font) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 15f };
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
            cellLeftNoBorder.Phrase = new Phrase("No. Nota Pajak", normal_font);
            tableIdentityLeft.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.NKPH}", normal_font);
            tableIdentityLeft.AddCell(cellLeftNoBorder);

            PdfPTable tableIdentityRight = new PdfPTable(2);
            cellLeftNoBorder.Phrase = new Phrase("Kode Supplier", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.SupplierCode}", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("Nama Supplier", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($":   {model.SupplierName}", normal_font);
            tableIdentityRight.AddCell(cellLeftNoBorder);

            PdfPCell cellIdentityLeft = new PdfPCell(tableIdentityLeft) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            tableIdentity.AddCell(cellIdentityLeft);
            PdfPCell cellIdentityRight = new PdfPCell(tableIdentityRight) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            tableIdentity.AddCell(cellIdentityRight);

            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingAfter = 5f;
            document.Add(tableIdentity);

            #endregion

            #region TableContent

            var columnHeaders = new List<string> { "No. Surat Jalan", "Tgl. Surat Jalan", "Tgl. Jatuh Tempo", "No. Invoice", "Nama Barang", "Rate", $"Total PPH ({model.CurrencyCode})" };

            PdfPTable tableContent = new PdfPTable(columnHeaders.Count);
            tableContent.SetWidths(new float[] { 1.1f, 1.2f, 1f, 1f, 1.1f, 0.5f, 1.2f });

            foreach (var columnName in columnHeaders)
            {
                cellCenter.Phrase = new Phrase(columnName, bold_font);
                tableContent.AddCell(cellCenter);
            }

            Dictionary<string, decimal> dictionaryUnitAmount = new Dictionary<string, decimal>();
            var totalAmountPPH = 0m;
            foreach (var item in model.Items)
            {
                var deliveryOrderItem = deliveryOrder.Items.First(i => i.Details.Any(d => d.Id == item.DODetailId));

                cellLeft.Phrase = new Phrase(deliveryOrder.DONo, normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(deliveryOrder.DODate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(deliveryOrder.DODate.AddDays(deliveryOrderItem.PaymentDueDays).ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
                tableContent.AddCell(cellLeft);

                if (invoice!=null)
                {
                    cellLeft.Phrase = new Phrase(invoice.InvoiceNo, normal_font);
                    tableContent.AddCell(cellLeft);
                }
                else
                {
                    cellLeft.Phrase = new Phrase("", normal_font);
                    tableContent.AddCell(cellLeft);
                }

                cellLeft.Phrase = new Phrase(item.ProductName, normal_font);
                tableContent.AddCell(cellLeft);

                cellRight.Phrase = new Phrase(((double)model.IncomeTaxRate).ToString("n", new CultureInfo("id-ID")), normal_font);
                tableContent.AddCell(cellRight);

                decimal totalPPH;
                
                totalPPH = model.IncomeTaxRate / 100 * item.PricePerDealUnitAfter * item.Quantity;
                
                totalAmountPPH += totalPPH;

                cellRight.Phrase = new Phrase(totalPPH.ToString("n", new CultureInfo("id-ID")), normal_font);
                tableContent.AddCell(cellRight);
            }

            PdfPCell cellRightMerge = new PdfPCell()
            {
                Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_TOP,
                Padding = 5,
                Colspan = columnHeaders.Count - 1
            };

            cellRightMerge.Phrase = new Phrase($"Total PPH ({model.CurrencyCode})", normal_font);
            tableContent.AddCell(cellRightMerge);

            cellRight.Phrase = new Phrase(totalAmountPPH.ToString("n", new CultureInfo("id-ID")), normal_font);
            tableContent.AddCell(cellRight);

            cellRightMerge.Phrase = new Phrase("Total PPH (IDR)", normal_font);
            tableContent.AddCell(cellRightMerge);

            cellRight.Phrase = new Phrase((totalAmountPPH * (decimal)deliveryOrder.DOCurrencyRate).ToString("n", new CultureInfo("id-ID")), normal_font);
            tableContent.AddCell(cellRight);

            PdfPCell cellContent = new PdfPCell(tableContent);
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 60f;
            document.Add(tableContent);

            #endregion

            #region TableSignature

            var signer = new List<string> { "Staff Pembelian", "Administrasi", "Keuangan", "Pembukuan" };

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
