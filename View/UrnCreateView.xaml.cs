using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PnC_Insurance.View
{
    /// <summary>
    /// Interaction logic for UrnCRUDView.xaml
    /// </summary>
    public partial class UrnCreateView : UserControl
    {
        public UrnCreateView()
        {
            InitializeComponent();
        }

        private void InputNumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = (new Regex("[^0-9\\s]+").IsMatch(e.Text));
        }

        private void PreventSpaceKey(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
