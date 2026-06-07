using FluentValidation;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.ApproveVersion;

public sealed class ApproveVersionValidator : AbstractValidator<ApproveVersionCommand>
{
    public ApproveVersionValidator()
    {
        RuleFor(x => x.ContentBlockId)
            .NotEmpty();
        RuleFor(x => x.VersionId)
            .NotEmpty();
        RuleFor(x => x.ApprovedBy)
            .NotEmpty();
    }
}
