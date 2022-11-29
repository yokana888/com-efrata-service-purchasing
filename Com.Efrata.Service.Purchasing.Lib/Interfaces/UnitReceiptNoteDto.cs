using System;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public class UnitReceiptNoteDto
    {
        public UnitReceiptNoteDto(int id, string unitReceiptNoteNo, DateTimeOffset unitReceiptNoteDate)
        {
            Id = id;
            UnitReceiptNoteNo = unitReceiptNoteNo;
            UnitReceiptNoteDate = unitReceiptNoteDate;
        }

        public int Id { get; set; }
        public string UnitReceiptNoteNo { get; set; }
        public DateTimeOffset UnitReceiptNoteDate { get; set; }
    }
}