using FinalProject.Domain;

namespace FinalProject.Services;

public sealed class PlanBuildResult
{
    public List<PlanItem> Items { get; } = new List<PlanItem>();
    public List<string> Warnings { get; } = new List<string>();
}
