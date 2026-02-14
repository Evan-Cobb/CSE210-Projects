using FinalProject.Core;
using FinalProject.Util;
using FinalProject.World;

namespace FinalProject.Abilities;

public class ViewInboxAbility : AbilityBase
{
    public ViewInboxAbility() : base("View Inbox")
    {
    }

    public override void Use(GameState state)
    {
        state.AddTurns(1);
        IReadOnlyList<VirtualFileItem> inbox = state.Vfs.GetFolderItems("Inbox");
        ConsoleUi.PrintInbox(inbox);
    }
}
