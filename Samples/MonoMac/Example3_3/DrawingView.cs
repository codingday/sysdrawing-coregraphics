
using System;
using System.Collections.Generic;
using System.Linq;
using System.DrawingNative.Drawing2D;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;
using System.DrawingNative;

using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using Rectangle = System.Drawing.Rectangle;
using SizeF = System.Drawing.SizeF;
using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;

namespace Example3_3
{
	public partial class DrawingView : MonoMac.AppKit.NSView, Form
	{

		private DataCollection dc;
		private ChartStyle cs;

		#region Constructors
		
		// Called when created from unmanaged code
		public DrawingView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public DrawingView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			this.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			BackColor = Color.Wheat;
			// Set Form1 size:
			//			this.Width = 350;
			//			this.Height = 300;
			dc = new DataCollection();
			cs = new ChartStyle(this);
			cs.XLimMin = 0f;
			cs.XLimMax = 6f;
			cs.YLimMin = -1.5f;
			cs.YLimMax = 1.5f;
			cs.XTick = 1f;
			cs.YTick = 0.5f;
			cs.TickFont = new Font("Arial", 7, FontStyle.Regular);
			cs.XLabel = "X Axis";
			cs.YLabel = "Y Axis";
			cs.Title = "Sine & Cosine Plot";
			cs.TitleFont = new Font("Arial", 10, FontStyle.Regular);

		}

		public DrawingView (RectangleF rect) : base (rect)
		{
			Initialize();
		}
		
#endregion
		#region Form interface
		public Rectangle ClientRectangle 
		{
			get {
				return new Rectangle((int)Bounds.X,
				                     (int)Bounds.Y,
				                     (int)Bounds.Width,
				                     (int)Bounds.Height);
			}
		}

		Color backColor = Color.White;
		public Color BackColor 
		{
			get {
				return backColor;
			}
			
			set {
				backColor = value;
			}
		}

		Font font;
		public Font Font
		{
			get {
				if (font == null)
					font = new Font("Helvetica",20);
				return font;
			}
			set 
			{
				font = value;
			}
		}
		#endregion

		public override void DrawRect (System.Drawing.RectangleF dirtyRect)
		{

			var g = Graphics.FromCurrentContext();

			g.Clear(backColor);

			cs.ChartArea = this.ClientRectangle;
			AddData();
			SetPlotArea(g);
			cs.AddChartStyle(g);
			dc.AddLines(g, cs);
			g.Dispose();
		}

		public void AddData()
		{
			dc.DataSeriesList.Clear();
			// Add Sine data with 20 data point:
			DataSeries ds1 = new DataSeries();
			ds1.LineStyle.LineColor = Color.Red;
			ds1.LineStyle.Thickness = 2f;
			ds1.LineStyle.Pattern = DashStyle.Dash;
			for (int i = 0; i < 20; i++)
			{
				ds1.AddPoint(new PointF(i / 5.0f, (float)Math.Sin(i / 5.0f)));
			}
			dc.Add(ds1);
			
			// Add Cosine data with 40 data point:
			DataSeries ds2 = new DataSeries();
			ds2.LineStyle.LineColor = Color.Blue;
			ds2.LineStyle.Thickness = 1f;
			ds2.LineStyle.Pattern = DashStyle.Solid;
			for (int i = 0; i < 40; i++)
			{
				ds2.AddPoint(new PointF(i / 5.0f, (float)Math.Cos(i / 5.0f)));
			}
			dc.Add(ds2);
		}

		private void SetPlotArea(Graphics g)
		{
			// Set PlotArea:
			float xOffset = cs.ChartArea.Width / 30.0f;
			float yOffset = cs.ChartArea.Height / 30.0f;
			SizeF labelFontSize = g.MeasureString("A", cs.LabelFont);
			SizeF titleFontSize = g.MeasureString("A", cs.TitleFont);
			if (cs.Title.ToUpper() == "NO TITLE")
			{
				titleFontSize.Width = 8f;
				titleFontSize.Height = 8f;
			}
			float xSpacing = xOffset / 3.0f;
			float ySpacing = yOffset / 3.0f;
			SizeF tickFontSize = g.MeasureString("A", cs.TickFont);
			float tickSpacing = 2f;
			SizeF yTickSize = g.MeasureString(cs.YLimMin.ToString(), cs.TickFont);
			for (float yTick = cs.YLimMin; yTick <= cs.YLimMax; yTick += cs.YTick)
			{
				SizeF tempSize = g.MeasureString(yTick.ToString(), cs.TickFont);
				if (yTickSize.Width < tempSize.Width)
				{
					yTickSize = tempSize;
				}
			}
			float leftMargin = xOffset + labelFontSize.Width +
				xSpacing + yTickSize.Width + tickSpacing;
			float rightMargin = 2 * xOffset;
			float topMargin = yOffset + titleFontSize.Height + ySpacing;
			float bottomMargin = yOffset + labelFontSize.Height +
				ySpacing + tickSpacing + tickFontSize.Height;
			
			// Define the plot area with one Y axis:
			int plotX = cs.ChartArea.X + (int)leftMargin;
			int plotY = cs.ChartArea.Y + (int)topMargin;
			int plotWidth = cs.ChartArea.Width - (int)leftMargin - (int)rightMargin;
			int plotHeight = cs.ChartArea.Height - (int)topMargin - (int)bottomMargin;
			cs.PlotArea = new Rectangle(plotX, plotY, plotWidth, plotHeight);
		}
		//		public override bool IsFlipped {
		//			get {
		//				//return base.IsFlipped;
		//				return false;
		//			}
		//		}

	}
}

public interface Form
{
	Rectangle ClientRectangle { get; }
	
	Color BackColor { get; set; } 
	
	Font Font { get; set; }
}