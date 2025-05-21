using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ArenaLink.ArenaLink;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace ArenaLink.Networking.Auth
{
    public class AuthClient
    {
        private static readonly HttpClient HttpClient = new();

        private readonly string _baseUrl = ArenaLinkService.Instance.GetApiBaseUrl() + "/Auth";

        public async Task<string> Login(string ticket)
        {
            try {
                var httpContentStr = JsonConvert.SerializeObject(new {AuthTicket = ticket});
                var content = new StringContent(httpContentStr, Encoding.UTF8, "application/json");
                var response = await HttpClient.PostAsync($"{_baseUrl}/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to login {response}");
                }

                var json = await response.Content.ReadAsStringAsync();

                return json;
            } catch (Exception e) {
                Debug.Log("Caught error: " + e);
                return "";
            }
        }
    }
}