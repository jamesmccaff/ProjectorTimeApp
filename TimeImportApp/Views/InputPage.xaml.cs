using Microsoft.Win32;
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

namespace TimeImportApp.Views
{
    /// <summary>
    /// Interaction logic for InputPage.xaml
    /// </summary>
    public partial class InputPage : Page
    {
        private ICSVHelper csvHelper;
        private IAPIHelper apiHelper;
        private IDatabaseHelper databaseHelper;
        private string filePath;
        public InputPage()
        {
            InitializeComponent();
            csvHelper = new CSVHelper();
            apiHelper = new APIHelper();
            databaseHelper = new DatabaseHelper();
            databaseHelper.CreateNewDatabaseIfNoneExists();
        }


        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "csv";
            openFileDialog.Filter = "Comma Separated Files (*.csv)|*.csv";
            openFileDialog.ShowDialog();
            filePath = openFileDialog.FileName;
            filePathTextBox.Text = System.IO.Path.GetFileName(filePath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(filePathTextBox.Text))
            {
                errorMessageLabel.Text = "Please select a valid file";
            }
            else if (String.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                errorMessageLabel.Text = "Please enter a valid username";
            }
            else if (String.IsNullOrWhiteSpace(passwordTextBox.Password))
            {
                errorMessageLabel.Text = "Please enter a valid password";
            }
            else if (String.IsNullOrWhiteSpace(accountCodeTextBox.Text))
            {
                errorMessageLabel.Text = "Please enter a valid account code";
            }
            else
            {
                //Authenticate then start processing 
                //if (!apiHelper.AuthenticateUser(accountCodeTextBox.Text, usernameTextBox.Text, passwordTextBox.Password))
                //{
                //    errorMessageLabel.Text = "Authentication Unsuccessful: Please check login details.";
                //}

                if (apiHelper.GetSessionTicket(accountCodeTextBox.Text, usernameTextBox.Text, passwordTextBox.Password) == null)
                {
                    errorMessageLabel.Text = "Authentication Unsuccessful: Please check login details.";
                }
                else
                {
                    errorMessageLabel.Text = String.Empty;
                    //Process the CSV File
                    NavigationService.Navigate(new ProcessingPage());
                    var people=csvHelper.ParseReportToObjects(filePath);
                }
            }
        }
    }
}
