using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnC_Insurance.CustomAttribute
{
    public sealed class MinimumElementsAttribute : ValidationAttribute
    {
        private string _errorMessage;

        private readonly int _minElements;
        public MinimumElementsAttribute(int minElements, string errorMessage)
        {
            this._minElements = minElements;
            this._errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IList;

            var result = list?.Count >= _minElements;

            return result
                ? ValidationResult.Success
                : new (_errorMessage);
        }
    }
}
