using FluentValidation;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.CreateContentBlock;

public sealed class CreateContentBlockValidator : AbstractValidator<CreateContentBlockCommand>
{
    public CreateContentBlockValidator() 
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required.");

        RuleFor(x => x.BlockType)
            .IsInEnum().WithMessage("BlockType must be a valid enum value.");

        RuleFor(x => x.LocaleCode)
            .NotEmpty().WithMessage("LocaleCode is required.")
            .MaximumLength(10).WithMessage("LocaleCode must not exceed 10 characters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(ContentBlock.NameMaxLength).WithMessage($"Name must not exceed {ContentBlock.NameMaxLength} characters.");

        RuleFor(x => x.ContentBody)
            .NotEmpty().WithMessage("ContentBody is required.")
            .MaximumLength(ContentBody.MaxLength).WithMessage($"ContentBody must not exceed {ContentBody.MaxLength} characters.");

        RuleFor(x => x.AuthoredBy)
            .NotEmpty().WithMessage("AuthoredBy is required.");
    }
}
