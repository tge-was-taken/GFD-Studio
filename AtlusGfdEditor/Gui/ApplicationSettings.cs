using System.ComponentModel;

namespace AtlusGfdEditor.Gui
{
    public enum FormTheme
    {
        Default,
        Dark
    }

    public class ApplicationSettings : INotifyPropertyChanged
    {
        private FormTheme m_Theme;
        public FormTheme Theme
        {
            get { return m_Theme; }
            set
            {
                m_Theme = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ApplicationSettings()
        {
            Theme = FormTheme.Default;
        }
    }
}
