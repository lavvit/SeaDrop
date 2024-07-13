namespace SeaDrop
{
    public class Settings
    {
        public Dictionary<string, string> AllKeys = [];

        public Settings() { }
        public Settings(string path)
        {
            Read(path);
        }

        public void Read(string path)
        {
            foreach (var line in Text.Read(path, false))
            {
                string[] spl = line.Split('=');
                if (spl.Length > 1)
                {
                    var spls = spl.ToList();
                    spls.RemoveAt(0);
                    string value = string.Join("=", spls);

                    AllKeys.Add(spl[0], value);
                }
            }
        }

        public string Get(string key)
        {
            return AllKeys[key];
        }
    }
}
