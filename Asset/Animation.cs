using System.Data;

namespace SeaDrop
{
    public class Animation
    {
        public Animation() { }

        public Animation(string path) { Load(path); }

        public string Path { get; set; } = "";
        public bool Loop { get; set; } = false;
        public Counter Timer { get; set; } = new();
        private Dictionary<string, double> Values = [];
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
                    switch (split[0].ToLower())
                    {
                        case "loop":
                            Loop = values[0] == "1";
                            break;
                        case "value":
                            if (values.Length > 1 && double.TryParse(values[1], out var v)) Values.Add(values[0], v);
                            break;
                        case "texture":
                            string pt = System.IO.Path.GetDirectoryName(path) + "\\" + values[1];
                            var point = values.Length > 2 && int.TryParse(values[2], out int rp) ? (ReferencePoint)rp : ReferencePoint.TopLeft;
                            Textures.Add(values[0], new Texture(pt)
                            {
                                ReferencePoint = point,
                            });
                            break;
                        case "numtexture":
                            pt = System.IO.Path.GetDirectoryName(path) + "\\" + values[1];
                            var texs = new Texture[Texture.GetCount(pt)];
                            point = values.Length > 2 && int.TryParse(values[2], out rp) ? (ReferencePoint)rp : ReferencePoint.TopLeft;
                            for (int i = 0; i < texs.Length; i++)
                            {
                                texs[i] = new Texture(pt + $"\\{i}.png")
                                {
                                    ReferencePoint = point,
                                };
                            }
                            NumTextures.Add(values[0], texs);
                            break;
                        case "sound":
                            pt = System.IO.Path.GetDirectoryName(path) + "\\" + values[1];
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
                            Parts.Add(new AnimPart()
                            {
                                Type = AnimCommand.TexDraw,
                                Time = (int)t,
                                Key = values[1],
                                sX = values[2],
                                sY = values[3],
                                sOpacity = values.Length > 4 ? values[4] : "",
                                sXSize = values.Length > 5 ? values[5] : "",
                                sYSize = values.Length > 6 ? values[6] : "",
                            });
                            break;
                        case "play":
                            double.TryParse(values[0], out t);
                            Parts.Add(new AnimPart()
                            {
                                Type = AnimCommand.Sound,
                                Time = (int)t,
                                Key = values[1],
                            });
                            break;
                        case "valueset":
                            double.TryParse(values[0], out t);
                            double.TryParse(values[2], out v);
                            double? datalen = values.Length > 3 ? (int)new DataTable().Compute(values[3], "") : null;
                            double? dataval = values.Length > 4 ? (int)new DataTable().Compute(values[4], "") : null;
                            var dataeas = values.Length > 5 ? (EEasing)new DataTable().Compute(values[5], "") : EEasing.Linear;
                            var dataio = values.Length > 6 ? (EInOut)new DataTable().Compute(values[6], "") : EInOut.In;
                            Parts.Add(new AnimPart()
                            {
                                Type = AnimCommand.Value,
                                Time = (int)t,
                                Key = values[1],
                                Value = v,
                                Length = (int?)datalen,
                                ToValue = dataval,
                                Easing = dataeas,
                                InOut = dataio,
                            });
                            break;
                    }
                }
            }
        }

        public void Draw()
        {
            Timer.Tick();
            if (Loop && Timer.Value == Timer.End)
            {
                Start();
            }
            Calc();
            foreach (var item in Parts)
            {
                switch (item.Type)
                {
                    case AnimCommand.DefDraw:
                        if (Timer.State == 0) Textures[item.Key].Draw(item.X, item.Y);
                        break;
                    case AnimCommand.TexDraw:
                        Textures[item.Key].XYScale = (item.XSize, item.YSize);
                        Textures[item.Key].SetOpacity(item.Opacity);
                        if (Timer.State > 0 && Timer.Value > item.Time) Textures[item.Key].Draw(item.X, item.Y);
                        break;
                }
            }
        }

        public void Start(int ch = 0)
        {
            foreach (var item in Parts) item.Hit = false;
            Timer.Reset();
            Timer.Start();
        }

        public virtual string Convert(string text)
        {
            foreach (var item in Values)
            {
                text = text.Replace(item.Key, $"{item.Value}");
            }
            return text;
        }

        public void Calc()
        {
            foreach (var item in Parts)
            {
                if (item.Type == AnimCommand.Value && Timer.State > 0 && Timer.Value > item.Time && !item.Hit)
                {
                    if (item.Length.HasValue)
                    {
                        Values[item.Key] = Easing.EaseL(Timer, item.Time, item.Length.Value,
                            item.Value, item.ToValue.HasValue ? item.ToValue.Value : item.Value, item.Easing, item.InOut);
                    }
                    else Values[item.Key] = item.Value;
                    if (Timer.Value >= (item.Length.HasValue ? item.Length.Value : item.Time)) item.Hit = true;
                }
                if (item.Type == AnimCommand.TexDraw)
                {
                    double datax = item.sX != "" ? Calc(Convert(item.sX)) : 0;
                    double datay = item.sY != "" ? Calc(Convert(item.sY)) : 0;
                    double dataopa = item.sOpacity != "" ? Calc(Convert(item.sOpacity)) : 1;
                    double dataxsca = item.sXSize != "" ? Calc(Convert(item.sXSize)) : 1;
                    double dataysca = item.sYSize != "" ? Calc(Convert(item.sYSize)) : dataxsca;
                    item.X = datax;
                    item.Y = datay;
                    item.XSize = dataxsca;
                    item.YSize = dataysca;
                    item.Opacity = dataopa;
                }
            }
        }
        public double Calc(string text)
        {
            return double.Parse(new DataTable().Compute("1.0 *" + Convert(text), "").ToString() ?? "");
        }
    }


    public class AnimPart
    {
        public AnimCommand Type;
        public int Time;
        public int? Length = null;
        public string Key = "";

        public string sX = "";
        public string sY = "";
        public string sXSize = "";
        public string sYSize = "";
        public string sOpacity = "";

        public double X;
        public double Y;
        public double XSize;
        public double YSize;
        public double Opacity;

        public double Value;
        public double? ToValue = null;
        public EEasing Easing;
        public EInOut InOut;
        public bool Hit;

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
        Value,
    }
}
