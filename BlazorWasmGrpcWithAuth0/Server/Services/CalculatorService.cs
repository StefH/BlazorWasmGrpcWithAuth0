using System.Threading.Tasks;
using BlazorWasmGrpcWithAuth0.Shared.Calculator;
using Microsoft.AspNetCore.Authorization;
using ProtoBuf.Grpc;

namespace BlazorWasmGrpcWithAuth0.Server.Services
{
    [Authorize]
    public class CalculatorService : ICalculator
    {
        ValueTask<MultiplyResult> ICalculator.MultiplyAsync(MultiplyRequest request, CallContext context = default)
        {
            var result = new MultiplyResult { Result = request.X * request.Y };
            return new ValueTask<MultiplyResult>(result);
        }
    }
}
