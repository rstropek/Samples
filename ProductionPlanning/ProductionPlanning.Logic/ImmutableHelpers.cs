using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProductionPlanning.Logic
{
	public static class ImmutableHelpers
	{
		public static TImmutable ToImmutable<TSource, TImmutable>(
			TSource source, Func<TSource, TImmutable> toImmutable) 
			where TSource : class where TImmutable : class
		{
			// Check if source is already immutable
			var immutableSource = source as TImmutable;
			return immutableSource != null
				? immutableSource // Already immutable, so don't copy
				: toImmutable(source); // Not yet immutable, to copy
		}

		public static IImmutableList<TImmutable> ToImmutableList<TSource, TImmutable>(
			IEnumerable<TSource> source, IDictionary<TSource, TImmutable> immutableFinder)
			where TSource : class where TImmutable : class
		{
			// Check if source is already immutable
			var immutableSource = source as IImmutableList<TImmutable>;
			if (immutableSource != null)
			{
				return immutableSource; // Already immutable, so don't copy
			}
			else
			{
				// Not yet immutable, so copy
				var immutableResult = ImmutableList<TImmutable>.Empty;
				if (source.Count() > 0)
				{
					// Create a builder to add all source items
					var resultBuilder = immutableResult.ToBuilder();
					resultBuilder.AddRange(source.Select(item =>
					{
						// Check if p is arely immutable
						var immutableItem = item as TImmutable;
						return immutableItem != null
							? immutableItem	// Already immutable, so don't copy
							: immutableFinder[item]; // Not yet immutable, so look up immutable
					}));
					immutableResult = resultBuilder.ToImmutable();
				}

				return immutableResult;
			}
		}
	}
}
