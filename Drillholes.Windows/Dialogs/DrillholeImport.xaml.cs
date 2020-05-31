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
using Drillholes.Domain.DataObject;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeImport.xaml
    /// </summary>
    public partial class DrillholeImport : Window
    {
        private List<DrillholeTable> tables { get; set; }

        public DrillholeImport()
        {
            InitializeComponent();
        }

        public DrillholeImport(List<DrillholeTable> _classes)
        {
            InitializeComponent();

            tables = _classes;

        }

        private void _tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cboImportAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTrace_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDesurvey_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCreateToe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void radSurvey_Click(object sender, RoutedEventArgs e)
        {

        }

        private void radDhole_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkSkip_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ValidateDrillholes(object sender, RoutedEventArgs e)
        {

        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void radVertical_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lblFile_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
