using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Sequence.MonoTouch.SlidingControls.Samples.Simple
{
	public partial class MainView : EclipsingViewController
	{
		UIViewController _contentViewController;
		UIScrollView _scrollView;
		float _buttonHeight = 50f;

		public MainView()
			: base()
		{
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			_contentViewController = new UIViewController();
			_contentViewController.View.BackgroundColor = UIColor.DarkGray;

			_scrollView = new UIScrollView();
			_contentViewController.View.AddSubview(_scrollView);

			var showEclipsedViewButton = GenerateButton(EclipseDirection.Left, 100);
			_scrollView.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Left);
			_scrollView.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Left, 300);
			_scrollView.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Right);
			_scrollView.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Top);
			_scrollView.AddSubview(showEclipsedViewButton);
			showEclipsedViewButton = GenerateButton(EclipseDirection.Bottom);
			_scrollView.AddSubview(showEclipsedViewButton);
			
			ContentViewController = _contentViewController;
			
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
			_buttonHeight = 50f;
			var yOffset = _buttonHeight * 1.2f * (_buttonCount - 2.5f);
			showEclipsedViewButton.Frame = new RectangleF(
				(View.Frame.Width - buttonWidth) / 2,
				(View.Frame.Height - _buttonHeight) / 2 + yOffset, 
				buttonWidth, 
				_buttonHeight);
			_buttonCount++;
			return showEclipsedViewButton;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			_scrollView.Frame = new RectangleF(0, 0, _contentViewController.View.Bounds.Width, _contentViewController.View.Bounds.Height);
			_scrollView.ContentSize = new SizeF(_contentViewController.View.Bounds.Width, _buttonHeight * 1.2f * (_buttonCount + 2));
		}
	}
}