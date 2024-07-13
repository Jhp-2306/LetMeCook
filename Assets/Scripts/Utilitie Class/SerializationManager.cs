using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager 
{
   static string _path = Application.dataPath + "/Resource/saves";
    public static bool Save(string _savename,object _data)
    {
        BinaryFormatter _formatter = new BinaryFormatter();
        //string _path = Application.dataPath + "/Resource/saves";
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
        string _filepath = $"{_path}/{_savename}.lmc";
        FileStream _file = File.Create(_filepath);
        _formatter.Serialize(_file, _data);
        _file.Close();
        return true;
    }

    public static object Load(string _savename)
    {
        string path = $"{_path}/{_savename}.lmc";
        if (!File.Exists(path))
        {
            return null;
        }
        BinaryFormatter _formatter = GetFormatter();
        FileStream _file = File.Open(path,FileMode.Open);

        try
        {
            object _data = _formatter.Deserialize(_file);
            _file.Close();
            return _data;
        }
        catch
        {
            CustomLogs.CC_Log($"Error:Failed to Load Data At {path}", "red");
            _file.Close();
            return null;
        }

    }
    public static BinaryFormatter GetFormatter()
    {
        BinaryFormatter _formatter = new BinaryFormatter();

        return _formatter;
    }
}
