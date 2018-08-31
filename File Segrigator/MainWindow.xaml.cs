using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace File_Segrigator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String RootFolder = "File Organiser";

        public MainWindow()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.Drop += new DragEventHandler(Form1_DragDrop);
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {

                try
                {
                    string sourceFolder = getFileDetails(file, "location");
                    string destinationPath = sourceFolder + "\\" + RootFolder;
                    string fileName = getFileDetails(file, "name");
                    string fileFormat = getFileDetails(file, "format");

                    setUpFolderToMoveIn(sourceFolder);

                    Thread fileTransfer = new Thread(() => moveFile(sourceFolder, destinationPath, fileName, fileFormat));
                    fileTransfer.Start();
                }
                catch (Exception z)
                {
                    addToHeader(z.ToString());
                }
            }
        }

        string getFileDetails(string file, string type)
        {
            string fileNameWithFormat = "";
            string fileFormat = "";
            string fileName = "";
            string fileLocation = "";

            // fileLocation
            for (int i = 0; i < file.Split('\\').Length - 1; i++)
            {
                fileLocation += (i == 0 ? "" : "\\") + file.Split('\\')[i];
            }

            // fileNameWithFormat
            fileNameWithFormat = file.Replace(fileLocation + "\\", "");

            // fileName
            for (int i = 0; i < fileNameWithFormat.Split('.').Length - 1; i++)
            {
                fileName += (i == 0 ? "" : ".") + fileNameWithFormat.Split('.')[i];
            }

            // fileFormat
            fileFormat = fileNameWithFormat.Split('.')[fileNameWithFormat.Split('.').Length - 1];

            return type == "name" ? fileName : type == "format" ? fileFormat : type == "location" ? fileLocation : file;
        }

        void moveFile(string sourceFolder, string destinationPath, string fileName, string fileFormat)
        {
            /*
             * NEED TO CHECK WHETHER THE FILE EXISTS OR NOT
             */
            if (fileFormat != "") 
            {
                string sourceFile = sourceFolder + "\\" + fileName + "." + fileFormat;
                string destinationFile = destinationPath + "\\" + fileFormat + "\\" + fileName + "." + fileFormat;

                // checking for FORMAT FOLDER
                if (!System.IO.Directory.Exists(destinationPath + "\\" + fileFormat))
                {
                    System.IO.Directory.CreateDirectory(destinationPath + "\\" + fileFormat);
                }

                // moving the file
                if (System.IO.File.Exists(destinationFile))
                {
                    Console.WriteLine("File alreday exists");
                }
                else
                {
                    System.IO.File.Move(sourceFile, destinationFile);
                }
            }
        }

        void setUpFolderToMoveIn(string sourceFolder)
        {
            string rootLocation = sourceFolder + "\\" + RootFolder;
            if (!System.IO.Directory.Exists(rootLocation))
            {
                System.IO.Directory.CreateDirectory(rootLocation);
            }
        }

        void addToHeader(string text)
        {
            header.Content += '\n' + text;
        }
    }
}
