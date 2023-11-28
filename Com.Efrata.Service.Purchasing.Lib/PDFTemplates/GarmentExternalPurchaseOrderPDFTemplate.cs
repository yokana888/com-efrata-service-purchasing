using Com.Efrata.Service.Purchasing.Lib.Helpers;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class GarmentExternalPurchaseOrderPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(GarmentExternalPurchaseOrderViewModel viewModel, int clientTimeZoneOffset)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            //Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font table_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

            Document document = new Document(PageSize.A4, 40, 40, 40, 40);
            document.AddHeader("Header", viewModel.EPONo);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = new PDFPages();
            document.Open();

            string EPONo = viewModel.IsOverBudget ? viewModel.EPONo + "-OB" : viewModel.EPONo;


            Chunk chkHeader = new Chunk(EPONo, bold_font);
            Phrase pheader = new Phrase(chkHeader);
            HeaderFooter header = new HeaderFooter(pheader, false);
            header.Border = Rectangle.NO_BORDER;
            header.Alignment = Element.ALIGN_RIGHT;
            document.Header = header;

            #region Header

            PdfPTable tableHeader = new PdfPTable(2);
            tableHeader.SetWidths(new float[] { 4f, 4f });
            PdfPCell cellHeaderContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellHeaderContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

            //cellHeaderContentLeft.Phrase = new Phrase("PT EFRATA GARMINDO UTAMA" + "\n" + "JL. Merapi No.23" + "\n" + "Banaran, Grogol, Kab. Sukoharjo" + "\n" + "Jawa Tengah 57552 - INDONESIA" + "\n" + "PO.BOX 166 Solo 57100" + "\n" + "Telp. (0271) 740888, 714400" + "\n" + "Fax. (0271) 735222, 740777", bold_font);
            cellHeaderContentLeft.Phrase = new Phrase("PT EFRATA GARMINDO UTAMA" +  "\n" + "Jl. Merapi No.23 Blok E1, Desa/Kelurahan Banaran," + "\n" + "Kec. Grogol, Kab. Sukoharjo, Provinsi Jawa Tengah" + "\n" + "Kode Pos: 57552, Telp: 02711740888", bold_font);
            tableHeader.AddCell(cellHeaderContentLeft);

            string noPO = EPONo;

            //string noPO = viewModel.Supplier.Import ? "FM-PB-00-06-009/R1" + "\n" + "PO: " + EPONo  : "PO: " + EPONo;

            cellHeaderContentRight.Phrase = new Phrase(noPO, bold_font);
            tableHeader.AddCell(cellHeaderContentRight);

            PdfPCell cellHeader = new PdfPCell(tableHeader); // dont remove
            tableHeader.ExtendLastRow = false;
            tableHeader.SpacingAfter = 10f;
            document.Add(tableHeader);

            string titleString = viewModel.Supplier.Import?"PURCHASE ORDER": "ORDER PEMBELIAN";
            Paragraph title = new Paragraph(titleString, bold_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);
            bold_font.SetStyle(Font.NORMAL);

            PdfPTable tableSupplier = new PdfPTable(3);
            tableSupplier.SetWidths(new float[] { 1.2f, 4f, 4f });
            PdfPCell cellSupplierLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellSupplierRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

            if (viewModel.Supplier.Import)
            {
                cellSupplierLeft.Phrase = new Phrase("Supplier :", normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierLeft.Phrase = new Phrase(viewModel.Supplier.Name, normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierRight.Phrase = new Phrase("Sukoharjo, " + viewModel.OrderDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("en-EN")), normal_font);
                tableSupplier.AddCell(cellSupplierRight);

                cellSupplierLeft.Phrase = new Phrase("", normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierLeft.Phrase = new Phrase("Attn. " + viewModel.Supplier.PIC, normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierLeft.Phrase = new Phrase("", normal_font);
                tableSupplier.AddCell(cellSupplierLeft);
            }
            else
            {
                cellSupplierLeft.Phrase = new Phrase("Kepada Yth :", normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierLeft.Phrase = new Phrase(viewModel.Supplier.Name, normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierRight.Phrase = new Phrase("Sukoharjo, " + viewModel.OrderDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
                tableSupplier.AddCell(cellSupplierRight);

                cellSupplierLeft.Phrase = new Phrase("", normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierLeft.Phrase = new Phrase("Attn. " + viewModel.Supplier.PIC, normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierLeft.Phrase = new Phrase("", normal_font);
                tableSupplier.AddCell(cellSupplierLeft);


                cellSupplierLeft.Phrase = new Phrase("", normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierLeft.Phrase = new Phrase("Telp. " + viewModel.Supplier.Contact, normal_font);
                tableSupplier.AddCell(cellSupplierLeft);

                cellSupplierLeft.Phrase = new Phrase("", normal_font);
                tableSupplier.AddCell(cellSupplierLeft);
            }
            

            PdfPCell cellSupplier = new PdfPCell(tableSupplier); // dont remove
            tableSupplier.ExtendLastRow = false;
            tableSupplier.SpacingAfter = 10f;
            document.Add(tableSupplier);
            #endregion

            if (viewModel.Supplier.Import)
            {
                string p1 = "The undersigned below, PT. EFRATA GARMINDO UTAMA, SOLO (hereinafter referred to as parties Purchasers) and " + viewModel.Supplier.Name + " (hereinafter referred to as seller\'s side) mutually agreed to enter into a sale and purchase contract with the following conditions: ";
                Paragraph firstParagraph = new Paragraph(p1, normal_font) { Alignment = Element.ALIGN_LEFT };

                firstParagraph.SpacingAfter = 10f;
                document.Add(firstParagraph);
            }
            else
            {
                string p1 = "Dengan hormat,\nYang bertanda tangan di bawah ini, PT. EFRATA GARMINDO UTAMA, SOLO (selanjutnya disebut sebagai pihak Pembeli) dan " + viewModel.Supplier.Name + " (selanjutnya disebut sebagai pihak Penjual) saling menyetujui untuk mengadakan kontrak jual beli dengan ketentuan sebagai berikut: ";
                Paragraph firstParagraph = new Paragraph(p1, normal_font) { Alignment = Element.ALIGN_LEFT };

                firstParagraph.SpacingAfter = 10f;
                document.Add(firstParagraph);
            }



            #region data
            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            PdfPCell cellRightMerge = new PdfPCell() { Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeftMerge = new PdfPCell() { Border = Rectangle.NO_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            PdfPTable tableContent = new PdfPTable(8);
            tableContent.SetWidths(new float[] { 1.2f,7f, 3.5f, 4f, 1.5f, 2.5f, 1.5f, 3f });
            if (viewModel.Supplier.Import)
            {
                cellCenter.Phrase = new Phrase("NO", bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("DESCRIPTION OF GOODS", bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("ARTICLE", bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("QUANTITY", bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("UNIT PRICE", bold_font);
                cellCenter.Colspan = 2;
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("SUB TOTAL", bold_font);
                cellCenter.Colspan = 2;
                tableContent.AddCell(cellCenter);
            }
            else
            {
                cellCenter.Phrase = new Phrase("NO", bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("NAMA DAN JENIS BARANG", bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("ARTIKEL", bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("JUMLAH", bold_font);
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("HARGA SATUAN", bold_font);
                cellCenter.Colspan = 2;
                tableContent.AddCell(cellCenter);
                cellCenter.Phrase = new Phrase("SUB TOTAL", bold_font);
                cellCenter.Colspan = 2;
                tableContent.AddCell(cellCenter);
            }
                

            double total = 0;
            int index = 0;
            double qtyTotal = 0;
            foreach (GarmentExternalPurchaseOrderItemViewModel item in viewModel.Items)
            {
                index++;

                double subTotal = item.PricePerDealUnit * item.DealQuantity;
                string productRemark = "";
                qtyTotal += item.DealQuantity;

                if (viewModel.Category.ToLower().Contains("fabric"))
                {
                    if (viewModel.Supplier.Import)
                    {
                        productRemark = item.Product.Code + "-" + item.Product.Name + "\nCOMPOSITION : " + item.Product.Composition + "\nCONSTRUCTION : " + item.Product.Const + "\nYARN : " + item.Product.Yarn + "\nWIDTH : " + item.Product.Width + "\nQUALITY : " + "EXPORT QUALITY" + "\nREMARK :" + item.Remark + "\n" + item.PRNo + "-" + item.PO_SerialNumber;
                    }
                    else
                    {
                        productRemark = item.Product.Code + "-" + item.Product.Name + "\nCOMPOSITION : " + item.Product.Composition + "\nKONSTRUKSI : " + item.Product.Const + "\nYARN : " + item.Product.Yarn + "\nLEBAR : " + item.Product.Width + "\nQUALITY : " + "EXPORT QUALITY" + "\nKETERANGAN :" + item.Remark + "\n" + item.PRNo + "-" + item.PO_SerialNumber;
                    }
                   
                    
                }
                else
                {
                    productRemark = item.Product.Code + "-" + item.Product.Name + "\n" + item.Remark + "\n" + item.PRNo + "-" + item.PO_SerialNumber;
                }

                if (item.IsOverBudget)
                {
                    productRemark = productRemark + "-OB";
                }

                cellLeft.Phrase = new Phrase(index.ToString(), table_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(productRemark, table_font);
                tableContent.AddCell(cellLeft);

                string shipmentDate = viewModel.Supplier.Import ? item.ShipmentDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("en-EN")) : item.ShipmentDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
                cellLeft.Phrase = new Phrase(item.Article + " - " + item.RONo + " - " + shipmentDate, table_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(string.Format("{0:n2}", item.DealQuantity) + " " + $"{item.DealUom.Unit}", table_font);
                tableContent.AddCell(cellLeft);

                cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.Code}", table_font);
                tableContent.AddCell(cellLeftMerge);

                cellRightMerge.Phrase = new Phrase(string.Format("{0:n4}", item.PricePerDealUnit), table_font);
                tableContent.AddCell(cellRightMerge);

                cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.Code}", table_font);
                tableContent.AddCell(cellLeftMerge);

                cellRightMerge.Phrase = new Phrase(string.Format("{0:n2}", subTotal), table_font);
                tableContent.AddCell(cellRightMerge);

                total += subTotal;
            }

            if (viewModel.Supplier.Import)
            {
                cellRight.Colspan = 3;
                cellRight.Phrase = new Phrase("Total Quantity", bold_font);
                tableContent.AddCell(cellRight);

                cellLeft.Phrase = new Phrase($"{qtyTotal.ToString("N2", new CultureInfo("en-EN"))}", table_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase("Total", bold_font);
                cellLeft.Colspan = 2;
                tableContent.AddCell(cellLeft);

                cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.Code}", table_font);
                tableContent.AddCell(cellLeftMerge);

                var isNotePpnShow = viewModel.IsUseVat && !viewModel.IsPayVAT;
                //var notePpnShow = isNotePpnShow ? "\n 10% VAT Defferred" : "";
                var notePpnShow = isNotePpnShow ? "\n "+viewModel.Vat.Rate+"% VAT Defferred" : "";
                //var additionalAmount = viewModel.IsUseVat ? total * 0.1:0;
                //total = total + additionalAmount;
                cellRightMerge.Phrase = new Phrase($"{total.ToString("N2", new CultureInfo("en-EN"))}", table_font);
                tableContent.AddCell(cellRightMerge);

            }
            else
            {
                cellRight.Colspan = 3;
                cellRight.Phrase = new Phrase("Total Jumlah", bold_font);
                tableContent.AddCell(cellRight);

                cellLeft.Phrase = new Phrase($"{qtyTotal.ToString("N2", new CultureInfo("id-ID"))}", table_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase("Jumlah", bold_font);
                cellLeft.Colspan = 2;
                tableContent.AddCell(cellLeft);

                cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.Code}", table_font);
                tableContent.AddCell(cellLeftMerge);


                var isNotePpnShow = viewModel.IsUseVat && !viewModel.IsPayVAT;
                var notePpnShow = isNotePpnShow ? " Ditangguhkan" : "";
                //var additionalAmount = viewModel.IsUseVat ? total * 0.1 : 0;
                //total = total + additionalAmount;
                cellRightMerge.Phrase = new Phrase($"{total.ToString("N2", new CultureInfo("id-ID"))}", table_font);
                tableContent.AddCell(cellRightMerge);

                double ppn = 0;
                if (viewModel.IsUseVat)
                {
                    ppn = ((double)viewModel.Vat.Rate/100)* total;
                }

                cellRight.Colspan = 4;
                cellRight.Phrase = new Phrase("PPN "+viewModel.Vat.Rate+"%"+notePpnShow, bold_font);
                tableContent.AddCell(cellRight);

                cellLeft.Phrase = new Phrase("", normal_font);
                cellLeft.Colspan = 2;
                tableContent.AddCell(cellLeft);

                cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.Code}", table_font);
                tableContent.AddCell(cellLeftMerge);

                cellRightMerge.Phrase = new Phrase($"{ppn.ToString("N2", new CultureInfo("id-ID"))}", table_font);
                tableContent.AddCell(cellRightMerge);

                double pph = 0;
                if (viewModel.IsIncomeTax && viewModel.IncomeTax != null && viewModel.IncomeTax.Id != 0)
                {
                    cellRight.Colspan = 4;
                    cellRight.Phrase = new Phrase($"PPH {viewModel.IncomeTax.Name} {viewModel.IncomeTax.Rate}%", bold_font);
                    tableContent.AddCell(cellRight);

                    cellLeft.Phrase = new Phrase("", normal_font);
                    cellLeft.Colspan = 2;
                    tableContent.AddCell(cellLeft);

                    cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.Code}", table_font);
                    tableContent.AddCell(cellLeftMerge);

                    pph = total * viewModel.IncomeTax.Rate / 100;
                    cellRightMerge.Phrase = new Phrase($"{pph.ToString("N2", new CultureInfo("id-ID"))}", table_font);
                    tableContent.AddCell(cellRightMerge);
                }
                double usePpnAmount = viewModel.IsPayVAT ? ppn : 0;
                double usePphAmount = viewModel.IsPayIncomeTax ? pph : 0;
                double grandTotal = usePpnAmount + total - usePphAmount;

                cellRight.Colspan = 4;
                cellRight.Phrase = new Phrase("Grand Total", bold_font);
                tableContent.AddCell(cellRight);

                cellLeft.Phrase = new Phrase("", normal_font);
                cellLeft.Colspan = 2;
                tableContent.AddCell(cellLeft);

                cellLeftMerge.Phrase = new Phrase($"{viewModel.Currency.Code}", table_font);
                tableContent.AddCell(cellLeftMerge);

                cellRightMerge.Phrase = new Phrase($"{grandTotal.ToString("N2", new CultureInfo("id-ID"))}", table_font);
                tableContent.AddCell(cellRightMerge);

            }
            
            PdfPCell cellContent = new PdfPCell(tableContent); // dont remove
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 20f;
            document.Add(tableContent);
            #endregion

            #region Footer

            PdfPTable tableFooter = new PdfPTable(6);
            tableFooter.SetWidths(new float[] { 3.3f,0.5f, 5.5f, 3f, 0.5f, 4.8f });

            //PdfPCell cellFooterContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            //PdfPCell cellFooterContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
            PdfPCell cellFooterContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

            if (viewModel.Supplier.Import)
            {
                cellFooterContent.Phrase = new Phrase("Delivery cost", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(": ", normal_font);
                tableFooter.AddCell(cellFooterContent);

                string freightCost = "Buyer";

                if (viewModel.FreightCostBy.ToLower() == "penjual")
                {
                    freightCost = "Seller";
                }

                cellFooterContent.Phrase = new Phrase(freightCost, normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase("Delivery date", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(": ", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase((viewModel.DeliveryDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))), normal_font);
                tableFooter.AddCell(cellFooterContent);

                cellFooterContent.Phrase = new Phrase("Term payment", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(": ", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(viewModel.PaymentType, normal_font);
                tableFooter.AddCell(cellFooterContent);

                cellFooterContent.Phrase = new Phrase("Other", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(": ", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(viewModel.Remark, normal_font);
                tableFooter.AddCell(cellFooterContent);
            }
            else
            {
                cellFooterContent.Phrase = new Phrase("Ongkos Kirim", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(": ", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(viewModel.FreightCostBy, normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase("Delivery", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(": ", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase((viewModel.DeliveryDate.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"))), normal_font);
                tableFooter.AddCell(cellFooterContent);

                cellFooterContent.Phrase = new Phrase("Pembayaran", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(": ", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(viewModel.PaymentType + ", " + viewModel.PaymentDueDays + " hari setelah terima barang", normal_font);
                tableFooter.AddCell(cellFooterContent);

                cellFooterContent.Phrase = new Phrase("Lain-lain", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(": ", normal_font);
                tableFooter.AddCell(cellFooterContent);
                cellFooterContent.Phrase = new Phrase(viewModel.Remark, normal_font);
                tableFooter.AddCell(cellFooterContent);

            }

                

            PdfPCell cellFooter = new PdfPCell(tableFooter); // dont remove
            tableFooter.ExtendLastRow = false;
            tableFooter.SpacingAfter = 20f;
            document.Add(tableFooter);

            #endregion

            #region standarQuality
            if (viewModel.Category.ToLower().Contains("fabric"))
            {
                PdfPTable tableSQ = new PdfPTable(7);
                tableSQ.SetWidths(new float[] { 6f,1f,4f,1f,4f,1f,5f });

                //PdfPCell cellFooterContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                //PdfPCell cellFooterContentRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                PdfPCell cellSQContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

                if (viewModel.Supplier.Import)
                {
                    cellSQContent.Phrase = new Phrase("Quality Standard", normal_font);
                    tableSQ.AddCell(cellSQContent);
                }
                else
                {
                    cellSQContent.Phrase = new Phrase("Standar Kualitas", normal_font);
                    tableSQ.AddCell(cellSQContent);
                }

                    
                cellSQContent.Phrase = new Phrase(": ", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(viewModel.QualityStandardType, normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);


                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("Shrinkage test", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(viewModel.Shrinkage, normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);

                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("Rubbing test", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("Wet Rubbing", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(viewModel.WetRubbing, normal_font);
                tableSQ.AddCell(cellSQContent);

                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("Dry Rubbing", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(viewModel.DryRubbing, normal_font);
                tableSQ.AddCell(cellSQContent);

                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("Washing test", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(viewModel.Washing, normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);

                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("Perspiration test", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("Dark", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(viewModel.DarkPerspiration, normal_font);
                tableSQ.AddCell(cellSQContent);

                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("Light/Med", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(viewModel.LightMedPerspiration, normal_font);
                tableSQ.AddCell(cellSQContent);

                cellSQContent.Phrase = new Phrase("Piece Length", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(":", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase(viewModel.PieceLength, normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);
                cellSQContent.Phrase = new Phrase("", normal_font);
                tableSQ.AddCell(cellSQContent);

                PdfPCell cellSQ = new PdfPCell(tableSQ); // dont remove
                tableSQ.ExtendLastRow = false;
                tableSQ.SpacingAfter = 20f;
                document.Add(tableSQ);
            }
            #endregion

            #region TableSignature

            PdfPTable tableSignature = new PdfPTable(2);

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            if (viewModel.Supplier.Import)
            {
                cellSignatureContent.Phrase = new Phrase("Buyer\n\n\n\n\n\n\n(  " + viewModel.CreatedBy + "  )", bold_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("Seller\n\n\n\n\n\n\n(  " + viewModel.Supplier.Name + "  )", bold_font);
                tableSignature.AddCell(cellSignatureContent);
            }
            else
            {
                cellSignatureContent.Phrase = new Phrase("Pembeli\n\n\n\n\n\n\n(  " + viewModel.CreatedBy + "  )", bold_font);
                tableSignature.AddCell(cellSignatureContent);
                cellSignatureContent.Phrase = new Phrase("Penjual\n\n\n\n\n\n\n(  " + viewModel.Supplier.Name + "  )", bold_font);
                tableSignature.AddCell(cellSignatureContent);
            }

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
