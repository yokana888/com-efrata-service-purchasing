using System;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public class ExpenditureDto
    {
        public ExpenditureDto(int id, string documentNo, DateTimeOffset date)
        {
            Id = id;
            DocumentNo = documentNo;
            Date = date;
        }

        public int Id { get; set; }
        public string DocumentNo { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}