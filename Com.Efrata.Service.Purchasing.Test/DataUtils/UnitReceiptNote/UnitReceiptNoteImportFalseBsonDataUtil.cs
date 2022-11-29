using Com.Efrata.Service.Purchasing.Lib.Facades.Report;
using MongoDB.Bson;
using System;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.UnitReceiptNote
{
    public class UnitReceiptNoteImportFalseBsonDataUtil
    {
        private readonly LocalPurchasingBookReportFacade Facade;

        public UnitReceiptNoteImportFalseBsonDataUtil(LocalPurchasingBookReportFacade Facade)
        {
            this.Facade = Facade;
        }

        BsonDocument GetNewData()
        {
            return new BsonDocument
            {
                { "_id", ObjectId.GenerateNewId() },
                { "_deleted", false },
                { "no", "123456" },
                { "date", new BsonDateTime (new DateTime(2018, 2, 2)) },
                { "unit", new BsonDocument
                    {
                        { "code", "U1" },
                        { "name", "Unit1" }
                    }
                },
                { "supplier", new BsonDocument
                    {
                        { "import", false }
                    }
                },
                { "items", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "product", new BsonDocument
                                {
                                    { "code", "P1" },
                                    { "name", "Produk1" }
                                }
                            },
                            { "purchaseOrder", new BsonDocument
                                {
                                    { "category", new BsonDocument
                                        {
                                            { "code", "C1" },
                                            { "name", "Category1" }
                                        }
                                    }
                                }
                            },
                            { "deliveredQuantity", 200 },
                            { "pricePerDealUnit", 1000 },
                            { "unitReceiptNote", new BsonDocument
                                {
                                    { "no", "123" }
                                }
                            }
                        }
                    }
                }
            };
        }

        BsonDocument GetNewData2()
        {
            return new BsonDocument
            {
                { "_id", ObjectId.GenerateNewId() },
                { "_deleted", false },
                { "no", "123456" },
                { "date", new BsonDateTime (new DateTime(2018, 2, 2)) },
                { "unit", new BsonDocument
                    {
                        { "code", "U1" },
                        { "name", "Unit1" }
                    }
                },
                { "supplier", new BsonDocument
                    {
                        { "import", false }
                    }
                },
                { "items", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "product", new BsonDocument
                                {
                                    { "code", "P1" },
                                    { "name", "Produk1" }
                                }
                            },
                            { "purchaseOrder", new BsonDocument
                                {
                                    { "category", new BsonDocument
                                        {
                                            { "code", "C1" },
                                            { "name", "Category1" }
                                        }
                                    },
                                    { "useIncomeTax",true}
                                }
                            },
                            { "deliveredQuantity", 200 },
                            { "pricePerDealUnit", 1000 },
                            { "unitReceiptNote", new BsonDocument
                                {
                                    { "no", "123" }
                                }
                            }
                        }
                    }
                }
            };
        }

        //public BsonDocument GetTestData()
        //{
        //    BsonDocument data = GetNewData();
        //    this.Facade.DeleteDataMongoByNo(data["no"].AsString);
        //    this.Facade.InsertToMongo(data);

        //    return data;
        //}

        //public BsonDocument GetTestData2()
        //{
        //    BsonDocument data = GetNewData2();
        //    this.Facade.DeleteDataMongoByNo(data["no"].AsString);
        //    this.Facade.InsertToMongo(data);

        //    return data;
        //}
    }
}
