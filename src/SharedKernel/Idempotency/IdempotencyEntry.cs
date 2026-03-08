namespace SharedKernel.Idempotency;

public sealed record IdempotencyEntry(string Key, string Request, string? Result);
