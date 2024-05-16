namespace Language
{
    public static class PickPositionLogMessages
    {
        public static string pickPositionCreated => Settings.UserPreferences.activeLanguage == Languages.Portuguese 
            ? "Posição de pega criada, mas não salva ainda!" : "Pick Position created, yet not saved!";
        public static string pickPositionSaved => Settings.UserPreferences.activeLanguage == Languages.Portuguese 
            ? "Posição de pega salva com sucesso!" : "Pick Position successfully saved!";
        
        public static string activePickPositionNotRemoved => Settings.UserPreferences.activeLanguage == Languages.Portuguese 
            ? "Não foi possível remover a posição de pega. Tente novamente!" : "Unable to remove OperatorPickPosition. Try again!!";
        public static string activePickPositionRemoved => Settings.UserPreferences.activeLanguage == Languages.Portuguese 
            ? "OperatorPickPosition removido da cena e do banco de dados." : "Removed OperatorPickPosition from scene and database.";
        
        public static string noAnchorLoadedOnScene => Settings.UserPreferences.activeLanguage == Languages.Portuguese 
            ? "Nenhum ponto de ancoragem ativo. Carregue a âncora ao clicar em 'B'" : "No active anchor. Load the anchor by clicking 'B'";
        public static string pickPositionAlreadyCreatedOrLoaded => Settings.UserPreferences.activeLanguage == Languages.Portuguese 
            ? "Já existe uma posição de pega criada ou salva na cena. Delete-a clicando em 'X'"
            : "There is already a OperatorPickPosition created or saved in the scene. Delete it by clicking 'X'";
        
        // Add the "GetControllers" method to the warning
        public static string failedToGetControllerPosition => Settings.UserPreferences.activeLanguage == Languages.Portuguese 
            ? "Não foi possível reconhecer a posição do controle direito. Tente reconhecer os controles novamente!"
            : "Unable to recognize right controller's position. Try recognizing the controls again!";
        public static string failedToGetControllersAndAnchor => Settings.UserPreferences.activeLanguage == Languages.Portuguese 
            ? "Não foi possível reconhecer a posição do controle e não há nenhuma âncora ativa."
            : "Unable to recognize controller's position and there's no active anchor on scene.";

        public static string LogErrorWhileSavingPickPosition(string error)
        {
            return Settings.UserPreferences.activeLanguage == Languages.Portuguese
                ? $"Posição de pega não salva. Ocorreu um problema: {error}"
                : $"Pick Position not saved. A problem occured: {error}";
        }

        public static string LogComponentError(string componentType)
        {
            return Settings.UserPreferences.activeLanguage == Languages.Portuguese 
                ? $"Não há nenhum componente do tipo {componentType}!"
                : $"There's no component from type {componentType} attached to the OperatorPickPosition!";
        }
    }
}