using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Participations;

namespace Domain.CodeAnalysis
{
    public record SuspectFunctionPlagiarismConfirmed(FunctionId Id) : IDomainEvent;

    public record PlagiarizedFunctionValidityStatus(FunctionId Id, bool? IsValid);

    public class SuspectFunctionAggregate : Aggregate<FunctionId>
    {
        private readonly List<PlagiarizedFunctionEntity> _plagiarizedFunctions;

        private SuspectFunctionAggregate(FunctionId id,
            List<PlagiarizedFunctionEntity> plagiarizedFunctions) :
            base(id)
        {
            Id = id;
            _plagiarizedFunctions = plagiarizedFunctions;
        }


        public static SuspectFunctionAggregate CreateNew(FunctionId id, string hash,
            IList<PlagiarizedFunctionEntity> plagiarizedFunctions)
        {
            var aggregate = new SuspectFunctionAggregate(id, new List<PlagiarizedFunctionEntity>());
            aggregate.SetPlagiarizedFunctions(plagiarizedFunctions);

            return aggregate;
        }

        public static SuspectFunctionAggregate Restore(FunctionId id,
            IList<PlagiarizedFunctionEntity> plagiarizedFunctions)
        {
            return new SuspectFunctionAggregate(id, plagiarizedFunctions.ToList());
        }


        public IReadOnlyList<PlagiarizedFunctionEntity> PlagiarizedFunctions =>
            _plagiarizedFunctions.AsReadOnly();

        public void SetPlagiarizedFunction(PlagiarizedFunctionEntity plagiarizedFunction)
        {
            var existingFunc = _plagiarizedFunctions.FirstOrDefault(pF => pF.Id == plagiarizedFunction.Id);
            if (existingFunc is null)
            {
                _plagiarizedFunctions.Add(plagiarizedFunction);
                return;
            }

            if (existingFunc.IsValid is not null
                && plagiarizedFunction.Hash == existingFunc.Hash
                && plagiarizedFunction.SuspectHash == existingFunc.SuspectHash) return;

            _plagiarizedFunctions.Remove(existingFunc);
            _plagiarizedFunctions.Add(plagiarizedFunction);
        }

        public void SetPlagiarizedFunctions(IEnumerable<PlagiarizedFunctionEntity> plagiarizedFunctions)
        {
            foreach (var plagiarizedFunction in plagiarizedFunctions)
            {
                SetPlagiarizedFunction(plagiarizedFunction);
            }
        }

        public void SetPlagiarizedFunctionsValidities(IList<PlagiarizedFunctionValidityStatus> functions)
        {
            var isInvalid = false;
            var errors = functions.Select(validity =>
                {
                    var plagiarizedFunc = _plagiarizedFunctions.FirstOrDefault(func => func.Id == validity.Id);
                    if (plagiarizedFunc is null)
                        return $"Plagiarized function : {validity.Id} doesn't exist for suspect function {Id}";
                    plagiarizedFunc.IsValid = validity.IsValid;
                    if (validity.IsValid == false)
                    {
                        isInvalid = true;
                    }

                    return null;
                })
                .RemoveNull()
                .ToList();
            if (errors.Any())
                throw new DomainException(errors);
            if (isInvalid)
                RegisterEvent(new SuspectFunctionPlagiarismConfirmed(Id));
        }
    }
}