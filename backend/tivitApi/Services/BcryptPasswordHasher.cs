public interface IPasswordHasher
{
	string Hash(string senha);
	bool Verificar(string senha, string hash);
}

public class BcryptPasswordHasher : IPasswordHasher
{
	public string Hash(string senha) =>
		BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 12);

	public bool Verificar(string senha, string hash) =>
		BCrypt.Net.BCrypt.Verify(senha, hash);
}