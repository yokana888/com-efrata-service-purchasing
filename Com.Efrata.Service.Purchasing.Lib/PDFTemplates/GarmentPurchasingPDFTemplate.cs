using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDispositionPurchase;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class GarmentPurchasingPDFTemplate
    {
        public GarmentPurchasingPDFTemplate()
        {

        }
        public MemoryStream GeneratePdfTemplate(FormDto viewModel, int clientTimeZoneOffset, string userName)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font small_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font smaller_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
            Font bold_font2 = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font3 = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font bold_font4 = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

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


            Document document = new Document(PageSize.A4, 30, 30, 100, 30);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = new TextEvents(viewModel.DispositionNo);
            document.Open();

            string fmString = "FM-PB-00-06-011";
            Paragraph fm = new Paragraph(fmString, bold_font4) { Alignment = Element.ALIGN_RIGHT };

            //string titleString = "DISPOSISI PEMBAYARAN";
            //Paragraph title = new Paragraph(titleString, bold_font4) { Alignment = Element.ALIGN_CENTER };

            //document.Add(title);
            bold_font.SetStyle(Font.NORMAL);


            //string NoString = "NO : " + viewModel.DispositionNo;
            //Paragraph dispoNumber = new Paragraph(NoString, bold_font4) { Alignment = Element.ALIGN_CENTER };
            //dispoNumber.SpacingAfter = 20f;
            //document.Add(dispoNumber);



            #region Identity

            PdfPTable tableIdentity = new PdfPTable(5);
            tableIdentity.SetWidths(new float[] { 5f, 0.5f, 2f, 7f, 4f });

            double dpp = 0;
            foreach (var item in viewModel.Items)
            {
                foreach (var detail in item.Details)
                {
                    dpp += detail.PaidPrice;
                }
            }

            double ppn = 0;
            string pph = "";
            double pphRate = 0;

            foreach (var item in viewModel.Items)
            {
                if (!item.IsUseVat)
                {
                    ppn = 0;
                }
                else
                {
                    var vatRatDouble = Convert.ToDouble(item.VatRate);
                    ppn = (dpp * vatRatDouble);
                }
                if (item.IsUseIncomeTax)
                {
                    pph = item.IncomeTaxName;
                    pphRate = dpp * (Convert.ToDouble(item.IncomeTaxRate) / 100);
                }
                break;
            }

            //Jumlah dibayar ke Supplier
            double paidToSupp = dpp + ppn - pphRate;
            //if (viewModel.IncomeTaxBy == "Efrata Garmindo Utama")
            //{
            //    paidToSupp = dpp + ppn;
            //}

            double amount = dpp + ppn;

            //if (viewModel.IncomeTaxBy == "Efrata Garmindo Utama")
            //{
            //    amount = dpp + ppn + pphRate;
            //}


            //calculate vat and incomeTax
            double vat = 0;
            double incomeTax = 0;
            foreach (var item in viewModel.Items)
            {
                if (item.IsPayVat)
                {
                    vat += item.VatValue;
                }

                if (item.IsPayIncomeTax)
                {
                    incomeTax += item.IncomeTaxValue;
                }
            }

            double AmountPDF = (viewModel.DPP + vat - incomeTax) + viewModel.MiscAmount;
            var payingDisposition = Math.Round((paidToSupp + viewModel.MiscAmount + pphRate), 2, MidpointRounding.AwayFromZero);
            cellLeftNoBorder.SetLeading(13f, 0f);
            cellLeftNoBorder.Phrase = new Phrase("Mohon Disposisi Pembayaran", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.PaymentType + "  " + viewModel.CurrencyCode + " " + $"{(AmountPDF).ToString("N", new CultureInfo("id-ID"))}", normal_font);/*$"{viewModel.Amount.ToString("N", new CultureInfo("id-ID"))}", normal_font);*/
            cellLeftNoBorder.Colspan = 2;
            tableIdentity.AddCell(cellLeftNoBorder);
            //cellLeftNoBorder.Phrase = new Phrase( viewModel.Currency.code + " " +  $"{(paidToSupp + viewModel.PaymentCorrection + pphRate).ToString("N", new CultureInfo("id-ID")) }", normal_font);
            //tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            cellLeftNoBorder.Colspan = 0;
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Terbilang", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($"{ NumberToTextIDN.terbilangv2(AmountPDF) }" + " " + (viewModel.CurrencyCode == "IDR" ? "Rupiah" : viewModel.CurrencyCode == "USD" ? "Dollar" : viewModel.CurrencyCode), normal_font);
            cellLeftNoBorder.Colspan = 2;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            cellLeftNoBorder.Colspan = 4;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Perhitungan :", bold_font3);
            cellLeftNoBorder.Colspan = 4;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            //calculate vat and incomeTax 
            //double vat = 0;
            //double incomeTax = 0;
            //foreach(var item in viewModel.Items)
            //{
            //    if (item.IsPayVat)
            //    {
            //        vat += item.VatValue;
            //    }

            //    if (item.IsPayIncomeTax)
            //    {
            //        incomeTax += item.IncomeTaxValue;
            //    }
            //}

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Biaya", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.CurrencyCode + "  " + $"{viewModel.DPP.ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("(PPn)", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.CurrencyCode + "  " + $"{viewModel.VatValue.ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Total", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.CurrencyCode + "  " + $"{(viewModel.DPP + vat).ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            cellLeftNoBorder.Colspan = 4;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            cellLeftNoBorder.Colspan = 4;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            var pphDanliris = pphRate;
            //if (viewModel.IncomeTaxBy == "Efrata Garmindo Utama")
            //{
            //    pphDanliris = 0;
            //}

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("PPh " + pph, normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.CurrencyCode + "  " + $"{viewModel.IncomeTaxValue.ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);



            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Jumlah dibayar ke Supplier ", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 2;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.CurrencyCode + "  " + $"{(viewModel.DPP+ vat - incomeTax).ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Biaya Lain - Lain", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.CurrencyCode + "  " + $"{viewModel.MiscAmount.ToString("N", new CultureInfo("id-ID"))}", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            cellLeftNoBorder.Colspan = 4;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            cellLeftNoBorder.Colspan = 4;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            PdfPCell cellSuppLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellSuppMid = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellSuppRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            cellSuppLeft.Phrase = new Phrase("Total dibayar ke Supplier", normal_font);
            tableIdentity.AddCell(cellSuppLeft);
            cellSuppMid.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellSuppMid);
            cellSuppRight.Colspan = 2;
            cellSuppRight.Phrase = new Phrase(viewModel.CurrencyCode + "  " + $"{(AmountPDF).ToString("N", new CultureInfo("id-ID"))}", normal_font);
            tableIdentity.AddCell(cellSuppRight);
            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            cellLeftNoBorder.Colspan = 4;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            cellLeftNoBorder.Colspan = 4;
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Pembayaran ditransfer ke", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Bank, normal_font);
            cellLeftNoBorder.Colspan = 3;
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Dibayar ke Kas Negara", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.CurrencyCode + "  " + $"{(viewModel.IncomeTaxValue).ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);


            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingAfter = 15f;
            document.Add(tableIdentity);
            #endregion

            #region Content
            PdfPTable tableContent = new PdfPTable(9);
            tableContent.SetWidths(new float[] { 6f, 5f, 4f, 3f, 3f, 2.5f, 2.5f, 3.5f, 3f });

            cellCenter.Phrase = new Phrase("Nama Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("No PO Internal", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("No PO Eksternal", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("QTY Dipesan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("QTY Dibayar", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("QTY Sisa", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Satuan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Harga Satuan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("% Over Qty", bold_font);
            tableContent.AddCell(cellCenter);

            double total = 0;
            double totalPurchase = 0;

            foreach (FormItemDto item in viewModel.Items)
            {
                for (int indexItem = 0; indexItem < item.Details.Count; indexItem++)
                {
                    FormDetailDto detail = item.Details[indexItem];
                    var unitName = detail.UnitName;
                    //var unitName = "";
                    //var unitId = detail.Unit._id;
                    //if (unitId == "50")
                    //{
                    //    unitName = "WEAVING";
                    //}
                    //else if (unitId == "35")
                    //{
                    //    unitName = "SPINNING 1";
                    //}
                    //else
                    //{
                    //unitName = detail.UnitName;
                    //}
                    cellLeft.Colspan = 0;
                    cellLeft.Phrase = new Phrase($"{detail.ProductName}", smaller_font);
                    tableContent.AddCell(cellLeft);
                    cellCenter.Colspan = 0;
                    cellCenter.Phrase = new Phrase($"{detail.IPONo}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{item.EPONo}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{Math.Round(detail.QTYOrder, 2)}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{Math.Round(detail.QTYPaid, 2)}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{Math.Round(detail.QTYRemains, 2)}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{detail.QTYUnit}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellRightMerge.Phrase = new Phrase($"{detail.PricePerQTY.ToString("N", new CultureInfo("id-ID"))}", smaller_font);
                    tableContent.AddCell(cellRightMerge);

                    cellCenter.Phrase = new Phrase($"{detail.PercentageOverQTY.ToString("N", new CultureInfo("id-ID"))}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    double subtotalPrice = detail.PercentageOverQTY;

                    total += detail.PaidPrice;

                    totalPurchase += (detail.PricePerQTY * detail.QTYOrder);
                }
            }


            PdfPCell cellContent = new PdfPCell(tableContent); // dont remove
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 10f;
            document.Add(tableContent);
            #endregion

            #region note

            PdfPTable tableNote = new PdfPTable(3);
            tableNote.SetWidths(new float[] { 4f, 0.5f, 11f });

            cellLeftNoBorder.Phrase = new Phrase("Note :", bold_font3);
            cellLeftNoBorder.Colspan = 4;
            tableNote.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Kategori", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Category, normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Supplier / Agent", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.SupplierName, normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("No Order Confirmation", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.ConfirmationOrderNo, normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            //cellLeftNoBorder.Phrase = new Phrase("No Invoice", normal_font);
            //tableNote.AddCell(cellLeftNoBorder);
            //cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            //tableNote.AddCell(cellLeftNoBorder);
            //cellLeftNoBorder.Phrase = new Phrase(viewModel.InvoiceNo, normal_font);
            //tableNote.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("No Proforma/Invoice", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.ProformaNo, normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            //cellLeftNoBorder.Phrase = new Phrase("Investasi", normal_font);
            //tableNote.AddCell(cellLeftNoBorder);
            //cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            //tableNote.AddCell(cellLeftNoBorder);
            //cellLeftNoBorder.Phrase = new Phrase(viewModel.Investation, normal_font);
            //tableNote.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Mohon dibayar Tanggal", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.PaymentDueDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            //cellLeftNoBorder.Phrase = new Phrase("Bank", normal_font);
            //tableNote.AddCell(cellLeftNoBorder);
            //cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            //tableNote.AddCell(cellLeftNoBorder);
            //cellLeftNoBorder.Phrase = new Phrase(viewModel.Bank, normal_font);
            //tableNote.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Keterangan", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Remark, normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            var ppnPurchase = viewModel.VatValue > 0 ? (totalPurchase * 10 / 100) : 0;


            cellLeftNoBorder.Phrase = new Phrase("Total Pembelian", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            //cellLeftNoBorder.Phrase = new Phrase($"{viewModel.CurrencyCode}" + " " + $"{(totalPurchase + ppnPurchase).ToString("N", new CultureInfo("id-ID"))}", normal_font);
            cellLeftNoBorder.Phrase = new Phrase($"{viewModel.CurrencyCode}" + " " + $"{((viewModel.DPP + vat)- incomeTax).ToString("N", new CultureInfo("id-ID"))}", normal_font);

            tableNote.AddCell(cellLeftNoBorder);

            PdfPCell cellNote = new PdfPCell(tableNote); // dont remove
            tableNote.ExtendLastRow = false;
            tableNote.SpacingAfter = 20f;
            document.Add(tableNote);
            #endregion

            #region beban
            //PdfPTable tableBeban = new PdfPTable(1);
            //tableBeban.SetWidths(new float[] { 5f });
            //cellLeftNoBorder.Phrase = new Phrase("Beban Unit :", bold_font3); ;
            //tableBeban.AddCell(cellLeftNoBorder);

            //var AmountPerUnit = viewModel.Items.SelectMany(s => s.Details)
            //    .GroupBy(
            //    key => new { key.UnitId, key.UnitName, key.UnitCode },
            //    val => val,
            //    (key, val) => new { Key = key, Value = val}
            //    ).ToList();
            //foreach(var perUnit in AmountPerUnit)
            //{
            //    var sumPerUnit = perUnit.Value.Sum(t =>
            //    (t.PaidPrice) +
            //    (viewModel.Items.Where(a => a.Id == t.GarmentDispositionPurchaseItemId).FirstOrDefault().IsPayVat? t.PaidPrice * Convert.ToDouble(viewModel.Items.Where(a => a.Id == t.GarmentDispositionPurchaseItemId).First().VatRate) / 100 : 0) -
            //    (t.PaidPrice * (viewModel.Items.Where(a => a.Id == t.GarmentDispositionPurchaseItemId).FirstOrDefault()?.IncomeTaxRate / 100)))?.ToString("N", new CultureInfo("id-ID"));
            //    cellLeftNoBorder.Phrase = new Phrase($"- {perUnit.Key.UnitName} = {sumPerUnit}", bold_font3);
            //    tableBeban.AddCell(cellLeftNoBorder);
            //}
            //PdfPCell cellBeban = new PdfPCell(tableBeban); // dont remove
            //tableBeban.ExtendLastRow = false;
            //document.Add(tableBeban);
            #endregion

            #region signature
            PdfPTable tableSignature = new PdfPTable(5);
            tableSignature.SetWidths(new float[] { 5f, 5f, 4f, 4f, 4f });

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellSignatureContent.Phrase = new Phrase("", bold_font3);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("", bold_font3);
            cellSignatureContent.Colspan = 3;
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Colspan = 0;
            cellSignatureContent.Phrase = new Phrase("Sukoharjo, " + viewModel.CreatedUtc.ToString("dd MMMM yyyy", new CultureInfo("id-ID")), bold_font3);
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Colspan = 2;
            cellSignatureContent.Phrase = new Phrase("Disetujui,", bold_font3);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Mengetahui,", bold_font3);
            cellSignatureContent.Colspan = 2;
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Colspan = 0;
            cellSignatureContent.Phrase = new Phrase("Dibuat,", bold_font3);
            tableSignature.AddCell(cellSignatureContent);

            if (AmountPDF > 3000000)
            {
                cellSignatureContent.Colspan = 0;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(Direktur Akt & Keu)", bold_font3);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(Manager Akt & Keu)", bold_font3);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Colspan = 0;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(General Manager)", bold_font3);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(Manager Pembelian)", bold_font3);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(     Staff     )", bold_font3);
                tableSignature.AddCell(cellSignatureContent);
            }
            else
            {
                cellSignatureContent.Colspan = 2;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n( Manager Akt & Keu )", bold_font3);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Colspan = 0;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(   General Manager    )", bold_font3);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(   Manager Pembelian    )", bold_font3);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Colspan = 0;
                cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(     Staff     )", bold_font3);
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
            tableSignature.SpacingBefore = 10f;
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