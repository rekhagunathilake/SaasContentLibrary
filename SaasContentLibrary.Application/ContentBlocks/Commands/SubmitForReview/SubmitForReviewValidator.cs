using FluentValidation;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.SubmitForReview;

public sealed class SubmitForReviewValidator : AbstractValidator<SubmitForReviewCommand>
{
    public SubmitForReviewValidator() {
        RuleFor(x => x.ContentBlockId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
    }
}
