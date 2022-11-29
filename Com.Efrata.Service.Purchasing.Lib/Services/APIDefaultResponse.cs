using System;

namespace Com.Efrata.Service.Purchasing.Lib.Services
{
    class APIDefaultResponse <T>
    {
        public T data { get; set; }
    }

    public class GarmentCurrency
    {
        public string UId { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public double? Rate { get; set; }
    }

	public class GarmentProduct
	{
		public string Code { get; set; }
		public string Const { get; set; }
		public string Yarn { get; set; }
		public string Width { get; set; }
	}
}
