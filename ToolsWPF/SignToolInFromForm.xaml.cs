/* Title:           Sign Tool In From Form
 * Date:            1-17-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to sign in a tool from the sign out form */

using KeyWordDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using NewToolsDLL;
using System;
using System.Windows;
using System.Windows.Input;
using ToolHistoryDLL;

namespace ToolsWPF
{
    /// <summary>
    /// Interaction logic for SignToolInFromForm.xaml
    /// </summary>
    public partial class SignToolInFromForm : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
                
        public SignToolInFromForm()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(MainWindow.gstrToolID);

                MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;

                txtToolID.Text = MainWindow.gstrToolID;
                txtDescription.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                txtFirstName.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].FirstName;
                txtLastName.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].LastName;

                MainWindow.gblnToolSignedIn = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Tools WPF // Sign Tool In From Form // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheToolsClass.UpdateToolSignOut(MainWindow.gintToolKey, MainWindow.gintWarehouseID, true, "TOOL SIGNED IN");

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheToolHistoryClass.InsertToolHistory(MainWindow.gintToolKey, MainWindow.gintWarehouseID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "TOOL SIGNED IN");

                if (blnFatalError == true)
                    throw new Exception();

                MainWindow.gblnToolSignedIn = true;

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Tools WPF // Sign Tool In From Form // Sign In Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
