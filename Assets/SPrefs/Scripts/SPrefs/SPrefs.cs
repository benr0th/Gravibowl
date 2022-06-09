using UnityEngine;
using System;

public class SPrefs
{
    public const string STRING_SALT = "7Snc1Lso";
    public const string INT_SALT = "t5HqItbY";
    public const string FLOAT_SALT = "ZieZO5cM";
    public const string BOOL_SALT = "E9LvW12n";

    /// <summary>
    /// Sets the value of the preference identified by key
    /// </summary>
    public static void SetString(string key, string value)
    {
        SecureSetString(STRING_SALT + key, value);
    }

    /// <summary>
    /// Returns the value corresponding to key in the preference file if it exists<para/>
    /// Default value is the empty string
    /// </summary>
    public static string GetString(string key)
    {
        return GetString(key, "");
    }

    /// <summary>
    /// Returns the value corresponding to key in the preference file if it exists
    /// </summary>
    public static string GetString(string key, string defaultValue)
    {
        if (!SecureHasKey(STRING_SALT + key))
        {
            return defaultValue;
        }

        try
        {
            return SecureGetString(STRING_SALT + key);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Sets the value of the preference identified by key
    /// </summary>
    public static void SetInt(string key, int value)
    {
        SecureSetString(INT_SALT + key, value.ToString());
    }

    /// <summary>
    /// Returns the value corresponding to key in the preference file if it exists<para/>
    /// Default value: 0
    /// </summary>
    public static int GetInt(string key)
    {
        return GetInt(key, 0);
    }

    /// <summary>
    /// Returns the value corresponding to key in the preference file if it exists
    /// </summary>
    public static int GetInt(string key, int defaultValue)
    {
        return GetIntCustomSalt(INT_SALT, key, defaultValue);
    }

    private static int GetIntCustomSalt(string salt, string key, int defaultValue)
    {
        if (!SecureHasKey(salt + key))
        {
            return defaultValue;
        }

        string result = "";

        try
        {
            result = SecureGetString(salt + key);

            if (result.Length < 1)
            {
                return defaultValue;
            }

            return int.Parse(result);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Sets the value of the preference identified by key
    /// </summary>
    public static void SetFloat(string key, float value)
    {
        SecureSetString(FLOAT_SALT + key, value.ToString());
    }

    /// <summary>
    /// Returns the value corresponding to key in the preference file if it exists<para/>
    /// Default value: 0
    /// </summary>
    public static float GetFloat(string key)
    {
        return GetFloat(key, 0);
    }

    /// <summary>
    /// Returns the value corresponding to key in the preference file if it exists
    /// </summary>
    public static float GetFloat(string key, float defaultValue)
    {
        if (!SecureHasKey(FLOAT_SALT + key))
        {
            return defaultValue;
        }

        string result = "";

        try
        {
            result = SecureGetString(FLOAT_SALT + key);

            if (result.Length < 1)
            {
                return defaultValue;
            }

            return float.Parse(result);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Quick access for bools (uses GetInt)
    /// </summary>
    public static bool GetBool(string key)
    {
        return GetBool(key, false);
    }

    /// <summary>
    /// Quick access for bools (uses GetInt)
    /// </summary>
    public static bool GetBool(string key, bool defaultValue)
    {
        return Convert.ToBoolean(GetIntCustomSalt(BOOL_SALT, key, Convert.ToInt32(defaultValue)));
    }

    /// <summary>
    /// Quick access for bools (uses GetInt)
    /// </summary>
    public static void SetBool(string key, bool value)
    {
        SecureSetString(BOOL_SALT + key, Convert.ToInt32(value).ToString());
    }

    /// <summary>
    /// Removes all keys and values from the preferences. Use with caution
    /// </summary>
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// Removes key and its corresponding value from the preferences
    /// </summary>
    public static void DeleteKey(string key)
    {
        SecureDeleteKey(STRING_SALT + key);
        SecureDeleteKey(INT_SALT + key);
        SecureDeleteKey(FLOAT_SALT + key);
        SecureDeleteKey(BOOL_SALT + key);
    }

    /// <summary>
    /// Writes all modified preferences to disk
    /// </summary>
    public static void Save()
    {
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Returns true if key exists in the preferences
    /// </summary>
    public static bool HasKey(string key)
    {
        return SecureHasKey(STRING_SALT + key) || SecureHasKey(INT_SALT + key) || SecureHasKey(FLOAT_SALT + key) || SecureHasKey(BOOL_SALT + key);
    }


    //-----------------------------------------------//
    //--------------------Magic----------------------//
    //-----------------------------------------------//

    private static void SecureSetString(string key, string value)
    {
        PlayerPrefs.SetString(Cryptor.Hash(key), Cryptor.Encrypt(value));
    }

    private static string SecureGetString(string key)
    {
        return Cryptor.Decrypt(PlayerPrefs.GetString(Cryptor.Hash(key)));
    }

    private static bool SecureHasKey(string key)
    {
        return PlayerPrefs.HasKey(Cryptor.Hash(key));
    }

    private static void SecureDeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(Cryptor.Hash(key));
    }

}
