using Ical.Net.DataTypes;
using NUnit.Framework;

namespace Ical.Net.CoreUnitTests;

[TestFixture]
public class DataTypeTest
{
    [Test, Category("DataType")]
    public void OrganizerConstructorMustAcceptNull()
    {
        Assert.DoesNotThrow(() => { var _ = new Organizer(null); });
    }

    [Test, Category("DataType")]
    public void AttachmentConstructorMustAcceptNull()
    {
        Assert.DoesNotThrow(() => { var _ = new Attachment((byte[])null); });
        Assert.DoesNotThrow(() => { var _ = new Attachment((string)null); });
    }
}