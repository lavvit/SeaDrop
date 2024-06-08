using static DxLibDLL.DX;
namespace SeaDrop
{
    public class Sound : IDisposable
    {
        public static Dictionary<string, int> IDs = [];

        public static void Load(string path)
        {
            if (IDs.ContainsKey(path)) return;
            IDs.Add(path, LoadSoundMem(path));
        }
        public static void Dispose(string path)
        {
            if (!IDs.ContainsKey(path)) return;
            DeleteSoundMem(IDs[path]);
            IDs.Remove(path);
        }

        public bool Enable;
        public string Path;
        public int ID;

        public Sound(string path)
        {
            Path = path;
            Load(path);
            ID = -1;
            IDs.TryGetValue(path, out ID);
            Enable = ID >= 0;
        }

        public void Set(string subpath)
        {
            if (!Enable)
            {
                Path = subpath;
                Load(subpath);
                ID = -1;
                IDs.TryGetValue(subpath, out ID);
                Enable = ID >= 0;
            }
        }
        ~Sound()
        {
            Dispose();
        }
        public void Dispose()
        {
            Stop();
            Dispose(Path);
            ID = -1;
            Enable = false;
        }

        public void Play()
        {
            PlaySoundMem(ID, DX_PLAYTYPE_BACK);
        }
        public void PlayWait()
        {
            PlaySoundMem(ID, DX_PLAYTYPE_NORMAL);
        }
        public void PlayLoop()
        {
            PlaySoundMem(ID, DX_PLAYTYPE_LOOP);
        }

        public void Stop()
        {
            StopSoundMem(ID);
        }

        public void SetPan(int value)
        {
            Pan = value;
            ChangePanSoundMem(value, ID);
        }
        public void SetVolume(int value)
        {
            Volume = value;
        }
        public void NextPan(int value)
        {
            ChangeNextPlayPanSoundMem(value, ID);
        }
        public void NextVolume(int value)
        {
            ChangeNextPlayVolumeSoundMem(value, ID);
        }

        public override string ToString()
        {
            if (!Enable) return $"{Path}";
            string play = Playing ? "Play" : "Stop";
            string pan = "C";
            if (Pan < 0) pan = "L";
            if (Pan > 0) pan = "R";
            pan += $"{Math.Abs(Pan)}";
            //long start, end;
            return $"{System.IO.Path.GetFileName(Path)} {play} {Time}/{Length} {Frequency}hz {pan} vol{Volume} {Loop}->{LoopStart}";
        }


        public bool Playing { get { return CheckSoundMem(ID) > 0; } }
        public int Pan
        {
            get { return GetPanSoundMem(ID); }
            set
            {
                ChangePanSoundMem(value, ID);
            }
        }
        public int Volume
        {
            get { return GetVolumeSoundMem2(ID); }
            set
            {
                ChangeVolumeSoundMem(value, ID);
            }
        }
        public int Frequency
        {
            get
            {
                var freq = GetFrequencySoundMem(ID);
                if (freq > 0) return freq;
                var sample = GetSoundTotalSample(ID);
                var len = GetSoundTotalSample(ID);
                return sample > -1 ? (int)((double)sample / len) : 44100;
            }
            set
            {
                if (value < 0) ResetFrequencySoundMem(ID);
                else SetFrequencySoundMem(value, ID);
            }
        }

        public int Loop
        {
            get { GetLoopAreaTimePosSoundMem(out long s, out long e, ID); return (int)s; }
            set
            {
                SetLoopTimePosSoundMem(value, ID);
            }
        }
        public int LoopStart
        {
            get { GetLoopAreaTimePosSoundMem(out long s, out long e, ID); return (int)e; }
            set
            {
                SetLoopStartTimePosSoundMem(value, ID);
            }
        }

        public int Time
        {
            get
            {
                try
                {
                    var time = GetSoundCurrentTime(ID);
                    return (int)time;
                }
                catch (Exception) { return 0; }
            }
            set
            {
                try
                {
                    SetSoundCurrentTime(value, ID);
                }
                catch (Exception) { }
            }
        }
        public int Length
        {
            get
            {
                int n = 0;
                while (n < 1000)
                {
                    int l = (int)GetSoundTotalTime(ID);
                    if (!Enable || l >= 0) return l;
                    Thread.Sleep(1);
                    n++;
                }
                return 0;
            }
        }
    }
}
