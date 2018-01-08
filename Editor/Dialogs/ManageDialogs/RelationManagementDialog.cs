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
    public class RelationManagementDialog : IDialog
    {
        [NonSerialized]
        private INodeRepository _nodeRepository;
        [NonSerialized]
        private IEdgeRepository _edgeRepository;
        [NonSerialized]
        private INodeRelationRepository _relationRepository;

        private Dictionary<string, Action<IDialogContext>> _actions;

        [OnDeserialized]
        private void Init(StreamingContext context)
        {
            Init();
        }

        public RelationManagementDialog()
        {
            Init();
        }

        public async Task StartAsync(IDialogContext context)
        {
            var options = _actions.Keys;
            PromptDialog.Choice(context, ResumeAfterRelationSelected, options, "Select the relation");
        }

        private Task ResumeAfterRelationSelected(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private void Init()
        {
            _edgeRepository = EditorContainer.Container.GetInstance<IEdgeRepository>();
            _nodeRepository = EditorContainer.Container.GetInstance<INodeRepository>();
            _relationRepository = EditorContainer.Container.GetInstance<INodeRelationRepository>();

            _actions =
                _relationRepository
                    .GetAllEdgeRelations()
                    .Select(LabelService.GetLabel)
                    .ToDictionary<string, string, Action<IDialogContext>>(
                        str => str,
                        str => ctx => RelationSelected(str, ctx));

            _actions.Add("Add new relation", AddNewRelationOptionSelected);

        }

        private async void AddNewRelationOptionSelected(IDialogContext context)
        {
            
        }

        private void RelationSelected(string str, IDialogContext ctx)
        {
            throw new NotImplementedException();
        }

    }
}