/* Title:           Tool Availability
 * Date:            01-29-18
 * Author:          Terry Holmes
 * 
 * Description:     This is the form that shows tool availability */

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
    /// Interaction logic for ToolAvailability.xaml
    /// </summary>
    public partial class ToolAvailability : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public ToolAvailability()
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

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }
    }
}
