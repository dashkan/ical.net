using System.IO;
using Ical.Net.Serialization.DataTypes;

namespace Ical.Net.DataTypes;

/// <summary>
/// A class that represents the return status of an iCalendar request.
/// </summary>
public sealed class RequestStatus : EncodableDataType
{
    private string _mDescription;
    private string _mExtraData;
    private StatusCode _mStatusCode;

    public string Description
    {
        get => _mDescription;
        set => _mDescription = value;
    }

    public string ExtraData
    {
        get => _mExtraData;
        set => _mExtraData = value;
    }

    public StatusCode StatusCode
    {
        get => _mStatusCode;
        set => _mStatusCode = value;
    }

    public RequestStatus() {}

    public RequestStatus(string value) : this()
    {
        var serializer = new RequestStatusSerializer();
        CopyFrom(serializer.Deserialize(new StringReader(value)) as ICopyable);
    }

    public override void CopyFrom(ICopyable obj)
    {
        base.CopyFrom(obj);
        if (!(obj is RequestStatus status))
        {
            return;
        }

        if (status.StatusCode != null)
        {
            StatusCode = status.StatusCode;
        }
        Description = status.Description;
        status.ExtraData = status.ExtraData;
    }

    public override string ToString()
    {
        var serializer = new RequestStatusSerializer();
        return serializer.SerializeToString(this);
    }

    private bool Equals(RequestStatus other) => string.Equals(_mDescription, other._mDescription) && string.Equals(_mExtraData, other._mExtraData) &&
                                                Equals(_mStatusCode, other._mStatusCode);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((RequestStatus) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = _mDescription?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ (_mExtraData?.GetHashCode() ?? 0);
            hashCode = (hashCode * 397) ^ (_mStatusCode?.GetHashCode() ?? 0);
            return hashCode;
        }
    }
}