using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace BeeGraph.Editor.Dialogs
{
    [Serializable]
    public class ManageDialog : IDialog
    {
        private readonly Dictionary<string, IDialog> _dialogs = new Dictionary<string, IDialog>()
        {
            { "Manage nodes", new NodeManagementDialog() }
        };

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hello! Here you can manage your bee graph!");
            var options = _dialogs.Keys;
            PromptDialog.Choice(context, ResumeAfterMenuSelection, options, "What do you want?");
        }

        private async Task ResumeAfterMenuSelection(IDialogContext context, IAwaitable<object> result)
        {
            var answer = (await result).ToString();
            var dialog = _dialogs[answer];
            await dialog.StartAsync(context);
        }
    }
}