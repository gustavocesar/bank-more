using SharedKernel.Idempotency;

namespace ContaCorrente.API.Idempotencia;

internal static class IdempotencyEndpointConventionBuilderExtensions
{
    internal static RouteHandlerBuilder RequireIdempotency(this RouteHandlerBuilder builder)
    {
        builder.WithMetadata(new RequireIdempotencyAttribute());
        return builder;
    }
}
