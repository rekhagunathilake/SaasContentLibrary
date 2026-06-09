using FluentValidation;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.GetContentBlock;

public class GetContentBlockValidator : AbstractValidator<GetContentBlockQuery>
{
    public GetContentBlockValidator() {
        RuleFor(x => x.Id).NotEmpty();
    }
}
