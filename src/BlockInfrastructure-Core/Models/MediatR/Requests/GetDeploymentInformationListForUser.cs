using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Core.Models.Internal;
using MediatR;

namespace BlockInfrastructure.Core.Models.MediatR.Requests;

public class GetDeploymentInformationListForUser : IRequest<List<DeploymentLog>>
{
    public ContextUser ContextUser { get; set; }
}