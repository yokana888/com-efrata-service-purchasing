using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Serializers
{
    public class CustomStringSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            switch (type)
            {
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.Int32:
                    return Convert.ToString(context.Reader.ReadInt32());
                case BsonType.Int64:
                    return Convert.ToString(context.Reader.ReadInt64());
                case BsonType.Double:
                    return Convert.ToString(context.Reader.ReadDouble());
                case BsonType.ObjectId:
                    return context.Reader.ReadObjectId().ToString();
                default:
                    var message = string.Format("Cannot convert a {0} to an String.", type);
                    throw new NotSupportedException(message);
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            context.Writer.WriteString(value);
        }
    }
}