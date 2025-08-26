using SkillsPortal.Core.Domain;

namespace SkillsPortal.API.Contracts;

public record CombinedValidationResponse(ICollection<User> Users, ICollection<Skill> Skills);