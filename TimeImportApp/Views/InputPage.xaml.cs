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
using TimeImportApp.Domain;

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
                string sessionTicket = apiHelper.GetSessionTicket(accountCodeTextBox.Text, usernameTextBox.Text, passwordTextBox.Password);
                if (sessionTicket == null)
                {
                    errorMessageLabel.Text = "Authentication Unsuccessful: Please check login details.";
                }
                else
                {
                    errorMessageLabel.Text = String.Empty;
                    //Process the CSV File
                    NavigationService.Navigate(new ProcessingPage());
                    var people=csvHelper.ParseReportToObjects(filePath);
                    bool timecardsAddedSuccessfuly = apiHelper.AddTimeCards(people, sessionTicket);

                    if (timecardsAddedSuccessfuly)
                    {
                        NavigationService.Navigate(new CompletedPage(BatchState.CompletedSuccesfully));
                    }
                    else
                    {
                        var errors = apiHelper.GetErrors();
                        foreach(var error in errors)
                        {
                            //Type 1
                            //Scenario: Job not in MIPAC - Comment field incorrect format 
                            //Action: Reject perosons's timecard and ask for manual entry at end

                            //Type 2
                            //Scenario: Job not in MIPAC - Comment field correct format - Job doesn't exist in Projector
                            //Action: Reject perosons's timecard and ask for manual entry at end


                            //Type 3
                            //Scenario: Job in MIPAC - Job in lookup table - Projector code no longer working
                            //Action: Reject perosons's timecard and ask for manual entry at end

                            //Type 4:
                            //Scenario: Job in MIPAC - Job not in lookup table 
                            //Action: Verify from user if job is in projector
                                //YES: Show mipac code+name and ask user to enter projector code+name. Save project to db. Proceed with adding timecard. 
                                //NO: Reject perosons's timecard and ask for manual entry at end
                        }
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProjectPage());
        }
    }
}
