using Microsoft.Win32;
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
    /// Interaction logic for PropertyPolicyExportToDocxView.xaml
    /// </summary>
    public partial class PropertyPolicyExportToDocxView : UserControl
    {
        public PropertyPolicyExportToDocxView()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Word document file (*.docx)|*.docx";

            if (dialog.ShowDialog() == true)
            {
                this.FilePathTextBox.Text = dialog.FileName;

                this.FilePathTextBox
                  .GetBindingExpression(TextBox.TextProperty)
                  .UpdateSource();
            }
        }
    }
}
