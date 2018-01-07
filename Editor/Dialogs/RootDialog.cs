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

        private readonly Dictionary<string, IDialog> _dialogs = new Dictionary<string, IDialog>()
        {
            { ManageDialogOption, new ManageDialog() }
        };

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }   

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync(WelcomeMessage);
            var options = _dialogs.Keys;
            PromptDialog.Choice(context, ResumeAfterMenuSelection, options,"What do you want?");
        }

        private async Task ResumeAfterMenuSelection(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            var dialog = _dialogs[answer];
            await dialog.StartAsync(context);
        }
    }
}