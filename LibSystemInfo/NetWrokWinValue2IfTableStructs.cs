using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LibSystemInfo
{

    public class NetWorkAdapter
    {
        private long _send;
        private long _recv;
        private string _mac;
        private string _ipAddress;

        public long Send
        {
            get => _send;
            set => _send = value;
        }

        public long Recv
        {
            get => _recv;
            set => _recv = value;
        }

        public string Mac
        {
            get => _mac;
            set => _mac = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
     /// <summary>
    /// 网络类型
    /// </summary>
    public enum NetType
    {
        Other = 1,
        Ethernet = 6,
        Tokenring = 9,
        FDDI = 15,
        PPP = 23,
        Loopback = 24,
        Slip = 28
    };

    /// <summary>
    /// 网络状态
    /// </summary>
    public enum NetState
    {
        NotOperational = 0,
        Operational = 1,
        Disconnected = 2,
        Connecting = 3,
        Connected = 4,
        Unreachable = 5
    };

    /// <summary>
    /// 网络信息类
    /// </summary>
    public class NetInfo
    {
        public NetInfo()
        {
        }

        private string m_Name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private uint m_Index;
        /// <summary>
        /// 有效编号
        /// </summary>
        public uint Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        private NetType m_Type;
        /// <summary>
        /// 类型
        /// </summary>
        public NetType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        private uint m_Speed;
        /// <summary>
        /// 速度
        /// </summary>
        public uint Speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }

        private uint m_InOctets;
        /// <summary>
        /// 总接收字节数
        /// </summary>
        public uint InOctets
        {
            get { return m_InOctets; }
            set { m_InOctets = value; }
        }

        private uint m_OutOctets;
        /// <summary>
        /// 总发送字节数
        /// </summary>
        public uint OutOctets
        {
            get { return m_OutOctets; }
            set { m_OutOctets = value; }
        }

        private NetState m_Status;
        /// <summary>
        /// 操作状态
        /// </summary>
        public NetState Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }

        private uint m_InErrors;
        /// <summary>
        /// 总错收字节数
        /// </summary>
        public uint InErrors
        {
            get { return m_InErrors; }
            set { m_InErrors = value; }
        }

        private uint m_OutErrors;
        /// <summary>
        /// 总错发字节数
        /// </summary>
        public uint OutErrors
        {
            get { return m_OutErrors; }
            set { m_OutErrors = value; }
        }

        private uint m_InUnknownProtos;
        /// <summary>
        /// 未知协议共收字节数
        /// </summary>
        public uint InUnknownProtos
        {
            get { return m_InUnknownProtos; }
            set { m_InUnknownProtos = value; }
        }

        private string m_PhysAddr;
        /// <summary>
        /// 物理地址
        /// </summary>
        public string PhysAddr
        {
            get { return m_PhysAddr; }
            set { m_PhysAddr = value; }
        }

    }
    /// <summary>
    /// IFTable
    /// </summary>
    public class MIB_IFTABLE : CustomMarshaler
    {
        public int dwNumEntries;
        
        [CustomMarshalAs(SizeField = "dwNumEntries")]
        public MIB_IFROW[] Table;

        public MIB_IFTABLE()
        {
            this.data = new byte[this.GetSize()];
        }

        public MIB_IFTABLE(int size)
        {
            this.data = new byte[size];
        }
    }
    public class MIB_IFROW : CustomMarshaler
    {
        [CustomMarshalAs(SizeConst = MAX_INTERFACE_NAME_LEN)]
        public string wszName;
        public uint dwIndex; // index of the interface
        public uint dwType; // type of interface
        public uint dwMtu; // max transmission unit 
        public uint dwSpeed; // speed of the interface 
        public uint dwPhysAddrLen; // length of physical address
        [CustomMarshalAs(SizeConst = MAXLEN_PHYSADDR)]
        public byte[] bPhysAddr; // physical address of adapter
        public uint dwAdminStatus; // administrative status
        public uint dwOperStatus; // operational status
        public uint dwLastChange; // last time operational status changed 
        public uint dwInOctets; // octets received
        public uint dwInUcastPkts; // unicast packets received 
        public uint dwInNUcastPkts; // non-unicast packets received 
        public uint dwInDiscards; // received packets discarded 
        public uint dwInErrors; // erroneous packets received 
        public uint dwInUnknownProtos; // unknown protocol packets received 
        public uint dwOutOctets; // octets sent 
        public uint dwOutUcastPkts; // unicast packets sent 
        public uint dwOutNUcastPkts; // non-unicast packets sent 
        public uint dwOutDiscards; // outgoing packets discarded 
        public uint dwOutErrors; // erroneous packets sent 
        public uint dwOutQLen; // output queue length 
        public uint dwDescrLen; // length of bDescr member 
        [CustomMarshalAs(SizeConst = MAXLEN_IFDESCR)]
        public byte[] bDescr; // interface description         

        private const int MAX_INTERFACE_NAME_LEN = 256;
        private const int MAXLEN_PHYSADDR = 8;
        private const int MAXLEN_IFDESCR = 256;
        private const int MAX_ADAPTER_NAME = 128;
    }
    
    /// <summary>
    /// CustomMarshaler class implementation.
    /// </summary>
    public abstract class CustomMarshaler
    {
        #region Fields
        // The internal buffer
        internal byte[] data;
        private MemoryStream stream;
        private BinaryReader binReader;
        private BinaryWriter binWriter;
        
        #endregion
    
        #region constructors

        public CustomMarshaler()
        {

        }
        
        #endregion

        #region public methods

        public void Deserialize()
        {
            if (data != null)
            {
                if (binReader != null)
                {
                    binReader.Close();
                    stream.Close();
                }
                // Create a steam from byte array
                stream = new MemoryStream(data);
                binReader = new BinaryReader(stream, System.Text.Encoding.Unicode);
                ReadFromStream(binReader);
                binReader.Close();
            }

        }

        public void Serialize()
        {
            if (data != null)
            {
                stream = new MemoryStream(data);
                binWriter = new BinaryWriter(stream, System.Text.Encoding.Unicode);
                WriteToStream(binWriter);
                binWriter.Close();
            }
        }

        public int GetSize()
        {    
            int size = 0;

            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields )
            {
                if (field.FieldType.IsArray)
                {
                    size += GetFieldSize(field);
                }
                else if (field.FieldType == typeof(string))
                {
                    size += GetFieldSize(field)*2;
                } 
                else if (field.FieldType.IsPrimitive)
                {
                    size += Marshal.SizeOf(field.FieldType);
                }
            }

            return size;
        }

        #endregion

        #region properties

        public byte[] ByteArray
        {
            get
            {
                return data;
            }
        }

        #endregion

        #region virtual and protected methods

        public virtual void ReadFromStream(BinaryReader reader)
        {
            object[] param = null;

            // Get all public fields
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            // Loop through the fields
            foreach(FieldInfo field in fields)
            {
                // Retrieve the read method from ReadMethods hashtable
                MethodInfo method = (MethodInfo)MarshallingMethods.ReadMethods[field.FieldType];

                if (field.FieldType.IsArray)
                {
                    Type element = field.FieldType.GetElementType();
                    if (element.IsValueType && element.IsPrimitive)
                    {
                        if ((element == typeof(char)) || element == typeof(byte))
                        {                                                                                 
                            param = new object[1];
                            param[0] = GetFieldSize(field);
                            field.SetValue(this, method.Invoke(reader, param)); 
                        }
                        else // any other value type array
                        {
                            param = new object[2];
                            param[0] = reader;
                            param[1] = GetFieldSize(field);
                            field.SetValue(this, method.Invoke(null, param)); 
                        }
                    }
                    else // array of sub structures
                    {
                        int size = GetFieldSize(field);
                        method = (MethodInfo)MarshallingMethods.ReadMethods[typeof(CustomMarshaler)];
                        Array objArray = Array.CreateInstance(element, size);
                        for(int i=0;i<size;i++)
                        {
                            objArray.SetValue(Activator.CreateInstance(element), i);
                            method.Invoke(objArray.GetValue(i), new object[]{reader});
                        }
                        field.SetValue(this, objArray); 
                    }
                }
                else if (field.FieldType == typeof(string))
                {    
                    param = new object[2];
                    param[0] = reader;
                    param[1] = GetFieldSize(field);
                    field.SetValue(this, method.Invoke(null, param)); 
                }
                else if (field.FieldType.IsValueType && field.FieldType.IsPrimitive)// regular value type
                {
                    field.SetValue(this, method.Invoke(reader, null)); 
                }
                else //process substructure 
                {
                    CustomMarshaler subStruct = (CustomMarshaler)Activator.CreateInstance(field.FieldType);
                    subStruct.ReadFromStream(reader);
                }
            }
        }

        public virtual void WriteToStream(BinaryWriter writer)
        {
            object[] param = null;

            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach(FieldInfo field in fields)
            {
                // Check if we have any value
                object value = field.GetValue(this);
                
                MethodInfo method = (MethodInfo)MarshallingMethods.WriteMethods[field.FieldType];
                
                if (field.FieldType.IsArray)
                {
                    Type element = field.FieldType.GetElementType();
                    if (element.IsValueType && element.IsPrimitive)
                    {
                        //method.Invoke(writer, new object[] {value});
                        Array arrObject = (Array)field.GetValue(this);
                        param = new object[2];
                        param[0] = writer;
                        param[1] = arrObject;
                        method.Invoke(null, param);
                    }
                    else
                    {
                        //Get field size
                        int size = GetFieldSize(field);
                        //Get WriteToStream method
                        method = (MethodInfo)MarshallingMethods.WriteMethods[typeof(CustomMarshaler)];
                        Array arrObject = (Array)field.GetValue(this);
                        for(int i=0;i<size;i++)
                        {
                            method.Invoke(arrObject.GetValue(i), new object[]{writer});
                        }
                    }                    
                }
                else if (field.FieldType == typeof(string))
                {    
                    param = new object[3];
                    param[0] = writer;
                    param[1] = field.GetValue(this);
                    param[2] = GetFieldSize(field);
                    method.Invoke(null, param);
                    

                }
                else if (field.FieldType.IsValueType && field.FieldType.IsPrimitive)// regular value type
                {
                    method.Invoke(writer, new object[] {value});
                }
            }
        }

        protected int GetFieldSize(FieldInfo field)
        {
            int size = 0;
            CustomMarshalAsAttribute attrib = (CustomMarshalAsAttribute)field.GetCustomAttributes(typeof(CustomMarshalAsAttribute), true)[0];
            
            if (attrib != null)
            {
                if (attrib.SizeField != null)
                {
                    FieldInfo sizeField = this.GetType().GetField(attrib.SizeField);
                    size = (int)sizeField.GetValue(this);
                }
                else
                {
                    size = attrib.SizeConst;    
                }
            }

            return size;
        }

        #endregion

        #region helper methods

        private static bool CompareByteArrays (byte[] data1, byte[] data2)
        {
            // If both are null, they're equal
            if (data1==null && data2==null)
            {
                return true;
            }
            // If either but not both are null, they're not equal
            if (data1==null || data2==null)
            {
                return false;
            }
            if (data1.Length != data2.Length)
            {
                return false;
            }
            for (int i=0; i < data1.Length; i++)
            {
                if (data1[i] != data2[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

    }

    #region MarshallingMethods class
    /// <summary>
    /// MarshallingMethods class implementation.
    /// </summary>
    public class MarshallingMethods
    {
        public static Hashtable ReadMethods = new Hashtable();
        public static Hashtable WriteMethods = new Hashtable();
        
        #region constructors

        static MarshallingMethods()
        {
            // Read Methods
            ReadMethods.Add(typeof(bool), typeof(BinaryReader).GetMethod("ReadBoolean"));
            ReadMethods.Add(typeof(byte), typeof(BinaryReader).GetMethod("ReadByte"));
            ReadMethods.Add(typeof(System.SByte), typeof(BinaryReader).GetMethod("ReadSByte"));
            ReadMethods.Add(typeof(System.Single), typeof(BinaryReader).GetMethod("ReadSingle"));
            ReadMethods.Add(typeof(byte[]), typeof(BinaryReader).GetMethod("ReadBytes"));
            ReadMethods.Add(typeof(char[]), typeof(BinaryReader).GetMethod("ReadChars"));
            ReadMethods.Add(typeof(System.Int16), typeof(BinaryReader).GetMethod("ReadInt16"));
            ReadMethods.Add(typeof(System.Int32), typeof(BinaryReader).GetMethod("ReadInt32"));
            ReadMethods.Add(typeof(System.UInt16), typeof(BinaryReader).GetMethod("ReadUInt16"));
            ReadMethods.Add(typeof(System.UInt32), typeof(BinaryReader).GetMethod("ReadUInt32"));
            ReadMethods.Add(typeof(System.String), typeof(MarshallingMethods).GetMethod("ReadString"));
            ReadMethods.Add(typeof(System.DateTime), typeof(MarshallingMethods).GetMethod("ReadDateTime"));
            ReadMethods.Add(typeof(System.Int16[]), typeof(MarshallingMethods).GetMethod("ReadInt16Array"));
            ReadMethods.Add(typeof(System.Int32[]), typeof(MarshallingMethods).GetMethod("ReadInt32Array"));
            ReadMethods.Add(typeof(System.UInt16[]), typeof(MarshallingMethods).GetMethod("ReadUInt16Array"));
            ReadMethods.Add(typeof(System.UInt32[]), typeof(MarshallingMethods).GetMethod("ReadUInt32Array"));
            ReadMethods.Add(typeof(CustomMarshaler), typeof(CustomMarshaler).GetMethod("ReadFromStream"));
            //Write Methods
            WriteMethods.Add(typeof(bool), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(bool)}));
            WriteMethods.Add(typeof(byte), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(byte)}));
            WriteMethods.Add(typeof(System.SByte), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(System.SByte)}));
            WriteMethods.Add(typeof(System.Single), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(System.Single)}));
            //WriteMethods.Add(typeof(byte[]), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(byte[])}));
            //WriteMethods.Add(typeof(char[]), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(char[])}));
            WriteMethods.Add(typeof(System.Int16), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(System.Int16)}));
            WriteMethods.Add(typeof(System.Int32), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(System.Int32)}));
            WriteMethods.Add(typeof(System.UInt16), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(System.UInt16)}));
            WriteMethods.Add(typeof(System.UInt32), typeof(BinaryWriter).GetMethod("Write", new Type[]{typeof(System.UInt32)}));
            WriteMethods.Add(typeof(System.String), typeof(MarshallingMethods).GetMethod("WriteString"));
            WriteMethods.Add(typeof(CustomMarshaler), typeof(CustomMarshaler).GetMethod("WriteToStream"));

            WriteMethods.Add(typeof(bool[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(bool[]) }));
            WriteMethods.Add(typeof(char[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(char[]) }));
            WriteMethods.Add(typeof(short[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(short[]) }));
            WriteMethods.Add(typeof(ushort[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(ushort[]) }));
            WriteMethods.Add(typeof(int[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(int[]) }));
            WriteMethods.Add(typeof(uint[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(uint[]) }));
            WriteMethods.Add(typeof(long[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(long[]) }));
            WriteMethods.Add(typeof(ulong[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(ulong[]) }));
            WriteMethods.Add(typeof(float[]), typeof(MarshallingMethods).GetMethod("WriteArray", new Type[] { typeof(BinaryWriter), typeof(float[]) }));

        }

        #endregion

        #region static helper methods

        public static short[] ReadInt16Array(BinaryReader reader, int count)
        {
            short[] result = new short[count];

            for(int i=0;i<count;i++)
            {
                result[i] = reader.ReadInt16();
            }
            return result;
        }

        public static int[] ReadInt32Array(BinaryReader reader, int count)
        {
            int[] result = new int[count];

            for(int i=0;i<count;i++)
            {
                result[i] = reader.ReadInt32();
            }
            return result;
        }

        public static ushort[] ReadUInt16Array(BinaryReader reader, int count)
        {
            ushort[] result = new ushort[count];

            for(int i=0;i<count;i++)
            {
                result[i] = reader.ReadUInt16();
            }
            return result;
        }

        public static uint[] ReadUInt32Array(BinaryReader reader, int count)
        {
            uint[] result = new uint[count];

            for(int i=0;i<count;i++)
            {
                result[i] = reader.ReadUInt32();
            }
            return result;
        }

        public static string ReadString(BinaryReader reader, int count)
        {
            string result = "";
            if (count == 0)
            {
                count = 255; //default    
            }
            char[] data = reader.ReadChars(count);

            result = new string(data).TrimEnd('\0');
            return result;
        }

        public static void WriteString(BinaryWriter writer, string value, int size)
        {
            if (value!=null)
            {
                byte[] bstring = System.Text.Encoding.Unicode.GetBytes(value.Substring(0, size)); 
                writer.Write(bstring);
            }
        }

        public static DateTime ReadDateTime(BinaryReader reader)
        {
            return DateTime.FromFileTime(reader.ReadInt64());
        }


        public static void WriteArray(BinaryWriter writer, bool[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }
        public static void WriteArray(BinaryWriter writer, char[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }

        public static void WriteArray(BinaryWriter writer, byte[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }

        public static void WriteArray(BinaryWriter writer, short[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }
        public static void WriteArray(BinaryWriter writer, ushort[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }
        public static void WriteArray(BinaryWriter writer, int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }
        public static void WriteArray(BinaryWriter writer, uint[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }
        public static void WriteArray(BinaryWriter writer, long[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }
        public static void WriteArray(BinaryWriter writer, ulong[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }
        public static void WriteArray(BinaryWriter writer, float[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i]);
            }
        }

        public static void WriteSerializers(BinaryWriter writer, CustomMarshaler[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].WriteToStream(writer);
            }
        }

        #endregion
    }

    #endregion

    #region CustomMarshalAsAttribute
    /// <summary>
    /// CustomMarshalAsAttribute implementaion.
    /// </summary>
    public sealed class CustomMarshalAsAttribute : Attribute
    {
        public int SizeConst = 0;
        public string  SizeField = null;
    }

    #endregion
}