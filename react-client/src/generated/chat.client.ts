// @generated by protobuf-ts 2.8.2 with parameter long_type_string
// @generated from protobuf file "chat.proto" (package "Chat", syntax proto3)
// tslint:disable
import type { RpcTransport } from "@protobuf-ts/runtime-rpc";
import type { ServiceInfo } from "@protobuf-ts/runtime-rpc";
import { ChatRoomService } from "./chat";
import type { GetMessageResponse } from "./chat";
import type { ServerStreamingCall } from "@protobuf-ts/runtime-rpc";
import type { Empty } from "./chat";
import type { SendMessageRequest } from "./chat";
import { stackIntercept } from "@protobuf-ts/runtime-rpc";
import type { JoinResponse } from "./chat";
import type { JoinRequest } from "./chat";
import type { UnaryCall } from "@protobuf-ts/runtime-rpc";
import type { RpcOptions } from "@protobuf-ts/runtime-rpc";
/**
 * @generated from protobuf service Chat.ChatRoomService
 */
export interface IChatRoomServiceClient {
    /**
     * @generated from protobuf rpc: Join(Chat.JoinRequest) returns (Chat.JoinResponse);
     */
    join(input: JoinRequest, options?: RpcOptions): UnaryCall<JoinRequest, JoinResponse>;
    /**
     * @generated from protobuf rpc: SendMessage(Chat.SendMessageRequest) returns (Chat.Empty);
     */
    sendMessage(input: SendMessageRequest, options?: RpcOptions): UnaryCall<SendMessageRequest, Empty>;
    /**
     * @generated from protobuf rpc: ReceiveMessages(Chat.Empty) returns (stream Chat.GetMessageResponse);
     */
    receiveMessages(input: Empty, options?: RpcOptions): ServerStreamingCall<Empty, GetMessageResponse>;
}
/**
 * @generated from protobuf service Chat.ChatRoomService
 */
export class ChatRoomServiceClient implements IChatRoomServiceClient, ServiceInfo {
    typeName = ChatRoomService.typeName;
    methods = ChatRoomService.methods;
    options = ChatRoomService.options;
    constructor(private readonly _transport: RpcTransport) {
    }
    /**
     * @generated from protobuf rpc: Join(Chat.JoinRequest) returns (Chat.JoinResponse);
     */
    join(input: JoinRequest, options?: RpcOptions): UnaryCall<JoinRequest, JoinResponse> {
        const method = this.methods[0], opt = this._transport.mergeOptions(options);
        return stackIntercept<JoinRequest, JoinResponse>("unary", this._transport, method, opt, input);
    }
    /**
     * @generated from protobuf rpc: SendMessage(Chat.SendMessageRequest) returns (Chat.Empty);
     */
    sendMessage(input: SendMessageRequest, options?: RpcOptions): UnaryCall<SendMessageRequest, Empty> {
        const method = this.methods[1], opt = this._transport.mergeOptions(options);
        return stackIntercept<SendMessageRequest, Empty>("unary", this._transport, method, opt, input);
    }
    /**
     * @generated from protobuf rpc: ReceiveMessages(Chat.Empty) returns (stream Chat.GetMessageResponse);
     */
    receiveMessages(input: Empty, options?: RpcOptions): ServerStreamingCall<Empty, GetMessageResponse> {
        const method = this.methods[2], opt = this._transport.mergeOptions(options);
        return stackIntercept<Empty, GetMessageResponse>("serverStreaming", this._transport, method, opt, input);
    }
}
