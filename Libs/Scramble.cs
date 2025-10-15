using System.Security.Cryptography;
namespace Libs;

public class Scramble
{
    private Random random = new Random();
    private string _salt = "";
    private bool _addSalt = false;
    public Scramble(string salt)
    {
        _salt = salt;
    }

    public string HashString(string text)
    {
        byte[] salt = System.Text.Encoding.UTF8.GetBytes(_salt);
        using (var sha256 = new SHA256Managed())
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

            // Concatenate password and salt
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            // Hash the concatenated password and salt
            byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

            string hashed = "";
            if(_addSalt)
            {
            byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
            Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
            Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

                hashed = Convert.ToBase64String(hashedPasswordWithSalt);

            }
            else
            {
                hashed = Convert.ToBase64String(hashedBytes);
            }
            hashed = hashed.Replace("+", "");
            hashed = hashed.Replace("-", "");
            hashed = hashed.Replace("/", "");
            hashed = hashed.Replace("=", "");


            return $"__{hashed}";
        }
    }
}
