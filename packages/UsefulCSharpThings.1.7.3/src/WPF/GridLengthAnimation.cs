using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Animates Grid properties. Not mine.
    /// </summary>
    #pragma warning disable CS1591
    public class GridLengthAnimation : AnimationTimeline
    {
        static GridLengthAnimation()
        {
            FromProperty = DependencyProperty.Register("From", typeof(GridLength),
                typeof(GridLengthAnimation));

            ToProperty = DependencyProperty.Register("To", typeof(GridLength),
                typeof(GridLengthAnimation));

            EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(GridLengthAnimation));
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            GridLengthAnimation anim = new GridLengthAnimation();
            anim.EasingFunction = new CubicEase();
            anim.Duration = TimeSpan.FromSeconds(1);
            return anim;
        }

        public static readonly DependencyProperty EasingFunctionProperty;
        public IEasingFunction EasingFunction
        {
            get
            {
                return (IEasingFunction)GetValue(GridLengthAnimation.EasingFunctionProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.EasingFunctionProperty, value);
            }
        }

        public static readonly DependencyProperty FromProperty;
        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.FromProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.FromProperty, value);
            }
        }

        public static readonly DependencyProperty ToProperty;
        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.ToProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.ToProperty, value);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue,
            object defaultDestinationValue, AnimationClock animationClock)
        {
            GridUnitType fromUnitType = ((GridLength)GetValue(GridLengthAnimation.FromProperty)).GridUnitType;
            GridUnitType toUnitType = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).GridUnitType;
            double fromVal = ((GridLength)GetValue(GridLengthAnimation.FromProperty)).Value;
            double toVal = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).Value;
            IEasingFunction easer = (IEasingFunction)GetValue(GridLengthAnimation.EasingFunctionProperty);

            if (fromVal > toVal)
            {
                if (easer == null)
                    return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal, fromUnitType);
                else
                    return new GridLength((1 - easer.Ease(animationClock.CurrentProgress.Value)) * (fromVal - toVal) + toVal, fromUnitType);
            }
            else
            {
                if (easer == null)
                    return new GridLength(animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal, toUnitType);
                else
                    return new GridLength(easer.Ease(animationClock.CurrentProgress.Value) * (toVal - fromVal) + fromVal, toUnitType);
            }
        }
    }
}
