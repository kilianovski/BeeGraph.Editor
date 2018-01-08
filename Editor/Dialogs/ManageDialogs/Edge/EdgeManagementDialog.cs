using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using BeeGraph.Data;
using Microsoft.Bot.Builder.Dialogs;
using static BeeGraph.Editor.Dialogs.DialogHelper;

namespace BeeGraph.Editor.Dialogs
{
    [Serializable]
    public class EdgeManagementDialog : IDialog
    {
        private IDictionary<string, Action<IDialogContext>> _actions = new Dictionary<string, Action<IDialogContext>>()
        {
            {"Show me all my edges!",  CallDialog<ListEdgeDialog>},
            {"Add new edge!", CallDialog<AddEdgeDialog>},
            {"Go back", CallDialog<ManageDialog>}
        };

        [OnDeserialized]
        private void Init(StreamingContext context)
        {
            //_actions = 
        }

        public async Task StartAsync(IDialogContext context)
        {
            var options = _actions.Keys;
            PromptDialog.Choice(context, ResumeAfterActionSelection, options, "What do you want?");
        }

        private async Task ResumeAfterActionSelection(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            var action = _actions[answer];
            action(context);
        }
    }
}