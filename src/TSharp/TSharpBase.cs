using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace TSharp
{
    public static class View
    {
        delegate void ProcedureDelegate();
        public static void Set(string options)
        {
            string[] args = options.Split(',');
            foreach (string arg in args)
            {
                switch (arg.Split(':')[0])
                {
                    case "graphics":
                        Window.current.Invoke(new ProcedureDelegate(() =>
                        {
                            Window.current.Size = new Size(int.Parse(arg.Split(':')[1].Split(';')[0]), int.Parse(arg.Split(';')[1]));
                        }));
                        break;
                    case "title":
                        Window.current.Invoke(new ProcedureDelegate(() =>
                        {
                            Window.current.Text = arg.Split(':')[1].Split(';')[0];
                        }));
                        break;
                    case "position":
                        Window.current.Invoke(new ProcedureDelegate(() =>
                        {
                            Window.current.Location = new Point(int.Parse(arg.Split(':')[1].Split(';')[0]), int.Parse(arg.Split(';')[1]));
                        }));
                        break;
                }
            }
        }
    }
    public static class Window
    {
        delegate void ProcedureDelegate();
        public static void Close()
        {
            current.Invoke(new ProcedureDelegate(() => current.Close()));
        }
        static List<Form> windows = new List<Form>();
        static int currentWindow = -1;
        public static Form current
        {
            get
            {
                return windows[currentWindow];
            }
        }
        static void FormPaint(object sender, PaintEventArgs e)
        {
            //in this method we need to prevent the window form redrawing in some way
        }
        public static void Open(string args)
        {
            int oldNumWindows = windows.Count;
            var thread = new Thread(new ThreadStart(() =>
            {
                Form form = new Form();
                currentWindow = windows.Count;
                Draw.graphics = form.CreateGraphics();
                //Draw.graphics.ScaleTransform(1.0f, -1.0f);
                //Draw.graphics.TranslateTransform(0.0F, -form.Size.Height+39);
                windows.Add(form);
                windows[currentWindow].Paint += FormPaint;
                Application.Run((Form)form);
            }));
            thread.Start();
            while (windows.Count == oldNumWindows) Thread.Sleep(10);
            currentWindow = windows.Count - 1;
            View.Set(args);
        }
    }
    public static class Draw
    {
        public static Graphics graphics;
        delegate void ProcedureDelegate();
        static Color backgroundColor;
        static Color foregroundColor;
        static public void FillBox(int x1, int y1, int x2, int y2, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.FillRectangle(new SolidBrush(color), x1, y1, x2 - x1, y2 - y1);
            }));
        }
        static public void Box(int x1, int y1, int x2, int y2, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.DrawRectangle(new Pen(color), x1, y1, x2 - x1, y2 - y1);
            }));
        }
        static public void FillOval(int x, int y, int xRadius, int yRadius, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.FillEllipse(new SolidBrush(color), x, y, xRadius, yRadius);
            }));
        }
        static public void Oval(int x, int y, int xRadius, int yRadius, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.DrawEllipse(new Pen(color), x, y, xRadius, yRadius);
            }));
        }
        static public void Cls()
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.Clear(backgroundColor);
            }));
        }
        static public void Dot(int x, int y, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.DrawPolygon(new Pen(color), new Point[] { new Point(x, y) });
            }));
        }
        static public void Line(int x1, int y1, int x2, int y2, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.DrawLine(new Pen(color), new Point(x1, y1), new Point(x2, y2));
            }));
        }
        static public void DashedLine(int x1, int y1, int x2, int y2, int lineStyle, Color color)
        {
            throw new NotImplementedException();//We need to figure out a good way to implement these
        }
        static public void ThickLine(int x1, int y1, int x2, int y2, int thickness, Color color)
        {
            throw new NotImplementedException();//Know how to implement this, but it's a little more difficult
        }
        static public void Arc(int x, int y, int xRadius, int yRadius, int initialAngle, int finalAngle, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.DrawArc(new Pen(color), x, y, xRadius, yRadius, initialAngle, finalAngle);
            }));
        }
        static public void FillArc(int x, int y, int xRadius, int yRadius, int initialAngle, int finalAngle, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.FillPie(new SolidBrush(color), x, y, xRadius, yRadius, initialAngle, finalAngle);
            }));
        }
        static public void Polygon(int[] x, int[] y, int n, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                Point[] points = new Point[n];
                for (int i = 0; i < n; i++) points[i] = new Point(x[i], y[i]);
                graphics.DrawPolygon(new Pen(color), points);
            }));
        }
        static public void FillPolygon(int[] x, int[] y, int n, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                Point[] points = new Point[n];
                for (int i = 0; i < n; i++) points[i] = new Point(x[i], y[i]);
                graphics.FillPolygon(new SolidBrush(color), points);
            }));
        }
        static public void Fill(int x, int y, Color fillColor, Color borderColor)
        {
            throw new NotImplementedException();//need to find get pixel and draw pixel functions
        }
        static public void Text(string text, int x, int y, int fontID, Color color)
        {
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                graphics.DrawString(text, Font.fonts[fontID], new SolidBrush(color), new Point(x, y));
            }));
        }

    }
    public static class Font
    {
        static public List<System.Drawing.Font> fonts = new List<System.Drawing.Font>();
        public static int New(string fontSelectStr)
        {
            string[] selectStrSplit = fontSelectStr.Split(':');
            string family = selectStrSplit[0];
            int size = int.Parse(selectStrSplit[1]);
            string style = "regular";
            if (selectStrSplit.Length > 2)
                style = selectStrSplit[2].ToLower();
            FontStyle fontStyle;
            switch (style)
            {
                case "regular":
                    fontStyle = FontStyle.Regular;
                    break;
                case "bold":
                    fontStyle = FontStyle.Bold;
                    break;
                case "italic":
                    fontStyle = FontStyle.Italic;
                    break;
                case "underline":
                    fontStyle = FontStyle.Underline;
                    break;
                case "strikeout":
                    fontStyle = FontStyle.Strikeout;
                    break;
                default:
                    throw new NotImplementedException();//sorry that style is not supported. At this time the font class doesn't support multiple styles
            }
            System.Drawing.Font font = new System.Drawing.Font(family, size, fontStyle);
            fonts.Add(font);
            return fonts.Count - 1;
        }
        public static void Free(int fontID)
        {
            fonts[fontID].Dispose();
            fonts.RemoveAt(fontID);
        }
        public static void Draw(string txtStr, int x, int y, int fontID, Color color)
        {
            TSharp.Draw.Text(txtStr, x, y, fontID, color);
        }
        public static int Width(string txtStr, int fontID)
        {
            return (int)TSharp.Draw.graphics.MeasureString(txtStr, fonts[fontID]).Width;
        }
        public static void Size(int fontID, ref int height, ref int ascent, ref int descent, ref int internalLeading)
        {
            height = fonts[fontID].Height;
            ascent = fonts[fontID].FontFamily.GetCellAscent(fonts[fontID].Style);
            descent = fonts[fontID].FontFamily.GetCellDescent(fonts[fontID].Style);
            internalLeading = -1;//this part is not yet implemented
        }
        public static string Name(int fontID)
        {
            return fonts[fontID].Name;
        }
        static FontFamily[] fontFamilies;
        static int fontFamilyIndex;
        public static void StartName()
        {
            fontFamilies = FontFamily.Families;
            fontFamilyIndex = 0;
        }
        public static string GetName()
        {
            if (fontFamilyIndex >= fontFamilies.Length)
                return "";
            return fontFamilies[fontFamilyIndex++].Name;
        }
        public static void GetStyle(string fontName, ref bool bold, ref bool italic, ref bool underline)
        {
            FontFamily fontFamily = new FontFamily(fontName);
            bold = fontFamily.IsStyleAvailable(FontStyle.Bold);
            italic = fontFamily.IsStyleAvailable(FontStyle.Italic);
            underline = fontFamily.IsStyleAvailable(FontStyle.Underline);
        }
        public static void GetStyle(string fontName, ref bool bold, ref bool italic, ref bool underline, ref bool strikeout)
        {
            FontFamily fontFamily = new FontFamily(fontName);
            bold = fontFamily.IsStyleAvailable(FontStyle.Bold);
            italic = fontFamily.IsStyleAvailable(FontStyle.Italic);
            underline = fontFamily.IsStyleAvailable(FontStyle.Underline);
            strikeout = fontFamily.IsStyleAvailable(FontStyle.Strikeout);
        }
        public static void StartSize(string fontFamily, string fontStyle)
        {
            throw new NotImplementedException();//need to implement the available sizes for fonts
        }
        public static int GetSize()
        {
            throw new NotImplementedException();//need to implement the available sizes for fonts
        }
    }
    public static class Pic
    {
        static Color? transparentColor = null;
        static List<Image> images = new List<Image>();
        delegate void ProcedureDelegate();
        public static int New(int x1, int y1, int x2, int y2)
        {
            Image newImage = null;

            images.Add(newImage);
            return images.Count - 1;
        }
        public static void Draw(int picID, int x, int y, int mode)
        {
            if (mode != 0)
                throw new NotImplementedException();//Only currently support picCopy
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                TSharp.Draw.graphics.DrawImage(images[picID], new Point(x, y));
            }));
        }
        public static void DrawSpecial(int picID, int x, int y, int mode, int transition, int duration)
        {
            if (mode != 0)
                throw new NotImplementedException();//Only currently support picCopy
            throw new NotImplementedException();//Don't support special drawing yet
        }
        public static void DrawSpecialBack(int picID, int x, int y, int mode, int transition, int duration)
        {
            if (mode != 0)
                throw new NotImplementedException();//Only currently support picCopy
            throw new NotImplementedException();//Don't support special drawing yet
        }
        public static void Free(int picID)
        {
            images[picID].Dispose();
            images.RemoveAt(picID);
        }
        public static int FileNew(string fileName)
        {
            images.Add(Image.FromFile(fileName));
            return images.Count - 1;
        }
        public static void Save(int picID, string fileName)
        {
            images[picID].Save(fileName);
        }
        public static void ScreenLoad(string fileName, int x, int y, int mode)
        {
            if (mode != 0)
                throw new NotImplementedException();//Only currently support picCopy
            Window.current.Invoke(new ProcedureDelegate(() =>
            {
                TSharp.Draw.graphics.DrawImage(Image.FromFile(fileName), new Point(x, y));
            }));
        }
        public static void ScreenSave(int x1, int y1, int x2, int y2, string fileName)
        {
            throw new NotImplementedException();//can't copy from the screen yet
        }
        public static int Rotate(int picID, int angle, int x, int y)
        {
            Image newImage = (Image)images[picID].Clone();
            switch (angle)
            {
                case 90:
                    newImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 180:
                    newImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 270:
                    newImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case 0:
                    newImage.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    break;
                default:
                    newImage.Dispose();
                    throw new NotImplementedException();//can't rotate images yet unless it's 90, 180 or 270 degrees
            }
            images.Add(newImage);
            return images.Count - 1;
        }
        public static int Scale(int picID, int newWidth, int newHeight)
        {
            throw new NotImplementedException();//don't know how to scale an image yet, might have to develop a new way
            Image newImage = (Image)images[picID].Clone();
            images.Add(newImage);
            return images.Count - 1;
        }
        public static int Flip(int picID)
        {
            Image newImage = (Image)images[picID].Clone();
            newImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            images.Add(newImage);
            return images.Count - 1;
        }
        public static int Mirror(int picID)
        {
            Image newImage = (Image)images[picID].Clone();
            newImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            images.Add(newImage);
            return images.Count - 1;
        }
        public static int Blend(int picID1, int picID2, int pct)
        {
            throw new NotImplementedException();//need to directly access pixel data to do this
        }
        public static int Blur(int picID, int blurAmount)
        {
            throw new NotImplementedException();//need to directly access pixel data to do this
        }
        public static int Width(int picID)
        {
            return images[picID].Width;
        }
        public static int Height(int picID)
        {
            return images[picID].Height;
        }
        public static int Frames(int picID)
        {
            if (images[picID].FrameDimensionsList.Length == 0)
                return 1;//is not a multi-frame picture
            return images[picID].GetFrameCount(new System.Drawing.Imaging.FrameDimension(images[picID].FrameDimensionsList[0]));
        }
        public static int Frames(string pathName)
        {
            Image gifImage = Image.FromFile(pathName);
            int frameCount = 1;//image by default is not considered multi-frame
            if (gifImage.FrameDimensionsList.Length > 0)
                frameCount = gifImage.GetFrameCount(new System.Drawing.Imaging.FrameDimension(gifImage.FrameDimensionsList[0]));
            gifImage.Dispose();
            return frameCount;
        }
        public static void FileNewFrames(string pathName, ref int[] picIds, ref int delayTime)
        {
            throw new NotImplementedException();//need to figure out how to deal with frames
        }
        public static void DrawFrames(int[] picIds, int x, int y, int mode, int numFrames, int delayBetweenFrames, bool eraseAfter)
        {
            throw new NotImplementedException();//need to figure out how to deal with frames
        }
        public static void DrawFramesBack(int[] picIds, int x, int y, int mode, int numFrames, int delayBetweenFrames, bool eraseAfter)
        {
            throw new NotImplementedException();//need to figure out how to deal with frames
        }
        public static void SetTransparentColor(Color color)
        {
            transparentColor = color;
        }
        public static void SetTransparentColor(int colorNumber)
        {
            throw new NotImplementedException();//need to create this method with Turing's old way of handling colors
        }
        public static void SetTransparentColour(Color color)
        {
            SetTransparentColor(color);
        }
        public static void SetTransparentColour(int colorNumber)
        {
            SetTransparentColor(colorNumber);
        }
    }
    public static class Dir
    {
        public static string Current()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }
        public static void Change(string directoryPathName)
        {
            System.IO.Directory.SetCurrentDirectory(directoryPathName);
        }

    }
    public static class File
    {
        public static bool Exists(string pathName)
        {
            return System.IO.File.Exists(pathName);
        }
    }
    public static class Music
    {
        public static void PlayFileReturn(string fileName)
        {
            throw new NotImplementedException();
        }

    }
    public static class Rand
    {
        static Random rand = new Random();
        public static int Int(int low, int high)
        {
            return rand.Next(low, high);
        }
        public static void Set(int seed)
        {
            rand = new Random(seed);
        }
        public static float Real()
        {
            return (float)rand.NextDouble();
        }
    }
    public static class Input
    {
        public static void Pause()
        {
            throw new NotImplementedException();
        }
    }
    public partial class TuringDotNet
    {
        static int picCopy = 0;
        static int picXor = 1;
        static int picMerge = 2;
        static int picUnderMerge = 3;
        static List<object> streams = new List<object>();
        protected static void locate(int row, int column)
        {
            throw new NotImplementedException();
        }
        protected static void putPart(params object[] items)
        {
            throw new NotImplementedException();
        }
        protected static void colour(int colNumber)
        {
            throw new NotImplementedException();
        }
        protected static void color(int colNumber) { colour(colNumber); }
        protected static string skip
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        protected static void open(ref int fileNumberVar, string fileName, string mode)
        {
            if (mode == "r")
                streams.Add(new System.IO.StreamReader(fileName).BaseStream);
            else if (mode == "w")
                streams.Add(new System.IO.StreamWriter(fileName).BaseStream);
            fileNumberVar = streams.Count - 1;
        }
        protected static bool eof(int streamNumber)
        {
             return ((System.IO.StreamReader)streams[streamNumber]).EndOfStream;
        }
        protected static void close(int streamNumber)
        {
            (streams[streamNumber] as System.IO.TextReader).Close();
        }
        protected static void cls()
        {
            Draw.Cls();
        }
        protected static int strint(string s)
        {
            return int.Parse(s);
        }
        protected static int upper<T>(List<T> obj)
        {
            return obj.Count()-1;
        }
        protected static int length(string s)
        {
            return s.Length;
        }
        protected static int floor(float r)
        {
            return (int)Math.Floor(r);
        }
        protected static int round(float r)
        {
            return (int)Math.Round(r);
        }
        protected static int abs(int expn)
        {
            return Math.Abs(expn);
        }
        protected static float pow(float expression, float exponent)
        {
            return (float)Math.Pow(expression, exponent);
        }
        protected static float abs(float expn)
        {
            return Math.Abs(expn);
        }
        protected static void put(params object[] items)
        {
            foreach(object item in items)
            Console.Write(item);
            Console.WriteLine();
        }
        protected static void get(ref string variable)
        {
            variable = Console.ReadLine();
        }
        protected static void get(ref int variable)
        {
            variable = int.Parse(Console.ReadLine());
        }
        protected static void get(ref float variable)
        {
            variable = float.Parse(Console.ReadLine());
        }
        protected static void get(int streamNumber, ref string variable)
        {
            variable = ((System.IO.StreamReader)streams[streamNumber]).ReadLine();
        }
        protected static void get(int streamNumber, ref int variable)
        {
            variable = int.Parse(((System.IO.StreamReader)streams[streamNumber]).ReadLine());
        }
        protected static void setscreen(string options)
        {
            View.Set(options);
        }
        protected static void drawfillbox(int x1, int y1, int x2, int y2, Color color)
        {
            Draw.FillBox(x1, y1, x2, y2, color);
        }
        protected static void drawbox(int x1, int y1, int x2, int y2, Color color)
        {
            Draw.Box(x1, y1, x2, y2, color);
        }
        protected class Quit : Exception { public Quit(string message) : base(message) { } };
        protected static void quit(char guiltyParty, int quitReason)
        {
            throw new Quit(String.Format("Quit #{0}", quitReason));
        }
        protected static void quit(int quitReason)
        {
            quit(' ', quitReason);
        }
        protected static void quit()
        {
            quit(1);
        }
        delegate void ProcedureDelegate();
        static void OldMain()
        {
            List<int> nums = new List<int>();
            nums.Add(3);
            nums.Add(1);
            nums.Add(2);
            nums.Sort();
            put(nums[0],nums[1],nums[2]);
            put("awesome");
            put("hello ",98, " ", 4.55345," ", ConsoleColor.Black);
            Window.Open("graphics:400;600,position:700;400,title:hello");
            drawfillbox(100, 100, 200, 200, Color.Black);
            Draw.FillOval(100, 100, 25, 25, Color.Blue);
            Draw.FillArc(50, 50, 100, 100, 0, 180, Color.Red);
            put("hello ", 98, " ", 4.55345, " ", ConsoleColor.Black);
            Console.ReadKey();
            //setscreen("graphics:800;900");
            drawfillbox(0, 0, 100, 100, Color.Black);
            Console.ReadKey();
            //quit();
            Window.Close();
        }
    }
}
