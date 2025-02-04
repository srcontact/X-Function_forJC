using clrev01.ClAction.ObjectSearch;
using clrev01.Programs.FuncPar;
using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace clrev01.Programs
{
    public interface VariableValueBase
    {
        public string Name { get; set; }
        public string ValueText { get; }
        public bool usedFlag { get; set; }
    }

    public interface IVariableValueNumeric : VariableValueBase
    {
        public float GetNumericValue(int index = 0);
        public void SetNumericValue(float v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace);
    }

    public interface IVariableValueVector3D : VariableValueBase
    {
        public Vector3 GetVector3DValue(int index = 0);
        public void SetVector3DValue(Vector3 v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace);
    }

    public interface IVariableValueLockOn : VariableValueBase
    {
        public ObjectSearchTgt GetLockOnValue(int index = 0);
        public void SetLockOnValue(ObjectSearchTgt v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace);
    }

    public class VariableValueNumeric : IVariableValueNumeric
    {
        private float _value;
        public string Name { get; set; }
        public string ValueText => "\n  " + _value;
        public bool usedFlag { get; set; }
        public float GetNumericValue(int index = 0)
        {
            usedFlag = true;
            return _value;
        }
        public void SetNumericValue(float v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace)
        {
            usedFlag = true;
            _value = v;
        }
    }

    public class VariableValueVector3 : IVariableValueVector3D
    {
        private Vector3 _value;
        public string Name { get; set; }
        public string ValueText => "\n  " + _value;
        public bool usedFlag { get; set; }
        public Vector3 GetVector3DValue(int index = 0)
        {
            usedFlag = true;
            return _value;
        }
        public void SetVector3DValue(Vector3 v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace)
        {
            usedFlag = true;
            _value = v;
        }
    }

    public class VariableValueLockOn : IVariableValueLockOn
    {
        private ObjectSearchTgt _value;
        public string Name { get; set; }
        public string ValueText => _value is null ? "-" : _value.name;
        public bool usedFlag { get; set; }
        public ObjectSearchTgt GetLockOnValue(int index = 0)
        {
            usedFlag = true;
            return _value;
        }
        public void SetLockOnValue(ObjectSearchTgt v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace)
        {
            usedFlag = true;
            _value = v;
        }
    }

    public class VariableValueNumericList : IVariableValueNumeric
    {
        private List<float> _value = new();
        public List<float> Value
        {
            get
            {
                usedFlag = true;
                return _value;
            }
            private set
            {
                usedFlag = true;
                _value = value;
            }
        }
        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();
        public string Name { get; set; }
        public string ValueText
        {
            get
            {
                if (Value == null) return null;
                sb.Clear();
                for (var i = 0; i < Value.Count; i++)
                {
                    sb.AppendLine();
                    sb.Append($"  {i:00} : {Value[i]}");
                }
                return sb.ToString();
            }
        }
        public bool usedFlag { get; set; }
        public float GetNumericValue(int index = 0)
        {
            Value ??= new List<float>();
            if (index >= 0 && index < Value.Count) return Value[index];
            return default;
        }
        public void SetNumericValue(float v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace)
        {
            Value ??= new List<float>();
            if (addToListMethodType is AddToListMethodType.Insert or AddToListMethodType.Replace)
            {
                if (index < 0) return;
                if (Value.Count <= index)
                {
                    while (Value.Count <= index)
                    {
                        Value.Add(0);
                    }
                    Value[index] = v;
                    return;
                }
            }
            switch (addToListMethodType)
            {
                case AddToListMethodType.Add:
                    Value.Add(v);
                    break;
                case AddToListMethodType.UniqueAdd:
                    if (!Value.Contains(v)) Value.Add(v);
                    break;
                case AddToListMethodType.Insert:
                    if (index >= 0 && index < Value.Count) Value.Insert(index, v);
                    break;
                case AddToListMethodType.Replace:
                    if (index >= 0 && index < Value.Count) Value[index] = v;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(addToListMethodType), addToListMethodType, null);
            }
        }
    }

    public class VariableValueVector3DList : IVariableValueVector3D
    {
        private List<Vector3> _value = new();
        public List<Vector3> Value
        {
            get
            {
                usedFlag = true;
                return _value;
            }
            private set
            {
                usedFlag = true;
                _value = value;
            }
        }
        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();
        public string Name { get; set; }
        public string ValueText
        {
            get
            {
                if (Value == null) return null;
                sb.Clear();
                for (var i = 0; i < Value.Count; i++)
                {
                    sb.AppendLine();
                    sb.Append($"  {i:00} : {Value[i]}");
                }
                return sb.ToString();
            }
        }
        public bool usedFlag { get; set; }
        public Vector3 GetVector3DValue(int index = 0)
        {
            Value ??= new List<Vector3>();
            if (index >= 0 && index < Value.Count) return Value[index];
            return default;
        }
        public void SetVector3DValue(Vector3 v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace)
        {
            Value ??= new List<Vector3>();
            if (addToListMethodType is AddToListMethodType.Insert or AddToListMethodType.Replace)
            {
                if (index < 0) return;
                if (Value.Count <= index)
                {
                    while (Value.Count <= index)
                    {
                        Value.Add(new Vector3());
                    }
                    Value[index] = v;
                    return;
                }
            }
            switch (addToListMethodType)
            {
                case AddToListMethodType.Add:
                    Value.Add(v);
                    break;
                case AddToListMethodType.UniqueAdd:
                    if (!Value.Contains(v)) Value.Add(v);
                    break;
                case AddToListMethodType.Insert:
                    if (index >= 0 && index < Value.Count) Value.Insert(index, v);
                    break;
                case AddToListMethodType.Replace:
                    if (index >= 0 && index < Value.Count) Value[index] = v;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(addToListMethodType), addToListMethodType, null);
            }
        }
    }

    public class VariableValueLockOnList : IVariableValueLockOn
    {
        private List<ObjectSearchTgt> _value = new();
        public List<ObjectSearchTgt> Value
        {
            get
            {
                usedFlag = true;
                return _value;
            }
            private set
            {
                usedFlag = true;
                _value = value;
            }
        }

        private Utf8ValueStringBuilder sb = ZString.CreateUtf8StringBuilder();
        public string Name { get; set; }
        public string ValueText
        {
            get
            {
                if (Value == null) return null;
                sb.Clear();
                for (var i = 0; i < Value.Count; i++)
                {
                    sb.AppendLine();
                    var name = Value?[i]?.name ?? "-";
                    sb.Append($"  {i:00} : {name}");
                }
                return sb.ToString();
            }
        }
        public bool usedFlag { get; set; }
        public ObjectSearchTgt GetLockOnValue(int index = 0)
        {
            Value ??= new List<ObjectSearchTgt>();
            if (index >= 0 && index < Value.Count) return Value[index];
            return default;
        }
        public void SetLockOnValue(ObjectSearchTgt v, int index = 0, AddToListMethodType addToListMethodType = AddToListMethodType.Replace)
        {
            Value ??= new List<ObjectSearchTgt>();
            if (addToListMethodType is AddToListMethodType.Insert or AddToListMethodType.Replace)
            {
                if (index < 0) return;
                if (Value.Count <= index)
                {
                    while (Value.Count <= index)
                    {
                        Value.Add(null);
                    }
                    Value[index] = v;
                    return;
                }
            }
            switch (addToListMethodType)
            {
                case AddToListMethodType.Add:
                    Value.Add(v);
                    break;
                case AddToListMethodType.UniqueAdd:
                    if (!Value.Contains(v)) Value.Add(v);
                    break;
                case AddToListMethodType.Insert:
                    if (index >= 0 && index < Value.Count) Value.Insert(index, v);
                    break;
                case AddToListMethodType.Replace:
                    if (index >= 0 && index < Value.Count) Value[index] = v;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(addToListMethodType), addToListMethodType, null);
            }
        }
    }
}