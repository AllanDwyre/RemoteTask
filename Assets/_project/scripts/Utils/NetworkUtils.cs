using System;
using System.Collections;
using Unity.Netcode;

namespace _project.scripts.utils
{
    public static class NetworkUtils
    {
        public static IEnumerator WaitForNetworkReady(Action onReady)
        {
            while (!NetworkManager.Singleton.IsListening)
            {
                yield return null; // wait for the next frame
            }
            onReady();
        }
    }
}