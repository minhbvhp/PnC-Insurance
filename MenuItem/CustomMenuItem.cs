﻿using MaterialDesignThemes.Wpf;
using PnC_Insurance.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.MenuItem
{
    public class CustomMenuItem
    {
        public BaseVM ContentViewModels { get; private set; }
        public string Label { get; private set; }
        public PackIconKind IconKind { get; private set; }

        public CustomMenuItem(BaseVM contentViewModels, string label, PackIconKind iconKind)
        {
            ContentViewModels = contentViewModels;
            Label = label;
            IconKind = iconKind;
        }
    }
}
