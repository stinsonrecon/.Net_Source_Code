using EVNSample.Clearsign;
using System;
using System.IO;

namespace EVNSample
{
    class Program
    {
        static string usename = "bank_name";
        //static string password = "bank_pass";

        static string password = "EVNCASHFOLW";
        static void Main(string[] args)
        {




            ////RSA key dùng để mã hoá và giải mã file do EVN sinh. NH dùng Pulickey để mã hoá, EVN dùng Sceretkey để giải mã(đây là test thử nghiệm thật dùng key do EVN đã gửi cho NH)
            //RSAGenkey.GenerateKey(usename, password, "EVNSceretRSA.asc", "EVNPublicRSA.asc");

            ////DSA key dùng để ký và xác thực file file do NH sinh. NH dùng Sceretkey để ký, EVN dùng Pulickey để xác thực 
            //DSAGenkey.GenerateKey(usename, password, "bankSceretDSA.asc", "bankPublicDSA.asc");


            // Ký file dùng Sceretkey của NH (Phía NH thực hiện)
            //PGPEncryptSign.Sign("Temfile.csv", "bankSceretDSA.asc", password, "Temfile.sign");

            //Sign("Temfile.csv", "bankSceretDSA.asc", password, "Temfile2.sign");

            // Mã hoá file dùng Pulickey của EVN (Phía NH thực hiện)
            //PGPEncryptSign.Encrypt("Temfile.sign", "EVNPublicRSA.asc", "Temfile.encrypt");

            // giải mã file sử dụng Sceretkey của EVN (phía EVN thực hiện)
            //PGPDecryptVerify.Decrypt("File1.encrypt", "EVNSceretRSA.asc", password, "File1.decrypt");

            // Xác thực file sử dụng Pulickey của NH (phía EVN thực hiện)
            //var result = PGPDecryptVerify.Verify("01201001_EVNIT_DETAIL_20200626_000001_1.sign", "01201001_PubRSA.asc", "01201001_EVNIT_DETAIL_20200626_000001_1.csv");

            //var result1 = PGPDecryptVerify.Verify("01201001_EVNIT_SUMMARY_20200626_000001_1.sign", "01201001_PubRSA.asc", "01201001_EVNIT_SUMMARY_20200626_000001_1.csv");

            var result2 = PGPDecryptVerify.DecryptAndVerify("01201001_EVNIT_SUMMARY_20200626_000001_1.encrypt", "EVNsecret.asc", password, "01201001_PubRSA.asc", "01201001_EVNIT_SUMMARY_20200626_000001_1.csv");

            //Verify("Temfile.sign", "bankPublicDSA.asc", "Temfile_Decrypt.csv");

            Console.ReadKey();

        }

        public static void Sign(string filePath, string secretKeyFile, string passPhrase, string outputFilePath)
        {
            Stream keyIn = File.OpenRead(secretKeyFile);
            Stream fos = File.Create(outputFilePath);
            ClearSignedFileProcessor.SignFile(filePath, keyIn, fos, passPhrase.ToCharArray(), "SHA256");
            keyIn.Close();
            fos.Close();
        }

    }
}
