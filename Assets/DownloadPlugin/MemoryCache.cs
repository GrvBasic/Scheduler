using System.Collections.Generic;
using UnityEngine;

namespace DownloadPlugin
{
    public class MemoryCache
    {
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        private static MemoryCache _instance;

        /// <summary>
        /// searches if image at given identifier is cached. Recommended to use full path if image is stored in local storage.
        /// </summary>
        /// <param name="imageIdentifier">identifier of image. identifier is unique</param>
        /// <returns>returns image if found in cache else returns null</returns>
        public static Sprite GetCachedImage(string imageIdentifier)
        {
            CheckInstance();
            if (_instance.spriteCache.ContainsKey(imageIdentifier))
            {
                return _instance.spriteCache[imageIdentifier];
            }

         return  ReadWriteImages.ReadImage(imageIdentifier);
           
        }

        /// <summary> 
        /// saves image at given identifier. Recommended to use full path if image is stored in local storage.
        /// </summary>
        /// <param name="imageIdentifier">saves image at given identifier. must be unique. will not save if identifier is not unique.</param>
        /// <param name="image">image to save at cache</param>
        /// <returns>was image save successful</returns>
        public static bool CacheImage(string imageIdentifier, Sprite image, bool overrideImage = false)
        {
            CheckInstance();
            if (_instance.spriteCache.ContainsKey(imageIdentifier))
            {
                if (overrideImage)
                {
                    //if override is allowed, override image
                    _instance.spriteCache[imageIdentifier] = image;
                    return true;
                }

                //given identifier already exists so don't save
                return false;
            }

            _instance.spriteCache.Add(imageIdentifier, image); //save image
            SaveImageToDisk(imageIdentifier);
            return true;
        }

        /// <summary>
        /// delete image from memory cache
        /// </summary>
        /// <param name="imageIdentifier">identifier to delete</param>
        /// <returns>was image successfully deleted, returns false if image not found</returns>
        public static bool DeleteImage(string imageIdentifier)
        {
            CheckInstance();
            if (_instance.spriteCache.ContainsKey(imageIdentifier))
            {
                //if image exists, delete it
                _instance.spriteCache.Remove(imageIdentifier);
                RemoveImageFromDisk(imageIdentifier);
                return true;
            }

            //image doesn't exists
            return false;
        }

        public static void CheckInstance()
        {
            if (_instance == null)
            {
                _instance = new MemoryCache();
            }
        }

        static void SaveImageToDisk(string imageNameR)
        {
            ReadWriteImages.WriteCachedImageInFile(_instance.spriteCache[imageNameR].texture, imageNameR);
        }

        static void RemoveImageFromDisk(string imageNameR)
        {
            ReadWriteImages.DeleteCachedImage(imageNameR);
        }
    }
}