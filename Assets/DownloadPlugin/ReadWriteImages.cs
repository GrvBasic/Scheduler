using System;
using System.IO;
using UnityEngine;

namespace DownloadPlugin
{
    public static class ReadWriteImages
    {
        private static string _imageFolderLocation = Path.Combine(Application.persistentDataPath, "data", "images");

        public static void WriteCachedImageInFile(Texture2D imageR, string imageNameR)
        {
            _imageFolderLocation = GetFilePath(_imageFolderLocation);
            string imageFile = Path.Combine(_imageFolderLocation, imageNameR+".png");

            Texture2D itemBGTex = imageR;
            byte[] itemBGBytes = itemBGTex.EncodeToPNG();

            try
            {
                File.WriteAllBytes(imageFile, itemBGBytes);
//    Debug.Log("Saved Data to: " + filePath.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To Save Data to: " + _imageFolderLocation.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }
        }

        public static void ReadAllCachedImage()
        {
            _imageFolderLocation = GetFilePath(_imageFolderLocation);
            string[] imageNames = System.IO.Directory.GetFiles(_imageFolderLocation);
            foreach (string imageName in imageNames)
            {
                /*Debug.Log(fileName);
                string imageFileToRead = Path.Combine(_imageFolderLocation, fileName);
      
                byte[] img = LoadCachedImage(fileName);
                var texture = new Texture2D(200, 200); //128 x 128 can be the download image size
                texture.LoadImage(img);
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Sprite savedSprite = Sprite.Create(texture, rect, new Vector2(1, 1));
                MemoryCache.CacheImage(Path.GetFileNameWithoutExtension(fileName), savedSprite, false);*/
                ReadImage(imageName);
            }
        }

        public static Sprite ReadImage(string imageNameR)
        {
          
         //   string imageFileToRead = Path.Combine(_imageFolderLocation, fileNameR);
      
            byte[] img = LoadCachedImage(imageNameR+".png");
            if (img == null)
                return null;
            var texture = new Texture2D(200, 200); //128 x 128 can be the download image size
            texture.LoadImage(img);
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite savedSprite = Sprite.Create(texture, rect, new Vector2(1, 1));
            MemoryCache.CacheImage(Path.GetFileNameWithoutExtension(imageNameR), savedSprite, false);
            return savedSprite;
        }


        public static byte[] LoadCachedImage(string imageName)
        {
            _imageFolderLocation = GetFilePath(_imageFolderLocation);
            byte[] dataByte = null;

            string imageFile = Path.Combine(_imageFolderLocation,imageName);

            if (!File.Exists(imageFile))
            {
                Debug.Log("File does not exist");
                return null;
            }

            try
            {
                dataByte = File.ReadAllBytes(imageFile);
                Debug.Log( imageFile+"Loaded Data from: " + _imageFolderLocation.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To Load Data from: " + _imageFolderLocation.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }

            return dataByte;
        }

        static string GetFilePath(string fileLocation)
        {
//Check the directory if available otherwise create new.
            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            return fileLocation;
        }

        public static void DeleteCachedImage(string imageName)
        {
            _imageFolderLocation = GetFilePath(_imageFolderLocation);
            string imageFile = Path.Combine(_imageFolderLocation,imageName);

            if (File.Exists(imageFile))
            {
                Debug.Log("Deleting File");
                File.Delete(imageFile);
            }
            else
            {
                Debug.Log("File not found");
            }
       
        }
    }
}