using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using BeeGraph.Data;
using Microsoft.Bot.Builder.Dialogs;

namespace BeeGraph.Editor.Dialogs
{
    [Serializable]
    public class ListEdgeDialog : IDialog
    {
        [NonSerialized]
        private Dictionary<string, Action<IDialogContext>> _actionsForEdgeSelection;
        private Dictionary<string, Action<IDialogContext>> _actionsForEdit;        

        private const int DefaultCurrentEdgeId = Int32.MinValue;

        private int _currentEdgeId = DefaultCurrentEdgeId;

        [OnDeserialized]
        private void Init(StreamingContext context)
        {
            _actionsForEdit = new Dictionary<string, Action<IDialogContext>>()
            {
                { "Delete this edge!", DeleteCurrentEdge},
                {"Go back", async ctx => await StartAsync(ctx)}

            };

            var edgeRepository = EditorContainer.Container.GetInstance<IEdgeRepository>();

            _actionsForEdgeSelection = edgeRepository
                .GetAll()
                .Select(LabelService.GetLabel)
                .ToDictionary<string, string, Action<IDialogContext>>(
                    str => str,
                    str => ctx => EdgeSelected(str, ctx));

            _actionsForEdgeSelection.Add("Go back!", DialogHelper.CallDialog<EdgeManagementDialog>);
        }

        private void DeleteCurrentEdge(IDialogContext context)
        {
            PromptDialog.Confirm(context, ConfirmEdgeDeletion, "Are you sure you want delete this EDGE?");
        }

        private async Task ConfirmEdgeDeletion(IDialogContext context, IAwaitable<bool> result)
        {
            bool isDeletionConfirmed = await result;
            if (isDeletionConfirmed)
            {
                EditorContainer.Container.GetInstance<IEdgeRepository>().Delete(_currentEdgeId);
            }
            else await context.PostAsync("OK. Returning you to the list");

            Init(default);
            await StartAsync(context);
        }

        public ListEdgeDialog()
        {
            Init(default);
        }

        private async void EdgeSelected(string str, IDialogContext context)
        {
            _currentEdgeId = LabelService.GetIdentifier(str);
            var edgeRepository = EditorContainer.Container.GetInstance<IEdgeRepository>();

            var currentEdge = edgeRepository.GetAll().Single(e => e.Id == _currentEdgeId);
            var options = _actionsForEdit.Keys;

            PromptDialog.Choice(context, ResumeAfterEditOptionSelected, options,
                "What do you want to do with your edge?\n\n" + currentEdge.Key);

        }

        private async Task ResumeAfterEditOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            var action = _actionsForEdit[answer];
            action(context);
        }

        public async Task StartAsync(IDialogContext context)
        {
            _currentEdgeId = DefaultCurrentEdgeId;
            var options = _actionsForEdgeSelection.Keys;
            PromptDialog.Choice(context, ResumeAfterEdgeSelected, options, "Select the edge to edit");
        }

        private async Task ResumeAfterEdgeSelected(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            var action = _actionsForEdgeSelection[answer];
            action(context);
        }
    }
}