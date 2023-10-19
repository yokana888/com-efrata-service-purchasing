using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitReceiptNoteViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates.GarmentUnitReceiptNotePDFTemplates
{
    public class GarmentUnitReceiptNotePDFTemplate
    {
        public static MemoryStream GeneratePdfTemplate(IServiceProvider serviceProvider, GarmentUnitReceiptNoteViewModel viewModel)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 15);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            Document document = new Document(PageSize.A4, 40, 40, 40, 40);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            IGarmentDeliveryOrderFacade garmentDeliveryOrderFacade = (IGarmentDeliveryOrderFacade)serviceProvider.GetService(typeof(IGarmentDeliveryOrderFacade));
            var garmentDeliveryOrder = viewModel.DOId>0 ?garmentDeliveryOrderFacade.ReadById((int)viewModel.DOId) : null;

            IdentityService identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            #region Header

            string titleString = "BON PENERIMAAN BARANG";
            Paragraph title = new Paragraph(titleString, header_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);

            string companyNameString = "PT EFRATA GARMINDO UTAMA";
            Paragraph companyName = new Paragraph(companyNameString, bold_font) { Alignment = Element.ALIGN_LEFT };
            document.Add(companyName);

            string companyAddressString = "Jl. Merapi No.23 Blok E1, Desa/Kelurahan Banaran," + "\n" + "Kec. Grogol, Kab. Sukoharjo, Provinsi Jawa Tengah" + "\n" + "Kode Pos: 57552, Telp: 02711740888";
            Paragraph companyAddress = new Paragraph(companyAddressString, normal_font) { Alignment = Element.ALIGN_LEFT };
            companyAddress.SpacingAfter = 10f;
            document.Add(companyAddress);

            LineSeparator lineSeparator = new LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1);
            document.Add(lineSeparator);

            #endregion

            #region Identity

            var receiptDate = viewModel.ReceiptDate.GetValueOrDefault().ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
            var receivedFrom = viewModel.URNType == "PROSES" ? "PROSES" : viewModel.URNType == "GUDANG LAIN" ? "GUDANG LAIN" : viewModel.URNType == "GUDANG SISA" ? "GUDANG SISA" : viewModel.URNType == "SISA SUBCON" ? "SISA SUBCON" : viewModel.Supplier.Name;
            var receiptNo = viewModel.URNType == "PROSES" ? viewModel.DRNo : viewModel.URNType == "GUDANG LAIN" ? viewModel.UENNo : viewModel.URNType == "GUDANG SISA" ? viewModel.ExpenditureNo : viewModel.URNType == "SISA SUBCON" ? viewModel.UENNo : viewModel.DONo;
            var date = garmentDeliveryOrder == null ? receiptDate : garmentDeliveryOrder.DODate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
            PdfPTable tableIdentity = new PdfPTable(4);
            tableIdentity.SetWidths(new float[] { 3f, 4f, 3f, 4f });
            PdfPCell cellIdentityContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

            cellIdentityContentLeft.Phrase = new Phrase("Tgl. Terima", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + date, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Tgl. Bon Penerimaan", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + receiptDate, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Diterima dari", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + receivedFrom, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Bagian", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.Unit.Name, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Dasar Penerimaan", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + receiptNo, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("No.", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.URNNo, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingAfter = 10f;
            tableIdentity.SpacingBefore = 20f;
            document.Add(tableIdentity);

            #endregion

            #region TableContent

            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            PdfPTable tableContent = new PdfPTable(5);
            tableContent.SetWidths(new float[] { 1f, 7f, 3f, 3f, 5f });

            cellCenter.Phrase = new Phrase("No", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Nama Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Jumlah", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Satuan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Keterangan", bold_font);
            tableContent.AddCell(cellCenter);

            int indexItem = 0;
            foreach (var item in viewModel.Items)
            {
                cellCenter.Phrase = new Phrase((++indexItem).ToString(), normal_font);
                tableContent.AddCell(cellCenter);

                cellLeft.Phrase = new Phrase($"{item.Product.Code} - {item.Product.Name}", normal_font);
                tableContent.AddCell(cellLeft);

                cellCenter.Phrase = new Phrase($"{item.ReceiptQuantity}", normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(item.Uom.Unit, normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase($"{item.POSerialNumber}; {item.RONo}; {item.Article}; {item.Product.Remark}", normal_font);
                tableContent.AddCell(cellCenter);
            }


            PdfPCell cellContent = new PdfPCell(tableContent);
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 20f;
            document.Add(tableContent);

            Paragraph remark = new Paragraph("Keterangan : " + viewModel.Remark, normal_font);
            document.Add(remark);

            #endregion

            #region TableSignature

            PdfPTable tableSignature = new PdfPTable(2);

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellSignatureContent.Phrase = new Phrase("", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase($"Sukoharjo, {viewModel.ReceiptDate.GetValueOrDefault().ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Mengetahui\n\n\n\n\n\n\n(  _____________________  )", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Yang Menerima\n\n\n\n\n\n\n(  _____________________  )", normal_font);
            tableSignature.AddCell(cellSignatureContent);

            PdfPCell cellSignature = new PdfPCell(tableSignature); // dont remove
            tableSignature.ExtendLastRow = false;
            tableSignature.SpacingBefore = 20f;
            tableSignature.SpacingAfter = 20f;
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
