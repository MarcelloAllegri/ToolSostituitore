using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<string> m_FileContentList;        
        private char m_InputFileDelimiter;
        private char m_OutputFileDelimiter;
        

        public List<FileInformation> fileInformation
        {
            get { return m_fileInformation; }
            set { m_fileInformation = value; }
        }
        public char InputFileDelimiter
        {
            set { m_InputFileDelimiter = value; }
        }
        public char OutputFileDelimiter
        {
            set { m_InputFileDelimiter = value; }
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
            bool r1 = CheckFields();           
            string OutputFilePath = SaveResult();
            if(r1 == true && !string.IsNullOrEmpty(OutputFilePath))            
                RunSubstitution(OutputFilePath);
        }
        private void RunSubstitution(string outputFilePath)
        {
            outputFilePath = Path.GetFullPath(outputFilePath);

            StreamReader file = new StreamReader(fileInformation[0].Path);
            if (file != null)
            {
                string line = file.ReadLine();
                if (!line.Contains(m_InputFileDelimiter))
                    MessageBox.Show("Input Delimiter not correct!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {                   
                    using (StreamWriter fileout = new System.IO.StreamWriter(outputFilePath))
                    {
                        string app = ChangeSeparator(line);
                        fileout.WriteLine(app);                        
                        while ((line = file.ReadLine()) != null)
                        {
                            app = ChangeSeparator(line);
                            fileout.WriteLine(app);                            
                        }
                        file.Close();
                        fileout.Close();
                    }                    
                }
            }
            
            MessageBox.Show("Sostituzione Completata!", "Avviso", MessageBoxButton.OK);
        }
        private string SaveResult()
        {
            bool retry = true;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Seleziona la cartella e immetti il nome del file in cui verrà scritto il risultato";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
            bool? dialogResult = saveFileDialog1.ShowDialog();
            while (retry)
            {
                if (dialogResult == true)                                    
                    return saveFileDialog1.FileName;                
                else if(dialogResult == false)                
                    return string.Empty;                
                else
                    retry = true;
                
            }
            return string.Empty;
        }
        private string ChangeSeparator(string item)
        {
            string replacement = item.Replace(m_InputFileDelimiter, m_OutputFileDelimiter);
            return replacement;
        }
        private bool CheckFields()
        {
            string itemsNotCompiled = string.Empty;

            if (m_fileInformation == null || string.IsNullOrEmpty(this.m_fileInformation[0].Path))
                itemsNotCompiled = string.Concat(itemsNotCompiled, "File not selected!");

            if(InputFileSeparatorChooseComboBox.SelectedItem == null)
                itemsNotCompiled = string.Concat(itemsNotCompiled, "\nInput file character separator not selected!");

            if (OutputFileSeparatorChooseComboBox.SelectedItem == null)
                itemsNotCompiled = string.Concat(itemsNotCompiled, "\nOutput file character separator not selected!");

            
            if (!string.IsNullOrEmpty(itemsNotCompiled))
            {
                MessageBox.Show("Errors:\n" + itemsNotCompiled);
                return false;
            }

            return true;
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

            OutputFileSeparatorChooseComboBox.ItemsSource = InputFileSeparatorChooseComboBox.ItemsSource = ListOfSeparator;
        }        
        private void InputFileSeparatorChooseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = InputFileSeparatorChooseComboBox.SelectedValue;

            if (selectedValue != null)
                switch (((KeyValuePair<string, char>)selectedValue).Key)
                {
                    case ".":
                    case ",":
                    case ";":
                    case "Tab":
                    case "Space": this.m_InputFileDelimiter = ListOfSeparator[((KeyValuePair<string, char>)selectedValue).Key]; break;
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
                    case true: m_InputFileDelimiter = text[0]; break;
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
                    case true: InputFileSeparatorChooseComboBox.SelectedItem = null; break;
                    case false: OutputFileSeparatorChooseComboBox.SelectedItem = null; break;
                }
            customCharacterUserControl.Visibility = Visibility.Collapsed;

        }
        public void ClearAll()
        {
            m_fileInformation = new List<FileInformation>();
            FileListView.ItemsSource = null;
            m_OutputFileDelimiter = m_InputFileDelimiter = '\0';
            InputFileSeparatorChooseComboBox.SelectedItem = OutputFileSeparatorChooseComboBox.SelectedItem = null;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }
    }
}
