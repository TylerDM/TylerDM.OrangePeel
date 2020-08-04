namespace TylerDM.OrangePeel
{
	public class AddServicesResult
	{
		public int AddedServices { get; }
		public int AddedInterfaces { get; }

		public static AddServicesResult Empty { get; } = new AddServicesResult(0, 0);

		public AddServicesResult(int addedServices, int addedInterfaces)
		{
			AddedServices = addedServices;
			AddedInterfaces = addedInterfaces;
		}
	}
}