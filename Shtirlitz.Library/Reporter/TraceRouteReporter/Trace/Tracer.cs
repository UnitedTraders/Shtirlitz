using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Shtirlitz.Reporter.TraceRouteReporter.Trace
{
    public class Tracer : ITracer
    {
        public IEnumerable<TracertEntry> Tracert(string hostNameOrAddress, int maxHops, int timeout, int packageSize)
        {
            if (maxHops < 1)
                throw new ArgumentException("Max hops can't be lower than 1.");

            if (timeout < 1)
                throw new ArgumentException("PingTimeout value must be higher than 0.");

            if (packageSize < 1)
                throw new ArgumentException("packageSize value must be higher than 0.");

            if (string.IsNullOrWhiteSpace(hostNameOrAddress))
                throw new ArgumentException("hostNameOrAddress must be no empty.");

            var ipAddress = GetHostIpAddress(hostNameOrAddress);
            var bytes = GenerateRandomBytes(packageSize);

            using (var ping = new Ping())
            {
                for (var pingOptions = new PingOptions(1, true); pingOptions.Ttl <= maxHops; pingOptions.Ttl++)
                {
                    var reply = ping.Send(ipAddress, timeout, bytes, pingOptions);

                    var te = new TracertEntry
                    {
                        Ttl = pingOptions.Ttl,
                        Address = reply.Address,
                        RoundTrip = reply.RoundtripTime,
                        ReplyStatus = reply.Status
                    };

                    yield return te;

                    if (reply.Status == IPStatus.Success)
                        break;
                }
            }
        }

        public PingEntry Ping(string hostNameOrAddress, int timeout)
        {
            if (timeout < 1)
                throw new ArgumentException("PingTimeout value must be higher than 0.");

            if (string.IsNullOrWhiteSpace(hostNameOrAddress))
                throw new ArgumentException("hostNameOrAddress must be no empty.");

            var ipAddress = GetHostIpAddress(hostNameOrAddress);

            using (var ping = new Ping())
            {
                var reply = ping.Send(ipAddress, timeout);
                return new PingEntry
                {
                    Address = reply.Address,
                    RoundTrip = reply.RoundtripTime,
                    ReplyStatus = reply.Status
                };
            }
        }

        public void GetHostNameAsync(IPAddress address, Action<IPAddress, string> result)
        {
            Dns.BeginGetHostEntry(address, ar => EndGetHostEntry(address, result, ar), address);
        }

        private static void EndGetHostEntry(IPAddress address, Action<IPAddress, string> result, IAsyncResult ar)
        {
            try
            {
                var ihe = Dns.EndGetHostEntry(ar);
                result(address, ihe.HostName);
            }
            catch
            {
            }
        }

        private static IPAddress GetHostIpAddress(string hostNameOrAddress)
        {
            const int tryCount = 3;
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    return Dns.GetHostAddresses(hostNameOrAddress)[0];
                }
                catch (SocketException) { }
            }
            throw new PingException();
        }
        private static byte[] GenerateRandomBytes(int packageSize)
        {
            if (packageSize < 1)
                throw new ArgumentException("Package Size value must be higher than 0.");

            var randarray = new byte[packageSize];
            var random = new Random();
            random.NextBytes(randarray);
            return randarray;
        }
    }
}