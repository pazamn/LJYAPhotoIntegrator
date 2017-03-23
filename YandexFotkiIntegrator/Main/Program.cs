using System;
using System.Collections.Generic;
using System.Reflection;
using YandexFotkiIntegrator.LogicHelpers;

namespace YandexFotkiIntegrator.Main
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Assembly.GetExecutingAssembly().FullName);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Start integration");

                string rootDirectoryPath = Environment.CurrentDirectory;
                Console.WriteLine("Root folder: " + rootDirectoryPath);

                string integrationFilePath = WorkerHelpers.GetIntegrationTextFile(rootDirectoryPath);
                Worker.CreatePreviewHtmlFile(rootDirectoryPath, integrationFilePath);

                List<string> imageTags = WorkerHelpers.GetYandexFotkiImageTags(rootDirectoryPath);
                Worker.IntergatePhotosInTextFile(rootDirectoryPath, integrationFilePath, imageTags);

                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}