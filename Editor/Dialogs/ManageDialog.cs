using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using static BeeGraph.Editor.Dialogs.DialogHelper;

namespace BeeGraph.Editor.Dialogs
{
    [Serializable]
    public class ManageDialog : IDialog
    {        
        private readonly Dictionary<string, Action<IDialogContext>> _actions = new Dictionary<string, Action<IDialogContext>>()
        {
            { "Manage nodes",  CallDialog<NodeManagementDialog> },
            { "Manage edges",  CallDialog<EdgeManagementDialog>},
            { "Go back", CallDialog<RootDialog> }
        };

        public async Task StartAsync(IDialogContext context)
        {
            await ShowChoice(context);
        }

        private async Task ShowChoice(IDialogContext context)
        {
            await context.PostAsync("Hello! Here you can manage your bee graph!");
            var options = _actions.Keys;
            PromptDialog.Choice(context, ResumeAfterMenuSelection, options, "What do you want?");
        }

        private async Task ResumeAfterMenuSelection(IDialogContext context, IAwaitable<object> result)
        {
            var answer = (await result).ToString();
            var action = _actions[answer];
            action(context);

        }
    }
}