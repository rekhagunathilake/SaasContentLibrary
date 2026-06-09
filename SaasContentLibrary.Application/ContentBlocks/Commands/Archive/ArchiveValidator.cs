using FluentValidation;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.Archive;

public class ArchiveValidator : AbstractValidator<ArchiveCommand>
{
    public ArchiveValidator()
    {
        RuleFor(x => x.ContentBlockId).NotEmpty();
    }
}
