using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YandexFotkiIntegrator.LogicHelpers
{
    public static class WorkerHelpers
    {
        public static string GetIntegrationTextFile(string rootPath)
        {
            try
            {
                string integratingFilePath = Directory.GetFiles(rootPath, "* - Integrated.txt").FirstOrDefault();

                if (string.IsNullOrEmpty(integratingFilePath) || !File.Exists(integratingFilePath))
                {
                    throw new Exception("Integrating files (* - Integrated.txt) were not found in the root directory.");
                }

                return integratingFilePath;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get integration text file. " + e.Message);
            }
        }

        public static List<string> GetYandexFotkiImageTags(string rootPath)
        {
            try
            {
                string fromYandexFilePath = Path.Combine(rootPath, "FromYandex.txt");
                if (!File.Exists(fromYandexFilePath))
                {
                    throw new Exception("File \"FromYandex.txt\" not found");
                }

                Console.WriteLine();
                Console.WriteLine("File \"FromYandex.txt\" found. Continue");
                string yandexFotkiCode = File.ReadAllText(fromYandexFilePath);
                if (string.IsNullOrEmpty(yandexFotkiCode))
                {
                    throw new Exception("File \"FromYandex.txt\" is empty");
                }

                Console.WriteLine("File \"FromYandex.txt\" opened. Reading <img> tags");
                List<string> imgTags = new List<string>();
                int oldCurrentImgTagPosition = -1;
                while (true)
                {
                    int currentImgTagPosition = yandexFotkiCode.IndexOf("<img src=", oldCurrentImgTagPosition + 1, StringComparison.InvariantCultureIgnoreCase);
                    if (currentImgTagPosition == -1 || currentImgTagPosition == oldCurrentImgTagPosition)
                    {
                        break;
                    }

                    oldCurrentImgTagPosition = currentImgTagPosition;

                    int tagClosingPosition = yandexFotkiCode.IndexOf("border=\"0\"/>", currentImgTagPosition + 1, StringComparison.InvariantCultureIgnoreCase);
                    if (tagClosingPosition == -1)
                    {
                        break;
                    }

                    string currentTag = yandexFotkiCode.Substring(currentImgTagPosition, tagClosingPosition - currentImgTagPosition + 12);
                    if (string.IsNullOrEmpty(currentTag))
                    {
                        break;
                    }

                    imgTags.Add(currentTag);
                }

                if (imgTags.Count <= 0)
                {
                    throw new Exception("No <img> tags found in \"FromYandex.txt\" file");
                }

                Console.WriteLine(imgTags.Count + " <img> tags found. Continue");
                return imgTags;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get image tags from yandex images file. " + e.Message);
            }
        }

        public static List<string> GetImages(string rootPath, string integrationFilePath)
        {
            try
            {
                string integratingFileName = Path.GetFileName(integrationFilePath);
                if (string.IsNullOrEmpty(integratingFileName))
                {
                    throw new Exception("Integration file name is empty.");
                }

                Console.WriteLine();
                Console.WriteLine("Use integrating file \"" + integratingFileName + "\"");
                string imagesFolderMask = integratingFileName.Replace(" - Integrated", string.Empty);
                if (string.IsNullOrEmpty(imagesFolderMask))
                {
                    throw new Exception("Imaging folder mask ( - Integrated) was not found in .TXT files.");
                }

                Console.WriteLine("Use image folder mask \"" + imagesFolderMask + "\"");
                string imagesDirectoryPath = Directory.GetDirectories(rootPath, "*[" + imagesFolderMask + "]*").FirstOrDefault();
                if (string.IsNullOrEmpty(imagesDirectoryPath) || !Directory.Exists(imagesDirectoryPath))
                {
                    throw new Exception("Imaging folder with mask \"" + imagesFolderMask + "\" not found");
                }

                Console.WriteLine("Use images folder \"" + Path.GetFileName(imagesDirectoryPath) + "\"");
                string[] photos = Directory.GetFiles(imagesDirectoryPath, "*.jpg");
                if (!photos.Any())
                {
                    throw new Exception("No .JPG files found in '" + Path.GetFileName(imagesDirectoryPath) + "'");
                }

                return photos.ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get photos from photos folder. " + e.Message);
            }
        }
    }
}