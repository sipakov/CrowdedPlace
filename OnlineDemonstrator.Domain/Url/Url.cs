namespace OnlineDemonstrator.Libraries.Domain.Url
{
    public static class Url
    {
        private const string BaseUrl = "https://www.onlinedemonstrator.ru/";
        public static string ActualDemonstrations = $"{BaseUrl}demonstration/getActualDemonstrations";
        public static string AddPoster = $"{BaseUrl}poster/add";
        public static string GetAllActualPosters = $"{BaseUrl}poster/getFromActualDemonstrations?postersCountInDemonstration="; 
        public static string GetNearestDemonstration = $"{BaseUrl}demonstration/getNearestDemonstration"; 
        public static string GetPostersByDemonstrationId = $"{BaseUrl}poster/getPostersByDemonstrationId?demonstrationId=";
        public static string GetDeviceById = $"{BaseUrl}device/get"; 
        public static string AddDevice = $"{BaseUrl}device/add";
        public static string GetPosterById = $"{BaseUrl}poster/getPosterById";
        public static string AddObjectionableReason = $"{BaseUrl}objectionableReason/add";
    }
}