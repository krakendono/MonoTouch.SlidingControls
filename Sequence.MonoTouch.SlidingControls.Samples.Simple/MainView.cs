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
			
			var showEclipsedViewButton = new UIButton(UIButtonType.RoundedRect);
			showEclipsedViewButton.SetTitle("Show Eclipsed View", UIControlState.Normal);
			showEclipsedViewButton.TouchUpInside += (object sender, EventArgs e) => 
			{
				EclipsedViewIsVisible = true;
			};

			var buttonWidth = 200f;
			var buttonHeight = 50f;

			showEclipsedViewButton.Frame = new RectangleF(
				(View.Frame.Width - buttonWidth) / 2,
				(View.Frame.Height - buttonHeight) / 2,
				buttonWidth,
				buttonHeight);

			contentViewController.View.AddSubview(showEclipsedViewButton);

			ContentViewController = contentViewController;

			var eclipsedViewController = new UIViewController();
			eclipsedViewController.View.BackgroundColor = UIColor.Red;

			var booLabel = new UILabel();
			booLabel.Text = "Boo!";
			booLabel.TextColor = UIColor.White;
			booLabel.BackgroundColor = UIColor.Clear;
			booLabel.Font = UIFont.FromName("Helvetica-Bold", 64f);
			booLabel.Frame = new RectangleF(20, 100, 150, 50);
			eclipsedViewController.View.AddSubview(booLabel);

			EclipsedViewController = eclipsedViewController;
		}
	}
}

