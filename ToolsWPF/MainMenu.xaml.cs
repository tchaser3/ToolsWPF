/* Title:           Main Menu
 * Date:            1-15-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the main menu for the application */

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

namespace ToolsWPF
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
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

        private void btnToolSignOut_Click(object sender, RoutedEventArgs e)
        {
            SignOutTool SignOutTool = new SignOutTool();
            SignOutTool.Show();
            Close();
        }

        private void btnToolSignIn_Click(object sender, RoutedEventArgs e)
        {
            SignInTool SignInTool = new SignInTool();
            SignInTool.Show();
            Close();
        }

        private void btnToolHistory_Click(object sender, RoutedEventArgs e)
        {
            ViewToolHistory ViewToolHistory = new ViewToolHistory();
            ViewToolHistory.Show();
            Close();
        }

        private void btnToolAvailability_Click(object sender, RoutedEventArgs e)
        {
            ToolAvailability ToolAvailability = new ToolAvailability();
            ToolAvailability.Show();
            Close();
        }
    }
}
