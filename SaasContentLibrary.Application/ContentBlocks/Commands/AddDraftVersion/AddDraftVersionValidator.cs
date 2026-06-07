using FluentValidation;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.AddDraftVersion;

public sealed class AddDraftVersionValidator : AbstractValidator<AddDraftVersionCommand>
{
    public AddDraftVersionValidator() {
        RuleFor(x => x.ContentBlockId).NotEmpty();
        RuleFor(x => x.ContentBody).NotEmpty().MaximumLength(ContentBody.MaxLength);
        RuleFor(x => x.AuthoredBy).NotEmpty();
    }
}
