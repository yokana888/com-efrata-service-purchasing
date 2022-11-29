using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Text.RegularExpressions;

namespace Com.Efrata.Service.Purchasing.Lib.Serializers
{
    public class CustomDoubleSerializer : SerializerBase<double>
    {
        public override double Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            switch (type)
            {
                case BsonType.Double:
                    return context.Reader.ReadDouble();
                case BsonType.Int32:
                    return Convert.ToDouble(context.Reader.ReadInt32());
                case BsonType.Int64:
                    return Convert.ToDouble(context.Reader.ReadInt64());
                case BsonType.String:
                    double result;
                    double.TryParse(Regex.Replace(context.Reader.ReadString(), "[^0-9]", ""), out result);
                    return result;
                default:
                    var message = string.Format("Cannot convert a {0} to an Double.", type);
                    throw new NotSupportedException(message);
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, double value)
        {
            context.Writer.WriteDouble(value);
        }
    }
}
