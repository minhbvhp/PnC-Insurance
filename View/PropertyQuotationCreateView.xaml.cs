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

namespace PnC_Insurance.View
{
    /// <summary>
    /// Interaction logic for QuotationCreateView.xaml
    /// </summary>
    public partial class PropertyQuotationCreateView : UserControl
    {
        public PropertyQuotationCreateView()
        {
            InitializeComponent();
        }

        private void ListOfChosenLocationDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
