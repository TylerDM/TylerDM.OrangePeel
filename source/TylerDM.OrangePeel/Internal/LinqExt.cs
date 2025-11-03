namespace TylerDM.OrangePeel.Internal;

internal static class LinqExt
{
	internal static IEnumerable<T> SelectFollow<T>(this T root, Func<T, T?> getNext)
	{
		var next = getNext(root);
		while (next is not null)
		{
			yield return next;
			next = getNext(next);
		}
	}
}
