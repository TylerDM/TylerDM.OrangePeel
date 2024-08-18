namespace TylerDM.OrangePeel.Internal;

internal record Implementation(Type type, DiAttribute? attribute, Type interfaceType)
{
	public Type Type { get; } = type;
	public DiAttribute? Attribute { get; } = attribute;
	public IReadOnlyCollection<Type> ClosedInterfaceTypes { get; } = getInterfaceTypes(type, interfaceType);

	private static List<Type> getInterfaceTypes(Type implementationType, Type openInterfaceType)
	{
		if (openInterfaceType.IsGenericType == false) return [openInterfaceType];

		//For generic interfaceTypes, we need to register the implementation with the closed generic, not the open one.
		var list = new List<Type>(1);

		var baseClassType = implementationType
			.SelectFollow(x => x.BaseType)
			.FirstOrDefault(x =>
				x.IsGenericType &&
				x.GetGenericTypeDefinition() == openInterfaceType
			);
		if (baseClassType is not null)
			list.Add(baseClassType);

		//There could be multiple interfaces.
		list.AddRange(
			implementationType
				.GetInterfaces()
				.Where(x =>
					x.IsGenericType &&
					x.GetGenericTypeDefinition() == openInterfaceType
				)
		);

		return list;
	}
}
