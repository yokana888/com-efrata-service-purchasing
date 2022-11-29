using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Enums
{
    public enum PurchaseRequestStatus
    {
        VOID = 0, /* Dibatalkan */
        CREATED = 1, /* Purchase request dibuat */
        POSTED = 2, /* Belum diterima Pembelian */
        ORDERED = 3, /* Sudah diorder ke Supplier */
        ARRIVING = 4, /* Barang sudah datang */
        PROCESSING = 7, /* Sudah diterima Pembelian */
        PREMATURE = 9, /* Di close */
        COMPLETE = 9, /* Barang sudah datang */
    }
}
