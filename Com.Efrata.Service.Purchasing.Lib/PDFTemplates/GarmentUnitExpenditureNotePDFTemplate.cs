using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitExpenditureNoteViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class GarmentUnitExpenditureNotePDFTemplate
    {
        public static MemoryStream GeneratePdfTemplate(IServiceProvider serviceProvider, GarmentUnitExpenditureNoteViewModel viewModel)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 15);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font normal_font1 = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);

            Document document = new Document(PageSize.A4, 40, 40, 40, 40);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            IGarmentUnitDeliveryOrderFacade garmentUnitDeliveryOrderFacade = (IGarmentUnitDeliveryOrderFacade)serviceProvider.GetService(typeof(IGarmentUnitDeliveryOrderFacade));
            var garmentUnitDeliveryOrder = garmentUnitDeliveryOrderFacade.ReadById((int)viewModel.UnitDOId);

            IdentityService identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));

            #region Header


            string formString = "";
            Paragraph form = new Paragraph(formString, bold_font) { Alignment = Element.ALIGN_RIGHT };
            document.Add(form);

            string titleString = "BON PENGELUARAN BARANG";
            Paragraph title = new Paragraph(titleString, header_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);

            string companyNameString = "PT EFRATA GARMINDO UTAMA";
            Paragraph companyName = new Paragraph(companyNameString, bold_font) { Alignment = Element.ALIGN_LEFT };
            document.Add(companyName);

            string companyAddressString = "Jl. Merapi No.23 Blok E1, Desa/Kelurahan Banaran," + "\n" + "Kec. Grogol, Kab. Sukoharjo, Provinsi Jawa Tengah" + "\n" + "Kode Pos: 57552, Telp: 02711740888";
            Paragraph companyAddress = new Paragraph(companyAddressString, normal_font1) { Alignment = Element.ALIGN_LEFT };
            companyAddress.SpacingAfter = 10f;
            document.Add(companyAddress);

            #endregion

            #region Identity

            PdfPTable tableIdentity = new PdfPTable(4);
            tableIdentity.SetWidths(new float[] { 3f, 4f, 3f, 4f });
            PdfPCell cellIdentityContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

            cellIdentityContentLeft.Phrase = new Phrase("No. Bukti Pengeluaran", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.UENNo, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Tgl. Bukti Pengeluaran", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + DateTimeOffset.Now.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Tgl. Pengeluaran", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.ExpenditureDate.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Gudang", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.Storage.name, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Tujuan Pengeluaran", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.ExpenditureType, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Konveksi", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.UnitSender.Name, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Dasar Pengeluaran", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.UnitDONo, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("RO Tujuan", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + garmentUnitDeliveryOrder.RONo, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("No Artikel", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + garmentUnitDeliveryOrder.Article, normal_font);
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
            tableContent.SetWidths(new float[] { 2f, 2f, 7f, 3f, 5f });

            cellCenter.Phrase = new Phrase("Kode", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Nama", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Keterangan Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Jumlah", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Satuan", bold_font);
            tableContent.AddCell(cellCenter);

            //int indexItem = 0;
            foreach (var item in viewModel.Items)
            {
                cellLeft.Phrase = new Phrase($"{item.ProductCode}", normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase($"{item.ProductName}", normal_font);
                tableContent.AddCell(cellLeft);

                cellCenter.Phrase = new Phrase($"{item.ProductRemark}", normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(item.Quantity.ToString(), normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase($"{item.UomUnit}", normal_font);
                tableContent.AddCell(cellCenter);
            }


            PdfPCell cellContent = new PdfPCell(tableContent);
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 20f;
            document.Add(tableContent);

            #endregion

            #region TableSignature

            PdfPTable tableSignature = new PdfPTable(3);

            PdfPCell cellSignatureContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellSignatureContent.Phrase = new Phrase("Penerima\n\n\n\n\n\n\n(  _____________________  )", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Mengetahui\n\n\n\n\n\n\n(  _____________________  )", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Dibuat Oleh\n\n\n\n\n\n\n(  _____________________  )", normal_font);
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContentLeft.Phrase = new Phrase($"\n\nDicetak Tanggal {DateTimeOffset.Now.ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
            tableSignature.AddCell(cellSignatureContentLeft);
            cellSignatureContent.Phrase = new Phrase(" ", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase(" ", normal_font);
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
