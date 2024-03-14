using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace TestRequests
{
     public static class UnityWebRequestHelper
    {
        public static async UniTask<string> GetRequest(CancellationToken cancellationToken, string url, HeaderData headerData = null)
        {
            using var request = UnityWebRequest.Get(url);
            if (headerData != null) request.SetRequestHeader(headerData.Header, headerData.Data);

            return await Request(cancellationToken, request);
        }

        public static async UniTask<string> GetRequestUrl(CancellationToken cancellationToken, string url, HeaderData headerData)
        {
            using var request = UnityWebRequest.Get(url);
            if (headerData != null) request.SetRequestHeader(headerData.Header, headerData.Data);

            _ = await Request(cancellationToken, request);
            return request.url;
        }

        public static async UniTask<string> PostRequest(CancellationToken cancellationToken, string url, WWWForm form, HeaderData headerData = null)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, form);

            if (headerData != null) request.SetRequestHeader(headerData.Header, headerData.Data);

            return await Request(cancellationToken, request);
        }

        private static async UniTask<string> Request(CancellationToken cancellationToken, UnityWebRequest request)
        {
            try
            {
                _ = await request.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException requestException)
            {
                Debug.LogException(requestException);
                request.Dispose();
                throw new RequestException() { Body = requestException.Message };
            }
            catch (OperationCanceledException canceledException)
            {
                Debug.LogException(canceledException);
                request.Dispose();
                throw new RequestException() { Body = canceledException.Message };
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"request to \"{request.url}\" isn't success");
                return null;
            }
            string requestInfo = request.downloadHandler.text;
            request.Dispose();

            return requestInfo;
        }

        public static bool TryParseFromResponse<TData>(string jsonResponse, out TData data)
        {
            data = default;

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                try
                {
                    data = JsonConvert.DeserializeObject<TData>(jsonResponse);
                }
                catch (JsonException exception)
                {
                    Debug.LogException(exception);
                }
            }

            return data is not null;
        }


        private static UnityWebRequest PostWithJsonContent(string url, string json)
        {
            // From: https://forum.unity.com/threads/posting-raw-json-into-unitywebrequest.397871/
            var bytePostData = Encoding.UTF8.GetBytes(json);
            //use PUT method to send simple stream of bytes
            var request = UnityWebRequest.Put(url, bytePostData);
            //hack to send POST to server instead of PUT
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }
    }
    public class HeaderData
    {
        public string Header;
        public string Data;
    }

    public class RequestException : Exception
    {
        public string Title;
        public string Body;
        public bool IsHandled;
    }
}