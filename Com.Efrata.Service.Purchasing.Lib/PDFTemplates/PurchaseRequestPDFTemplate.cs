using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class PurchaseRequestPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(PurchaseRequestViewModel viewModel, int clientTimeZoneOffset)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font normal_font2 = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
            Font normal_font1 = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

            Document document = new Document(PageSize.A5, 15, 15, 15, 15);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            #region Header

            string companyNameString = "PT. EFRATA GARMINDO UTAMA";
            Paragraph companyName = new Paragraph(companyNameString, header_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(companyName);

            string companyAddressString = "Jl. Merapi No.23 Blok E1, Desa/Kelurahan Banaran," +" Kec. Grogol, Kab. Sukoharjo, Provinsi Jawa Tengah\n" +" Kode Pos: 57552, Telp: 02711740888";
            Paragraph companyAddress = new Paragraph(companyAddressString, normal_font2) { Alignment = Element.ALIGN_CENTER };
            companyAddress.SpacingAfter = 10f;
            document.Add(companyAddress);

            LineSeparator lineSeparator = new LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1);
            document.Add(lineSeparator);

            //string codeNoString = "FM-PB-00-06-006/R1";
            string codeNoString = "";
            Paragraph codeNo = new Paragraph(codeNoString, bold_font) { Alignment = Element.ALIGN_RIGHT };
            codeNo.SpacingBefore = 5f;
            document.Add(codeNo);

            string titleString = "ORDER PEMESANAN";
            bold_font.SetStyle(Font.UNDERLINE);
            Paragraph title = new Paragraph(titleString, bold_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);
            bold_font.SetStyle(Font.NORMAL);

            #endregion


            var unitName = "";
            var unitId = viewModel.unit._id;
            if (unitId == "50")
            {
                unitName = "WEAVING";
            }
            else if (unitId == "35")
               {
                  unitName = "SPINNING 1";
               }
            else
            {
                unitName = viewModel.unit.name;
            }
            #region Identity

            PdfPTable tableIdentity = new PdfPTable(3);
            tableIdentity.SetWidths(new float[] { 1f, 4.5f, 2.5f });
            PdfPCell cellIdentityContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellIdentityContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
            cellIdentityContentLeft.Phrase = new Phrase("Bagian", normal_font2);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + unitName, normal_font2);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentRight.Phrase = new Phrase("Sukoharjo, " + viewModel.date.GetValueOrDefault().ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font2);
            tableIdentity.AddCell(cellIdentityContentRight);
            cellIdentityContentLeft.Phrase = new Phrase("Budget", normal_font2);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.budget.name, normal_font2);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentRight.Phrase = new Phrase("");
            tableIdentity.AddCell(cellIdentityContentRight);
            cellIdentityContentLeft.Phrase = new Phrase("Nomor", normal_font2);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.no, normal_font2);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentRight.Phrase = new Phrase("");
            tableIdentity.AddCell(cellIdentityContentRight);
            PdfPCell cellIdentity = new PdfPCell(tableIdentity); // dont remove
            tableIdentity.ExtendLastRow = false;
            document.Add(tableIdentity);

            #endregion

            string firstParagraphString = "Mohon dibelikan/diusahakan barang tersebut dibawah ini :";
            Paragraph firstParagraph = new Paragraph(firstParagraphString, normal_font2) { Alignment = Element.ALIGN_LEFT };
            firstParagraph.SpacingBefore = 10f;
            firstParagraph.SpacingAfter = 10f;
            document.Add(firstParagraph);

            #region TableContent

            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            PdfPTable tableContent = new PdfPTable(5);
            tableContent.SetWidths(new float[] { 1f, 4f, 7f, 3f, 4f });

            cellCenter.Phrase = new Phrase("NO", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("KODE", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("BARANG", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("JUMLAH", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("HARGA", bold_font);
            tableContent.AddCell(cellCenter);

            //for (int a = 0; a < 20; a++) // coba kalau banyak baris ^_^
            for (int indexItem = 0; indexItem < viewModel.items.Count; indexItem++)
            {
                PurchaseRequestItemViewModel item = viewModel.items[indexItem];

                cellCenter.Phrase = new Phrase((indexItem + 1).ToString(), normal_font2);
                tableContent.AddCell(cellCenter);

                cellLeft.Phrase = new Phrase(item.product.code, normal_font2);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(item.product.name + Environment.NewLine + item.remark, normal_font2);
                tableContent.AddCell(cellLeft);

                cellCenter.Phrase = new Phrase(string.Format("{0:n2}",item.quantity) + $" {item.product.uom.unit}", normal_font2);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(" ", normal_font2);
                tableContent.AddCell(cellCenter);
            }

            for (int i = 0; i < 5; i++)
            {
                tableContent.AddCell(cellCenter);
            }

            PdfPCell cellContent = new PdfPCell(tableContent); // dont remove
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 20f;
            document.Add(tableContent);

            #endregion

            #region Footer

            PdfPTable tableFooter = new PdfPTable(2);
            tableFooter.SetWidths(new float[] { 2f, 8f });
            PdfPCell cellFooterContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            cellFooterContent.Phrase = new Phrase("Kategori", normal_font2);
            tableFooter.AddCell(cellFooterContent);
            cellFooterContent.Phrase = new Phrase(": " + viewModel.category.name, normal_font2);
            tableFooter.AddCell(cellFooterContent);
            cellFooterContent.Phrase = new Phrase("Diminta Datang", normal_font2);
            tableFooter.AddCell(cellFooterContent);
            cellFooterContent.Phrase = new Phrase(": " + (viewModel.expectedDeliveryDate != null && viewModel.expectedDeliveryDate != DateTimeOffset.MinValue ? viewModel.expectedDeliveryDate.GetValueOrDefault().ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")) : "-"), normal_font2);
            tableFooter.AddCell(cellFooterContent);
            cellFooterContent.Phrase = new Phrase("Keterangan", normal_font2);
            tableFooter.AddCell(cellFooterContent);
            cellFooterContent.Phrase = new Phrase(": " + viewModel.remark, normal_font2);
            tableFooter.AddCell(cellFooterContent);
            PdfPCell cellFooter = new PdfPCell(tableFooter); // dont remove
            tableFooter.ExtendLastRow = false;
            document.Add(tableFooter);

            #endregion

            #region TableSignature

            PdfPTable tableSignature = new PdfPTable(5);
            tableSignature.SetWidths(new float[] { 1f, 1f, 1f, 1f, 1f });

            cellCenter.Phrase = new Phrase("BAGIAN ANGGARAN", bold_font);
            tableSignature.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("ACC MENGETAHUI", bold_font);
            tableSignature.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("BAGIAN PEMBELIAN", bold_font);
            tableSignature.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("KEPALA BAGIAN", bold_font);
            tableSignature.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("YANG MEMERLUKAN", bold_font);
            tableSignature.AddCell(cellCenter);

            cellCenter.Phrase = new Phrase("\n\n\n\n");
            tableSignature.AddCell(cellCenter);
            tableSignature.AddCell(cellCenter);
            tableSignature.AddCell(cellCenter);
            tableSignature.AddCell(cellCenter);
            tableSignature.AddCell(cellCenter);

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
