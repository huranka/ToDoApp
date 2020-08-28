using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace ToDoManagement
{
    public class FileControl
    {
        static public readonly string topFolderPath_ = @".\saveData";
        static public (bool result,string resultString) SaveFile(string data, string filename)
        {
            try
            {
                var path = Path.Combine(topFolderPath_, filename);
                Directory.CreateDirectory(topFolderPath_);

                File.WriteAllText(path, data);

            } catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
                return (false, exception.Message);
            }

            return (true, "");
        }

        static public (bool result, string resultString) LoadFile(ref string data, string path)
        {
            try
            {
                data = File.ReadAllText(path);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return (false, exception.Message);
            }

            return (true, "");
        }

    }
}
