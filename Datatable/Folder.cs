using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CreateFileOrFolder
{
    public class Folder
    {
        public string folderName = @"C:\Database - Telemetria\";


        public void Create()
        {
            // Specify a name for your top-level folder.
             // lolo

            // Create the subfolder. You can verify in File Explorer that you have this
            // structure in the C: drive.
            //    Local Disk (C:)
            //        Top-Level Folder
            System.IO.Directory.CreateDirectory(folderName);
        }



        public string[] ProcessFiles(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            int i = 0, count = 0;

            foreach (string fileName in fileEntries)
            {
                if (fileEntries[i].IndexOf(".xml") != -1)
                {
                    count++;
                }
                i++;
            }
            
            string[] filexml = new string[count];

            for (i = 0; i == count - 1; i++)
            {
                filexml[i] = "lala";
            }

            i = 0;
            count = 0;

            foreach (string fileName in fileEntries)
            {
                if (fileEntries[i].IndexOf(".xml") != -1)
                {
                    filexml[count] = fileEntries[i].Substring(25).TrimEnd('.','x','m','l');
                    count++;
                }
                i++;
            }

            return filexml;
        }
    }
}
