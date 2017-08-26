namespace ForeignExchange2.Services
{
    using System.Threading.Tasks;
    using Helpers;
    using Xamarin.Forms;

	public class DialogService
    {
        public async Task ShowMessage(string title, string messsage)
        {
            await Application.Current.MainPage.DisplayAlert(
                title,
                messsage,
                Lenguages.Accept);
        }
    }
}
