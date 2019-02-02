using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using ToolSostituitore.Resources.Classes;

namespace ToolSostituitore
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<FileInformation> m_fileInformation;
        Dictionary<string, char> ListOfSeparator;
        private char m_ImputFileDelimiter;
        private char m_OutputFileDelimiter;
        private int m_ItemPorRow = -1;

        public List<FileInformation> fileInformation
        {
            get { return m_fileInformation; }
            set { m_fileInformation = value; }
        }

        public char InputFileDelimiter
        {
            set { m_ImputFileDelimiter = value; }
        }

        public char OutputFileDelimiter
        {
            set { m_ImputFileDelimiter = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddFileToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            SelectFile();
        }

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Csv file (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


            if (openFileDialog.ShowDialog() == true)
            {
                fileInformation = new List<FileInformation>();
                fileInformation.Add(new FileInformation(Path.GetFileName(openFileDialog.FileName), Path.GetFullPath(openFileDialog.FileName)));
                this.FileListView.ItemsSource = fileInformation;
            }


        }

        private void Start_Substitution_Click(object sender, RoutedEventArgs e)
        {
            CheckField();

        }

        private void CheckField()
        {
            string itemsNotCompiled = string.Empty;

            if (m_fileInformation == null || string.IsNullOrEmpty(this.m_fileInformation[0].Path))
                itemsNotCompiled = string.Concat(itemsNotCompiled, "File not selected!");

            if(ImputFileSeparatorChooseComboBox.SelectedItem == null)
                itemsNotCompiled = string.Concat(itemsNotCompiled, "\nInput file character separator not selected!");

            if (OutputFileSeparatorChooseComboBox.SelectedItem == null)
                itemsNotCompiled = string.Concat(itemsNotCompiled, "\nOutput file character separator not selected!");

            if(m_ItemPorRow == -1)
                itemsNotCompiled = string.Concat(itemsNotCompiled, "\nNumber of item not specified!");

            if (!string.IsNullOrEmpty(itemsNotCompiled))
                MessageBox.Show("Errors:\n" + itemsNotCompiled);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ListOfSeparator = new Dictionary<string, char>();

            ListOfSeparator.Add(".", '.');
            ListOfSeparator.Add(",", ',');
            ListOfSeparator.Add(";", ';');
            ListOfSeparator.Add("Tab", Convert.ToChar(11));
            ListOfSeparator.Add("Space", ' ');
            ListOfSeparator.Add("Other", 'o');

            OutputFileSeparatorChooseComboBox.ItemsSource = ImputFileSeparatorChooseComboBox.ItemsSource = ListOfSeparator;
        }

        private IEnumerable<string> ImportFile()
        {
            try
            {
                return File.ReadAllLines(fileInformation[0].Path);
            }
            catch (Exception)
            {
                MessageBox.Show("Errore durante l'importazione del file");
            }

            return null;
        }

        private void ImputFileSeparatorChooseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = ImputFileSeparatorChooseComboBox.SelectedValue;

            if (selectedValue != null)
                switch (((KeyValuePair<string, char>)selectedValue).Key)
                {
                    case ".":
                    case ",":
                    case ";":
                    case "Tab":
                    case "Space": this.m_ImputFileDelimiter = ListOfSeparator[((KeyValuePair<string, char>)selectedValue).Key]; break;
                    case "Other": InsertCustomDelimiter(true); break;
                    default:
                        break;
                }
        }

        private void InsertCustomDelimiter(bool isInputDelimiter)
        {
            customCharacterUserControl.Visibility = Visibility.Visible;
            customCharacterUserControl.IsSelected = true;
            insertDelimitatorUserControl.IsInputDelimiter = isInputDelimiter;
            MainTabItem.Visibility = Visibility.Collapsed;
        }

        private void OutputFileSeparatorChooseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = OutputFileSeparatorChooseComboBox.SelectedValue;

            if (selectedValue != null)
                switch (((KeyValuePair<string, char>)selectedValue).Key)
                {
                    case ".":
                    case ",":
                    case ";":
                    case "Tab":
                    case "Space": this.m_OutputFileDelimiter = ListOfSeparator[((KeyValuePair<string, char>)selectedValue).Key]; break;
                    case "Other": InsertCustomDelimiter(false); break;
                    default:
                        break;
                }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string text = insertDelimitatorUserControl.Save();


            if (text.Count() == 1)
            {
                switch (insertDelimitatorUserControl.IsInputDelimiter)
                {
                    case true: m_ImputFileDelimiter = text[0]; break;
                    case false: m_OutputFileDelimiter = text[0]; break;
                }

                MessageBox.Show("Saved!");
            }
            else
            {
                if (text.Count() == 0)
                    MessageBox.Show("Non è stato inserito alcun carattere!");
                else
                    MessageBox.Show("Hai inserito più di 1 carattere!");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabItem.Visibility = Visibility.Visible;
            MainTabItem.IsSelected = true;
            string text = insertDelimitatorUserControl.Save();
            if (!(text.Count() == 1))
                switch (insertDelimitatorUserControl.IsInputDelimiter)
                {
                    case true: ImputFileSeparatorChooseComboBox.SelectedItem = null; break;
                    case false: OutputFileSeparatorChooseComboBox.SelectedItem = null; break;
                }
            customCharacterUserControl.Visibility = Visibility.Collapsed;

        }

        private void ColumnsCounterNumericUpAndDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.m_ItemPorRow = (int)this.ColumnsCounterNumericUpAndDown.Value;
        }
    }
}
