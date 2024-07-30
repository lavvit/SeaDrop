namespace SeaDrop
{
    public class Setting
    {
        public static Dictionary<string, string> ConfigList = [];
        public static List<Setting> SettingList = [];

        public static void Load(string path, char splitpart = '=')
        {
            ConfigList.Clear();
            foreach (string line in Text.Read(path))
            {
                string[] split = line.Split(splitpart);
                if (split.Length < 2) continue;
                string key = split[0], value = split[1];
                ConfigList.Add(key, value);
            }
        }

        #region Items

        public string Name = "", Description = "";
        public bool Selected;
        public ESetting Type;
        public ESetType ValueType;

        public Setting() { }

        public Setting(string name, string desc)
        {
            Name = name;
            Description = desc;
        }

        /*public virtual void Init()
        {
            
        }
        public virtual void Set(object value)
        {
            
        }*/
        public virtual void Reset(bool isdefault)
        {

        }

        public virtual void Toggle()
        {

        }
        public virtual void Up()
        {

        }
        public virtual void Down()
        {

        }

        public void SetValue(bool up = true)
        {
            if (ValueType == ESetType.Value)
            {
                if (up) Up();
                else Down();
            }
            else Toggle();
        }

        public virtual object? GetValue()
        {
            return null;
        }

        public virtual int GetIndex()
        {
            return -1;
        }

        public virtual void SetIndex(int value)
        {
        }

        public override string ToString()
        {
            return $"{Name}\n{Description}";
        }

        #endregion
    }


    public class SBool : Setting
    {
        public bool Value, Default;
        private bool Preview;

        public SBool(string name, string desc, bool value, bool? defaultvalue = null)
        {
            Name = name;
            Description = desc;

            Value = value;
            Preview = value;
            if (defaultvalue != null) Default = default;

            Type = ESetting.Bool;
            ValueType = ESetType.Toggle;
        }

        public override void Reset(bool isdefault)
        {
            Value = isdefault ? Default : Preview;
        }

        public override void Toggle()
        {
            Value = !Value;
        }

        public override object? GetValue()
        {
            return Value;
        }
        public override int GetIndex()
        {
            return Value ? 1 : 0;
        }

        public override void SetIndex(int value)
        {
            Value = value > 0;
        }

        public override string ToString()
        {
            return $"{Name}\n{Description}\nValue:{Value} (default:{Default})";
        }
    }


    public class SInt : Setting
    {
        public int Value, Default;
        private int Preview, Scale = 1;
        private int? Min = null, Max = null;

        public SInt(string name, string desc, int value, int? defaultvalue = null, int? min = null, int? max = null, int scale = 1)
        {
            Name = name;
            Description = desc;

            Value = value;
            Preview = value;
            if (defaultvalue != null) Default = default;
            Min = min;
            Max = max;
            Scale = scale;

            Type = ESetting.Bool;
            ValueType = ESetType.Toggle;
        }

        public override void Reset(bool isdefault)
        {
            Value = isdefault ? Default : Preview;
        }

        public override void Toggle()
        {
            Value++;
        }

        public override void Up()
        {
            Value += Scale;
            if (Max.HasValue && Value > Max.Value) Value = ValueType == ESetType.ValueLoop && Min.HasValue ? Min.Value : Max.Value;
        }

        public override void Down()
        {
            Value -= Scale;
            if (Min.HasValue && Value < Min.Value) Value = ValueType == ESetType.ValueLoop && Max.HasValue ? Max.Value : Min.Value;
        }

        public override object? GetValue()
        {
            return Value;
        }
        public override int GetIndex()
        {
            return Value;
        }

        public override void SetIndex(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            string range = Max.HasValue || Min.HasValue ? $"\nRange:{(Min.HasValue ? Min.Value : "")} - {(Max.HasValue ? Max.Value : "")}" : "";
            return $"{Name}\n{Description}\nValue:{Value} (default:{Default})";
        }
    }

    public enum ESetting
    {
        none,
        Bool,
        Int,
    }
    public enum ESetType
    {
        Toggle,
        Value,
        ValueLoop,
    }
}