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
using Drillholes.Windows.Dialogs;
using System.Windows.Controls.Ribbon;
using System.IO;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeStartup.xaml
    /// </summary>
    public partial class DrillholeStartup : RibbonWindow
    {
        public DrillholeStartup()
        {
            InitializeComponent();
        }

        private void ExitForm(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HelpDocumentation(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }

        private void New_Session(object sender, RoutedEventArgs e)
        {
           // string path = XmlDefaultPath.GetFullPathAndFilename();

            
                frameMain.Source = new Uri("DrillholeDialogPage.xaml", UriKind.Relative);
                txtStart.Visibility = Visibility.Collapsed;
            

            
        }

        private void Open_Session(object sender, RoutedEventArgs e)
        {
            frameMain.Source = new Uri("DrillholeImportPage.xaml", UriKind.Relative);
            txtStart.Visibility = Visibility.Collapsed;

        }


    }
}
