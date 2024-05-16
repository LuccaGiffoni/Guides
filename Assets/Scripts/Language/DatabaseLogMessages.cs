namespace Language
{
    public static class DatabaseLogMessages
    {
        public static string serverConnectionSucceeded => Settings.UserPreferences.activeLanguage == Languages.Portuguese
            ? "Conexão estabelecida com sucesso!" : "Connection established successfully!";
        public static string serverConnectionFailed => Settings.UserPreferences.activeLanguage == Languages.Portuguese
            ? "Ocorreu algum erro na conexão com o servidor:" : "An error occurred while connecting to the server:";
        public static string serverAddressNotConfigured => Settings.UserPreferences.activeLanguage == Languages.Portuguese
            ? "O endereço do servidor não foi configurado" : "The server address has not been configured";
        public static string noServerDataReturned => Settings.UserPreferences.activeLanguage == Languages.Portuguese
            ? "O servidor não retornou nenhum registro para a tabela selecionada." : "The server did not return any records for the selected table.";

        public static string LogErrorWhileReadingData(string error)
        {
            return Settings.UserPreferences.activeLanguage == Languages.Portuguese
                ? $"Erro ao ler dados do servidor: {error}" : 
                $"Error while reading data from the server: {error}";
        }
        public static string ReturnedOperations(int operations)
        {
            return Settings.UserPreferences.activeLanguage == Languages.Portuguese
                ? $"Foram encontradas {operations} Operações ativas no banco de dados"
                : $"Found {operations} active Operations in the database";
        }
        
        public static string ReturnedSteps(int steps)
        {
            return Settings.UserPreferences.activeLanguage == Languages.Portuguese
                ? $"Foram encontrados {steps} Passos para a operação selecionada"
                : $"Found {steps} Steps for the selected Operation";
        }
        
        public static string NoneStepFoundOnDatabase => Settings.UserPreferences.activeLanguage == Languages.Portuguese
            ? "O servidor não retornou nenhum passo para a operação selecionada." : "The server did not return Step for the selected Operation.";
    }
}