using System;
using System.IO;

namespace YandexFotkiIntegrator.LogicHelpers
{
    public static class StringHelper
    {
        public static string GetFileNameWithoutExtension(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            try
            {
                string result = Path.GetFileNameWithoutExtension(source);
                return result ?? string.Empty;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get file name without extension from path " + source + ". " + e.Message);
            }
        }
    }
}