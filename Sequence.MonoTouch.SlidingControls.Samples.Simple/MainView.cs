using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Sequence.MonoTouch.SlidingControls.Samples.Simple
{
	public partial class MainView : EclipsingViewControllerBase
	{
		public MainView()
			: base()
		{
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			var contentViewController = new UIViewController();
			contentViewController.View.BackgroundColor = UIColor.DarkGray;
			
			var showEclipsedViewButton = GenerateButton(EclipseDirection.Left, 100);
			contentViewController.View.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Left);
			contentViewController.View.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Left, 300);
			contentViewController.View.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Right);
			contentViewController.View.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Top);
			contentViewController.View.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Bottom);
			contentViewController.View.AddSubview(showEclipsedViewButton);
			
			ContentViewController = contentViewController;
			
			var eclipsedViewController = new UIViewController();
			eclipsedViewController.View.BackgroundColor = UIColor.Red;
			
			var booLabel = new UILabel()
			{
				Text = "Boo!",
				TextColor = UIColor.White,
				BackgroundColor = UIColor.Clear,
				Font = UIFont.FromName("Helvetica-Bold", 64f),
				Frame = new RectangleF(20, 100, 150, 50)
			};
			eclipsedViewController.View.AddSubview(booLabel);
			
			EclipsedViewController = eclipsedViewController;
		}
		
		int _buttonCount = 0;
		
		UIButton GenerateButton(EclipseDirection direction, int size = 200)
		{
			var showEclipsedViewButton = new UIButton(UIButtonType.RoundedRect);
			var title = string.Format("Show {0} ({1})", direction, size);
			showEclipsedViewButton.SetTitle(title, UIControlState.Normal);
			showEclipsedViewButton.TouchUpInside += (object sender, EventArgs e) => {
				EclipseDirection = direction;
				EclipsedViewSize = size;
				EclipsedViewIsVisible = true;
			};
			var buttonWidth = 200f;
			var buttonHeight = 50f;
			var yOffset = buttonHeight * 1.2f * (_buttonCount - 2.5f);
			showEclipsedViewButton.Frame = new RectangleF(
				(View.Frame.Width - buttonWidth) / 2,
				(View.Frame.Height - buttonHeight) / 2 + yOffset, 
				buttonWidth, 
				buttonHeight);
			_buttonCount++;
			return showEclipsedViewButton;
		}
	}
}