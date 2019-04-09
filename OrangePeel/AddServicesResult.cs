namespace TylerDM.OrangePeel
{
	public class AddServicesResult
	{
		public int AddedServices { get; }
		public int AddedInterfaces { get; }

		public AddServicesResult(int addedServices, int addedInterfaces)
		{
			AddedServices = addedServices;
			AddedInterfaces = addedInterfaces;
		}
	}
}
