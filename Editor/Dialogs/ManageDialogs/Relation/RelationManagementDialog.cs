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
        private Dictionary<string, Action<IDialogContext>> _relationActions;

        private const int DefaultId = Int32.MaxValue;
        private int _currentRelationId = DefaultId;


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
            _currentRelationId = DefaultId;
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

            _relationActions = new Dictionary<string, Action<IDialogContext>>()
            {
                { "Delete this relation!", DeleteRelation },
                { "Go back!", async ctx => await StartAsync(ctx) }
            };
        }

        private async void DeleteRelation(IDialogContext context)
        {
            _relationRepository.Delete(_currentRelationId);
            //await context.PostAsync("Deleted.");
            Init();
            await StartAsync(context);
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
            _currentRelationId = LabelService.GetIdentifier(str);

            var options = _relationActions.Keys;
            PromptDialog.Choice(ctx, RelationActionSelected, options, "What to do with this relation?");

            async Task RelationActionSelected(IDialogContext context, IAwaitable<string> result)
            {
                var action = _relationActions[await result];
                action(context);
            }
        } 
    }
}