using System.Collections.Generic;

namespace Ical.Net.Collections;

public sealed class MultiLinkedList<TType> :
    List<TType>,
    IMultiLinkedList<TType>
{
    private IMultiLinkedList<TType> _previous;

    public int StartIndex => _previous?.ExclusiveEnd ?? 0;

    public int ExclusiveEnd => Count > 0 ? StartIndex + Count : StartIndex;
}