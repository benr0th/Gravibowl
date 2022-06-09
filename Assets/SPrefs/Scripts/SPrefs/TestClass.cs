using UnityEngine;
using UnityEngine.UI;

public class TestClass : MonoBehaviour {

    public Text output1, output2, output3, output4, output5, output6;

    private const string someKey = "KEY";

    private const string someString = "testString";
    private const int someInt = 152;
    private const float someFloat = -0.001f;

    private const string someCrazyKey = "KEYÖäÜ$!?@";
    private const string someCrazyString = "ÖÄÜ$!@";
    private const int someCrazyInt = int.MinValue;
    private const float someCrazyFloat = 0.00000000000000000001f;
    private const string emptyString = "";
    private const int zeroInt = 0;
    private const float zeroFloat = 0;

    private int errorCount = 0;

    //-----------------------------------------------//
    //------------------Warning!---------------------//
    //-----------------------------------------------//

    // Using this test class will delete all entries 
    // in PlayerPrefs

    // Check the console for the following Debug logs. No logs -> Script works

    //-----------------------------------------------//
    //-----------------------------------------------//
    //-----------------------------------------------//

    void Start ()
    {
        GeneralTest();

        TestKeySystem();
        TestIntStoring();
        TestSPrefsDeleteKey();

        TestDefaultValues();
        TestExtremeValues();

        if(errorCount < 1)
        {
            Debug.Log("All tests succeeded");
            output6.text = "All tests succeeded";
        }
        else
        {
            Debug.Log("Some tests failed. Error count: " + errorCount);
            output6.text = "Some tests failed. Error count: " + errorCount;
        }
    }

    private void GeneralTest()
    {
        string tempString = "";
        SPrefs.SetString(someKey, someString);
        if (SPrefs.GetString(someKey).Equals(someString))
        {
            output1.text = "SPrefs working!";
        }
        else
        {
            output1.text = "Encryption is probably not possible on this platform";
            AddError("SPrefs SetString or GetString not working");
        }

        tempString = Cryptor.Encrypt(someString);
        output2.text = "Original: " + someString;
        output3.text = "Encrypted: " + tempString;
        tempString = Cryptor.Decrypt(tempString);
        output4.text = "Decrypted: " + tempString;
        output5.text = "Cryptor working: " + tempString.Equals(someString);

        if(!tempString.Equals(someString))
        {
            AddError("Crypting not working");
        }
        SPrefs.DeleteAll();
    }

    private void TestKeySystem()
    {
        SPrefs.SetInt(someKey, someInt);
        if (SPrefs.GetString(someKey).Length > 0)
        {
            AddError("key system not ok");
        }
        SPrefs.DeleteAll();
    }

    private void TestIntStoring()
    {
        SPrefs.SetInt(someKey, someInt);
        if(someInt != SPrefs.GetInt(someKey))
        {
            AddError("int was not stored");
        }
        SPrefs.DeleteAll();
    }

    private void TestSPrefsDeleteKey()
    {
        if(SPrefs.HasKey(someKey))
        {
            AddError("HasKey detected wrong key!");
        }

        SPrefs.SetFloat(someKey, someFloat);

        if (!Mathf.Approximately(SPrefs.GetFloat(someKey), someFloat))
        {
            AddError("float was not stored");
        }

        if (!SPrefs.HasKey(someKey))
        {
            AddError("HasKey didn't detect key!");
        }

        SPrefs.SetFloat("example", 1f);
        SPrefs.DeleteKey(someKey);

        if (SPrefs.HasKey(someKey))
        {
            AddError("Key was not deleted!");
        }

        if (!Mathf.Approximately(SPrefs.GetFloat("example"), 1f))
        {
            AddError("Key deletion affects wrong key!");
        }

        SPrefs.DeleteAll();
    }

    // Testing overloads with defaultValue
    private void TestDefaultValues()
    {
        if (!SPrefs.GetString(someKey, "abcd").Equals("abcd"))
        {
            AddError("Default string not returned!");
        }

        if (3 != SPrefs.GetInt(someKey, 3))
        {
            AddError("Default int not returned!");
        }

        if (!Mathf.Approximately(SPrefs.GetFloat(someKey, 2f), 2f))
        {
            AddError("Default float not returned!");
        }

        SPrefs.SetFloat(someKey, someFloat);

        if (!Mathf.Approximately(SPrefs.GetFloat(someKey, 2f), someFloat))
        {
            AddError("Default float overwrite!");
        }

        if (3 != SPrefs.GetInt(someKey, 3))
        {
            AddError("Default int not returned after float set!");
        }

        if(SPrefs.GetBool(someKey))
        {
            AddError("Default bool is not 'false'");
        }

        SPrefs.SetBool(someKey, true);

        if (!SPrefs.GetBool(someKey, false))
        {
            AddError("Default bool overwrite!");
        }

        SPrefs.DeleteAll();
    }

    private void TestExtremeValues()
    {
        SPrefs.SetString(someCrazyKey, someCrazyString);
        if(!someCrazyString.Equals(SPrefs.GetString(someCrazyKey)))
        {
            AddError("String Umlauts problem");
        }

        SPrefs.SetInt(someCrazyKey, someCrazyInt);
        if (someCrazyInt != SPrefs.GetInt(someCrazyKey))
        {
            AddError("Negative int max value not ok");
        }

        SPrefs.SetFloat(someCrazyKey, someCrazyFloat);
        if (!Mathf.Approximately(someCrazyFloat, SPrefs.GetFloat(someCrazyKey)))
        {
            AddError("Problem with very small float numbers");
        }

        SPrefs.DeleteKey(someCrazyKey);

        SPrefs.SetString(someCrazyKey, emptyString);
        if(!emptyString.Equals(SPrefs.GetString(someCrazyKey)))
        {
            AddError("Empty string problem");
        }

        SPrefs.SetInt(someCrazyKey, zeroInt);
        if(zeroInt != SPrefs.GetInt(someCrazyKey))
        {
            AddError("Int zero problem");
        }

        SPrefs.SetFloat(someCrazyKey, zeroFloat);
        if(!Mathf.Approximately(zeroFloat, SPrefs.GetFloat(someCrazyKey)))
        {
            AddError("Float zero problem");
        }

        SPrefs.DeleteKey(someCrazyKey);

        // nullpointers (no errors should be thrown)
        SPrefs.SetString(null, null);
        SPrefs.GetString(null);
        SPrefs.SetInt(null, 0);
        SPrefs.GetInt(null);
        SPrefs.SetFloat(null, 0f);
        SPrefs.GetFloat(null);
        SPrefs.GetBool(null, true);
        SPrefs.GetBool(null);
        SPrefs.DeleteAll();
    }

    private void AddError(string msg)
    {
        Debug.Log(msg);
        errorCount++;
    }
}
