using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class GarmentInternNotePDFTemplate
    {
        private class TableContent
        {
            public string BillNo { get; set; }
            public string PaymentBill { get; set; }
            public string DONo { get; set; }
            public string DODate { get; set; }
            public string RefNo { get; set; }
            public string Product { get; set; }
            public string Quantity { get; set; }
            public string UomUnit { get; set; }
            public string PricePerdealUnit { get; set; }
            public string PriceTotal { get; set; }
        }

        public MemoryStream GeneratePdfTemplate(GarmentInternNoteViewModel viewModel, IServiceProvider serviceProvider, int clientTimeZoneOffset, IGarmentDeliveryOrderFacade DOfacade)
        {
            IGarmentCorrectionNoteQuantityFacade correctionNote = (IGarmentCorrectionNoteQuantityFacade)serviceProvider.GetService(typeof(IGarmentCorrectionNoteQuantityFacade));

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font normal_font1 = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            //Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            Document document = new Document(PageSize.A4, 40, 40, 40, 40);
            document.AddHeader("Header", viewModel.inNo);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = new PDFPages();
            document.Open();


            PdfPCell cellLeftNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellRightNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

            Chunk chkHeader = new Chunk(" ");
            Phrase pheader = new Phrase(chkHeader);
            HeaderFooter header = new HeaderFooter(pheader, false);
            header.Border = Rectangle.NO_BORDER;
            header.Alignment = Element.ALIGN_RIGHT;
            document.Header = header;

            #region Header

            //string addressString = "PT DAN LIRIS" + "\n" + "JL. Merapi No.23" + "\n" + "Banaran, Grogol, Kab. Sukoharjo" + "\n" + "Jawa Tengah 57552 - INDONESIA" + "\n" + "PO.BOX 166 Solo 57100" + "\n" + "Telp. (0271) 740888, 714400" + "\n" + "Fax. (0271) 735222, 740777";
            string addressString = "PT EFRATA GARMINDO UTAMA" + "\n" + "Banaran, Grogol, Sukoharjo" + "\n" + "Jawa Tengah 57552 - INDONESIA" + "\n" + "Telp (+62 271)719911, (+62 21)2900977";
            Paragraph address = new Paragraph(addressString, bold_font) { Alignment = Element.ALIGN_LEFT };
            document.Add(address);
            bold_font.SetStyle(Font.NORMAL);

            string titleString = "NOTA INTERN\n\n";
            Paragraph title = new Paragraph(titleString, bold_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);
            bold_font.SetStyle(Font.NORMAL);

            PdfPTable tableInternNoteHeader = new PdfPTable(2);
            tableInternNoteHeader.SetWidths(new float[] { 4.5f, 4.5f });
            PdfPCell cellInternNoteHeaderLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellInternNoteHeaderRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

            cellInternNoteHeaderLeft.Phrase = new Phrase("No. Nota Intern" + "      : " + viewModel.inNo, normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            cellInternNoteHeaderRight.Phrase = new Phrase("Tanggal Nota Intern" + "       : " + viewModel.inDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);

            cellInternNoteHeaderLeft.Phrase = new Phrase("Kode Supplier" + "        : " + viewModel.supplier.Code, normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            string paymentmethods = "";
            List<DateTimeOffset> coba = new List<DateTimeOffset>();
            foreach (GarmentInternNoteItemViewModel item in viewModel.items)
            {
                foreach (GarmentInternNoteDetailViewModel detail in item.details)
                {
                    coba.Add(detail.paymentDueDate);
                    paymentmethods = detail.deliveryOrder.paymentMethod;
                }
            }
            DateTimeOffset coba1 = coba.Min(p => p);
            cellInternNoteHeaderRight.Phrase = new Phrase("Tanggal Jatuh Tempo" + "    : " + coba1.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);

            cellInternNoteHeaderLeft.Phrase = new Phrase("Nama Supplier" + "       : " + viewModel.supplier.Name, normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            cellInternNoteHeaderRight.Phrase = new Phrase("Term Pembayaran" + "         : " + paymentmethods, normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);


            PdfPCell cellInternNoteHeader = new PdfPCell(tableInternNoteHeader); // dont remove
            tableInternNoteHeader.ExtendLastRow = false;
            tableInternNoteHeader.SpacingAfter = 10f;
            document.Add(tableInternNoteHeader);
            #endregion

            #region Table_Of_Content
            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            //PdfPTable tableContent = new PdfPTable(10);
            PdfPTable tableContent = new PdfPTable(9);
            //tableContent.SetWidths(new float[] { 4f, 4f, 4f, 4.5f, 5f, 4.3f, 3.3f, 2.9f, 4.2f, 4.3f });
            tableContent.SetWidths(new float[] {  4f, 4f, 4.5f, 5f, 4.3f, 3.3f, 2.9f, 4.2f, 4.3f });
            //cellCenter.Phrase = new Phrase("NO. Bon Pusat", bold_font);
            //tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("NO. BP Kecil", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("NO. Surat Jalan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Tgl. Surat Jalan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("No. Referensi PR", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Keterangan Barang", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Jumlah", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Satuan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Harga Satuan", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Harga Total", bold_font);
            tableContent.AddCell(cellCenter);

            double totalPriceTotal = 0;
            double total = 0;
            double ppn = 0;
            double pph = 0;
            double maxtotal = 0;
            decimal totalcorrection = 0;
            Dictionary<string, double> units = new Dictionary<string, double>();
            units.Add("EFR", 0);
            //units.Add("AG2", 0);
            
            Dictionary<long, decimal> koreksi = new Dictionary<long, decimal>();
            Dictionary<long, double> kurs = new Dictionary<long, double>();

            List<TableContent> TableContents = new List<TableContent>();

            foreach (GarmentInternNoteItemViewModel item in viewModel.items)
            {
                foreach (GarmentInternNoteDetailViewModel detail in item.details)
                {
                    TableContents.Add(new TableContent
                    {
                        //BillNo = detail.deliveryOrder.billNo,
                        PaymentBill = detail.deliveryOrder.paymentBill,
                        DONo = detail.deliveryOrder.doNo,
                        DODate = detail.deliveryOrder.doDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")),
                        RefNo = detail.poSerialNumber + " - " + detail.ePONo,
                        Product = detail.product.Name,
                        Quantity = detail.quantity.ToString("N", new CultureInfo("id-ID")),
                        UomUnit = detail.uomUnit.Unit,
                        PricePerdealUnit = detail.pricePerDealUnit.ToString("N", new CultureInfo("id-ID")),
                        PriceTotal = detail.priceTotal.ToString("N", new CultureInfo("id-ID"))
                    });

                    totalPriceTotal += detail.priceTotal;

                    total += detail.priceTotal * detail.deliveryOrder.docurrency.Rate;

                    if (units.ContainsKey(detail.unit.Code))
                    {
                        units[detail.unit.Code] += detail.priceTotal;
                    }
                    else
                    {
                        units.Add(detail.unit.Code, detail.priceTotal);
                    }

                    var correctionNotes = correctionNote.ReadByDOId((int)detail.deliveryOrder.Id);

                    if (!koreksi.ContainsKey(detail.deliveryOrder.Id))
                    {
                        totalcorrection += correctionNotes.Sum(s =>
                        {
                            if (s.CorrectionType.ToUpper() == "RETUR")
                            {
                                return s.Items.Sum(i => i.PricePerDealUnitAfter * i.Quantity);
                            }
                            else
                            {
                                return s.TotalCorrection;
                            }
                        });
                        koreksi.Add(detail.deliveryOrder.Id, correctionNotes.Sum(s => s.TotalCorrection));
                    }

                    if (item.garmentInvoice.useVat == true && item.garmentInvoice.isPayVat == true)
                    {
                        ppn = (item.garmentInvoice.vatRate / 100) * (totalPriceTotal + (double)totalcorrection);
                    }
                    else if (item.garmentInvoice.isPayVat == false)
                    {
                        ppn = 0;
                    }

                    if (item.garmentInvoice.useIncomeTax == true && item.garmentInvoice.isPayTax == true)
                    {
                        pph = (item.garmentInvoice.incomeTaxRate / 100) * (totalPriceTotal + (double)totalcorrection);
                    }
                    else if (item.garmentInvoice.isPayTax == false)
                    {
                        pph = 0;
                    }

                    maxtotal = (totalPriceTotal + ppn - pph) + (double)totalcorrection;
                }
            }

            foreach (TableContent c in TableContents.OrderBy(o => o.DONo))
            {
                //cellLeft.Phrase = new Phrase(c.BillNo, normal_font1);
                //tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(c.PaymentBill, normal_font1);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(c.DONo, normal_font1);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(c.DODate, normal_font1);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(c.RefNo, normal_font1);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(c.Product, normal_font1);
                tableContent.AddCell(cellLeft);

                cellRight.Phrase = new Phrase(c.Quantity, normal_font1);
                tableContent.AddCell(cellRight);

                cellRight.Phrase = new Phrase(c.UomUnit, normal_font1);
                tableContent.AddCell(cellRight);

                cellRight.Phrase = new Phrase(c.PricePerdealUnit, normal_font1);
                tableContent.AddCell(cellRight);

                cellRight.Phrase = new Phrase(c.PriceTotal, normal_font1);
                tableContent.AddCell(cellRight);
            }

            PdfPCell cellContent = new PdfPCell(tableContent); // dont remove
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 20f;
            document.Add(tableContent);
            #endregion

            #region Footer

            PdfPTable tableFooter = new PdfPTable(2);
            tableFooter.SetWidths(new float[] { 1f, 1f });

            PdfPTable tableFooterLeft = new PdfPTable(2);
            tableFooterLeft.SetWidths(new float[] { 3f, 5f });

            PdfPCell cellInternNoteFooterLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellInternNoteFooterRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            foreach (var unit in units)
            {
                if (unit.Value == 0)
                {

                    cellLeftNoBorder.Phrase = new Phrase($"Total {unit.Key}", normal_font);
                    tableFooterLeft.AddCell(cellLeftNoBorder);
                    cellLeftNoBorder.Phrase = new Phrase($":   -", normal_font);
                    tableFooterLeft.AddCell(cellLeftNoBorder);
                }
                else
                {
                    cellLeftNoBorder.Phrase = new Phrase($"Total {unit.Key}", normal_font);
                    tableFooterLeft.AddCell(cellLeftNoBorder);
                    cellLeftNoBorder.Phrase = new Phrase($":   {unit.Value.ToString("n", new CultureInfo("id-ID"))}", normal_font);
                    tableFooterLeft.AddCell(cellLeftNoBorder);
                }
            }

            PdfPTable tableFooterRight = new PdfPTable(2);
            tableFooterRight.SetWidths(new float[] { 5f, 5f });

            cellLeftNoBorder.Phrase = new Phrase($"Total Harga Pokok (DPP)", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($": " + totalPriceTotal.ToString("N", new CultureInfo("id-ID")), normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Mata Uang", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($": " + viewModel.currency.Code, normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Total Harga Pokok (Rp)", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($": " + total.ToString("N", new CultureInfo("id-ID")), normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Total Nota Koreksi", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            if (correctionNote != null)
            {
                cellLeftNoBorder.Phrase = new Phrase($": " + totalcorrection.ToString("N", new CultureInfo("id-ID")), normal_font);
                tableFooterRight.AddCell(cellLeftNoBorder);
            }
            else
            {
                cellLeftNoBorder.Phrase = new Phrase($": " + 0, normal_font);
                tableFooterRight.AddCell(cellLeftNoBorder);
            }

            cellLeftNoBorder.Phrase = new Phrase("Total Nota PPn", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($": " + ppn.ToString("N", new CultureInfo("id-ID")), normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Total Nota PPh", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($": " + pph.ToString("N", new CultureInfo("id-ID")), normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase("Total yang Harus Dibayar", normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            cellLeftNoBorder.Phrase = new Phrase($": " + maxtotal.ToString("N", new CultureInfo("id-ID")), normal_font);
            tableFooterRight.AddCell(cellLeftNoBorder);

            PdfPCell cellFooterLeft = new PdfPCell(tableFooterLeft) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            tableFooter.AddCell(cellFooterLeft);
            PdfPCell cellFooterRight = new PdfPCell(tableFooterRight) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            tableFooter.AddCell(cellFooterRight);


            PdfPCell cellFooter = new PdfPCell(tableFooter); // dont remove
            tableFooter.ExtendLastRow = false;
            tableFooter.SpacingAfter = 20f;
            document.Add(tableFooter);

            #endregion

            #region TableSignature

            PdfPTable tableSignature = new PdfPTable(5);

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellSignatureContent.Phrase = new Phrase("Staff Pembelian\n\n\n\n\n\n\n(  " + "Nama & Tanggal" + "  )", bold_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Manager Pembelian\n\n\n\n\n\n\n(  " + "Nama & Tanggal" + "  )", bold_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Verifikasi\n\n\n\n\n\n\n(  " + "Nama & Tanggal" + "  )", bold_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Manager Keuangan\n\n\n\n\n\n\n(  " + "Nama & Tanggal" + "  )", bold_font);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Anggaran\n\n\n\n\n\n\n(  " + "Nama & Tanggal" + "  )", bold_font);
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
