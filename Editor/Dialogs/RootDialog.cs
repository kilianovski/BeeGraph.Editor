using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace BeeGraph.Editor.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string WelcomeMessage = "Welcome you in admin panel for your story bot";

        private const string ManageDialogOption = "Manage your dialog";

        private Dictionary<string, IDialog> dialogs = new Dictionary<string, IDialog>()
        {
            { ManageDialogOption, DialogLocator.ManageDialog }
        };

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(WelcomeMessage);
            var options = dialogs.Keys;
            PromptDialog.Choice(context, ResumeAfterMenuSelection, options,"What do you want?");
        }

        private async Task ResumeAfterMenuSelection(IDialogContext context, IAwaitable<object> result)
        {
            var answer = (await result).ToString();

            if (dialogs.TryGetValue(answer, out var dialog))
            {
                await dialog.StartAsync(context);
            }
            else
            {
                await context.PostAsync("I cant understand you!");
            }
        }
    }
}