using PaddleOCRSharp;
using Spire.OCR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static 易用OCR.Form1;

namespace 易用OCR
{
    public partial class Form1 : Form
    {
        public PaddleOCREngine engine;
        public Form1()
        {
            InitializeComponent();
            //engine = CreateOCRParameter();// 这个只能引用一次，否则会出现内存一直增加的问题
        }


        #region SpireOCR
        private void SpireOCR_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            this.openFileDialog1.FileName = "";
            this.openFileDialog1.Filter = "所有文件(*.*)|*.*";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OcrScanner scanner = new OcrScanner();
                scanner.Scan(this.openFileDialog1.FileName);
                scanner.Dispose();
                //textBox1.Text = "SpireOCR识别结果：\r\n" + scanner.Text.ToString().Split("Evaluation")[0];
                textBox1.Text = "SpireOCR识别结果：\r\n" + scanner.Text.ToString();
            }
        }
        #endregion

        #region PaddleOCRSharp
        public PaddleOCREngine CreateOCRParameter()
        {
            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.cpu_math_library_num_threads = 6;//预测并发线程数
            oCRParameter.enable_mkldnn = true;//web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
            oCRParameter.cls = true; //是否执行文字方向分类；默认false
            oCRParameter.det = true;//是否开启方向检测，用于检测识别180旋转
            oCRParameter.use_angle_cls = true;//是否开启方向检测，用于检测识别180旋转
            oCRParameter.det_db_score_mode = true;//是否使用多段线，即文字区域是用多段线还是用矩形，
            oCRParameter.det_db_unclip_ratio = 8.6f;
            oCRParameter.max_side_len = 960;
            OCRModelConfig config = null;
            PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
            return engine;
        }

        private void PaddleOCRSharp_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";

            if (ofd.ShowDialog() != DialogResult.OK) return;
            //MessageBox.Show(ofd.FileName);
            var imagebyte = File.ReadAllBytes(ofd.FileName);

            Bitmap bitmap = new Bitmap(new MemoryStream(imagebyte));

            OCRModelConfig config = null;

            OCRParameter oCRParameter = new OCRParameter();
            //oCRParameter.cpu_math_library_num_threads = 6;//预测并发线程数
            //oCRParameter.enable_mkldnn = true;//web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
            //oCRParameter.cls = true; //是否执行文字方向分类；默认false
            //oCRParameter.det = true;//是否开启方向检测，用于检测识别180旋转
            //oCRParameter.use_angle_cls = true;//是否开启方向检测，用于检测识别180旋转
            //oCRParameter.det_db_score_mode = false;//是否使用多段线，即文字区域是用多段线还是用矩形，
            //oCRParameter.det_db_unclip_ratio = 8.6f;
            //oCRParameter.max_side_len = 960;

            OCRResult ocrResult = new OCRResult();

            //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。    

            PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);

            {

                ocrResult = engine.DetectText(bitmap);

            }

            if (ocrResult != null)

            {

                textBox1.Text = ocrResult.Text;

            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            ScreenSnapshot();
        }
        private void ScreenSnapshot()
        {
            try
            {
                //Screen screen = Screen.AllScreens.FirstOrDefault();//获取当前第一块屏幕(根据需求也可以换其他屏幕)
                //创建需要截取的屏幕区域  
                Rectangle rc = new Rectangle(0, 0, 600, 200);
                //生成截图的位图容器  
                Bitmap bitmap = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
                //GDI+图像画布  
                using (Graphics memoryGrahics = Graphics.FromImage(bitmap))
                {
                    memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);//对屏幕指定区域进行图像复制
                }
                bitmap.Save("d://123.png", ImageFormat.Png);
                //OCR识别
                var imagebyte = File.ReadAllBytes("d://123.png");

                Bitmap bitmap_ = new Bitmap(new MemoryStream(imagebyte));

                OCRModelConfig config = null;

                OCRParameter oCRParameter = new OCRParameter();

                OCRResult ocrResult = new OCRResult();

                //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。    
                PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
                {
                    ocrResult = engine.DetectText(bitmap_);
                }
                if (ocrResult != null)
                {
                    textBox1.Text = ocrResult.Text;
                }
            }
            catch (Exception ex)
            {
                //异常处理
                MessageBox.Show(ex.ToString());
            }
        }

        //鼠标区域截图
        private void button2_Click(object sender, EventArgs e)
        {
            
            CaptureImageTool captureImageTool = new CaptureImageTool();

            captureImageTool.ShowDialog();
            textBox1.Text = captureImageTool.text_ocr;

        }

        #region 鼠标区域截图相关方法
        internal enum SizeGrip
        {
            None = 0,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            All
        }

        internal class OperateObject
        {
            private OperateType _operateType;
            private Color _color;
            private object _data;

            public OperateObject() { }

            public OperateObject(
                OperateType operateType, Color color, object data)
            {
                _operateType = operateType;
                _color = color;
                _data = data;
            }

            public OperateType OperateType
            {
                get { return _operateType; }
                set { _operateType = value; }
            }

            public Color Color
            {
                get { return _color; }
                set { _color = value; }
            }

            public object Data
            {
                get { return _data; }
                set { _data = value; }
            }
        }

        internal enum OperateType
        {
            None = 0,
            DrawRectangle,
            DrawEllipse,
            DrawArrow,
            DrawLine,
            DrawText
        }

        internal class OperateManager : IDisposable
        {
            private List<OperateObject> _operateList;

            private static readonly int MaxOperateCount = 1000;

            public OperateManager()
            {
            }

            public List<OperateObject> OperateList
            {
                get
                {
                    if (_operateList == null)
                    {
                        _operateList = new List<OperateObject>(100);
                    }
                    return _operateList;
                }
            }

            public int OperateCount
            {
                get { return OperateList.Count; }
            }

            public void AddOperate(
                OperateType operateType,
                Color color,
                object data)
            {
                OperateObject obj = new OperateObject(
                    operateType, color, data);
                if (OperateList.Count > MaxOperateCount)
                {
                    OperateList.RemoveAt(0);
                }
                OperateList.Add(obj);
            }

            public bool RedoOperate()
            {
                if (OperateList.Count > 0)
                {
                    OperateList.RemoveAt(OperateList.Count - 1);
                    return true;
                }
                return false;
            }

            public void Clear()
            {
                OperateList.Clear();
            }

            #region IDisposable 成员

            public void Dispose()
            {
                if (_operateList != null)
                {
                    _operateList.Clear();
                    _operateList = null;
                }
            }

            #endregion
        }

        public enum DrawStyle
        {
            None = 0,
            Rectangle,
            Ellipse,
            Arrow,
            Text,
            Line
        }

        internal class DrawTextData
        {
            private string _text;
            private Font _font;
            private Rectangle _textRect;
            private bool _completed;

            public DrawTextData() { }

            public DrawTextData(string text, Font font, Rectangle textRect)
            {
                _text = text;
                _font = font;
                _textRect = textRect;
            }

            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }

            public Font Font
            {
                get { return _font; }
                set { _font = value; }
            }

            public Rectangle TextRect
            {
                get { return _textRect; }
                set { _textRect = value; }
            }

            public bool Completed
            {
                get { return _completed; }
                set { _completed = value; }
            }
        }

        internal class NativeMethods
        {
            public const int WS_EX_TRANSPARENT = 0x00000020;

            [DllImport("user32.dll")]
            public static extern bool ClipCursor(ref RECT lpRect);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetDC(IntPtr ptr);

            [DllImport("user32.dll")]
            public static extern int ReleaseDC(IntPtr hwnd, IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(
                IntPtr hObject, int nXDest, int nYDest, int nWidth,
               int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc,
                TernaryRasterOperations dwRop);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;

                public RECT(int left, int top, int right, int bottom)
                {
                    Left = left;
                    Top = top;
                    Right = right;
                    Bottom = bottom;
                }

                public RECT(Rectangle rect)
                {
                    Left = rect.Left;
                    Top = rect.Top;
                    Right = rect.Right;
                    Bottom = rect.Bottom;
                }

                public Rectangle Rect
                {
                    get
                    {
                        return new Rectangle(
                            Left,
                            Top,
                            Right - Left,
                            Bottom - Top);
                    }
                }

                public Size Size
                {
                    get
                    {
                        return new Size(Right - Left, Bottom - Top);
                    }
                }

                public static RECT FromXYWH(int x, int y, int width, int height)
                {
                    return new RECT(x,
                                    y,
                                    x + width,
                                    y + height);
                }

                public static RECT FromRectangle(Rectangle rect)
                {
                    return new RECT(rect.Left,
                                     rect.Top,
                                     rect.Right,
                                     rect.Bottom);
                }
            }

            public enum TernaryRasterOperations
            {
                SRCCOPY = 0x00CC0020, /* dest = source*/
                SRCPAINT = 0x00EE0086, /* dest = source OR dest*/
                SRCAND = 0x008800C6, /* dest = source AND dest*/
                SRCINVERT = 0x00660046, /* dest = source XOR dest*/
                SRCERASE = 0x00440328, /* dest = source AND (NOT dest )*/
                NOTSRCCOPY = 0x00330008, /* dest = (NOT source)*/
                NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
                MERGECOPY = 0x00C000CA, /* dest = (source AND pattern)*/
                MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest*/
                PATCOPY = 0x00F00021, /* dest = pattern*/
                PATPAINT = 0x00FB0A09, /* dest = DPSnoo*/
                PATINVERT = 0x005A0049, /* dest = pattern XOR dest*/
                DSTINVERT = 0x00550009, /* dest = (NOT dest)*/
                BLACKNESS = 0x00000042, /* dest = BLACK*/
                WHITENESS = 0x00FF0062, /* dest = WHITE*/
            }
        }

        /// <summary>
        /// 鼠标拖拽区域截图
        /// </summary>
        public class CaptureImageTool : Form
        {
            /// <summary>
            /// 遮罩层颜色
            /// </summary>
            private static SolidBrush mask = new SolidBrush(Color.FromArgb(100, 0, 0, 0));
            /// <summary>
            /// 原始屏幕图
            /// </summary>
            private Image ScreenImage;

            #region Fields

            private Image _image;
            private CaptureImageToolColorTable _colorTable;
            private Cursor _selectCursor = Cursors.Default;
            private Cursor _drawCursor = Cursors.Cross;

            private Point _mouseDownPoint;
            private Point _endPoint;
            private bool _mouseDown;
            private Rectangle _selectImageRect;
            private Rectangle _selectImageBounds;
            private bool _selectedImage;
            private SizeGrip _sizeGrip;
            private Dictionary<SizeGrip, Rectangle> _sizeGripRectList;
            private OperateManager _operateManager;
            private List<Point> _linePointList;

            private static readonly Font TextFont =
               new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            //private static readonly string ToolTipStartCapture = "按住左键不放选择截图区域";

            #endregion

            #region Constructors

            public CaptureImageTool()
            {
                Init();
                KeyUp += CaptureImageTool_KeyUp;
            }

            #endregion

            #region Properties

            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public CaptureImageToolColorTable ColorTable
            {
                get
                {
                    if (_colorTable == null)
                    {
                        _colorTable = new CaptureImageToolColorTable();
                    }
                    return _colorTable;
                }
                set
                {
                    _colorTable = value;
                    base.Invalidate();
                    SetControlColorTable();
                }
            }

            private void SetControlColorTable()
            {
                CaptureImageToolColorTable colorTable = ColorTable;
            }

            public Image Image
            {
                get { return _image; }
            }

            public Cursor SelectCursor
            {
                get { return _selectCursor; }
                set { _selectCursor = value; }
            }

            public Cursor DrawCursor
            {
                get { return _drawCursor; }
                set { _drawCursor = value; }
            }

            internal bool SelectedImage
            {
                get { return _selectedImage; }
                set { _selectedImage = value; }
            }

            internal Rectangle SelectImageRect
            {
                get { return _selectImageRect; }
                set
                {
                    _selectImageRect = value;
                    if (!_selectImageRect.IsEmpty)
                    {
                        CalCulateSizeGripRect();
                        base.Invalidate();
                    }
                }
            }

            internal SizeGrip SizeGrip
            {
                get { return _sizeGrip; }
                set { _sizeGrip = value; }
            }

            internal Dictionary<SizeGrip, Rectangle> SizeGripRectList
            {
                get
                {
                    if (_sizeGripRectList == null)
                    {
                        _sizeGripRectList = new Dictionary<SizeGrip, Rectangle>();
                    }
                    return _sizeGripRectList;
                }
            }

            internal OperateManager OperateManager
            {
                get
                {
                    if (_operateManager == null)
                    {
                        _operateManager = new OperateManager();
                    }
                    return _operateManager;
                }
            }

            private DrawStyle DrawStyle
            {
                get { return DrawStyle.None; }
            }

            private List<Point> LinePointList
            {
                get
                {
                    if (_linePointList == null)
                    {
                        _linePointList = new List<Point>(100);
                    }
                    return _linePointList;
                }
            }

            #endregion

            #region Override Methods

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                //toolTip.SetToolTip(this, ToolTipStartCapture);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                Cursor = SelectCursor;
                if (!SelectedImage)
                {
                    this.Invalidate(true);
                    this.Update();
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button == MouseButtons.Left)
                {
                    if (SelectedImage)
                    {
                        if (SizeGrip != SizeGrip.None)
                        {
                            _mouseDown = true;
                            _mouseDownPoint = e.Location;
                            //HideDrawToolsControl();
                            base.Invalidate();
                        }

                        if (DrawStyle != DrawStyle.None)
                        {
                            if (SelectImageRect.Contains(e.Location))
                            {
                                _mouseDown = true;
                                _mouseDownPoint = e.Location;

                                if (DrawStyle == DrawStyle.Line)
                                {
                                    LinePointList.Add(_mouseDownPoint);
                                }
                                ClipCursor(false);
                            }
                        }
                    }
                    else
                    {
                        _mouseDown = true;
                        _mouseDownPoint = e.Location;
                    }
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (_mouseDown)
                {
                    if (!SelectedImage)
                    {
                        SelectImageRect = GetSelectImageRect(e.Location);
                    }
                    else
                    {
                        if (DrawStyle != DrawStyle.None)
                        {
                            _endPoint = e.Location;
                            if (DrawStyle == DrawStyle.Line)
                            {
                                LinePointList.Add(_endPoint);
                            }
                            base.Invalidate();
                        }
                        else if (SizeGrip != SizeGrip.None)
                        {
                            ChangeSelctImageRect(e.Location);
                        }
                    }
                }
                else
                {
                    if (!SelectedImage)
                    {
                        //toolTip.SetToolTip(this, ToolTipStartCapture);
                    }
                    else
                    {
                        if (DrawStyle == DrawStyle.None)
                        {
                            if (OperateManager.OperateCount == 0)
                            {
                                SetSizeGrip(e.Location);
                            }
                        }
                        else
                        {
                            if (SelectImageRect.Contains(e.Location))
                            {
                                Cursor = DrawCursor;
                            }
                            else
                            {
                                Cursor = SelectCursor;
                            }
                        }
                    }
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (e.Button == MouseButtons.Left)
                {
                    if (!SelectedImage)
                    {
                        SelectImageRect = GetSelectImageRect(e.Location);
                        if (!SelectImageRect.IsEmpty)
                        {
                            SelectedImage = true;
                            //ShowDrawToolsControl();
                            DrawToolsControlButtonSaveClick(null, null);
                        }
                    }
                    else
                    {
                        _endPoint = e.Location;
                        base.Invalidate();
                        if (DrawStyle != DrawStyle.None)
                        {
                            ClipCursor(true);
                            AddOperate(e.Location);
                        }
                        else if (SizeGrip != SizeGrip.None)
                        {
                            _selectImageBounds = SelectImageRect;
                            //ShowDrawToolsControl();
                            DrawToolsControlButtonSaveClick(null, null);
                            SizeGrip = SizeGrip.None;
                        }
                    }

                    _mouseDown = false;
                    _mouseDownPoint = Point.Empty;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (SelectedImage)
                    {
                        if (SelectImageRect.Contains(e.Location))
                        {
                            //contextMenuStrip.Show(this, e.Location);
                            contextMenuStripVisible = true;
                        }
                        else
                        {
                            ResetSelectImage();
                        }
                    }
                    else
                    {
                        DialogResult = DialogResult.Cancel;
                    }
                }
            }

            protected override void OnMouseDoubleClick(MouseEventArgs e)
            {
                base.OnMouseDoubleClick(e);

                bool contains = SelectImageRect.Contains(e.Location);
                if (e.Button == MouseButtons.Left)
                {
                    if (contains)
                    {
                        DrawLastImage();
                        DialogResult = DialogResult.OK;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (!contains)
                    {
                        DialogResult = DialogResult.Cancel;
                    }
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                //将所选择区域原色显示
                //画突出显示的部分
                Graphics oldg = e.Graphics;
                oldg.DrawImage(this.ScreenImage, SelectImageRect, SelectImageRect, GraphicsUnit.Pixel);

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (SelectImageRect.Width != 0 && SelectImageRect.Height != 0)
                {
                    Rectangle rect = SelectImageRect;
                    CaptureImageToolColorTable colorTable = ColorTable;
                    if (_mouseDown)
                    {
                        if (!SelectedImage || SizeGrip != SizeGrip.None)
                        {
                            using (SolidBrush brush = new SolidBrush(
                                Color.FromArgb(0, colorTable.BackColorNormal)))
                            {
                                g.FillRectangle(brush, rect);
                            }

                            DrawImageSizeInfo(g, rect);
                        }
                    }

                    using (Pen pen = new Pen(colorTable.BorderColor))
                    {
                        g.DrawRectangle(pen, rect);

                        using (SolidBrush brush = new SolidBrush(colorTable.BackColorPressed))
                        {
                            foreach (Rectangle sizeGripRect in SizeGripRectList.Values)
                            {
                                g.FillRectangle(
                                    brush,
                                    sizeGripRect);
                            }
                        }
                    }

                    DrawOperate(g);

                    if (DrawStyle != DrawStyle.None)
                    {
                        DrawTools(g, _endPoint);
                    }
                }


            }

            protected override void OnClosing(CancelEventArgs e)
            {
                base.OnClosing(e);
                if (_sizeGripRectList != null)
                {
                    _sizeGripRectList.Clear();
                    _sizeGripRectList = null;
                }
                if (_operateManager != null)
                {
                    _operateManager.Dispose();
                    _operateManager = null;
                }
                if (_linePointList != null)
                {
                    _linePointList.Clear();
                    _linePointList = null;
                }

                _selectCursor = null;
                _drawCursor = null;
            }

            #endregion

            #region Draw Methods

            private void DrawImageSizeInfo(Graphics g, Rectangle rect)
            {
                string text = string.Format(
                                "{0}x{1}",
                                rect.Width,
                                rect.Height);
                Size textSize = TextRenderer.MeasureText(text, TextFont);
                Rectangle screenBounds = Screen.GetBounds(this);
                int x = 0;
                int y = 0;
                if (rect.X + textSize.Width > screenBounds.Right - 3)
                {
                    x = screenBounds.Right - textSize.Width - 3;
                }
                else
                {
                    x = rect.X + 2;
                }

                if (rect.Y - textSize.Width < screenBounds.Y + 3)
                {
                    y = rect.Y + 2;
                }
                else
                {
                    y = rect.Y - textSize.Height - 2;
                }

                Rectangle textRect = new Rectangle(
                    x, y, textSize.Width, textSize.Height);
                g.FillRectangle(Brushes.Black, textRect);
                TextRenderer.DrawText(
                    g,
                    text,
                    TextFont,
                    textRect,
                    Color.White);
            }

            private void DrawTools(Graphics g, Point point)
            {
                if (!SelectImageRect.Contains(_mouseDownPoint))
                {
                    return;
                }
                Color color = Color.Red;

                switch (DrawStyle)
                {
                    case DrawStyle.Rectangle:
                        using (Pen pen = new Pen(color))
                        {
                            g.DrawRectangle(
                                pen,
                                ImageBoundsToRect(Rectangle.FromLTRB(
                                _mouseDownPoint.X,
                                _mouseDownPoint.Y,
                                point.X,
                                point.Y)));
                        }
                        break;
                    case DrawStyle.Ellipse:
                        using (Pen pen = new Pen(color))
                        {
                            g.DrawEllipse(
                                pen,
                                ImageBoundsToRect(Rectangle.FromLTRB(
                                _mouseDownPoint.X,
                                _mouseDownPoint.Y,
                                point.X,
                                point.Y)));
                        }
                        break;
                    case DrawStyle.Arrow:
                        using (Pen pen = new Pen(color))
                        {
                            pen.EndCap = LineCap.ArrowAnchor;
                            pen.EndCap = LineCap.Custom;
                            pen.CustomEndCap = new AdjustableArrowCap(4, 4, true);
                            g.DrawLine(pen, _mouseDownPoint, point);
                        }
                        break;
                    case DrawStyle.Text:
                        using (Pen pen = new Pen(color))
                        {
                            pen.DashStyle = DashStyle.DashDot;
                            pen.DashCap = DashCap.Round;
                            pen.DashPattern = new float[] { 9f, 3f, 3f, 3f };

                            g.DrawRectangle(
                                pen,
                                ImageBoundsToRect(Rectangle.FromLTRB(
                                _mouseDownPoint.X,
                                _mouseDownPoint.Y,
                                point.X,
                                point.Y)));
                        }
                        break;
                    case DrawStyle.Line:
                        if (LinePointList.Count < 2)
                        {
                            return;
                        }

                        Point[] points = LinePointList.ToArray();

                        using (Pen pen = new Pen(color))
                        {
                            g.DrawLines(
                               pen,
                               points);
                        }
                        break;
                }
            }

            private void DrawOperate(Graphics g)
            {
                foreach (OperateObject obj in OperateManager.OperateList)
                {
                    switch (obj.OperateType)
                    {
                        case OperateType.DrawRectangle:
                            using (Pen pen = new Pen(obj.Color))
                            {
                                g.DrawRectangle(
                                    pen,
                                    (Rectangle)obj.Data);
                            }
                            break;
                        case OperateType.DrawEllipse:
                            using (Pen pen = new Pen(obj.Color))
                            {
                                g.DrawEllipse(
                                    pen,
                                    (Rectangle)obj.Data);
                            }
                            break;
                        case OperateType.DrawArrow:
                            Point[] points = obj.Data as Point[];
                            using (Pen pen = new Pen(obj.Color))
                            {
                                pen.EndCap = LineCap.Custom;
                                pen.CustomEndCap = new AdjustableArrowCap(4, 4, true);
                                g.DrawLine(pen, points[0], points[1]);
                            }
                            break;
                        case OperateType.DrawText:
                            DrawTextData textdata = obj.Data as DrawTextData;

                            if (string.IsNullOrEmpty(textdata.Text))
                            {
                                using (Pen pen = new Pen(obj.Color))
                                {
                                    pen.DashStyle = DashStyle.DashDot;
                                    pen.DashCap = DashCap.Round;
                                    pen.DashPattern = new float[] { 9f, 3f, 3f, 3f };
                                    g.DrawRectangle(
                                        pen,
                                        textdata.TextRect);
                                }
                            }
                            else
                            {
                                using (SolidBrush brush = new SolidBrush(obj.Color))
                                {
                                    g.DrawString(
                                        textdata.Text,
                                        textdata.Font,
                                        brush,
                                        textdata.TextRect);
                                }
                            }
                            break;
                        case OperateType.DrawLine:
                            using (Pen pen = new Pen(obj.Color))
                            {
                                g.DrawLines(pen, obj.Data as Point[]);
                            }
                            break;
                    }
                }
            }

            private void DrawLastImage()
            {
                using (Bitmap allBmp = new Bitmap(
                    Width, Height, PixelFormat.Format32bppArgb))
                {
                    using (Graphics allGraphics = Graphics.FromImage(allBmp))
                    {
                        allGraphics.InterpolationMode =
                            InterpolationMode.HighQualityBicubic;
                        allGraphics.SmoothingMode = SmoothingMode.AntiAlias;

                        //使用原始图片获取截图（涂剑凯修改）
                        allGraphics.DrawImage(ScreenImage, Point.Empty);
                        //使用窗体背景图片截图（原版）
                        //allGraphics.DrawImage(BackgroundImage,Point.Empty);

                        DrawOperate(allGraphics);
                        allGraphics.Flush();

                        Bitmap bmp = new Bitmap(
                           SelectImageRect.Width,
                           SelectImageRect.Height,
                           PixelFormat.Format32bppArgb);
                        Graphics g = Graphics.FromImage(bmp);
                        g.DrawImage(
                            allBmp,
                            0,
                            0,
                            SelectImageRect,
                            GraphicsUnit.Pixel);

                        g.Flush();
                        g.Dispose();
                        _image = bmp;
                    }
                }
                //将图片放到剪贴板（涂剑凯添加）
                Clipboard.SetDataObject(this.Image, true);
            }

            #endregion

            #region Events Methods

            private void DrawToolsControlButtonRedoClick(object sender, EventArgs e)
            {
                if (OperateManager.OperateCount > 0)
                {
                    OperateManager.RedoOperate();
                    base.Invalidate();
                }
                else
                {
                    if (SelectedImage)
                    {
                        ResetSelectImage();
                        base.Invalidate();
                    }
                }
            }

            public string text_ocr;
            private void DrawToolsControlButtonSaveClick(object sender, EventArgs e)
            {
                if (SelectedImage)
                {
                    //默认文件名
                    //saveFileDialog.FileName = "JD" +DateTime.Now.ToString("yyyyMMddHHmmss")+"_"+ new Random().Next(1000000, 9999999) + "." + saveFileDialog.DefaultExt;
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JPEG 文件(*.jpg)|*.jpg|BMP 文件(*.bmp)|*.bmp|PNG 文件(*.png)|*.png|GIF 文件" + "(*.gif)|*.gif";
                    saveFileDialog.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
                    saveFileDialog.FileName = "区域截图" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    //if (saveFileDialog.ShowDialog() == DialogResult.OK)   //不需要弹出保存对话框时注释
                    //{
                    DrawLastImage();
                    string fileName = saveFileDialog.FileName;
                    int index = fileName.LastIndexOf('.');
                    string extion = fileName.Substring(
                        index + 1, fileName.Length - index - 1);
                    extion = extion.ToLower();

                    ImageFormat imageFormat = ImageFormat.Bmp;

                    switch (extion)
                    {
                        case "jpg":
                        case "jpeg":
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        case "png":
                            imageFormat = ImageFormat.Png;
                            break;
                        case "gif":
                            imageFormat = ImageFormat.Gif;
                            break;
                    }
                    Image.Save("d:\\" + saveFileDialog.FileName + ".jpg", imageFormat);
                    //DialogResult = DialogResult.Cancel;  //不需要弹出保存对话框时注释
                    this.Close();
                    //}
                    //OCR识别
                    string aa = "d:\\" + saveFileDialog.FileName + ".jpg";
                    //MessageBox.Show(aa);
                    var imagebyte = File.ReadAllBytes(aa);

                    Bitmap bitmap_ = new Bitmap(new MemoryStream(imagebyte));

                    OCRModelConfig config = null;

                    OCRParameter oCRParameter = new OCRParameter();

                    OCRResult ocrResult = new OCRResult();

                    //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。    
                    PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
                    {
                        ocrResult = engine.DetectText(bitmap_);
                    }
                    if (ocrResult != null)
                    {
                        //MessageBox.Show( ocrResult.Text);
                        text_ocr = ocrResult.Text;

                    }


                }
                else
                {
                    MessageBox.Show("请先选择图像。", "截图", MessageBoxButtons.OK);
                }
            }

            private void DrawToolsControlButtonAcceptClick(object sender, EventArgs e)
            {
                if (SelectedImage)
                {
                    DrawLastImage();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                }
            }

            private void DrawToolsControlButtonExitClick(object sender, EventArgs e)
            {
                DialogResult = DialogResult.Cancel;
            }

            private void MenuItemReselectClick(object sender, EventArgs e)
            {
                if (SelectedImage)
                {
                    ResetSelectImage();
                }
            }


            #endregion

            #region Private Methods

            private void Init()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
                //双缓冲绘制，避免闪烁
                //this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

                //drawToolsControl.Visible = false;
                //colorSelector.Visible = false;
                //textBox.Visible = false;

                this.TopMost = true;
                this.ShowInTaskbar = false;
                this.FormBorderStyle = FormBorderStyle.None;
                //Bounds = Screen.GetBounds(this);
                this.Bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

                //保留当前屏
                ScreenImage = GetDestopImage();
                //复制当前屏
                Image BackScreen = new Bitmap(ScreenImage);
                Graphics g = Graphics.FromImage(BackScreen);
                //画遮罩
                g.FillRectangle(mask, 0, 0, BackScreen.Width, BackScreen.Height);
                g.Dispose();
                //将有遮罩的图像作为背景
                this.BackgroundImage = BackScreen;

                //BackgroundImage = GetDestopImage();

                //try
                //{
                //    _selectCursor = new Cursor(Properties.Resources.Arrow_M.Handle);
                //}
                //catch { }
                Cursor = SelectCursor;
                //contextMenuStrip.Renderer = new ToolStripRendererEx();

                //textBox.LostFocus += new EventHandler(TextBoxExLostFocus);

                //colorSelector.ColorChanged += new EventHandler(
                //    ColorSelectorColorChanged);

                //drawToolsControl.ButtonExitClick += new EventHandler(
                //    DrawToolsControlButtonExitClick);
                //drawToolsControl.ButtonAcceptClick += new EventHandler(
                //    DrawToolsControlButtonAcceptClick);
                //drawToolsControl.ButtonSaveClick += new EventHandler(
                //    DrawToolsControlButtonSaveClick);
                //drawToolsControl.ButtonRedoClick += new EventHandler(
                //    DrawToolsControlButtonRedoClick);
                //drawToolsControl.ButtonDrawStyleClick += new EventHandler(
                //    DrawToolsControlButtonDrawStyleClick);

                //menuItemExit.Click += new EventHandler(
                //    DrawToolsControlButtonExitClick);
                //menuItemAccept.Click += new EventHandler(
                //    DrawToolsControlButtonAcceptClick);
                //menuItemSave.Click += new EventHandler(
                //    DrawToolsControlButtonSaveClick);
                //menuItemRedo.Click += new EventHandler(
                //    DrawToolsControlButtonRedoClick);
                //menuItemReselect.Click += new EventHandler(
                //    MenuItemReselectClick);
            }

            /// <summary>
            /// 截取完整屏幕图片
            /// </summary>
            /// <returns></returns>
            private Image GetDestopImage()
            {
                Rectangle rect = Screen.GetBounds(this);
                Bitmap bmp = new Bitmap(
                    rect.Width, rect.Height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);

                IntPtr gHdc = g.GetHdc();
                IntPtr deskHandle = NativeMethods.GetDesktopWindow();

                IntPtr dHdc = NativeMethods.GetDC(deskHandle);
                NativeMethods.BitBlt(
                    gHdc,
                    0,
                    0,
                    Width,
                    Height,
                    dHdc,
                    0,
                    0,
                    NativeMethods.TernaryRasterOperations.SRCCOPY);
                NativeMethods.ReleaseDC(deskHandle, dHdc);
                g.ReleaseHdc(gHdc);
                return bmp;
            }

            private Rectangle GetSelectImageRect(Point endPoint)
            {
                _selectImageBounds = Rectangle.FromLTRB(
                    _mouseDownPoint.X,
                    _mouseDownPoint.Y,
                    endPoint.X,
                    endPoint.Y);

                return ImageBoundsToRect(_selectImageBounds);
            }

            private void CalCulateSizeGripRect()
            {
                Rectangle rect = SelectImageRect;

                int x = rect.X;
                int y = rect.Y;
                int centerX = x + rect.Width / 2;
                int centerY = y + rect.Height / 2;

                Dictionary<SizeGrip, Rectangle> list = SizeGripRectList;
                list.Clear();

                list.Add(
                    SizeGrip.TopLeft,
                    new Rectangle(x - 2, y - 2, 5, 5));
                list.Add(
                    SizeGrip.TopRight,
                    new Rectangle(rect.Right - 2, y - 2, 5, 5));
                list.Add(
                    SizeGrip.BottomLeft,
                    new Rectangle(x - 2, rect.Bottom - 2, 5, 5));
                list.Add(
                    SizeGrip.BottomRight,
                    new Rectangle(rect.Right - 2, rect.Bottom - 2, 5, 5));
                list.Add(
                    SizeGrip.Top,
                    new Rectangle(centerX - 2, y - 2, 5, 5));
                list.Add(
                    SizeGrip.Bottom,
                    new Rectangle(centerX - 2, rect.Bottom - 2, 5, 5));
                list.Add(
                    SizeGrip.Left,
                    new Rectangle(x - 2, centerY - 2, 5, 5));
                list.Add(
                    SizeGrip.Right,
                    new Rectangle(rect.Right - 2, centerY - 2, 5, 5));
            }

            private void SetSizeGrip(Point point)
            {
                SizeGrip = SizeGrip.None;
                foreach (SizeGrip sizeGrip in SizeGripRectList.Keys)
                {
                    if (SizeGripRectList[sizeGrip].Contains(point))
                    {
                        SizeGrip = sizeGrip;
                        break;
                    }
                }

                if (SizeGrip == SizeGrip.None)
                {
                    if (SelectImageRect.Contains(point))
                    {
                        SizeGrip = SizeGrip.All;
                    }
                }

                switch (SizeGrip)
                {
                    case SizeGrip.TopLeft:
                    case SizeGrip.BottomRight:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case SizeGrip.TopRight:
                    case SizeGrip.BottomLeft:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case SizeGrip.Top:
                    case SizeGrip.Bottom:
                        Cursor = Cursors.SizeNS;
                        break;
                    case SizeGrip.Left:
                    case SizeGrip.Right:
                        Cursor = Cursors.SizeWE;
                        break;
                    case SizeGrip.All:
                        Cursor = Cursors.SizeAll;
                        break;
                    default:
                        Cursor = SelectCursor;
                        break;
                }
            }

            private void ChangeSelctImageRect(Point point)
            {
                Rectangle rect = _selectImageBounds;
                int left = rect.Left;
                int top = rect.Top;
                int right = rect.Right;
                int bottom = rect.Bottom;
                bool sizeGripAll = false;

                switch (SizeGrip)
                {
                    case SizeGrip.All:
                        rect.Offset(
                            point.X - _mouseDownPoint.X, point.Y - _mouseDownPoint.Y);
                        sizeGripAll = true;
                        break;
                    case SizeGrip.TopLeft:
                        left = point.X;
                        top = point.Y;
                        break;
                    case SizeGrip.TopRight:
                        right = point.X;
                        top = point.Y;
                        break;
                    case SizeGrip.BottomLeft:
                        left = point.X;
                        bottom = point.Y;
                        break;
                    case SizeGrip.BottomRight:
                        right = point.X;
                        bottom = point.Y;
                        break;
                    case SizeGrip.Top:
                        top = point.Y;
                        break;
                    case SizeGrip.Bottom:
                        bottom = point.Y;
                        break;
                    case SizeGrip.Left:
                        left = point.X;
                        break;
                    case SizeGrip.Right:
                        right = point.X;
                        break;
                }

                //_selectImageBounds = rect;
                if (!sizeGripAll)
                {
                    rect.X = left;
                    rect.Y = top;
                    rect.Width = right - left;
                    rect.Height = bottom - top;
                }
                _mouseDownPoint = point;
                _selectImageBounds = rect;
                SelectImageRect = ImageBoundsToRect(rect); ;
            }

            private Rectangle ImageBoundsToRect(Rectangle bounds)
            {
                Rectangle rect = bounds;
                int x = 0;
                int y = 0;

                x = Math.Min(rect.X, rect.Right);
                y = Math.Min(rect.Y, rect.Bottom);

                rect.X = x;
                rect.Y = y;
                rect.Width = Math.Max(1, Math.Abs(rect.Width));
                rect.Height = Math.Max(1, Math.Abs(rect.Height));
                return rect;
            }

            private void ResetSelectImage()
            {
                SelectedImage = false;
                _selectImageBounds = Rectangle.Empty;
                SelectImageRect = Rectangle.Empty;
                SizeGrip = SizeGrip.None;

                //HideDrawToolsControl();
                //if (textBox.Visible)
                //{
                //    HideTextBox();
                //}
                OperateManager.Clear();
                base.Invalidate();
            }


            /// <summary>
            /// 显示添加文字的输入框
            /// </summary>
            private void ShowTextBox()
            {
                if (SelectImageRect.Contains(_mouseDownPoint))
                {
                    Rectangle bounds = ImageBoundsToRect(
                        Rectangle.FromLTRB(
                        _mouseDownPoint.X,
                        _mouseDownPoint.Y,
                        _endPoint.X,
                        _endPoint.Y));

                    bounds.Inflate(-1, -1);
                    //textBox.Bounds = bounds;
                    //textBox.Text = "";
                    //textBox.ForeColor = SelectedColor;
                    //textBox.Font = new Font(
                    //   textBox.Font.FontFamily,
                    //   (float)FontSize);
                    //textBox.Visible = true;
                    //textBox.Focus();
                }
            }

            /// <summary>
            /// 隐藏添加文字的输入框
            /// </summary>
            //private void HideTextBox()
            //{
            //    textBox.Visible = false;
            //    textBox.Text = string.Empty;
            //}

            private void AddOperate(Point point)
            {
                if (!SelectImageRect.Contains(_mouseDownPoint))
                {
                    return;
                }

                //Color color = SelectedColor;
                Color color = Color.Red;
                switch (DrawStyle)
                {
                    case DrawStyle.Rectangle:
                        OperateManager.AddOperate(
                            OperateType.DrawRectangle,
                            color,
                            ImageBoundsToRect(Rectangle.FromLTRB(
                            _mouseDownPoint.X,
                            _mouseDownPoint.Y,
                            point.X,
                            point.Y)));
                        break;
                    case DrawStyle.Ellipse:
                        OperateManager.AddOperate(
                           OperateType.DrawEllipse,
                           color,
                           ImageBoundsToRect(Rectangle.FromLTRB(
                           _mouseDownPoint.X,
                           _mouseDownPoint.Y,
                           point.X,
                           point.Y)));
                        break;
                    case DrawStyle.Arrow:
                        Point[] points = new Point[] { _mouseDownPoint, point };
                        OperateManager.AddOperate(
                            OperateType.DrawArrow,
                            color,
                            points);
                        break;
                    case DrawStyle.Text:
                        ShowTextBox();
                        Rectangle textRect = ImageBoundsToRect(Rectangle.FromLTRB(
                           _mouseDownPoint.X,
                           _mouseDownPoint.Y,
                           point.X,
                           point.Y));
                        DrawTextData textData = new DrawTextData(
                            string.Empty,
                            base.Font,
                            textRect);

                        OperateManager.AddOperate(
                            OperateType.DrawText,
                            color,
                            textData);
                        break;
                    case DrawStyle.Line:
                        if (LinePointList.Count < 2)
                        {
                            return;
                        }
                        OperateManager.AddOperate(
                            OperateType.DrawLine,
                            color,
                            LinePointList.ToArray());
                        LinePointList.Clear();
                        break;
                }
            }

            private void ClipCursor(bool reset)
            {
                Rectangle rect;
                if (reset)
                {
                    rect = Screen.GetBounds(this);
                }
                else
                {
                    rect = SelectImageRect;
                }

                NativeMethods.RECT nativeRect = new NativeMethods.RECT(rect);
                NativeMethods.ClipCursor(ref nativeRect);
            }

            #endregion

            /// <summary>
            /// 键盘事件响应
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void CaptureImageTool_KeyUp(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    //按下ESC，首先退出右键菜单，再退出截图
                    if (contextMenuStripVisible)
                    {
                        //contextMenuStrip.Hide();
                        contextMenuStripVisible = false;
                    }
                    else
                    {
                        DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                }
            }

            /// <summary>
            /// 右键菜单是否显示状态
            /// </summary>
            private bool contextMenuStripVisible = false;
        }

        public class CaptureImageToolColorTable
        {
            private static readonly Color _borderColor = Color.FromArgb(65, 173, 236);
            private static readonly Color _backColorNormal = Color.FromArgb(229, 243, 251);
            private static readonly Color _backColorHover = Color.FromArgb(65, 173, 236);
            private static readonly Color _backColorPressed = Color.FromArgb(24, 142, 206);
            private static readonly Color _foreColor = Color.FromArgb(12, 83, 124);

            public CaptureImageToolColorTable() { }

            public virtual Color BorderColor
            {
                get { return _borderColor; }
            }

            public virtual Color BackColorNormal
            {
                get { return _backColorNormal; }
            }

            public virtual Color BackColorHover
            {
                get { return _backColorHover; }
            }

            public virtual Color BackColorPressed
            {
                get { return _backColorPressed; }
            }

            public virtual Color ForeColor
            {
                get { return _foreColor; }
            }
        }




        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.Hide();
            //this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

        }
    }
}
