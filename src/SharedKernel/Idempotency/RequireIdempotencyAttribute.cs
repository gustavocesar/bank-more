namespace SharedKernel.Idempotency;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireIdempotencyAttribute : Attribute;
