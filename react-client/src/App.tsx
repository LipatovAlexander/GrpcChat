import React, {useState} from 'react';
import './App.css';
import useChat from "./hooks/useChat";
import {GetMessageResponse} from "./generated/chat";

function App() {
  const chat = useChat();
  const [name, setName] = useState("");
  const [message, setMessage] = useState("");
  const [token, setToken] = useState("");
  const [messages, setMessages] = useState<GetMessageResponse[]>([]);
  
  const authorize = async () => {
    const response = await chat.join({user: name});
    setToken(response.response.token);
    
    const messageStream = chat.receiveMessages({}, {meta: {Authorization: `Bearer ${response.response.token}`}})
    for await (let message of messageStream.responses) {
      setMessages(m => [...m, message])
    }
  }
  
  const sendMessage = async () => {
    await chat.sendMessage({text: message}, {meta: {Authorization: `Bearer ${token}`}})
    setMessage("");
  }

  return (
    <div className="App">
      <span>Nickname: </span>
      <input
        name="name"
        onChange={(e) => setName(e.target.value)}
        value={name}
      />
      <button onClick={authorize}>Authorize</button>
      <span> Token: {token}</span>
      <br/>
      <ul>
        {messages.map((m, i) => <li key={i}>{m.user}: {m.text}</li>)}
      </ul>
      <br/>
      <span>Message: </span>
      <input
        name="message"
        onChange={(e) => setMessage(e.target.value)}
        value={message}
      />
      <button onClick={sendMessage}>Send</button>
    </div>
  );
}

export default App;