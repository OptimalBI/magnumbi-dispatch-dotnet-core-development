using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Optimal.MagnumMicroservices.Library{
    public class MagnumMicroservicesClient{
        private readonly string accessToken;
        private readonly HttpClient httpClient;
        private readonly string secretToken;
        private readonly string serverAddress;

        public MagnumMicroservicesClient(string serverAddress, string accessToken, string secretToken,
            bool verifySsl = true){
            this.serverAddress = serverAddress;
            this.accessToken = accessToken;
            this.secretToken = secretToken;

            HttpClientHandler handler =
                new HttpClientHandler{
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12,
                };
            if (!verifySsl) {
                handler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
            }

            byte[] byteArray = Encoding.ASCII.GetBytes($"{this.accessToken}:{this.secretToken}");
            this.httpClient = new HttpClient(handler);
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<bool> CheckStatus(){
            return await this.CheckStatusCode() == HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> CheckStatusCode(){
            HttpResponseMessage response = await this.httpClient.GetAsync($"{this.serverAddress}/job/");
            return response.StatusCode;
        }

    }
}