using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace Sequence.Touch.SlidingControls
{
	public class EclipsingViewController : UIViewController
	{
		protected readonly UIView _shadowView;
		private readonly UIButton _contentOverlayButton;
		private readonly UISwipeGestureRecognizer _gestureRecogniser;
		private readonly int _shadowSize;
		private UIViewController _contentViewController;
		private UIImageView _contentBackgroundImageView;
		private EclipseDirection _eclipseDirection;
		private UIViewController _eclipsedViewController;
		private bool _eclipsedViewIsVisible;
		private int _eclipsedViewSize;
				
		public EclipsingViewController(UIViewController initialContentViewController = null,
                                       UIViewController initialEclipsedViewController = null,
                                       int eclipsedViewSize = 200,
                                       EclipseDirection eclipseDirection = EclipseDirection.Left,
                                       int shadowWidth = 4)
		{
			_eclipsedViewSize = eclipsedViewSize;
			_eclipseDirection = eclipseDirection;
			_shadowSize = shadowWidth;
			if (_shadowSize > 0)
			{
				_shadowView = new UIView();
				_shadowView.BackgroundColor = UIColor.White;
				_shadowView.Layer.ShadowColor = UIColor.Black.CGColor;
				_shadowView.Layer.ShadowOpacity = .75f;
			}
					
			_contentOverlayButton = new UIButton();
			_contentOverlayButton.TouchUpInside += delegate
			{
				CoverEclipsedView();
			};

			_contentBackgroundImageView = new UIImageView();
			View.AddSubview(_contentBackgroundImageView);
					
			_gestureRecogniser = new UISwipeGestureRecognizer();
			_gestureRecogniser.AddTarget(() => EclipsedViewIsVisible = false);
			SetSwipeDirection();
			_contentOverlayButton.AddGestureRecognizer(_gestureRecogniser);
		
			EclipsedViewController = initialEclipsedViewController;
			ContentViewController = initialContentViewController;
		}
				
		public UIViewController EclipsedViewController
		{
			get { return _eclipsedViewController; }
			set
			{
				if (_eclipsedViewController != null)
				{
					_eclipsedViewController.WillMoveToParentViewController(null);
					_eclipsedViewController.View.RemoveFromSuperview();
					_eclipsedViewController.RemoveFromParentViewController();
				}
						
				_eclipsedViewController = value;
						
				if (value == null)
				{
					return;
				}

				AddChildViewController(_eclipsedViewController);
				View.InsertSubview(_eclipsedViewController.View, 0);
				_eclipsedViewController.DidMoveToParentViewController(this);	
						
				RecalculateChildFrames();
			}
		}

		public UIImageView ContentBackgroundImageView
		{
			get { return _contentBackgroundImageView; }
		}

		public UIViewController ContentViewController
		{
			get { return _contentViewController; }
			set
			{
				if (_contentViewController != null)
				{
					_contentViewController.WillMoveToParentViewController(null);
					_contentViewController.View.RemoveFromSuperview();
					_contentViewController.RemoveFromParentViewController();
				}

				_contentViewController = value;
						
				if (value == null)
				{
					return;
				}
						
				AddChildViewController(_contentViewController);
				View.AddSubview(_contentViewController.View);
				_contentViewController.DidMoveToParentViewController(this);
						
				RecalculateChildFrames();
			}
		}
				
		public EclipseDirection EclipseDirection
		{
			get { return _eclipseDirection; }
			set
			{
				_eclipseDirection = value;
				SetSwipeDirection();
				RecalculateChildFrames();
			}
		}
				
		public bool EclipsedViewIsVisible
		{
			get { return _eclipsedViewIsVisible; }
			set
			{
				if (_eclipsedViewIsVisible == value)
					return;
				if (value)
					UncoverEclipsedView();
				else
					CoverEclipsedView();
			}
		}
				
		public int EclipsedViewSize
		{
			get { return _eclipsedViewSize; }
			set { _eclipsedViewSize = value; }
		}
				
		protected virtual void OnContentViewWillUncoverEclipsedView()
		{
		}
				
		protected virtual void OnContentViewWillCoverEclipsedView()
		{
		}
				
		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
			RecalculateChildFrames();
		}
				
		private void RecalculateChildFrames()
		{
			RecalculateContentViewFrame();
			RecalculateEclipsedViewFrame();
		}
				
		private void RecalculateContentViewFrame()
		{
			if (ContentViewController == null)
			{
				return;
			}
					
			var bounds = View.Bounds;
			float xOffset = 0;
			float yOffset = 0;
					
			if (EclipseDirection == EclipseDirection.Left || EclipseDirection == EclipseDirection.Right)
			{
				xOffset = CalculateOffsetForContentView();
			}
			else
			{
				yOffset = CalculateOffsetForContentView();
			}
					
			var frameForContentDisplay = new RectangleF(xOffset, yOffset, bounds.Width, bounds.Height);

			ContentViewController.View.Frame = frameForContentDisplay;
			ContentBackgroundImageView.Frame = frameForContentDisplay;

			if (_shadowSize > 0)
			{
				_shadowView.Frame = frameForContentDisplay;
				switch (EclipseDirection)
				{
					case EclipseDirection.Left:
						_shadowView.Layer.ShadowOffset = new SizeF(-_shadowSize, 0);
						break;
					case EclipseDirection.Right:
						_shadowView.Layer.ShadowOffset = new SizeF(_shadowSize, 0);
						break;
					case EclipseDirection.Top:
						_shadowView.Layer.ShadowOffset = new SizeF(0, -_shadowSize);
						break;
					case EclipseDirection.Bottom:
						_shadowView.Layer.ShadowOffset = new SizeF(0, _shadowSize);
						break;
				}
			}
			_contentOverlayButton.Frame = frameForContentDisplay;
		}
				
		private void RecalculateEclipsedViewFrame()
		{
			if (EclipsedViewController == null)
			{
				return;
			}
					
			if (EclipseDirection == EclipseDirection.Left || EclipseDirection == EclipseDirection.Right)
			{
				EclipsedViewController.View.Frame = new RectangleF(
							CalculateOffsetForEclipsedViewFrame(), 0, EclipsedViewSize, View.Bounds.Height);
			}
			else
			{
				EclipsedViewController.View.Frame = new RectangleF(
							0, CalculateOffsetForEclipsedViewFrame(), View.Bounds.Width, EclipsedViewSize);
			}
		}
				
		private float CalculateOffsetForEclipsedViewFrame()
		{
			if (EclipseDirection == EclipseDirection.Left || EclipseDirection == EclipseDirection.Top)
			{
				return 0f;
			}
			else if (EclipseDirection == EclipseDirection.Right)
			{
				return View.Bounds.Width - EclipsedViewSize;
			}
					
			return View.Bounds.Height - EclipsedViewSize;
		}
				
		private float CalculateOffsetForContentView()
		{
			var unsignedOffset = EclipsedViewIsVisible ? EclipsedViewSize : 0;
			if (EclipseDirection == EclipseDirection.Left || EclipseDirection == EclipseDirection.Top)
			{
				return unsignedOffset;
			}
					
			return -unsignedOffset;
		}
				
		private void UncoverEclipsedView()
		{
			if (EclipsedViewIsVisible)
				return;
					
			EnsureInvokedOnMainThread(delegate
			{
				OnContentViewWillUncoverEclipsedView();
				_eclipsedViewIsVisible = true;
				AddShadowAndCloseButton();
				DoCoverOrUncoverAnimation();
			});
		}
				
		private void CoverEclipsedView()
		{
			if (!EclipsedViewIsVisible)
				return;
					
			EnsureInvokedOnMainThread(delegate
			{
				_eclipsedViewIsVisible = false;
				OnContentViewWillCoverEclipsedView();
				RemoveCloseButton();
				DoCoverOrUncoverAnimation();
			});
		}
				
		private void DoCoverOrUncoverAnimation()
		{
			UIView.Animate(0.5, RecalculateContentViewFrame, () => { 
				if (!EclipsedViewIsVisible && _shadowSize > 0)
					RemoveShadow();
			});
		}
				
		private void RemoveShadow()
		{
			_shadowView.RemoveFromSuperview();
		}
				
		private void RemoveCloseButton()
		{
			_contentOverlayButton.RemoveFromSuperview();
		}
				
		private void AddShadowAndCloseButton()
		{
			var mainView = ContentViewController.View;
					
			if (_shadowSize > 0)
			{
				View.InsertSubviewBelow(_shadowView, mainView);
			}
					
			View.AddSubview(_contentOverlayButton);
		}

		private void SetSwipeDirection()
		{
			switch (_eclipseDirection)
			{
				case EclipseDirection.Bottom:
					_gestureRecogniser.Direction = UISwipeGestureRecognizerDirection.Down;
					break;
				case EclipseDirection.Left:
					_gestureRecogniser.Direction = UISwipeGestureRecognizerDirection.Left;
					break;
				case EclipseDirection.Right:
					_gestureRecogniser.Direction = UISwipeGestureRecognizerDirection.Right;
					break;
				case EclipseDirection.Top:
					_gestureRecogniser.Direction = UISwipeGestureRecognizerDirection.Up;
					break;
			}
		}
				
		protected void EnsureInvokedOnMainThread(Action action)
		{
			if (NSThread.Current.IsMainThread)
			{
				action();
				return;
			}
			BeginInvokeOnMainThread(() => action());
		}
	}
}


