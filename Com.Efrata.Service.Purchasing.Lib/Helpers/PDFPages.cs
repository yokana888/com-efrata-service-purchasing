using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Com.Efrata.Service.Purchasing.Lib.Helpers
{
    public class PDFPages : iTextSharp.text.pdf.PdfPageEventHelper
    {

        public override void OnEndPage(PdfWriter writer, Document doc)
        {

            BaseColor grey = new BaseColor(128, 128, 128);
            Font font = FontFactory.GetFont("Arial", 9, Font.NORMAL, grey);
            //tbl footer
            PdfPTable footerTbl = new PdfPTable(1);
            footerTbl.TotalWidth = doc.PageSize.Width;
            //img footer
            //iTextSharp.text.Image foot = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("footer.jpg"));
            //foot.ScalePercent(45);

            //footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
            //PdfPCell cell = new PdfPCell(foot);
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.Border = 0;
            //footerTbl.AddCell(cell);


            //page number
            Chunk myFooter = new Chunk("Page " + (doc.PageNumber), FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, grey));
            PdfPCell footer = new PdfPCell(new Phrase(myFooter));
            footer.Border = Rectangle.NO_BORDER;
            footer.HorizontalAlignment = Element.ALIGN_CENTER;
            footerTbl.AddCell(footer);

            //this is for the position of the footer
            footerTbl.WriteSelectedRows(0, -1, 0, (doc.BottomMargin - 10), writer.DirectContent);
        }

    }
}