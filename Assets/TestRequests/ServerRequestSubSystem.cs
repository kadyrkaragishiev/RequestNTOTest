using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TestRequests
{
    public class ServerRequestSubSystem
    {
        public static async UniTask<string> GetTokenAuth(CancellationToken cancellationToken, string url, string login,
            string password)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", login);
            form.AddField("password", password);
            string jsonResponse = "";
            try
            {
                jsonResponse = await UnityWebRequestHelper.PostRequest(cancellationToken, url, form);
            }
            catch (RequestException e)
            {
                Debug.LogException(e);
            }

            return UnityWebRequestHelper.TryParseFromResponse(jsonResponse, out AuthData data)
                ? data.token
                : throw new Exception("Token not found");
        }

        public static async UniTask<string> PostSendFile(CancellationToken cancellationToken, string url, string path,
            HeaderData headerData = null)
        {
            string response = "";
            WWWForm form = new WWWForm();

            form.AddBinaryData("save_file", await File.ReadAllBytesAsync(path, cancellationToken),
                Path.GetFileName(path));
            try
            {
                response = await UnityWebRequestHelper.PostRequestSendFile(cancellationToken, url, form, headerData);
                Debug.Log(response);
            }
            catch (RequestException e)
            {
                Debug.LogException(e);
            }

            return response;
        }

        public static async UniTask<string> GetRequestFile(CancellationToken cancellationToken, string url, string path,
            HeaderData headerData = null)
        {
            string response = "";
            try
            {
                response = await UnityWebRequestHelper.GetRequestFile(cancellationToken, url, path, headerData);
            }
            catch (RequestException e)
            {
                Debug.LogException(e);
            }
            
            Debug.Log(response);

            return response;
        }
    }


    [Serializable]
    public class AuthData
    {
        public string token;
    }
}