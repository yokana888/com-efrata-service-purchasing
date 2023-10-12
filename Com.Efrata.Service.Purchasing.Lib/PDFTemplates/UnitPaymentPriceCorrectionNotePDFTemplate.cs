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
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class UnitPaymentPriceCorrectionNotePDFTemplate
    {
        private readonly CultureInfo _cultureInfo = new CultureInfo("en-us");
        public MemoryStream GeneratePdfTemplate(UnitPaymentCorrectionNoteViewModel viewModel, UnitPaymentOrderViewModel viewModelSpb, string username, int clientTimeZoneOffset, DateTimeOffset? receiptDate)
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
            //DateTime receiptDate = new DateTime();

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

                cellHeaderContentLeft.Phrase = new Phrase("PT. EFRATA GARMINDO UTAMA", bold_font);
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentCenter.Phrase = new Phrase("NOTA KOREKSI", bold_font);
                tableHeader.AddCell(cellHeaderContentCenter);

                cellHeaderContentRight.Phrase = new Phrase("");
                tableHeader.AddCell(cellHeaderContentRight);

                cellHeaderContentLeft.Phrase = new Phrase("Kel. Banaran, Kec. Grogol, Kab.Sukoharjo, Jawa Tengah" + "\n" + "57552" + "\n" + "Telp (+62 271)719911, (+62 21)2900977", small_normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentCenter.Phrase = new Phrase(viewModel.correctionType, small_normal_font);
                tableHeader.AddCell(cellHeaderContentCenter);

                //cellHeaderContentLeft.Phrase = new Phrase("FM-PB-00-06-015/R3", terbilang_bold_font);
                //tableHeader.AddCell(cellHeaderContentLeft);
                cellHeaderContentLeft.Phrase = new Phrase(" ", terbilang_bold_font);
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentLeft.Phrase = new Phrase("");
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentCenter.Phrase = new Phrase("");
                tableHeader.AddCell(cellHeaderContentCenter);

                cellHeaderContentLeft.Phrase = new Phrase($"SUKOHARJO, {viewModel.correctionDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))}", normal_font);
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentLeft.Phrase = new Phrase("");
                tableHeader.AddCell(cellHeaderContentLeft);

                cellHeaderContentCenter.Phrase = new Phrase("");
                tableHeader.AddCell(cellHeaderContentCenter);

                cellHeaderContentLeft.Phrase = new Phrase("(" + viewModel.supplier.code + ") " + viewModel.supplier.name + "\n" + viewModel.supplier.address, normal_font);
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


                if (viewModel.correctionType == "Harga Satuan")
                {
                    PdfPTable tableContent = new PdfPTable(8);
                    tableContent.SetWidths(new float[] { 1f, 6f, 2f, 1.5f, 2f, 1.5f, 2f, 3f });

                    cellCenter.Phrase = new Phrase("No", small_bold_font);
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Nama Barang", small_bold_font);
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Jumlah", small_bold_font);
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Harga Satuan SPB", small_bold_font);
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Harga Baru", small_bold_font);
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Harga Koreksi", small_bold_font);
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Nilai Koreksi", small_bold_font);
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

                        cellRight.Phrase = new Phrase(string.Format("{0:n2}", item.quantity) + $" {item.uom.unit}", normal_font);
                        tableContent.AddCell(cellRight);

                        cellRight.Phrase = new Phrase($"{item.currency.code} {upoDetail.pricePerDealUnit.ToString("N03", _cultureInfo)}", normal_font);
                        tableContent.AddCell(cellRight);

                        double priceCorrectionUnit = item.pricePerDealUnitAfter;
                        if (viewModel.correctionType == "Harga Satuan")
                        {
                            priceCorrectionUnit = item.pricePerDealUnitAfter - item.pricePerDealUnitBefore;
                        }

                        cellRight.Phrase = new Phrase($"{item.currency.code} {item.pricePerDealUnitAfter.ToString("N03", _cultureInfo)}", normal_font);
                        tableContent.AddCell(cellRight);



                        double priceCorrectionTotal = item.priceTotalAfter;
                        //if (viewModel.correctionType == "Harga Total")
                        //{
                        //    priceCorrectionTotal = item.priceTotalAfter - item.priceTotalBefore;
                        //}
                        //else if (viewModel.correctionType == "Harga Satuan")
                        //{
                        priceCorrectionTotal = priceCorrectionUnit * item.quantity;
                        //}

                        cellRight.Phrase = new Phrase($"{item.currency.code} {priceCorrectionUnit.ToString("N03", _cultureInfo)}", normal_font);
                        tableContent.AddCell(cellRight);

                        cellRight.Phrase = new Phrase($"{item.currency.code} {priceCorrectionTotal.ToString("N03", _cultureInfo)}", normal_font);
                        tableContent.AddCell(cellRight);

                        cellLeft.Phrase = new Phrase(item.pRNo, normal_font);
                        tableContent.AddCell(cellLeft);

                        currencyCodePPn = item.currency.code;
                        currencyDesc = viewModelSpb.currency.description;
                        currencyCodeTotal = item.currency.code;

                        total += priceCorrectionTotal;

                    }
                    totalPPn = ((Convert.ToDouble(viewModelSpb.vatTax.rate) / 100) * total);
                    double pph = double.Parse(viewModelSpb.incomeTax.rate);
                    totalPPh = (pph * total) / 100;
                    totalDibayar = total - totalPPh;


                    PdfPCell cellContent = new PdfPCell(tableContent);
                    tableContent.ExtendLastRow = false;
                    tableContent.SpacingAfter = 5f;
                    document.Add(tableContent);
                }
                else
                {
                    PdfPTable tableContent = new PdfPTable(7);
                    tableContent.SetWidths(new float[] { 1f, 6f, 0.6f, 2f, 0.6f, 1.5f, 2f });

                    cellCenter.Phrase = new Phrase("No", small_bold_font);
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Nama Barang", small_bold_font);
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Total SPB", small_bold_font);
                    cellCenter.Colspan = 2;
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Nilai Koreksi", small_bold_font);
                    cellCenter.Colspan = 2;
                    tableContent.AddCell(cellCenter);
                    cellCenter.Phrase = new Phrase("Nomor Order", small_bold_font);
                    tableContent.AddCell(cellCenter);

                    //for (int a = 0; a < 20; a++) // coba kalau banyak baris ^_^
                    for (int indexItem = 0; indexItem < viewModel.items.Count; indexItem++)
                    {
                        UnitPaymentCorrectionNoteItemViewModel item = viewModel.items[indexItem];

                        cellCenter.Phrase = new Phrase((indexItem + 1).ToString(), normal_font);
                        cellCenter.Colspan = 0;
                        tableContent.AddCell(cellCenter);

                        cellLeft.Phrase = new Phrase($"{item.product.code} - {item.product.name}", normal_font);
                        tableContent.AddCell(cellLeft);

                        cellLeftMerge.Phrase = new Phrase($"{item.currency.code}", normal_font);
                        tableContent.AddCell(cellLeftMerge);

                        double priceCorrectionUnit = item.pricePerDealUnitAfter;
                        //if (viewModel.correctionType == "Harga Satuan")
                        //{
                        //    priceCorrectionUnit = item.pricePerDealUnitAfter - item.pricePerDealUnitBefore;
                        //}

                        cellRightMerge.Phrase = new Phrase(string.Format("{0:n4}", item.priceTotalBefore), normal_font);
                        tableContent.AddCell(cellRightMerge);

                        cellLeftMerge.Phrase = new Phrase($"{item.currency.code}", normal_font);
                        tableContent.AddCell(cellLeftMerge);

                        double priceCorrectionTotal = item.priceTotalAfter;
                        if (viewModel.correctionType == "Harga Total")
                        {
                            priceCorrectionTotal = item.priceTotalAfter - item.priceTotalBefore;
                        }
                        //else if (viewModel.correctionType == "Harga Satuan")
                        //{
                        //    priceCorrectionTotal = priceCorrectionUnit * item.quantity;
                        //}

                        cellRightMerge.Phrase = new Phrase($"{priceCorrectionTotal.ToString("N", CultureInfo.InvariantCulture)}", normal_font);
                        tableContent.AddCell(cellRightMerge);

                        cellLeft.Phrase = new Phrase(item.pRNo, normal_font);
                        tableContent.AddCell(cellLeft);

                        currencyCodePPn = item.currency.code;
                        currencyDesc = viewModelSpb.currency.description;
                        currencyCodeTotal = item.currency.code;

                        total += priceCorrectionTotal;

                    }
                    totalPPn = ((Convert.ToDouble(viewModelSpb.vatTax.rate) / 100) * total);
                    double pph = double.Parse(viewModelSpb.incomeTax.rate);
                    totalPPh = (pph * total) / 100;
                    totalDibayar = total - totalPPh;


                    PdfPCell cellContent = new PdfPCell(tableContent);
                    tableContent.ExtendLastRow = false;
                    tableContent.SpacingAfter = 5f;
                    document.Add(tableContent);
                }

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
                    totalPPh = 0;
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
                if (viewModel.useVat == false)
                {
                    totalPPn = 0;
                    cellIdentityTotalContentLeft.Phrase = new Phrase("PPn %", normal_font);
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                    tableTotal.AddCell(cellIdentityTotalContentLeft);
                    cellIdentityTotalContentRight.Phrase = new Phrase(" - ");
                    tableTotal.AddCell(cellIdentityTotalContentRight);
                }
                else
                {
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"PPn {viewModelSpb.vatTax.rate}%", normal_font);
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
                    cellIdentityTotalContentRight.Phrase = new Phrase((total + totalPPn - totalPPh).ToString("N", CultureInfo.InvariantCulture), normal_font);
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

                cellIdentityTotalContentLeft.Phrase = new Phrase($"Terbilang : { NumberToTextIDN.terbilang(total + totalPPn - totalPPh)} {currencyDesc.ToLower()}", terbilang_bold_font);

                if (viewModel.useIncomeTax == false)
                    cellIdentityTotalContentLeft.Phrase = new Phrase($"Terbilang : { NumberToTextIDN.terbilang(total + totalPPn)} {currencyDesc.ToLower()}", terbilang_bold_font);

                cellIdentityTotalContentLeft.Colspan = 3;
                tableTotal.AddCell(cellIdentityTotalContentLeft);


                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
                cellIdentityTotalContentLeft.Phrase = new Phrase(" ");
                tableTotal.AddCell(cellIdentityTotalContentLeft);
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
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Barang Datang ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                DateTimeOffset date = receiptDate != null ? (DateTimeOffset)receiptDate : new DateTime();
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {(date.ToString("dd MMMM yyyy", new CultureInfo("id-ID")))} ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Nota ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {viewModel.uPONo}", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Nomor Nota Retur ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {viewModel.returNoteNo}", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase("Keterangan ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase($" : {viewModel.remark}", small_bold_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase(" ", normal_font);
                tableKeterangan.AddCell(cellIdentityKeteranganContentLeft);
                cellIdentityKeteranganContentLeft.Phrase = new Phrase(" ", small_bold_font);
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
    }
}
