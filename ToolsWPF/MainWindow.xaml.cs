/* Title:           Tools WFP - Main Window
 * Date:            1-15-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the main window for the Tools Applications */

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
using NewEventLogDLL;
using NewEmployeeDLL;
using DataValidationDLL;
using KeyWordDLL;

namespace ToolsWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();

        //setting up the data set
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();
        public static FindPartsWarehousesDataSet TheFindPartsWarehouseDataSet = new FindPartsWarehousesDataSet();
        public static FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();

        //setting up global variables
        int gintNoOfMisses;
        public static int gintToolKey;
        public static string gstrToolID;
        public static int gintEmployeeID;
        public static bool gblnToolSignedIn;
        public static int gintWarehouseID;
        public static string gstrHomeOffice;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                gintNoOfMisses = 0;

                pbxPassword.Focus();

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Tools WPF // Main  Window // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LogonFailed()
        {
            gintNoOfMisses++;

            if (gintNoOfMisses == 3)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There Have Been Three Attemps to Sign Into Tools WPF");

                TheMessagesClass.ErrorMessage("You Have Tried To Sign In Three Times\nThe Program Will Now Close");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("You Have Failed The Sign In Process");
                return;
            }
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intEmployeeID = 0;
            string strLastName;
            bool blnFatalError = false;
            int intRecordsReturned;
            string strErrorMessage = "";
            bool blnLogonPassed = true;

            TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();

            //beginning data validation
            strValueForValidation = pbxPassword.Password;
            strLastName = txtLastName.Text;
            blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
            if (blnFatalError == true)
            {
                strErrorMessage = "The Employee ID is not an Integer\n";
            }
            else
            {
                intEmployeeID = Convert.ToInt32(strValueForValidation);
            }
            if (strLastName == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Last Name Was Not Entered\n";
            }
            if (blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }

            //filling the data set
            TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(intEmployeeID, strLastName);

            intRecordsReturned = TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

            if (intRecordsReturned == 0)
            {
                LogonFailed();
                blnLogonPassed = false;
            }
            else
            {
                if (TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup != "ADMIN")
                {
                    if (TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup != "MANAGERS")
                    {
                        if (TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup != "WAREHOUSE")
                        {
                            LogonFailed();
                            blnLogonPassed = false;
                        }
                    }
                }
            }

            if (blnLogonPassed == true)
            {
                SetHomeOffice();

                MainMenu MainMenu = new MainMenu();
                MainMenu.Show();
                Hide();
            }
        }
        private void SetHomeOffice()
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnKeyWordNotFound;

            try
            {
                gstrHomeOffice = TheVerifyLogonDataSet.VerifyLogon[0].HomeOffice;

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnKeyWordNotFound = TheKeyWordClass.FindKeyWord(gstrHomeOffice, TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);

                    if(blnKeyWordNotFound == false)
                    {
                        gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Tools WPF // Main Window // Set Home Office " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
    
}
