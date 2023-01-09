using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Fims.Data.Models;


namespace Fims.Client.Shared.Infrastructure.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<Result> ToResult(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();

                return Result.Failure(errors);
            }

            return Result.Success;
        }
    }
}
