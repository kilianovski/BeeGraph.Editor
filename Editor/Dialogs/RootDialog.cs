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
    public class RootDialog : IDialog
    {
        private const string WelcomeMessage = "Welcome you in admin panel for your story bot";
        private const string ManageDialogOption = "Manage your dialog";

        private readonly Dictionary<string, Action<IDialogContext>> _actions = new Dictionary<string, Action<IDialogContext>>()
        {
            { ManageDialogOption, DialogHelper.CallDialog<ManageDialog> }
        };

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }   

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync(WelcomeMessage);
            var options = _actions.Keys;
            PromptDialog.Choice(context, ResumeAfterMenuSelection, options,"What do you want?");
        }

        private async Task ResumeAfterMenuSelection(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            var action = _actions[answer];
            action(context);
        }
    }
}