using Com.Efrata.Service.Purchasing.Lib.Models.Expedition;
using Com.Efrata.Service.Purchasing.Lib.Utilities;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.Expedition;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates
{
    public class PPHBankExpenditureNotePDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(PPHBankExpenditureNote model, int clientTimeZoneOffset)
        {
            const int MARGIN = 20;

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            Document document = new Document(PageSize.A4, MARGIN, MARGIN, MARGIN, MARGIN);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            #region Header

            PdfPTable headerTable = new PdfPTable(2);
            headerTable.SetWidths(new float[] { 10f, 10f });
            headerTable.WidthPercentage = 100;
            PdfPTable headerTable1 = new PdfPTable(1);
            PdfPTable headerTable2 = new PdfPTable(2);
            headerTable2.SetWidths(new float[] { 15f, 40f });
            headerTable2.WidthPercentage = 100;

            PdfPCell cellHeader1 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeader2 = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderBody = new PdfPCell() { Border = Rectangle.NO_BORDER };

            PdfPCell cellHeaderCS2 = new PdfPCell() { Border = Rectangle.NO_BORDER, Colspan = 2 };


            cellHeaderCS2.Phrase = new Phrase("BUKTI PENGELUARAN BANK PPH", bold_font);
            cellHeaderCS2.HorizontalAlignment = Element.ALIGN_CENTER;
            headerTable.AddCell(cellHeaderCS2);

            cellHeaderBody.Phrase = new Phrase("PT. Efrata GARMINDO", normal_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("Banaran, Grogol, Sukoharjo, Jawa Tengah", normal_font);
            headerTable1.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase("57552" + "Telp (0271) 732888, 7652913", normal_font);
            headerTable1.AddCell(cellHeaderBody);

            cellHeader1.AddElement(headerTable1);
            headerTable.AddCell(cellHeader1);

            cellHeaderCS2.Phrase = new Phrase("", bold_font);
            headerTable2.AddCell(cellHeaderCS2);

            cellHeaderBody.Phrase = new Phrase("Tanggal", normal_font);
            headerTable2.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(": " + DateTimeOffset.UtcNow.AddHours(clientTimeZoneOffset).ToString("dd MMMM yyyy"), normal_font);
            headerTable2.AddCell(cellHeaderBody);

            cellHeaderBody.Phrase = new Phrase("NO", normal_font);
            headerTable2.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(": " + model.No, normal_font);
            headerTable2.AddCell(cellHeaderBody);

            var purchasingDocumentExpedition = model.Items
                .GroupBy(p => new { IncomeTaxName = p.PurchasingDocumentExpedition.IncomeTaxName, IncomeTaxRate = p.PurchasingDocumentExpedition.IncomeTaxRate, 
                                    IncomeTaxId = p.PurchasingDocumentExpedition.IncomeTaxId, PPHBankExpenditureNoteId = p.PPHBankExpenditureNoteId })
                .OrderBy(p => p.Key.IncomeTaxId)
                .Select(g => new { IncomeTaxName = g.Key.IncomeTaxName, IncomeTaxRate = g.Key.IncomeTaxRate, IncomeTaxId = g.Key.IncomeTaxId, PPHBankExpenditureNoteId = g.Key.PPHBankExpenditureNoteId }).ToList();

            cellHeaderBody.Phrase = new Phrase("Pasal PPH", normal_font);
            headerTable2.AddCell(cellHeaderBody);

            var count = 0;
            foreach (var item in purchasingDocumentExpedition)
            {
                if (count == 0)
                {
                    cellHeaderBody.Phrase = new Phrase(": - " + item.IncomeTaxName + " - " + item.IncomeTaxRate, normal_font);
                    headerTable2.AddCell(cellHeaderBody);
                }
                else
                {
                    cellHeaderBody.Phrase = new Phrase("", normal_font);
                    headerTable2.AddCell(cellHeaderBody);
                    cellHeaderBody.Phrase = new Phrase("  - " + item.IncomeTaxName + " - " + item.IncomeTaxRate, normal_font);
                    headerTable2.AddCell(cellHeaderBody);
                }
                count++;
            }

            cellHeaderBody.Phrase = new Phrase("Bank", normal_font);
            headerTable2.AddCell(cellHeaderBody);
            cellHeaderBody.Phrase = new Phrase(": " + model.BankAccountName + " - A/C : " + model.BankAccountNumber, normal_font);
            headerTable2.AddCell(cellHeaderBody);

            cellHeader2.AddElement(headerTable2);
            headerTable.AddCell(cellHeader2);

            document.Add(headerTable);
            document.Add(new Paragraph("\n"));

            #endregion Header

            #region Body

            PdfPTable bodyTable = new PdfPTable(7);
            PdfPCell bodyCell = new PdfPCell();

            float[] widthsBody = new float[] { 5f, 12f, 10f, 5f, 5f, 15f, 15f };
            bodyTable.SetWidths(widthsBody);
            bodyTable.WidthPercentage = 100;

            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
            bodyCell.Colspan = 1;
            bodyCell.Rowspan = 2;
            bodyCell.Phrase = new Phrase("No.", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 4;
            bodyCell.Rowspan = 1;
            bodyCell.Phrase = new Phrase("Uraian", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 1;
            bodyCell.Rowspan = 2;
            bodyCell.Phrase = new Phrase("PPH", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Phrase = new Phrase("DPP", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 1;
            bodyCell.Rowspan = 1;
            bodyCell.Phrase = new Phrase("No. SPB", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Phrase = new Phrase("Supplier", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Phrase = new Phrase("Unit", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Phrase = new Phrase("Mata Uang", normal_font);
            bodyTable.AddCell(bodyCell);

            int index = 1;
            double totalDPP = 0, totalPPH = 0;

            Dictionary<string, double> units = new Dictionary<string, double>();

            model.Items = model.Items.OrderBy(p => p.PurchasingDocumentExpedition.SupplierName).ToList();

            foreach (PPHBankExpenditureNoteItem item in model.Items)
            {
                var pdeItems = item.PurchasingDocumentExpedition.Items
                    .GroupBy(m => new { m.UnitCode, m.UnitName })
                    .Select(s => new
                    {
                        UnitCode = s.First().UnitCode,
                        UnitName = s.First().UnitName,
                        TotalDPP = s.Sum(d => d.Price),
                        TotalPPH = item.PurchasingDocumentExpedition.IncomeTax//(s.Sum(d => d.Price) * ((model.IncomeTaxRate == 0 ? 1 : model.IncomeTaxRate) / 100))
                    });

                foreach (var pdeItem in pdeItems)
                {
                    bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    bodyCell.VerticalAlignment = Element.ALIGN_TOP;
                    bodyCell.Phrase = new Phrase((index++).ToString(), normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    bodyCell.Phrase = new Phrase(item.PurchasingDocumentExpedition.UnitPaymentOrderNo, normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.Phrase = new Phrase(item.PurchasingDocumentExpedition.SupplierName, normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    bodyCell.Phrase = new Phrase(pdeItem.UnitCode, normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.Phrase = new Phrase(item.PurchasingDocumentExpedition.Currency, normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bodyCell.Phrase = new Phrase(string.Format("{0:n4}", pdeItem.TotalPPH), normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.Phrase = new Phrase(string.Format("{0:n4}", pdeItem.TotalDPP), normal_font);
                    bodyTable.AddCell(bodyCell);

                    if (units.ContainsKey(pdeItem.UnitCode))
                    {
                        units[pdeItem.UnitCode] += pdeItem.TotalPPH;
                    }
                    else
                    {
                        units.Add(pdeItem.UnitCode, pdeItem.TotalPPH);
                    }

                    totalPPH += pdeItem.TotalPPH;
                    totalDPP += pdeItem.TotalDPP;
                }
            }

            bodyCell.Colspan = 3;
            bodyCell.Border = Rectangle.NO_BORDER;
            bodyCell.Phrase = new Phrase("", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 1;
            bodyCell.Border = Rectangle.RECTANGLE;
            bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyCell.Phrase = new Phrase("Total", bold_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 1;
            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
            bodyCell.Phrase = new Phrase(model.Currency, bold_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            bodyCell.Phrase = new Phrase(string.Format("{0:n4}", totalPPH), bold_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            bodyCell.Phrase = new Phrase(string.Format("{0:n4}", totalDPP), bold_font);
            bodyTable.AddCell(bodyCell);

            document.Add(bodyTable);
            document.Add(new Paragraph("\n"));

            #endregion Body

            #region BodyFooter
            PdfPTable bodyFooterTable = new PdfPTable(6);
            bodyFooterTable.SetWidths(new float[] { 3f, 8f, 2f, 6f, 10f, 10f });
            bodyFooterTable.WidthPercentage = 100;

            PdfPCell bodyFooterCell = new PdfPCell() { Border = Rectangle.NO_BORDER };

            bodyFooterCell.Colspan = 1;
            bodyFooterCell.Phrase = new Phrase("");
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.Colspan = 5;
            bodyFooterCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyFooterCell.Phrase = new Phrase("Rincian PPH per bagian:", normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);
            
            bodyFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            foreach (var unit in units)
            {
                bodyFooterCell.Colspan = 1;
                bodyFooterCell.Phrase = new Phrase("");
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Phrase = new Phrase(unit.Key, normal_font);
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Phrase = new Phrase(model.Currency, normal_font);
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Phrase = new Phrase(string.Format("{0:n4}", unit.Value), normal_font);
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Colspan = 2;
                bodyFooterCell.Phrase = new Phrase("");
                bodyFooterTable.AddCell(bodyFooterCell);
            }

            bodyFooterCell.Colspan = 1;
            bodyFooterCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyFooterCell.Phrase = new Phrase("");
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.Phrase = new Phrase("Terbilang", normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            bodyFooterCell.Phrase = new Phrase(": " + model.Currency, normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.Colspan = 3;
            bodyFooterCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyFooterCell.Phrase = new Phrase(NumberToTextIDN.terbilang(totalPPH), normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);


            document.Add(bodyFooterTable);
            document.Add(new Paragraph("\n"));
            #endregion BodyFooter

            #region Footer
            PdfPTable footerTable = new PdfPTable(2);
            PdfPCell cellFooter = new PdfPCell() { Border = Rectangle.NO_BORDER };

            float[] widthsFooter = new float[] { 10f, 5f };
            footerTable.SetWidths(widthsFooter);
            footerTable.WidthPercentage = 100;

            cellFooter.Phrase = new Phrase("Dikeluarkan dengan cek/BG No. : " + model.BGNo, normal_font);
            footerTable.AddCell(cellFooter);

            cellFooter.Phrase = new Phrase("", normal_font);
            footerTable.AddCell(cellFooter);

            PdfPTable signatureTable = new PdfPTable(3);
            PdfPCell signatureCell = new PdfPCell() { HorizontalAlignment = Element.ALIGN_CENTER };
            signatureCell.Phrase = new Phrase("Bag. Keuangan", normal_font);
            signatureTable.AddCell(signatureCell);
            
            signatureCell.Colspan = 2;
            signatureCell.HorizontalAlignment = Element.ALIGN_CENTER;
            signatureCell.Phrase = new Phrase("Direksi", normal_font);
            signatureTable.AddCell(signatureCell);

            signatureTable.AddCell(new PdfPCell()
            {
                Phrase = new Phrase("---------------------------", normal_font),
                FixedHeight = 40,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            signatureTable.AddCell(new PdfPCell()
            {
                Phrase = new Phrase("---------------------------", normal_font),
                FixedHeight = 40,
                Border = Rectangle.NO_BORDER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            signatureTable.AddCell(new PdfPCell()
            {
                Phrase = new Phrase("---------------------------", normal_font),
                FixedHeight = 40,
                Border = Rectangle.NO_BORDER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            footerTable.AddCell(new PdfPCell(signatureTable));

            cellFooter.Phrase = new Phrase("", normal_font);
            footerTable.AddCell(cellFooter);
            document.Add(footerTable);
            #endregion Footer

            document.Close();

            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}
