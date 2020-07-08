using System;
using System.Collections.Generic;
using System.Linq;

namespace PrsnlMgmt.Mvvm.Model
{
    public static class SelectableItemExtensions
    {
        public static IEnumerable<SelectableItem<TItem>> ToSelectableItems<TItem>(this IEnumerable<TItem> source)
        {
            return source.Select(i => new SelectableItem<TItem>(i));
        }

        public static IEnumerable<SelectableItem<TItem>> ToSelectableItems<TSource, TItem>(this IEnumerable<TSource> source, Func<TSource, TItem> itemSelector)
        {
            return source.Select(i => new SelectableItem<TItem>(itemSelector(i), i));
        }
    }
}