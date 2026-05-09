using System;
using System.Net.Http;

namespace Fut7Manager.Admin.Helpers {
    public static class ApiClient {
        private static HttpClient? _instance;

        public static HttpClient Instance
        {
            get {
                if (_instance == null) {
                    var handler = new HttpClientHandler {
                        ServerCertificateCustomValidationCallback =
                            (msg, cert, chain, errors) => true
                    };

                    _instance = new HttpClient(handler);

                    _instance.BaseAddress =
                        new Uri(AppConfig.ApiBaseUrl);

                    _instance.Timeout =
                        TimeSpan.FromSeconds(30);
                }

                return _instance;
            }
        }
    }
}