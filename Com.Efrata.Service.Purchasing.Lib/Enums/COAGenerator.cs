using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Enums
{
    public static class COAGenerator
    {
        public const string TEMP_COA_CODE = "99";

        public const string HUTANG_USAHA_LOKAL = "3010";
        public const string HUTANG_USAHA_IMPOR = "3020";

        public const string HUTANG_USAHA_OPERASIONAL = "01";
        public const string HUTANG_USAHA_INVESTASI = "02";

        public const string DIVISI_SPINNING = "1";
        public const string UNIT_SPINNING1 = "01";
        public const string UNIT_SPINNING2 = "02";
        public const string UNIT_SPINNING3 = "03";
        public const string UNIT_SPINNINGMS = "04";

        public const string DIVISI_WEAVING = "2";
        public const string UNIT_WEAVING1 = "01";
        public const string UNIT_WEAVING2 = "02";

        public const string DIVISI_FINISHING_PRINTING = "3";
        public const string UNIT_FINISHING = "01";
        public const string UNIT_PRINTING = "02";

        public const string DIVISI_GARMENT = "4";
        public const string UNIT_CENTRAL1A = "01";
        public const string UNIT_CENTRAL1B = "02";
        public const string UNIT_CENTRAL2A = "03";
        public const string UNIT_CENTRAL2B = "04";
        public const string UNIT_CENTRAL2C = "05";

        public const string PEMBELIAN_BAHAN_BAKU = "5901";
        public const string PEMBELIAN_BARANG_JADI = "5902";
        public const string PEMBELIAN_BAHAN_PEMBANTU = "5903";
        public const string PEMBELIAN_BAHAN_EMBALASE = "5904";
        public const string PEMBELIAN_BARANG_DAGANGAN = "5906";

        public const string PPH23_YMH = "3330";
        public const string PPH_FINAL = "3331";
        public const string PPH21_YMH = "3340";
        public const string PPH26_YMH = "3350";

        public const string PERSEDIAAN_BAHAN_BAKU = "1403";
        public const string PERSEDIAAN_BARANG_JADI = "1401";
        public const string PERSEDIAAN_CHEMICAL = "1405";
        public const string PERSEDIAAN_BAHAN_BAKAR_INDUSTRI = "1406";
        public const string PERSEDIAAN_PELUMAS = "1407";
        public const string PERSEDIAAN_SPARE_PART = "1408";
        public const string PERSEDIAAN_BAHAN_PEMBANTU = "1410";

        public const string PPN_KELUARAN = "3320";
        public const string PPN_MASUKAN = "1509";

        private const string DEFAULT_COA_IF_EMPTY = "9999";

        public static string GetDebtCOA(bool isImport, string division, string unitCode)
        {
            var result = "";

            if (isImport)
                result += HUTANG_USAHA_IMPOR + "." + HUTANG_USAHA_OPERASIONAL;
            else
                result += HUTANG_USAHA_LOKAL + "." + HUTANG_USAHA_OPERASIONAL;

            result += "." + GetDivisionAndUnitCOACode(division, unitCode);

            return result;
        }

        public static string GetDivisionAndUnitCOACode(string division, string unitCode)
        {
            var result = "";
            switch (division?.ToUpper().Replace(" ", ""))
            {
                case "SPINNING":
                    result = DIVISI_SPINNING;
                    switch (unitCode)
                    {
                        case "S1":
                            result += "." + UNIT_SPINNING1;
                            break;
                        case "S2":
                            result += "." + UNIT_SPINNING2;
                            break;
                        case "S3":
                            result += "." + UNIT_SPINNING3;
                            break;
                        case "S4":
                            result += "." + UNIT_SPINNINGMS;
                            break;
                        default:
                            result += ".00";
                            break;
                    }
                    break;
                case "WEAVING":
                    result = DIVISI_WEAVING;
                    switch (unitCode)
                    {
                        case "W2":
                            result += "." + UNIT_WEAVING2;
                            break;
                        case "W1":
                            result += "." + UNIT_WEAVING1;
                            break;
                        default:
                            result += ".00";
                            break;
                    }
                    break;
                case "DYEING&PRINTING":
                    result = DIVISI_FINISHING_PRINTING;
                    switch (unitCode)
                    {
                        case "F1":
                            result += "." + UNIT_FINISHING;
                            break;
                        case "F2":
                            result += "." + UNIT_PRINTING;
                            break;
                        default:
                            result += ".00";
                            break;
                    }
                    break;
                case "GARMENT":
                    result = DIVISI_GARMENT;
                    switch (unitCode)
                    {
                        case "C1A":
                            result += "." + UNIT_CENTRAL1A;
                            break;
                        case "C1B":
                            result += "." + UNIT_CENTRAL1B;
                            break;
                        case "C2A":
                            result += "." + UNIT_CENTRAL2A;
                            break;
                        case "C2B":
                            result += "." + UNIT_CENTRAL2B;
                            break;
                        case "C2C":
                            result += "." + UNIT_CENTRAL2C;
                            break;
                        default:
                            result += ".00";
                            break;
                    }
                    break;
                default:
                    result = "0.00";
                    break;
            }

            return result;
        }

        public static string GetPurchasingCOA(string division, string unitCode, string category)
        {
            var result = "";

            switch (category.ToString().ToUpper().Replace(" ", ""))
            {
                case "EM":
                    result += PEMBELIAN_BAHAN_EMBALASE + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "BB":
                    result += PEMBELIAN_BAHAN_BAKU + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "BP":
                    result += PEMBELIAN_BAHAN_PEMBANTU + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "BJ":
                    result += PEMBELIAN_BARANG_JADI + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                default:
                    result += DEFAULT_COA_IF_EMPTY + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
            }
            return result;
        }

        public static string GetStockCOA(string division, string unitCode, string categoryCode)
        {
            var result = "";

            switch (categoryCode.ToString().ToUpper().Replace(" ", ""))
            {
                case "BB":
                    result += PERSEDIAAN_BAHAN_BAKU + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "BJ":
                    result += PERSEDIAAN_BARANG_JADI + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "E":
                    result += PERSEDIAAN_CHEMICAL + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "MM":
                    result += PERSEDIAAN_BAHAN_BAKAR_INDUSTRI + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "PL":
                    result += PERSEDIAAN_PELUMAS + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "SP":
                    result += PERSEDIAAN_SPARE_PART + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "BP":
                    result += PERSEDIAAN_BAHAN_PEMBANTU + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "S":
                    result += PERSEDIAAN_CHEMICAL + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "R":
                    result += PERSEDIAAN_CHEMICAL + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                default:
                    result += DEFAULT_COA_IF_EMPTY + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
            }
            return result;
        }

        public static string GetIncomeTaxCOA(string incomeTaxArticle, string division, string unitCode)
        {
            var result = "";
            switch (incomeTaxArticle.ToString().ToUpper().Replace(" ", ""))
            {
                case "FINAL":
                    result = PPH_FINAL + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "PASAL21":
                    result = PPH21_YMH + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "PASAL23":
                    result = PPH23_YMH + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                case "PASAL26":
                    result = PPH26_YMH + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
                default:
                    result = DEFAULT_COA_IF_EMPTY + ".00." + GetDivisionAndUnitCOACode(division, unitCode);
                    break;
            }

            return result;
        }

        public static List<string> LIST_OF_STOCK_COA = new List<string>() { "BP", "BB", "EM", "S", "R", "E", "PL", "MM", "SP" };

        //public static string GetVATCOA(string division, string unitCode)
        //{
        //    return "";
        //}

        public static bool IsHavingStockCOA(string categoryCode)
        {

            return LIST_OF_STOCK_COA.Contains(categoryCode);
        }

        public static bool IsSparePart(string categoryCode)
        {
            return categoryCode.Equals("SP");
        }

        public static string GetCOAByCategoryCodeAndDivisionUnit(string categoryCode, string divisionName, string unitCode)
        {
            //var result = "";
            if (new List<string>() { "Q", "L", "OB", "MT", "MN", "LL", "EF" }.Contains(categoryCode))
            {
                return $"6020.35.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "U", "P2" }.Contains(categoryCode))
            {
                return $"5510.02.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "F" }.Contains(categoryCode))
            {
                return $"5510.09.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "B" }.Contains(categoryCode))
            {
                return $"5510.07.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "O", "KD" }.Contains(categoryCode))
            {
                return $"6020.19.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "CT", "AT" }.Contains(categoryCode))
            {
                return $"6020.08.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "IV", "IT" }.Contains(categoryCode))
            {
                return $"2110.00.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "AK" }.Contains(categoryCode))
            {
                return $"6020.18.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "MS" }.Contains(categoryCode))
            {
                return $"2303.00.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else if (new List<string>() { "PY" }.Contains(categoryCode))
            {
                return $"2302.00.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
            else
            {
                return $"9999.00.{GetDivisionAndUnitCOACode(divisionName, unitCode)}";
            }
        }

        //public static string 
    }

    public class JournalTransaction
    {
        public JournalTransaction()
        {
            Status = "POSTED";
        }

        public string Status { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? Date { get; set; }
        public string ReferenceNo { get; set; }
        public List<JournalTransactionItem> Items { get; set; }
        public string Remark { get; set; }
    }

    public class JournalTransactionItem
    {
        public COA COA { get; set; }
        public string Remark { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
    }

    public class COA
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
