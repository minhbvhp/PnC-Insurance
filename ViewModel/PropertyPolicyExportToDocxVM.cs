using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Words.NET;

namespace PnC_Insurance.ViewModel
{
    public partial class PropertyPolicyExportToDocxVM : BaseVM
    {
        #region Search Quotations
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchQuotationCommand))]
        private string? quotationSearch;

        private List<PropertyPolicy> listOfQuotations = new List<PropertyPolicy>();
        public List<PropertyPolicy>? ListOfQuotations
        {
            get
            {
                return new List<PropertyPolicy>(listOfQuotations);
            }

        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ListOfMatchExtensions))]
        [NotifyCanExecuteChangedFor(nameof(QuotationExportToDocxVNCommand))]
        private PropertyPolicy? selectedQuotation;

        [RelayCommand(CanExecute = nameof(CanSearchQuotation))]
        private async Task SearchQuotationAsync()
        {
            var result = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from quotation in context.PropertyPolicies.Include(nameof(Customer)).AsNoTracking()
                                where quotation.IsDeleted == 0 && quotation.Confirm == 0 &&
                                      (EF.Functions.Like(quotation.PolicyNo, "%" + QuotationSearch + "%") ||
                                      EF.Functions.Like(quotation.Customer.Name, "%" + QuotationSearch + "%"))
                                orderby quotation.Id
                                select quotation;

                    return query.ToListAsync();
                }
            });

            listOfQuotations = result;
            OnPropertyChanged(nameof(ListOfQuotations));
        }

        private bool CanSearchQuotation()
        {
            if (!String.IsNullOrEmpty(QuotationSearch) && !String.IsNullOrWhiteSpace(QuotationSearch))
                return true;

            return false;
        }

        #endregion

        #region Match Extensions
        public List<PropertyPoliciesExtension>? ListOfMatchExtensions 
        {
            get
            {
                using (var context = new InsuranceDbContext())
                {
                    if (SelectedQuotation != null)
                    {
                        var query = from propertyPolicyExtension in context.PropertyPoliciesExtensions.Include(nameof(Extension)).AsNoTracking()
                                    where propertyPolicyExtension.PropertyPolicyId == SelectedQuotation.Id
                                    select propertyPolicyExtension;

                        if (query.Any())
                        {
                            return query.ToList();
                        }
                    }

                    return new List<PropertyPoliciesExtension>();
                }
            }
        }
        #endregion

        #region Export Quotation to Docx
        [ObservableProperty]
        [Required (ErrorMessage = "Chọn đường dẫn lưu file")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(QuotationExportToDocxVNCommand))]
        [NotifyPropertyChangedFor(nameof(DocumentFileName))]
        private string? destinationPath;

        public string? DocumentFileName 
        {
            get
            {
                if (!String.IsNullOrEmpty(DestinationPath) && !String.IsNullOrWhiteSpace(DestinationPath))
                {
                    string input = DestinationPath;
                    int index = input.IndexOf(".DOCX");
                    return input.Substring(0, index);
                }

                return null;
            }
        }

        [RelayCommand(CanExecute = nameof(CanQuotationExportToDocxVN))]
        private async Task QuotationExportToDocxVNAsync()
        {
            ResultNotification = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            DocX document;

            try
            {
                document = DocX.Load(@".\Template\PropertyQuotationTemplate.docx");
            }
            catch
            {
                ResultNotification.Enqueue("Không tìm thấy file Template để tạo bản chào");
                return;
            }

            if (document != null && SelectedQuotation != null)
            {
                using (document)
                {
                    try
                    {
                        document.ReplaceText("<<[customer." + nameof(SelectedQuotation.Customer.Name) + "]>>", SelectedQuotation.Customer.Name);
                        document.ReplaceText("<<[customer." + nameof(SelectedQuotation.Customer.ClientCode) + "]>>", SelectedQuotation.Customer.ClientCode);
                        document.ReplaceText("<<[customer." + nameof(SelectedQuotation.Customer.Address) + "]>>", SelectedQuotation.Customer.Address);
                        document.ReplaceText("<<[customer." + nameof(SelectedQuotation.Customer.Business) + "]>>", SelectedQuotation.Customer.Business);

                        document.SaveAs(DestinationPath);
                        ResultNotification.Enqueue("Đã lập bản chào đề xuất");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                        ResultNotification.Enqueue("Lỗi không lưu được file");
                    }

                }
            }


        }

        private bool CanQuotationExportToDocxVN()
        {
            if (SelectedQuotation != null && !String.IsNullOrEmpty(DestinationPath) && !String.IsNullOrWhiteSpace(DestinationPath))
                return true;

            return false;
        }
        #endregion
    }
}
