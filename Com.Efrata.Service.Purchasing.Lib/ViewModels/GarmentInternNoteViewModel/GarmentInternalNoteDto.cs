using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternNoteViewModel
{
    public class GarmentInternalNoteDto
    {
        public GarmentInternalNoteDto(int internalNoteId, string internalNoteNo, DateTimeOffset internalNoteDate, DateTimeOffset internalNoteDueDate, int supplierId, string supplierName, string supplierCode, int currencyId, string currencyCode, double currencyRate, List<InternalNoteInvoiceDto> internalNoteInvoices)
        {
            InternalNote = new InternalNoteDto(internalNoteId, internalNoteNo, internalNoteDate, internalNoteDueDate, supplierId, supplierName, supplierCode, currencyId, currencyCode, currencyRate, internalNoteInvoices);
        }

        public InternalNoteDto InternalNote { get; set; }

        
    }
}
