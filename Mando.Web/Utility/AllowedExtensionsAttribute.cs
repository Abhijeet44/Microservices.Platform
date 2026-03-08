using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility
{
	public class MaxFileSizeAttribute : ValidationAttribute
	{
		public readonly int _maxFileSize;

		public MaxFileSizeAttribute(int maxFileSize)
		{
			_maxFileSize = maxFileSize;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var maxFileSize = 1024 * 1024 * 1;
			var file = value as IFormFile;
			if (file != null)
			{
				if (file.Length > maxFileSize)
				{
					return new ValidationResult($"File size should not exceed 1 MB.");
				}
			}

			return ValidationResult.Success;
		}
	}
}
