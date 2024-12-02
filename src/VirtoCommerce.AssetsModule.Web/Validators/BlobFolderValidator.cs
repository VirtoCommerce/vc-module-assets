using System.Text.RegularExpressions;
using FluentValidation;
using VirtoCommerce.AssetsModule.Core.Assets;

namespace VirtoCommerce.AssetsModule.Web.Validators
{
    public partial class BlobFolderValidator : AbstractValidator<BlobFolder>
    {
        public BlobFolderValidator()
        {
            RuleFor(context => context.Name)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Folder name must not be null.")
                .NotEmpty()
                .WithMessage("Folder name must not be empty.")
                .MinimumLength(3)
                .WithMessage(x => $"Folder name must be at least 3 characters long. You entered {x.Name.Length} characters.")
                .MaximumLength(63)
                .WithMessage(x => $"Folder name must be at most 63 characters long. You entered {x.Name.Length} characters.")
                .Must(x => !x.StartsWith('-'))
                .WithMessage("Folder name must not start with a dash symbol.")
                .Must(x => !x.EndsWith('-'))
                .WithMessage("Folder name must not end with a dash symbol.")
                .Must(x => !x.Contains("--"))
                .WithMessage("Folder name must not contain consecutive dash symbols.")
                .Must(x => !FolderNameRegex().IsMatch(x))
                .WithMessage("Folder name must only contain lowercase letters, numbers, spaces, and single dashes.");
        }

        [GeneratedRegex("[^0-9a-z -]")]
        private static partial Regex FolderNameRegex();
    }
}
