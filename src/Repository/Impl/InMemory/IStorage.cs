// Decompiled with JetBrains decompiler
// Type: Microsoft.Bot.Builder.IStorage
// Assembly: Microsoft.Bot.Builder, Version=4.4.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: D5F02FD0-ECC9-4458-BDDD-DF2C80E4BF71
// Assembly location: C:\Users\nkhusnullin\.nuget\packages\microsoft.bot.builder\4.4.3\lib\netstandard2.0\Microsoft.Bot.Builder.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Repository.Impl.InMemory
{
    public interface IStorage
    {
        Task<IDictionary<string, T>> ReadAsync<T>(
            string[] keys,
            CancellationToken cancellationToken = default (CancellationToken));

        Task WriteAsync(IDictionary<string, object> changes, CancellationToken cancellationToken = default (CancellationToken));

        Task DeleteAsync(string[] keys, CancellationToken cancellationToken = default (CancellationToken));
    }
}