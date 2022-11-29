using MongoDB.Bson;
using System;

namespace Com.Efrata.Service.Purchasing.Lib.Helpers
{
    public class GetBsonValue
    {
        public static string ToString(BsonDocument bsonDocument, string field, BsonString bsonString)
        {
            MongoDB.Bson.BsonValue bsonValue;
            string[] fields = field.Split(".");

            bsonValue = fields.Length > 1 ?
                bsonDocument.GetValue(fields[0], new BsonDocument()) :
                bsonDocument.GetValue(fields[0], bsonString);

            for (int i = 1; i < fields.Length; i++)
            {
                bsonValue = i < field.Length ?
                    bsonValue.IsBsonArray ?
                        bsonValue.AsBsonArray[0].AsBsonDocument.GetValue(fields[i], new BsonDocument()) :
                        bsonValue.AsBsonDocument.GetValue(fields[i], new BsonDocument()) :
                    bsonValue.IsBsonArray ?
                        bsonValue.AsBsonArray[0].AsBsonDocument.GetValue(fields[i], bsonString) :
                        bsonValue.AsBsonDocument.GetValue(fields[i], bsonString);
            }

            if (bsonValue.IsString) return bsonValue.AsString;
            else if (bsonValue.IsInt32) return bsonValue.AsInt32.ToString();
            else if (bsonValue.IsDouble) return bsonValue.AsDouble.ToString();
            else throw new Exception("Cannot convert to string");
        }

        public static string ToString(BsonDocument bsonDocument, string field)
        {
            return GetBsonValue.ToString(bsonDocument, field, new BsonString(""));
        }

        public static double ToDouble(BsonDocument bsonDocument, string field)
        {
            MongoDB.Bson.BsonValue bsonValue;
            string[] fields = field.Split(".");

            bsonValue = fields.Length > 1 ?
                bsonDocument.GetValue(fields[0], new BsonDocument()) :
                bsonDocument.GetValue(fields[0], new BsonDouble(0));

            for (int i = 1; i < fields.Length; i++)
            {
                bsonValue = i < field.Length ?
                    bsonValue.IsBsonArray ?
                        bsonValue.AsBsonArray[0].AsBsonDocument.GetValue(fields[i], new BsonDocument()) :
                        bsonValue.AsBsonDocument.GetValue(fields[i], new BsonDocument()) :
                    bsonValue.IsBsonArray ?
                        bsonValue.AsBsonArray[0].AsBsonDocument.GetValue(fields[i], new BsonDouble(0)) :
                        bsonValue.AsBsonDocument.GetValue(fields[i], new BsonDouble(0));
            }

            if (bsonValue.IsString) return double.Parse(bsonValue.AsString);
            else if (bsonValue.IsInt32) return bsonValue.AsInt32;
            else if (bsonValue.IsDouble) return bsonValue.AsDouble;
            else throw new Exception("Cannot convert to double");
        }
    }
}