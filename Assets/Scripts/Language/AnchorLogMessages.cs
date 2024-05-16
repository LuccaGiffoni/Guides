namespace Language
{
    public static class AnchorLogMessages
    {
        public static string anchorNotSavedYet => Settings.UserPreferences.activeLanguage == Languages.Portuguese ? "Âncora não salva ainda!" : "Anchor not saved yet!";
        public static string anchorLocalized => Settings.UserPreferences.activeLanguage == Languages.Portuguese ? "Âncora localizada na memória!" : "Anchor localized in memory!";
        public static string savedAnchorIsNotLoaded => Settings.UserPreferences.activeLanguage == Languages.Portuguese ?
            "Existe uma âncora salva no banco de dados para essa operação que ainda não foi carregada. Carregue-a clicando em 'B' ou a delete - com o botão 'Y' para criar e salvar outra âncora." :
            "There is an anchor saved in the database for this operation that has not been loaded yet. Load it by clicking 'B' or delete it - with the 'Y' button to create and save another anchor.";
        public static string anchorSavedToDatabase => Settings.UserPreferences.activeLanguage == Languages.Portuguese ? "Âncora salva no banco de dados!" : "Anchor saved to database!";
        public static string anchorClearedFromDatabase => Settings.UserPreferences.activeLanguage == Languages.Portuguese ? "Âncora removida do banco de dados!" : "Anchor removed from database!";
        public static string anchorClearedFromDatabaseAndMemory => Settings.UserPreferences.activeLanguage == Languages.Portuguese ? "Âncora removida do banco de dados e da memória!" : "Anchor removed from database!";
        public static string anchorNotFoundOnDatabase => Settings.UserPreferences.activeLanguage == Languages.Portuguese ? "Âncora não encontrada no banco de dados!" : "Anchor not found on database!";
        public static string tryingToFindAnchor => Settings.UserPreferences.activeLanguage == Languages.Portuguese ? "Buscando âncora no banco de dados e na memória!" : "Searching anchor on database and memory!";

        public static string createdAnchorNotSavedYet => Settings.UserPreferences.activeLanguage == Languages.Portuguese ? "Âncora criada, mas não salva no banco de dados!" : "Anchor created, but not saved to database!";
        public static string LogErrorWhileSavingAnchor(string error)
        {
            return Settings.UserPreferences.activeLanguage == Languages.Portuguese
                ? $"Âncora não salva no banco de dados!\n{error}"
                : $"Anchor not saved to database!\n{error}";
        }
        
        public static string LogErrorWhileClearingAnchor(string error)
        {
            return Settings.UserPreferences.activeLanguage == Languages.Portuguese
                ? $"Âncora não removida do banco de dados!\n{error}"
                : $"Anchor not removed from database!\n{error}";
        }
    }
}