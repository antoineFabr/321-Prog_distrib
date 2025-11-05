using System;
using System.Net;
using System.Net.Sockets;

string ntpServer = "0.ch.pool.ntp.org";

byte[] timeMessage = new byte[48];
timeMessage[0] = 0x1B;

IPEndPoint ntpReference = new IPEndPoint(Dns.GetHostAddresses(ntpServer)[0], 123);
using (UdpClient client = new UdpClient())
{
    client.Connect(ntpReference);

    client.Send(timeMessage, timeMessage.Length);

    timeMessage = client.Receive(ref ntpReference);

    DateTime ntpTime = NtpPacket.ToDateTime(timeMessage);

    Console.WriteLine("- " + ntpTime.ToString("D"));
    Console.WriteLine();
    Console.WriteLine("- " + ntpTime.ToString("dd/MM/yyyy HH:mm:ss"));
    Console.WriteLine();
    Console.WriteLine("- " + ntpTime.ToString("yyyy-mm-dd'T'HH:mm:ss'Z'"));

    DateTime ntpTimeUtc = ntpTime;
    DateTime systemTimeUtc = DateTime.UtcNow;

    TimeSpan timeDiff = systemTimeUtc - ntpTimeUtc;

    Console.WriteLine($"Différence de temps : {timeDiff.TotalSeconds:F2} secondes");

    DateTime localTime = TimeZoneInfo.ConvertTimeToUtc(ntpTimeUtc, TimeZoneInfo.Local);
    Console.WriteLine($"Heure locale : {localTime}");

    TimeZoneInfo swissTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
    DateTime swissTime = TimeZoneInfo.ConvertTimeFromUtc(ntpTimeUtc, swissTimeZone);
    Console.WriteLine($"Heure suisse : {swissTime}");
    /*
    TimeZoneInfo utcTimeZone = TimeZoneInfo.Utc;
    DateTime backToUtc = TimeZoneInfo.ConvertTime(localTime, TimeZoneInfo.Local, utcTimeZone);
    Console.WriteLine($"Retour vers UTC : {backToUtc}");*/
    client.Close();

}

public class NtpPacket
{
    public static DateTime ToDateTime(byte[] ntpData)
    {
        ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
        ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
        var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);
        return networkDateTime;
    }

}