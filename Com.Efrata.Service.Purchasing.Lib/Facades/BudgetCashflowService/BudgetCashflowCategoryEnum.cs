using System.ComponentModel;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.Facades.BudgetCashflowService
{
    public enum BudgetCashflowCategoryLayoutOrder
    {
        [Description("Penjualan Export")]
        ExportSales = 1,
        [Description("Penjualan Lokal")]
        LocalSales,
        [Description("Penjualan Tunai")]
        CashSales,
        [Description("Penjualan Intern (Antar Divisi)")]
        InteralDivisionSales,
        [Description("Penjualan Intern (Antar Antar Unit Satu Divisi)")]
        InternalUnitSales,
        [Description("PPN Masukan Intern (Perhitungan)")]
        InternalIncomeVATCalculation,
        [Description("Penjualan Lain-Lain")]
        OthersSales,
        [Description("PPN Masukan Extern (Pembelian Lokal)")]
        ExternalIncomeVATCalculation,
        [Description("Bahan Baku Import")]
        ImportedRawMaterial,
        [Description("Bahan Baku Lokal")]
        LocalRawMaterial,
        [Description("Biaya Tenaga Kerja Langsung/U. Karyawan")]
        DirectLaborCostEmployeeFee,
        [Description("Bahan Pembantu")]
        AuxiliaryMaterial,
        [Description("SubCount")]
        SubCount,
        [Description("Embalage")]
        Embalage,
        [Description("Listrik (PLN)")]
        Electricity,
        [Description("Batu Bara")]
        Coal,
        [Description("BBM & Pelumas")]
        FuelOil,
        [Description("Spare Part & Pemeliharaan Mesin")]
        SparePartsMachineMaintenance,
        [Description("BTKL Staf Unit")]
        DirectLaborCost,
        [Description("THR/Bonus Karyawan & Staf")]
        HolidayAllowanceLaborCost,
        [Description("Gaji/Honor Konsultan & TKA")]
        ConsultantCost,
        [Description("Askes & Jamsostek Unit")]
        HealthInsuranceSocialSecurity,
        [Description("Pesangon/Pensiun Unit")]
        SeveranceCost,
        [Description("Pengolahan Limbah, ABT, dll")]
        UtilityCost,
        [Description("Biaya Import (Inklaring, Demurage, dll)")]
        ImportCost,
        [Description("Pembelian Intern (Antar Divisi)")]
        InternalDivisionPurchase,
        [Description("Pembelian Intern (Antar Unit Satu Divisi)")]
        InternalUnitPurchase,
        [Description("PPN Keluaran Intern (Perhitungan)")]
        InternalOutcomeVATCalculation,
        [Description("Beban Gaji Staff")]
        MarketingSalaryCost,
        [Description("Upah Karyawan")]
        MarketingSalaryExpense,
        [Description("JPK & Jamsostek Staff & Karyawan")]
        MarketingHealthInsuranceSocialSecurity,
        [Description("THR & Bonus Karyawan & Staf")]
        MarketingHolidayAllowance,
        [Description("Beban Iklan, Reklame, & Pameran")]
        AdvertisementCost,
        [Description("Beban Perjalanan Dinas")]
        BusinessTripCost,
        [Description("Beban Pengiriman/Ongkos Angkut")]
        ShippingCost,
        [Description("Beban Komisi Penjualan Lokal/Ekspor")]
        SalesComission,
        [Description("Beban Freight/EMKL")]
        FreightCost,
        [Description("Biaya Klaim")]
        ClaimCost,
        [Description("Biaya Pengurusan Dokumentasi")]
        DocumentationCost,
        [Description("Beban Asuransi")]
        InsuranceCost,
        [Description("Beban Penjualan Lain-Lain")]
        OtherSalesCost,
        [Description("PPN Keluaran Extern")]
        GeneralAdministrativeExternalOutcomeVATCalculation,
        [Description("Pajak (PPN, PPh, PBB, PNBP, dll)")]
        TaxCost,
        [Description("Beban Gaji Staff Kantor")]
        GeneralAdministrativeSalaryCost,
        [Description("Upah Karyawan Kantor")]
        GeneralAdministrativeSalaryExpense,
        [Description("Askes & Jamsostek Karyawan & Staf")]
        GeneralAdministrativeSocialSecurity,
        [Description("Beban Gaji Direksi/Direktur")]
        GeneralAdministrativeDirectorsSalary,
        [Description("Beban Pemeliharaan Gedung")]
        GeneralAdministrativeBuildingMaintenance,
        [Description("Beban Perjalanan Dinas")]
        GeneralAdministrativeBusinessTrip,
        [Description("Beban Pengiriman Surat")]
        GeneralAdministrativeMailingCost,
        [Description("Beban Alat Tulis")]
        GeneralAdministrativeStationary,
        [Description("Beban Air/ABT")]
        GeneralAdministrativeWaterCost,
        [Description("Beban Listrik")]
        GeneralAdministrativeElectricityCost,
        [Description("Beban Notaris & Konsultan")]
        GeneralAdministrativeConsultant,
        [Description("Beban Training & Pendidikan")]
        GeneralAdministrativeTraining,
        [Description("Beban Perizinan & Sertifikat")]
        GeneralAdministrativeCertification,
        [Description("Sumbangan")]
        GeneralAdministrativeDonation,
        [Description("Representative, Entertainment Tamu, dll")]
        GeneralAdministrativeGuestEntertainment,
        [Description("Ass Kendaraan, Gedung, dan Mesin")]
        GeneralAdministrativeVehicleBuildingMachineInsurance,
        [Description("Beban URTP")]
        GeneralAdministrativeCorporateHousehold,
        [Description("Pesangon Staf & Karyawan")]
        GeneralAdministrativeSeveranceCost,
        [Description("THR & Bonus Karyawan & Staf Umum, Direktur & Direksi")]
        GeneralAdministrativeHolidayAllowance,
        [Description("Beban Kendaraan")]
        GeneralAdministrativeVehicleCost,
        [Description("Beban Keamanan")]
        GeneralAdministrativeSecurityCost,
        [Description("Beban Lain-Lain")]
        GeneralAdministrativeOthersCost,
        [Description("Telephone, Fax, & Internet")]
        GeneralAdministrativeCommunicationCost,
        [Description("Biaya Lainnya")]
        OthersOperationalCost,
        [Description("Deposito")]
        CashInDeposit,
        [Description("Lain Lain")]
        CashInOthers,
        [Description("Mesin")]
        MachineryPurchase,
        [Description("Kendaraan")]
        VehiclePurchase,
        [Description("Inventaris")]
        InventoryPurchase,
        [Description("Alat Komputer")]
        ComputerToolsPurchase,
        //[Description("Alat & Bahan Produksi")]
        //ProductionToolsMaterialsPurchase,
        [Description("Proyek")]
        ProjectPurchase,
        [Description("Deposito")]
        CashOutDeposit,
        [Description("Pencairan pinjaman (Loan Withdrawal)")]
        CashInLoanWithdrawal,
        [Description("Afiliasi")]
        CashInAffiliates,
        [Description("Jual Beli Valas")]
        CashInForexTrading,
        [Description("Cadangan Perusahaan")]
        CashInCompanyReserves,
        [Description("Lain-Lain (Klaim Ass)/ Tab THR/VB Import/Giro/DLL")]
        CashInLoanWithdrawalOthers,
        [Description("Angsuran Kredit")]
        CashOutInstallments,
        [Description("Biaya Bunga Bank")]
        CashOutBankInterest,
        [Description("Biaya Adm Bank")]
        CashOutBankAdministrationFee,
        [Description("Afiliasi (Psr, Group)")]
        CashOutAffiliates,
        [Description("Jual Beli Valas")]
        CashOutForexTrading,
        [Description("Lain-Lain/Efrata/B Mandiri/MD/Cad THR")]
        CashOutOthers
    }

    // Display Friendly Name for enum
    // source : https://www.codingame.com/playgrounds/2487/c---how-to-display-friendly-names-for-enumerations
    public static class BudgetCashflowCategoryLayoutOrderEnumExtensions
    {
        public static string ToDescriptionString(this BudgetCashflowCategoryLayoutOrder me)
        {
            var enumType = me.GetType();
            var memberInfo = enumType.GetMember(me.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var _attr = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (_attr != null && _attr.Count() > 0)
                {
                    return ((DescriptionAttribute)_attr.ElementAt(0)).Description;
                }
            }
            return me.ToString();
        }
    }
}
