using libplctag;
using libplctag.DataTypes;

namespace TestBench;

internal class Program
{
    static void Main(string[] args)
    {
        LibPlcTag.DebugLevel = DebugLevel.Info;

        var tag = new Tag()
        {
            Name = "hr21",
            Gateway = "192.168.100.209:502",
            Path = "0",
            Protocol = Protocol.modbus_tcp,
        };

        tag.Read();
        Console.WriteLine(tag.GetUInt16(0));

        tag.SetUInt16(0, 1234);
        tag.Write();
    }
}
