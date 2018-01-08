using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using BeeGraph.Data;
using BeeGraph.Data.Entities;
using Microsoft.Bot.Builder.Dialogs;

namespace BeeGraph.Editor.Dialogs
{
    [Serializable]
    public class NodeManagementDialog : IDialog
    {
        [NonSerialized]
        private INodeRepository _nodeRepository;

        private const int DefaultCurrentNodeId = Int32.MinValue;

        private int _currentNodeId = DefaultCurrentNodeId;

        private IDictionary<string, Action<IDialogContext>> _actions;
        private Dictionary<string, Action<IDialogContext>> _actionsForNodeSelection;
        private Dictionary<string, Action<IDialogContext>> _actionsOnNodeEditMenu;

        public NodeManagementDialog()
        {
            Init(default);
        }
        [OnDeserialized]
        private void Init(StreamingContext context)
        {
            _nodeRepository = EditorContainer.Container.GetInstance<INodeRepository>();
            _actions = new Dictionary<string, Action<IDialogContext>>()
            {
                {"List all nodes", ListAllNodesOptionSelected},
                {"Add node", AddNodeOptionSelected },
                {"Lets go back", DialogHelper.CallDialog<ManageDialog> }
            };

            _actionsOnNodeEditMenu = new Dictionary<string, Action<IDialogContext>>()
            {
                { "Edit this node", EditCurrentNodeOptionSelected},
                { "Delete this node", DeleteCurrentNodeOptionSelected},
                { "Go back",  ListAllNodesOptionSelected }

            };

            SetActionsAfterNodeSelected();
        }

        public async Task StartAsync(IDialogContext context)
        {
            var options = _actions.Keys;
            PromptDialog.Choice(context, ResumeAfterActionSelection, options, "What do you want?");
        }

        private void SetActionsAfterNodeSelected()
        {
            _actionsForNodeSelection = _nodeRepository
                .GetAll()
                .Select(LabelService.GetLabel)
                .ToDictionary<string, string, Action<IDialogContext>>(
                    str => str,
                    str => ctx => NodeSelected(str, ctx));

            _actionsForNodeSelection.Add("Go back", async ctx => await StartAsync(ctx));
        }

        private async void DeleteCurrentNodeOptionSelected(IDialogContext context)
        {
            PromptDialog.Confirm(context, ConfirmNodeDialog, "Are you sure you want delete this node?");
        }

        private async Task ConfirmNodeDialog(IDialogContext context, IAwaitable<bool> result)
        {
            bool isDeletionConfirmed = await result;
            if (isDeletionConfirmed)
            {
                _nodeRepository.Delete(_currentNodeId);
                SetActionsAfterNodeSelected();
            }
            else await context.PostAsync("OK. Returning you to the list");
            ListAllNodesOptionSelected(context);
        }

        private async void EditCurrentNodeOptionSelected(IDialogContext context)
        {

        }

        private async Task ResumeAfterEditOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            string option = await result;
            var action = _actionsOnNodeEditMenu[option];
            action(context);
        }

        private async void NodeSelected(string nodeLabel, IDialogContext context)
        {
            _currentNodeId = LabelService.GetIdentifier(nodeLabel);

            var options = _actionsOnNodeEditMenu.Keys;
            var currentNode = _nodeRepository.Get(_currentNodeId).Value;

            PromptDialog.Choice(context, ResumeAfterEditOptionSelected, options,
                "What do you want to do with your node?\n\n" + currentNode.Body);
        }

        private void AddNodeOptionSelected(IDialogContext context)
        {
            var promptDialog = new PromptDialog.PromptString("Give me the body of the node!", "", 7);
            context.Call(promptDialog, ResumeAfterNewNodeCreated);
        }

        private async Task ResumeAfterNewNodeCreated(IDialogContext context, IAwaitable<string> result)
        {
            var body = await result;
            CreateNode(body);
            await context.PostAsync("OK, node has been added!");
            ListAllNodesOptionSelected(context);
        }

        private void CreateNode(string body)
        {
            _nodeRepository.CreateNode(body);
            SetActionsAfterNodeSelected();
        }

        private async void ListAllNodesOptionSelected(IDialogContext context)
        {
            _currentNodeId = DefaultCurrentNodeId;
            var options = _actionsForNodeSelection.Keys;
            PromptDialog.Choice(context, ResumeAfterListSelected, options, "What node do you want to edit?");
        }

        private async Task ResumeAfterListSelected(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            var action = _actionsForNodeSelection[answer];
            action(context);
        }

        private async Task ResumeAfterActionSelection(IDialogContext context, IAwaitable<string> result)
        {
            var answer = (await result).ToString();
            var action = _actions[answer];
            action(context);
        }
    }
}