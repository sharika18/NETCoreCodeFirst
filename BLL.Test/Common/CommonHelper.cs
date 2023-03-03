﻿using Newtonsoft.Json;
using System;
using System.IO;

namespace BLL.Test.Common
{
    public class CommonHelper
    {
        public static T LoadDataFromFile<T>(string folderFilePath)
        {
            Console.WriteLine(folderFilePath);
            string currentDirectory = Environment.CurrentDirectory;
            string path = Path.Combine(currentDirectory, folderFilePath);
            T result = default;
            using (var reader = new StreamReader(path))
            {
                var data = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(data);
            }
            return result;
        }
    }
}
