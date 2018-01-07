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
                {"List all nodes", ListAllNodes}
            };
        }
        public NodeManagementDialog()
        {

        }


        private async void ListAllNodes(IDialogContext context)
        {
            var nodeList = _nodeRepository.GetAll().Select(MapToViewModel).Aggregate((acc, str) => acc + str + "\n");
            await context.PostAsync(nodeList);
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