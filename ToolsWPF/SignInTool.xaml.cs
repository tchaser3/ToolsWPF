/* Title:           Sign In Tool
 * Date:            1-19-18
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to sign in a tool */

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
using System.Windows.Shapes;
using NewToolsDLL;
using ToolHistoryDLL;
using NewEventLogDLL;

namespace ToolsWPF
{
    /// <summary>
    /// Interaction logic for SignInTool.xaml
    /// </summary>
    public partial class SignInTool : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();

        public SignInTool()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnSignIn.IsEnabled = false;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strToolID;
            int intRecordsReturned;

            strToolID = txtEnterToolID.Text;
            if(strToolID == "")
            {
                TheMessagesClass.ErrorMessage("The Tool ID Was Not Entered");
                return;
            }

            TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

            intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

            if(intRecordsReturned == 0)
            {
                TheMessagesClass.InformationMessage("Tool ID Was Not Found");
                return;
            }
            
            if(TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].Available == true)
            {
                TheMessagesClass.InformationMessage("Tool Is Already Signed In");
                return;
            }

            MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;
            txtDescription.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
            txtFirstName.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].FirstName;
            txtLastName.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].LastName;
            btnSignIn.IsEnabled = true;
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //setting local variable
            bool blnFatalError;

            try
            {
                blnFatalError = TheToolsClass.UpdateToolSignOut(MainWindow.gintToolKey, MainWindow.gintWarehouseID, true, "TOOL SIGNED IN");

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheToolHistoryClass.InsertToolHistory(MainWindow.gintToolKey, MainWindow.gintWarehouseID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "TOOL SIGNED IN");

                if (blnFatalError == true)
                    throw new Exception();

                txtDescription.Text = "";
                txtEnterToolID.Text = "";
                txtFirstName.Text = "";
                txtLastName.Text = "";
                btnSignIn.IsEnabled = false;
                TheMessagesClass.InformationMessage("The Tool Has Been Signed In");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Tools WPF // Sign In Tool // Sign In Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
