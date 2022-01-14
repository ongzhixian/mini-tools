using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MiniTools.HostApp.Services;

internal class CertService
{
    public void MakeCert()
    {
        //System.Security.Cryptography.X509Certificates.X509Certificate. 
        //X509Certificate2 cert = new X509Certificate2
        //{
        //    FriendlyName = "CN"
        //};




        //Microsoft.CertificateServices.Commands.Certificate
        System.Security.Cryptography.RSACryptoServiceProvider rsaProvider = new System.Security.Cryptography.RSACryptoServiceProvider();
        rsaProvider.ExportPkcs8PrivateKey(); // #PKCS#8
        rsaProvider.ExportSubjectPublicKeyInfo();// X509 SubjectPublicKeyInfo
        rsaProvider.ExportRSAPrivateKey();  // PCS#1
        rsaProvider.ExportRSAPublicKey(); // PCS#1

        //X509Certificate2.CreateFromSignedFile();// Signed file? ASN.1 DER 
        //X509Certificate2.CreateFromPemFile();     //Expect PEM encoded X509
        // X509Certificate2.CreateFromCertFile(); //Expect PKCS7 signed file


    }

    public X509Certificate2 BuildSelfSignedServerCertificate(string commonName)
    {
        SubjectAlternativeNameBuilder sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddIpAddress(IPAddress.Loopback);
        sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
        sanBuilder.AddDnsName("localhost");
        sanBuilder.AddDnsName(Environment.MachineName);

        X500DistinguishedName distinguishedName = new X500DistinguishedName($"CN={commonName}");

        using (RSA rsa = RSA.Create(2048))
        {
            var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

            // OID 1.3.6.1.5.5.7.3.1 serverAuth (Indicates that a certificate can be used as an SSL server certificate)
            //1 iso
            //3 identified - organization, org, iso - identified - organization
            //6 dod
            //1 internet
            //5 security
            //5 mechanisms
            //7 pkix
            //3 kp
            //1 serverAuth
            // See: https://oidref.com/1.3.6.1.1.15
            //      http://oid-info.com/get/1.3.6.1.5.5.7.3.1

            request.CertificateExtensions.Add(
               new X509EnhancedKeyUsageExtension(
                   new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

            request.CertificateExtensions.Add(sanBuilder.Build());

            var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
            //certificate.FriendlyName = commonName
            certificate.FriendlyName = "asd";


            

            // create self signed certificates that I later import into My store to use them on IIS.
            //return new X509Certificate2(certificate.Export(X509ContentType.Pfx, "WeNeedASaf3rPassword"), "WeNeedASaf3rPassword", X509KeyStorageFlags.MachineKeySet);
            return new X509Certificate2(certificate.Export(X509ContentType.Pfx, "WeNeedASaf3rPassword"), "WeNeedASaf3rPassword");

            // If you want the pfx, the Export function on X509Certificate2 should do the trick. It returns a byte array with the raw pfx data.
            // Use GetRawCertData() to get `.cer` 
        }
    }



    //var cert2 = X509Certificate2.CreateFromCertFile(@"D:\src\github\mini-tools\test-cert.cer");
    //cert2.GetRawCertData();
    
    // Correct usage
    //CertService certService = new CertService();
    //var cert3 = certService.BuildSelfSignedServerCertificate("sadasd");
    //cert3.Export(X509ContentType.SerializedCert);
    //using (System.IO.FileStream fs = new FileStream(@"D:\src\github\mini-tools\example-cert.cer", FileMode.OpenOrCreate))
    //{
    //    fs.Write(cert3.Export(X509ContentType.Cert));
    //}

}
