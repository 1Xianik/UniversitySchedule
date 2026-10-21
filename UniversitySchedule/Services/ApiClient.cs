using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniversitySchedule.WPF.Models;

namespace UniversitySchedule.WPF.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private string? _token;

        public ApiClient()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };
            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Сохранение токена после логина
        public void SetToken(string token)
        {
            _token = token;
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearToken()
        {
            _token = null;
            _http.DefaultRequestHeaders.Authorization = null;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

        // ==================== GET ====================
        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("Требуется авторизация");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");

                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, GetJsonOptions());
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Не удалось подключиться к API. Запущен ли сервер? ({ex.Message})");
            }
        }

        // ==================== POST ====================
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var json = JsonSerializer.Serialize(data, GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Требуется авторизация");
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Недостаточно прав");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {(int)response.StatusCode}: {responseJson}");

            return JsonSerializer.Deserialize<TResponse>(responseJson, GetJsonOptions());
        }

        // ==================== POST без ответа ====================
        public async Task<bool> PostAsync<TRequest>(string url, TRequest data)
        {
            var json = JsonSerializer.Serialize(data, GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        // ==================== PUT ====================
        public async Task<bool> PutAsync<TRequest>(string url, TRequest data)
        {
            var json = JsonSerializer.Serialize(data, GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PutAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        // ==================== DELETE ====================
        public async Task<bool> DeleteAsync(string url)
        {
            var response = await _http.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        // ==================== JSON OPTIONS ====================
        private static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }
}