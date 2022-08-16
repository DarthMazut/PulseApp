using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseApp.Utils
{
    public class AnimationHelper
    {
        public static readonly DependencyProperty AnimationPlayerProperty = DependencyProperty.RegisterAttached("AnimationPlayer", typeof(AnimationPlayer), typeof(AnimationHelper), new PropertyMetadata(null));

        public static AnimationPlayer GetAnimationPlayer(DependencyObject obj)
            => (AnimationPlayer)obj.GetValue(AnimationPlayerProperty);

        public static void SetAnimationPlayer(DependencyObject obj, AnimationPlayer value)
            => obj.SetValue(AnimationPlayerProperty, value);
    }

    public sealed class AnimationPlayer : DependencyObject
    {
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(nameof(Binding), typeof(object), typeof(AnimationPlayer), new PropertyMetadata(null, OnValueChanged));

        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.Register(nameof(Animation), typeof(Storyboard), typeof(AnimationPlayer), new PropertyMetadata(null));

        public static readonly DependencyProperty SupressAnimationProperty =
            DependencyProperty.Register(nameof(SupressAnimation), typeof(bool), typeof(AnimationPlayer), new PropertyMetadata(false));

        public object Binding
        {
            get { return (object)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        public Storyboard Animation
        {
            get { return (Storyboard)GetValue(AnimationProperty); }
            set { SetValue(AnimationProperty, value); }
        }

        public bool SupressAnimation
        {
            get { return (bool)GetValue(SupressAnimationProperty); }
            set { SetValue(SupressAnimationProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // This is required because first change is from null to object when app starts.
            // If we ignore *IF* below tha starting animation will play no matter what and that
            // is not expected.
            if (e.OldValue is null)
            {
                return;
            }

            AnimationPlayer animationPlayer = (AnimationPlayer)d;
            if (!animationPlayer.SupressAnimation)
            {
                animationPlayer.Animation.Begin();
            }
        }
    }
}
