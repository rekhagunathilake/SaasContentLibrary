using FluentValidation;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.ListBlocksByType
{
    public sealed class ListBlocksByTypeValidator : AbstractValidator<ListBlocksByTypeQuery>
    {
        public ListBlocksByTypeValidator() {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.BlockType).IsInEnum().When(x => x.BlockType.HasValue);
        }
    }
}
