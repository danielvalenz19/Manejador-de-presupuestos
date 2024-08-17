using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Views.Validaciones
{
	public class PrimeraLetraMayusculaAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null || string.IsNullOrEmpty(value.ToString()))
			{
				return ValidationResult.Success;
			}

			var primeraLetra = value.ToString()[0].ToString();

			if (primeraLetra != primeraLetra.ToUpper())
			{
				return new ValidationResult("La Primera letra debe de ser MAYUSCULA");
			}
			return ValidationResult.Success;

		}
	}
}
