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
using Drillholes.Domain.Enum;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeCreate.xaml
    /// </summary>
    public partial class DrillholeCreate : Window
    {
        private DrillholeDesurveyEnum desurveyMethod { get; set; }
        private int _tabControl { get; set; }
        public DrillholeCreate()
        {
            desurveyMethod = DrillholeDesurveyEnum.Tangential;

            InitializeComponent();
        }

        private void btnDesurvey_Click(object sender, RoutedEventArgs e)
        {
            if (_tabControl == 0)
            {

            }
            else if (_tabControl == 1)
            {
            }
            else if (_tabControl == 2)
            {
            }
            else if (_tabControl == 3)
            {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
