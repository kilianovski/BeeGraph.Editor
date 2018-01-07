using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace BeeGraph.Editor.Dialogs
{
    [Serializable]
    public class ManageDialog : IDialog
    {
        private Dictionary<string, IDialog> dialogs = new Dictionary<string, IDialog>()
        {
            { "Manage nodes", DialogLocator.NodeManagementDialog }
        };

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hello! Here you can manage your bee graph!");
            var options = dialogs.Keys;
            PromptDialog.Choice(context, ResumeAfterMenuSelection, options, "What do you want?");
        }

        private async Task ResumeAfterMenuSelection(IDialogContext context, IAwaitable<object> result)
        {
            var answer = (await result).ToString();
            var dialog = dialogs[answer];
            await dialog.StartAsync(context);
        }
    }
}