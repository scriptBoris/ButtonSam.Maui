using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ButtonSam.Maui.Platforms.iOS
{
    public class ButtonIos : LayoutView
    {
        private readonly CAShapeLayer _maskLayer = new();
        private CornerRadius _cornerRadius;
        private double _borderWidth;
        private UIColor? _borderColor;

        public ButtonIos()
        {
            Layer.Mask = _maskLayer;
            Layer.EdgeAntialiasingMask = CAEdgeAntialiasingMask.All;
        }

        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value;
                SetNeedsDisplay();
            }
        }

        public double BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                SetNeedsDisplay();
            }
        }

        public UIColor? BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                SetNeedsDisplay();
            }
        }

        public void SetupBackground(UIColor backgroundColor)
        {
            BackgroundColor = backgroundColor;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            _maskLayer.Frame = rect;
            if (CornerRadius.TopLeft > 0 || CornerRadius.TopRight > 0 || CornerRadius.BottomRight > 0 || CornerRadius.BottomLeft > 0)
            {
                var path = RoundedRectWithCustomCorners(rect, CornerRadius);
                _maskLayer.Path = path?.CGPath;
            }
            else
            {
                _maskLayer.Path = CGPath.FromRect(rect);
            }

            if (BorderColor != null && BorderWidth > 0)
            {
                var pathBorder = RoundedRectWithCustomCorners(rect, CornerRadius, BorderWidth / 2);
                if (pathBorder != null)
                {
                    pathBorder.LineWidth = (nfloat)BorderWidth;
                    var strokeColor = BorderColor;
                    strokeColor.SetStroke();
                    pathBorder.Stroke();
                }

                if (pathBorder?.CGPath != null)
                {
                    using var context = UIGraphics.GetCurrentContext();
                    if (context == null)
                        return;

                    context.AddPath(pathBorder.CGPath);
                }
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SetNeedsDisplay();
        }

        public static UIBezierPath? RoundedRectWithCustomCorners(CGRect rect, CornerRadius cornerRadius,
            double insets = 0)
        {
            if (rect.GetMaxX() <= 0 || rect.GetMaxY() <= 0)
                return null;

            if (insets > 0)
                rect = rect.Inset((nfloat)insets, (nfloat)insets);

            var TLRadius = (nfloat)(cornerRadius.TopLeft);
            var TRRadius = (nfloat)(cornerRadius.TopRight);
            var BLRadius = (nfloat)(cornerRadius.BottomLeft);
            var BRRadius = (nfloat)(cornerRadius.BottomRight);
            var path = new UIBezierPath();
            var minX = rect.GetMinX();
            var minY = rect.GetMinY();
            var maxX = rect.GetMaxX();
            var maxY = rect.GetMaxY();

            if (maxX <= 0 || maxY <= 0)
                return null;

            var tl = new CGPoint(minX + TLRadius, minY + TLRadius);
            path.ArcWithCenter(tl, TLRadius, 180, 90, true);
            path.AddLineTo(new CGPoint(maxX - TRRadius, minY));

            var tr = new CGPoint(maxX - TRRadius, minY + TRRadius);
            path.ArcWithCenter(tr, TRRadius, 90, 0, true);
            path.AddLineTo(new CGPoint(maxX, maxY - BRRadius));

            var br = new CGPoint(maxX - BRRadius, maxY - BRRadius);
            path.ArcWithCenter(br, BRRadius, 0, 270, true);
            path.AddLineTo(new CGPoint(minX + BLRadius, maxY));

            var bl = new CGPoint(minX + BLRadius, maxY - BLRadius);
            path.ArcWithCenter(bl, BLRadius, 270, 180, true);

            path.ClosePath();
            return path;
        }
    }

    public static class DrawExtensions
    {
        public static UIBezierPath ArcWithCenter(this UIBezierPath path, CGPoint center, nfloat radius, nfloat startAngleDegrees, nfloat endAngleDegrees, bool clockwise)
        {
            nfloat startAngleRadians = DegreesToRadians(startAngleDegrees);
            nfloat endAngleRadians = DegreesToRadians(endAngleDegrees);

            startAngleRadians = -startAngleRadians;
            endAngleRadians = -endAngleRadians;

            path.AddArc(center, radius, startAngleRadians, endAngleRadians, clockwise);

            return path;
        }

        private static nfloat DegreesToRadians(nfloat degrees)
        {
            return (nfloat)(degrees * Math.PI / 180);
        }
    }
}
