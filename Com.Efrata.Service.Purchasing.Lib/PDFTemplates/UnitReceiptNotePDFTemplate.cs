using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
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
    public class UnitReceiptNotePDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(UnitReceiptNoteViewModel viewModel)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font1 = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

            Document document = new Document(PageSize.A6.Rotate(), 15, 15, 15, 15);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            #region Header

            string titleString = "BON PENERIMAAN BARANG";
            Paragraph title = new Paragraph(titleString, bold_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);

            string companyNameString = "PT EFRATA GARMINDO UTAMA";
            Paragraph companyName = new Paragraph(companyNameString, bold_font) { Alignment = Element.ALIGN_LEFT };
            document.Add(companyName);

            PdfPTable tableHeader = new PdfPTable(2);
            tableHeader.SetWidths(new float[] { 4f, 4f });
            PdfPCell cellHeaderContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellHeaderContentCenter = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            PdfPCell cellHeaderContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

            cellHeaderContentLeft.Phrase = new Phrase("Jl. Merapi No.23 Blok E1, Desa/Kelurahan Banaran," + "\n" + "Kec. Grogol, Kab. Sukoharjo, Provinsi Jawa Tengah" + "\n" + "Kode Pos: 57552, Telp: 02711740888", bold_font1);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentLeft.Phrase = new Phrase(" ", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);
            cellHeaderContentCenter.Phrase = new Phrase(" ", bold_font);
            tableHeader.AddCell(cellHeaderContentCenter);
            cellHeaderContentCenter.Phrase = new Phrase(" ", bold_font);
            tableHeader.AddCell(cellHeaderContentCenter);
            cellHeaderContentCenter.Colspan = 2;
            //cellHeaderContentCenter.Phrase = new Phrase("BON PENERIMAAN BARANG", bold_font);
            //tableHeader.AddCell(cellHeaderContentCenter);

            //cellHeaderContentRight.Phrase = new Phrase("FM-PB-00-06-010/R2", bold_font);
            //cellHeaderContentRight.Phrase = new Phrase("  ", bold_font);
            //tableHeader.AddCell(cellHeaderContentRight);

            PdfPCell cellHeader = new PdfPCell(tableHeader);
            tableHeader.ExtendLastRow = false;
            tableHeader.SpacingAfter = 5f;
            document.Add(tableHeader);

            LineSeparator lineSeparator = new LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1);
            document.Add(lineSeparator);


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


            PdfPTable tableIdentity = new PdfPTable(4);
            tableIdentity.SetWidths(new float[] { 3f, 7f, 2f, 5f });
            PdfPCell cellIdentityContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

            cellIdentityContentLeft.Phrase = new Phrase("Tanggal", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.date.ToString("dd/MM/yyyy", new CultureInfo("id-ID")), normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Bagian", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + unitName, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Diterima dari", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.supplier.name, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("No.", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.no, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("Surat Jalan", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.doNo, normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);
            cellIdentityContentLeft.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellIdentityContentLeft);

            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingAfter = 5f;
            tableIdentity.SpacingBefore = 5f;
            document.Add(tableIdentity);

            #endregion

            #region TableContent

            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            PdfPTable tableContent = new PdfPTable(5);
            tableContent.SetWidths(new float[] { 1.5f, 9f, 2.5f, 2.5f, 5f });

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

            //for (int a = 0; a < 20; a++) // coba kalau banyak baris ^_^
            for (int indexItem = 0; indexItem < viewModel.items.Count; indexItem++)
            {
                UnitReceiptNoteItemViewModel item = viewModel.items[indexItem];

                cellCenter.Phrase = new Phrase((indexItem + 1).ToString(), normal_font);
                tableContent.AddCell(cellCenter);

                cellLeft.Phrase = new Phrase($"{item.product.code} - {item.product.name}", normal_font);
                tableContent.AddCell(cellLeft);
                //cellCenter.Phrase = new Phrase(string.Format("{0:n2}", item.deliveredQuantity), normal_font);
                cellCenter.Phrase = new Phrase($"{item.deliveredQuantity}", normal_font);
                tableContent.AddCell(cellCenter);

                cellLeft.Phrase = new Phrase(item.product.uom.unit, normal_font);
                tableContent.AddCell(cellLeft);

                cellCenter.Phrase = new Phrase(item.prNo + "\n" + item.productRemark, normal_font);
                tableContent.AddCell(cellCenter);
            }


            PdfPCell cellContent = new PdfPCell(tableContent);
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 5f;
            document.Add(tableContent);

            #endregion


            Paragraph date = new Paragraph($"Sukoharjo, {viewModel.date.ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font) { Alignment = Element.ALIGN_RIGHT };
            document.Add(date);

            #region TableSignature

            PdfPTable tableSignature = new PdfPTable(2);

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            cellSignatureContent.Phrase = new Phrase("Mengetahui\n\n\n\n\n(  _____________________  )", bold_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Yang Menerima\n\n\n\n\n(  _____________________  )", bold_font);
            tableSignature.AddCell(cellSignatureContent);


            PdfPCell cellSignature = new PdfPCell(tableSignature); // dont remove
            tableSignature.ExtendLastRow = false;
            tableSignature.SpacingBefore = 10f;
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
