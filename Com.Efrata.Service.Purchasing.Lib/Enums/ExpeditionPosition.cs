using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Enums
{
    public enum ExpeditionPosition
    {
        INVALID = 0,
        PURCHASING_DIVISION = 1,
        SEND_TO_VERIFICATION_DIVISION = 2,
        VERIFICATION_DIVISION = 3,
        SEND_TO_CASHIER_DIVISION = 4,
        SEND_TO_ACCOUNTING_DIVISION = 5,
        SEND_TO_PURCHASING_DIVISION = 6,
        CASHIER_DIVISION = 7,
        FINANCE_DIVISION = 8,
        //ACCOUNTING_DIVISION = 8,
    }

    public enum PurchasingGarmentExpeditionPosition
    {
        Invalid = 0,
        [Description("Pembelian")]
        Purchasing = 1,
        [Description("Kirim ke Verifikasi")]
        SendToVerification = 2,
        [Description("Verifikasi (Diterima)")]
        VerificationAccepted = 3,
        [Description("Kirim ke Kasir")]
        SendToCashier = 4,
        [Description("Kasir (Diterima)")]
        CashierAccepted = 5,
        [Description("Kirim ke Pembelian (Not Verified)")]
        SendToPurchasing = 6,
        [Description("Kirim ke Accounting")]
        SendToAccounting = 7,
        [Description("Accounting (Diterima)")]
        AccountingAccepted = 8
    }
}
