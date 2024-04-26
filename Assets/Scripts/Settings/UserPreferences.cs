using Language;

namespace Settings
{
    public static class UserPreferences
    {
        public static Languages activeLanguage { get; private set; } = Languages.Portuguese;
        
        public static void ChangeActiveLanguage(Languages language)
        {
            activeLanguage = language;
        }
    }
}