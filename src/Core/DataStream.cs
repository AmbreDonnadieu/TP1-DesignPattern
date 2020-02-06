using System;
using System.Linq;
using System.Text;



//Regroupe les interfaces et classes nécessaires pour faire un decorateur
//On utilise le décorateur pour chiffrer et compresser les données 
//On l'appelle dans le job pour protéger les données à l'execution
internal interface IDataStream {
    void WriteData(byte[] data);

    byte[] ReadData();
}

internal class BufferDataStream: IDataStream
{
    private byte[] Buffer;

    public BufferDataStream()
    {
    }

    public void WriteData(byte[] data)
    {
        Buffer = data;
    }

    public byte[] ReadData()
    {
        return Buffer;
    }
}

internal abstract class DataStreamDecorator: IDataStream
{
    protected IDataStream Decorable;

    public DataStreamDecorator(IDataStream decorable)
    {
        Decorable = decorable;
    }

    public abstract void WriteData(byte[] data);

    public abstract byte[] ReadData();
}

internal class Compression: DataStreamDecorator
{
    public Compression(IDataStream decorable): base(decorable)
    {
    }

    public override void WriteData(byte[] data)
    {
        Decorable.WriteData(Encoding.UTF8.GetBytes("Compressed:").Concat(data).ToArray());
    }

    public override byte[] ReadData()
    {
        byte[] data = Decorable.ReadData();
        data.Skip("Compressed:".Length).ToArray();
        return data.Skip("Compressed:".Length).ToArray();
    }
}

internal class Encryption: DataStreamDecorator
{
    public Encryption(IDataStream decorable): base(decorable)
    {
    }

    public override void WriteData(byte[] data)
    {
        Decorable.WriteData(Encoding.UTF8.GetBytes("Encrypted:").Concat(data).ToArray());
    }

    public override byte[] ReadData()
    {
        byte[] data = Decorable.ReadData();
        data.Skip("Encrypted:".Length).ToArray();
        return data.Skip("Encrypted:".Length).ToArray();
    }
}

