using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace BeeGraph.Editor.Dialogs
{
    public class RootDialog : IDialog<object>
    {
        private const string WelcomeMessage = "Welcome you in admin panel for your story bot";

        private const string ManageDialogOption = "Manage your dialog";

        private Dictionary<string, IDialog> dialogs = new Dictionary<string, IDialog>()
        {

        };

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(WelcomeMessage);
            //PromptDialog.Choice(context, ResumeAfterMenuSelection, Enum.GetNames(typeof(MainMenuButtons)),"How Are You?");
        }

        private Task ResumeAfterMenuSelection(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
    }
}