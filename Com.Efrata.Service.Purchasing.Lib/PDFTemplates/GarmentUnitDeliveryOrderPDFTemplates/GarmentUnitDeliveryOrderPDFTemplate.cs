using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentUnitDeliveryOrderViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates.GarmentUnitDeliveryOrderPDFTemplates
{
    public class GarmentUnitDeliveryOrderPDFTemplate
    {
        public static MemoryStream GeneratePdfTemplate(IServiceProvider serviceProvider, GarmentUnitDeliveryOrderViewModel viewModel)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 14);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            Document document = new Document(PageSize.A4, 40, 40, 40, 40);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            
            #region Header

            string titleString = "DELIVERY ORDER";
            Paragraph title = new Paragraph(titleString, header_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);

            //string companyNameString = "PT EFRATA RETAILINDO";
            //Paragraph companyName = new Paragraph(companyNameString, bold_font) { Alignment = Element.ALIGN_LEFT };
            //document.Add(companyName);

            //string companyAddressString = "Banaran, Grogol, Sukoharjo, Jawa Tengah" + "\n" + "57552" + "\n" + "Telp (0271) 732888, 7652913";
            //Paragraph companyAddress = new Paragraph(companyAddressString, normal_font) { Alignment = Element.ALIGN_LEFT };
            //companyAddress.SpacingAfter = 10f;
            //document.Add(companyAddress);
            
            #endregion

            #region Identity

            //var receiptDate = viewModel.ReceiptDate.GetValueOrDefault().ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
            //var receivedFrom = viewModel.URNType == "PROSES" ? "PROSES" : viewModel.URNType == "GUDANG LAIN" ? "GUDANG LAIN" : viewModel.URNType == "GUDANG SISA" ? "GUDANG SISA" : viewModel.URNType == "SISA SUBCON" ? "SISA SUBCON" : viewModel.Supplier.Name;
            //var receiptNo = viewModel.URNType == "PROSES" ? viewModel.DRNo : viewModel.URNType == "GUDANG LAIN" ? viewModel.UENNo : viewModel.URNType == "GUDANG SISA" ? viewModel.ExpenditureNo : viewModel.URNType == "SISA SUBCON" ? viewModel.UENNo : viewModel.DONo;
            var date =  viewModel.UnitDODate.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
            PdfPTable tableIdentity = new PdfPTable(4);
            tableIdentity.SetWidths(new float[] { 3f, 4f, 3f, 4f });
            PdfPCell cellIdentityContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

            cellIdentityContentLeft.Phrase = new Phrase("Nomor Unit Delivery Order", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.UnitDONo, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("Unit yang Mengirim", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.UnitSender.Name, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("Jenis Unit Delivery Order", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.UnitDOType, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("Gudang yang Mengirim", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.Storage.name, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("Tgl. Delivery Order", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + date, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("No. RO Job", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.RONo, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("Unit yang Meminta", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.UnitRequest.Name, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("Artikel", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.Article, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingAfter = 10f;
            tableIdentity.SpacingBefore = 20f;
            document.Add(tableIdentity);

            #endregion

            #region TableContent

            Paragraph please = new Paragraph("Harap dikeluarkan barang dengan rincian sbb.", normal_font);
            document.Add(please);

            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            PdfPTable tableContent = new PdfPTable(8);
            tableContent.SetWidths(new float[] { 1.5f, 3f, 3f, 6f, 3f, 3f, 2f, 4f });

            cellCenter.Phrase = new Phrase("No", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Kode Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Nama Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Keterangan Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("RO Asal", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Jumlah DO", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Satuan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Tipe Fabric", bold_font);
            tableContent.AddCell(cellCenter);

            int indexItem = 0;
            foreach (var item in viewModel.Items)
            {
                cellCenter.Phrase = new Phrase((++indexItem).ToString(), normal_font);
                tableContent.AddCell(cellCenter);

                cellLeft.Phrase = new Phrase($"{item.ProductCode}", normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase($"{item.ProductName}", normal_font);
                tableContent.AddCell(cellLeft);

                cellCenter.Phrase = new Phrase($"{item.ProductRemark}", normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(item.RONo, normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase($"{item.Quantity}", normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase($"{item.UomUnit}", normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase($"{item.FabricType}", normal_font);
                tableContent.AddCell(cellCenter);
            }


            PdfPCell cellContent = new PdfPCell(tableContent);
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 10f;
            tableContent.SpacingBefore = 10f;
            document.Add(tableContent);

            #endregion

            #region footer
            PdfPTable tablefooter = new PdfPTable(2);
            tablefooter.SetWidths(new float[] { 1f, 2f });


            cellIdentityContentLeft.Phrase = new Phrase("TANGGAL DITERIMA", normal_font);
            tablefooter.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": ", normal_font);
            tablefooter.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("DIKIRIM KEPADA", normal_font);
            tablefooter.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": ", normal_font);
            tablefooter.AddCell(cellIdentityContentLeft);

            cellIdentityContentLeft.Phrase = new Phrase("KETERANGAN", normal_font);
            tablefooter.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": ", normal_font);
            tablefooter.AddCell(cellIdentityContentLeft);
            
            tablefooter.ExtendLastRow = false;
            tablefooter.SpacingAfter = 10f;
            document.Add(tablefooter);

            #endregion

            #region TableSignature

            PdfPTable tableSignature = new PdfPTable(3);

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellSignatureContent.Phrase = new Phrase("Bagian yang Membutuhkan\n\n\n\n\n\n\n(  _____________________  )", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("", normal_font);
            tableSignature.AddCell(cellSignatureContent);
            //cellSignatureContent.Phrase = new Phrase("Manager Produksi\n\n\n\n\n\n\n(  _____________________  )", normal_font);
            //tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Gudang\n\n\n\n\n\n\n(  _____________________  )", normal_font);
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
