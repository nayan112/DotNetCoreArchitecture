using DotNetCore.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreArchitecture.Application
{
    public interface IFileApplicationService
    {
        Task<IEnumerable<BinaryFile>> AddAsync(string directory, IEnumerable<BinaryFile> files);

        Task<BinaryFile> SelectAsync(string directory, Guid id);
    }
}
