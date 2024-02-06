using System.Security.Cryptography;

namespace BlockInfrastructure.Common.Extensions;

public static class RsaExtensions
{
    public static RSA CreateRsaFromXml(string xml)
    {
        var rsa = RSA.Create();
        rsa.FromXmlString(xml);
        return rsa;
    }
}