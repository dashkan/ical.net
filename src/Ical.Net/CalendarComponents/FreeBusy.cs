﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ical.Net.DataTypes;
using Ical.Net.Utility;

namespace Ical.Net.CalendarComponents;

public sealed class FreeBusy : UniqueComponent, IMergeable
{
    public static FreeBusy Create(ICalendarObject obj, FreeBusy freeBusyRequest)
    {
        if (!(obj is IGetOccurrencesTyped getOccurrences))
        {
            return null;
        }

        var occurrences = getOccurrences.GetOccurrences<CalendarEvent>(freeBusyRequest.Start, freeBusyRequest.End);
        var contacts = new List<string>();
        var isFilteredByAttendees = false;

        if (freeBusyRequest.Attendees is {Count: > 0})
        {
            isFilteredByAttendees = true;
            var attendees = freeBusyRequest.Attendees
                .Where(a => a.Value != null)
                .Select(a => a.Value.OriginalString.Trim());
            contacts.AddRange(attendees);
        }

        var fb = freeBusyRequest;
        fb.Uid = Guid.NewGuid().ToString();
        fb.Entries.Clear();
        fb.DtStamp = CalDateTime.Now;

        foreach (var o in occurrences)
        {
            var uc = o.Source as IUniqueComponent;

            if (uc == null)
            {
                continue;
            }

            var evt = uc as CalendarEvent;
            var accepted = false;
            var type = FreeBusyStatus.Busy;

            // We only accept events, and only "opaque" events.
            if (evt != null && evt.Transparency != TransparencyType.Transparent)
            {
                accepted = true;
            }

            // If the result is filtered by attendees, then
            // we won't accept it until we find an event
            // that is being attended by this person.
            if (accepted && isFilteredByAttendees)
            {
                accepted = false;

                var participatingAttendeeQuery = uc.Attendees
                    .Where(attendee =>
                        attendee.Value != null
                        && contacts.Contains(attendee.Value.OriginalString.Trim())
                        && attendee.ParticipationStatus != null)
                    .Select(pa => pa.ParticipationStatus.ToUpperInvariant());

                foreach (var participatingAttendee in participatingAttendeeQuery)
                {
                    switch (participatingAttendee)
                    {
                        case EventParticipationStatus.Tentative:
                            accepted = true;
                            type = FreeBusyStatus.BusyTentative;
                            break;
                        case EventParticipationStatus.Accepted:
                            accepted = true;
                            type = FreeBusyStatus.Busy;
                            break;
                    }
                }
            }

            if (accepted)
            {
                // If the entry was accepted, add it to our list!
                fb.Entries.Add(new FreeBusyEntry(o.Period, type));
            }
        }

        return fb;
    }

    public static FreeBusy CreateRequest(IDateTime fromInclusive, IDateTime toExclusive, Organizer organizer, IEnumerable<Attendee> contacts)
    {
        var fb = new FreeBusy
        {
            DtStamp = CalDateTime.Now,
            DtStart = fromInclusive,
            DtEnd = toExclusive
        };
        if (organizer != null)
        {
            fb.Organizer = organizer;
        }

        if (contacts == null)
        {
            return fb;
        }
        foreach (var attendee in contacts)
        {
            fb.Attendees.Add(attendee);
        }

        return fb;
    }

    public FreeBusy()
    {
        Name = Components.Freebusy;
    }

    public IList<FreeBusyEntry> Entries
    {
        get => Properties.GetMany<FreeBusyEntry>("FREEBUSY");
        set => Properties.Set("FREEBUSY", value);
    }

    public IDateTime DtStart
    {
        get => Properties.Get<IDateTime>("DTSTART");
        set => Properties.Set("DTSTART", value);
    }

    public IDateTime DtEnd
    {
        get => Properties.Get<IDateTime>("DTEND");
        set => Properties.Set("DTEND", value);
    }

    public IDateTime Start
    {
        get => Properties.Get<IDateTime>("DTSTART");
        set => Properties.Set("DTSTART", value);
    }

    public IDateTime End
    {
        get => Properties.Get<IDateTime>("DTEND");
        set => Properties.Set("DTEND", value);
    }

    public FreeBusyStatus GetFreeBusyStatus(Period period)
    {
        var status = FreeBusyStatus.Free;
        if (period == null)
        {
            return status;
        }

        foreach (var fbe in Entries.Where(fbe => fbe.CollidesWith(period) && status < fbe.Status))
        {
            status = fbe.Status;
        }
        return status;
    }

    public FreeBusyStatus GetFreeBusyStatus(IDateTime dt)
    {
        var status = FreeBusyStatus.Free;
        if (dt == null)
        {
            return status;
        }

        foreach (var fbe in Entries.Where(fbe => fbe.Contains(dt) && status < fbe.Status))
        {
            status = fbe.Status;
        }
        return status;
    }

    public void MergeWith(IMergeable obj)
    {
        if (!(obj is FreeBusy fb))
        {
            return;
        }

        Entries.AddRange(fb.Entries.Where(entry => !Entries.Contains(entry)));
    }
}