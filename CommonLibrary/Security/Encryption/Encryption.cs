using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System;

// <summary>
// A simple, string-oriented wrapper class for encryption functions, including 
// Hashing, Symmetric Encryption, and Asymmetric Encryption.
// </summary>
// <remarks>
//   Jeff Atwood
//   http://www.codinghorror.com/
// </remarks>
namespace Library.Security.Encryption
{
    #region   Hash

    /// <summary>
    /// Hash functions are fundamental to modern cryptography. These functions map binary 
    /// strings of an arbitrary length to small binary strings of a fixed length, known as 
    /// hash values. A cryptographic hash function has the property that it is computationally
    /// infeasible to find two distinct inputs that hash to the same value. Hash functions 
    /// are commonly used with digital signatures and for data integrity.
    /// </summary>
    public class Hash
    {

        /// <summary>
        /// Type of hash; some are security oriented, others are fast and simple
        /// </summary>
        public enum Provider : int
        {
            /// <summary>
            /// Cyclic Redundancy Check provider, 32-bit
            /// </summary>
            CRC32,
            /// <summary>
            /// Secure Hashing Algorithm provider, SHA-1 variant, 160-bit
            /// </summary>
            SHA1,
            /// <summary>
            /// Secure Hashing Algorithm provider, SHA-2 variant, 256-bit
            /// </summary>
            SHA256,
            /// <summary>
            /// Secure Hashing Algorithm provider, SHA-2 variant, 384-bit
            /// </summary>
            SHA384,
            /// <summary>
            /// Secure Hashing Algorithm provider, SHA-2 variant, 512-bit
            /// </summary>
            SHA512,
            /// <summary>
            /// Message Digest algorithm 5, 128-bit
            /// </summary>
            MD5
        }

        private HashAlgorithm _Hash;
        private Data _HashValue = new Data();

        private Hash()
        {
        }

        /// <summary>
        /// Instantiate a new hash of the specified type
        /// </summary>
        public Hash(Provider p)
        {
            switch (p)
            {
                case Provider.CRC32:
                    _Hash = new CRC32();
                    break;
                case Provider.MD5:
                    _Hash = new MD5CryptoServiceProvider();
                    break;
                case Provider.SHA1:
                    _Hash = new SHA1Managed();
                    break;
                case Provider.SHA256:
                    _Hash = new SHA256Managed();
                    break;
                case Provider.SHA384:
                    _Hash = new SHA384Managed();
                    break;
                case Provider.SHA512:
                    _Hash = new SHA512Managed();
                    break;
            }
        }

        /// <summary>
        /// Returns the previously calculated hash
        /// </summary>
        public Data Value
        {
            get
            {
                return _HashValue;
            }
        }

        /// <summary>
        /// Calculates hash on a stream of arbitrary length
        /// </summary>
        public Data Calculate(ref System.IO.Stream s)
        {
            _HashValue.Bytes = _Hash.ComputeHash(s);
            return _HashValue;
        }

        /// <summary>
        /// Calculates hash for fixed length <see cref="Data"/>
        /// </summary>
        public Data Calculate(Data d)
        {
            return CalculatePrivate(d.Bytes);
        }

        /// <summary>
        /// Calculates hash for a string with a prefixed salt value. 
        /// A "salt" is random data prefixed to every hashed value to prevent 
        /// common dictionary attacks.
        /// </summary>
        public Data Calculate(Data d, Data salt)
        {
            byte[] nb = new byte[d.Bytes.Length + salt.Bytes.Length];
            salt.Bytes.CopyTo(nb, 0);
            d.Bytes.CopyTo(nb, salt.Bytes.Length);
            return CalculatePrivate(nb);
        }

        /// <summary>
        /// Calculates hash for an array of bytes
        /// </summary>
        private Data CalculatePrivate(byte[] b)
        {
            _HashValue.Bytes = _Hash.ComputeHash(b);
            return _HashValue;
        }

        #region   CRC32 HashAlgorithm
        private class CRC32 : HashAlgorithm
        {

            private uint result = 0XFFFFFFFF;

            protected override void HashCore(byte[] array, int ibStart, int cbSize)
            {
                int lookup = 0;
                for (int i = ibStart; i < cbSize; i++)
                {
                    lookup = (int)(result & 0XFF) ^ array[i];
                    result = ((result & 0XFFFFFF00) / 0X100) & 0XFFFFFF;
                    result = result ^ (uint)crcLookup[lookup];
                }
            }

            protected override byte[] HashFinal()
            {
                byte[] b = BitConverter.GetBytes(~result);
                Array.Reverse(b);
                return b;
            }

            public override void Initialize()
            {
                result = 0XFFFFFFFF;
            }

            private int[] crcLookup = { 0X0, 0X77073096, 0X76DC419, 0X706AF48F, 0XEDB8832, 0X79DCB8A4, 0X9B64C2B, 0X7EB17CBD, 0X1DB71064, 0X6AB020F2, 0X1ADAD47D, 0X6DDDE4EB, 0X136C9856, 0X646BA8C0, 0X14015C4F, 0X63066CD9, 0X3B6E20C8, 0X4C69105E, 0X3C03E4D1, 0X4B04D447, 0X35B5A8FA, 0X42B2986C, 0X32D86CE3, 0X45DF5C75, 0X26D930AC, 0X51DE003A, 0X21B4F4B5, 0X56B3C423, 0X5F058808, 0X2F6F7C87, 0X58684C11, 0X1DB7106, 0X6B6B51F, 0XF00F934, 0X86D3D2D, 0X1C6C6162, 0X1B01A57B, 0X12B7E950, 0X15DA2D49, 0X3AB551CE, 0X3DD895D7, 0X346ED9FC, 0X33031DE5, 0X270241AA, 0X206F85B3, 0X29D9C998, 0X2EB40D81, 0X74B1D29A, 0X73DC1683, 0X7A6A5AA8, 0X7D079EB1, 0X6906C2FE, 0X6E6B06E7, 0X67DD4ACC, 0X60B08ED5, 0X4FDFF252, 0X48B2364B, 0X41047A60, 0X4669BE79, 0X5268E236, 0X5505262F, 0X5CB36A04, 0X5BDEAE1D, 0X26D930A, 0X5005713, 0XBDBDF21, 0X1FDA836E, 0X18B74777, 0X66063BCA, 0X11010B5C, 0X616BFFD3, 0X166CCF45, 0X4E048354, 0X3903B3C2, 0X4969474D, 0X3E6E77DB, 0X37D83BF0, 0X30B5FFE9, 0X24B4A3A6, 0X23D967BF, 0X2A6F2B94, 0X2D02EF8D };

            public override byte[] Hash
            {
                get
                {
                    byte[] b = BitConverter.GetBytes(~result);
                    Array.Reverse(b);
                    return b;
                }
            }
        }

        #endregion

    }
    #endregion

    #region   Symmetric

    /// <summary>
    /// Symmetric encryption uses a single key to encrypt and decrypt. 
    /// Both parties (encryptor and decryptor) must share the same secret key.
    /// </summary>
    public class Symmetric
    {

        private const string _DefaultIntializationVector = "%1Az=-@qT";
        private const int _BufferSize = 2048;

        public enum Provider : int
        {
            /// <summary>
            /// The Data Encryption Standard provider supports a 64 bit key only
            /// </summary>
            DES,
            /// <summary>
            /// The Rivest Cipher 2 provider supports keys ranging from 40 to 128 bits, default is 128 bits
            /// </summary>
            RC2,
            /// <summary>
            /// The Rijndael (also known as AES) provider supports keys of 128, 192, or 256 bits with a default of 256 bits
            /// </summary>
            Rijndael,
            /// <summary>
            /// The TripleDES provider (also known as 3DES) supports keys of 128 or 192 bits with a default of 192 bits
            /// </summary>
            TripleDES
        }

        //private Data _data;
        private Data _key;
        private Data _iv;
        private SymmetricAlgorithm _crypto;
        //private byte[] _EncryptedBytes;
        //private bool _UseDefaultInitializationVector;

        private Symmetric()
        {
        }

        /// <summary>
        /// Instantiates a new symmetric encryption object using the specified provider.
        /// </summary>

        public Symmetric(Provider provider)
            : this(provider, true)
        {
        }

        //INSTANT C# NOTE: C# does not support optional parameters. Overloaded method(s) are created above.
        //ORIGINAL LINE: Public Sub new(ByVal provider As Provider, Optional ByVal useDefaultInitializationVector As Boolean = true)
        public Symmetric(Provider provider, bool useDefaultInitializationVector)
        {
            switch (provider)
            {
                case Provider.DES:
                    _crypto = new DESCryptoServiceProvider();
                    break;
                case Provider.RC2:
                    _crypto = new RC2CryptoServiceProvider();
                    break;
                case Provider.Rijndael:
                    _crypto = new RijndaelManaged();
                    break;
                case Provider.TripleDES:
                    _crypto = new TripleDESCryptoServiceProvider();
                    break;
            }

            //-- make sure key and IV are always set, no matter what
            this.Key = RandomKey();
            if (useDefaultInitializationVector)
            {
                this.IntializationVector = new Data(_DefaultIntializationVector);
            }
            else
            {
                this.IntializationVector = RandomInitializationVector();
            }
        }

        /// <summary>
        /// Key size in bytes. We use the default key size for any given provider; if you 
        /// want to force a specific key size, set this property
        /// </summary>
        public int KeySizeBytes
        {
            get
            {
                return _crypto.KeySize / 8;
            }
            set
            {
                _crypto.KeySize = value * 8;
                _key.MaxBytes = value;
            }
        }

        /// <summary>
        /// Key size in bits. We use the default key size for any given provider; if you 
        /// want to force a specific key size, set this property
        /// </summary>
        public int KeySizeBits
        {
            get
            {
                return _crypto.KeySize;
            }
            set
            {
                _crypto.KeySize = value;
                _key.MaxBits = value;
            }
        }

        /// <summary>
        /// The key used to encrypt/decrypt data
        /// </summary>
        public Data Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                _key.MaxBytes = _crypto.LegalKeySizes[0].MaxSize / 8;
                _key.MinBytes = _crypto.LegalKeySizes[0].MinSize / 8;
                _key.StepBytes = _crypto.LegalKeySizes[0].SkipSize / 8;
            }
        }

        /// <summary>
        /// Using the default Cipher Block Chaining (CBC) mode, all data blocks are processed using
        /// the value derived from the previous block; the first data block has no previous data block
        /// to use, so it needs an InitializationVector to feed the first block
        /// </summary>
        public Data IntializationVector
        {
            get
            {
                return _iv;
            }
            set
            {
                _iv = value;
                _iv.MaxBytes = _crypto.BlockSize / 8;
                _iv.MinBytes = _crypto.BlockSize / 8;
            }
        }

        /// <summary>
        /// generates a random Initialization Vector, if one was not provided
        /// </summary>
        public Data RandomInitializationVector()
        {
            _crypto.GenerateIV();
            Data d = new Data(_crypto.IV);
            return d;
        }

        /// <summary>
        /// generates a random Key, if one was not provided
        /// </summary>
        public Data RandomKey()
        {
            _crypto.GenerateKey();
            Data d = new Data(_crypto.Key);
            return d;
        }

        /// <summary>
        /// Ensures that _crypto object has valid Key and IV
        /// prior to any attempt to encrypt/decrypt anything
        /// </summary>
        private void ValidateKeyAndIv(bool isEncrypting)
        {
            if (_key.IsEmpty)
            {
                if (isEncrypting)
                {
                    _key = RandomKey();
                }
                else
                {
                    throw new CryptographicException("No key was provided for the decryption operation!");
                }
            }
            if (_iv.IsEmpty)
            {
                if (isEncrypting)
                {
                    _iv = RandomInitializationVector();
                }
                else
                {
                    throw new CryptographicException("No initialization vector was provided for the decryption operation!");
                }
            }
            _crypto.Key = _key.Bytes;
            _crypto.IV = _iv.Bytes;
        }

        /// <summary>
        /// Encrypts the specified Data using provided key
        /// </summary>
        public Data Encrypt(Data d, Data key)
        {
            this.Key = key;
            return Encrypt(d);
        }

        /// <summary>
        /// Encrypts the specified Data using preset key and preset initialization vector
        /// </summary>
        public Data Encrypt(Data d)
        {
            MemoryStream ms = new MemoryStream();

            ValidateKeyAndIv(true);

            CryptoStream cs = new CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(d.Bytes, 0, d.Bytes.Length);
            cs.Close();
            ms.Close();

            return new Data(ms.ToArray());
        }

        /// <summary>
        /// Encrypts the stream to memory using provided key and provided initialization vector
        /// </summary>
        public Data Encrypt(Stream s, Data key, Data iv)
        {
            this.IntializationVector = iv;
            this.Key = key;
            return Encrypt(s);
        }

        /// <summary>
        /// Encrypts the stream to memory using specified key
        /// </summary>
        public Data Encrypt(Stream s, Data key)
        {
            this.Key = key;
            return Encrypt(s);
        }

        /// <summary>
        /// Encrypts the specified stream to memory using preset key and preset initialization vector
        /// </summary>
        public Data Encrypt(Stream s)
        {
            MemoryStream ms = new MemoryStream();
            byte[] b = new byte[_BufferSize + 1];
            int i = 0;

            ValidateKeyAndIv(true);

            CryptoStream cs = new CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write);
            i = s.Read(b, 0, _BufferSize);
            while (i > 0)
            {
                cs.Write(b, 0, i);
                i = s.Read(b, 0, _BufferSize);
            }

            cs.Close();
            ms.Close();

            return new Data(ms.ToArray());
        }

        /// <summary>
        /// Decrypts the specified data using provided key and preset initialization vector
        /// </summary>
        public Data Decrypt(Data encryptedData, Data key)
        {
            this.Key = key;
            return Decrypt(encryptedData);
        }

        /// <summary>
        /// Decrypts the specified stream using provided key and preset initialization vector
        /// </summary>
        public Data Decrypt(Stream encryptedStream, Data key)
        {
            this.Key = key;
            return Decrypt(encryptedStream);
        }

        /// <summary>
        /// Decrypts the specified stream using preset key and preset initialization vector
        /// </summary>
        public Data Decrypt(Stream encryptedStream)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] b = new byte[_BufferSize + 1];

            ValidateKeyAndIv(false);
            CryptoStream cs = new CryptoStream(encryptedStream, _crypto.CreateDecryptor(), CryptoStreamMode.Read);

            int i = 0;
            i = cs.Read(b, 0, _BufferSize);

            while (i > 0)
            {
                ms.Write(b, 0, i);
                i = cs.Read(b, 0, _BufferSize);
            }
            cs.Close();
            ms.Close();

            return new Data(ms.ToArray());
        }

        /// <summary>
        /// Decrypts the specified data using preset key and preset initialization vector
        /// </summary>
        public Data Decrypt(Data encryptedData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(encryptedData.Bytes, 0, encryptedData.Bytes.Length);
            byte[] b = new byte[encryptedData.Bytes.Length];

            ValidateKeyAndIv(false);
            CryptoStream cs = new CryptoStream(ms, _crypto.CreateDecryptor(), CryptoStreamMode.Read);

            try
            {
                cs.Read(b, 0, encryptedData.Bytes.Length - 1);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException("Unable to decrypt data. The provided key may be invalid.", ex);
            }
            finally
            {
                cs.Close();
            }
            return new Data(b);
        }

    }

    #endregion

    #region   Asymmetric

    /// <summary>
    /// Asymmetric encryption uses a pair of keys to encrypt and decrypt.
    /// There is a "public" key which is used to encrypt. Decrypting, on the other hand, 
    /// requires both the "public" key and an additional "private" key. The advantage is 
    /// that people can send you encrypted messages without being able to decrypt them.
    /// </summary>
    /// <remarks>
    /// The only provider supported is the <see cref="RSACryptoServiceProvider"/>
    /// </remarks>
    public class Asymmetric
    {

        private RSACryptoServiceProvider _rsa;
        private string _KeyContainerName = "Encryption.AsymmetricEncryption.DefaultContainerName";
        //private bool _UseMachineKeystore = true;
        private int _KeySize = 1024;

        private const string _ElementParent = "RSAKeyValue";
        private const string _ElementModulus = "Modulus";
        private const string _ElementExponent = "Exponent";
        private const string _ElementPrimeP = "P";
        private const string _ElementPrimeQ = "Q";
        private const string _ElementPrimeExponentP = "DP";
        private const string _ElementPrimeExponentQ = "DQ";
        private const string _ElementCoefficient = "InverseQ";
        private const string _ElementPrivateExponent = "D";

        //-- http://forum.java.sun.com/thread.jsp?forum=9&thread=552022&tstart=0&trange=15 
        private const string _KeyModulus = "PublicKey.Modulus";
        private const string _KeyExponent = "PublicKey.Exponent";
        private const string _KeyPrimeP = "PrivateKey.P";
        private const string _KeyPrimeQ = "PrivateKey.Q";
        private const string _KeyPrimeExponentP = "PrivateKey.DP";
        private const string _KeyPrimeExponentQ = "PrivateKey.DQ";
        private const string _KeyCoefficient = "PrivateKey.InverseQ";
        private const string _KeyPrivateExponent = "PrivateKey.D";

        #region   PublicKey Class
        /// <summary>
        /// Represents a public encryption key. Intended to be shared, it 
        /// contains only the Modulus and Exponent.
        /// </summary>
        public class PublicKey
        {
            public string Modulus;
            public string Exponent;

            public PublicKey()
            {
            }

            public PublicKey(string KeyXml)
            {
                LoadFromXml(KeyXml);
            }

            /// <summary>
            /// Load public key from App.config or Web.config file
            /// </summary>
            public void LoadFromConfig()
            {
                this.Modulus = Utils.GetConfigString(_KeyModulus);
                this.Exponent = Utils.GetConfigString(_KeyExponent);
            }

            /// <summary>
            /// Returns *.config file XML section representing this public key
            /// </summary>
            public string ToConfigSection()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Utils.WriteConfigKey(_KeyModulus, this.Modulus));
                sb.Append(Utils.WriteConfigKey(_KeyExponent, this.Exponent));
                return sb.ToString();
            }

            /// <summary>
            /// Writes the *.config file representation of this public key to a file
            /// </summary>
            public void ExportToConfigFile(string filePath)
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.Write(this.ToConfigSection());
                sw.Close();
            }

            /// <summary>
            /// Loads the public key from its XML string
            /// </summary>
            public void LoadFromXml(string keyXml)
            {
                this.Modulus = Utils.GetXmlElement(keyXml, "Modulus");
                this.Exponent = Utils.GetXmlElement(keyXml, "Exponent");
            }

            /// <summary>
            /// Converts this public key to an RSAParameters object
            /// </summary>
            public RSAParameters ToParameters()
            {
                RSAParameters r = new RSAParameters();
                r.Modulus = Convert.FromBase64String(this.Modulus);
                r.Exponent = Convert.FromBase64String(this.Exponent);
                return r;
            }

            /// <summary>
            /// Converts this public key to its XML string representation
            /// </summary>
            public string ToXml()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Utils.WriteXmlNode(_ElementParent));
                sb.Append(Utils.WriteXmlElement(_ElementModulus, this.Modulus));
                sb.Append(Utils.WriteXmlElement(_ElementExponent, this.Exponent));
                sb.Append(Utils.WriteXmlNode(_ElementParent, true));
                return sb.ToString();
            }

            /// <summary>
            /// Writes the Xml representation of this public key to a file
            /// </summary>
            public void ExportToXmlFile(string filePath)
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.Write(this.ToXml());
                sw.Close();
            }

        }
        #endregion

        #region   PrivateKey Class

        /// <summary>
        /// Represents a private encryption key. Not intended to be shared, as it 
        /// contains all the elements that make up the key.
        /// </summary>
        public class PrivateKey
        {
            public string Modulus;
            public string Exponent;
            public string PrimeP;
            public string PrimeQ;
            public string PrimeExponentP;
            public string PrimeExponentQ;
            public string Coefficient;
            public string PrivateExponent;

            public PrivateKey()
            {
            }

            public PrivateKey(string keyXml)
            {
                LoadFromXml(keyXml);
            }

            /// <summary>
            /// Load private key from App.config or Web.config file
            /// </summary>
            public void LoadFromConfig()
            {
                this.Modulus = Utils.GetConfigString(_KeyModulus);
                this.Exponent = Utils.GetConfigString(_KeyExponent);
                this.PrimeP = Utils.GetConfigString(_KeyPrimeP);
                this.PrimeQ = Utils.GetConfigString(_KeyPrimeQ);
                this.PrimeExponentP = Utils.GetConfigString(_KeyPrimeExponentP);
                this.PrimeExponentQ = Utils.GetConfigString(_KeyPrimeExponentQ);
                this.Coefficient = Utils.GetConfigString(_KeyCoefficient);
                this.PrivateExponent = Utils.GetConfigString(_KeyPrivateExponent);
            }

            /// <summary>
            /// Converts this private key to an RSAParameters object
            /// </summary>
            public RSAParameters ToParameters()
            {
                RSAParameters r = new RSAParameters();
                r.Modulus = Convert.FromBase64String(this.Modulus);
                r.Exponent = Convert.FromBase64String(this.Exponent);
                r.P = Convert.FromBase64String(this.PrimeP);
                r.Q = Convert.FromBase64String(this.PrimeQ);
                r.DP = Convert.FromBase64String(this.PrimeExponentP);
                r.DQ = Convert.FromBase64String(this.PrimeExponentQ);
                r.InverseQ = Convert.FromBase64String(this.Coefficient);
                r.D = Convert.FromBase64String(this.PrivateExponent);
                return r;
            }

            /// <summary>
            /// Returns *.config file XML section representing this private key
            /// </summary>
            public string ToConfigSection()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Utils.WriteConfigKey(_KeyModulus, this.Modulus));
                sb.Append(Utils.WriteConfigKey(_KeyExponent, this.Exponent));
                sb.Append(Utils.WriteConfigKey(_KeyPrimeP, this.PrimeP));
                sb.Append(Utils.WriteConfigKey(_KeyPrimeQ, this.PrimeQ));
                sb.Append(Utils.WriteConfigKey(_KeyPrimeExponentP, this.PrimeExponentP));
                sb.Append(Utils.WriteConfigKey(_KeyPrimeExponentQ, this.PrimeExponentQ));
                sb.Append(Utils.WriteConfigKey(_KeyCoefficient, this.Coefficient));
                sb.Append(Utils.WriteConfigKey(_KeyPrivateExponent, this.PrivateExponent));
                return sb.ToString();
            }

            /// <summary>
            /// Writes the *.config file representation of this private key to a file
            /// </summary>
            public void ExportToConfigFile(string strFilePath)
            {
                StreamWriter sw = new StreamWriter(strFilePath, false);
                sw.Write(this.ToConfigSection());
                sw.Close();
            }

            /// <summary>
            /// Loads the private key from its XML string
            /// </summary>
            public void LoadFromXml(string keyXml)
            {
                this.Modulus = Utils.GetXmlElement(keyXml, "Modulus");
                this.Exponent = Utils.GetXmlElement(keyXml, "Exponent");
                this.PrimeP = Utils.GetXmlElement(keyXml, "P");
                this.PrimeQ = Utils.GetXmlElement(keyXml, "Q");
                this.PrimeExponentP = Utils.GetXmlElement(keyXml, "DP");
                this.PrimeExponentQ = Utils.GetXmlElement(keyXml, "DQ");
                this.Coefficient = Utils.GetXmlElement(keyXml, "InverseQ");
                this.PrivateExponent = Utils.GetXmlElement(keyXml, "D");
            }

            /// <summary>
            /// Converts this private key to its XML string representation
            /// </summary>
            public string ToXml()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Utils.WriteXmlNode(_ElementParent));
                sb.Append(Utils.WriteXmlElement(_ElementModulus, this.Modulus));
                sb.Append(Utils.WriteXmlElement(_ElementExponent, this.Exponent));
                sb.Append(Utils.WriteXmlElement(_ElementPrimeP, this.PrimeP));
                sb.Append(Utils.WriteXmlElement(_ElementPrimeQ, this.PrimeQ));
                sb.Append(Utils.WriteXmlElement(_ElementPrimeExponentP, this.PrimeExponentP));
                sb.Append(Utils.WriteXmlElement(_ElementPrimeExponentQ, this.PrimeExponentQ));
                sb.Append(Utils.WriteXmlElement(_ElementCoefficient, this.Coefficient));
                sb.Append(Utils.WriteXmlElement(_ElementPrivateExponent, this.PrivateExponent));
                sb.Append(Utils.WriteXmlNode(_ElementParent, true));
                return sb.ToString();
            }

            /// <summary>
            /// Writes the Xml representation of this private key to a file
            /// </summary>
            public void ExportToXmlFile(string filePath)
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.Write(this.ToXml());
                sw.Close();
            }

        }

        #endregion

        /// <summary>
        /// Instantiates a new asymmetric encryption session using the default key size; 
        /// this is usally 1024 bits
        /// </summary>
        public Asymmetric()
        {
            _rsa = GetRSAProvider();
        }

        /// <summary>
        /// Instantiates a new asymmetric encryption session using a specific key size
        /// </summary>
        public Asymmetric(int keySize)
        {
            _KeySize = keySize;
            _rsa = GetRSAProvider();
        }

        /// <summary>
        /// Sets the name of the key container used to store this key on disk; this is an 
        /// unavoidable side effect of the underlying Microsoft CryptoAPI. 
        /// </summary>
        /// <remarks>
        /// http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/q322/3/71.asp&amp;NoWebContent=1
        /// </remarks>
        public string KeyContainerName
        {
            get
            {
                return _KeyContainerName;
            }
            set
            {
                _KeyContainerName = value;
            }
        }

        /// <summary>
        /// Returns the current key size, in bits
        /// </summary>
        public int KeySizeBits
        {
            get
            {
                return _rsa.KeySize;
            }
        }

        /// <summary>
        /// Returns the maximum supported key size, in bits
        /// </summary>
        public int KeySizeMaxBits
        {
            get
            {
                return _rsa.LegalKeySizes[0].MaxSize;
            }
        }

        /// <summary>
        /// Returns the minimum supported key size, in bits
        /// </summary>
        public int KeySizeMinBits
        {
            get
            {
                return _rsa.LegalKeySizes[0].MinSize;
            }
        }

        /// <summary>
        /// Returns valid key step sizes, in bits
        /// </summary>
        public int KeySizeStepBits
        {
            get
            {
                return _rsa.LegalKeySizes[0].SkipSize;
            }
        }

        /// <summary>
        /// Returns the default public key as stored in the *.config file
        /// </summary>
        public PublicKey DefaultPublicKey
        {
            get
            {
                PublicKey pubkey = new PublicKey();
                pubkey.LoadFromConfig();
                return pubkey;
            }
        }

        /// <summary>
        /// Returns the default private key as stored in the *.config file
        /// </summary>
        public PrivateKey DefaultPrivateKey
        {
            get
            {
                PrivateKey privkey = new PrivateKey();
                privkey.LoadFromConfig();
                return privkey;
            }
        }

        /// <summary>
        /// Generates a new public/private key pair as objects
        /// </summary>
        public void GenerateNewKeyset(ref PublicKey publicKey, ref PrivateKey privateKey)
        {
            string PublicKeyXML = null;
            string PrivateKeyXML = null;
            GenerateNewKeyset(ref PublicKeyXML, ref PrivateKeyXML);
            publicKey = new PublicKey(PublicKeyXML);
            privateKey = new PrivateKey(PrivateKeyXML);
        }

        /// <summary>
        /// Generates a new public/private key pair as XML strings
        /// </summary>
        public void GenerateNewKeyset(ref string publicKeyXML, ref string privateKeyXML)
        {
            RSA rsa = RSA.Create();
            publicKeyXML = rsa.ToXmlString(false);
            privateKeyXML = rsa.ToXmlString(true);
        }

        /// <summary>
        /// Encrypts data using the default public key
        /// </summary>
        public Data Encrypt(Data d)
        {
            PublicKey PublicKey = DefaultPublicKey;
            return Encrypt(d, PublicKey);
        }

        /// <summary>
        /// Encrypts data using the provided public key
        /// </summary>
        public Data Encrypt(Data d, PublicKey publicKey)
        {
            _rsa.ImportParameters(publicKey.ToParameters());
            return EncryptPrivate(d);
        }

        /// <summary>
        /// Encrypts data using the provided public key as XML
        /// </summary>
        public Data Encrypt(Data d, string publicKeyXML)
        {
            LoadKeyXml(publicKeyXML, false);
            return EncryptPrivate(d);
        }

        private Data EncryptPrivate(Data d)
        {
            try
            {
                return new Data(_rsa.Encrypt(d.Bytes, false));
            }
            catch (CryptographicException ex)
            {
                if (ex.Message.ToLower().IndexOf("bad length") > -1)
                {
                    throw new CryptographicException("Your data is too large; RSA encryption is designed to encrypt relatively small amounts of data. The exact byte limit depends on the key size. To encrypt more data, use symmetric encryption and then encrypt that symmetric key with asymmetric RSA encryption.", ex);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Decrypts data using the default private key
        /// </summary>
        public Data Decrypt(Data encryptedData)
        {
            PrivateKey PrivateKey = new PrivateKey();
            PrivateKey.LoadFromConfig();
            return Decrypt(encryptedData, PrivateKey);
        }

        /// <summary>
        /// Decrypts data using the provided private key
        /// </summary>
        public Data Decrypt(Data encryptedData, PrivateKey PrivateKey)
        {
            _rsa.ImportParameters(PrivateKey.ToParameters());
            return DecryptPrivate(encryptedData);
        }

        /// <summary>
        /// Decrypts data using the provided private key as XML
        /// </summary>
        public Data Decrypt(Data encryptedData, string PrivateKeyXML)
        {
            LoadKeyXml(PrivateKeyXML, true);
            return DecryptPrivate(encryptedData);
        }

        private void LoadKeyXml(string keyXml, bool isPrivate)
        {
            try
            {
                _rsa.FromXmlString(keyXml);
            }
            catch (System.Security.XmlSyntaxException ex)
            {
                string s = null;
                if (isPrivate)
                {
                    s = "private";
                }
                else
                {
                    s = "public";
                }
                throw new System.Security.XmlSyntaxException(string.Format("The provided {0} encryption key XML does not appear to be valid.", s), ex);
            }
        }

        private Data DecryptPrivate(Data encryptedData)
        {
            return new Data(_rsa.Decrypt(encryptedData.Bytes, false));
        }

        /// <summary>
        /// gets the default RSA provider using the specified key size; 
        /// note that Microsoft's CryptoAPI has an underlying file system dependency that is unavoidable
        /// </summary>
        /// <remarks>
        /// http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/q322/3/71.asp&amp;NoWebContent=1
        /// </remarks>
        private RSACryptoServiceProvider GetRSAProvider()
        {
            RSACryptoServiceProvider rsa = null;
            CspParameters csp = null;
            try
            {
                csp = new CspParameters();
                csp.KeyContainerName = _KeyContainerName;
                rsa = new RSACryptoServiceProvider(_KeySize, csp);
                rsa.PersistKeyInCsp = false;
                System.Security.Cryptography.RSACryptoServiceProvider.UseMachineKeyStore = true;
                return rsa;
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                if (ex.Message.ToLower().IndexOf("csp for this implementation could not be acquired") > -1)
                {
                    throw new Exception("Unable to obtain Cryptographic Service Provider. " + "Either the permissions are incorrect on the " + "'C:\\Documents and Settings\\All Users\\Application Data\\Microsoft\\Crypto\\RSA\\MachineKeys' " + "folder, or the current security context '" + System.Security.Principal.WindowsIdentity.GetCurrent().Name + "'" + " does not have access to this folder.", ex);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (rsa != null)
                {
                    rsa = null;
                }
                if (csp != null)
                {
                    csp = null;
                }
            }
            //INSTANT C# NOTE: Inserted the following 'return' since all code paths must return a value in C#:
            //return null;
        }

    }

    #endregion

    #region   Data

    /// <summary>
    /// represents Hex, Byte, Base64, or String data to encrypt/decrypt;
    /// use the .Text property to set/get a string representation 
    /// use the .Hex property to set/get a string-based Hexadecimal representation 
    /// use the .Base64 to set/get a string-based Base64 representation 
    /// </summary>
    public class Data
    {
        private byte[] _b;
        private int _MaxBytes = 0;
        private int _MinBytes = 0;
        private int _StepBytes = 0;

        /// <summary>
        /// Determines the default text encoding across ALL Data instances
        /// </summary>
        public static System.Text.Encoding DefaultEncoding = System.Text.Encoding.GetEncoding("Windows-1252");


        /// <summary>
        /// Determines the default text encoding for this Data instance
        /// </summary>
        public System.Text.Encoding Encoding = DefaultEncoding;

        /// <summary>
        /// Creates new, empty encryption data
        /// </summary>
        public Data()
        {
        }

        /// <summary>
        /// Creates new encryption data with the specified byte array
        /// </summary>
        public Data(byte[] b)
        {
            _b = b;
        }

        /// <summary>
        /// Creates new encryption data with the specified string; 
        /// will be converted to byte array using default encoding
        /// </summary>
        public Data(string s)
        {
            this.Text = s;
        }

        /// <summary>
        /// Creates new encryption data using the specified string and the 
        /// specified encoding to convert the string to a byte array.
        /// </summary>
        public Data(string s, System.Text.Encoding encoding)
        {
            this.Encoding = encoding;
            this.Text = s;
        }

        /// <summary>
        /// returns true if no data is present
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (_b == null)
                {
                    return true;
                }
                if (_b.Length == 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// allowed step interval, in bytes, for this data; if 0, no limit
        /// </summary>
        public int StepBytes
        {
            get
            {
                return _StepBytes;
            }
            set
            {
                _StepBytes = value;
            }
        }

        /// <summary>
        /// allowed step interval, in bits, for this data; if 0, no limit
        /// </summary>
        public int StepBits
        {
            get
            {
                return _StepBytes * 8;
            }
            set
            {
                _StepBytes = value / 8;
            }
        }

        /// <summary>
        /// minimum number of bytes allowed for this data; if 0, no limit
        /// </summary>
        public int MinBytes
        {
            get
            {
                return _MinBytes;
            }
            set
            {
                _MinBytes = value;
            }
        }

        /// <summary>
        /// minimum number of bits allowed for this data; if 0, no limit
        /// </summary>
        public int MinBits
        {
            get
            {
                return _MinBytes * 8;
            }
            set
            {
                _MinBytes = value / 8;
            }
        }

        /// <summary>
        /// maximum number of bytes allowed for this data; if 0, no limit
        /// </summary>
        public int MaxBytes
        {
            get
            {
                return _MaxBytes;
            }
            set
            {
                _MaxBytes = value;
            }
        }

        /// <summary>
        /// maximum number of bits allowed for this data; if 0, no limit
        /// </summary>
        public int MaxBits
        {
            get
            {
                return _MaxBytes * 8;
            }
            set
            {
                _MaxBytes = value / 8;
            }
        }

        /// <summary>
        /// Returns the byte representation of the data; 
        /// This will be padded to MinBytes and trimmed to MaxBytes as necessary!
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                if (_MaxBytes > 0)
                {
                    if (_b.Length > _MaxBytes)
                    {
                        byte[] b = new byte[_MaxBytes];
                        System.Array.Copy(_b, b, b.Length);
                        _b = b;
                    }
                }
                if (_MinBytes > 0)
                {
                    if (_b.Length < _MinBytes)
                    {
                        byte[] b = new byte[_MinBytes];
                        System.Array.Copy(_b, b, _b.Length);
                        _b = b;
                    }
                }
                return _b;
            }
            set
            {
                _b = value;
            }
        }

        /// <summary>
        /// Sets or returns text representation of bytes using the default text encoding
        /// </summary>
        public string Text
        {
            get
            {
                if (_b == null)
                {
                    return "";
                }
                else
                {
                    //-- need to handle nulls here; oddly, C# will happily convert
                    //-- nulls into the string whereas VB stops converting at the
                    //-- first null!
                    int i = Array.IndexOf(_b, System.Convert.ToByte(0));
                    if (i >= 0)
                    {
                        return this.Encoding.GetString(_b, 0, i);
                    }
                    else
                    {
                        return this.Encoding.GetString(_b);
                    }
                }
            }
            set
            {
                _b = this.Encoding.GetBytes(value);
            }
        }

        /// <summary>
        /// Sets or returns Hex string representation of this data
        /// </summary>
        public string Hex
        {
            get
            {
                return Utils.ToHex(_b);
            }
            set
            {
                _b = Utils.FromHex(value);
            }
        }

        /// <summary>
        /// Sets or returns Base64 string representation of this data
        /// </summary>
        public string Base64
        {
            get
            {
                return Utils.ToBase64(_b);
            }
            set
            {
                _b = Utils.FromBase64(value);
            }
        }

        /// <summary>
        /// Returns text representation of bytes using the default text encoding
        /// </summary>
        public new string ToString()
        {
            return this.Text;
        }

        /// <summary>
        /// returns Base64 string representation of this data
        /// </summary>
        public string ToBase64()
        {
            return this.Base64;
        }

        /// <summary>
        /// returns Hex string representation of this data
        /// </summary>
        public string ToHex()
        {
            return this.Hex;
        }

    }

    #endregion

    #region   Utils

    /// <summary>
    /// Friend class for shared utility methods used by multiple Encryption classes
    /// </summary>
    internal class Utils
    {

        /// <summary>
        /// converts an array of bytes to a string Hex representation
        /// </summary>
        internal static string ToHex(byte[] ba)
        {
            if (ba == null || ba.Length == 0)
            {
                return "";
            }
            const string HexFormat = "{0:X2}";
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ba)
            {
                sb.Append(string.Format(HexFormat, b));
            }
            return sb.ToString();
        }

        /// <summary>
        /// converts from a string Hex representation to an array of bytes
        /// </summary>
        internal static byte[] FromHex(string hexEncoded)
        {
            if (hexEncoded == null || hexEncoded.Length == 0)
            {
                return null;
            }
            try
            {
                int l = Convert.ToInt32(hexEncoded.Length / 2.0);
                byte[] b = new byte[l];
                for (int i = 0; i < l; i++)
                {
                    b[i] = Convert.ToByte(hexEncoded.Substring(i * 2, 2), 16);
                }
                return b;
            }
            catch (Exception ex)
            {
                throw new System.FormatException("The provided string does not appear to be Hex encoded:" + Environment.NewLine + hexEncoded + Environment.NewLine, ex);
            }
        }

        /// <summary>
        /// converts from a string Base64 representation to an array of bytes
        /// </summary>
        internal static byte[] FromBase64(string base64Encoded)
        {
            if (base64Encoded == null || base64Encoded.Length == 0)
            {
                return null;
            }
            try
            {
                return Convert.FromBase64String(base64Encoded);
            }
            catch (System.FormatException ex)
            {
                throw new System.FormatException("The provided string does not appear to be Base64 encoded:" + Environment.NewLine + base64Encoded + Environment.NewLine, ex);
            }
        }

        /// <summary>
        /// converts from an array of bytes to a string Base64 representation
        /// </summary>
        internal static string ToBase64(byte[] b)
        {
            if (b == null || b.Length == 0)
            {
                return "";
            }
            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// retrieve an element from an XML string
        /// </summary>
        internal static string GetXmlElement(string xml, string element)
        {
            Match m = null;
            m = Regex.Match(xml, "<" + element + ">(?<Element>[^>]*)</" + element + ">", RegexOptions.IgnoreCase);
            if (m == null)
            {
                throw new Exception("Could not find <" + element + "></" + element + "> in provided Public Key XML.");
            }
            return m.Groups["Element"].ToString();
        }

        /// <summary>
        /// Returns the specified string value from the application .config file
        /// </summary>

        internal static string GetConfigString(string key)
        {
            return GetConfigString(key, true);
        }

        //INSTANT C# NOTE: C# does not support optional parameters. Overloaded method(s) are created above.
        //ORIGINAL LINE: Friend Shared Function GetConfigString(ByVal key As String, Optional ByVal isRequired As Boolean = true) As String
        internal static string GetConfigString(string key, bool isRequired)
        {

            string s = System.Convert.ToString(ConfigurationManager.AppSettings.Get(key));
            if (s == "")
            {
                if (isRequired)
                {
                    throw new ConfigurationErrorsException("key <" + key + "> is missing from .config file");
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return s;
            }
        }

        internal static string WriteConfigKey(string key, string value)
        {
            string s = "<add key=\"{0}\" value=\"{1}\" />" + Environment.NewLine;
            return string.Format(s, key, value);
        }

        internal static string WriteXmlElement(string element, string value)
        {
            string s = "<{0}>{1}</{0}>" + Environment.NewLine;
            return string.Format(s, element, value);
        }


        internal static string WriteXmlNode(string element)
        {
            return WriteXmlNode(element, false);
        }

        //INSTANT C# NOTE: C# does not support optional parameters. Overloaded method(s) are created above.
        //ORIGINAL LINE: Friend Shared Function WriteXmlNode(ByVal element As String, Optional ByVal isClosing As Boolean = false) As String
        internal static string WriteXmlNode(string element, bool isClosing)
        {
            string s = null;
            if (isClosing)
            {
                s = "</{0}>" + Environment.NewLine;
            }
            else
            {
                s = "<{0}>" + Environment.NewLine;
            }
            return string.Format(s, element);
        }

    }

    #endregion
}
