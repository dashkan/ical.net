using System;
using System.Collections.Generic;
using System.Linq;
using Ical.Net.DataTypes;
using NUnit.Framework;

namespace Ical.Net.CoreUnitTests;

internal class CollectionHelpersTests
{
    private static readonly DateTime _now = DateTime.UtcNow;

    private static List<PeriodList> GetExceptionDates()
        => new() { new PeriodList { new Period(new CalDateTime(_now.AddDays(1).Date)) } };

    [Test]
    public void ExDateTests()
    {
        Assert.AreEqual(GetExceptionDates(), GetExceptionDates());
        Assert.AreNotEqual(GetExceptionDates(), null);
        Assert.AreNotEqual(null, GetExceptionDates());

        var changedPeriod = GetExceptionDates();
        changedPeriod.First().First().StartTime = new CalDateTime(_now.AddHours(-1));

        Assert.AreNotEqual(GetExceptionDates(), changedPeriod);
    }
}