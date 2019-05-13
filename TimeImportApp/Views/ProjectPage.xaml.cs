using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page
    {
        public ObservableCollection<ParsedSharedProject> SharedProjects { get; set; } 
        public ProjectPage()
        {
            InitializeComponent();
            DataTable dataTable = new DataTable();
            using (var context = new ProjectorIntegrationContext())
            {
                //SharedProjects=context.SharedProjects.AsEnumerable().ToList();
                //dataGrid.ItemsSource = SharedProjects;
            }
            //dataGrid.d
            DataContext = this.SharedProjects;
        }

        private void BackButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (projectorProjectDataGrid.Visibility == Visibility.Visible)
            {
                NavigationService.GoBack();
            }
            else
            {
                ShowProjectTable();
            }
        }

        private void ShowProjectTable()
        {
            projectorProjectDataGrid.Visibility = Visibility.Visible;
            addProjectButton.Visibility = Visibility.Visible;
            projectTextblock.Visibility = Visibility.Visible;
            addProjectTextblock.Visibility = Visibility.Collapsed;
            addProjectGrid.Visibility = Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateProjectList();
        }

        private void GenerateProjectList()
        {
            using (var context = new ProjectorIntegrationContext())
            {
                SharedProjects = new ObservableCollection<ParsedSharedProject>();
                var projectList = context.SharedProjects.AsEnumerable().ToList();
                foreach (var project in projectList)
                {
                    SharedProjects.Add(new ParsedSharedProject
                    {
                        SharedProjectID = project.SharedProjectID,
                        MIPACProjectCode = project.MIPACProject.Code,
                        MIPACProjectName = project.MIPACProject.Name,
                        ProjectorProjectCode = project.ProjectorProject.Code,
                        ProjectorProjectName = project.ProjectorProject.Name
                    });
                }
                //dataGrid.ItemsSource = SharedProjects;
                // dataGrid.Items.Refresh();
                projectorProjectDataGrid.ItemsSource = SharedProjects;
                DataContext = this.SharedProjects;
            }
        }

        public class ParsedSharedProject
        {
            public int SharedProjectID { get; set; }
            public string ProjectorProjectName { get; set; }
            public string ProjectorProjectCode { get; set; }
            public string MIPACProjectName { get; set; }
            public string MIPACProjectCode { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HideProjectTable();
        }

        private void HideProjectTable()
        {
            projectorProjectDataGrid.Visibility = Visibility.Collapsed;
            addProjectButton.Visibility = Visibility.Collapsed;
            projectTextblock.Visibility = Visibility.Collapsed;
            addProjectTextblock.Visibility = Visibility.Visible;
            addProjectGrid.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(mipacNameBox.Text))
            {
                errorMessageLabel.Text = "Please enter a valid MiPAC Project Name";
            }
            else if (String.IsNullOrWhiteSpace(mipacCodeTextBox.Text))
            {
                errorMessageLabel.Text = "Please enter a valid MiPAC Project Code";
                
            }
            else if (String.IsNullOrWhiteSpace(projectorNameBox.Text))
            {
                errorMessageLabel.Text = "Please enter a valid Projector Project Name";
            }
            else if (String.IsNullOrWhiteSpace(projectorCodeTextBox.Text))
            {
                errorMessageLabel.Text = "Please enter a valid Projector Project Code";
            }
            else
            {
                using (var context = new ProjectorIntegrationContext())
                {
                    var mipacProject = new MIPACProject { Code = mipacCodeTextBox.Text, Name = mipacNameBox.Text };
                    var projectorProject = new ProjectorProject { Code = projectorCodeTextBox.Text, Name = projectorNameBox.Text };
                    context.SharedProjects.Add(new SharedProject { MIPACProject = mipacProject, ProjectorProject = projectorProject });
                    context.SaveChanges();
                }
                GenerateProjectList();
                ShowProjectTable();
            }
        }
    }
}
