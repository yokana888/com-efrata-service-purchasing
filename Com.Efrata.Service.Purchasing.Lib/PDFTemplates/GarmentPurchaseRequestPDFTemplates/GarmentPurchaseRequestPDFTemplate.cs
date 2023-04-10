using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPurchaseRequestViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.PDFTemplates.GarmentPurchaseRequestPDFTemplates
{
    public class GarmentPurchaseRequestPDFTemplate
    {
        public static MemoryStream Generate(IServiceProvider serviceProvider, GarmentPurchaseRequestViewModel viewModel)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 6);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 6);

            Document document = new Document(PageSize.A4, 10, 10, 10, 10);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            IdentityService identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
            IGarmentPurchaseRequestFacade garmentPurchaseRequestFacade = (IGarmentPurchaseRequestFacade)serviceProvider.GetService(typeof(IGarmentPurchaseRequestFacade));
            var salesContract = garmentPurchaseRequestFacade.GetGarmentPreSalesContract(viewModel.SCId);

            #region Header

            Paragraph title = new Paragraph("PT EFRATA GARMINDO UTAMA", normal_font) { Alignment = Element.ALIGN_LEFT };
            document.Add(title);

            Paragraph companyName = new Paragraph("BUDGET MASTER GARMENT", header_font) { Alignment = Element.ALIGN_LEFT };
            document.Add(companyName);

            #endregion

            PdfPCell cellLeftNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            #region Identity

            PdfPTable tableIdentity = new PdfPTable(6);
            tableIdentity.SetWidths(new float[] { 1f, 2f, 1f, 3f, 1f, 2f });

            cellLeftNoBorder.Phrase = new Phrase("RO", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(": " + viewModel.RONo, normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("NO PRE S/C", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(": " + viewModel.SCNo, normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase("LEAD TIME : 35", normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);
            cellLeftNoBorder.Phrase = new Phrase(string.Empty, normal_font);
            tableIdentity.AddCell(cellLeftNoBorder);

            PdfPCell cellIdentity = new PdfPCell(tableIdentity);
            tableIdentity.ExtendLastRow = false;
            tableIdentity.SpacingBefore = 5f;
            tableIdentity.SpacingAfter = 10f;
            document.Add(tableIdentity);

            #endregion

            #region TableContent

            PdfPTable tableContent = new PdfPTable(9);
            tableContent.SetWidths(new float[] { 36f, 116f, 100f, 175f, 90f, 68f, 153f, 132f, 130f });

            cellCenter.Phrase = new Phrase("NO", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("CATEGORIES", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("KODE PRODUK", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("DESCRIPTION", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("QUANTITY", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("UNIT", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("PRICE", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("AMOUNT", bold_font);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("PO NUMBER", bold_font);
            tableContent.AddCell(cellCenter);

            double totalAmount = 0;
            int indexItem = 0;
            foreach (var item in viewModel.Items)
            {
                cellCenter.Phrase = new Phrase((++indexItem).ToString(), normal_font);
                tableContent.AddCell(cellCenter);

                cellLeft.Phrase = new Phrase(item.Category.Name, normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(item.Product.Code, normal_font);
                tableContent.AddCell(cellLeft);

                cellLeft.Phrase = new Phrase(item.ProductRemark, normal_font);
                tableContent.AddCell(cellLeft);

                cellRight.Phrase = new Phrase(Math.Round(item.Quantity).ToString(), normal_font);
                tableContent.AddCell(cellRight);

                cellCenter.Phrase = new Phrase(item.Uom.Unit, normal_font);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(string.Concat(item.BudgetPrice.ToString("n", new CultureInfo("id-ID")), "/", item.PriceUom.Unit), normal_font);
                tableContent.AddCell(cellCenter);

                var amount = item.Quantity * item.BudgetPrice / item.PriceConversion;
                cellRight.Phrase = new Phrase(amount.ToString("n", new CultureInfo("id-ID")), normal_font);
                tableContent.AddCell(cellRight);

                cellCenter.Phrase = new Phrase(item.PO_SerialNumber, normal_font);
                tableContent.AddCell(cellCenter);

                totalAmount += amount;
            }


            PdfPCell cellContent = new PdfPCell(tableContent);
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 20f;
            document.Add(tableContent);

            #endregion

            #region TableFooter

            PdfPTable tableFooter = new PdfPTable(5);
            tableFooter.SetWidths(new float[] { 121f, 302f, 57f, 304f, 217f });

            cellLeft.PaddingTop = 7f;
            cellLeft.PaddingBottom = 7f;

            cellLeft.Phrase = new Phrase("BUYER AGENT", normal_font);
            tableFooter.AddCell(cellLeft);

            cellLeft.Phrase = new Phrase(salesContract.BuyerAgentCode + " - " + salesContract.BuyerAgentName, normal_font);
            tableFooter.AddCell(cellLeft);

            tableFooter.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER, Rowspan = 5 });

            tableFooter.AddCell(new PdfPCell() {
                Padding = 7f,
                Rowspan = 3,
                Border = cellLeft.Border,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                Phrase = new Phrase(string.Concat("TOTAL BUDGET : ", totalAmount.ToString("n", new CultureInfo("id-ID"))), normal_font)
            });

            tableFooter.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER, Rowspan = 5 });

            cellLeft.Phrase = new Phrase("BUYER BRAND", normal_font);
            tableFooter.AddCell(cellLeft);

            cellLeft.Phrase = new Phrase(viewModel.Buyer.Code + " - " + viewModel.Buyer.Name, normal_font);
            tableFooter.AddCell(cellLeft);

            cellLeft.Phrase = new Phrase("ARTICLE", normal_font);
            tableFooter.AddCell(cellLeft);

            cellLeft.Phrase = new Phrase(viewModel.Article, normal_font);
            tableFooter.AddCell(cellLeft);

            cellLeft.Phrase = new Phrase("DESCRIPTION", normal_font);
            tableFooter.AddCell(cellLeft);

            cellLeft.Phrase = new Phrase(viewModel.Remark, normal_font);
            tableFooter.AddCell(cellLeft);

            tableFooter.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER, Rowspan = 2 });

            cellLeft.Phrase = new Phrase("SHIPMENT", normal_font);
            tableFooter.AddCell(cellLeft);

            cellLeft.Phrase = new Phrase(viewModel.ShipmentDate.GetValueOrDefault().ToOffset(new TimeSpan(identityService.TimezoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), normal_font);
            tableFooter.AddCell(cellLeft);

            PdfPCell cellfooter = new PdfPCell(tableFooter);
            tableFooter.ExtendLastRow = false;
            document.Add(tableFooter);

            #endregion

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}
