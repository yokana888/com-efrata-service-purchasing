using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitPaymentOrderViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class UnitPaymentQuantityCorrectionNotePDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(UnitPaymentCorrectionNoteViewModel viewModel, UnitPaymentOrderViewModel viewModelSpb, DateTimeOffset receiptDate, string supplier_address, string username, int clientTimeZoneOffset = 7)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 14);
            Font small_normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 14);
            Font middle_bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font terbilang_bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font small_bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            double totalPPn = 0;
            double totalPPh = 0;
            double totalDibayar = 0;
            double total = 0;
            string currencyCodePPn = "";
            string currencyCodeTotal = "";
            string currencyDesc = "";

            Document document = new Document(PageSize.A5.Rotate(), 18, 18, 17, 10);
            //document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate())
            MemoryStream stream = new MemoryStream();
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                #region Header

                //string titleString = "NOTA KOREKSI";
                //Paragraph title = new Paragraph(titleString, bold_font) { Alignment = Element.ALIGN_CENTER };
                //document.Add(title);

                //string companyNameString = "PT EFRATA GARMINDO UTAMA";
                //Paragraph companyName = new Paragraph(companyNameString, header_font) { Alignment = Element.ALIGN_LEFT };
                //document.Add(companyName);

                PdfPTable tableHeader = new PdfPTable(3);
                tableHeader.SetWidths(new float[] { 4f, 5f, 4f });
                PdfPCell cellHeaderContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                PdfPCell cellHeaderContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell cellHeaderContentCenter = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

                cellHeaderContentLeft.Phrase = new Phrase("PT. EFRATA RENTAILINDO", bold_font);
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentCenter.Phrase = new Phrase("NOTA KOREKSI", bold_font);
                tableHeader.AddCell(cellHeaderContentCenter);

                cellHeaderContentRight.Phrase = new Phrase("");
                tableHeader.AddCell(cellHeaderContentRight);

                cellHeaderContentLeft.Phrase = new Phrase("Kel. Banaran, Kec. Grogol, Kab.Sukoharjo, Jawa Tengah" + "\n" + "57552" + "\n" + "Telp (+62 271)719911, (+62 21)2900977", small_normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentCenter.Phrase = new Phrase(viewModel.correctionType, small_normal_font);
                tableHeader.AddCell(cellHeaderContentCenter);

                //cellHeaderContentLeft.Phrase = new Phrase("FM-PB-00-06-004", terbilang_bold_font);
                //tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentLeft.Phrase = new Phrase("  ", terbilang_bold_font);
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentLeft.Phrase = new Phrase("");
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentCenter.Phrase = new Phrase("");
                tableHeader.AddCell(cellHeaderContentCenter);
                cellHeaderContentLeft.Phrase = new Phrase($"SUKOHARJO, {viewModel.correctionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}" + "\n" + $"({viewModel.supplier.code}) {viewModel.supplier.name}" + "\n" + $"{supplier_address}", normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);



                PdfPCell cellHeader = new PdfPCell(tableHeader);
                tableHeader.ExtendLastRow = false;
                tableHeader.SpacingAfter = 1f;
                document.Add(tableHeader);

                #endregion

                #region Identity

                PdfPTable tableIdentity = new PdfPTable(4);
                tableIdentity.SetWidths(new float[] { 3f, 7f, 1.3f, 5f });
                PdfPCell cellIdentityContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                PdfPCell cellIdentityContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

                cellIdentityContentLeft.Phrase = new Phrase("Retur/Potongan", normal_font);
                tableIdentity.AddCell(cellIdentityContentLeft);
                cellIdentityContentLeft.Phrase = new Phrase(":    " + viewModel.category.name, normal_font);
                tableIdentity.AddCell(cellIdentityContentLeft);
                cellIdentityContentRight.Phrase = new Phrase("", normal_font);
                tableIdentity.AddCell(cellIdentityContentRight);
                cellIdentityContentLeft.Phrase = new Phrase($"Nomor : {viewModel.uPCNo}", middle_bold_font);
                cellIdentityContentLeft.Rowspan = 2;
                tableIdentity.AddCell(cellIdentityContentLeft);
                cellIdentityContentLeft.Phrase = new Phrase("Untuk", normal_font);
                tableIdentity.AddCell(cellIdentityContentLeft);
                cellIdentityContentLeft.Phrase = new Phrase(":    " + viewModel.division.name, normal_font);
                tableIdentity.AddCell(cellIdentityContentLeft);
                cellIdentityContentLeft.Phrase = new Phrase("", normal_font);
                tableIdentity.AddCell(cellIdentityContentLeft);
                cellIdentityContentLeft.Phrase = new Phrase("", normal_font);
                tableIdentity.AddCell(cellIdentityContentLeft);
                //cellIdentityContentLeft.Phrase = new Phrase("No.", normal_font);
                //tableIdentity.AddCell(cellIdentityContentLeft);
                //cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.no, normal_font);
                //tableIdentity.AddCell(cellIdentityContentLeft);

                PdfPCell cellIdentity = new PdfPCell(tableIdentity);
                tableIdentity.ExtendLastRow = false;
                tableIdentity.SpacingAfter = 10f;
                tableIdentity.SpacingBefore = 3f;
                document.Add(tableIdentity);

                #endregion

                #region TableContent

                PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_TOP, Padding = 5 };
                PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_TOP, Padding = 5 };
                PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_TOP, Padding = 5 };

                PdfPCell cellRightMerge = new PdfPCell() { Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_TOP, Padding = 5 };
                PdfPCell cellLeftMerge = new PdfPCell() { Border = Rectangle.NO_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_TOP, Padding = 5 };


                PdfPTable tableContent = new PdfPTable(9);
                tableContent.SetWidths(new float[] { 1f, 5f, 2f, 2f, 1f, 3f, 1f, 3f, 3f });

                cellCenter.Phrase = new Phrase("No", small_bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Nama Barang", small_bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Jumlah SPB", small_bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Jumlah Retur", small_bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Harga Satuan", small_bold_font);
                cellCenter.Colspan = 2;
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Harga Total", small_bold_font);
                cellCenter.Colspan = 2;
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Nomor Order", small_bold_font);
                tableContent.AddCell(cellCenter);

                //for (int a = 0; a < 20; a++) // coba kalau banyak baris ^_^
                for (int indexItem = 0; indexItem < viewModel.items.Count; indexItem++)
                {
                    UnitPaymentCorrectionNoteItemViewModel item = viewModel.items[indexItem];
                    var upoDetail = viewModelSpb.items.SelectMany(upoItem => upoItem.unitReceiptNote.items).FirstOrDefault(upoItem => upoItem.Id == item.uPODetailId);

                    if (upoDetail == null)
                        upoDetail = new UnitPaymentOrderDetailViewModel();

                    cellCenter.Phrase = new Phrase((indexItem + 1).ToString(), normal_font);
                    cellCenter.Colspan = 0;
                    tableContent.AddCell(cellCenter);

                    cellLeft.Phrase = new Phrase($"{item.product.code} - {item.product.name}", normal_font);
                    tableContent.AddCell(cellLeft);

                    cellRight.Phrase = new Phrase(string.Format("{0:n2}", upoDetail.deliveredQuantity) + $" {item.uom.unit}", normal_font);
                    tableContent.AddCell(cellRight);

                    cellRight.Phrase = new Phrase(string.Format("{0:n2}", (item.quantity * -1)) + $" {item.uom.unit}", normal_font);
                    tableContent.AddCell(cellRight);

                    cellLeftMerge.Phrase = new Phrase($"{item.currency.code}", normal_font);
                    tableContent.AddCell(cellLeftMerge);

                    cellRightMerge.Phrase = new Phrase(string.Format("{0:n4}", item.pricePerDealUnitAfter), normal_font);
                    tableContent.AddCell(cellRightMerge);

                    cellLeftMerge.Phrase = new Phrase($"{item.currency.code}", normal_font);
                    tableContent.AddCell(cellLeftMerge);

                    cellRightMerge.Phrase = new Phrase($"{(item.priceTotalAfter * -1).ToString("N", CultureInfo.InvariantCulture)}", normal_font);
                    tableContent.AddCell(cellRightMerge);

                    cellLeft.Phrase = new Phrase(item.pRNo, normal_font);
                    tableContent.AddCell(cellLeft);

                    currencyCodePPn = item.currency.code;
                    currencyDesc = viewModelSpb.currency.description;
                    currencyCodeTotal = item.currency.code;

                    total += (item.priceTotalAfter * -1);

                }
                totalPPn = ( (Convert.ToDouble(viewModelSpb.vatTax.rate) / 100) * total);
                double pph = double.Parse(viewModelSpb.incomeTax.rate);
                totalPPh = (pph * total) / 100;

                PdfPCell cellContent = new PdfPCell(tableContent);
                tableContent.ExtendLastRow = false;
                tableContent.SpacingAfter = 5f;
                document.Add(tableContent);

                #endregion

                #region TableTotal

                PdfPTable tableTotal = new PdfPTable(7);
                tableTotal.SetWidths(new float[] { 6f, 2f, 4f, 2f, 3f, 2f, 4f });
                PdfPCell cellIdentityTotalContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                PdfPCell cellIdentityTotalContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell cellIdentityTotalContentCenter = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase("Jumlah", normal_font);
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase($"{currencyCodePPn}", normal_font);
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentRight.Phrase = new Phrase(total.ToString("N", CultureInfo.InvariantCulture), normal_font);
                tableTotal.AddCell(cellIdentityTotalContentRight);

                if (viewModel.useIncomeTax == false)
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                }
                else
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"PPh {viewModelSpb.incomeTax.name} {viewModelSpb.incomeTax.rate} %", normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"{currencyCodePPn}", normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentRight.Phrase = new Phrase(totalPPh.ToString("N", CultureInfo.InvariantCulture), normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentRight);
                }
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                //cellIdentityTotalContentLeft.Phrase = new Phrase($"PPn {viewModelSpb.vatTax.rate} %", normal_font);
                //tableTotal.AddCell(cellIdentityTotalContentLeft);
                if (viewModel.useVat == false)
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"PPn %", normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentRight.Phrase = new Phrase("-");
                    tableTotal.AddCell(cellIdentityTotalContentRight);
                    totalPPn = 0;
                }
                else
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"PPn {viewModelSpb.vatTax.rate} %", normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"{currencyCodePPn}", normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentRight.Phrase = new Phrase(totalPPn.ToString("N", CultureInfo.InvariantCulture), normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentRight);
                }
                if (viewModel.useIncomeTax == false)
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                }
                else
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"Jumlah dibayar Ke Supplier", normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"{currencyCodePPn}", normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    totalDibayar = (total + totalPPn) - totalPPh;
                    cellIdentityTotalContentRight.Phrase = new Phrase(totalDibayar.ToString("N", CultureInfo.InvariantCulture), normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentRight);
                }
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase("Total", normal_font);
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase($"{currencyCodeTotal}", normal_font);
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentRight.Phrase = new Phrase((total + totalPPn).ToString("N", CultureInfo.InvariantCulture), normal_font);
                tableTotal.AddCell(cellIdentityTotalContentRight);

                if (viewModel.useIncomeTax == true)
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"Terbilang : { NumberToTextIDN.terbilang(totalDibayar)} {currencyDesc.ToLower()}", terbilang_bold_font);
                    cellIdentityTotalContentLeft.Colspan = 7;
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                }
                else
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"Terbilang : { NumberToTextIDN.terbilang(total + totalPPn)} {currencyDesc.ToLower()}", terbilang_bold_font);
                    cellIdentityTotalContentLeft.Colspan = 7;
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                }

                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                //cellIdentityContentLeft.Phrase = new Phrase("No.", normal_font);
                //tableIdentity.AddCell(cellIdentityContentLeft);
                //cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.no, normal_font);
                //tableIdentity.AddCell(cellIdentityContentLeft);

                PdfPCell cellIdentityTotal = new PdfPCell(tableTotal);
                tableTotal.ExtendLastRow = false;
                //tableTotal.SpacingAfter = 1f;
                tableTotal.SpacingBefore = 2f;
                document.Add(tableTotal);

                #endregion

                #region TableKeterangan

                PdfPTable tableKeterangan = new PdfPTable(4);
                tableKeterangan.SetWidths(new float[] { 3f, 4f, 3f, 4f });
                PdfPCell cellIdentityKeteranganContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                PdfPCell cellIdentityKeteranganContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Perjanjian Pembayaran ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {viewModel.dueDate.GetValueOrDefault().ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentRight.Phrase = new Phrase(" ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentRight);
                cellIdentityKeteranganContentRight.Phrase = new Phrase(" ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentRight);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Nota ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {viewModel.uPONo}", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Barang Datang ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {receiptDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Keterangan ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {viewModel.remark}", small_bold_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Nomor Nota Retur ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {viewModel.returNoteNo}", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                //cellIdentityContentLeft.Phrase = new Phrase("No.", normal_font);
                //tableIdentity.AddCell(cellIdentityContentLeft);
                //cellIdentityContentLeft.Phrase = new Phrase(": " + viewModel.no, normal_font);
                //tableIdentity.AddCell(cellIdentityContentLeft);

                PdfPCell cellIdentityKeterangan = new PdfPCell(tableKeterangan);
                tableKeterangan.ExtendLastRow = false;
                tableKeterangan.SpacingAfter = 12f;
                //tableKeterangan.SpacingBefore = 2f;
                document.Add(tableKeterangan);

                #endregion
                //Paragraph date = new Paragraph($"Sukoharjo, {viewModel.correctionDate.ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font) { Alignment = Element.ALIGN_RIGHT };
                //document.Add(date);

                #region TableSignature

                PdfPTable tableSignature = new PdfPTable(4);

                PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
                PdfPCell cellSignatureContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

                cellSignatureContent.Phrase = new Phrase("Diperiksa,\nVerifikasi\n\n\n\n\n(                              )", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("Mengetahui,\nPimpinan Bagian\n\n\n\n\n(                              )", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("Tanda Terima,\nBagian Pembelian\n\n\n\n\n(                              )", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase($"Dibuat Oleh,\n\n\n\n\n\n(        {viewModel.CreatedBy}        )", normal_font);
                tableSignature.AddCell(cellSignatureContent);

                cellSignatureContentLeft.Phrase = new Phrase($"\n\nDicetak Oleh {username}", normal_font);
                tableSignature.AddCell(cellSignatureContentLeft);
                cellSignatureContent.Phrase = new Phrase(" ", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase(" ", normal_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase(" ", normal_font);
                tableSignature.AddCell(cellSignatureContent);


                PdfPCell cellSignature = new PdfPCell(tableSignature); // dont remove
                tableSignature.ExtendLastRow = false;
                //tableSignature.SpacingBefore = 20f;
                tableSignature.SpacingAfter = 20f;
                document.Add(tableSignature);

                #endregion

                document.Close();
                byte[] byteInfo = stream.ToArray();
                stream.Write(byteInfo, 0, byteInfo.Length);
                stream.Position = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return stream;
        }

        public MemoryStream GeneratePdfNotaReturTemplate(UnitPaymentCorrectionNoteViewModel viewModel, UnitPaymentOrderViewModel viewModelSpb, int clientTimeZoneOffset = 7)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 14);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 11);
            Font smaller_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font smallest_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font smaller_bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font smallest_bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

            double totalPPn = 0;
            double total = 0;
            string currencyCodePPn = "";
            string currencyCodeTotal = "";

            Document document = new Document(PageSize.A5, 20, 20, 17, 20);
            //document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate())
            MemoryStream stream = new MemoryStream();

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();
                #region Header

                string titleString = "NOTA RETUR";
                Paragraph title = new Paragraph(titleString, normal_font) { Alignment = Element.ALIGN_CENTER };
                document.Add(title);

                Paragraph space = new Paragraph(" ", smaller_font) { Alignment = Element.ALIGN_CENTER };
                document.Add(space);

                Paragraph nomor = new Paragraph($"Nomor : {viewModel.returNoteNo}", smaller_font) { Alignment = Element.ALIGN_CENTER };
                document.Add(nomor);

                Paragraph pajakNomorString = new Paragraph($"Atas Faktur Pajak Nomor : {viewModel.vatTaxCorrectionNo} Tanggal : {viewModel.vatTaxCorrectionDate.GetValueOrDefault().ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", smaller_font) { Alignment = Element.ALIGN_CENTER };
                document.Add(pajakNomorString);

                document.Add(space);
                LineSeparator lineSeparator = new LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1);
                document.Add(lineSeparator);

                document.Add(space);

                Paragraph pembeliBkp = new Paragraph("Pembeli PKB", smaller_bold_font) { Alignment = Element.ALIGN_LEFT };
                document.Add(pembeliBkp);

                Paragraph companyName = new Paragraph("Nama     :  PT EFRATA RETAILINDO", smaller_font) { Alignment = Element.ALIGN_LEFT };
                document.Add(companyName);

                Paragraph companyAddress = new Paragraph("Alamat     : Banaran, Grogol, Sukoharjo, Jawa Tengah 57552", smaller_font) { Alignment = Element.ALIGN_LEFT };
                document.Add(companyAddress);

                Paragraph companyNPWP = new Paragraph("N P W P  :  00.000.000.0 - 000.000", smaller_font) { Alignment = Element.ALIGN_LEFT };
                document.Add(companyNPWP);

                document.Add(space);

                Paragraph seller = new Paragraph("Kepada Penjual", smaller_bold_font) { Alignment = Element.ALIGN_LEFT };
                document.Add(seller);

                Paragraph sellerName = new Paragraph($"Nama     :  {viewModel.supplier.name}", smaller_font) { Alignment = Element.ALIGN_LEFT };
                document.Add(sellerName);

                Paragraph sellerAddress = new Paragraph($"Alamat     :  {viewModelSpb.supplier.address}", smaller_font) { Alignment = Element.ALIGN_LEFT };
                document.Add(sellerAddress);

                Paragraph sellerNPWP = new Paragraph($"N P W P  :  {viewModel.supplier.npwp}", smaller_font) { Alignment = Element.ALIGN_LEFT };
                document.Add(sellerNPWP);

                document.Add(space);

                #endregion

                #region TableContent

                PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
                PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
                PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
                PdfPCell cellRightBorderless = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.NO_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
                PdfPCell cellLeftBorderless = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

                PdfPTable tableContent = new PdfPTable(7);

                tableContent.SetWidths(new float[] { 2f, 5f, 4f, 1.5f, 4f, 1.5f, 4f });


                cellCenter.Phrase = new Phrase("No.", smallest_bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Macam dan Jenis BKP", smallest_bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Kuantum", smallest_bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Harga Satuan Menurut Faktur Pajak", smallest_bold_font);
                cellCenter.Colspan = 2;
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("Harga Jual BKP", smallest_bold_font);
                cellCenter.Colspan = 2;
                tableContent.AddCell(cellCenter);

                //for (int a = 0; a < 20; a++) // coba kalau banyak baris ^_^
                for (int indexItem = 0; indexItem < viewModel.items.Count; indexItem++)
                {
                    UnitPaymentCorrectionNoteItemViewModel item = viewModel.items[indexItem];

                    cellCenter.Phrase = new Phrase((indexItem + 1).ToString(), smallest_font);
                    cellCenter.Colspan = 0;
                    tableContent.AddCell(cellCenter);

                    cellLeft.Phrase = new Phrase($"{item.product.name}", smallest_font);
                    tableContent.AddCell(cellLeft);

                    cellRight.Phrase = new Phrase(string.Format("{0:n2}", item.quantity) + $" {item.uom.unit}", smallest_font);
                    tableContent.AddCell(cellRight);

                    cellLeftBorderless.Phrase = new Phrase($"{item.currency.code}", smallest_font);
                    tableContent.AddCell(cellLeftBorderless);

                    cellRightBorderless.Phrase = new Phrase(string.Format("{0:n4}", item.pricePerDealUnitAfter), smallest_font);
                    tableContent.AddCell(cellRightBorderless);

                    cellLeftBorderless.Phrase = new Phrase($"{item.currency.code}", smallest_font);
                    tableContent.AddCell(cellLeftBorderless);

                    cellRightBorderless.Phrase = new Phrase($"{item.priceTotalAfter.ToString("N", CultureInfo.InvariantCulture)}", smallest_font);
                    tableContent.AddCell(cellRightBorderless);

                    currencyCodePPn = item.currency.code;
                    currencyCodeTotal = item.currency.code;
                    totalPPn += (0.1 * item.priceTotalAfter);
                    total += item.priceTotalAfter;

                }

                cellLeft.Phrase = new Phrase("Jumlah Harga Jual BKP yang dikembalikan", smallest_font);
                cellLeft.Colspan = 5;
                tableContent.AddCell(cellLeft);
                cellLeftBorderless.Phrase = new Phrase($"{currencyCodeTotal}", smallest_font);
                cellLeftBorderless.Colspan = 0;
                tableContent.AddCell(cellLeftBorderless);
                cellRightBorderless.Phrase = new Phrase($"{total.ToString("N", CultureInfo.InvariantCulture)}", smallest_font);
                cellRightBorderless.Colspan = 0;
                tableContent.AddCell(cellRightBorderless);

                cellLeft.Phrase = new Phrase("PPN yang diminta kembali", smallest_font);
                cellLeft.Colspan = 5;
                tableContent.AddCell(cellLeft);
                cellLeftBorderless.Phrase = new Phrase($"{currencyCodeTotal}", smallest_font);
                cellLeftBorderless.Colspan = 0;
                tableContent.AddCell(cellLeftBorderless);
                cellRightBorderless.Phrase = new Phrase($"{totalPPn.ToString("N", CultureInfo.InvariantCulture)}", smallest_font);
                cellRightBorderless.Colspan = 0;
                tableContent.AddCell(cellRightBorderless);

                cellRight.Phrase = new Phrase($"\nSukoharjo, {viewModel.correctionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}\n\n\n\n\n\n_________________________\n\n", smallest_font);
                cellRight.Colspan = 7;
                //cellLeft.Rowspan = 8;
                tableContent.AddCell(cellRight);

                cellLeft.Phrase = new Phrase("Lembar ke-1 : untuk PKP Penjual\nLembar ke-2 : untuk Pembeli\nLembar ke-3 : untuk LPP tempat Pembeli terdaftar (dalam hal Pembeli bukan PKP)", smallest_font);
                cellLeft.Colspan = 7;
                //cellLeft.Rowspan = 3;
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(" ");
                cellLeft.Colspan = 7;
                //cellLeft.Rowspan = 2;
                tableContent.AddCell(cellLeft);

                PdfPCell cellContent = new PdfPCell(tableContent);
                tableContent.ExtendLastRow = false;
                tableContent.SpacingAfter = 20f;
                document.Add(tableContent);

                #endregion

                document.Close();
                byte[] byteInfo = stream.ToArray();
                stream.Write(byteInfo, 0, byteInfo.Length);
                stream.Position = 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return stream;
        }
    }
}
