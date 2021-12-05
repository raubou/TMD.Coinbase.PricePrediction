using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TMDInvestment.APIProxy
{
    public class APIProxy<T> where T : class
    {
        public APIProxy()
        {
            
        }
        public static T Get(string url, ref dynamic error)
        {
            T results = default(T);
            using (HttpClient _client = new HttpClient())
            {
                HttpResponseMessage responseMsg = null;
                try
                {
                    responseMsg = _client.GetAsync(url).Result;
                }
                catch (Exception)
                {
                    return null;
                }
                
                if (responseMsg.IsSuccessStatusCode && responseMsg.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        results = JsonSerializer.DeserializeAsync<T>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
                    }
                    catch (Exception)
                    {
                        return null;
                    }                    
                }
                else
                {
                    try
                    {
                        error = JsonSerializer.DeserializeAsync<dynamic>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
                    }
                    catch (System.Exception)
                    {
                        error = responseMsg.Content.ReadAsStringAsync().Result;
                    }
                }
                _client.Dispose();
            }
            return results;
        }
        //public static T Get(string url, string token, ref dynamic error)
        //{
        //    T results = default(T);
        //    using (HttpClient _client = new HttpClient())
        //    {
        //        _client.DefaultRequestHeaders.Authorization
        //                 = new AuthenticationHeaderValue("Bearer", token);
        //        HttpResponseMessage responseMsg = null;
        //        try
        //        {
        //            responseMsg = _client.GetAsync(url).Result;
        //        }
        //        catch (Exception)
        //        {
        //            return null;
        //        }
        //        if (responseMsg.IsSuccessStatusCode && responseMsg.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            results = JsonSerializer.DeserializeAsync<T>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //        }
        //        else
        //        {
        //            try
        //            {
        //                error = JsonSerializer.DeserializeAsync<dynamic>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //            }
        //            catch (System.Exception)
        //            {
        //                error = responseMsg.Content.ReadAsStringAsync().Result;
        //            }
        //        }
        //        _client.Dispose();
        //    }
        //    return results;
        //}

        //public static T Delete(string url, string token, ref dynamic error)
        //{
        //    T results = default(T);
        //    using (HttpClient _client = new HttpClient())
        //    {
        //        _client.DefaultRequestHeaders.Authorization
        //                 = new AuthenticationHeaderValue("Bearer", token);
        //        HttpResponseMessage responseMsg = _client.DeleteAsync(url).Result;
        //        if (responseMsg.IsSuccessStatusCode && responseMsg.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            results = JsonSerializer.DeserializeAsync<T>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //        }
        //        else
        //        {
        //            try
        //            {
        //                error = JsonSerializer.DeserializeAsync<dynamic>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //            }
        //            catch (System.Exception)
        //            {
        //                error = responseMsg.Content.ReadAsStringAsync().Result;
        //            }
        //        }
        //        _client.Dispose();
        //    }
        //    return results;
        //}

        public static T Post(string url, HttpContent _content, ref dynamic error)
        {
            T results = default(T);
            using (HttpClient _client = new HttpClient())
            {
                HttpResponseMessage responseMsg = null;
                try
                {
                    responseMsg = _client.PostAsync(url, _content).Result;
                }
                catch (Exception)
                {
                    return null;
                }
                if (responseMsg.IsSuccessStatusCode && responseMsg.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        results = JsonSerializer.DeserializeAsync<T>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    try
                    {
                        error = JsonSerializer.DeserializeAsync<dynamic>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
                    }
                    catch (System.Exception)
                    {
                        error = responseMsg.Content.ReadAsStringAsync().Result;
                    }
                }
                _client.Dispose();
            }
            return results;
        }

        //public static T Post(string url, string token, HttpContent _content, ref dynamic error)
        //{
        //    T results = default(T);
        //    using (HttpClient _client = new HttpClient())
        //    {
        //        _client.DefaultRequestHeaders.Authorization
        //            = new AuthenticationHeaderValue("Bearer", token);
        //        HttpResponseMessage responseMsg = _client.PostAsync(url, _content).Result;
        //        if (responseMsg.IsSuccessStatusCode && responseMsg.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            results = JsonSerializer.DeserializeAsync<T>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //        }
        //        else
        //        {
        //            try
        //            {
        //                error = JsonSerializer.DeserializeAsync<dynamic>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //            }
        //            catch (System.Exception)
        //            {
        //                error = responseMsg.Content.ReadAsStringAsync().Result;
        //            }
        //        }
        //        _client.Dispose();
        //    }
        //    return results;
        //}

        //public static T Put(string url, string token, HttpContent _content, ref dynamic error)
        //{
        //    T results = default(T);
        //    using (HttpClient _client = new HttpClient())
        //    {
        //        _client.DefaultRequestHeaders.Authorization
        //            = new AuthenticationHeaderValue("Bearer", token);
        //        HttpResponseMessage responseMsg = _client.PutAsync(url, _content).Result;
        //        if (responseMsg.IsSuccessStatusCode && responseMsg.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            results = JsonSerializer.DeserializeAsync<T>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //        }
        //        else
        //        {
        //            try
        //            {
        //                error = JsonSerializer.DeserializeAsync<dynamic>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //            }
        //            catch (System.Exception)
        //            {
        //                error = responseMsg.Content.ReadAsStringAsync().Result;
        //            }
        //        }
        //        _client.Dispose();
        //    }
        //    return results;
        //}

        //public static T Send(HttpRequestMessage request, ref dynamic error)
        //{
        //    T results = default(T);
        //    using (HttpClient _client = new HttpClient())
        //    {
        //        HttpResponseMessage responseMsg = _client.SendAsync(request).Result;
        //        if (responseMsg.IsSuccessStatusCode && responseMsg.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            results = JsonSerializer.DeserializeAsync<T>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //        }
        //        else
        //        {
        //            try
        //            {
        //                error = JsonSerializer.DeserializeAsync<dynamic>(responseMsg.Content.ReadAsStreamAsync().Result, InitialSerializerOptions()).Result;
        //            }
        //            catch (System.Exception)
        //            {
        //                error = responseMsg.Content.ReadAsStringAsync().Result;
        //            }                    
        //        }
        //        _client.Dispose();
        //    }
        //    return results;
        //}
        private static JsonSerializerOptions InitialSerializerOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.AllowTrailingCommas = true;
            options.PropertyNameCaseInsensitive = true;
            options.IgnoreNullValues = true;
            options.IgnoreReadOnlyProperties = true;
            options.MaxDepth = 128;
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            return options;
        }
    }
}
