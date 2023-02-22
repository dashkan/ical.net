using System;
using System.IO;
using Ical.Net.DataTypes;

namespace Ical.Net.Serialization.DataTypes;

public class EnumSerializer : EncodableDataTypeSerializer
{
    private readonly Type _mEnumType;

    public EnumSerializer(Type enumType)
    {
        _mEnumType = enumType;
    }

    public EnumSerializer(Type enumType, SerializationContext ctx) : base(ctx)
    {
        _mEnumType = enumType;
    }

    public override Type TargetType => _mEnumType;

    public override string SerializeToString(object enumValue)
    {
        try
        {
            if (SerializationContext.Peek() is not ICalendarObject obj) return enumValue.ToString();
            // Encode the value as needed.
            var dt = new EncodableDataType
            {
                AssociatedObject = obj
            };
            return Encode(dt, enumValue.ToString());
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
            if (SerializationContext.Peek() is not ICalendarObject obj)
                return Enum.Parse(_mEnumType, value.Replace("-", ""), true);
            // Decode the value, if necessary!
            var dt = new EncodableDataType
            {
                AssociatedObject = obj
            };
            value = Decode(dt, value);

            // Remove "-" characters while parsing Enum values.
            return Enum.Parse(_mEnumType, value.Replace("-", ""), true);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch {}

        return value;
    }
}