using System;
using System.Security.Cryptography;
using System.Text;

namespace MegaManager.Utilities
{
    public class Cypher
    {

        private byte[] salt = [0x26, 0xdc, 0x1f, 0x2a, 0x38, 0x40, 0xad, 0x21];

        // Метод для генерации ключа из мастер-пароля
        private byte[] GenerateKey(string password)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                return deriveBytes.GetBytes(32); // 32 байта для AES-256
            }
        }

        // Метод для шифрования текста
        public string Encrypt(string plainText, string password)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] key = GenerateKey(password);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Метод для дешифрования текста
        public string Decrypt(string cipherText, string password)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] key = GenerateKey(password);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;

                    byte[] iv = new byte[aesAlg.BlockSize / 8];
                    Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                    aesAlg.IV = iv;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream())
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                        {
                            csDecrypt.Write(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
                            csDecrypt.FlushFinalBlock();
                        }

                        return Encoding.UTF8.GetString(msDecrypt.ToArray());
                    }
                }
            }
            catch (FormatException ex)
            {
                // Логирование ошибки декодирования Base64 строки
                Console.WriteLine($"Ошибка декодирования Base64 строки: {ex.Message}");
                throw; // Пробрасываем исключение дальше для обработки
            }
            catch (CryptographicException ex)
            {
                // Логирование ошибки дешифрования (например, если неверный ключ или IV)
                Console.WriteLine($"Ошибка при дешифровании: {ex.Message}");
                return "Неправильно подобран Мастер-пароль";
            }
            catch (Exception ex)
            {
                // Другие исключения, которые могут возникнуть при дешифровании
                Console.WriteLine($"Ошибка при дешифровании: {ex.Message}");
                throw;
            }
        }
    }
}
