using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BeeGraph.Editor.Dialogs;

namespace BeeGraph.Editor
{
    public static class DialogLocator
    {
        private static readonly Lazy<RootDialog> _rootDialog;
        private static readonly Lazy<ManageDialog> _manageDialog;
        private static readonly Lazy<NodeManagementDialog> _nodeManagementDialog;

        static DialogLocator()
        {
            _rootDialog = new Lazy<RootDialog>(() => EditorContainer.Container.GetInstance<RootDialog>());
            _manageDialog = new Lazy<ManageDialog>(() => EditorContainer.Container.GetInstance<ManageDialog>());
            _nodeManagementDialog = new Lazy<NodeManagementDialog>(() => EditorContainer.Container.GetInstance<NodeManagementDialog>());
        }

        public static RootDialog RootDialog => _rootDialog.Value;
        public static ManageDialog ManageDialog => _manageDialog.Value;

        public static NodeManagementDialog NodeManagementDialog => _nodeManagementDialog.Value;
    }
}