﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PnC_Insurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PnC_Insurance.ViewModel
{
    public partial class ExtensionInfoVM : BaseVM
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchExtensionCommand))]
        private string? extensionSearch;

        private List<Extension> listOfExtensions = new List<Extension>();
        public List<Extension>? ListOfExtensions
        {
            get
            {                
                return new List<Extension>(listOfExtensions);
            }

        }

        [ObservableProperty]
        private Extension? selectedExtension;

        [RelayCommand(CanExecute = nameof(CanSearchExtension))]
        private async Task SearchExtensionAsync()
        {
            var result = await Task.Run(() =>
            {
                using (var context = new InsuranceDbContext())
                {
                    var query = from extension in context.Extensions.AsNoTracking()
                                where extension.IsDeleted == 0 &&
                                      (EF.Functions.Like(extension.Code, "%" + ExtensionSearch + "%") ||
                                      EF.Functions.Like(extension.Name, "%" + ExtensionSearch + "%"))
                                orderby extension.Id
                                select extension;
                    return query.ToListAsync();
                }
            });

            listOfExtensions = result;
            OnPropertyChanged(nameof(ListOfExtensions));
        }

        private bool CanSearchExtension()
        {
            if (!String.IsNullOrEmpty(ExtensionSearch) && !String.IsNullOrWhiteSpace(ExtensionSearch))
                return true;

            return false;
        }
    }
}
