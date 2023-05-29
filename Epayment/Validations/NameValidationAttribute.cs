using BCXN.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Validations
{
    public class NameValidationAttribute : ValidationAttribute
    {
        private string _name;
        public NameValidationAttribute(string name)
        {
            _name = name;
        }

        // protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        // {
        //     Customer customer = (Customer)validationContext.ObjectInstance;

        //     if (!customer.Name.Contains(_name))
        //     {
        //         return new ValidationResult(GetErrorMessage());
        //     }

        //     return ValidationResult.Success;
        // }

        public string GetErrorMessage()
        {
            return $"Customer's name must contain {_name}.";
        }
    }
}
