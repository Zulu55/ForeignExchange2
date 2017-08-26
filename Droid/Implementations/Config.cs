using Xamarin.Forms;

[assembly: Dependency(typeof(ForeignExchange2.Droid.Implementations.Config))]

namespace ForeignExchange2.Droid.Implementations
{
    using ForeignExchange2.Interfaces;
	using SQLite.Net.Interop;

	public class Config : IConfig
	{
		string directoryDB;
		ISQLitePlatform platform;

		public string DirectoryDB
		{
			get
			{
				if (string.IsNullOrEmpty(directoryDB))
				{
					directoryDB = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
				}

				return directoryDB;
			}
		}

		public ISQLitePlatform Platform
		{
			get
			{
				if (platform == null)
				{
					platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
				}

				return platform;

			}
		}
	}
}
