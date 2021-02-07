using System.Threading;
using System.Threading.Tasks;

namespace TasqR.TestProject.Test3
{
    public class Test3Model
    {
        public int StartNumber { get; set; }
    }

    public class CommandWithAsyncWithoutReturn : ITasq
    {
        public CommandWithAsyncWithoutReturn(Test3Model model)
        {
            Model = model;
        }

        public Test3Model Model { get; }
    }

    public class CommandWithAsyncWithoutReturnHandler : TasqHandlerAsync<CommandWithAsyncWithoutReturn>
    {
        public async override Task InitializeAsync(CommandWithAsyncWithoutReturn tasq, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => Thread.Sleep(2000));
        }

        public async override Task RunAsync(CommandWithAsyncWithoutReturn process, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => Thread.Sleep(2000));

            process.Model.StartNumber = process.Model.StartNumber + 1;
        }
    }
}
