using System.Security.Cryptography;
using System;
using System.Text;

public class Cryptor
{
    // Please replace this key (32 chars, [a-z, 0-9])
    private const string ENCRYPTION_KEY = "471f58d72fd273ds47c49f9e481cabe3";

    public static string Hash(string value)
    {
        return GetSha1Hash(value);
    }

    public static string Encrypt(string value)
    {
        if(value == null || value.Length < 1)
        {
            return "";
        }

        return GetEncryptedString(value);
    }

    public static string Decrypt(string value)
    {
        if (value == null || value.Length < 1)
        {
            return "";
        }

        return GetDecryptedString(value);
    }

    private static string GetSha1Hash(string strToEncrypt)
    {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
        SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        byte[] hashBytes = sha1.ComputeHash(bytes);
        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }

    private static string GetEncryptedString(string toEncrypt)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
        RijndaelManaged rijn = new RijndaelManaged();
        rijn.Key = keyArray;
        rijn.Mode = CipherMode.ECB;
        rijn.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rijn.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private static string GetDecryptedString(string toDecrypt)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
        byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
        RijndaelManaged rijn = new RijndaelManaged();
        rijn.Key = keyArray;
        rijn.Mode = CipherMode.ECB;
        rijn.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rijn.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
}
