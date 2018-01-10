using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace BeeGraph.Editor.Dialogs
{
    public static class DialogHelper
    {
        public static async void CallDialog<T>(IDialogContext ctx)
            where T : IDialog, new()
        {
            
            var dialog = new T();
            //ctx.Call(dialog, Foo);
            ctx.Call(dialog, Foo);
            //await dialog.StartAsync(ctx);
        }

        private static async Task Foo(IDialogContext context, IAwaitable<object> result)
        {
            var answer = await result;
            context.Done(answer);
        }
    }
}