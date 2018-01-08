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

        private async Task ResumeAfterRelationSelected(IDialogContext context, IAwaitable<string> result)
        {
            var action = _actions[await result];
            action(context);
        }

        private void Init()
        {

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
            var editDialog = new EditRelationDialog();
            context.Call(editDialog, ResumeAfterRelationCreated);

            async Task ResumeAfterRelationCreated(IDialogContext ctx, IAwaitable<RelationEditModel> result)
            {
                var relation = await result;
                _relationRepository.AddRelation(relation.EdgeId, relation.FromNodeId, relation.ToNodeId);
                Init();
                await StartAsync(ctx);
            }
        }

        private void RelationSelected(string str, IDialogContext ctx)
        {
            throw new NotImplementedException();
        }

    }
}