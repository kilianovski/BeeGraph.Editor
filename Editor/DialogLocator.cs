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

        static DialogLocator()
        {
            _rootDialog = new Lazy<RootDialog>(() => EditorContainer.Container.GetInstance<RootDialog>());
        }

        public static RootDialog RootDialog => _rootDialog.Value;
    }
}