using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace EVNSample
{
    public class PGPDecryptVerify
    {
        public static HashAlgorithmTag HashAlgorithmTag
        {
            get;
            set;
        }
        public static void Decrypt(string filePath, string privateKeyFile, string passPhrase, string outputFilePath)
        {
            Stream fin = File.OpenRead(filePath);
            Stream keyIn = File.OpenRead(privateKeyFile);
            DecryptFile(fin, keyIn, passPhrase.ToCharArray(), outputFilePath);
            fin.Close();
            keyIn.Close();
        }

        private static void DecryptFile(Stream inputStream, Stream keyIn, char[] passwd, string outputFilePath)
        {
            //var aaa = inputStream;
            //string aaaa = BitConverter.ToString(StreamToByteArray(aaa)).Replace("-", "");
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            //string bbbb = BitConverter.ToString(StreamToByteArray(inputStream)).Replace("-", "");
            try
            {
                // dùng để warp các đối tượng PGP stream
                PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);

                // lấy đối tượng tiếp theo
                PgpObject o = pgpF.NextPgpObject();

                PgpEncryptedDataList enc;
                if (o is PgpEncryptedDataList list)
                {
                    enc = list;
                }
                else
                {
                    enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
                }

                PgpPrivateKey privateKey = null;
                PgpPublicKeyEncryptedData pbe = null;
                PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(keyIn));

                foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
                {
                    privateKey = ReadPrivateKey(pgpSec, pked.KeyId, passwd);
                    if (privateKey != null)
                    {
                        pbe = pked;
                        break;
                    }
                }
                if (privateKey == null)
                {
                    throw new ArgumentException("Secret key for message not found.");
                }
                Stream clear = pbe.GetDataStream(privateKey);
                PgpObjectFactory plainFact = new PgpObjectFactory(clear);
                PgpObject message = plainFact.NextPgpObject();
                if (message is PgpCompressedData cData)
                {
                    PgpObjectFactory pgpFact = new PgpObjectFactory(cData.GetDataStream());
                    message = pgpFact.NextPgpObject();
                }

                if (message is PgpLiteralData)
                {
                    PgpLiteralData ld = (PgpLiteralData)message;
                    string outFileName = ld.FileName;
                    Stream fOut = File.Create(outputFilePath);
                    Stream unc = ld.GetInputStream();
                    Streams.PipeAll(unc, fOut);
                    fOut.Close();
                }
                else if (message is PgpOnePassSignatureList)
                {
                    throw new PgpException("Encrypted message contains a signed message - not literal data.");
                }
                else
                {
                    throw new PgpException("Message is not a simple encrypted file - type unknown.");
                }

            }
            catch (PgpException e)
            {
                //Logger.LogException(e);
            }

        }

        private static PgpPrivateKey ReadPrivateKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] pass)
        {
            PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyId);
            if (pgpSecKey == null)
            {
                return null;
            }
            return pgpSecKey.ExtractPrivateKey(pass);
        }

        private static bool VerifyFile(Stream inputStream, Stream keyIn, string fileOutput)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            PgpObjectFactory pgpFact = new PgpObjectFactory(inputStream);
            //var abc = pgpFact.NextPgpObject();
            PgpCompressedData c1 = (PgpCompressedData)pgpFact.NextPgpObject();
            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();
            PgpOnePassSignature ops = p1[0];

            PgpLiteralData p2 = (PgpLiteralData)pgpFact.NextPgpObject();
            Stream dIn = p2.GetInputStream();
            PgpPublicKeyRingBundle pgpRing = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(keyIn));
            PgpPublicKey key = pgpRing.GetPublicKey(ops.KeyId);
            //Stream fos = File.Create(p2.FileName);

            Stream fos = File.Create(fileOutput);

            ops.InitVerify(key);

            int ch;
            while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
                fos.WriteByte((byte)ch);
            }
            fos.Close();

            PgpSignatureList p3 = (PgpSignatureList)pgpFact.NextPgpObject();
            PgpSignature firstSig = p3[0];
            if (ops.Verify(firstSig))
            {
                Console.Out.WriteLine("signature verified.");
                return true;
            }
            else
            {
                Console.Out.WriteLine("signature verification failed.");
                return false;
            }
        }

        public static bool Verify(string filePath, string publicKeyFile, string fileoutput)
        {
            Stream input = File.OpenRead(filePath);
            Stream keyIn = File.OpenRead(publicKeyFile);
            var result = VerifyFile(input, keyIn, fileoutput);
            keyIn.Close();
            input.Close();
            return result;
        }

        public static bool DecryptAndVerify(string filePath, string secretKeyFile, string passPhrase, string publicKeyFile, string outputFilePath)
        {
            try
            {
                using (Stream fin = File.OpenRead(filePath))
                {
                    using (Stream secretkeyIn = File.OpenRead(secretKeyFile))
                    {
                        using (Stream publickeyIn = File.OpenRead(publicKeyFile))
                        {
                            return DecryptandVerifyFile(fin, secretkeyIn, passPhrase.ToCharArray(), publickeyIn, outputFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Utils.WriteLog("Error :" + '\t' + "Decrypt file error : " + filePath, ex.Message);
                return false;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static bool DecryptandVerifyFile(Stream inputStream, Stream secretkeyIn, char[] passwd, Stream publicKeyIn, string outputFilePath)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            //string result3 = Encoding.ASCII.GetString(ReadFully(inputStream));

            PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
            PgpEncryptedDataList enc;
            PgpObject o = pgpF.NextPgpObject();
            if (o is PgpEncryptedDataList list)
            {
                enc = list;
            }
            else
            {
                enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
            }

            PgpPrivateKey sKey = null;
            PgpPublicKeyEncryptedData pbe = null;
            PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(secretkeyIn));
            foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
            {
                sKey = ReadPrivateKey(pgpSec, pked.KeyId, passwd);
                if (sKey != null)
                {
                    pbe = pked;
                    break;
                }
            }
            if (sKey == null)
            {
                throw new ArgumentException("Secret key for message not found.");
            }
            // get dữ liệu giải mã
            Stream clear = pbe.GetDataStream(sKey);
            //string result4 = Encoding.ASCII.GetString(ReadFully(clear));

            PgpObjectFactory plainFact = new PgpObjectFactory(clear);
            PgpObject message = plainFact.NextPgpObject();
            PgpObjectFactory pgpFact;
            // kiểm tra dữ liệu có nén
            if (message is PgpCompressedData cData)
            {
                // get dữ liệu sau giải nén
                var svc = cData.GetDataStream();
                //string result = Encoding.ASCII.GetString(ReadFully(svc));

                pgpFact = new PgpObjectFactory(svc);
                message = pgpFact.NextPgpObject();
              
            }

            Stream verifyStream = new MemoryStream();
            // kiểm tra dữ liệu tường mình
            if (message is PgpLiteralData data)
            {
                Stream unc = data.GetInputStream();
                Streams.PipeAll(unc, verifyStream);
                verifyStream.Position = 0;
            }
            else if (message is PgpOnePassSignatureList)
            {
                throw new PgpException("Encrypted message contains a signed message - not literal data.");
            }
            else
            {
                throw new PgpException("Message is not a simple encrypted file - type unknown.");
            }


            ////////////////////////////////////////////////
            verifyStream = PgpUtilities.GetDecoderStream(verifyStream);

            pgpFact = new PgpObjectFactory(verifyStream);

            PgpCompressedData c1 = (PgpCompressedData)pgpFact.NextPgpObject();

            //PgpCompressedData c1 = (PgpCompressedData)message;

            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();
            PgpOnePassSignature ops = p1[0];

            PgpLiteralData p2 = (PgpLiteralData)pgpFact.NextPgpObject();
            Stream dIn = p2.GetInputStream();
            PgpPublicKeyRingBundle pgpRing = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(publicKeyIn));
            PgpPublicKey key = pgpRing.GetPublicKey(ops.KeyId);
            //Stream fos = File.Create(p2.FileName);

            ops.InitVerify(key);
            using (Stream fos = File.Create(outputFilePath))
            {
                int ch;
                while ((ch = dIn.ReadByte()) >= 0)
                {
                    ops.Update((byte)ch);
                    fos.WriteByte((byte)ch);
                }
            }
            PgpSignatureList p3 = (PgpSignatureList)pgpFact.NextPgpObject();
            PgpSignature firstSig = p3[0];
            if (ops.Verify(firstSig))
            {
                Console.Out.WriteLine("signature verified.");
                return true;
            }
            else
            {
                Console.Out.WriteLine("signature verification failed.");
                return false;
            }
        }
    }


}
