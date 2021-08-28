using System;
using System.Collections.Generic;

namespace Planly.DomainModel.Utility
{
	internal static class EnumerableExtensions
	{
		/// <summary>
		/// Lazily merges two sorted collections with a linear time complexity.
		/// </summary>
		public static IEnumerable<T> Merge<T>(this IEnumerable<T> firstCollection, IEnumerable<T> secondCollection)
			where T : IComparable<T>
		{
			var firstEnumerator = firstCollection.GetEnumerator();
			var secondEnumerator = secondCollection.GetEnumerator();

			var firstHasElements = firstEnumerator.MoveNext();
			var secondHasElements = secondEnumerator.MoveNext();
			while (firstHasElements && secondHasElements)
			{
				if (firstEnumerator.Current.CompareTo(secondEnumerator.Current) < 0)
				{
					yield return firstEnumerator.Current;
					firstHasElements = firstEnumerator.MoveNext();
				}
				else
				{
					yield return secondEnumerator.Current;
					secondHasElements = secondEnumerator.MoveNext();
				}
			}

			if (firstHasElements)
			{
				do
				{
					yield return firstEnumerator.Current;
				} while (firstEnumerator.MoveNext());
			}

			if (secondHasElements)
			{
				do
				{
					yield return secondEnumerator.Current;
				} while (secondEnumerator.MoveNext());
			}
		}
	}
}