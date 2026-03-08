using SharedKernel.Idempotency;

namespace ContaCorrente.API.Idempotency;

internal static class IdempotencyEndpointConventionBuilderExtensions
{
    internal static RouteHandlerBuilder RequireIdempotency(this RouteHandlerBuilder builder)
    {
        builder.WithMetadata(new RequireIdempotencyAttribute());
        return builder;
    }
}
