using SkillsPortal.API.Contracts;

namespace SkillsPortal.API.Shared
{
    public interface IRelationalValidationService
    {
        /// <summary>
        /// Validates relational entities exist based on provided  request ids
        /// </summary>
        /// <param name="request" cref="RelationalValidationRequest">Validation request with relational ids</param>
        /// <returns>ServiceResult of failed or success outcome.
        ///Success - collections of requested domain entities to validate
        /// </returns>
        Task<ServiceResult<CombinedValidationResponse>> ValidateRelationalProperties(
            RelationalValidationRequest request);
    }
}