namespace ForeignExchange2.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
	using Models;
    using Newtonsoft.Json;
    using Plugin.Connectivity;

    public class ApiService
    {
        public async Task<Response> CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Check your internet settings.",
                };
            }

			var reachable = await CrossConnectivity.Current.IsReachable(
				"google.com");
			if (!reachable)
			{
				return new Response
				{
					IsSuccess = false,
					Message = "Check your internet connection.",
				};
			}

			return new Response
			{
				IsSuccess = true,
			};
		}

        public async Task<Response> GetList<T>(string urlBase, string controller)
        {
			try
			{
				var client = new HttpClient();
				client.BaseAddress = new Uri(urlBase);
                var response = await client.GetAsync(controller);
				var result = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode)
				{
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
				}

                var list = JsonConvert.DeserializeObject<List<T>>(result);
				return new Response
				{
					IsSuccess = true,
					Result = list,
				};
			}
			catch (Exception ex)
			{
				return new Response
				{
					IsSuccess = false,
					Message = ex.Message,
				};
			}
		}
    }
}
