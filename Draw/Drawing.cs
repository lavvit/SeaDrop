using System.Drawing;
using System.Drawing.Text;
using static DxLibDLL.DX;

namespace SeaDrop
{
    public class Drawing
    {
        public static Handle DebugHandle = new();

        public static void Init()
        {
            DebugHandle = new(SystemFonts.MenuFont != null ? SystemFonts.MenuFont.Name : "", 20, 2, 1, false, EFontType.AntialiasingEdge);
        }

        #region Shape
        public static void Blackout(double opacity = 1.0, int color = 0)
        {
            Box(0, 0, DXLib.Width, DXLib.Height, color, true, 1, opacity);
        }

        public static void Line(double x, double y, double x1, double y1, int color = 0xffffff, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawLineAA((float)x, (float)y, (float)(x + x1), (float)(y + y1), (uint)color, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        public static void LineZ(double x, double y, double x1, double y1, int color = 0xffffff, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawLineAA((float)x, (float)y, (float)x1, (float)y1, (uint)color, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        public static void Box(double x, double y, double x1, double y1, int color = 0xffffff, bool fill = true, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawBoxAA((float)x, (float)y, (float)(x + x1), (float)(y + y1), (uint)color, fill ? 1 : 0, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        public static void BoxZ(double x, double y, double x1, double y1, int color = 0xffffff, bool fill = true, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawBoxAA((float)x, (float)y, (float)x1, (float)y1, (uint)color, fill ? 1 : 0, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        public static void Circle(double x, double y, double r, int color = 0xffffff, bool fill = true, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None, int pos = 255)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawCircleAA((float)x, (float)y, (float)r, pos, (uint)color, fill ? 1 : 0, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        #endregion

        #region Text
        public static void Text(double x, double y, object? str, int color = 0xffffff, Handle? handle = null, int edgecolor = 0, ReferencePoint point = ReferencePoint.TopLeft, double opacity = 1.0, bool vertical = false, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            if (str == null) return;
            if (str.ToString() == null) return;
            if (str.GetType().ToString().Contains("[]"))
            {
                var objects = str as object[];
                Text(x, y, objects, color, handle, edgecolor, point, opacity, vertical, blend);
            }
            else if (str.GetType().ToString().Contains("Generic.List"))
            {
                var objects = (List<string>)(IEnumerable<string>)str;
                for (int i = 0; i < objects.Count; i++)
                {
                    Text(x, y + TextSize(objects[i], -1, handle).Height * i, objects[i], color, handle, edgecolor, point, opacity, vertical, blend);
                }
            }
            else
            {
                drawtext(x, y, str, color, handle, edgecolor, vertical, point);
            }
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        private static void drawtext(double x, double y, object? str, int color, Handle? handle, int edgecolor, bool vertical, ReferencePoint point)
        {
            var po = TextPoint(point, str, -1, handle);
            float x1 = (float)x - po.X, y1 = (float)y - po.Y;
            if (handle != null && handle.Enable) DrawStringFToHandle(x1, y1, str?.ToString(), (uint)color, handle.ID, (uint)edgecolor, vertical ? 1 : 0);
            else DrawStringF(x1, y1, str?.ToString(), (uint)color);
        }

        public static (int Width, int Height, int Count) TextSize(object? str, int length = -1, Handle? handle = null)
        {
            if (str == null) return (0, 0, 0);
            var s = str.ToString();
            if (s == null) return (0, 0, 0);
            if (length < 0) length = s.Length;
            int s1, s2, c;
            if (handle != null && handle.Enable) GetDrawStringSizeToHandle(out s1, out s2, out c, s, length, handle.ID);
            else GetDrawStringSize(out s1, out s2, out c, s, length);
            return (s1, s2, c);
        }
        private static Point TextPoint(ReferencePoint refpoint, object? str, int length = -1, Handle? handle = null)
        {
            var size = TextSize(str, length, handle);
            Point point = new Point();
            switch (refpoint)
            {
                case ReferencePoint.TopLeft:
                    point.X = 0;
                    point.Y = 0;
                    break;

                case ReferencePoint.TopCenter:
                    point.X = size.Width / 2;
                    point.Y = 0;
                    break;

                case ReferencePoint.TopRight:
                    point.X = size.Width;
                    point.Y = 0;
                    break;

                case ReferencePoint.CenterLeft:
                    point.X = 0;
                    point.Y = size.Height / 2;
                    break;

                case ReferencePoint.Center:
                    point.X = size.Width / 2;
                    point.Y = size.Height / 2;
                    break;

                case ReferencePoint.CenterRight:
                    point.X = size.Width;
                    point.Y = size.Height / 2;
                    break;

                case ReferencePoint.BottomLeft:
                    point.X = 0;
                    point.Y = size.Height;
                    break;

                case ReferencePoint.BottomCenter:
                    point.X = size.Width / 2;
                    point.Y = size.Height;
                    break;

                case ReferencePoint.BottomRight:
                    point.X = size.Width;
                    point.Y = size.Height;
                    break;

                default:
                    point.X = 0;
                    point.Y = 0;
                    break;
            }
            return point;
        }

        public static void Text(double x, double y, object? str, Handle? handle, int color = 0xffffff, int edgecolor = 0, bool vertical = false, ReferencePoint point = ReferencePoint.TopLeft, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            Text(x, y, str, color, handle, edgecolor, point, opacity, vertical, blend);
        }
        public static void Text(double x, double y, object[]? str, int color = 0xffffff, Handle? handle = null, int edgecolor = 0, bool vertical = false, ReferencePoint point = ReferencePoint.TopLeft, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            int h = 0;
            for (int i = 0; i < str?.Length; i++)
            {
                Text(x, y + h, str[i], color, handle, edgecolor, point, opacity, vertical, blend);
                h += TextSize(str[i], -1, handle).Height;
            }
        }
        public static void Text(double x, double y, List<object>? str, int color = 0xffffff, Handle? handle = null, int edgecolor = 0, bool vertical = false, ReferencePoint point = ReferencePoint.TopLeft, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            int h = 0;
            for (int i = 0; i < str?.Count; i++)
            {
                Text(x, y + h, str[i], color, handle, edgecolor, point, opacity, vertical, blend);
                h += TextSize(str[i], -1, handle).Height;
            }
        }

        public static void Text(double x, double y, object? str, Texture texture, Handle? handle = null, ReferencePoint point = ReferencePoint.TopLeft, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            var size = TextSize(str?.ToString(), -1, handle);
            var scr = MakeScreen(size.Width, size.Height, TRUE);
            SetDrawScreen(scr);
            SetBackgroundColor(0, 0, 0);
            ClearDrawScreen();

            Text(0, 0, str, handle, 0xffffff, 0, false, point, opacity, blend);
            texture.XYScale = ((double)size.Width / texture.Width, (double)size.Height / texture.Height);
            texture.Blend = BlendMode.Multiply;
            texture.BlendDepth = 255;
            texture.Draw(0, 0);

            //作成したスクリーンの内容を裏画面に描画する
            SetDrawScreen(DX_SCREEN_BACK);
            DrawGraph((int)x, (int)y, scr, TRUE);

            DeleteGraph(scr);
        }
        public static void Text(double x, double y, object? str, Color top, Color bottom, Handle? handle = null, double border = 0, double center = 0.5, ReferencePoint point = ReferencePoint.TopLeft, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            var size = TextSize(str?.ToString(), -1, handle);

            double bd = -Math.Abs((center - 0.5) * 2) + 1;
            double cent = 1 - ((1 - border) * bd);
            int width = size.Width;
            int height = (int)(size.Height * cent);
            int height2 = (int)(size.Height * (1 - cent));
            int mid = size.Height - height - height2;

            var scr1 = MakeScreen(width, height, TRUE);
            var scr2 = MakeScreen(width, mid, TRUE);
            var scr3 = MakeScreen(width, height2, TRUE);
            if (scr1 > -1)
            {
                SetDrawScreen(scr1);
                SetBackgroundColor(0, 0, 0);
                ClearDrawScreen();
                Text(0, 0, str, handle, 0xffffff, 0, false, ReferencePoint.TopLeft, opacity, blend);
            }
            if (scr2 > -1)
            {
                SetDrawScreen(scr2);
                SetBackgroundColor(0, 0, 0);
                ClearDrawScreen();
                Text(0, -height, str, handle, 0xffffff, 0, false, ReferencePoint.TopLeft, opacity, blend);
            }
            if (scr3 > -1)
            {
                SetDrawScreen(scr3);
                SetBackgroundColor(0, 0, 0);
                ClearDrawScreen();
                Text(0, -height - mid, str, handle, 0xffffff, 0, false, ReferencePoint.TopLeft, opacity, blend);
            }

            var po = TextPoint(point, str, -1, handle);
            float x1 = (float)x - po.X, y1 = (float)y - po.Y;

            //作成したスクリーンの内容を裏画面に描画する
            SetDrawScreen(DX_SCREEN_BACK);
            Polygon(scr1, (int)x1, (int)y1, width, height, top, top, top, top);
            Polygon(scr2, (int)x1, (int)y1 + height, width, mid, top, top, bottom, bottom);
            Polygon(scr3, (int)x1, (int)y1 + size.Height - height2, width, height2, bottom, bottom, bottom, bottom);

            DeleteGraph(scr1);
            DeleteGraph(scr2);
            DeleteGraph(scr3);
        }
        public static void Polygon(int handle, int x, int y, int width, int height, Color color1, Color color2, Color color3, Color color4)
        {
            VERTEX[] Vertex = new VERTEX[6];
            int x1 = x; int y1 = y;
            int x2 = x + width; int y2 = y + height;

            // 本体部分は DrawPolygon を使用して上下に赤から青にグラデーションさせながら描画
            Vertex[0].x = x1; Vertex[0].y = y1;
            Vertex[1].x = x2; Vertex[1].y = y1;
            Vertex[2].x = x1; Vertex[2].y = y2;
            Vertex[3].x = x2; Vertex[3].y = y2;

            Vertex[0].u = 0.0f; Vertex[0].v = 0.0f;
            Vertex[1].u = 1.0f; Vertex[1].v = 0.0f;
            Vertex[2].u = 0.0f; Vertex[2].v = 1.0f;
            Vertex[3].u = 1.0f; Vertex[3].v = 1.0f;

            Vertex[0].r = color1.R; Vertex[0].g = color1.G; Vertex[0].b = color1.B; Vertex[0].a = color1.A;
            Vertex[1].r = color2.R; Vertex[1].g = color2.G; Vertex[1].b = color2.B; Vertex[1].a = color2.A;
            Vertex[2].r = color3.R; Vertex[2].g = color3.G; Vertex[2].b = color3.B; Vertex[2].a = color3.A;
            Vertex[3].r = color4.R; Vertex[3].g = color4.G; Vertex[3].b = color4.B; Vertex[3].a = color4.A;

            Vertex[4] = Vertex[2];
            Vertex[5] = Vertex[1];

            DrawPolygon(Vertex, 2, handle, TRUE, TRUE);
        }

        #endregion

        #region Color
        /// <summary>
        /// RGB値からint形式の色を取得します。
        /// </summary>
        /// <returns>色</returns>
        public static int Color(int red, int green, int blue)
        {
            return (int)GetColor(red, green, blue);
        }
        /// <summary>
        /// RGB値からint形式の色を取得します。
        /// </summary>
        /// <returns>色</returns>
        public static int Color((int, int, int) rgb)
        {
            return (int)GetColor(rgb.Item1, rgb.Item2, rgb.Item3);
        }
        public static int Color((int, int, int, int) rgba)
        {
            return (int)GetColor(rgba.Item1, rgba.Item2, rgba.Item3);
        }
        /// <summary>
        /// カラーコードからint形式の色を取得します。
        /// </summary>
        /// <returns>色</returns>
        public static int Color(string color)
        {
            return Color(ColorTranslator.FromHtml(color));
        }
        /// <summary>
        /// Colorからint形式の色を取得します。
        /// </summary>
        /// <returns>色</returns>
        public static int Color(Color color)
        {
            return Color(color.R, color.G, color.B);
        }
        /// <summary>
        /// HSBカラーモデルからint形式の色を取得します。
        /// </summary>
        /// <returns>色</returns>
        public static int Color(double hue, double saturation, double brightness)
        {
            (int, int, int) color = ColorRGB(hue, saturation, brightness);
            return Color(color.Item1, color.Item2, color.Item3);
        }
        /// <summary>
        /// HSBカラーモデルから色を生成します。
        /// </summary>
        /// <returns>色</returns>
        public static (int R, int G, int B) ColorRGB(double hue, double saturation, double brightness)
        {
            double percent = hue / 60.0;
            int max = (int)(255 * brightness);
            int min = max - (int)(max * saturation);
            int m = max - min;
            double d = percent - (int)percent;
            switch ((int)percent)
            {
                case 0:
                default:
                    return (max, (int)(m * d) + min, min);
                case 1:
                    return ((int)(m * (1.0 - d)) + min, max, min);
                case 2:
                    return (min, max, (int)(m * d) + min);
                case 3:
                    return (min, (int)(m * (1.0 - d)) + min, max);
                case 4:
                    return ((int)(m * d) + min, min, max);
                case 5:
                    return (max, min, (int)(m * (1.0 - d)) + min);
            }
        }
        /// <summary>
        /// ゲーミング色を生成します。
        /// </summary>
        /// <returns>色</returns>
        public static (int R, int G, int B) RainbowRGB(Counter timer, int potition = 0, double white = 0, bool reverse = false)
        {
            double percent = ((double)(reverse ? timer.End - timer.Value : timer.Value) / timer.End * 360) + potition;
            return ColorRGB((int)percent % 360, 1.0 - white, 1.0);
        }
        /// <summary>
        /// タイマーの値を参考にゲーミング色を取得します。
        /// </summary>
        /// /// <param name="potition">色相(0~360)</param>
        /// <returns>色</returns>
        public static int Rainbow(Counter timer, int potition = 0, double white = 0, bool reverse = false)
        {
            return Color(RainbowRGB(timer, potition, white, reverse));
        }
        /// <summary>
        /// タイマーの値を参考にゲーミング色を取得します。(Color出力)
        /// </summary>
        /// /// <param name="potition">色相(0~360)</param>
        /// <returns>色</returns>
        public static Color RainbowC(Counter timer, int potition = 0, double white = 0, bool reverse = false)
        {
            return System.Drawing.Color.FromArgb(RainbowRGB(timer, potition, white, reverse).Item1, RainbowRGB(timer, potition, white, reverse).Item2, RainbowRGB(timer, potition, white, reverse).Item3);
        }

        public static Color ReadColor(string color)
        {
            if (color.StartsWith("#"))
            {
                return ColorTranslator.FromHtml(color);
            }
            else if (int.TryParse(color, out int colorx))
            {
                return System.Drawing.Color.FromArgb(colorx);
            }
            else
            {
                string[] ch = color.Split(' ');
                if (color.Contains(' '))
                {
                    int r = 0, g = 0, b = 0, a = 0;
                    if (ch.Length >= 1) int.TryParse(ch[0].Trim(), out r);
                    if (ch.Length >= 2) int.TryParse(ch[1].Trim(), out g);
                    if (ch.Length >= 3) int.TryParse(ch[2].Trim(), out b);
                    if (ch.Length >= 4) int.TryParse(ch[3].Trim(), out a);
                    return System.Drawing.Color.FromArgb(a, r, g, b);
                }
                else
                {
                    return System.Drawing.Color.FromName(color);
                }
            }
        }

        public static Color DColor(int r, int g, int b, int a = 255)
        {
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }
        #endregion

        #region Override
        public static void StartOverride()
        {

        }
        public static void EndOverride()
        {

        }
        #endregion
    }

    public class Handle : IDisposable
    {
        public int ID;
        public bool Enable;
        public string? Font;
        public int Size, Thick, Edge;
        public bool Italic;
        public EFontType Type;

        public Handle(string fontpath, string font, int size = 16, int thick = 1, int edge = 1, bool italic = false, EFontType type = EFontType.Normal)
        {
            AddFont(fontpath);
            Font = font;
            Set(size, thick, edge, italic, type);
        }
        public Handle(string? font = null, int size = 16, int thick = 1, int edge = 1, bool italic = false, EFontType type = EFontType.Normal)
        {
            Font = GetFont(font);
            Set(size, thick, edge, italic, type);
        }
        public Handle(int size, int thick = 1, int edge = 1, bool italic = false, EFontType type = EFontType.Normal)
        {
            Set(size, thick, edge, italic, type);
        }

        public Handle() { }
        ~Handle() { Dispose(); }

        public void Dispose()
        {
            DeleteFontToHandle(ID);
            ID = -1;
            Enable = false;
        }

        public void Set(int size, int thick, int edge, bool italic, EFontType type)
        {
            Size = size;
            Thick = thick;
            Edge = edge;
            Italic = italic;
            Type = type;
            Set();
        }

        public void Set()
        {
            DeleteFontToHandle(ID);
            ID = CreateFontToHandle(Font, Size, Thick, (int)Type, -1, Edge, Italic ? 1 : 0);
            Enable = ID >= 0;
        }

        public static void AddFont(string? path)
        {
            if (File.Exists(path)) AddFontFile(path);
        }

        public static string GetFont(string? font)
        {
            string name = GetFontName();
            if (font == null) return name;
            if (File.Exists(font)) AddFontFile(font);
            else return font;

            PrivateFontCollection pfc = new();
            pfc.AddFontFile(font);

            font = pfc.Families[0].Name;

            return font;
        }
    }

    public enum EFontType
    {
        Normal = DX_FONTTYPE_NORMAL,
        Edge = DX_FONTTYPE_EDGE,
        Antialiasing = DX_FONTTYPE_ANTIALIASING,
        Antialiasing4 = DX_FONTTYPE_ANTIALIASING_4X4,
        Antialiasing8 = DX_FONTTYPE_ANTIALIASING_8X8,
        Antialiasing16 = DX_FONTTYPE_ANTIALIASING_16X16,
        AntialiasingEdge = DX_FONTTYPE_ANTIALIASING_EDGE,
        AntialiasingEdge4 = DX_FONTTYPE_ANTIALIASING_EDGE_4X4,
        AntialiasingEdge8 = DX_FONTTYPE_ANTIALIASING_EDGE_8X8,
        AntialiasingEdge16 = DX_FONTTYPE_ANTIALIASING_EDGE_16X16,
    }
}