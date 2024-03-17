using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

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

        private HeaderData headerData = null;

        private string token;

        private string filePath;


        private IEnumerator Start()
        {
            filePath = Application.dataPath + "/cutting_mat.fbx";
            GetTokenAuth();
            yield return new WaitForSeconds(2f);
            // PostSendFile();
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
            byte[] data = await ServerRequestSubSystem.GetRequestFile(CancellationToken.None,
                url + "/cloud-saving/save/",
                Application.dataPath + "/mat_.fbx",
                new HeaderData {Header = "Authorization", Data = "Token " + token});
            File.WriteAllBytes(Application.dataPath + "/mat_.fbx", data);
        }
    }
}