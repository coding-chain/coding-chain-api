using System;
using System.Threading.Tasks;
using Application.Read.Functions;

namespace Application.Read.Contracts
{
    public interface IReadFunctionRepository
    {
        public Task<FunctionNavigation?> GetById(Guid id, bool includeDeleted);
        public Task<Guid?> GetLastEditorIdById(Guid id, bool includeDeleted);
        
    }
}