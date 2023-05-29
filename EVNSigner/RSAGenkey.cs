using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Encryption.PGP
{
    public class RSAGenkey
    {
        public static void GenerateKey(string username, string password, string secretKeyPath, string publicKeyPath)
        {
            IAsymmetricCipherKeyPairGenerator kpg = new RsaKeyPairGenerator();
            var keygen = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x13), new SecureRandom(), 2048, 16);
            kpg.Init(keygen);
            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            using (FileStream outsecret = new FileInfo(secretKeyPath).OpenWrite())
            {
                using (FileStream outpublic = new FileInfo(publicKeyPath).OpenWrite())
                {
                    ExportKeyPair(outsecret, outpublic, kp.Public, kp.Private, username, password.ToCharArray(), true);

                }
            }
        }

        private static void ExportKeyPair(Stream secretOut, Stream publicOut, AsymmetricKeyParameter publicKey, AsymmetricKeyParameter privateKey,
                    string identity, char[] passPhrase, bool armor)
        {
            if (armor)
            {
                secretOut = new ArmoredOutputStream(secretOut);
            }

            PgpSecretKey secretKey = new PgpSecretKey(PgpSignature.DefaultCertification, PublicKeyAlgorithmTag.RsaGeneral, publicKey, privateKey, DateTime.Now, identity,
                SymmetricKeyAlgorithmTag.Cast5, passPhrase, null, null, new SecureRandom());

            secretKey.Encode(secretOut);

            secretOut.Close();

            if (armor)
            {
                publicOut = new ArmoredOutputStream(publicOut);
            }

            PgpPublicKey key = secretKey.PublicKey;

            key.Encode(publicOut);

            publicOut.Close();
        }
    }
}
