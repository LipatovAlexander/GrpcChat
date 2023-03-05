import {GrpcWebFetchTransport} from "@protobuf-ts/grpcweb-transport";
import {ChatRoomServiceClient} from "../generated/chat.client";
import {useEffect, useState} from "react";


function useChat() {
  const [client, setClient] = useState<ChatRoomServiceClient>();
  
  useEffect(() => {
    const transport = new GrpcWebFetchTransport({
      baseUrl: "https://localhost:5000"
    });

    const c = new ChatRoomServiceClient(transport);
    setClient(c);
  }, [])
  
  return client!;
}

export default useChat;