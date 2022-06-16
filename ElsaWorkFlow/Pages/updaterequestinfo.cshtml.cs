using ElsaWorkFlow.DomainDataBase;
using ElsaWorkFlow.DomainDataBase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ElsaWorkFlow.Pages
{
    public class updaterequestinfoModel : PageModel
    {
        private readonly BlogDBContext blogDBContext;

        public updaterequestinfoModel(BlogDBContext blogDBContext)
        {
            this.blogDBContext = blogDBContext;
        }
        public void OnGet()
        {
            var requestId = Request.Query["id"];
            var request=new Request();
            if(!string.IsNullOrEmpty(requestId))
                request= blogDBContext.Requests.Find(requestId);
        }
        public async void OnPost()
        {
            var body = new { workflowInstanceId = Request.Query["workflowInstanceId"][0].ToString() };

            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://localhost:44303/v1/signals/request-update-info/execute";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            var result = await response.Content.ReadAsStringAsync();
        }
    }
}
