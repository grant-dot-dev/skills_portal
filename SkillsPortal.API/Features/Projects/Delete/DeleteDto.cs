namespace SkillsPortal.API.Features.Projects.Delete;

public record DeleteProjectRequest(int ProjectId);

public record DeleteProjectResponse(bool Success);