using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokerSlackAvatar.Repository;

namespace PokerSlackAvatar
{
    class Program
    {
        static void Main(string[] args)
        {
            //setup DI
            var serviceProvider = new ServiceCollection()
                .AddLogging( configure => configure.AddConsole())
                .AddSingleton<ISecrets, Secrets>()
                .AddSingleton<IOperations, Operations>()
                .BuildServiceProvider();

            IOperations ops = serviceProvider.GetService<IOperations>();
            var ids = ops.GetPokerUserSlackIDs();
            ops.GetSlackAvatars(ids).Wait();
        }
    }
}
