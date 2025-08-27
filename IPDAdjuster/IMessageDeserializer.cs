using Renderite.Shared;

namespace IPDAdjuster;

public interface IMessageDeserializer 
{
	public void Deserialize( string messageID, MemoryUnpacker unpacker );	
}
