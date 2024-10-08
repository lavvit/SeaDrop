﻿using System.Drawing;
using static DxLibDLL.DX;

namespace SeaDrop
{
    /// <summary>
    /// 画像クラス。
    /// </summary>
    public class Texture : IDisposable
    {
        public static double DefaultScale = 1.0;
        public bool Enable;
        public string Path = "";
        public int ID;

        public int Width, Height;
        public double Scale = 1.0, Angle;
        public (double X, double Y)? XYScale = null;
        public Point? Center = null;
        public ReferencePoint ReferencePoint;
        public Rectangle? Rectangle;
        public BlendMode Blend = BlendMode.None;
        public int BlendDepth = 255;
        public Color Color = Color.White;
        public Color AddColor = Color.Black;

        public bool TurnX, TurnY;

        public int ScaleWidth
        {
            get
            {
                return (int)(Width * (XYScale?.X ?? Scale) * DefaultScale);
            }
        }
        public int ScaleHeight
        {
            get
            {
                return (int)(Height * (XYScale?.Y ?? Scale) * DefaultScale);
            }
        }

        public Texture() { }

        /// <summary>
        /// 画像データを生成します。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public Texture(string path)
        {
            Path = path;
            if (!File.Exists(Path)) return;
            ID = LoadGraph(path);
            Enable = ID > 0;

            if (Width == 0) GetGraphSize(ID, out Width, out Height);
        }

        public void Set(string subpath)
        {
            if (!Enable)
            {
                Path = subpath;
                ID = LoadGraph(subpath);
                Enable = ID > 0;

                if (Width == 0) GetGraphSize(ID, out Width, out Height);
            }
        }

        ~Texture()
        {
            Dispose();
        }
        public void Dispose()
        {
            DeleteGraph(ID);
            Enable = false;
            ID = -1;
        }

        public static void Init()
        {
            InitGraph();
        }

        public void Draw(double x, double y)
        {
            if (!Enable) return;

            SetDrawBlendMode((int)Blend, BlendDepth);

            SetDrawMode((XYScale != null && (XYScale?.X != 1.0 || XYScale?.Y != 1.0)) || Scale * DefaultScale != 1.0D ?
                DX_DRAWMODE_BILINEAR : DX_DRAWMODE_NEAREST);

            SetDrawBright(Color.R, Color.G, Color.B);
            SetDrawAddColor(AddColor.R, AddColor.G, AddColor.B);

            Point point = Center != null ? Center.Value : Point(Rectangle);
            float fx = (float)(x * DefaultScale);
            float fy = (float)(y * DefaultScale);
            if (Rectangle.HasValue) DrawRect(fx, fy);
            else if (XYScale.HasValue)
            {
                DrawRotaGraph3F(fx, fy, point.X, point.Y, XYScale.Value.X * DefaultScale, XYScale.Value.Y * DefaultScale, Angle * Math.PI, ID, TRUE, TurnX ? 1 : 0, TurnY ? 1 : 0);
            }
            else
            {
                DrawRotaGraph2F(fx, fy, point.X, point.Y, Scale * DefaultScale, Angle * Math.PI, ID, TRUE, TurnX ? 1 : 0, TurnY ? 1 : 0);
            }

            SetDrawBlendMode(0, 0);
            SetDrawBright(255, 255, 255);
            SetDrawAddColor(0, 0, 0);
        }
        public void DrawExtend(double x, double y, double width, double height)
        {
            if (!Enable) return;

            Point point = Center != null ? Center.Value : Point(Rectangle);
            float fx = (float)(x * DefaultScale);
            float fy = (float)(y * DefaultScale);
            float x1 = fx - point.X;
            float y1 = fy - point.Y;
            float x2 = x1 + (float)(width * DefaultScale);
            float y2 = y1 + (float)(height * DefaultScale);
            DrawExtendGraphF(x1, y1, x2, y2, ID, TRUE);
        }
        public void DrawRect(double x, double y)
        {
            if (!Enable || !Rectangle.HasValue) return;
            Point point = Center != null ? Center.Value : Point(Rectangle);
            float fx = (float)(x * DefaultScale);
            float fy = (float)(y * DefaultScale);
            if (XYScale.HasValue) DrawRectRotaGraph3F(fx, fy,
                Rectangle.Value.X, Rectangle.Value.Y, Rectangle.Value.Width, Rectangle.Value.Height,
                point.X, point.Y, XYScale.Value.X * DefaultScale, XYScale.Value.Y * DefaultScale, Angle * Math.PI, ID, TRUE, TurnX ? 1 : 0, TurnY ? 1 : 0);
            else DrawRectRotaGraph2F((float)x, (float)y,
                Rectangle.Value.X, Rectangle.Value.Y, Rectangle.Value.Width, Rectangle.Value.Height,
                point.X, point.Y, Scale * DefaultScale, Angle * Math.PI, ID, TRUE, TurnX ? 1 : 0, TurnY ? 1 : 0);
        }

        public void SetRectangle(int x, int y, int width, int height)
        {
            if (width < 0) width = Width;
            if (height < 0) height = Height;
            Rectangle = new Rectangle(x, y, width, height);
        }

        public void SetCenter(double x, double y)
        {
            Center = new Point((int)x, (int)y);
        }

        public void SetOpacity(double opacity)
        {
            BlendDepth = (int)(255.0 * opacity);
        }

        private Point Point(Rectangle? rectangle = null)
        {
            if (!rectangle.HasValue) rectangle = new Rectangle(0, 0, Width, Height);
            Point point = new Point();
            switch (ReferencePoint)
            {
                case ReferencePoint.TopLeft:
                    point.X = 0;
                    point.Y = 0;
                    break;

                case ReferencePoint.TopCenter:
                    point.X = rectangle.Value.Width / 2;
                    point.Y = 0;
                    break;

                case ReferencePoint.TopRight:
                    point.X = rectangle.Value.Width;
                    point.Y = 0;
                    break;

                case ReferencePoint.CenterLeft:
                    point.X = 0;
                    point.Y = rectangle.Value.Height / 2;
                    break;

                case ReferencePoint.Center:
                    point.X = rectangle.Value.Width / 2;
                    point.Y = rectangle.Value.Height / 2;
                    break;

                case ReferencePoint.CenterRight:
                    point.X = rectangle.Value.Width;
                    point.Y = rectangle.Value.Height / 2;
                    break;

                case ReferencePoint.BottomLeft:
                    point.X = 0;
                    point.Y = rectangle.Value.Height;
                    break;

                case ReferencePoint.BottomCenter:
                    point.X = rectangle.Value.Width / 2;
                    point.Y = rectangle.Value.Height;
                    break;

                case ReferencePoint.BottomRight:
                    point.X = rectangle.Value.Width;
                    point.Y = rectangle.Value.Height;
                    break;

                default:
                    point.X = 0;
                    point.Y = 0;
                    break;
            }
            return point;
        }

        public void SetColor(int r = 255, int g = 255, int b = 255, double a = 1.0)
        {
            Color = Color.FromArgb((int)(255 * a), r < 256 ? r : 255, g < 256 ? g : 255, b < 256 ? b : 255);
            AddColor = Color.FromArgb(255, r > 255 ? r - 256 : 0, g > 255 ? g - 256 : 0, b > 255 ? b - 256 : 0);
        }
        public void SetColor((int r, int g, int b, int a) color)
        {
            Color = Color.FromArgb(color.a < 256 ? color.a : 255, color.r < 256 ? color.r : 255, color.g < 256 ? color.g : 255, color.b < 256 ? color.b : 255);
            AddColor = Color.FromArgb(255, color.r > 255 ? color.r - 256 : 0, color.g > 255 ? color.g - 256 : 0, color.b > 255 ? color.b - 256 : 0);
        }
        public void SetAddColor(int r = 255, int g = 255, int b = 255, double a = 1.0)
        {
            AddColor = Color.FromArgb((int)(255 * a), r < 256 ? r : 255, g < 256 ? g : 255, b < 256 ? b : 255);
        }

        public override string ToString()
        {
            if (!Enable) return $"{Path}";
            string size = $"{Width}*{Height}";
            if (Rectangle.HasValue) size = $"Rec:{Rectangle.Value.X},{Rectangle.Value.Y} {Rectangle.Value.Width}*{Rectangle.Value.Height}";
            string turn = "";
            if (TurnX) { turn += "RevX"; if (TurnY) turn += " "; }
            if (TurnY) turn += "RevY";
            return $"{Path} {size} {Scale:0.0}x {Angle:0.0}rot {ReferencePoint} {turn} {Blend}:{BlendDepth}";
        }

        public static bool IsEnable(Texture? texture)
        {
            return texture != null && texture.Enable;
        }

        public static (int Width, int Height) GetSize(string path)
        {
            if (!File.Exists(path)) return (0, 0);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Seek(16, SeekOrigin.Begin);
            byte[] buf = new byte[8];
            fs.Read(buf, 0, 8);
            fs.Dispose();
            uint width = ((uint)buf[0] << 24) | ((uint)buf[1] << 16) | ((uint)buf[2] << 8) | (uint)buf[3];
            uint height = ((uint)buf[4] << 24) | ((uint)buf[5] << 16) | ((uint)buf[6] << 8) | (uint)buf[7];
            return ((int)width, (int)height);
        }

        public static int GetCount(string dir, string prefix = "", string ext = ".png")
        {
            int num = 0;
            while (File.Exists(dir + prefix + num + ext))
            {
                num++;
            }
            return num;
        }
    }

    public enum BlendMode
    {
        /// <summary>
        /// なし
        /// </summary>
        None = DX_BLENDMODE_ALPHA,

        /// <summary>
        /// 加算合成
        /// </summary>
        Add = DX_BLENDMODE_ADD,

        /// <summary>
        /// 減算合成
        /// </summary>
        Subtract = DX_BLENDMODE_SUB,

        /// <summary>
        /// 乗算合成
        /// </summary>
        Multiply = DX_BLENDMODE_MULA,

        /// <summary>
        /// 反転合成
        /// </summary>
        Reverse = DX_BLENDMODE_INVSRC,

        PMAAlpha = DX_BLENDMODE_PMA_ALPHA,
        PMAAdd = DX_BLENDMODE_PMA_ADD,
        PMASubtract = DX_BLENDMODE_PMA_SUB,
        PMAReverse = DX_BLENDMODE_PMA_INVSRC,

    }
    public enum ReferencePoint
    {
        /// <summary>
        /// 左上
        /// </summary>
        TopLeft,

        /// <summary>
        /// 中央上
        /// </summary>
        TopCenter,

        /// <summary>
        /// 右上
        /// </summary>
        TopRight,

        /// <summary>
        /// 左中央
        /// </summary>
        CenterLeft,

        /// <summary>
        /// 中央
        /// </summary>
        Center,

        /// <summary>
        /// 右中央
        /// </summary>
        CenterRight,

        /// <summary>
        /// 左下
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 中央下
        /// </summary>
        BottomCenter,

        /// <summary>
        /// 右下
        /// </summary>
        BottomRight
    }
}
