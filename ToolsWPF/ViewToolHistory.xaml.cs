/* Title:           View Tool History
 * Date:            1-22-18
 * Author:          Terrance Holmes
 * 
 * Description:     This form will allow the user to export Tool history */

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
using NewEmployeeDLL;
using NewEventLogDLL;
using NewToolsDLL;
using ToolHistoryDLL;
using DataValidationDLL;
using DateSearchDLL;
using Microsoft.Win32;

namespace ToolsWPF
{
    /// <summary>
    /// Interaction logic for ViewToolHistory.xaml
    /// </summary>
    public partial class ViewToolHistory : Window
    {
        //setting the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ToolsClass TheToolsClass = new ToolsClass();
        ToolHistoryClass TheToolHistoryClass = new ToolHistoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDataSearchClass = new DateSearchClass();

        //setting up the data
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindToolHistoryByDateRangeDataSet TheFindToolHistoryByDateRangeDataSet = new FindToolHistoryByDateRangeDataSet();
        FindToolHistoryByEmployeeIDDataSet TheFindToolHistoryByEmployeeIDDataSet = new FindToolHistoryByEmployeeIDDataSet();
        FindToolHistoryByToolKeyDataSet TheFindToolHistoryByToolKeyDataSet = new FindToolHistoryByToolKeyDataSet();
        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        ViewToolHistoryDataSet TheViewToolHistoryDataSet = new ViewToolHistoryDataSet();
        FindEmployeeByLastNameDataSet TheFindEmployeeByLastNameDataSet = new FindEmployeeByLastNameDataSet();

        //setting up the variables
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        string gstrToolHistoryType;

        public ViewToolHistory()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgrResults.ItemsSource = TheViewToolHistoryDataSet.toolhistory;

            cboSelectToolHistory.Items.Add("Select Tool History");
            cboSelectToolHistory.Items.Add("Date Range Search");
            cboSelectToolHistory.Items.Add("Employee Search");
            cboSelectToolHistory.Items.Add("Tool ID Search");
            cboSelectToolHistory.SelectedIndex = 0;
            HideAllControls();
        }
        private void HideAllControls()
        {
            txtEnterLastName.Visibility = Visibility.Hidden;
            cboSelectEmployee.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;
            lblEnterLastName.Visibility = Visibility.Hidden;
            btnFind.IsEnabled = false;
            txtEndDate.Text = "";
            txtStartDate.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
        }
        private void ShowToolIDControls()
        {
            txtEnterLastName.Visibility = Visibility.Visible;
            lblEnterLastName.Visibility = Visibility.Visible;
            lblEnterLastName.Content = "Enter Tool ID";
        }
        private void ShowEmployeeControls()
        {
            txtEnterLastName.Visibility = Visibility.Visible;
            lblEnterLastName.Visibility = Visibility.Visible;
            lblEnterLastName.Content = "Enter Last Name";
            lblSelectEmployee.Visibility = Visibility.Visible;
            cboSelectEmployee.Visibility = Visibility.Visible;
        }

        private void cboSelectToolHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectToolHistory.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                gstrToolHistoryType = cboSelectToolHistory.SelectedItem.ToString();
                HideAllControls();

                if(gstrToolHistoryType == "Employee Search")
                {
                    ShowEmployeeControls();
                }
                else if(gstrToolHistoryType == "Tool ID Search")
                {
                    ShowToolIDControls();
                }

                btnFind.IsEnabled = true;
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strToolID = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseEmployee;
            string strWarehouseEmployeeName;

            try
            {
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                    gdatEndDate = TheDataSearchClass.AddingDays(gdatEndDate, 1);
                }
                if(gstrToolHistoryType == "Employee Search")
                {
                    if(cboSelectEmployee.SelectedIndex == 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Employee Was Not Selected\n";
                    }
                }
                if(gstrToolHistoryType == "Tool ID Search")
                {
                    strToolID = txtEnterLastName.Text;
                    if(strToolID == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "Tool ID Was Not Entered\n";
                    }
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheViewToolHistoryDataSet.toolhistory.Rows.Clear();

                if(gstrToolHistoryType == "Date Range Search")
                {
                    TheFindToolHistoryByDateRangeDataSet = TheToolHistoryClass.FindToolHistoryByDateRange(gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intWarehouseEmployee = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange[intCounter].WarehouseEmployeeID;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseEmployee);

                            strWarehouseEmployeeName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " " + TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                            ViewToolHistoryDataSet.toolhistoryRow NewHistoryRow = TheViewToolHistoryDataSet.toolhistory.NewtoolhistoryRow();

                            NewHistoryRow.LastName = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange[intCounter].LastName;
                            NewHistoryRow.Description = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange[intCounter].ToolDescription;
                            NewHistoryRow.FirstName = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange[intCounter].FirstName;
                            NewHistoryRow.ToolID = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange[intCounter].ToolID;
                            NewHistoryRow.TransactionDate = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange[intCounter].TransactionDate;
                            NewHistoryRow.TransactionID = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange[intCounter].TransactionID;
                            NewHistoryRow.TransactionNotes = TheFindToolHistoryByDateRangeDataSet.FindToolHistoryByDateRange[intCounter].TransactionNotes;
                            NewHistoryRow.WarehouseEmployee = strWarehouseEmployeeName;

                            TheViewToolHistoryDataSet.toolhistory.Rows.Add(NewHistoryRow);
                        }
                    }
                }
                else if(gstrToolHistoryType == "Employee Search")
                {
                    TheFindToolHistoryByEmployeeIDDataSet = TheToolHistoryClass.FindToolHistoryByEmployeeID(gdatStartDate, gdatEndDate, MainWindow.gintEmployeeID);

                    intNumberOfRecords = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intWarehouseEmployee = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID[intCounter].WarehouseEmployeeID;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseEmployee);

                            strWarehouseEmployeeName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " " + TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                            ViewToolHistoryDataSet.toolhistoryRow NewHistoryRow = TheViewToolHistoryDataSet.toolhistory.NewtoolhistoryRow();

                            NewHistoryRow.LastName = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID[intCounter].LastName;
                            NewHistoryRow.Description = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID[intCounter].ToolDescription;
                            NewHistoryRow.FirstName = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID[intCounter].FirstName;
                            NewHistoryRow.ToolID = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID[intCounter].ToolID;
                            NewHistoryRow.TransactionDate = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID[intCounter].TransactionDate;
                            NewHistoryRow.TransactionID = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID[intCounter].TransactionID;
                            NewHistoryRow.TransactionNotes = TheFindToolHistoryByEmployeeIDDataSet.FindToolHistoryByEmployeeID[intCounter].TransactionNotes;
                            NewHistoryRow.WarehouseEmployee = strWarehouseEmployeeName;

                            TheViewToolHistoryDataSet.toolhistory.Rows.Add(NewHistoryRow);
                        }
                    }
                }
                else if(gstrToolHistoryType == "Tool ID Search")
                {
                    TheFindActiveToolByToolIDDataSet = TheToolsClass.FindActiveToolByToolID(strToolID);

                    intNumberOfRecords = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

                    if(intNumberOfRecords == 0)
                    {
                        TheMessagesClass.ErrorMessage("The Tool is not Active or Does Not Exist");
                        return;
                    }

                    MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;

                    TheFindToolHistoryByToolKeyDataSet = TheToolHistoryClass.FindToolHistoryByToolKey(gdatStartDate, gdatEndDate, MainWindow.gintToolKey);

                    intNumberOfRecords = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intWarehouseEmployee = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey[intCounter].WarehouseEmployeeID;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseEmployee);

                            strWarehouseEmployeeName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " " + TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                            ViewToolHistoryDataSet.toolhistoryRow NewHistoryRow = TheViewToolHistoryDataSet.toolhistory.NewtoolhistoryRow();

                            NewHistoryRow.LastName = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey[intCounter].LastName;
                            NewHistoryRow.Description = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey[intCounter].ToolDescription;
                            NewHistoryRow.FirstName = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey[intCounter].FirstName;
                            NewHistoryRow.ToolID = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey[intCounter].ToolID;
                            NewHistoryRow.TransactionDate = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey[intCounter].TransactionDate;
                            NewHistoryRow.TransactionID = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey[intCounter].TransactionID;
                            NewHistoryRow.TransactionNotes = TheFindToolHistoryByToolKeyDataSet.FindToolHistoryByToolKey[intCounter].TransactionNotes;
                            NewHistoryRow.WarehouseEmployee = strWarehouseEmployeeName;

                            TheViewToolHistoryDataSet.toolhistory.Rows.Add(NewHistoryRow);
                        }
                    }
                }

                dgrResults.ItemsSource = TheViewToolHistoryDataSet.toolhistory;

            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Tools WPF // View Tool History // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;
            string strName;

            if(gstrToolHistoryType == "Employee Search")
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;
                
                if(intLength > 3)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheFindEmployeeByLastNameDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(strLastName);

                    intNumberOfRecords = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].FirstName + " ";
                            strName += TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].LastName;
                            cboSelectEmployee.Items.Add(strName);
                        }

                        cboSelectEmployee.SelectedIndex = 0;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                    }
                }
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intSelectedIndex].EmployeeID;

                btnFind.IsEnabled = true;
            }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //this will print the report
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;


            try
            {
                PrintDialog pdHistoryReport = new PrintDialog();

                if (pdHistoryReport.ShowDialog().Value)
                {
                    FlowDocument fdHistoryReport = new FlowDocument();
                    Thickness thickness = new Thickness(100, 50, 50, 50);
                    fdHistoryReport.PagePadding = thickness;

                    pdHistoryReport.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                    //Set Up Table Columns
                    Table HistoryReportTable = new Table();
                    fdHistoryReport.Blocks.Add(HistoryReportTable);
                    HistoryReportTable.CellSpacing = 0;
                    intColumns = TheViewToolHistoryDataSet.toolhistory.Columns.Count;

                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        HistoryReportTable.Columns.Add(new TableColumn());
                    }
                    HistoryReportTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    HistoryReportTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = HistoryReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Blue Jay Communications Tool History Report"))));
                    newTableRow.Cells[0].FontSize = 16;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Header Row
                    HistoryReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = HistoryReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Tool ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Decription"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Warehouse Employee"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Notes"))));
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 20);

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 11;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();
                    }

                    intNumberOfRecords = TheViewToolHistoryDataSet.toolhistory.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        HistoryReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = HistoryReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheViewToolHistoryDataSet.toolhistory[intReportRowCounter][intColumnCounter].ToString()))));


                            newTableRow.Cells[intColumnCounter].FontSize = 8;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                        }
                    }



                    //Set up page and print
                    fdHistoryReport.ColumnWidth = pdHistoryReport.PrintableAreaWidth;
                    fdHistoryReport.PageHeight = pdHistoryReport.PrintableAreaHeight;
                    fdHistoryReport.PageWidth = pdHistoryReport.PrintableAreaWidth;
                    pdHistoryReport.PrintDocument(((IDocumentPaginatorSource)fdHistoryReport).DocumentPaginator, "Blue Jay Communications Tool History Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Tools WPF // View Tool History // Print Button " + Ex.Message);
            }
        }

        private void btnExportHistoryToExcel_Click(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "InventorySheet";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheViewToolHistoryDataSet.toolhistory.Rows.Count;
                intColumnNumberOfRecords = TheViewToolHistoryDataSet.toolhistory.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheViewToolHistoryDataSet.toolhistory.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheViewToolHistoryDataSet.toolhistory.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                PleaseWait.Close();

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Inventory Project // Print Inventory Sheet // Export to Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
