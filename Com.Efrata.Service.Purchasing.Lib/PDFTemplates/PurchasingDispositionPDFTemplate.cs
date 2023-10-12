using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class PurchasingDispositionPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(PurchasingDispositionViewModel viewModel, int clientTimeZoneOffset)
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


            Document document = new Document(PageSize.A4, 30, 30, 30, 30);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            //writer.PageEvent = new TextEvents(viewModel.DispositionNo);
            document.Open();

            string fmString = "FM-PB-00-06-011";
            Paragraph fm = new Paragraph(fmString, bold_font4) { Alignment = Element.ALIGN_RIGHT };

            string titleString = "DISPOSISI PEMBAYARAN";
            Paragraph title = new Paragraph(titleString, bold_font4) { Alignment = Element.ALIGN_CENTER };

            document.Add(title);
            bold_font.SetStyle(Font.NORMAL);


            string NoString = "NO : " + viewModel.DispositionNo;
            Paragraph dispoNumber = new Paragraph(NoString, bold_font4) { Alignment = Element.ALIGN_CENTER };
            dispoNumber.SpacingAfter = 20f;
            document.Add(dispoNumber);



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
            double ppnRate = 0;

            foreach (var item in viewModel.Items)
            {
                if (!item.UseVat)
                {
                    ppn = 0;
                }
                else
                {
                    ppnRate = (Convert.ToDouble(item.vatTax.rate) / 100);
                    ppn = (dpp * (Convert.ToDouble(item.vatTax.rate) / 100));
                }
                if (item.UseIncomeTax)
                {
                    pph = item.IncomeTax.name;
                    pphRate = dpp * (Convert.ToDouble(item.IncomeTax.rate) / 100);
                }
                break;
            }

            //Jumlah dibayar ke Supplier
            double paidToSupp = dpp + ppn - pphRate;
            if (viewModel.IncomeTaxBy.ToUpper() == "EFRATA GARMINDO UTAMA")
            {
                paidToSupp = dpp + ppn;
            }

            double amount = dpp + ppn;

            if (viewModel.IncomeTaxBy.ToUpper() == "SUPPLIER")
            {
                amount = dpp + ppn - pphRate;
            }

            var amountPDF = amount + viewModel.PaymentCorrection;
            double paidDispo = amountPDF + pphRate;
            var payingDisposition = Math.Round((paidToSupp + viewModel.PaymentCorrection), 2, MidpointRounding.AwayFromZero);
            cellLeftNoBorder.SetLeading(13f, 0f);
            cellLeftNoBorder.Phrase = new Phrase("Mohon Disposisi Pembayaran", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.PaymentMethod + "  " + viewModel.Currency.code + " " + $"{paidDispo.ToString("N", new CultureInfo("id-ID"))}", normal_font);
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
            cellLeftNoBorder.Phrase = new Phrase($"{ NumberToTextIDN.terbilangv2(paidDispo) }" + " " + viewModel.Currency.description.ToLower(), normal_font);
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


            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Biaya", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Currency.code + "  " + $"{viewModel.DPP.ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("(PPn)", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Currency.code + "  " + $"{ppn.ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Total", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Currency.code + "  " + $"{(dpp + ppn).ToString("N", new CultureInfo("id-ID")) }", normal_font);
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
            if (viewModel.IncomeTaxBy == "Efrata Garmindo Utama")
            {
                pphDanliris = 0;
            }

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("PPh pasal " + pph, normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Currency.code + "  " + $"{pphDanliris.ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);



            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Jumlah dibayar ke Supplier ", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 2;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Currency.code + "  " + $"{(paidToSupp).ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Koreksi Bayar", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Colspan = 3;
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Currency.code + "  " + $"{viewModel.PaymentCorrection.ToString("N", new CultureInfo("id-ID"))}", normal_font);
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
            cellSuppRight.Phrase = new Phrase(viewModel.Currency.code + "  " + $"{(amountPDF).ToString("N", new CultureInfo("id-ID"))}", normal_font);
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
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Currency.code + "  " + $"{(pphRate).ToString("N", new CultureInfo("id-ID")) }", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);


            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingAfter = 15f;
            document.Add(tableIdentity);
            #endregion

            #region Content
            PdfPTable tableContent = new PdfPTable(8);
            tableContent.SetWidths(new float[] { 6f, 5f, 4f, 3f, 3f, 2.5f, 2f, 3.5f });

            cellCenter.Phrase = new Phrase("Nama Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("No PR", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("No PO Eksternal", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Unit", bold_font);
            tableContent.AddCell(cellCenter);
            //cellCenter.Phrase = new Phrase("Kategori", bold_font);
            //tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Quantity", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Satuan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Harga Satuan", bold_font);
            cellCenter.Colspan = 2;
            tableContent.AddCell(cellCenter);
            //cellCenter.Phrase = new Phrase("Harga yang dibayar", bold_font);
            //cellCenter.Colspan = 2;
            //tableContent.AddCell(cellCenter);

            double total = 0;
            double totalPurchase = 0;
            double ppnpurchase = 0;
            foreach (PurchasingDispositionItemViewModel item in viewModel.Items)
            {
                for (int indexItem = 0; indexItem < item.Details.Count; indexItem++)
                {
                    PurchasingDispositionDetailViewModel detail = item.Details[indexItem];
                    //var unitName = detail.Unit._id == "50" ? "WEAVING" : detail.Unit.name;
                    var unitName = "";
                    var unitId = detail.Unit._id;
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
                        unitName = detail.Unit.name;
                    }
                    cellLeft.Colspan = 0;
                    cellLeft.Phrase = new Phrase($"{detail.Product.name}", smaller_font);
                    tableContent.AddCell(cellLeft);
                    cellCenter.Colspan = 0;
                    cellCenter.Phrase = new Phrase($"{detail.PRNo}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{item.EPONo}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{unitName}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    //cellCenter.Phrase = new Phrase($"{detail.Category.name}", smaller_font);
                    //tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase(string.Format("{0:n2}", detail.PaidQuantity), smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellCenter.Phrase = new Phrase($"{detail.DealUom.unit}", smaller_font);
                    tableContent.AddCell(cellCenter);

                    cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.code}", smaller_font);
                    tableContent.AddCell(cellLeftMerge);

                    cellRightMerge.Phrase = new Phrase($"{detail.PricePerDealUnit.ToString("N", new CultureInfo("id-ID"))}", smaller_font);
                    tableContent.AddCell(cellRightMerge);

                    double subtotalPrice = detail.PaidPrice * detail.PaidQuantity;

                    //cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.code}", smaller_font);
                    //tableContent.AddCell(cellLeftMerge);

                    //cellRightMerge.Phrase = new Phrase($"{detail.PaidPrice.ToString("N", new CultureInfo("id-ID"))}", smaller_font);
                    //tableContent.AddCell(cellRightMerge);

                    total += detail.PaidPrice;

                    totalPurchase += (detail.PricePerDealUnit * detail.DealQuantity);

                }
            }


            //cellRight.Colspan = 8;
            //cellRight.Phrase = new Phrase("Total Amount", bold_font);
            //tableContent.AddCell(cellRight);

            //cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.code}", smaller_font);
            //tableContent.AddCell(cellLeftMerge);
            //cellRightMerge.Phrase = new Phrase($"{total.ToString("N", new CultureInfo("id-ID"))}", smaller_font);
            //tableContent.AddCell(cellRightMerge);


            PdfPCell cellContent = new PdfPCell(tableContent); // dont remove
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 20f;
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
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Category.name, normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Colspan = 0;
            cellLeftNoBorder.Phrase = new Phrase("Supplier / Agent", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Supplier.name, normal_font);
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

            cellLeftNoBorder.Phrase = new Phrase("No Proforma", normal_font);
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

            cellLeftNoBorder.Phrase = new Phrase("Perhitungan", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Calculation, normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Keterangan", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(viewModel.Remark, normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            var ppnPurchase = viewModel.VatValue > 0 ? (totalPurchase * ppnRate) : 0;

            cellLeftNoBorder.Phrase = new Phrase("Total Pembelian", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(":", normal_font);
            tableNote.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase($"{viewModel.Currency.code}" + " " + $"{(totalPurchase + ppnPurchase).ToString("N", new CultureInfo("id-ID"))}", normal_font);
            tableNote.AddCell(cellLeftNoBorder);

            PdfPCell cellNote = new PdfPCell(tableNote); // dont remove
            tableNote.ExtendLastRow = false;
            tableNote.SpacingAfter = 20f;
            document.Add(tableNote);
            #endregion

            #region signature
            PdfPTable tableSignature = new PdfPTable(4);
            tableSignature.SetWidths(new float[] { 4f, 4f, 4f, 4.1f });

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellSignatureContent.Phrase = new Phrase("", bold_font3);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("", bold_font3);
            cellSignatureContent.Colspan = 2;
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Colspan = 0;
            cellSignatureContent.Phrase = new Phrase("Sukoharjo, " + viewModel.CreatedUtc.ToString("dd MMMM yyyy", new CultureInfo("id-ID")), bold_font3);
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Phrase = new Phrase("Menyetujui,", bold_font3);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Mengetahui,", bold_font3);
            cellSignatureContent.Colspan = 2;
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Colspan = 0;
            cellSignatureContent.Phrase = new Phrase("Hormat Kami,", bold_font3);
            tableSignature.AddCell(cellSignatureContent);

            cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(                                       )", bold_font3);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(                                       )", bold_font3);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(                                       )", bold_font3);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("\n\n\n\n\n\n\n(                                       )", bold_font3);
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

    public class TextEvents : PdfPageEventHelper
    {
        private readonly string _documentNo;
        private DateTime _printTime;
        private BaseFont _baseFont;
        private PdfContentByte _cb;
        private PdfTemplate _headerTemplate;
        private PdfTemplate _footerTemplate;

        public TextEvents(string documentNo) : base()
        {
            _documentNo = documentNo;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                _printTime = DateTime.Now;
                _baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                _cb = writer.DirectContent;
                _headerTemplate = _cb.CreateTemplate(100, 100);
                _footerTemplate = _cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (IOException ioe)
            {
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            var baseFontNormal = new Font(Font.HELVETICA, 12f, Font.NORMAL, BaseColor.Black);
            var baseFontBig = new Font(Font.HELVETICA, 12f, Font.BOLD, BaseColor.Black);
            var p1Header = $"DISPOSISI PEMBAYARAN\nNO : {_documentNo}";

            //if (writer.PageNumber == 1)
            //    p1Header = "";

            //Create PdfTable object
            var pdfTab = new PdfPTable(3);

            //We will have to create separate cells to include image logo and 2 separate strings
            //Row 1
            var text = writer.PageNumber.ToString();

            var pdfCell1 = new PdfPCell();
            var pdfCell2 = new PdfPCell(new Phrase(text, baseFontNormal));
            var pdfCell3 = new PdfPCell();

            ////Add paging to header
            {
                _cb.BeginText();
                _cb.SetFontAndSize(_baseFont, 11);
                _cb.SetTextMatrix(document.PageSize.GetRight(document.PageSize.Width / 2), document.PageSize.GetTop(20));
                _cb.ShowTextAligned(Element.ALIGN_CENTER, "DISPOSISI PEMBAYARAN", document.PageSize.GetRight(document.PageSize.Width / 2), document.PageSize.GetTop(50), 0);
                _cb.ShowTextAligned(Element.ALIGN_CENTER, $"NO : {_documentNo}", document.PageSize.GetRight(document.PageSize.Width / 2), document.PageSize.GetTop(62), 0);
                //_cb.ShowText(p1Header);
                _cb.EndText();
                var len = _baseFont.GetWidthPoint(p1Header, 12);
                //Adds "12" in Page 1 of 12
                _cb.AddTemplate(_headerTemplate, document.PageSize.GetRight(150) + len, document.PageSize.GetTop(20));
            }

            //Add paging to footer
            {
                _cb.BeginText();
                _cb.SetFontAndSize(_baseFont, 11);
                _cb.ShowTextAligned(Element.ALIGN_CENTER, text, document.PageSize.GetRight(document.PageSize.Width / 2), document.PageSize.GetBottom(10), 0);
                //_cb.ShowText(text);
                _cb.EndText();
                var len1 = _baseFont.GetWidthPoint(text, 12);
                _cb.AddTemplate(_footerTemplate, document.PageSize.GetRight(document.PageSize.Width / 2) + len1, document.PageSize.GetBottom(10));
            }

            //set the alignment of all three cells and set border to 0
            pdfCell1.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;

            pdfCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
            pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;

            pdfCell1.Border = 0;
            pdfCell2.Border = 0;
            pdfCell3.Border = 0;

            //add all three cells into PdfTable
            pdfTab.AddCell(pdfCell1);
            pdfTab.AddCell(pdfCell2);
            pdfTab.AddCell(pdfCell3);

            pdfTab.TotalWidth = document.PageSize.Width - 80f;
            pdfTab.WidthPercentage = 70;
            //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;    

            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write
            //Third and fourth param is x and y position to start writing
            //pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
            //set pdfContent value

            //Move the pointer and draw line to separate header section from rest of page
            //_cb.MoveTo(40, document.PageSize.Height - 100);
            //_cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
            //_cb.Stroke();

            //Move the pointer and draw line to separate footer section from rest of page
            //_cb.MoveTo(40, document.PageSize.GetBottom(50));
            //_cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
            //_cb.Stroke();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            //_headerTemplate.al
            //_headerTemplate.BeginText();
            //_headerTemplate.SetFontAndSize(_baseFont, 12);
            //_headerTemplate.SetTextMatrix(0, 0);
            //_headerTemplate.ShowText($"No: {_documentNo}");
            //_headerTemplate.EndText();

            //_footerTemplate.BeginText();
            //_footerTemplate.SetFontAndSize(_baseFont, 12);
            //_footerTemplate.SetTextMatrix(0, 0);
            //_footerTemplate.ShowText((writer.PageNumber - 1).ToString());
            //_footerTemplate.EndText();
        }
    }
}
