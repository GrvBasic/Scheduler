using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DownloadPlugin
{
    public class ImageDownloadTester : MonoBehaviour
    {
        // Start is called before the first frame update
        public string url =
            "https://downloadhdwallpapers.in/wp-content/uploads/2017/12/Green-Apple-on-Tree.jpg";

        public Image image;
        void Start()
        {
            ImageDownloader.GetImage(url,"DownloadtestIamge",OnDownloadComplete,OnFailed);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene("SampleScene");
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadScene("SampleScene 1");
            }
        }

        void OnDownloadComplete(Sprite sprite)
        {
            image.sprite = sprite;
        }

        void OnFailed(string error)
        {
            Debug.Log($"error {error}");
        }
    }
}
