using System;
using System.Collections.Generic;
using System.Linq;
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

}
