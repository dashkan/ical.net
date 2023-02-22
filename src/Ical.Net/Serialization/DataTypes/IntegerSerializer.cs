using System;
using System.IO;
using Ical.Net.DataTypes;

namespace Ical.Net.Serialization.DataTypes;

public class IntegerSerializer : EncodableDataTypeSerializer
{
    public IntegerSerializer() { }

    public IntegerSerializer(SerializationContext ctx) : base(ctx) { }

    public override Type TargetType => typeof (int);

    public override string SerializeToString(object integer)
    {
        try
        {
            var i = Convert.ToInt32(integer);

            if (SerializationContext.Peek() is not ICalendarObject obj) return i.ToString();
            // Encode the value as needed.
            var dt = new EncodableDataType
            {
                AssociatedObject = obj
            };
            return Encode(dt, i.ToString());
        }
        catch
        {
            return null;
        }
    }

    public override object Deserialize(TextReader tr)
    {
        var value = tr.ReadToEnd();

        try
        {
            if (SerializationContext.Peek() is ICalendarObject obj)
            {
                // Decode the value, if necessary!
                var dt = new EncodableDataType
                {
                    AssociatedObject = obj
                };
                value = Decode(dt, value);
            }

            if (Int32.TryParse(value, out var i))
            {
                return i;
            }
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch {}

        return value;
    }
}