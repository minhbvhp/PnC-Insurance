using MaterialDesignThemes.Wpf;
using PnC_Insurance.ViewModel;

namespace PnC_Insurance.MenuItem
{
    public class CustomSubItem : CustomMenuItem
    {
        public CustomSubItem(BaseVM? contentViewModels, string label, PackIconKind iconKind = PackIconKind.None) : base(contentViewModels, label, iconKind)
        {
        }
    }
}