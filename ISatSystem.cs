
namespace SatBox
{
    public interface ISatSystem
    {
        bool QuerySystemStatus();
        bool ValidateResponse();

        //bool AcquisitionStatus { get; }

        //bool ValidateProperties();
        string IpAddress { get; }

        double EbNo { get; }
        double Lat { get; }
        double Lon { get; }
        double Hdg { get; }
        
        double Rel { get; }
        double SatLon { get; }

        double Azimuth { get; }
        double Elevation { get; }
        double CrossPol { get; }

        double SigStrength { get; }

        string SatName { get; }
        string ConStatus { get; }
        //double Skew { get; }
    }
}
