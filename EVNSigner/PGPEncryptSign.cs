using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;

namespace EVNSample
{
    public static class PGPEncryptSign
    {

        /// <summary>
        /// Hàm mã hoá file 
        /// </summary>
        /// <param name="filePath">file dữ liệu đầu vào</param>
        /// <param name="publicKeyFile">file public key đầu vào</param>
        /// <param name="outputFilePath">đầu ra file mã hoá</param>
        public static void Encrypt(string filePathInput, string publicKeyFileInput, string filePathOutput)
        {
            Stream keyStream = File.OpenRead(publicKeyFileInput);
            PgpPublicKey keyIn = ReadPublicKey(keyStream);
            Stream fos = File.Create(filePathOutput);
            EncryptFile(fos, filePathInput, keyIn, true, true);
            keyStream.Close();
            fos.Close();
        }

        private static PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);
            foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey k in kRing.GetPublicKeys())
                {
                    if (k.IsEncryptionKey)
                    {
                        return k;
                    }
                }
            }
            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        private static PgpSecretKey ReadSecretKey(Stream input)
        {
            PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(
                PgpUtilities.GetDecoderStream(input));

            //
            // we just loop through the collection till we find a key suitable for encryption, in the real
            // world you would probably want to be a bit smarter about this.
            //

            foreach (PgpSecretKeyRing keyRing in pgpSec.GetKeyRings())
            {
                foreach (PgpSecretKey key in keyRing.GetSecretKeys())
                {
                    if (key.IsSigningKey)
                    {
                        return key;
                    }
                }
            }

            throw new ArgumentException("Can't find signing key in key ring.");
        }
        private static void EncryptFile(Stream outputStream, string fileName, PgpPublicKey encKey, bool armor, bool withIntegrityCheck)
        {
            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }

            try
            {
                MemoryStream bOut = new MemoryStream();
                PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                PgpUtilities.WriteFileToLiteralData(comData.Open(bOut), PgpLiteralData.Binary, new FileInfo(fileName));
                comData.Close();

                PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());
                cPk.AddMethod(encKey);

                byte[] bytes = bOut.ToArray();
                Stream cOut = cPk.Open(outputStream, bytes.Length);
                cOut.Write(bytes, 0, bytes.Length);
                cOut.Close();

                if (armor)
                {
                    outputStream.Close();
                }
            }
            catch (PgpException e)
            {
                throw new ArgumentException("Encrypt file error: " +e.Message);
            }
        }

        /// <summary>
        /// Hàm tạo file Hash dùng SHA256
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] CalculateHash(string filePath)
        {
            byte[] hash = null;
            using (HMACSHA256 sha = new HMACSHA256())
            {
                using (FileStream f = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    hash = sha.ComputeHash(f);
                }
            }

            return hash;
        }

        private static void SignFile(string fileName, Stream keyIn, Stream outputStream, char[] pass, bool armor, bool compress)
        {
            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }

            PgpSecretKey pgpSec = ReadSecretKey(keyIn);
            PgpPrivateKey pgpPrivKey = pgpSec.ExtractPrivateKey(pass);
 
            PgpSignatureGenerator sGen = new PgpSignatureGenerator(pgpSec.PublicKey.Algorithm, HashAlgorithmTag.Sha256);

            sGen.InitSign(PgpSignature.BinaryDocument, pgpPrivKey);

            foreach (string userId in pgpSec.PublicKey.GetUserIds())
            {
                PgpSignatureSubpacketGenerator spGen = new PgpSignatureSubpacketGenerator();
                spGen.SetSignerUserId(false, userId);
                sGen.SetHashedSubpackets(spGen.Generate());
                // Just the first one!
                break;
            }

            Stream cOut = outputStream;
            PgpCompressedDataGenerator cGen = null;

            if (compress)
            {
                cGen = new PgpCompressedDataGenerator(CompressionAlgorithmTag.ZLib);

                cOut = cGen.Open(cOut);
            }

            BcpgOutputStream bOut = new BcpgOutputStream(cOut);

            sGen.GenerateOnePassVersion(false).Encode(bOut);

            FileInfo file = new FileInfo(fileName);
            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();
            Stream lOut = lGen.Open(bOut, PgpLiteralData.Binary, file);
            FileStream fIn = file.OpenRead();
            int ch;
            while ((ch = fIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte)ch);
                sGen.Update((byte)ch);
            }

            fIn.Close();
            lGen.Close();

            sGen.Generate().Encode(bOut);

            if (cGen != null)
            {
                cGen.Close();
            }

            if (armor)
            {
                outputStream.Close();
            }
        }

        public static void Sign(string filePath, string secretKeyFile, string passPhrase, string outputFilePath)
        {
            Stream keyIn = File.OpenRead(secretKeyFile);
            Stream fos = File.Create(outputFilePath);
            SignFile(filePath, keyIn, fos, passPhrase.ToCharArray(), true, true);
            keyIn.Close();
            fos.Close();
        }
       
    }
}
