namespace WePrepClass.Application.Interfaces;

public interface IBlobStorageServices
{
    string GetImage(string fileName);
    string UploadImage(string fileName, Stream stream);
}