using System.Collections.Generic;

namespace DestinyMod.Core.Extensions
{
	public static class IListExtensions
	{
		public static void Add<T>(this IList<T> list, params T[] items)
		{
			foreach (T item in items)
			{
				list.Add(item);
			}
		}
	}
}