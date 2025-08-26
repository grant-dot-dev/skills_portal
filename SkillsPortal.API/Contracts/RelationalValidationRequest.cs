namespace SkillsPortal.API.Contracts;

public record RelationalValidationRequest(int[] UserIds, int[] SkillIds);