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
        private IDictionary<string, Action<IDialogContext>> _actions;

        [OnDeserialized]
        private void Init(StreamingContext context)
        {
            _nodeRepository = EditorContainer.Container.GetInstance<INodeRepository>();
            _actions = new Dictionary<string, Action<IDialogContext>>()
            {
                {"List all nodes", ListAllNodesOptionSelected},
                {"Add node", AddNodeOptionSelected }
            };
            _actionsAfterNodeSelected = _nodeRepository
                .GetAll()
                .Select(MapToViewModel)
                .ToDictionary<string, string, Action<IDialogContext>>(str => str, str => ctx => EditNodeOptionSelected(str, ctx));

            _actionsAfterNodeSelected.Add("Go back", async ctx => await StartAsync(ctx));
        }

        private void EditNodeOptionSelected(string nodeRepresentation, IDialogContext context)
        {
            
        }

        private void AddNodeOptionSelected(IDialogContext context)
        {
            var promptDialog = new PromptDialog.PromptString("Give me the body of the node!", "", 7);
            context.Call(promptDialog, ResumeAfterNewNodeCreated);
        }

        private async Task ResumeAfterNewNodeCreated(IDialogContext context, IAwaitable<string> result)
        {
            var body = await result;
            _nodeRepository.CreateNode(body);
            await context.PostAsync("OK, node has been added!");
            ListAllNodesOptionSelected(context);
        }

        private Dictionary<string, Action<IDialogContext>> _actionsAfterNodeSelected;

        private async void ListAllNodesOptionSelected(IDialogContext context)
        {
            var options = _actionsAfterNodeSelected.Keys;
            PromptDialog.Choice(context, ResumeAfterNodeSelected, options, "What node do you want to edit?");
            //await context.PostAsync(nodeList);
        }

        private async Task ResumeAfterNodeSelected(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;
            var action = _actionsAfterNodeSelected[answer];
            action(context);
        }

        private string MapToViewModel(NodeEntity node) => $"{node.Id} - {node.Body}";

        public async Task StartAsync(IDialogContext context)
        {
            var options = _actions.Keys;
            PromptDialog.Choice(context, ResumeAfterActionSelection, options, "What do you want?");
        }

        private async Task ResumeAfterActionSelection(IDialogContext context, IAwaitable<object> result)
        {
            var answer = (await result).ToString();
            var action = _actions[answer];
            action(context);
        }
    }
}