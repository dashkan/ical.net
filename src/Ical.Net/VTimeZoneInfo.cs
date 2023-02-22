﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;

namespace Ical.Net;

public sealed class VTimeZoneInfo : CalendarComponent, IRecurrable
{
    TimeZoneInfoEvaluator _evaluator;

    public VTimeZoneInfo()
    {
        // TODO: how do we ensure SEQUENCE doesn't get serialized?
        //base.Sequence = null;
        // iCalTimeZoneInfo does not allow sequence numbers
        // Perhaps we should have a custom serializer that fixes this?

        Initialize();
    }
    public VTimeZoneInfo(string name) : this()
    {
        Name = name;
    }

    void Initialize()
    {
        _evaluator = new TimeZoneInfoEvaluator(this);
        SetService(_evaluator);
    }

    protected override void OnDeserializing(StreamingContext context)
    {
        base.OnDeserializing(context);

        Initialize();
    }

    public override bool Equals(object obj)
    {
        if (obj is VTimeZoneInfo tzi)
        {
            return Equals(TimeZoneName, tzi.TimeZoneName) &&
                   Equals(OffsetFrom, tzi.OffsetFrom) &&
                   Equals(OffsetTo, tzi.OffsetTo);
        }
        return base.Equals(obj);
    }

    public string TzId =>
        Parent is not VTimeZone tz
            ? null
            : tz.TzId;

    /// <summary>
    /// Returns the name of the current Time Zone.
    /// <example>
    ///     The following are examples:
    ///     <list type="bullet">
    ///         <item>EST</item>
    ///         <item>EDT</item>
    ///         <item>MST</item>
    ///         <item>MDT</item>
    ///     </list>
    /// </example>
    /// </summary>
    public string TimeZoneName
    {
        get => TimeZoneNames.Count > 0
            ? TimeZoneNames[0]
            : null;
        set
        {
            TimeZoneNames.Clear();
            TimeZoneNames.Add(value);
        }
    }

    public UtcOffset TzOffsetFrom
    {
        get => OffsetFrom;
        set => OffsetFrom = value;
    }

    public UtcOffset OffsetFrom
    {
        get => Properties.Get<UtcOffset>("TZOFFSETFROM");
        set => Properties.Set("TZOFFSETFROM", value);
    }

    public UtcOffset OffsetTo
    {
        get => Properties.Get<UtcOffset>("TZOFFSETTO");
        set => Properties.Set("TZOFFSETTO", value);
    }

    public UtcOffset TzOffsetTo
    {
        get => OffsetTo;
        set => OffsetTo = value;
    }

    public IList<string> TimeZoneNames
    {
        get => Properties.GetMany<string>("TZNAME");
        set => Properties.Set("TZNAME", value);
    }

    public IDateTime DtStart
    {
        get => Start;
        set => Start = value;
    }

    public IDateTime Start
    {
        get => Properties.Get<IDateTime>("DTSTART");
        set => Properties.Set("DTSTART", value);
    }

    public IList<PeriodList> ExceptionDates
    {
        get => Properties.GetMany<PeriodList>("EXDATE");
        set => Properties.Set("EXDATE", value);
    }

    public IList<RecurrencePattern> ExceptionRules
    {
        get => Properties.GetMany<RecurrencePattern>("EXRULE");
        set => Properties.Set("EXRULE", value);
    }

    public IList<PeriodList> RecurrenceDates
    {
        get => Properties.GetMany<PeriodList>("RDATE");
        set => Properties.Set("RDATE", value);
    }

    public IList<RecurrencePattern> RecurrenceRules
    {
        get => Properties.GetMany<RecurrencePattern>("RRULE");
        set => Properties.Set("RRULE", value);
    }

    public IDateTime RecurrenceId
    {
        get => Properties.Get<IDateTime>("RECURRENCE-ID");
        set => Properties.Set("RECURRENCE-ID", value);
    }

    public void ClearEvaluation()
    {
        RecurrenceUtil.ClearEvaluation(this);
    }

    public HashSet<Occurrence> GetOccurrences(IDateTime dt)
        => RecurrenceUtil.GetOccurrences(this, dt, true);

    public HashSet<Occurrence> GetOccurrences(DateTime dt)
        => RecurrenceUtil.GetOccurrences(this, new CalDateTime(dt, true), true);

    public HashSet<Occurrence> GetOccurrences(IDateTime startTime, IDateTime endTime)
        => RecurrenceUtil.GetOccurrences(this, startTime, endTime, true);

    public HashSet<Occurrence> GetOccurrences(DateTime startTime, DateTime endTime)
        => RecurrenceUtil.GetOccurrences(this, new CalDateTime(startTime, true), new CalDateTime(endTime, true), true);
}