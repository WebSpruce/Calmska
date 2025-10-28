namespace Calmska.Api.Interfaces;

public interface IModule
{
    void RegisterEndpoints(IEndpointRouteBuilder app);
}