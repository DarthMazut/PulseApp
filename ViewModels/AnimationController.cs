using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    /// <summary>
    /// Allows to control animation flow from within ViewModel object.
    /// Use together with technology-specific *AnimationPlayer*.
    /// <code>
    /// &#60;AnimationPlayer 
    ///     Binding="{AnimationController.Binding}" 
    ///     Animation="{StaticResource MyStoryboard}"/&#62;
    /// </code>
    /// </summary>
    public class AnimationController : INotifyPropertyChanged, IDisposable
    {
        private INotifyPropertyChanged? _host;
        private List<string> _registeredProperties = new();
        private bool _supressSubsequent;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Property to be bind to. It's used by *view*.
        /// </summary>
        public object Binding => new();

        /// <summary>
        /// Supresses animation invoked by change of properties registered 
        /// by <see cref="RegisterAnimationWhenPropertyChanges(INotifyPropertyChanged, string)"/> method.
        /// </summary>
        public bool SupressAnimations { get; set; }

        /// <summary>
        /// Plays animation. This method ignores <see cref="SupressAnimations"/> proeprty.
        /// </summary>
        public void PlayAnimation()
        {
            UpdateBinding();
        }

        /// <summary>
        /// Registers the names of the properties, upon change of which the animation will play.
        /// </summary>
        /// <param name="host">Property owner.</param>
        /// <param name="proeprtyName">Name of property to be registered.</param>
        public void RegisterAnimationWhenPropertyChanges(INotifyPropertyChanged host, string proeprtyName)
        {
            _host = host;
            _registeredProperties.Add(proeprtyName);
            host.PropertyChanged += HostPropertyChanged;
        }

        /// <summary>
        /// Supresses the next animation in line.
        /// </summary>
        public void SupressSubsequentAnimation()
        {
            _supressSubsequent = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_host is not null)
            {
                _host.PropertyChanged -= HostPropertyChanged;
                _host = null;
            }

            _registeredProperties.Clear();
        }

        private void HostPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!SupressAnimations)
            {
                _registeredProperties.ForEach(registeredPropertyName =>
                {
                    if (e.PropertyName == registeredPropertyName)
                    {
                        UpdateBindingIfNotSupressed();
                    }
                });
            }
        }

        private void UpdateBinding() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Binding)));

        private void UpdateBindingIfNotSupressed()
        {
            if (_supressSubsequent)
            {
                _supressSubsequent = false;
            }
            else
            {
                UpdateBinding();
            }
        }


    }
}
