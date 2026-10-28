using System.ComponentModel.DataAnnotations;

namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly RequiredAttribute _innerAttribute = new RequiredAttribute();
        private string DependentProperty { get; set; }
        private object TargetValue { get; set; }

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            DependentProperty = dependentProperty;
            TargetValue = targetValue;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty(DependentProperty);
            if (field != null)
            {
                var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
                if ((dependentValue == null && TargetValue == null) || dependentValue.Equals(TargetValue))
                {
                    if (!_innerAttribute.IsValid(value))
                    {
                        return new ValidationResult(ErrorMessage = validationContext.DisplayName + " is required.");
                    }
                }
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(DependentProperty));
            }
        }
    }
}
