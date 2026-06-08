using FluentValidation;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.GetVersionHistory
{
    public sealed class GetVersionHistoryValidator : AbstractValidator<GetVersionHistoryQuery>
    {
        public GetVersionHistoryValidator() => RuleFor(x => x.ContentBlockId).NotEmpty();
    }
}
