namespace FeedR.Shared.Serialization;

public interface ISerializer
{
    string Serialize<T>(T value) where T : class;
    T? Deserialize<T>(string value) where T : class;
    byte[] SerializeToBytes<T>(T value) where T : class;
    T? DeserializeBytes<T>(byte[] value) where T : class;
    T? DeserializeBytes<T>(ReadOnlySpan<byte> value) where T : class;
}