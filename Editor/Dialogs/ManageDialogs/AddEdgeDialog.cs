using System;
using System.Threading.Tasks;
using BeeGraph.Data;
using Microsoft.Bot.Builder.Dialogs;

namespace BeeGraph.Editor.Dialogs
{
    [Serializable]
    internal class AddEdgeDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {           
            PromptDialog.Text(context, ResumeAfterKeyEntered, "What is the key of the edge?");
        }

        private async Task ResumeAfterKeyEntered(IDialogContext context, IAwaitable<string> result)
        {
            var newKey = await result;
            var repository = EditorContainer.Container.GetInstance<IEdgeRepository>();
            repository.CreateEdge(newKey);

            await context.PostAsync($"New edge with the key {newKey} successfully created!");
            DialogHelper.CallDialog<ListEdgeDialog>(context);
        }
    }
}