using System.Data;

namespace SeaDrop
{
    public class Animation
    {
        public Animation() { }

        public Animation(string path) { Load(path); }

        public string Path { get; set; } = "";
        public Counter Timer { get; set; } = new();
        private Dictionary<string, Texture> Textures = [];
        private Dictionary<string, Texture[]> NumTextures = [];
        private Dictionary<string, Sound> Sounds = [];

        public List<AnimPart> Parts = [];

        public virtual void Load(string path)
        {
            Path = path;
            Textures.Clear();
            NumTextures.Clear();
            Sounds.Clear();
            foreach (var line in Text.Read(path))
            {
                var split = line.Trim().Split(':');
                if (split.Length > 1)
                {
                    var values = split[1].Trim().Split(';');
                    string pt = System.IO.Path.GetDirectoryName(path) + "\\" + values[1];
                    switch (split[0].ToLower())
                    {
                        case "texture":
                            Textures.Add(values[0], new Texture(pt));
                            break;
                        case "numtexture":
                            var texs = new Texture[Texture.GetCount(pt)];
                            for (int i = 0; i < texs.Length; i++)
                            {
                                texs[i] = new Texture(pt + $"\\{i}.png");
                            }
                            NumTextures.Add(values[0], texs);
                            break;
                        case "sound":
                            Sounds.Add(values[0], new Sound(pt));
                            break;

                        case "defaultdraw":
                            Parts.Add(new AnimPart()
                            {
                                Type = AnimCommand.DefDraw,
                                Key = values[0],
                                X = values.Length > 1 && double.TryParse(values[1], out var x) ? x : 0,
                                Y = values.Length > 2 && double.TryParse(values[2], out var y) ? y : 0,
                            });
                            break;
                        case "length":
                            Timer.ChangeEnd(int.Parse(values[0]));
                            break;
                        case "draw":
                            double.TryParse(values[0], out var t);
                            double datax = values.Length > 2 ? (int)new DataTable().Compute(values[2], "") : 0;
                            double datay = values.Length > 3 ? (int)new DataTable().Compute(values[3], "") : 0;
                            Parts.Add(new AnimPart()
                            {
                                Type = AnimCommand.TexDraw,
                                Time = (int)t,
                                Key = values[1],
                                X = datax,
                                Y = datay,
                            });
                            break;
                    }
                }
            }
        }

        public void Draw()
        {
            Timer.Tick();
            foreach (var item in Parts)
            {
                switch (item.Type)
                {
                    case AnimCommand.DefDraw:
                        if (Timer.State == 0) Textures[item.Key].Draw(item.X, item.Y);
                        break;
                    case AnimCommand.TexDraw:
                        if (Timer.State > 0 && Timer.Value > item.Time) Textures[item.Key].Draw(item.X, item.Y);
                        break;

                }
            }
        }

        public void Start(int ch = 0)
        {
            Timer.Reset();
            Timer.Start();
        }
    }


    public class AnimPart
    {
        public AnimCommand Type;
        public int Time;
        public string Key = "";

        public double X;
        public double Y;
        public double Size;

        public int Channel;
    }

    public enum AnimCommand
    {
        DefDraw,
        DefSound,
        TexDraw,
        TexDrawAnim,
        TexNumDraw,
        Sound,
    }
}
