using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using ProtoBuf.Grpc;

namespace BlazorWasmGrpcWithAuth0.Shared.Calculator
{
    [ServiceContract(Name = "Hyper.Calculator")]
    public interface ICalculator
    {
        [OperationContract]
        ValueTask<MultiplyResult> MultiplyAsync(MultiplyRequest request, CallContext context = default);
    }

    [DataContract]
    public class MultiplyRequest
    {
        [DataMember(Order = 1)]
        public int X { get; set; }

        [DataMember(Order = 2)]
        public int Y { get; set; }
    }

    [DataContract]
    public class MultiplyResult
    {
        [DataMember(Order = 1)]
        public int Result { get; set; }
    }
}