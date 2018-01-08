using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using BeeGraph.Data;
using Microsoft.Bot.Builder.Dialogs;

namespace BeeGraph.Editor.Dialogs 
{
    public class EditRelationDialog : IDialog<RelationEditModel>
    {
        private IEnumerable<string> _nodes;
        private IEnumerable<string> _edges;
        private RelationEditModel _value = new RelationEditModel();

        [OnDeserialized]
        private void Init(StreamingContext context)
        {
            Init();
        }
        public EditRelationDialog()
        {
            Init();
        }

        private void Init()
        {
            var _edgeRepository = EditorContainer.Container.GetInstance<IEdgeRepository>();
            var _nodeRepository = EditorContainer.Container.GetInstance<INodeRepository>();

            _nodes = _nodeRepository.GetAll().Select(LabelService.GetLabel);
            _edges = _edgeRepository.GetAll().Select(LabelService.GetLabel);
        }

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(context, FromNodeSelected, _nodes, "From what node do you want to go?");
        }

        private async Task FromNodeSelected(IDialogContext context, IAwaitable<string> result)
        {
            _value.FromNodeId = LabelService.GetIdentifier(await result);
            PromptDialog.Choice(context, EdgeSelected, _edges, "With what edge do you want to go?");
        }

        private async Task EdgeSelected(IDialogContext context, IAwaitable<string> result)
        {
            _value.EdgeId = LabelService.GetIdentifier(await result);
            PromptDialog.Choice(context, ToNodeSelected, _nodes, "To what node do you want to go?L");
        }

        private async Task ToNodeSelected(IDialogContext context, IAwaitable<string> result)
        {
            _value.ToNodeId = LabelService.GetIdentifier(await result);
            context.Done(_value);
        }
    }
}