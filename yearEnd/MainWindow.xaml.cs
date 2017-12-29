using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace yearEnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Accession Documents
    /// \\cfileserver\Documents\ConsultsConsults - by hand
    /// \\cfileserver\Documents\MolecularTesting - by hand
    /// \\cfileserver\Documents\Summary - by hand
    /// \\cfileserver\Documents\Reports\Lab\LabOrdersLog Months
    /// \\cfileserver\Documents\Reports\Surgical\MasterLog Months
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Visibility m_MessageVisibility;

        public MainWindow()
        {
            DataContext = this;
            this.MessageVisibility = Visibility.Collapsed;
            InitializeComponent();
        }
        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public Visibility MessageVisibility
        {
            get { return this.m_MessageVisibility; }
            set
            {
                this.m_MessageVisibility = value;
                NotifyPropertyChanged("MessageVisibility");
            }
        }
            
        private void HyperLink_AddAccessionDocumentFolders_Click(object sender, RoutedEventArgs e)
        {
            this.textBlockMessage.Visibility = Visibility.Collapsed;
            string year = GetYear();
            if (string.IsNullOrEmpty(year) == false)
            {
                string basePath = @"\\cfileserver\AccessionDocuments";
                string path = System.IO.Path.Combine(basePath, year);
                if (Directory.Exists(path) == false)
                {
                    //Console.WriteLine(path);
                    Directory.CreateDirectory(path);
                }

                string twoDigitYear = year.Substring(2, 2);
                int dirNum = GetDirectoryCount();
                if (dirNum > 0)
                {
                    string startDir = GetDirStart();
                    if(string.IsNullOrEmpty(startDir) == false)
                    MakeTousandsDirectories(path, dirNum, twoDigitYear, startDir);
                }
            }
            this.textBlockMessage.Visibility = Visibility.Visible;
        }

        private void HyperLink_AddCalendarFolders_Click(object sender, RoutedEventArgs e)
        {
            this.textBlockMessage.Visibility = Visibility.Collapsed;
            string year = GetYear();
            if (string.IsNullOrEmpty(year) == false)
            {
                string basePath = @"\\cfileserver\Documents\Reports\Lab\LabOrdersLog";
                string path = System.IO.Path.Combine(basePath, year);
                this.MakeCalendarFolders(path);

                basePath = @"\\cfileserver\Documents\Reports\Surgical\MasterLog";
                path = System.IO.Path.Combine(basePath, year);
                this.MakeCalendarFolders(path);
            }
            this.textBlockMessage.Visibility = Visibility.Visible;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowFolderBrowser_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = @"\\cfileserver\AccessionDocuments";

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBoxPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private string GetYear()
        {
            string result = textBoxYear.Text;
            bool isOK = true;

            if (string.IsNullOrEmpty(result) == true) isOK = false;

            if (isOK == true)
            {
                if (result.Length != 4) isOK = false;
            }

            if (isOK == true)
            {
                foreach (char c in result)
                {
                    if (Char.IsDigit(c) == false)
                    {
                        isOK = false;
                        break;
                    }
                }
            }

            if (isOK == false)
            {
                System.Windows.MessageBox.Show("Must be 4 digit year.");
                result = string.Empty;
            }

            return result;
        }

        private int GetDirectoryCount()
        {
            string cnt = textBoxDirNumber.Text;
            int result = Convert.ToInt32(cnt);
            bool isOK = true;

            if (result <= 0 || result > 50) isOK = false;

            if (isOK == false) System.Windows.MessageBox.Show("Directory count Must be a digit between 1 and 50.");

            return result;
        }

        private string GetDirStart()
        {
            string result = textBoxDirStart.Text;
            if (result != "0")
            {
                int num = Convert.ToInt32(result);

                if (num == 0)
                {
                    System.Windows.MessageBox.Show("Directory start Must be a digit.");
                    result = null;
                }
            }
            return result;
        }

        private void MakeTousandsDirectories(string path, int count, string twoDigitYear, string startDir)
        {
            int startDirNum = Convert.ToInt32(startDir);
            int dirNum = startDirNum + count;
            for(int idx = startDirNum; idx < dirNum; idx++)
            {
                int start = idx * 1000;
                int end = start + 999;
                if(start == 0) start = 1;

                string dirName = start.ToString("D5") + "-" + end.ToString("D5");
                string dirPath = System.IO.Path.Combine(path, dirName);
                if(Directory.Exists(dirPath) == false)
                {
                    //Console.WriteLine(dirPath);
                    Directory.CreateDirectory(dirPath);
                    MakeIndividualDirectories(dirPath, twoDigitYear, idx * 1000);
                }
            }
        }

        private void MakeIndividualDirectories(string path, string twoDigitYear, int baseNum)
        {
            for(int idx = 0; idx < 1000; idx++)
            {
                if (baseNum == 0 && idx == 0) continue;

                int dirNum = baseNum + idx;
                string dirName = twoDigitYear + "-" + dirNum.ToString();
                string dirPath = System.IO.Path.Combine(path, dirName);
                if (Directory.Exists(dirPath) == false)
                {
                    //Console.WriteLine(dirPath);
                    Directory.CreateDirectory(dirPath);
                }
            }
        }

        private void MakeCalendarFolders(string path)
        {
            if (Directory.Exists(path) == false)
            {
                //Console.WriteLine(path);
                Directory.CreateDirectory(path);
            }

            DateTime date = new DateTime(2017, 1, 1);
            for (int idx = 0; idx < 12; idx++)
            {
                string month = date.ToString("MMMM");
                string monthPath = Path.Combine(path, month);
                if (Directory.Exists(monthPath) == false)
                {
                    //Console.WriteLine(monthPath);
                    Directory.CreateDirectory(monthPath);
                }
                    date = date.AddMonths(1);
            }
        }
    }
}
