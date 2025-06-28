namespace Catalogue.Shared.Interfaces;

public interface IDataReader<out T>
{
    IEnumerable<T> GetData();
}