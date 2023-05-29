using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography.Xml;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace CERTSSL
{
    //=====================================================================================
    //          Date    : 06/12/2012
    //          Author  : thinhdt
    //          Perpose : This class is built to implement interface ICrypto.
    //          Modified date 26/04/2014: add function sign by HSM
    //=====================================================================================
    public sealed class Crypto
    {
        #region Private variables

        private string enc_publickey = "";
        private string enc_privatekey = "";
        private string enc_passpharse = "";

        private string sign_publickey = "";// System.Configuration.ConfigurationManager.AppSettings["cert"];
        private string sign_privatekey = "";// System.Configuration.ConfigurationManager.AppSettings["private"];
        private string sign_passpharse = "";//System.Configuration.ConfigurationManager.AppSettings["sign_passpharse"];
        private string hashAlgorithm = "SHA1"; //"MD5";

        //Added by thinhdt - date 16/08/2014
        private bool _ignore_expire_date = false;

        //Added by thinhdt - date 04/06/2015
        private Encoding _encoding = Encoding.UTF8;//Default

        //cert = GetCertificate2FromFile(sign_privatekey, sign_passpharse);
        public Encoding ENCODING
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        //Added by thinhdt - date 20/07/2015

        bool _UsingX509KeyStorageFlags = false;

        public bool UsingX509KeyStorageFlags
        {
            get { return _UsingX509KeyStorageFlags; }
            set { _UsingX509KeyStorageFlags = value; }
        }

        X509KeyStorageFlags _X509KeyStorageFlags = X509KeyStorageFlags.MachineKeySet;

        public X509KeyStorageFlags X509KeyStorageFLAGS
        {
            get { return _X509KeyStorageFlags; }
            set { _X509KeyStorageFlags = value; }
        }

        /// <summary>
        /// This property is used to ignore check expire date of certificate when set value true.
        /// </summary>
        public bool IGNORE_EXPIRE_DATE
        {
            get { return _ignore_expire_date; }
            set { _ignore_expire_date = value; }
        }

        /// <summary>
        /// Used to set hash algorithm (SHA1, MD5) for signing data; Default is 'SHA1'.
        /// </summary>
        public string HASH_ALGORITHM
        {
            get { return hashAlgorithm; }
            set { hashAlgorithm = value; }
        }

        UTF8Encoding encoding = null;
        RSACryptoServiceProvider rsaCSP = null;
        private X509Certificate2 cert = null;

        XmlDocument xmlDoc = null;
        // SignedXml signedXML = null;
        //KeyInfo keyinfo = null;
        //KeyInfoX509Data keyInfoData = null;
        //Reference reference = null;

        RSAPKCS1SignatureDeformatter formater;
        MD5CryptoServiceProvider md5;
        SHA1CryptoServiceProvider sha1;

        //Added by thinhdt - Date 25/04/2014
        private string m_strHSM_IP = "10.0.43.31";
        private string m_strHSM_Port = "01500";
        //HSM8000 HSM8000 = null;
        StreamReader sr = null;//Read private key pem
        //Added by thinhdt - Date 26/04/2014
        private bool m_CheckExpiredDate = false;
        private bool m_pemFormat = false;
        //Added by thinhdt - Date 28/04/2014
        private bool m_EnableHSM = false;
        private bool m_EnableVerifyHash = false;

        //Added by thinhdt - Date 29/04/2014
        private string m_strAESPassWord = "daotungthinh@gmail.com";
        /// <summary>
        /// Password for encrypting and decrypting AES.
        /// </summary>
        public string AES_PASSWORD
        {
            get { return m_strAESPassWord; }
            set { m_strAESPassWord = value; }
        }
        private int m_iAESBit = 128;
        /// <summary>
        /// 128, 192 or 256 for algorithm.
        /// </summary>
        public int AES_BIT
        {
            get { return m_iAESBit; }
            set { m_iAESBit = value; }
        }



        /// <summary>
        /// This property used to enable verifyhash for VerifyData().
        /// </summary>
        public bool ENABLE_VERIFY_HASH
        {
            get { return m_EnableVerifyHash; }
            set { m_EnableVerifyHash = value; }
        }


        /// <summary>
        /// This property used to set HSM for signing.
        /// Default is false.
        /// </summary>
        public bool ENABLE_HSM
        {
            get { return m_EnableHSM; }
            set { m_EnableHSM = value; }
        }

        public bool PEM_FORMAT
        {
            get { return m_pemFormat; }
            set { m_pemFormat = value; }
        }
        #endregion Private variables

        #region Properties


        /// <summary>
        /// Public key used to Verify functions (format *.cer, *.der).
        /// </summary>
        public string SIGN_PUBLICKEY
        {
            get { return sign_publickey; }
            set { sign_publickey = value; }
        }
        /// <summary>
        /// Private key used to Sign functions (format *.p12).
        /// </summary>
        public string SIGN_PRIVATEKEY
        {
            get { return sign_privatekey; }
            set { sign_privatekey = value; }
        }
        /// <summary>
        /// Password used to protect sign private key.
        /// </summary>
        public string SIGN_PASSPHARSE
        {
            get { return sign_passpharse; }
            set { sign_passpharse = value; }
        }
        /// <summary>
        /// Used to set check expire date of certificate; Default is false.
        /// </summary>
        public bool CHECK_EXPIRE_DATE
        {
            get { return m_CheckExpiredDate; }
            set { m_CheckExpiredDate = value; }
        }


        #endregion Properties

        #region Contructor, Destructor

        public Crypto()
        {

        }

        ~Crypto()
        {

        }


        #endregion Contructor, Destructor

        #region ICrypto Members

        #region EncryptData,SignData




        /// <summary>
        /// This function used to sign data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SignData(string data, string str_privatekey, string pass)
        {
            try
            {
                //Added by thinhdt - Date 28/04/2014

                sign_passpharse = pass; //"123456";
                encoding = new UTF8Encoding();
                rsaCSP = new RSACryptoServiceProvider();
                cert = GetCertificate2FromFile(str_privatekey, sign_passpharse);
                rsaCSP = GetPrivateKey(cert);
                //return Convert.ToBase64String(rsaCSP.SignData(encoding.GetBytes(data), "SHA1"));
                return Convert.ToBase64String(rsaCSP.SignData(encoding.GetBytes(data), hashAlgorithm));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cert = null;
                rsaCSP = null;
                encoding = null;
            }
        }

        /// <summary>
        /// This function used to sign data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SignDataSHA256(string data, string str_privatekey, string pass)
        {
            try
            {                             
                SHA256Managed shHash = new SHA256Managed();
                byte[] computedHash = shHash.ComputeHash(Encoding.Default.GetBytes(data));

                sign_passpharse = pass; //"123456";
                encoding = new UTF8Encoding();
                rsaCSP = new RSACryptoServiceProvider();
                UsingX509KeyStorageFlags = true;
                cert = GetCertificate2FromFile(str_privatekey, sign_passpharse);
                rsaCSP = GetPrivateKey(cert);               
                var rsaClear = new RSACryptoServiceProvider();
                var paras = rsaCSP.ExportParameters(true);
                rsaClear.ImportParameters(paras);
                //return Convert.ToBase64String(rsaClear.SignData(computedHash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
                return Convert.ToBase64String(rsaClear.SignData(encoding.GetBytes(data), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cert = null;
                rsaCSP = null;
                encoding = null;
            }
        }
        //public static string DecryptUsingCertificate(string data)
        //{
        //    try
        //    {
        //        byte[] byteData = Convert.FromBase64String(data);
        //        string path = Path.Combine(_hostEnvironment.WebRootPath, "mycertprivatekey.pfx");
        //        var Password = "123"; //Note This Password is That Password That We Have Put On Generate Keys  
        //        var collection = new X509Certificate2Collection();
        //        collection.Import(System.IO.File.ReadAllBytes(path), Password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
        //        X509Certificate2 certificate = new X509Certificate2();
        //        certificate = collection[0];
        //        foreach (var cert in collection)
        //        {
        //            if (cert.FriendlyName.Contains("my certificate"))
        //            {
        //                certificate = cert;
        //            }
        //        }
        //        if (certificate.HasPrivateKey)
        //        {
        //            RSA csp = (RSA)certificate.PrivateKey;
        //            var privateKey = certificate.PrivateKey as RSACryptoServiceProvider;
        //            var keys = Encoding.UTF8.GetString(csp.Decrypt(byteData, RSAEncryptionPadding.OaepSHA1));
        //            return keys;
        //        }
        //    }
        //    catch (Exception ex) { }
        //    return null;
        //}


        public string SignDataXML(string pathfileXML, string pathSign, string str_privatekey, string pass)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.Load(pathfileXML);
                var t = new System.Security.Cryptography.Xml.XmlDsigC14NTransform();
                t.LoadInput(doc);
                var s = (Stream)t.GetOutput(typeof(Stream));
                //var sha1 = SHA1.Create();
                string base64String = "";
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    var hashData = sha1.ComputeHash(s);
                    base64String = Convert.ToBase64String(hashData);
                }

                s.Close();

                //return base64String;
                //var serializer = new System.Security.Cryptography.Xml.XmlDsigC14NTransform();
                //serializer.LoadInput(doc);
                //string c14n = new StreamReader((Stream)serializer.GetOutput(typeof(Stream))).ReadToEnd();

                //var sha1 = SHA1.Create();

                //var hash = sha1.ComputeHash(s);

                //byte[] hashData = sha1.ComputeHash(Encoding.UTF8.GetBytes(hash));
                //var hashbase64String = Convert.ToBase64String(hashData);
                sign_passpharse = pass; //"123456";
                encoding = new UTF8Encoding();
                rsaCSP = new RSACryptoServiceProvider();
                cert = GetCertificate2FromFile(str_privatekey, sign_passpharse);
                rsaCSP = GetPrivateKey(cert);

                string hashTest64 = "CHSNobiIFUGoBHI7QnqvCsSWfVU=";
                byte[] DataSign = rsaCSP.SignData(Convert.FromBase64String(hashTest64), hashAlgorithm);
                //return Convert.ToBase64String(rsaCSP.SignData(encoding.GetBytes(data), "SHA1"));

                Signature XMLSignature = new Signature();
                Reference reference = new Reference
                {
                    Uri = ""
                };

                //reference.DigestMethod = "";
                reference.DigestValue = Convert.FromBase64String(hashTest64);

                XmlDsigEnvelopedSignatureTransform transform = new XmlDsigEnvelopedSignatureTransform();
                XmlDsigEnvelopedSignatureTransform env = new System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform(true);

                //transform = SignedXml.XmlDsigRSASHA1Url;
                //reference.AddTransform(env);
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigExcC14NTransform());
                reference.DigestMethod = SignedXml.XmlDsigSHA1Url;

                XMLSignature.SignedInfo = new SignedInfo();
                XMLSignature.SignedInfo.AddReference(reference);
                XMLSignature.SignedInfo.CanonicalizationMethod = @"http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
                XMLSignature.SignedInfo.SignatureMethod = @"http://www.w3.org/2000/09/xmldsig#rsa-sha1";

                KeyInfo keyInfo = new KeyInfo();
                KeyInfoX509Data keyInfoData = new KeyInfoX509Data(cert);
                keyInfo.AddClause(keyInfoData);
                XMLSignature.KeyInfo = keyInfo;

                XMLSignature.SignatureValue = DataSign;
                XmlElement node = XMLSignature.GetXml();
                doc.DocumentElement.AppendChild(doc.ImportNode(node, true));

                //XmlTextWriter xmltw1 = new XmlTextWriter(pathSign, new UTF8Encoding(false));
                //doc.WriteTo(xmltw1);
                doc.Save(pathSign);
                return Convert.ToBase64String(DataSign);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cert = null;
                rsaCSP = null;
                encoding = null;
            }
        }
        public string SignDataXML2(string pathfileXML, string pathSign, string str_privatekey, string pass)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.Load(pathfileXML);
                sign_passpharse = pass; //"123456";

                rsaCSP = new RSACryptoServiceProvider();
                cert = GetCertificate2FromFile(str_privatekey, sign_passpharse);
                rsaCSP = GetPrivateKey(cert);

                SignedXml xmlSign = new SignedXml(doc)
                {
                    SigningKey = rsaCSP
                };
                Signature XMLSignature = new Signature();
                Reference reference = new Reference();
                reference.Uri = "";

                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform(true);
                reference.AddTransform(env);
                //var serializer = new System.Security.Cryptography.Xml.XmlDsigC14NTransform();
                //serializer.LoadInput(doc);
                //string c14n = new StreamReader((Stream)serializer.GetOutput(typeof(Stream))).ReadToEnd();

                if (true)
                {
                    XmlDsigC14NTransform c14t = new XmlDsigC14NTransform();
                    reference.AddTransform(c14t);
                }
                KeyInfo keyInfo = new KeyInfo();

                KeyInfoX509Data keyInfoData = new KeyInfoX509Data(cert);
                keyInfo.AddClause(keyInfoData);
                xmlSign.KeyInfo = keyInfo;

                // Add the reference to the SignedXml object.
                xmlSign.AddReference(reference);

                // Compute the signature.
                xmlSign.ComputeSignature();
                XmlElement xmlDigitalSignature = xmlSign.GetXml();
                doc.DocumentElement.AppendChild(xmlDigitalSignature);
                ////XmlTextWriter xmltw1 = new XmlTextWriter(pathSign, new UTF8Encoding(false));
                ////doc.WriteTo(xmltw1);
                ///
                doc.Save(pathSign);

                XmlNodeList nodeList = xmlDigitalSignature.GetElementsByTagName("DigestValue");
                string hash = nodeList[0].InnerText;

                return doc.OuterXml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cert = null;
                rsaCSP = null;
                encoding = null;
            }
        }
        // algorithm and return the result.
        public bool VerifyDataXml(string pathfile, string publickey)
        {
            encoding = new UTF8Encoding();
            rsaCSP = new RSACryptoServiceProvider();

            //byte[] bufData = encoding.GetBytes(data);

            //byte[] bufSigned = Convert.FromBase64String(signedData);

            cert = GetCertificate2FromFile(publickey, "");
            rsaCSP = GetPublicKey(cert);
            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument();

            // Load an XML file into the XmlDocument object.
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(pathfile);

            //XmlHash(doc);
            var serializer = new System.Security.Cryptography.Xml.XmlDsigC14NTransform();
            serializer.LoadInput(xmlDoc);
            string c14n = new StreamReader((Stream)serializer.GetOutput(typeof(Stream))).ReadToEnd();

            //string s = node.OuterXml;

            // The XmlDsigC14NTransform will strip the UTF8 BOM
            string result = "";
            using (MemoryStream msIn = new MemoryStream(Encoding.UTF8.GetBytes(xmlDoc.OuterXml)))
            {
                System.Security.Cryptography.Xml.XmlDsigC14NTransform t = new System.Security.Cryptography.Xml.XmlDsigC14NTransform(true);
                t.LoadInput(msIn);

                using (var hash2 = new SHA1Managed())
                {
                    byte[] digest = t.GetDigestedOutput(hash2);
                    result = Convert.ToBase64String(digest);
                    //result = BitConverter.ToString(digest).Replace("-", String.Empty);
                }
            }

            XmlNodeList nodeListcer = xmlDoc.GetElementsByTagName("X509Certificate");

            // Load the signature node.
            if (nodeListcer.Count > 0)
            {
                string baseCer = nodeListcer[0].InnerText;
                Org.BouncyCastle.X509.X509Certificate c = GetX509Cert(baseCer);
            }

            //X509Certificate

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.
            signedXml.LoadXml((XmlElement)nodeList[0]);
            XmlNodeList nodeListVal = xmlDoc.GetElementsByTagName("SignatureValue");


            XmlNodeList nodeListDig = xmlDoc.GetElementsByTagName("DigestValue");

            //byte[] hash = Convert.FromBase64String(nodeListDig[0].InnerText);
            byte[] hash = Convert.FromBase64String(result);
            byte[] sig = Convert.FromBase64String(nodeListVal[0].InnerText);
            bool bltest = rsaCSP.VerifyHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm), sig);
            string hsstest = nodeListVal[0].InnerText;
            return bltest;
            // Check the signature and return the result.
            return signedXml.CheckSignature(rsaCSP);
        }
        public static Org.BouncyCastle.X509.X509Certificate GetX509Cert(string CertStr)
        {
            try
            {
                System.Security.Cryptography.X509Certificates.X509Certificate cert = new System.Security.Cryptography.X509Certificates.X509Certificate();
                //byte[] rawData = GetBytes(CertStr);
                //cert.Import(rawData);
                string strPath = createBase64ToFile(CertStr);
                cert = new System.Security.Cryptography.X509Certificates.X509Certificate(File.ReadAllBytes(strPath));
                Org.BouncyCastle.X509.X509Certificate certBouncyCastle = DotNetUtilities.FromX509Certificate(cert);
                //File.Delete(strPath);
                return certBouncyCastle;
            }
            catch (Exception e)
            {
                throw new Exception("Error get Chain cert: " + e.Message);
            }
        }
        public static string createBase64ToFile(string str)
        {
            //string strPath = Directory.GetCurrentDirectory();
            //if (!File.Exists(strPath + "/upload"))
            //{
            //    System.IO.Directory.CreateDirectory(strPath + "/upload");
            //}
            string strName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string file = @"D:\Working\Projects\EPAY_BE\CERTSSL\Files\" + strName + ".cer";
            File.WriteAllBytes(file, Convert.FromBase64String(str));
            return file;
        }
        /// <summary>
        /// This function used to sign data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SignData(string data, ref string digestValue)
        {
            try
            {
                //Added by thinhdt - Date 28/04/2014

                encoding = new UTF8Encoding();
                rsaCSP = new RSACryptoServiceProvider();

                byte[] tmpBuf = encoding.GetBytes(data);

                //Compute hash based on source data
                sha1 = new SHA1CryptoServiceProvider();
                tmpBuf = sha1.ComputeHash(tmpBuf);

                digestValue = Convert.ToBase64String(tmpBuf);

                cert = GetCertificate2FromFile(sign_privatekey, sign_passpharse);
                rsaCSP = GetPrivateKey(cert);
                //return Convert.ToBase64String(rsaCSP.SignData(encoding.GetBytes(data), "SHA1"));
                return Convert.ToBase64String(rsaCSP.SignData(encoding.GetBytes(data), hashAlgorithm));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cert = null;
                rsaCSP = null;
                encoding = null;
                sha1 = null;
            }
        }


        public string SignDataXML(string data, ref string digestValue)
        {
            try
            {
                //Added by thinhdt - Date 28/04/2014

                encoding = new UTF8Encoding();
                rsaCSP = new RSACryptoServiceProvider();

                byte[] tmpBuf = encoding.GetBytes(data);

                //Compute hash based on source data
                sha1 = new SHA1CryptoServiceProvider();
                tmpBuf = sha1.ComputeHash(tmpBuf);

                digestValue = Convert.ToBase64String(tmpBuf);

                cert = GetCertificate2FromFile(sign_privatekey, sign_passpharse);
                rsaCSP = GetPrivateKey(cert);
                //return Convert.ToBase64String(rsaCSP.SignData(encoding.GetBytes(data), "SHA1"));
                return Convert.ToBase64String(rsaCSP.SignData(encoding.GetBytes(data), hashAlgorithm));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cert = null;
                rsaCSP = null;
                encoding = null;
                sha1 = null;
            }
        }

        /// <summary>
        /// This function used to verify data.
        /// </summary>
        /// <param name="signedData"></param>
        /// <param name="data"></param>
        /// <returns>Return true if verify successfull; else return false.</returns>
        public bool VerifyData(string signedData, string data, string publickey)
        {
            try
            {

                encoding = new UTF8Encoding();
                rsaCSP = new RSACryptoServiceProvider();

                byte[] bufData = encoding.GetBytes(data);

                byte[] bufSigned = Convert.FromBase64String(signedData);

                cert = GetCertificate2FromFile(publickey, "");
                rsaCSP = GetPublicKey(cert);
                ////Added by thinhdt date 26/04/2014
                ////For checking expire date of certificate
                if (m_CheckExpiredDate)
                {
                    DateTime expireDate = DateTime.Parse(cert.GetExpirationDateString());
                    if (expireDate.CompareTo(DateTime.Now) <= 0)
                    {
                        throw new Exception("VerifyData() failed by certificate is expired!");
                    }
                }
                //Added by thinhdt date 28/04/2014
                //For checking enable verifyhash
                if (m_EnableVerifyHash)
                {
                    if (hashAlgorithm == "SHA1")
                    {
                        sha1 = new SHA1CryptoServiceProvider();

                        byte[] hash = sha1.ComputeHash(bufData);
                        return rsaCSP.VerifyHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm), bufSigned);
                    }
                    else
                    {
                        SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                        byte[] hash = sha256.ComputeHash(bufData);
                        return rsaCSP.VerifyHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm), bufSigned);
                    }

                }

                //return rsaCSP.VerifyData(bufData, "SHA1", bufSigned);
                return rsaCSP.VerifyData(bufData, hashAlgorithm, bufSigned);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rsaCSP = null;
                encoding = null;
                sha1 = null;
            }
        }
        /// <summary>
        /// This function used to verify data.
        /// </summary>
        /// <param name="signedData"></param>
        /// <param name="data"></param>
        /// <returns>Return true if verify successfull; else return false.</returns>
        public bool VerifyDataSHA256(string signedData, string data, string publickey)
        {
            hashAlgorithm = "SHA256";
            try
            {

                encoding = new UTF8Encoding();
                rsaCSP = new RSACryptoServiceProvider();

                byte[] bufData = encoding.GetBytes(data);

                byte[] bufSigned = Convert.FromBase64String(signedData);

                cert = GetCertificate2FromFile(publickey, "");
                rsaCSP = GetPublicKey(cert);
                ////Added by thinhdt date 26/04/2014
                ////For checking expire date of certificate
                //if (m_CheckExpiredDate)
                //{
                //    DateTime expireDate = DateTime.Parse(cert.GetExpirationDateString());
                //    if (expireDate.CompareTo(DateTime.Now) <= 0)
                //    {
                //        throw new Exception("VerifyData() failed by certificate is expired!");
                //    }
                //}
                //Added by thinhdt date 28/04/2014
                //For checking enable verifyhash
                if (m_EnableVerifyHash)
                {
                    sha1 = new SHA1CryptoServiceProvider();

                    byte[] hash = sha1.ComputeHash(bufData);
                    return rsaCSP.VerifyHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm), bufSigned);
                }

                //return rsaCSP.VerifyData(bufData, "SHA1", bufSigned);
                return rsaCSP.VerifyData(bufData, hashAlgorithm, bufSigned);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rsaCSP = null;
                encoding = null;
                sha1 = null;
            }
        }








        private RSACryptoServiceProvider ReadPublicKey()
        {
            // read the XML formated public key
            try
            {
                string modStr = "";
                string expStr = "";
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                XmlTextReader reader = new XmlTextReader(sign_publickey);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "Modulus")
                        {
                            reader.Read();
                            modStr = reader.Value;
                        }
                        else if (reader.Name == "Exponent")
                        {
                            reader.Read();
                            expStr = reader.Value;
                        }
                    }
                }
                if (modStr.Equals("") || expStr.Equals(""))
                {
                    //throw exception
                    throw new Exception("Invalid public key");
                }
                RSAKeyInfo.Modulus = Convert.FromBase64String(modStr);
                RSAKeyInfo.Exponent = Convert.FromBase64String(expStr);
                rsa.ImportParameters(RSAKeyInfo);
                return rsa;
            }
            catch (Exception e)
            {
                throw new Exception("Invalid Public Key.");
            }
            finally
            {
                //rsa = null;
            }
        }

        //==============================================
        //      Date        : 15/08/2013
        //      Author      : thinhdt
        //==============================================
        /// <summary>
        /// This function used to verify data signed by publickey xml
        /// </summary>
        /// <param name="data"></param>
        /// <param name="signedData"></param>
        /// <returns></returns>
        public bool VerifySignature(string signedData, string data)
        {
            try
            {
                rsaCSP = LoadPublicKeyXML(sign_publickey);

                byte[] signature = Convert.FromBase64String(signedData);
                formater = new RSAPKCS1SignatureDeformatter(rsaCSP);
                bool result = false;
                if (hashAlgorithm.Equals("MD5"))
                {
                    md5 = new MD5CryptoServiceProvider();
                    byte[] hash = md5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(data));
                    formater.SetHashAlgorithm("MD5");
                    result = formater.VerifySignature(hash, signature);
                }
                else if (hashAlgorithm.Equals("SHA1"))
                {
                    sha1 = new SHA1CryptoServiceProvider();
                    byte[] hash = sha1.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(data));
                    formater.SetHashAlgorithm("SHA1");
                    result = formater.VerifySignature(hash, signature);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                md5 = null;
                sha1 = null;
                formater = null;
            }

        }


        #endregion EncryptData,SignData







        #endregion

        #region Utils

        private RSACryptoServiceProvider GetPublicKey(X509Certificate2 cert)
        {
            try
            {
                PublicKey publickey = cert.PublicKey;
                return (RSACryptoServiceProvider)publickey.Key;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private RSACryptoServiceProvider GetPrivateKey(X509Certificate2 cert)
        {
            try
            {
                //var privKey = (RSACryptoServiceProvider)cert.PrivateKey;
                //// Force use of the Enhanced RSA and AES Cryptographic Provider with openssl-generated SHA256 keys
                //var enhCsp = new RSACryptoServiceProvider().CspKeyContainerInfo;

                //if (!Enum.TryParse<KeyNumber>(privKey.CspKeyContainerInfo.KeyNumber.ToString(), out var keyNumber))
                //    throw new Exception($"Unknown key number {privKey.CspKeyContainerInfo.KeyNumber}");

                //var cspparams = new CspParameters(enhCsp.ProviderType, enhCsp.ProviderName, privKey.CspKeyContainerInfo.KeyContainerName)
                //{
                //    KeyNumber = (int)keyNumber,
                //    Flags = CspProviderFlags.UseExistingKey
                //};

                //privKey = new RSACryptoServiceProvider(cspparams);
               //return privKey;
               return (RSACryptoServiceProvider)cert.PrivateKey;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCertString(string filePath)
        {
            try
            {

                sr = new StreamReader(filePath);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("GetCertString(): " + ex.Message);
            }
        }
        private X509Certificate2 GetCertificate2FromFile(string filePath, string passWord)
        {
            try
            {
                X509Certificate2 cert = null;

                //Added by thinhdt - Date 26/04/2014
                if (m_pemFormat)
                {

                    string sPublicKey = GetCertString(filePath);

                    //Modified by thinhdt - Date 26/04/2014
                    string cf = sPublicKey.Replace("-----BEGIN CERTIFICATE-----", "");
                    cf = cf.Replace("-----END CERTIFICATE-----", "");
                    byte[] bInput;
                    bInput = Convert.FromBase64String(cf);
                    cert = new X509Certificate2();
                    if (string.IsNullOrEmpty(passWord))
                    {
                        cert.Import(bInput);
                    }
                    else
                    {
                        cert.Import(bInput);
                        //cert.Import(bInput, passWord, X509KeyStorageFlags.Exportable);
                    }

                    return cert;
                }

                if (passWord != "")
                {
                    if (_UsingX509KeyStorageFlags == false)
                    {
                        cert = new X509Certificate2(filePath, passWord);
                    }
                    else
                    {
                        //For Windows 2008 64bit
                        cert = new X509Certificate2(filePath, passWord, X509KeyStorageFlags.Exportable);//X509KeyStorageFlags.MachineKeySet
                    }
                }
                else
                {
                    cert = new X509Certificate2(filePath);
                }

                return cert;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private X509Certificate2 GetCertificate2FromFile(string filePath, string passWord, bool bExport)
        {
            try
            {
                X509Certificate2 cert = null;

                if (passWord != "")
                {
                    if (bExport)
                    {
                        cert = new X509Certificate2(filePath, passWord, X509KeyStorageFlags.Exportable);
                    }
                    else
                    {

                        if (_UsingX509KeyStorageFlags == false)
                        {
                            cert = new X509Certificate2(filePath, passWord);
                        }
                        else
                        {
                            //For Windows 2008 64bit
                            cert = new X509Certificate2(filePath, passWord, _X509KeyStorageFlags);//X509KeyStorageFlags.MachineKeySet
                        }
                    }
                }
                else
                {
                    cert = new X509Certificate2(filePath);
                }

                return cert;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //------------------------------------------------------
        //      Date        : 15/08/2013
        //      Author      : thinhdt
        //      Perpose     : Load public key from XML file.
        //------------------------------------------------------
        private RSACryptoServiceProvider LoadPublicKeyXML(String pathXMLfile)
        {
            try
            {
                rsaCSP = new RSACryptoServiceProvider();
                StreamReader sr = File.OpenText(pathXMLfile);
                string rsaXml = sr.ReadToEnd();
                sr.Close();
                rsaCSP.FromXmlString(rsaXml);
                return rsaCSP;
            }
            catch (Exception ex)
            {
                throw new Exception("" + ex.Message);
            }
            finally
            {
                rsaCSP = null;
            }
        }

        private RSACryptoServiceProvider LoadPrivateKeyXML(String pathXMLfile)
        {
            try
            {
                rsaCSP = new RSACryptoServiceProvider();
                StreamReader sr = File.OpenText(pathXMLfile);
                string rsaXml = sr.ReadToEnd();
                sr.Close();
                rsaCSP.FromXmlString(rsaXml);
                return rsaCSP;
            }
            catch (Exception ex)
            {
                throw new Exception("" + ex.Message);
            }
            finally
            {
                rsaCSP = null;
            }
        }


        public string GetDigestValue(string MessageInPut)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            ASCIIEncoding aaa = new ASCIIEncoding();

            byte[] MessageDigestBytes = aaa.GetBytes(MessageInPut);
            byte[] MessageDigestHash = sha1.ComputeHash(MessageDigestBytes);
            string MessageDigestBase64 = Convert.ToBase64String(MessageDigestHash);
            return MessageDigestBase64;
        }

        public string GetDigestValueUTF(string MessageInPut)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            UnicodeEncoding aaa = new UnicodeEncoding();

            byte[] MessageDigestBytes = aaa.GetBytes(MessageInPut);
            byte[] MessageDigestHash = sha1.ComputeHash(MessageDigestBytes);
            string MessageDigestBase64 = Convert.ToBase64String(MessageDigestHash);
            return MessageDigestBase64;
        }
        #endregion Utils

    }
    public class RSAPKCS1SHA256SignatureDescription : SignatureDescription
    {
        public RSAPKCS1SHA256SignatureDescription()
        {
            base.KeyAlgorithm = "System.Security.Cryptography.RSACryptoServiceProvider";
            base.DigestAlgorithm = "System.Security.Cryptography.SHA256Managed";
            base.FormatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureFormatter";
            base.DeformatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureDeformatter";
        }

        public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
        {
            AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = (AsymmetricSignatureDeformatter)
                CryptoConfig.CreateFromName(base.DeformatterAlgorithm);
            asymmetricSignatureDeformatter.SetKey(key);
            asymmetricSignatureDeformatter.SetHashAlgorithm("SHA256");
            return asymmetricSignatureDeformatter;
        }
    }
}
