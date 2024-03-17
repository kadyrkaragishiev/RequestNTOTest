using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace TestRequests
{
    public class RequestTester : MonoBehaviour
    {
        [SerializeField]
        private string url = "";

        [SerializeField]
        private string login;

        [SerializeField]
        private string password;

        [SerializeField]
        private string sendFileName;

        [SerializeField]
        private string saveFileName;

        private HeaderData headerData = null;

        private string token;

        private string filePath;


        private void Start()
        {
            filePath = Application.dataPath + "/" + sendFileName;
        }

        public void Auth()
        {
            GetTokenAuth();
        }
        
        public void SendFile()
        {
            PostSendFile();
        }
        
        public void GetFile()
        {
            GetFileUrl();
        }

        private async void GetTokenAuth()
        {
            if (login == "" || password == "") return;
            token = (await ServerRequestSubSystem.GetTokenAuth(CancellationToken.None, url + "/auth/", login,
                password));
        }


        private async void PostSendFile()
        {
            if (token == "") return;
            await ServerRequestSubSystem.PostSendFile(CancellationToken.None, url + "/cloud-saving/save/",
                filePath, new HeaderData {Header = "Authorization", Data = "Token " + token});
        }

        private async void GetFileUrl()
        {
            if (token == "") return;
            var data = await ServerRequestSubSystem.GetRequestFile(CancellationToken.None,
                url + "/cloud-saving/save/",
                Application.dataPath + "/" + saveFileName,
                new HeaderData {Header = "Authorization", Data = "Token " + token});
            StartCoroutine(DownloadFile(data));
            Debug.Log("saved");
        }


        IEnumerator DownloadFile(string fileName)
        {
            fileName = fileName.Replace("\"", "");
            var uwr = new UnityWebRequest(url + "/cloud_saving/savings/cloud_saving/savings/" + fileName,
                UnityWebRequest.kHttpVerbGET);

            string path = Path.Combine(Application.dataPath, "1", fileName);
            uwr.downloadHandler = new DownloadHandlerFile(path);
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.LogError(uwr.error);
            else
                Debug.Log("File successfully downloaded and saved to " + path);
        }
    }
}