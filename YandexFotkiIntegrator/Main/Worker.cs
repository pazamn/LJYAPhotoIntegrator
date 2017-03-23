using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YandexFotkiIntegrator.LogicHelpers;

namespace YandexFotkiIntegrator.Main
{
    public static class Worker
    {
        public static void CreatePreviewHtmlFile(string rootPath, string integrationFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(integrationFilePath) || !File.Exists(integrationFilePath))
                {
                    throw new Exception("Integration text file not found.");
                }

                Console.WriteLine();
                string htmlFilePath = integrationFilePath.Replace(".txt", ".html");
                if (File.Exists(htmlFilePath))
                {
                    File.Delete(htmlFilePath);
                    Console.WriteLine("Old .HTML file deleted.");
                }

                List<string> textContent = File.ReadAllLines(integrationFilePath, Encoding.GetEncoding("Windows-1251")).ToList();
                List<string> photos = WorkerHelpers.GetImages(rootPath, integrationFilePath);

                StringBuilder page = new StringBuilder();
                page.AppendLine("<!DOCTYPE html>");
                page.AppendLine("<html>");
                page.AppendLine("<head>");
                page.AppendLine("</head>");
                page.AppendLine("<body>");

                foreach (string line in textContent)
                {
                    if (!Regex.IsMatch(line, "\\([\\w\\d]+\\)"))
                    {
                        page.AppendLine(line + "<br>");
                        continue;
                    }

                    string imageName = line.Trim('(', ')');
                    string photoPath = photos.FirstOrDefault(p => StringHelper.GetFileNameWithoutExtension(p).Equals(imageName, StringComparison.OrdinalIgnoreCase));
                    if (string.IsNullOrEmpty(photoPath))
                    {
                        page.AppendLine(line + "<br>");
                        continue;
                    }

                    string relativePath = photoPath.Replace("/", "\\").Replace(rootPath.Replace("/", "\\"), string.Empty).Trim('\\');
                    string imageLine = "<img src=\"" + relativePath + "\" title=\"\" alt=\"" + Constants.NoPhotoMessage + "\" border=\"0\"/>";

                    page.AppendLine(imageLine + "<br>");
                }

                page.AppendLine("</body>");
                page.AppendLine("</html>");

                File.WriteAllText(htmlFilePath, page.ToString(), Encoding.UTF8);
                Console.WriteLine(".HTML file saved by path " + htmlFilePath);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create preview .HTML file. " + e.Message);
            }
        }

        public static void IntergatePhotosInTextFile(string rootPath, string integrationFilePath, List<string> imgTags)
        {
            try
            {
                string integratingFileName = Path.GetFileName(integrationFilePath);
                if (string.IsNullOrEmpty(integratingFileName))
                {
                    throw new Exception("Integration file name is empty.");
                }

                List<string> photos = WorkerHelpers.GetImages(rootPath, integrationFilePath);
                Console.WriteLine(photos.Count() + " .JPG files found in imaging folder.");
                if (imgTags.Count != photos.Count())
                {
                    throw new Exception("Failed. Counts are different");
                }

                List<string> photoFileNames = photos.Select(f => "(" + Path.GetFileNameWithoutExtension(f) + ")").ToList();
                string mainReportText = File.ReadAllText(integrationFilePath, Encoding.GetEncoding("WINDOWS-1251"));
                Console.WriteLine("Integrating file \"" + integratingFileName + "\" read");

                foreach (string photoFileName in photoFileNames)
                {
                    int inArrayPosition = photoFileNames.IndexOf(photoFileName);
                    string currentImgTag = imgTags[inArrayPosition];

                    mainReportText = mainReportText.Replace(photoFileName, currentImgTag);
                }

                Console.WriteLine(photoFileNames.Count + " entries changed in \"" + integratingFileName + "\" file");
                File.WriteAllText(integrationFilePath, mainReportText, Encoding.GetEncoding("WINDOWS-1251"));
                Console.WriteLine(integratingFileName + "\" file saved");
            }
            catch (Exception e)
            {
                throw new Exception("Failed to integrate photos in text file. " + e.Message);
            }
        }
    }
}