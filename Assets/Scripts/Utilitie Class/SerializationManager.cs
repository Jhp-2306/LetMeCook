using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager 
{
   static string _path = Application.dataPath + "/Resource/saves";
    public static bool Save(string _savename,object _data)
    {
        BinaryFormatter _formatter = GetFormatter();
        //string _path = Application.dataPath + "/Resource/saves";
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
        string _filepath = $"{_path}/{_savename}.lmc";
        FileStream _file = File.Create(_filepath);
        _formatter.Serialize(_file, _data);
        _file.Close();
        CustomLogs.CC_Log($"*** \"{_savename}\" File is saved at \n {_path}\n successfully ***","cyan");
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
        
        SurrogateSelector selector = new SurrogateSelector();
        Vector3SerializationSurrogate vector3SerializationSurrogate = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate quaternionSerializationSurrogate = new QuaternionSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3),new StreamingContext(StreamingContextStates.All),vector3SerializationSurrogate);
        selector.AddSurrogate(typeof(Quaternion),new StreamingContext(StreamingContextStates.All),quaternionSerializationSurrogate);
        _formatter.SurrogateSelector = selector;

        return _formatter;
    }
}

public class Vector3SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 vector= (Vector3)obj;
        info.AddValue("x", vector.x);
        info.AddValue("y",vector.y);
        info.AddValue("z", vector.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 vector = (Vector3)obj;
        vector.x= (float)info.GetValue("x",typeof(float));
        vector.y = (float)info.GetValue("y", typeof(float));
        vector.z= (float)info.GetValue("z",typeof(float));
        obj = vector;
        return obj;
    }
}

public class QuaternionSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Quaternion vector = (Quaternion)obj;
        info.AddValue("x", vector.x);
        info.AddValue("y", vector.y);
        info.AddValue("z", vector.z);
        info.AddValue("w", vector.w);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Quaternion quaternion = (Quaternion)obj;
        quaternion.x = (float)info.GetValue("x", typeof(float));
        quaternion.y = (float)info.GetValue("y", typeof(float));
        quaternion.z = (float)info.GetValue("z", typeof(float));
        quaternion.w = (float)info.GetValue("w", typeof(float));
        obj = quaternion;
        return obj;
    }
}