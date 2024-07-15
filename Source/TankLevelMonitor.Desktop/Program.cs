using Meadow;
using System.Threading.Tasks;

namespace TankLevelMonitor.DesktopApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await MeadowOS.Start(args);
        }
    }
}