using ContaCorrente.Application.Commands.CriarContaCorrente;
using ContaCorrente.Tests.TestDoubles;

namespace ContaCorrente.Tests.Application.Commands.CriarContaCorrente;

public class CriarContaCorrenteCommandHandlerTests
{
    [Fact]
    public async Task Handler_DeveRetornarDocumentoInvalido_QuandoCpfForInvalido()
    {
        var repository = new FakeContaCorrenteRepository();
        var handler = new CriarContaCorrenteCommandHandler(repository);

        var response = await handler.Handle(new CriarContaCorrenteCommand("123", "Senha@123"), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("INVALID_DOCUMENT", response.TipoFalha);
        Assert.Equal(0, repository.CreateAsyncCalls);
    }

    [Fact]
    public async Task Handler_DeveRetornarContaAtivaJaExistente_QuandoCpfJaPossuirConta()
    {
        var repository = new FakeContaCorrenteRepository
        {
            ExistsByCpfResult = true
        };
        var handler = new CriarContaCorrenteCommandHandler(repository);

        var response = await handler.Handle(new CriarContaCorrenteCommand("52998224725", "Senha@123"), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("ACTIVE_ACCOUNT_ALREADY_EXISTS", response.TipoFalha);
        Assert.Equal(0, repository.CreateAsyncCalls);
    }

    [Fact]
    public async Task Handler_DeveCriarConta_QuandoRequestForValido()
    {
        var repository = new FakeContaCorrenteRepository
        {
            LastNumberResult = 100250
        };
        var handler = new CriarContaCorrenteCommandHandler(repository);

        var response = await handler.Handle(new CriarContaCorrenteCommand("529.982.247-25", "Senha@123"), CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(100251, response.NumeroConta);
        Assert.NotNull(repository.CreatedConta);
        Assert.Equal(100251, repository.CreatedConta!.NumeroConta.Value);
        Assert.Equal("52998224725", repository.CreatedConta.Cpf.Value);
        Assert.True(repository.CreatedConta.Autenticar("Senha@123"));
        Assert.Equal(1, repository.CreateAsyncCalls);
    }
}
