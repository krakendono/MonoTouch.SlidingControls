using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace Sequence.MonoTouch.SlidingControls
{
	public abstract class EclipsingViewControllerBase : UIViewController
	{
		protected readonly UIView _shadowView;
		private readonly UIButton _contentOverlayButton;
		private readonly int _shadowSize;
		private UIViewController _contentViewController;
		private EclipseDirection _eclipseDirection;
		private UIViewController _eclipsedViewController;
		private bool _eclipsedViewIsVisible;
		private int _eclipsedViewSize;
				
		protected EclipsingViewControllerBase(UIViewController initialContentViewController = null,
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
					
			EclipsedViewController = initialEclipsedViewController;
			ContentViewController = initialContentViewController;
		}
				
		public UIViewController EclipsedViewController
		{
			get { return _eclipsedViewController; }
			protected set
			{
				if (_eclipsedViewController != null)
				{
					_eclipsedViewController.View.RemoveFromSuperview();
				}
						
				_eclipsedViewController = value;
						
				if (value == null)
				{
					return;
				}
						
				View.InsertSubview(_eclipsedViewController.View, 0);
						
				RecalculateChildFrames();
			}
		}
				
		public UIViewController ContentViewController
		{
			get { return _contentViewController; }
			protected set
			{
				if (_contentViewController != null)
				{
					_contentViewController.View.RemoveFromSuperview();
				}
						
				_contentViewController = value;
						
				if (value == null)
				{
					return;
				}
						
				View.AddSubview(_contentViewController.View);
						
				RecalculateChildFrames();
			}
		}
				
		public EclipseDirection EclipseDirection
		{
			get { return _eclipseDirection; }
			set
			{
				_eclipseDirection = value;
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
			protected set { _eclipsedViewSize = value; }
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
					
			var frame = View.Frame;
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
					
			var frameForContentDisplay = new RectangleF(xOffset, yOffset, frame.Width, frame.Height);
			;
			ContentViewController.View.Frame = frameForContentDisplay;
					
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
							CalculateOffsetForEclipsedViewFrame(), 0, EclipsedViewSize, View.Frame.Height);
			}
			else
			{
				EclipsedViewController.View.Frame = new RectangleF(
							0, CalculateOffsetForEclipsedViewFrame(), View.Frame.Width, EclipsedViewSize);
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
				return View.Frame.Width - EclipsedViewSize;
			}
					
			return View.Frame.Height - EclipsedViewSize;
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
			UIView.Animate(0.5, RecalculateChildFrames, () => { 
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


