using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace Com.Efrata.Service.Purchasing.Lib.Serializers
{
    public class CustomDateTimeOffsetSerializer : SerializerBase<DateTimeOffset>
    {
        public override DateTimeOffset Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            switch (type)
            {
                case BsonType.DateTime:
                    return DateTimeOffset.FromUnixTimeMilliseconds(context.Reader.ReadDateTime());
                default:
                    var message = string.Format("Cannot convert a {0} to an DateTimeOffset.", type);
                    throw new NotSupportedException(message);
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTimeOffset value)
        {
            context.Writer.WriteDateTime(value.Millisecond);
        }
    }
}
