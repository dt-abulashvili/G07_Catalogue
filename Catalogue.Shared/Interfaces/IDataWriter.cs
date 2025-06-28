namespace Catalogue.Shared.Interfaces;

public interface IDataWriter<in T>
{
    void WriteData(IEnumerable<T> data);
}