namespace OnlineDemonstrator.MobileApi.Interfaces
{
    public interface IDistanceCalculator
    {
        double GetDistanceInKilometers(double latitudePointA, double longitudePointA, double latitudePointB, double longitudePointB);
    }
}