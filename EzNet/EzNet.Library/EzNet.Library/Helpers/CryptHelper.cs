using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EzNet.Library.Helpers
{
    public class EzNetEnCryptFileStream : IDisposable
    {
        private CryptoStream m_outCryptStream;
        private CryptoStream m_hashStream;
        private bool m_needCrypt = false;
        private long m_fileSize = 0;
        private FileStream m_fileStream;

        public string Path { get; set; }
        public byte[] Password { get; set; }
        public bool IsCrypt { get; set; }

        public EzNetEnCryptFileStream(string path, byte[] password, bool needCrypt)
        {
            Path = path;
            Password = password;
            IsCrypt = needCrypt;

            m_fileStream = new FileStream(path, FileMode.Create);
            m_needCrypt = needCrypt;
            if (m_needCrypt)
            {
                // 获取IV和salt
                byte[] IV = CryptHelper.GenerateRandomBytes(16);
                byte[] salt = CryptHelper.GenerateRandomBytes(16);

                // 创建加密对象
                SymmetricAlgorithm sma = CryptHelper.CreateRijndael(password, salt);
                sma.IV = IV;

                // 在输出文件开始部分写入IV和salt
                m_fileStream.Write(IV, 0, IV.Length);
                m_fileStream.Write(salt, 0, salt.Length);

                // 创建散列加密

                m_outCryptStream = new CryptoStream(m_fileStream, sma.CreateEncryptor(), CryptoStreamMode.Write);
                m_hashStream = new CryptoStream(Stream.Null, m_hasher, CryptoStreamMode.Write);

                BinaryWriter bw = new BinaryWriter(m_outCryptStream);
                bw.Write(FC_TAG);
            }

        }

        public void Write(byte[] bytes, int offset, int count)
        {
            if (m_needCrypt)
            {
                m_outCryptStream.Write(bytes, offset, count);
                m_fileSize += count;
                m_hashStream.Write(bytes, offset, count);
            }
            else
            {
                m_fileStream.Write(bytes, offset, count);
            }
        }

        public void Flush()
        {
            if (m_needCrypt)
            {
                m_outCryptStream.Flush();
                m_hashStream.Flush();
            }

            m_fileStream.Flush();
        }

        public void Close()
        {
            if (m_needCrypt)
            {
                // 关闭加密流
                m_hashStream.Flush();
                m_hashStream.Close();

                // 读取散列
                byte[] hash = m_hasher.Hash;

                // 输入文件写入散列
                m_outCryptStream.Write(hash, 0, hash.Length);

                // 关闭文件流
                m_outCryptStream.FlushFinalBlock();

                m_outCryptStream.Close();
            }

        }

        /// <summary>
        /// 加密文件随机数生成
        /// </summary>
        private static RandomNumberGenerator rand = new RNGCryptoServiceProvider();

        private const ulong FC_TAG = 0xFC010203040506CF;
        private HashAlgorithm m_hasher = SHA256.Create();

        public void Dispose()
        {
            if (m_needCrypt)
            {
                m_outCryptStream.Dispose();
                m_hashStream.Dispose();
            }
            m_fileStream.Dispose();
        }
    }

    public class EzNetDeCryptFileStream : Stream
    {
        private const ulong FC_TAG = 0xFC010203040506CF;
        private Stream m_stream;
        private long m_fileSize;
        private long m_readLength = 0;

        public EzNetDeCryptFileStream(string path, byte[] password, long fileSize = 0, bool isCrypt = true)
        {
            if (fileSize <= 0)
            {
                // 创建打开文件流
                using (FileStream fin = File.OpenRead(path))
                {
                    byte[] bytes = new byte[8192];
                    int outValue = 0;

                    byte[] IV = new byte[16];
                    fin.Read(IV, 0, 16);
                    byte[] salt = new byte[16];
                    fin.Read(salt, 0, 16);


                    SymmetricAlgorithm sma = CryptHelper.CreateRijndael(password, salt);
                    sma.IV = IV;

                    //value = 32;


                    //探测文件长度
                    //sha256 HASH固定长度256 bits
                    HashAlgorithm sha256Hasher = SHA256.Create();
                    try
                    {
                        using (CryptoStream cin = new CryptoStream(fin, sma.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            BinaryReader br2 = new BinaryReader(cin);
                            //lSize = br.ReadInt64();
                            ulong tag = br2.ReadUInt64();

                            if (FC_TAG != tag)
                                throw new ApplicationException("密钥不对或文件被破坏");

                            while (cin.ReadByte() >= 0)
                            {
                                ++outValue;
                            }
                        }
                    }
                    catch (CryptographicException ex)
                    {
                    }

                    fileSize = outValue - 256 / 8;
                }
            }

            m_fileSize = fileSize;
            FileStream fileStream = File.OpenRead(path);
            if (isCrypt)
            {
                byte[] IV = new byte[16];
                fileStream.Read(IV, 0, 16);
                byte[] salt = new byte[16];
                fileStream.Read(salt, 0, 16);
                SymmetricAlgorithm sma = CryptHelper.CreateRijndael(password, salt);
                sma.IV = IV;

                CryptoStream cin = new CryptoStream(fileStream, sma.CreateDecryptor(), CryptoStreamMode.Read);
                BinaryReader br2 = new BinaryReader(cin);
                //lSize = br.ReadInt64();
                ulong tag = br2.ReadUInt64();

                if (FC_TAG != tag)
                    throw new ApplicationException("密钥不对或文件被破坏");

                m_stream = cin;
            }
            else
            {
                m_stream = fileStream;
            }

        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (m_readLength < m_fileSize)
            {
                count = m_readLength + (long)count < m_fileSize ? count : (int)(m_fileSize - m_readLength);
                int returnedLength = m_stream.Read(array, offset, count);
                m_readLength += returnedLength;
                return returnedLength;
            }
            else
            {
                return 0;
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            return;
        }

        public override long Length
        {
            get { return m_fileSize; }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    public class CryptHelper
    {
        private const ulong FC_TAG = 0xFC010203040506CF;

        private const int BUFFER_SIZE = 128 * 1024;

        /// <summary>
        /// 检验两个Byte数组是否相同
        /// </summary>
        /// <param name="b1">Byte数组</param>
        /// <param name="b2">Byte数组</param>
        /// <returns>true－相等</returns>
        private static bool CheckByteArrays(byte[] b1, byte[] b2)
        {
            if (b1.Length == b2.Length)
            {
                for (int i = 0; i < b1.Length; ++i)
                {
                    if (b1[i] != b2[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 创建Rijndael SymmetricAlgorithm
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="salt"></param>
        /// <returns>加密对象</returns>
        public static SymmetricAlgorithm CreateRijndael(byte[] password, byte[] salt)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt, "SHA256", 1000);

            SymmetricAlgorithm sma = Rijndael.Create();
            sma.KeySize = 256;
            sma.Key = pdb.GetBytes(32);
            sma.Padding = PaddingMode.PKCS7;
            return sma;
        }

        /// <summary>
        /// 加密文件随机数生成
        /// </summary>
        private static RandomNumberGenerator rand = new RNGCryptoServiceProvider();

        /// <summary>
        /// 生成指定长度的随机Byte数组
        /// </summary>
        /// <param name="count">Byte数组长度</param>
        /// <returns>随机Byte数组</returns>
        public static byte[] GenerateRandomBytes(int count)
        {
            byte[] bytes = new byte[count];
            rand.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="inFile">待解密文件</param>
        /// <param name="outFile">解密后输出文件</param>
        /// <param name="password">解密密码</param>
        public static void DecryptFile(string inFile, string outFile, byte[] password)
        {
            // 创建打开文件流
            using (FileStream fin = File.OpenRead(inFile), fin2 = File.OpenRead(inFile),
                fout = File.OpenWrite(outFile))
            {
                int size = (int)fin.Length;
                byte[] bytes = new byte[BUFFER_SIZE];
                int outValue = 0;
                int read = -1;

                byte[] IV = new byte[16];
                fin.Read(IV, 0, 16);
                byte[] salt = new byte[16];
                fin.Read(salt, 0, 16);


                SymmetricAlgorithm sma = CreateRijndael(password, salt);
                sma.IV = IV;

                //value = 32;


                //探测文件长度
                //sha256 HASH固定长度256 bits
                HashAlgorithm sha256Hasher = SHA256.Create();
                try
                {
                    using (CryptoStream cin = new CryptoStream(fin, sma.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        BinaryReader br2 = new BinaryReader(cin);
                        //lSize = br.ReadInt64();
                        ulong tag = br2.ReadUInt64();

                        if (FC_TAG != tag)
                            throw new ApplicationException("密钥不对或文件被破坏");

                        while (cin.ReadByte() >= 0)
                        {
                            ++outValue;
                        }
                    }
                }
                catch (CryptographicException ex)
                {
                }

                long fileSize = outValue - 256 / 8;

                //解密文件
                HashAlgorithm sha256Hasher2 = SHA256.Create();
                using (CryptoStream cin = new CryptoStream(fin2, sma.CreateDecryptor(), CryptoStreamMode.Read),
                    chash = new CryptoStream(Stream.Null, sha256Hasher2, CryptoStreamMode.Write))
                {

                    fin2.Read(IV, 0, 16);
                    fin2.Read(salt, 0, 16);

                    BinaryReader br2 = new BinaryReader(cin);
                    //lSize = br.ReadInt64();
                    br2.ReadUInt64();


                    long numReads = fileSize / BUFFER_SIZE;
                    long slack = (long)fileSize % BUFFER_SIZE;

                    for (int i = 0; i < numReads; ++i)
                    {
                        read = cin.Read(bytes, 0, bytes.Length);
                        fout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                    }

                    if (slack > 0)
                    {
                        read = cin.Read(bytes, 0, (int)slack);
                        fout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                    }

                    chash.Flush();
                    chash.Close();

                    fout.Flush();
                    fout.Close();

                    byte[] curHash = sha256Hasher2.Hash;


                    // 通过获取比较和旧的散列对象检测解密是否成功
                    byte[] oldHash = new byte[sha256Hasher2.HashSize / 8];
                    read = cin.Read(oldHash, 0, oldHash.Length);
                    if ((oldHash.Length != read) || (!CheckByteArrays(oldHash, curHash)))
                        throw new ApplicationException("密钥不对或文件被破坏");

                    /*if (outValue != lSize)
                    {
                        throw new ApplicationException("文件大小不匹配");
                    }*/
                }
            }
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="inFile">待加密文件</param>
        /// <param name="outFile">加密后输入文件</param>
        /// <param name="password">加密密码</param>
        public static void EncryptFile(string inFile, string outFile, byte[] password)
        {
            using (FileStream fin = File.OpenRead(inFile),
                fout = File.OpenWrite(outFile))
            {
                long lSize = fin.Length; // 输入文件长度
                //int size = (int)lSize;
                byte[] bytes = new byte[BUFFER_SIZE]; // 缓存
                int read = -1; // 输入文件读取数量
                int value = 0;

                // 获取IV和salt
                byte[] IV = GenerateRandomBytes(16);
                byte[] salt = GenerateRandomBytes(16);

                // 创建加密对象
                SymmetricAlgorithm sma = CreateRijndael(password, salt);
                sma.IV = IV;

                // 在输出文件开始部分写入IV和salt
                fout.Write(IV, 0, IV.Length);
                fout.Write(salt, 0, salt.Length);


                // 创建散列加密
                HashAlgorithm hasher = SHA256.Create();
                using (CryptoStream cout = new CryptoStream(fout, sma.CreateEncryptor(), CryptoStreamMode.Write),
                    chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
                {
                    BinaryWriter bw = new BinaryWriter(cout);
                    //bw.Write(lSize);
                    bw.Write(FC_TAG);

                    // 读写字节块到加密流缓冲区
                    while ((read = fin.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        cout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                    }


                    // 关闭hash流
                    chash.Flush();
                    chash.Close();

                    // 读取散列
                    byte[] hash = hasher.Hash;

                    // 输入文件写入散列
                    cout.Write(hash, 0, hash.Length);

                    // 关闭文件流
                    cout.FlushFinalBlock();

                    cout.Close();
                }
            }
        }

    }
}
