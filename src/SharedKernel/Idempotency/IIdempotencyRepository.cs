namespace SharedKernel.Idempotency;

public interface IIdempotencyRepository
{
    Task<IdempotencyEntry?> GetAsync(string key, CancellationToken cancellationToken);
    Task<bool> TryCreateAsync(string key, string request, CancellationToken cancellationToken);
    Task SaveResultAsync(string key, string result, CancellationToken cancellationToken);
    Task RemoveAsync(string key, CancellationToken cancellationToken);
}
